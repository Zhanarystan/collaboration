using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Collaboration.Models
{
    public class Countries
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Field must be set")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Field must be set")]
        [Display(Name = "Code")]
        public string Code { get; set; }

        public virtual ICollection<Projects> Projects { get; set; }
        
    }

    public class CountriesViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Field must be set")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Field must be set")]
        [Display(Name = "Code")]
        public string Code { get; set; }

    }
}