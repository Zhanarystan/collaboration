using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Collaboration.Models
{
    public class Specifications : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Field should be set")]
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Field should be set")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public virtual ICollection<Projects> Projects { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            if (string.IsNullOrEmpty(this.Title)) {
                errors.Add(new ValidationResult("Title isn't set"));
            }
            else if (this.Title.Length < 5)
            {
                errors.Add(new ValidationResult("Titles should contain at least 6 characters"));
            }
            return errors;
        }
    }
}