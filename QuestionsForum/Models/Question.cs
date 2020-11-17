using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionsForum.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string UserId { get; set; }

        public User User { get; set; }

        public List<Tag> Tags { get; set; } = new List<Tag>();
        public List<Answer> Answers { get; set; } = new List<Answer>();
        public List<Vote> Votes { get; set; } = new List<Vote>();
    }
}
