using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestionsForum.Data;
using QuestionsForum.Models;
using QuestionsForum.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionsForum.Controllers
{ 
    public class QuestionController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;

        public QuestionController(ApplicationDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            var questions = _db.Questions
                .Where(q => q.User.UserName == User.Identity.Name)
                .ToList();
            return View(questions);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Ask()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Ask(QuestionViewModel questionModel)
        {
            if(ModelState.IsValid)
            {
                List<string> insertedTags = getInsertedTagsArray(questionModel.Tags);
                List<Tag> duplicates = getTagsDuplicates(insertedTags);
                var tagsToAddToDb = getTagsToAddToDb(insertedTags, duplicates);

                await _db.AddRangeAsync(tagsToAddToDb);
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                var question = new Question
                {
                    Name = questionModel.Name,
                    Description = questionModel.Description,
                    User = user
                };
                await _db.Questions.AddAsync(question);

                question.Tags.AddRange(tagsToAddToDb.Concat(duplicates));

                await _db.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View(questionModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var question = await _db.Questions.Include(q => q.Tags).FirstOrDefaultAsync(q => q.Id == id);

            if(question == null)
            {
                return NotFound();
            }

            var tagArr = question.Tags.ConvertAll(t => t.Name).ToArray();
            var tagStr = string.Join(" ", tagArr);

            return View(new QuestionViewModel()
            {
                QuestionId = question.Id,
                Name = question.Name,
                Description = question.Description,
                Tags = tagStr
            });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(QuestionViewModel questionVM)
        {
            if(ModelState.IsValid)
            {
                var question = await _db.Questions
                    .Include(q => q.Tags)
                    .FirstOrDefaultAsync(u => u.Id == questionVM.QuestionId);

                if(question == null)
                {
                    return NotFound();
                }

                List<string> insertedTags = getInsertedTagsArray(questionVM.Tags);
                List<Tag> duplicates = getTagsDuplicates(insertedTags);
                var tagsToAddToDb = getTagsToAddToDb(insertedTags, duplicates);

                await _db.Tags.AddRangeAsync(tagsToAddToDb);
                
                question.Name = questionVM.Name;
                question.Description = question.Description;

                var tagsToAddToQuestion = tagsToAddToDb.Concat(duplicates.Where(d => !question.Tags.Contains(d)));
                question.Tags.AddRange(tagsToAddToQuestion);

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(questionVM);
        }

        private List<string> getInsertedTagsArray(string str)
        {
            return str.Split(new char[] { ' ', ',', '-' }).ToList();
        }

        private List<Tag> getTagsDuplicates(List<string> insertedTags)
        {
            return _db.Tags.Where(t => insertedTags.Contains(t.Name)).ToList();
        }

        private List<Tag> getTagsToAddToDb(List<string> insertedTags, List<Tag> duplicates)
        {
            var duplicatesStrArr = duplicates.ConvertAll(d => d.Name);
            var tagsToAddToDb = insertedTags.Where(t => !duplicatesStrArr.Contains(t)).ToList()
                .ConvertAll(t => new Tag { Name = t });
            return tagsToAddToDb;
        }
    }
}
