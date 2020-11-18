using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestionsForum.Data;
using QuestionsForum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionsForum.Controllers
{
    public class VoteController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;

        public VoteController(ApplicationDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API_CALLS
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Index(short assesment, int questionId)
        {
            if(assesment != 1 && assesment != -1)
            {
                return Json(new { success = false, message = "Assesment must have value either 1 or -1" });
            }

            var question = await _db.Questions.FirstOrDefaultAsync(q => q.Id == questionId);
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (question == null || user == null)
            {
                return NotFound();
            }

            // if user already voted, remove his vote, else add new one
            var vote = await _db.Votes.FirstOrDefaultAsync(v => v.UserId == user.Id);

            if(vote == null)
            {
                vote = new Vote()
                {
                    Assesment = assesment,
                    User = user,
                    Question = question
                };

                await _db.Votes.AddAsync(vote);
            }
            else
            {
                // if vote.assessment == assesment -> delete vote. else update vote.
                if(vote.Assesment == assesment)
                {
                    _db.Votes.Remove(vote);
                }
                else
                {
                    vote.Assesment = assesment;
                }
            }

            await _db.SaveChangesAsync();

            return Json(new { success = true });
        }
        #endregion
    }
}
