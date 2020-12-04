using Chat.DataAccess;
using Chat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Business
{
    public class DataBusiness
    {
        private readonly MyDbContext _dbContext;

        public DataBusiness(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<GroupInfo>> InitGroups()
        {

            var groupLists = GetGroups();
            if (groupLists != null && groupLists.Any())
            {
                return groupLists;
            }

            var groups = new List<GroupInfo>() {
                new GroupInfo()
                {
                    GroupName="default",
                    Users=new List<UserInfo>()
                },
                 new GroupInfo()
                {
                    GroupName="testGroup",
                    Users=new List<UserInfo>()
                }
            };

            await CreateGroup(groups.ToArray());

            return groups;
        }



        public async Task JoinGroup(string groupName, string userName)
        {
            var group = await GetGroup(groupName);
            if (group != null)
            {
                if (group.Users == null)
                {
                    group.Users = new List<UserInfo>();
                }

                var isUserExist = group.Users.Any(t => t.UserName.Equals(userName));

                if (!isUserExist)
                {
                    var user = await GetUser(userName);
                    user.GroupName = groupName;
                    await ModifyUser(user);
                }
            }
            else
            {
                group = new GroupInfo()
                {
                    GroupName = groupName
                };

                await CreateGroup(group);

                var user = await GetUser(userName);
                user.GroupName = groupName;
                await ModifyUser(user);
            }
        }

        public async Task LeaveGroup(string groupName, string userName)
        {
            var group = await GetGroup(groupName);
            if (group != null)
            {
                var isUserExist = group.Users.Any(t => t.UserName.Equals(userName));

                if (isUserExist)
                {
                    var user = await GetUser(userName);
                    user.GroupName = null;
                    await ModifyUser(user);
                }
            }
        }

        public List<GroupInfo> GetGroups()
        {
            var users = _dbContext.Users.ToList();
            var groups = _dbContext.Groups.ToList();
            groups.ForEach(t =>
            {
                t.Users = t.Users == null ? new List<UserInfo>() : users.Where(s => s.GroupName == t.GroupName).ToList();
            });

            return groups;
        }


        public async Task<int> CreateGroup(GroupInfo groupInfo)
        {
            await _dbContext.Groups.AddAsync(groupInfo);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CreateGroup(params GroupInfo[] groups)
        {
            await _dbContext.Groups.AddRangeAsync(groups);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<GroupInfo> GetGroup(string groupName)
        {
            var group = await _dbContext.Groups.FindAsync(groupName);
            if (group == null)
            {
                return group;
            }
            var users = _dbContext.Users.Where(t => t.GroupName == groupName).ToList();
            group.Users = users;

            return group;
        }

        public async Task<int> CreateUser(UserInfo userInfo)
        {
            await _dbContext.Users.AddAsync(userInfo);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> ModifyUser(UserInfo userInfo)
        {
            _dbContext.Users.Update(userInfo);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<UserInfo> GetUser(string userName)
        {
            var user = await _dbContext.Users.FindAsync(userName);
            if (user == null)
                return user;

            var connections = _dbContext.Connections.Where(t => t.UserName == userName).ToList();
            user.Connections = connections;
            return user;
        }

        public async Task<bool> IsExistsUser(string userName)
        {
            return await _dbContext.Users.FindAsync(userName) != null;
        }

        public async Task<int> DeleteUser(string userName)
        {
            var user = await _dbContext.Users.FindAsync(userName);
            _dbContext.Users.Remove(user);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CreateConnection(ConnectionInfo connectionInfo)
        {
            await _dbContext.Connections.AddAsync(connectionInfo);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteConnection(string connectionId)
        {
            var connection = await _dbContext.Connections.FindAsync(connectionId);
            _dbContext.Connections.Remove(connection);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
