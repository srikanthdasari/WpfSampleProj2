using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WpfSampleProj2.Services.Entities;

namespace WpfSampleProj2.Services
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LibraryContext>
    {
        public LibraryContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<LibraryContext>();
            var connectionStr = configuration.GetConnectionString("DBCoreConnection");

            builder.UseSqlServer(connectionStr);

            var context=new LibraryContext(builder.Options);

            context.EnsureSeedDataForContext();
            return context;
        }
    }
}
