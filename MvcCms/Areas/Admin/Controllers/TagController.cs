using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MvcCms.Data;

namespace MvcCms.Areas.Admin.Controllers
{
    [RouteArea("admin")]
    [RoutePrefix("tag")]
    [Authorize]
    public class TagController : Controller
    {
        private readonly ITagRepository _repository;

        public TagController() : this (new TagRepository())
        { }

        public TagController(ITagRepository repository)
        {
            _repository = repository;
        }

        // GET: Admin/Tag
        [Route("")]
        [Authorize(Roles = "admin, editor")]
        public ActionResult Index()
        {
            IEnumerable<string> tags = _repository.GetAll();

            if (Request.AcceptTypes.Contains("application/json"))
            {
                return Json(tags, JsonRequestBehavior.AllowGet);
            }

            if (User.IsInRole("author"))
            {
                return new HttpUnauthorizedResult();
            }

            return View(tags);
        }

        [Route("edit/{tag}")]
        [Authorize(Roles = "admin, editor")]
        public ActionResult Edit(string tag)
        {
            try
            {
                var model = _repository.Get(tag);
                return View(model: model);
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }
        }

        [Route("edit/{tag}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, editor")]
        public ActionResult Edit(string tag, string newTag)
        {
            if (string.IsNullOrWhiteSpace(newTag))
            {
                ModelState.AddModelError("key", "New tag value cannot be empty.");
                return View(model: tag);
            }

            IEnumerable<string> tags = _repository.GetAll();

            if (!tags.Contains(tag))
            {
                return HttpNotFound();
            }
            if (tags.Contains(newTag))
            {
                return RedirectToAction("index");
            }

            _repository.Edit(tag, newTag);

            return RedirectToAction("index");
        }

        [Route("delete/{tag}")]
        [Authorize(Roles = "admin, editor")]
        public ActionResult Delete(string tag)
        {
            try
            {
                string model = _repository.Get(tag);
                return View(model: model);
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }
        }

        [Route("delete/{tag}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, editor")]
        public ActionResult Delete(string tag, string foo)
        {
            try
            {
                _repository.Delete(tag);
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }

            return RedirectToAction("index");
        }

    }
}