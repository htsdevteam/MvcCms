using MvcCms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCms.Data;
using System.Threading.Tasks;

namespace MvcCms.Areas.Admin.Controllers
{
    // /admin/post
    [RouteArea("admin")]
    [RoutePrefix("post")]
    [Authorize]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private bool _isDisposed;

        public PostController()
            : this(new PostRepository(), new UserRepository())
        { }

        public PostController(IPostRepository repository,
            IUserRepository userRepository)
        {
            _postRepository = repository;
            _userRepository = userRepository;
        }

        // GET: admin/post
        [Route("")]
        public async Task<ActionResult> Index()
        {
            if (!User.IsInRole("author"))
            {
                return View(await _postRepository.GetAllAsync());
            }

            var user = await GetLoggedInUser();
            var posts = await _postRepository.GetPostsByAuthorAsync(user.Id);
            return View(posts);
        }

        // /admin/post/create
        [Route("create")]
        public ActionResult Create()
        {
            return View(new Post());
        }

        // /admin/post/create
        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Post model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetLoggedInUser();

            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = model.Title;
            }
            model.Id = model.Id.MakeUrlFriendly();
            model.Tags = model.Tags.Select(t => t.MakeUrlFriendly()).ToList();
            model.Created = DateTime.Now;

            model.AuthorId = user.Id;

            try
            {
                _postRepository.Create(model);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // /admin/post/edit/post-to-edit
        [Route("edit/{postId}")]
        public async Task<ActionResult> Edit(string postId)
        {
            Post post = _postRepository.Get(postId);

            if (post == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole("author"))
            { 
                var user = await GetLoggedInUser();
                if (post.AuthorId != user.Id)
                {
                    return new HttpUnauthorizedResult();
                }
            }

            return View(post);
        }

        // /admin/post/edit/post-to-edit
        [HttpPost]
        [Route("edit/{postId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string postId, Post model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (User.IsInRole("author"))
            {
                var user = await GetLoggedInUser();
                var post = _postRepository.Get(postId);
                try
                {
                    if (post.AuthorId != user.Id)
                    {
                        return new HttpUnauthorizedResult();
                    }
                }
                catch { }
            }

            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = model.Title;
            }
            model.Id = model.Id.MakeUrlFriendly();
            model.Tags = model.Tags.Select(t => t.MakeUrlFriendly()).ToList();

            try
            {
                _postRepository.Edit(postId, model);
                return RedirectToAction("index");
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // /admin/post/delete/post-to-edit
        [Route("delete/{postId}")]
        [Authorize(Roles = "admin, editor")]
        public ActionResult Delete(string postId)
        {
            // TODO: to retrieve the model from the data store
            Post post = _postRepository.Get(postId);

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // /admin/post/delete/post-to-edit
        [HttpPost]
        [Route("delete/{postId}")]
        [Authorize(Roles = "admin, editor")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string postId, string foo)
        {
            try
            {
                _postRepository.Delete(postId);
                return RedirectToAction("index");
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }
        }

        private async Task<CmsUser> GetLoggedInUser()
        {
            return await _userRepository.GetUserByNameAsync(User.Identity.Name);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _userRepository.Dispose();
            }
            _isDisposed = true;
            base.Dispose(disposing);
        }

    }
}