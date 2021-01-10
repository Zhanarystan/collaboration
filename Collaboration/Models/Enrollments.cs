using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Collaboration.Models
{
    public class Enrollments
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public int? ProjectId { get; set; }
        public virtual Projects Project { get; set; }
    }
}