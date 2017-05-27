using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcCms.Models;

namespace MvcCms.Data
{
    public class PostRepository : IPostRepository
    {
        public Post Get(string id)
        {
            using (var db = new CmsContext())
            {
                return db.Posts.Include("Author").SingleOrDefault(p => p.Id == id);
            }
        }

        public void Create(Post model)
        {
            using (var db = new CmsContext())
            {
                var post = db.Posts.SingleOrDefault(p => p.Id == model.Id);
                if (post != null)
                {
                    throw new ArgumentException(string.Format(
                        "A post with the id of {0} already exists", model.Id));
                }

                db.Posts.Add(model);
                db.SaveChanges();
            }
        }

        public void Edit(string id, Post updatedItem)
        {
            using (var db = new CmsContext())
            {
                var post = db.Posts.SingleOrDefault(p => p.Id == id);
                if (post == null)
                {
                    throw new KeyNotFoundException(string.Format(
                        "A post with the id of {0} does not exist in the data store.", id));
                }

                post.Id = updatedItem.Id;
                post.Title = updatedItem.Title;
                post.Content = updatedItem.Content;
                post.Published = updatedItem.Published;
                post.Tags = updatedItem.Tags;

                db.SaveChanges();
            }
        }

        public IEnumerable<Post> GetAll()
        {
            using (var db = new CmsContext())
            {
                return db.Posts.Include("Author").OrderByDescending(p => p.Created).ToArray();
            }
        }

        public void Delete(string id)
        {
            using (var db = new CmsContext())
            {
                var post = db.Posts.SingleOrDefault(p => p.Id == id);
                if (post == null)
                {
                    throw new KeyNotFoundException(
                        string.Format("The post with the id of {0} does not exist.", id));
                }
                db.Posts.Remove(post);
                db.SaveChanges();
            }
        }
    }
}