using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TspChatClient
{
    abstract class AbstractClient
    {
        protected static string UserName { get; set; }

        public abstract Task Run();
        protected abstract Task SendMessageAsync(StreamWriter writer);
        protected abstract Task ReceiveMessageAsync(StreamReader reader, StreamWriter writer);
        protected abstract void Print(string message);
    }
}
