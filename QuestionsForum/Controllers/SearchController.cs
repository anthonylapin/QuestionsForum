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
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SearchController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(string searchQuery)
        {
            var questions = _db.Questions
                .Where(q => q.Name.Contains(searchQuery) || q.Description.Contains(searchQuery))
                .ToList();
            var tags = _db.Tags.Include(t => t.Questions)
                .Where(t => t.Name.Contains(searchQuery));
            
            foreach(var t in tags)
            {
                questions = questions.Union(t.Questions).ToList();
            }

            return View(questions);
        }
    }
}
