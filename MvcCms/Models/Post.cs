using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcCms.Models
{
    public class Post
    {
        [Display(Name = "Slug")]
        public string Id { get; set; }
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Display(Name = "Post content")]
        public string Content { get; set; }
        [Display(Name = "Date created")]
        public DateTime Created { get; set; }
        [Display(Name = "Date published")]
        public DateTime? Published { get; set; }

        public IList<string> Tags { get; set; }

        public int AuthorId { get; set; }
    }
}