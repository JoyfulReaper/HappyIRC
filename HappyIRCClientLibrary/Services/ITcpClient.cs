﻿/*
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

using HappyIRCClientLibrary.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HappyIRCClientLibrary.Services
{
    public interface ITcpClient
    {
        event Action<ITcpClient, bool> ClosedCallback;
        Task ClosedTask { get; }
        event Func<ITcpClient, Task> ConnectedCallback;
        TimeSpan ConnectTimeout { get; set; }
        bool IsClosing { get; }
        bool IsConnected { get; }
        Queue<string> MessageQueue { get; set; }
        event Func<ITcpClient, int, Task> ReceivedCallback;
        IServer Server { get; }

        void Disconnect();
        void Dispose();
        Task RunAsync();
        Task Send(string message, CancellationToken cancellationToken = default);
    }
}