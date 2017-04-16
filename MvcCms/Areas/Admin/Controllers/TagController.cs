using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCms.Data;

namespace MvcCms.Areas.Admin.Controllers
{
    public class TagController : Controller
    {
        private readonly ITagRepository _repository;

        public TagController(ITagRepository repository)
        {
            _repository = repository;
        }

        // GET: Admin/Tag
        public ActionResult Index()
        {
            IEnumerable<string> tags = _repository.GetAll();
            return View(tags);
        }

        public ActionResult Edit(string tag)
        {
            if (!_repository.Exists(tag))
            {
                return HttpNotFound();
            }
            return View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string tag, string newTag)
        {
            if (string.IsNullOrWhiteSpace(newTag))
            {
                ModelState.AddModelError("key", "New tag value cannot be empty.");
                return View(tag);
            }

            IEnumerable<string> tags = _repository.GetAll();

            if (!tags.Contains(tag))
            {
                return HttpNotFound();
            }
            if (!tags.Contains(newTag))
            {
                return RedirectToAction("index");
            }

            _repository.Edit(tag, newTag);

            return RedirectToAction("index");
        }

        public ActionResult Delete(string tag)
        {
            if (!_repository.Exists(tag))
            {
                return HttpNotFound();
            }
            return View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string tag, bool foo)
        {
            if (!_repository.Exists(tag))
            {
                return HttpNotFound();
            }

            _repository.Delete(tag);

            return RedirectToAction("index");
        }

    }
}