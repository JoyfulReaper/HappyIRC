using HappyIRCClientLibrary.Models;

namespace HappyIRCClientLibrary.Factories
{
    public interface IChannelFactory
    {
        Channel Channel(string name, string key);
    }
}