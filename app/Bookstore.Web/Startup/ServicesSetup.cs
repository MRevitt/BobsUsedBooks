using Amazon.Rekognition;
using Amazon.S3;
using Amazon.SecretsManager.Model;
using Amazon.SecretsManager;
using Bookstore.Data;
using Bookstore.Domain.AdminUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Npgsql;


namespace Bookstore.Web.Startup
{
    public static class ServicesSetup
    {
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllersWithViews(x =>
            {
                x.Filters.Add(new AuthorizeFilter());
                x.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
            builder.Services.AddAWSService<IAmazonS3>();
            builder.Services.AddAWSService<IAmazonRekognition>();

            var connString = GetDatabaseConnectionString(builder.Configuration);
            builder.Services.AddDbContext<ApplicationDbContext>(options => 
            {
                options.UseNpgsql(connString);
                options.EnableServiceProviderCaching(false);
            });
            builder.Services.AddSession();

            return builder;
        }

        private static string GetDatabaseConnectionString(ConfigurationManager configuration)
        {
            const string DbSecretsParameterName = "dbsecretsname";

            var connString = configuration.GetConnectionString("BookstoreDbDefaultConnection");
            if (!string.IsNullOrEmpty(connString))
            {
                // Replace environment variable placeholder with actual value
                if (connString.Contains("{DB_PASSWORD}"))
                {
                    var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "YourPassword123!";
                    connString = connString.Replace("{DB_PASSWORD}", dbPassword);
                    Console.WriteLine("Using bookstore-db connection string");
                    return connString;
                }
                
                Console.WriteLine("Using bookstore-db connection string");
                return connString;
            }

            try
            {
                var dbSecretId = configuration[DbSecretsParameterName];
                Console.WriteLine($"Reading db credentials from secret {dbSecretId}");

                IAmazonSecretsManager secretsManagerClient;
                var options = configuration.GetAWSOptions();
                if (options != null)
                {
                    secretsManagerClient = new AmazonSecretsManagerClient();
                }
                else
                {
                    secretsManagerClient = new AmazonSecretsManagerClient();
                }
                var response = secretsManagerClient.GetSecretValueAsync(new GetSecretValueRequest
                {
                    SecretId = dbSecretId
                }).Result;

                var dbSecrets = JsonSerializer.Deserialize<DbSecrets>(response.SecretString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var partialConnString = $"Host={dbSecrets.Host};Port={dbSecrets.Port};Database=postgres";

                var builder = new NpgsqlConnectionStringBuilder(partialConnString)
                {
                    Username = dbSecrets.Username,
                    Password = dbSecrets.Password
                };

                connString = builder.ConnectionString;
            }
            catch (AmazonSecretsManagerException e)
            {
                Console.WriteLine($"Failed to read secret {configuration[DbSecretsParameterName]}, error {e.Message}, inner {e.InnerException.Message}");
            }
            catch (JsonException e)
            {
                Console.WriteLine($"Failed to parse content for secret {configuration[DbSecretsParameterName]}, error {e.Message}");
            }

            return connString;
        }
    }
}