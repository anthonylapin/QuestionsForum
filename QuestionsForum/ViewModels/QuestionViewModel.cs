using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionsForum.ViewModels
{
    public class QuestionViewModel
    {
        public int QuestionId { get; set; }

        [Required]
        [Display(Name = "Header")]
        [MaxLength(32)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        [MaxLength(300)]
        public string Description { get; set; }

        [Display(Name =  "Tags")]
        public string Tags { get; set; }
    }
}
