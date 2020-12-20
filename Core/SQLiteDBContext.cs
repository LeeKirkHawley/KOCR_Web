using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Models;

namespace Core {
    public class SQLiteDBContext : DbContext{
        public DbSet<User> Users { get; set; }
        public DbSet<Document> Documents { get; set; }

        //public SQLiteDBContext(DbContextOptions<SQLiteDBContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=CWDocs.db");
    }
}
