using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.SqlServer
{
    public class SqlServerContext : Context
    {
        public SqlServerContext(IConfigurationRoot config) : base(config) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(config["ConnectionStrings:DefaultConnection"]);
        }
    }
}
