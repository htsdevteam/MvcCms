﻿using MvcCms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcCms.Data
{
    public class TagRepository : ITagRepository
    {
        public IEnumerable<string> GetAll()
        {
            using (var db = new CmsContext())
            {
                var tagsCollection = db.Posts
                    .Select(p => p.CombinedTags)
                    .ToList();
                return string.Join(",", tagsCollection).Split(',').Distinct();
                //return db.Posts.ToList().SelectMany(p => p.Tags).Distinct();
            }
        }

        public void Delete(string tag)
        {
            using (var db = new CmsContext())
            {
                var posts = db.Posts.Where(p => p.CombinedTags.Contains(tag))
                    .ToList();
                posts = posts.Where(p => p.Tags
                        .Contains(tag, StringComparer.CurrentCultureIgnoreCase))
                    .ToList();

                if (!posts.Any())
                {
                    throw new KeyNotFoundException(
                        string.Format("The tag {} does not exist.", tag));
                }

                foreach (Post post in posts)
                {
                    post.Tags.Remove(tag);
                }
                db.SaveChanges();
            }
        }

        public void Edit(string existingTag, string newTag)
        {
            using (var db = new CmsContext())
            {
                var posts = db.Posts.Where(p => p.CombinedTags.Contains(existingTag))
                    .ToList();
                posts = posts.Where(p => p.Tags
                        .Contains(existingTag, StringComparer.CurrentCultureIgnoreCase))
                    .ToList();

                if (!posts.Any())
                {
                    throw new KeyNotFoundException(
                        string.Format("The tag {} does not exist.", existingTag));
                }

                foreach (Post post in posts)
                {
                    post.Tags.Remove(existingTag);
                    post.Tags.Add(newTag);
                }
                db.SaveChanges();
            }
        }

        public string Get(string tag)
        {
            using (var db = new CmsContext())
            {
                var posts = db.Posts.Where(p => p.CombinedTags.Contains(tag))
                    .ToList();
                posts = posts.Where(p => p.Tags
                        .Contains(tag, StringComparer.CurrentCultureIgnoreCase))
                    .ToList();

                if (!posts.Any())
                {
                    throw new KeyNotFoundException(
                        string.Format("The tag {} does not exist.", tag));
                }
            }
            return tag.ToLower();
        }
    }
}