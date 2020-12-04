using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Chat.SignalRChat.Models;

namespace Chat.SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        //HubMethodName属性可以指定方法名，默认使用.net方法名
        //[HubMethodName("SendMessageToUser")]
        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}

        //方法重载会有问题，参数用自定义对象类型
        public async Task SendMessage(MessageInfo messageInfo)
        {
            await Clients.All.SendAsync("ReceiveMessage", new { sender = messageInfo.Sender, message = messageInfo.Message });
        }
    }
}
