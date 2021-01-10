using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Collaboration.Models
{
    public class Works
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string description { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}