﻿using Formix.Helper;
using Formix.Models.DB;
using Formix.Models.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Net;
using System.Security.Claims;

namespace Formix.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly GenerateEmail _generateEmail;

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            GenerateEmail generateEmail)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _generateEmail = generateEmail;
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Signup(SignupViewModel signupView)
        {
            if (!ModelState.IsValid)
            {
                return View(signupView);
            }
            if(await _userManager.FindByEmailAsync(signupView.Email) != null 
                && await _userManager.FindByNameAsync(signupView.Login) != null)
            {
                ModelState.AddModelError("", "This user already exists");
                return View(signupView);
            }
            
            TempData["ConfirmationCode"] = await _generateEmail.CodeAsync(signupView.Email);
            TempData["Action"] = "Signup";
            var confirmationEmailView = new ConfirmationEmailViewModel
            {
                Login = signupView.Login,
                Email = signupView.Email,
                Password = signupView.Password,
            };
            return View("ConfirmationEmail", confirmationEmailView);
        }
        [HttpGet]
        public IActionResult Signin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Signin(SigninViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.LoginOrEmail)
                       ?? await _userManager.FindByNameAsync(model.LoginOrEmail);

            if (user == null)
            {
                ModelState.AddModelError("LoginOrEmail", "User does not exist");
                return View(model);
            }

            if (user.TwoFactorEnabled && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                TempData["ConfirmationCode"] = await _generateEmail.CodeAsync(user.Email);
                TempData["Action"] = "Signin";

                return View("ConfirmationEmail", new ConfirmationEmailViewModel
                {
                    Login = user.UserName,
                    Email = user.Email,
                    Password = model.Password,
                    RememberMe = model.RememberMe
                });
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                var userPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
                var claimsIdentity = userPrincipal.Identity as ClaimsIdentity;
                claimsIdentity?.AddClaim(new Claim("ProfilePhotoUrl", user.UrlPhoto ?? "/AvaDef.png"));

                await _signInManager.Context.SignInAsync(
                    IdentityConstants.ApplicationScheme,
                    userPrincipal,
                    new AuthenticationProperties { IsPersistent = model.RememberMe });
                
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(result.IsLockedOut ? "LoginOrEmail" : "Password",
                                     result.IsLockedOut ? "User locked" : "Wrong password");

            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnurl = null)
        {
            var redirecturl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnurl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirecturl);
            return Challenge(properties, provider);
        }
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnurl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Signin));
            }
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email!);
            if(user == null)
            {
                user = new AppUser
                {
                    UserName = email!.Split('@')[0],
                    Email = email
                };
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to create user.");
                    return RedirectToAction("Signin");
                }
            }
            var userPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            var claimsIdentity = userPrincipal.Identity as ClaimsIdentity;
            claimsIdentity?.AddClaim(new Claim("ProfilePhotoUrl", user.UrlPhoto ?? "/AvaDef.png"));

            await _signInManager.Context.SignInAsync(
                IdentityConstants.ApplicationScheme,
                userPrincipal);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult ForgotPassword() => View();
        
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Email does not exist");
                return View(model);
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account",
                new { userId = user.Id, 
                      code = WebUtility.UrlEncode(code) },
                HttpContext.Request.Scheme);

            await _generateEmail.LinkAsync(user.Email, callbackUrl);
            return View("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation() => View();
        [HttpGet]
        public async Task<IActionResult> ResetPassword([FromQuery] string userId, [FromQuery] string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return BadRequest("Invalid request.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("User does not exist.");
            }

            var isTokenValid = await _userManager.VerifyUserTokenAsync(
                user,
                TokenOptions.DefaultProvider,
                "ResetPassword",
                WebUtility.UrlDecode(code)
            );

            if (!isTokenValid)
            {
                return BadRequest("The password reset link has expired or is invalid.");
            }

            return View(new ResetPasswordViewModel { Login = user.UserName, Code = code });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetView)
        {
            if (!ModelState.IsValid)
            {
                return View(resetView);
            }

            var user = await _userManager.FindByNameAsync(resetView.Login);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User does not exist.");
                return View(resetView);
            }

            var result = await _userManager.ResetPasswordAsync(
                user, 
                WebUtility.UrlDecode(resetView.Code), 
                resetView.Password);

            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault();
                if (error != null)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(resetView);
            }

            return RedirectToAction("Signin", "Account");
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmationEmail(ConfirmationEmailViewModel model)
        {
            var expectedCode = TempData["ConfirmationCode"] as string;
            var action = TempData["Action"] as string;
            if (string.Concat(model.Code) != expectedCode)
            {
                TempData["ConfirmationCode"] = expectedCode;
                ModelState.AddModelError("Code", "Wrong code");
                return View();
            }
            
            if (action == "Signup")
            {
                var user = new AppUser { UserName = model.Login, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "user");
                    await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var error = result.Errors.FirstOrDefault();
                    if (error != null)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                        return View("Signup", new SignupViewModel { Login = model.Login, Email = model.Email });
                    }
                }
            }
            if (action == "Signin")
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("LoginOrEmail", "User does not exist");
                    return View("Signin", new SigninViewModel { });
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var userPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
                    var claimsIdentity = userPrincipal.Identity as ClaimsIdentity;
                    claimsIdentity?.AddClaim(new Claim("ProfilePhotoUrl", user.UrlPhoto ?? "/AvaDef.png"));

                    await _signInManager.Context.SignInAsync(
                        IdentityConstants.ApplicationScheme,
                        userPrincipal,
                        new AuthenticationProperties { IsPersistent = model.RememberMe });

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(result.IsLockedOut ? "LoginOrEmail" : "Password",
                                         result.IsLockedOut ? "User locked" : "Wrong password");

                return View("Signin", new SigninViewModel { });
            }
            if(action == "ChangeEmail" && model.Email != null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    user.Email = model.Email;
                    var result = await _userManager.UpdateAsync(user);
                    if(result.Succeeded)
                    {
                        TempData["ToastMessage"] = "Email updated!";
                        return RedirectToAction("Settings", "UserMenu");
                    }
                }
            }

            return View();
        }
    }
}
