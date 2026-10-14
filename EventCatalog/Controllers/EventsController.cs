using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EventCatalog.Models;

namespace EventCatalog.Controllers
{
    public class EventsController : Controller
    {
        private AppEntities db = new AppEntities();

        private List<Event> Query(int? category, decimal? minPrice, decimal? maxPrice, string search)
        {
            var q = db.Events.Include("Category").Where(e => e.IsPublished);
            if (category.HasValue) q = q.Where(e => e.CategoryId == category.Value);
            if (minPrice.HasValue) q = q.Where(e => e.Price >= minPrice.Value);
            if (maxPrice.HasValue) q = q.Where(e => e.Price <= maxPrice.Value);
            if (!string.IsNullOrWhiteSpace(search)) q = q.Where(e => e.Title.Contains(search));
            return q.OrderBy(e => e.StartDate).ToList();
        }

        public ActionResult Index(int? category, decimal? minPrice, decimal? maxPrice, string search)
        {
            ViewBag.Categories = new SelectList(db.Categories, "Id", "Name", category);
            return View(Query(category, minPrice, maxPrice, search));
        }

        [OutputCache(Duration = 30, VaryByParam = "*")]
        public ActionResult Filter(int? category, decimal? minPrice, decimal? maxPrice, string search)
        {
            ViewBag.ServerTime = DateTime.Now.ToString("HH:mm:ss");
            return PartialView("_EventList", Query(category, minPrice, maxPrice, search));
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return HttpNotFound();
            var ev = db.Events.Include("Category").FirstOrDefault(e => e.Id == id && e.IsPublished);
            if (ev == null) return HttpNotFound();
            return View(ev);
        }

        public JsonResult CheckSeats(int id)
        {
            var ev = db.Events.Find(id);
            if (ev == null)
                return Json(new { ok = false, message = "Мероприятие не найдено" }, JsonRequestBehavior.AllowGet);

            int free = ev.Seats;
            return Json(new
            {
                ok = true,
                id = ev.Id,
                title = ev.Title,
                freeSeats = free,
                status = free > 0 ? "места есть" : "мест нет"
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Manage()
        {
            return View(db.Events.Include("Category").OrderBy(e => e.Id).ToList());
        }

        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View(new EventFormViewModel { StartDate = DateTime.Now.AddDays(7) });
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public ActionResult Create(EventFormViewModel vm)
        {
            if (!db.Categories.Any(c => c.Id == vm.CategoryId))
                ModelState.AddModelError("CategoryId", "Такой категории нет");
            if (vm.StartDate < new DateTime(2000, 1, 1))
                ModelState.AddModelError("StartDate", "Недопустимая дата");

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", vm.CategoryId);
                return View(vm);
            }

            db.Events.Add(new Event
            {
                Title = vm.Title,
                Description = vm.Description,
                StartDate = vm.StartDate,
                Price = vm.Price,
                Seats = vm.Seats,
                CategoryId = vm.CategoryId,
                IsPublished = false
            });
            db.SaveChanges();
            return RedirectToAction("Manage");
        }

        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Edit(int? id)
        {
            if (id == null) return HttpNotFound();
            var ev = db.Events.Find(id);
            if (ev == null) return HttpNotFound();

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", ev.CategoryId);
            return View(new EventFormViewModel
            {
                Id = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                StartDate = ev.StartDate,
                Price = ev.Price,
                Seats = ev.Seats,
                CategoryId = ev.CategoryId
            });
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public ActionResult Edit(EventFormViewModel vm)
        {
            var ev = db.Events.Find(vm.Id);
            if (ev == null) return HttpNotFound();

            if (!db.Categories.Any(c => c.Id == vm.CategoryId))
                ModelState.AddModelError("CategoryId", "Такой категории нет");
            if (vm.StartDate < new DateTime(2000, 1, 1))
                ModelState.AddModelError("StartDate", "Недопустимая дата");

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", vm.CategoryId);
                return View(vm);
            }

            ev.Title = vm.Title;
            ev.Description = vm.Description;
            ev.StartDate = vm.StartDate;
            ev.Price = vm.Price;
            ev.Seats = vm.Seats;
            ev.CategoryId = vm.CategoryId;
            db.SaveChanges();
            return RedirectToAction("Manage");
        }

        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Delete(int? id)
        {
            if (id == null) return HttpNotFound();
            var ev = db.Events.Include("Category").FirstOrDefault(e => e.Id == id);
            if (ev == null) return HttpNotFound();
            return View(ev);
        }

        [HttpPost, ActionName("Delete"), Authorize, ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var ev = db.Events.Find(id);
            if (ev == null) return HttpNotFound();
            db.Events.Remove(ev);
            db.SaveChanges();
            return RedirectToAction("Manage");
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public JsonResult TogglePublish(int id)
        {
            var ev = db.Events.Find(id);
            if (ev == null) return Json(new { ok = false, message = "Мероприятие не найдено" });

            ev.IsPublished = !ev.IsPublished;
            db.SaveChanges();
            return Json(new { ok = true, isPublished = ev.IsPublished });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}