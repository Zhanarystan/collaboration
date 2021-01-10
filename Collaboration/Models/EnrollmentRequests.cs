using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Collaboration.Models
{
    public class EnrollmentRequests
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Field must be set")]
        [Display(Name = "Request Message")]
        public string RequestMessage { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public int? ProjectId { get; set; }
        public virtual Projects Project { get; set; }
    }
}