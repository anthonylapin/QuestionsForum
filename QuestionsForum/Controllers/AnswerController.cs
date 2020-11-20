using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuestionsForum.Data;
using QuestionsForum.Models;
using QuestionsForum.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionsForum.Controllers
{
    public class AnswerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;

        public AnswerController(ApplicationDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        #region API_CALLS

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index(string description, int questionId)
        {
            Question question = _db.Questions.FirstOrDefault(q => q.Id == questionId);
            User user = await _userManager.FindByNameAsync(User.Identity.Name);

            if(description == null || question == null)
            {
                return Json(new { success = false, message = "Wrong data entered, try again." });
            }

            var answer = new Answer() { Description = description, Question = question, User = user };
            await _db.Answers.AddAsync(answer);
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Answer added successfully" });
        }

        #endregion
    }
}
