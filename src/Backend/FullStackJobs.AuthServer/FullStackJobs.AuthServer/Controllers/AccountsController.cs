﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity; 
using FullStackJobs.AuthServer.Models;
using FullStackJobs.AuthServer.Infrastructure.Data.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FullStackJobs.AuthServer.Controllers
{
    public class AccountsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppIdentityDbContext _appIdentityDbContext;

        public AccountsController(UserManager<AppUser> userManager, AppIdentityDbContext appIdentityDbContext)
        {
            _userManager = userManager;
            _appIdentityDbContext = appIdentityDbContext;
        }

        [Route("api/[controller]")]
        public async Task<IActionResult> Post([FromBody]SignupRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new AppUser { UserName = model.Email, FullName = model.FullName, Email = model.Email };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("userName", user.UserName));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("name", user.FullName));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("role", model.Role));

            // ToDo: We'll resotre this later when signup view and employer/applicant entities are implemented 
            // Insert in entity table
            // var commandText = $"INSERT {model.Role + "s"} (Id,Created,FullName) VALUES (@Id,getutcdate(),@FullName)";
            // var id = new SqlParameter("@Id", user.Id);
            // var name = new SqlParameter("@FullName", user.FullName);
            // await _appIdentityDbContext.Database.ExecuteSqlRawAsync(commandText, id, name);

            return Ok(new SignupResponse(user, model.Role));
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
           /* var vm = await BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("Challenge", "External", new { provider = vm.ExternalLoginScheme, returnUrl });
            }*/

            return View();
        }
    }
}
