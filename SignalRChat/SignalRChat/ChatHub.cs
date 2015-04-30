using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignalRChat
{
    public class ChatHub : Hub
    {
        public void Send(String name, String message)
        {
            Clients.All.broadcastMessage(name, message);
        }

        public void UserConnected(String name)
        {
            var encodeName = HttpUtility.HtmlEncode(name);
            var message = " 歡迎使用者 " + encodeName + " 加入聊天室 ";

            Clients.Others.addList(Context.ConnectionId, encodeName);
            Clients.Others.hello(message);

            var userList = UserHandler.ConnectedIds.Select(p => new {id = p.Key, name = p.Value}).ToList();

            Clients.Caller.getList(userList);
            UserHandler.ConnectedIds.Add(Context.ConnectionId, encodeName);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var message = "使用者 " + UserHandler.ConnectedIds.Single(c => c.Key==Context.ConnectionId).Value+" 離開聊天室了";
            Clients.All.removeList(Context.ConnectionId,message);
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public void SendConfirm(string message)
        {
            Clients.All.showConfirm(message);
        }

        public static class UserHandler
        {
            public static Dictionary<string, string> ConnectedIds = new Dictionary<string, string>();
        }


    }
}