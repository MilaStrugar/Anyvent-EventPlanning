using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeddingPlanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        public User LoggedInUser()
        {
            int? LoggedID = HttpContext.Session.GetInt32("UserId");
            User logged = _context.Users.FirstOrDefault(u => u.UserId == LoggedID);
            return logged;
        }
        public int UserID()
        {
            int UserID = LoggedInUser().UserId;
            return UserID;
        }
        private MyContext _context;

        public HomeController(MyContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Register");
        }
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost("Register")]
        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(user => user.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use!");
                    return View("Register");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                Console.WriteLine(newUser.Password);
                _context.Add(newUser);
                _context.SaveChanges();
                Console.WriteLine(newUser.UserId);
                HttpContext.Session.SetInt32("UserId", newUser.UserId);
                return RedirectToAction("Dashboard");
            }
            return View("Register");
        }
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Register");
            }
            List<Wedding> AllWeddings = _context.Weddings
            .Include(w => w.Guests)
            .ThenInclude(w => w.User)
            .ToList();
            ViewBag.AllWeddings = AllWeddings;
            ViewBag.UserId = (int)HttpContext.Session.GetInt32("UserId");
            return View("Dashboard", AllWeddings);
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost("login")]
        public IActionResult LoginUser(LoginUser loginUser)
        {
            if (ModelState.IsValid)
            {
                var dbuser = _context.Users.FirstOrDefault(user => user.Email == loginUser.LoginEmail);
                if (dbuser == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid email");
                    return View("Login");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(loginUser, dbuser.Password, loginUser.LoginPassword);
                if (result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid email");
                    return View("Login");
                }
                HttpContext.Session.SetInt32("UserId", dbuser.UserId);
                return RedirectToAction("Dashboard");
            }
            return View("Login");
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        [HttpGet("delete/{WeddingId}")]
        public IActionResult DeleteWedding(int WeddingId)
        {
            Wedding DeleteWedding = _context.Weddings.FirstOrDefault(w => w.WeddingId == WeddingId);
            _context.Weddings.Remove(DeleteWedding);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpGet("newwedding")]
        public IActionResult NewWedding()
        {
            if (LoggedInUser() == null)
            {
                return RedirectToAction("Register");
            }
            return View("NewWedding");
        }
        [HttpPost("addnewwedding")]
        public IActionResult CreateNewWedding(Wedding NewWedding)
        {
            if (ModelState.IsValid)
            {
                NewWedding.PlannerId = (int)HttpContext.Session.GetInt32("UserId");
                _context.Add(NewWedding);
                _context.SaveChanges();
                Wedding AddedWedding = _context.Weddings.OrderByDescending(wedding => wedding.CreatedAt)
                .FirstOrDefault();
                return Redirect("dashboard");
            }
            return View("NewWedding", NewWedding);
        }
        [HttpGet("wedding/{WeddingId}")]
        public IActionResult TheWedding(int WeddingId)
        {
            Wedding SpecificWedding = _context.Weddings.FirstOrDefault(w => w.WeddingId == WeddingId);
            ViewBag.ThisWedding = SpecificWedding;

            var GroupOfGuests = _context.Weddings
                .Include(w => w.Guests)
                .ThenInclude(u => u.User)
                .FirstOrDefault(w => w.WeddingId == WeddingId);

            ViewBag.AllGuests = GroupOfGuests.Guests;
            return View("TheWedding");
        }
        [HttpGet("wedding/{WeddingId}/rsvpyes")]
        public IActionResult RSVPYES(int WeddingId)
        {
            int? LoggedID = HttpContext.Session.GetInt32("UserId");
            if (LoggedID == null)
            {
                return RedirectToAction("Index");
            }
            RSVP going = new RSVP();
            going.UserId = (int)LoggedID;
            going.WeddingId = WeddingId;
            _context.Add(going);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("wedding/{WeddingId}/rsvpno")]
        public IActionResult RSVPNO(int WeddingId)
        {
            int? LoggedID = HttpContext.Session.GetInt32("UserId");
            if (LoggedID == null)
            {
                return RedirectToAction("Index");
            }
            RSVP notGoing = _context.Guests.FirstOrDefault(no => no.UserId == LoggedID && no.WeddingId == WeddingId);
            if (notGoing != null)
            {
                _context.Guests.Remove(notGoing);
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }
    }
}
