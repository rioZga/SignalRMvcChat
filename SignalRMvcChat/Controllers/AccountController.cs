using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignalRMvcChat.ViewModels;

namespace SignalRMvcChat.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            var user = await _userManager.FindByNameAsync(loginVM.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid credentials, please try again.");
                return View(loginVM);
            }
            var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
            if (!passwordCheck)
            {
                ModelState.AddModelError("", "Invalid credentials, please try again.");
                return View(loginVM);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid credentials, please try again.");
                return View(loginVM);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if (!ModelState.IsValid)
                return View(registerVM);

            var user = await _userManager.FindByNameAsync(registerVM.UserName);
            if (user != null)
            {
                ModelState.AddModelError("", "User with this username already exists!");
                return View(registerVM);
            }

            user = new IdentityUser
            {
                UserName = registerVM.UserName,
            };

            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "An error occurred, please try again!");
                return View(registerVM);
            }

            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
