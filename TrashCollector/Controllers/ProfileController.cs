using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TrashCollector.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TrashCollector.Extensions;

namespace TrashCollector.Controllers
{
    public class ProfileController : Controller
    {
        //member variables
        private ApplicationDbContext db = new ApplicationDbContext();
        Profile profile;

        // GET: Profile
        public ActionResult Index()
        {
            int profileId = Convert.ToInt32(User.Identity.GetProfileId());
            //var userProfile = db.Profiles.Include(p => p.Addresses).Include(p => p.TrashCollections).First(p => p.ProfileId == profileId);
            var userProfile = db.Profiles.Include(p => p.Addresses).Include("Addresses.City").Include("Addresses.State").Include("Addresses.ZipCode").Include(p => p.TrashCollections).First(p => p.ProfileId == profileId);
            return View(userProfile);
        }

        // GET: Profile/Details
        public ActionResult Details()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return HttpNotFound();
            }

            if ( User.Identity.HasProfile() )
            {
                int profileId = Convert.ToInt32(User.Identity.GetProfileId());
                profile = db.Profiles.Find(profileId);
            }
            else
            {
                profile = null;
            }
            return View(profile);
        }

        // GET: Profile/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Profile/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProfileId")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                db.Profiles.Add(profile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(profile);
        }

        // GET: Profile/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // POST: Profile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProfileId")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(profile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(profile);
        }

        // GET: Profile/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // POST: Profile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Profile profile = db.Profiles.Find(id);
            db.Profiles.Remove(profile);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
