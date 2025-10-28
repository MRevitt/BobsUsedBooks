using Bookstore.Domain.Customers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bookstore.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        private IWebHostEnvironment webHostEnvironment;
        private IConfiguration configuration;

        public AuthenticationController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.configuration = configuration;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(string redirectUri = null, string dbPassword = null)
        {
            // Set DB password if provided
            if (!string.IsNullOrEmpty(dbPassword))
            {
                var connectionString = configuration.GetConnectionString("BookstoreDbDefaultConnection");
                var updatedConnectionString = connectionString.Replace("%DB_PASSWORD%", dbPassword);
                configuration["ConnectionStrings:BookstoreDbDefaultConnection"] = updatedConnectionString;
            }
            
            // Create fake user identity
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, "Mike Revitt"));
            identity.AddClaim(new Claim("sub", "FB6135C7-1464-4A72-B74E-4B63D343DD09"));
            identity.AddClaim(new Claim("given_name", "Bookstore"));
            identity.AddClaim(new Claim("family_name", "User"));
            identity.AddClaim(new Claim(ClaimTypes.Role, "Administrators"));

            // Sign in the user
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            
            // Save customer details
            try {
                var customerService = HttpContext.RequestServices.GetService<ICustomerService>();
                if (customerService != null)
                {
                    var dto = new CreateOrUpdateCustomerDto("FB6135C7-1464-4A72-B74E-4B63D343DD09", "bookstoreuser", "Bookstore", "User");
                    await customerService.CreateOrUpdateCustomerAsync(dto);
                }
            } catch { }
            
            return LocalRedirect(redirectUri ?? "/");
        }

        public async Task<IActionResult> LogOut()
        {
            return webHostEnvironment.IsDevelopment() ? await LocalSignOut() : CognitoSignOut();
        }

        private async Task<IActionResult> LocalSignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        private IActionResult CognitoSignOut()
        {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
