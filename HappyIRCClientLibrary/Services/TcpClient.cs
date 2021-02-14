/*
MIT License

Copyright(c) 2021 Kyle Givler
https://github.com/JoyfulReaper

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

// Heavily based off https://github.com/ygoe/AsyncTcpClient/blob/master/AsyncTcpClient/AsyncTcpClient.cs
// Copyright (c) 2018-2020, Yves Goergen, https://unclassified.software
//
// Copying and distribution of this file, with or without modification, are permitted provided the
// copyright notice and this notice are preserved. This file is offered as-is, without any warranty.

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HappyIRCClientLibrary.Services
{
    public class TcpClient : IDisposable, ITcpClient
    {
        private readonly ILogger<TcpClient> log;
        private System.Net.Sockets.TcpClient tcpClient;
        private NetworkStream stream;
        private TaskCompletionSource<bool> closedTcs = new TaskCompletionSource<bool>();

        public TcpClient(ILogger<TcpClient> log,
            IServer server)
        {
            closedTcs.SetResult(true);
            this.log = log;
            this.Server = server;
        }

        public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(5);
        public IServer Server { get; private set; }
        public bool IsConnected => tcpClient.Client.Connected;

        /// <summary>
        /// A Task that can be awaited to close the connection. This task will
        /// complete when the connection was closed remotely.
        /// </summary>
        public Task ClosedTask => closedTcs.Task;

        /// <summary>
        /// Gets a value indicating whether the ClosedTask has completed.
        /// </summary>
        public bool IsClosing => ClosedTask.IsCompleted;

        /// <summary>
        /// Called when the client has connected to the remote host. This method can implement the
        /// communication logic to execute when the connection was established. The connection will
        /// not be closed before this method completes.
        /// </summary>
        public event Func<ITcpClient, Task> ConnectedCallback;

        /// <summary>
        /// Called when the connection was closed. The parameter specifies whether the connection
        /// was closed by the remote host.
        /// </summary>
        public event Action<ITcpClient, bool> ClosedCallback;

        /// <summary>
        /// Called when data was received from the remote host. The parameter specifies the number
        /// of message added to the queue. This method can implement the communication logic to
        /// execute every time data was received. New data will not be received before this method
        /// completes.
        /// </summary>
        public event Func<ITcpClient, int, Task> ReceivedCallback;

        public Queue<string> MessageQueue { get; set; } = new Queue<string>();

        public async Task RunAsync()
        {
            // Could add reconnection logic here
            //do
            //{
            tcpClient = new System.Net.Sockets.TcpClient(AddressFamily.InterNetworkV6);
            tcpClient.Client.DualMode = true;

            log.LogInformation("Connecting to server: {server}:{port}", Server.ServerAddress, Server.Port);

            Task connectTask;
            connectTask = tcpClient.ConnectAsync(Server.ServerAddress, Server.Port);

            var timeoutTask = Task.Delay(ConnectTimeout);
            if (await Task.WhenAny(connectTask, timeoutTask) == timeoutTask)
            {
                log.LogError("Connection timeout");

                closedTcs.TrySetResult(true);
                OnClosed(true);
                return;
            }

            try
            {
                await connectTask;
            }
            catch (Exception e)
            {
                log.LogError("Exception: {exception}", e.Message);
                closedTcs.TrySetResult(true);
                OnClosed(true);
                return;
            }

            stream = tcpClient.GetStream();

            // Read until the connection is closed.
            // A closed connection can only be detected while reading, so we need to read
            // permanently, not only when we might use received data.
            var networkReadTask = Task.Run(async () =>
            {
                byte[] buffer = new byte[10240];
                while (true)
                {
                    int readLength;
                    try
                    {
                        readLength = await stream.ReadAsync(buffer, 0, buffer.Length);
                    }
                    catch (IOException ex) when ((ex.InnerException as SocketException)?.ErrorCode == (int)SocketError.OperationAborted ||
                    (ex.InnerException as SocketException)?.ErrorCode == 125 /* Operation canceled (Linux) */)
                    {
                        // Warning: This error code number (995) may change.
                        // See https://docs.microsoft.com/en-us/windows/desktop/winsock/windows-sockets-error-codes-2
                        // Note: NativeErrorCode and ErrorCode 125 observed on Linux.
                        log.LogInformation("Connection closed locally: {exception}", ex.Message);
                        readLength = -1;
                    }
                    catch (IOException ex) when ((ex.InnerException as SocketException)?.ErrorCode == (int)SocketError.ConnectionAborted)
                    {
                        log.LogWarning("Connection aborted: {exception}", ex.Message);
                        readLength = -1;
                    }
                    catch (IOException ex) when ((ex.InnerException as SocketException)?.ErrorCode == (int)SocketError.ConnectionReset)
                    {
                        log.LogWarning("Connection reset remotely {exception}", ex.Message);
                        readLength = -2;
                    }
                    if (readLength <= 0)
                    {
                        if (readLength == 0)
                        {
                            log.LogWarning("Connection closed remotely");
                        }
                        closedTcs.TrySetResult(true);
                        OnClosed(readLength != -1);
                        return;
                    }
                    var message = Encoding.ASCII.GetString(buffer, 0, readLength); // Convert from bytes to ASCII
                    var splitMessages = message.Split('\n', StringSplitOptions.RemoveEmptyEntries); // The sever may have given us more than a single line, split on newline
                    int messages = 0;
                    foreach (var currentMessage in splitMessages)
                    {
                        messages++;
                        MessageQueue.Enqueue(currentMessage);
                    }
                    await OnReceivedAsync(messages);
                }
            });

            closedTcs = new TaskCompletionSource<bool>();
            await OnConnectedAsync();

            //} while (//reconnect logic);
        }

        public void Disconnect()
        {
            log.LogInformation("Disconnecting from: {server}:{port}", Server.ServerAddress, Server.Port);
            tcpClient.Client.Disconnect(false);
        }

        public void Dispose()
        {
            tcpClient?.Dispose();
            stream = null;
        }

        public async Task Send(string message, CancellationToken cancellationToken = default)
        {
            if (!tcpClient.Client.Connected)
            {
                throw new InvalidOperationException("Not Connected.");
            }

            byte[] writeBuffer = Encoding.ASCII.GetBytes(message);
            log.LogDebug("Sending: {message}", message.Replace("\r", "").Replace("\n", ""));

            await stream.WriteAsync(writeBuffer, 0, writeBuffer.Length, cancellationToken);
        }

        /// <summary>
        /// Called when the client has connected to the remote host. This method can implement the
        /// communication logic to execute when the connection was established. The connection will
        /// not be closed before this method completes.
        /// </summary>
        private Task OnConnectedAsync()
        {
            if (ConnectedCallback != null)
            {
                return ConnectedCallback(this);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when the connection was closed.
        /// </summary>
        private void OnClosed(bool remote)
        {
            ClosedCallback?.Invoke(this, remote);
        }

        /// <summary>
        /// Called when data was received from the remote host. This method can implement the
        /// communication logic to execute every time data was received. New data will not be
        /// received before this method completes.
        /// </summary>
        /// <param name="count">The number of messages added to the queue</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        private async Task OnReceivedAsync(int count)
        {
            if (ReceivedCallback != null)
            {
                await ReceivedCallback(this, count);
            }
            //return Task.CompletedTask;
        }
    }
}