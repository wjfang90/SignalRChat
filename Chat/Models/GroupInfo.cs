using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Models
{
    public class GroupInfo
    {
        public string GroupName { get; set; }
        public List<UserInfo> Users { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class UserInfo
    {
        public string UserName { get; set; }

        /// <summary>
        /// 一个User 只能在一个group中
        /// </summary>
        public string GroupName { get; set; }
        public List<ConnectionInfo> Connections { get; set; }
    }

    public class ConnectionInfo
    {
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
    }
}
