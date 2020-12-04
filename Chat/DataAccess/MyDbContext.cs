using Chat.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.DataAccess
{
    public class MyDbContext : DbContext
    {
        public DbSet<GroupInfo> Groups { get; set; }
        public DbSet<UserInfo> Users { get; set; }

        public DbSet<ConnectionInfo> Connections { get; set; }

        public MyDbContext(DbContextOptions<MyDbContext> option) : base(option)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroupInfo>()
                .HasKey(t => t.GroupName);

            modelBuilder.Entity<UserInfo>()
                .HasKey(t => t.UserName);

            modelBuilder.Entity<ConnectionInfo>()
                .HasKey(t => t.ConnectionId);
        }
    }
}
