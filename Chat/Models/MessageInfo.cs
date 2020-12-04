using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.SignalRChat.Models
{
    public class MessageInfo
    {
        public string Sender { get; set; }
        public string Message { get; set; }
        public DateTime SendTime => DateTime.Now;

        public MessageType MessageType { get; set; }

        public SystemMessageType SystemMessageType { get; set; }

    }

    public enum MessageType
    {
        SystemMessage = 0,
        UserMessage = 1
    }

    public enum SystemMessageType
    {
        Connect = 0,
        Distconnect = 1,
        JoinGroup = 2,
        LeftGroup = 3
    }
}
