using Events.Data;
using Events.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Events.Web.Controllers
{
    [Authorize]
    public class EventsController : BaseController
    {
        public ActionResult My()
        {
            string currentUserId = this.User.Identity.GetUserId();
            var events = this.db.Events

               .Where(e => e.AuthorId == currentUserId)
               .OrderBy(e => e.StartDateTime)
               .Select(EventViewModel.ViewModel);
               
            var UpcomingEvents = events.Where(e => e.StartDateTime > DateTime.Now);
            var PassEvents = events.Where(e => e.StartDateTime <= DateTime.Now);
            return View(new UpcomingPassedEventsViewModel()
            {
                UpcomingEvents = UpcomingEvents,
                PassEvents = PassEvents,
            });
        }
        // GET: Events
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create (EventInputModel model)
        {
            if (model != null && this.ModelState.IsValid)
            {
                var e = new Event()
                {
                    AuthorId = this.User.Identity.GetUserId(),
                    Title = model.Title,
                    StartDateTime = model.StartDateTime,
                    Duration = model.Duration,
                    Description = model.Description,
                    Location = model.Location,
                    IsPublic = model.IsPublic
                };
                this.db.Events.Add(e);
                this.db.SaveChanges();
                return this.RedirectToAction("My");
            }
            return this.View(model);
        }
        
    }
}