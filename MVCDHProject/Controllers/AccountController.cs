using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCDHProject.Models;
using System.Text;
using MimeKit;
using MailKit.Net.Smtp;
using System.Security.Claims;

namespace MVCDHProject.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        #region Fields & Consturctors
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> singInManager;
        public AccountController(UserManager<IdentityUser> userManager,SignInManager<IdentityUser> singInManager)
        {
            this.userManager = userManager;
            this.singInManager = singInManager;
        }
        #endregion

        #region Registration
        [HttpGet]
        public ViewResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel userModel)
        {
            if(ModelState.IsValid)
            {
                IdentityUser identityUser = new IdentityUser
                {
                    UserName = userModel.Name,
                    Email = userModel.Email,
                    PhoneNumber = userModel.Mobile
                };
                var result = await userManager.CreateAsync(identityUser, userModel.Password);
                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                    var confirmationUrlLink = Url.Action("ConfirmEmail", "Account", new { userId = identityUser.Id,Token = token }, Request.Scheme);
                    SendMail(identityUser, confirmationUrlLink, "Email Confirmation Link");
                    TempData["Title"] = "Email Confirmation Link";
                    TempData["Message"] = "Please check your email to confirm your account.";
                    return View("DisplayMessages");
                }
                else
                {
                    foreach(var Error in result.Errors)
                    {
                        ModelState.AddModelError("",Error.Description);
                    }
                }
            }
            return View(userModel);   
        }
        #endregion

        #region Login & Logout
        [HttpGet]
        public ViewResult Login()
        {
            return View();
        }
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(loginModel.Name);
                if (user != null && (await userManager.CheckPasswordAsync(user, loginModel.Password)) && user.EmailConfirmed == false)
                {
                    ModelState.AddModelError("", "Please confirm your email before login.");
                    return View(loginModel);
                }

                var result = await singInManager.PasswordSignInAsync(loginModel.Name, loginModel.Password, loginModel.RememberMe, false);
                if (result.Succeeded)
                {
                    if(string.IsNullOrEmpty(loginModel.ReturnUrl))
                        return RedirectToAction("Index", "Home");
                    else
                        return LocalRedirect(loginModel.ReturnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Login Credentials");
                }
            }
            return View(loginModel);
        }
        
        public async Task<IActionResult> Logout()
        {
            await singInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        #endregion

        #region Email Confirmation
        public void SendMail(IdentityUser identityUser, string requestLink, string subject)
        {
            StringBuilder mailBody = new StringBuilder();
            mailBody.Append("Hello" + identityUser.UserName + "<br><br>");
            if(subject == "Email Confirmation Link")
            {
                mailBody.Append("Please click on the below link to confirm your email.");
            }
            else if(subject == "Reset Password Link")
            {
                mailBody.Append("Please click on the below link to reset your password.");
            }
            mailBody.Append("<br />");
            mailBody.Append(requestLink);
            mailBody.Append("<br><br>");
            mailBody.Append("Regards");
            mailBody.Append("<br /><br />");
            mailBody.Append("Customer Support.");

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = mailBody.ToString();

            MailboxAddress fromAddress = new MailboxAddress("Customer Support", "superaddmin6370@gmail.com");
            MailboxAddress toAddress = new MailboxAddress(identityUser.UserName, identityUser.Email);
            MimeMessage mailMessage = new MimeMessage();
            mailMessage.From.Add(fromAddress);
            mailMessage.To.Add(toAddress);
            mailMessage.Subject = subject;
            mailMessage.Body = bodyBuilder.ToMessageBody();
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Connect("smtp.gmail.com", 587, false);
            smtpClient.Authenticate("superaddmin6370@gmail.com", "gpuw hrlh rxim rdbi");
            smtpClient.Send(mailMessage);
        }


        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId != null && token != null)
            {
                var User = await userManager.FindByIdAsync(userId);
                if (User != null)
                {
                    var result = await userManager.ConfirmEmailAsync(User, token);
                    if (result.Succeeded)
                    {
                        TempData["Title"] = "Email Confirmation Success.";
                        TempData["Message"] = "Email confirmation is completed. You can now login into the application.";
                        return View("DisplayMessages");
                    }
                    else
                    {
                        StringBuilder Errors = new StringBuilder();
                        foreach (var Error in result.Errors)
                        {
                            Errors.Append(Error.Description);
                        }
                        TempData["Title"] = "Confirmation Email Failure";
                        TempData["Message"] = Errors.ToString();
                        return View("DisplayMessages");
                    }
                }
                else
                {
                    TempData["Title"] = "Invalid User Id.";
                    TempData["Message"] = "User Id which is present in confirm email link is in-valid.";
                    return View("DisplayMessages");
                }
            }
            else
            {
                TempData["Title"] = "Invalid Email Confirmation Link.";
                TempData["Message"] = "Email confirmation link is invalid, either missing the User Id or Confirmation Token.";
                return View("DisplayMessages");
            }
        }

        #endregion

        #region Reset Password
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var User = await userManager.FindByNameAsync(model.Name);
                if (User != null && await userManager.IsEmailConfirmedAsync(User))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(User);
                    var confirmationUrlLink = Url.Action("ResetPassword", "Account", new { UserId = User.Id, Token = token }, Request.Scheme);
                    SendMail(User, confirmationUrlLink, "Reset Password Link");
                    TempData["Title"] = "Reset Password Link";
                    TempData["Message"] = "Link has been send, Please check your email to reset your password.";
                }
                else 
                {
                    TempData["Title"] = "Invalid Request";
                    TempData["Message"] = "Either user is not present with the given name or email is not confirmed yet.";
                    return View("DisplayMessages");
                }
            }
            return View(model);
        }
        public IActionResult ResetPassword(string userId, string token)
        {
            if (userId == null || token == null)
                return RedirectToAction("Login");

            var model = new ResetPasswordViewModel
            {
                UserId = userId,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                var User = await userManager.FindByIdAsync(model.UserId);
               
                if (User != null)
                {
                    var result = await userManager.ResetPasswordAsync(User, model.Token, model.Password);
                    if(result.Succeeded)
                    {
                        TempData["Title"] = "Reset Password Success";
                        TempData["Message"] = "Your password has been reset successfully.";
                        return View("DisplayMessages");
                    }
                    else
                    {
                        foreach(var Error in result.Errors)
                        {
                            ModelState.AddModelError("", Error.Description);
                        }
                        return View(model);
                    }
                }
                else
                {
                    TempData["Title"] = "Invalid User";
                    TempData["Message"] = "No user exists with the given User Id.";
                    return View("DisplayMessages");
                }
            }
            ModelState.AddModelError("", "Either User Id or Token is Missing in the reset password link.");
            return View(model);
        }
        #endregion

        #region External Login

        public IActionResult ExternalLogin(string returnUrl,string Provider)
        {
            var url = Url.Action("CallBack","Account", new { ReturnUrl = returnUrl});
            var properties = singInManager.ConfigureExternalAuthenticationProperties(Provider, url);
            return new ChallengeResult(Provider, properties);
        }

        public async Task<IActionResult> CallBack(string returnUrl)
        {
            if(string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "~/";
            }
            var info = await singInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError("", "Error while getting external login information during callback.");
                return View("Login");
            }
            var signInResult = await singInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    var user = await userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new IdentityUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            PhoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone),
                        };
                        var identityResult = await userManager.CreateAsync(user);
                    }
                    await userManager.AddLoginAsync(user, info);
                    await singInManager.SignInAsync(user, false);
                    return LocalRedirect(returnUrl);
                }
                TempData["Title"] = "Email Claim Not Received";
                TempData["Message"] = "We are unable to get your email from " + info.LoginProvider + " provider. Please contact support team for further assistance.";
                return RedirectToAction("DisplayMessages");
            }
        }

        #endregion

    }
}
