using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace Bookstore.Web.Startup
{
    public static class ConfigurationSetup
    {
        public static WebApplicationBuilder ConfigureConfiguration(this WebApplicationBuilder builder)
        {
            if (!builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddSystemsManager("/BobsBookstore/");
            }

            // Replace environment variables in connection strings
            var connectionString = builder.Configuration.GetConnectionString("BookstoreDbDefaultConnection");
            if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains("%DB_PASSWORD%"))
            {
                var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
                if (!string.IsNullOrEmpty(dbPassword))
                {
                    connectionString = connectionString.Replace("%DB_PASSWORD%", dbPassword);
                    builder.Configuration["ConnectionStrings:BookstoreDbDefaultConnection"] = connectionString;
                }
            }

            return builder;
        }
    }
}
