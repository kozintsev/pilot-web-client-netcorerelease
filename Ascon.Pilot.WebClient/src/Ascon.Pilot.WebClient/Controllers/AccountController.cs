using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Ascon.Pilot.Core;
using Ascon.Pilot.WebClient.Extensions;
using Ascon.Pilot.WebClient.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Authentication;
using Ascon.Pilot.WebClient.Models;

namespace Ascon.Pilot.WebClient.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<FilesController> _logger;
        private IContextHolder _contextHolder;

        public AccountController(ILogger<FilesController> logger, IContextHolder contextHolder)
        {
            _logger = logger;
            _contextHolder = contextHolder;
        }

        [AllowAnonymous]
        public IActionResult LogIn(string returnUrl = null)
        {
            var logInViewModel = new LogInViewModel();

            ViewData["ReturnUrl"] = returnUrl;

#if (DEBUG)
                //DatabaseName = "3d-storage_ru",
                //Login = "admin",
                //Password = "123456"
                logInViewModel.DatabaseName = "pilot-ice_ru";
                logInViewModel.Login = "pavlenko";
                logInViewModel.Password = "123456";
    #endif

            return View(logInViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn(LogInViewModel model)
        {
            if (!ModelState.IsValid)
                return View("LogIn");

            var clientId = Guid.NewGuid();
            var context = _contextHolder.NewContext(clientId);
            try
            {
                var creds = Credentials.GetConnectionCredentials(model.DatabaseName, model.Login, model.Password);
                var dbInfo = context.Connect(creds);
                if (dbInfo == null)
                {
                    ModelState.AddModelError("", "Авторизация не удалась, проверьте данные и повторите вход");
                    return View("LogIn", model);
                }

                await SignInAsync(dbInfo, creds.DatabaseName, creds.ProtectedPassword, clientId, model.RememberMe);

                //var dMetadata = context.ServerApi.GetMetadata(dbInfo.MetadataVersion);
                //Debug.WriteLine(dMetadata.Types.ToString());
                //Debug.WriteLine(dMetadata.Version.ToString());
                //HttpContext.Session.SetSessionValues(SessionKeys.MetaTypes, dMetadata.Types.ToDictionary(x => x.Id, y => y));
            }
            catch (Exception ex)
            {
                _logger.LogError(1, "Не удалось подключиться к серверу", ex);
                ModelState.AddModelError("", ex.Message);
                return View();
            }

            return RedirectToAction("Index", "Files");
        }
        
        private async Task SignInAsync(DDatabaseInfo dbInfo, string dbName, string protectedPassword, Guid clientId, bool isPersistent)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Surname, dbName),
                new Claim(ClaimTypes.Name, dbInfo.Person.Login),
                new Claim(ClaimTypes.GivenName, dbInfo.Person.DisplayName),
                new Claim(ClaimTypes.Role, dbInfo.Person.IsAdmin ? Roles.Admin : Roles.User),
                new Claim(ClaimTypes.UserData, protectedPassword),
                new Claim(ClaimTypes.Sid, clientId.ToString())
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, ApplicationConst.PilotMiddlewareInstanceName));
            
            await HttpContext.Authentication.SignInAsync(ApplicationConst.PilotMiddlewareInstanceName, principal, new AuthenticationProperties { IsPersistent = isPersistent });
        }
        
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.Authentication.SignOutAsync(ApplicationConst.PilotMiddlewareInstanceName);
            _contextHolder.Dispose();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
