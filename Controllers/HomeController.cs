using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bank_Accounts.Models;
using Bank_Accounts.Persistence;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Bank_Accounts.Controllers
{
    public class HomeController : Controller
    {   
        private BankAccountDbContext _dbContext;
        public HomeController(BankAccountDbContext context) {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Index(User user)
        {
            // Check initial ModelState
            if(ModelState.IsValid)
            {
                // If a User exists with provided email
                if(_dbContext.Users.Any(u => u.Email == user.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");
                    
                    // You may consider returning to the View at this point
                    return View();
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                _dbContext.Add(user);
                _dbContext.SaveChanges();
                
                return Redirect("/login");
            }

            // other code
            return View();
        } 

        [HttpGet]
        [Route("/login")]
        public IActionResult Login(){
            return View();
        }

        [HttpPost]
        [Route("/login")]
        public IActionResult Login(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = _dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                // If no user exists with provided email
                if(userInDb == null || userSubmission.Password == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View();
                }
                
                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();
                
                // varify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled
                    ModelState.AddModelError("Password", "Invalid Email/Password");
                    return View();
                }
                
                // I usually wouldn't store an entire user's information in a session variable.
                HttpContext.Session.SetInt32("User", userInDb.UserId);
                return Redirect("/bankaccount");
            }

            return View();
        }

        // I changed the url route from the wireframe so that the account number doesnt get displayed.
        // I deviated from the requirements.
        [HttpGet]
        [Route("/bankaccount")]
        public IActionResult BankAccount(){
            int? userid = HttpContext.Session.GetInt32("User");

            if(HttpContext.Session.GetInt32("User") != null) {
                ViewData["Message"] = "Your success page.";
                BankAccountViewModel vm = new BankAccountViewModel();
                User user = _dbContext.Users.Include(u => u.Transactions).Where(u => u.UserId == userid).FirstOrDefault();
                vm.UserName = user.FirstName + " " + user.LastName;
                vm.CurrentBalance = user.Transactions.Sum(t => t.Amount);
                vm.DepositOrWithdrawalAmount = null;
                vm.Transactions = user.Transactions;

                return View(vm);
            } else {
                return Redirect("/");
            }
        }

        [HttpPost]
        [Route("/bankaccount")]
        public IActionResult BankAccount(BankAccountViewModel model){
            int? userid = HttpContext.Session.GetInt32("User");

            if(HttpContext.Session.GetInt32("User") != null ) {
                ViewData["Message"] = "Your success page.";
                BankAccountViewModel vm = model;
                User user = _dbContext.Users.Include(u => u.Transactions).Where(u => u.UserId == userid).FirstOrDefault();        
                vm.Transactions = user.Transactions;
                
                if(vm.CurrentBalance + vm.DepositOrWithdrawalAmount < 0) {          
                    ModelState.AddModelError("DepositOrWithdrawalAmount", "You cant withdraw more than you have!");
                } else {
                    Transaction transaction = new Transaction();
                    transaction.Amount = (int)model.DepositOrWithdrawalAmount; 
                    transaction.User = user;
                    vm.CurrentBalance = user.Transactions.Sum(t => t.Amount);
                    
                    _dbContext.Add(transaction);
                    _dbContext.SaveChanges();
                }
                return View(vm);
            } else {
                return Redirect("/");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
