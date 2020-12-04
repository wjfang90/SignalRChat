using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.SignalRChat.Models;
using Chat.Business;
using Chat.Models;
using Microsoft.AspNetCore.Http;

namespace Chat.SignalRChat.Hubs
{
    public class ChatHubT : Hub<IHubClient>
    {
        ILogger _logger = null;
        private DataBusiness _dataBusiness;
        public ChatHubT(ILogger<ChatHubT> logger, DataBusiness dataBusiness)
        {
            _logger = logger;
            _dataBusiness = dataBusiness;
        }

        public async Task SendToAll(MessageInfo message)
        {
            await Clients.All.ReceiveMessage(message);
        }
        public async Task SendToOhters(MessageInfo message)
        {
            await Clients.Others.ReceiveMessage(message);
        }

        public async Task SendToUser(MessageInfo message, string userId)
        {
            var user =  await _dataBusiness.GetUser(userId);
            var connections = user.Connections.Select(t => t.ConnectionId).ToArray();
            await SendToConnection(message,connections);
        }

        public async Task SendToUsers(MessageInfo message, params string[] userIds)
        {
            await Clients.Users(userIds).ReceiveMessage(message);
        }

        public async Task SendToGroup(MessageInfo message, string groupName)
        {
            await Clients.Group(groupName).ReceiveMessage(message);
        }

        public async Task SendToGroups(MessageInfo message, params string[] groupNames)
        {
            await Clients.Groups(groupNames).ReceiveMessage(message);
        }

        public async Task SendToConnection(MessageInfo message, params string[] connnectionIds)
        {
            await Clients.Clients(connnectionIds).ReceiveMessage(message);
        }

        public async Task JoinGroup(string groupName, string userName)
        {
            var connectionId = this.Context.ConnectionId;
            await _dataBusiness.JoinGroup(groupName, userName);

            var message = new MessageInfo
            {
                Sender = userName,
                Message = $"{userName} joined {groupName} connectionId={connectionId}",
                MessageType = MessageType.SystemMessage,
                SystemMessageType = SystemMessageType.JoinGroup
            };

            await Clients.Group(groupName).OnNotice(message);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            _logger.LogInformation(message.Message);
        }

        public async Task LeaveGroup(string groupName, string userName)
        {
            var connectionId = this.Context.ConnectionId;
            await _dataBusiness.LeaveGroup(groupName, userName);

            var message = new MessageInfo
            {
                Sender = userName,
                Message = $"{userName} left {groupName} connectionId={connectionId}",
                MessageType = MessageType.SystemMessage,
                SystemMessageType = SystemMessageType.LeftGroup
            };

            await Clients.Group(groupName).OnNotice(message);

            await Groups.RemoveFromGroupAsync(connectionId, groupName);

            _logger.LogInformation(message.Message);
        }

        public override async Task OnConnectedAsync()
        {
            var connectionId = this.Context.ConnectionId;
            var userName = Context.GetHttpContext().Request.Query["userName"];

            var connection = new Chat.Models.ConnectionInfo()
            {
                UserName = userName,
                ConnectionId = connectionId
            };

            var user = new UserInfo()
            {
                UserName = userName,
                Connections = new List<Chat.Models.ConnectionInfo>() { connection }
            };

            await _dataBusiness.CreateConnection(connection);

            if (!await _dataBusiness.IsExistsUser(userName))
            {
                await _dataBusiness.CreateUser(user);
            }

            var message = new MessageInfo
            {
                Sender = userName,
                Message = $"{userName} Connected  connectiondId={Context.ConnectionId}",
                MessageType = MessageType.SystemMessage,
                SystemMessageType = SystemMessageType.Connect
            };

            await Clients.All.OnNotice(message);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userName = Context.GetHttpContext().Request.Query["userName"];
            var connectionId = Context.ConnectionId;

            var message = new MessageInfo
            {
                Sender = userName,
                Message = $"{userName} Disconnected connectiondId={connectionId}",
                MessageType = MessageType.SystemMessage,
                SystemMessageType = SystemMessageType.Distconnect
            };
            await Clients.All.OnNotice(message);

            await _dataBusiness.DeleteConnection(connectionId);
            await _dataBusiness.DeleteUser(userName);

            await base.OnDisconnectedAsync(exception);
        }
    }

    public interface IHubClient
    {
        Task ReceiveMessage(MessageInfo messageInfo);

        Task OnNotice(MessageInfo messageInfo);
    }
}
