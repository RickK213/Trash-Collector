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

        // GET: Profile
        public ActionResult Index()
        {
            int profileId = Convert.ToInt32(User.Identity.GetProfileId());
            //var userProfile = db.Profiles.Include(p => p.Addresses).Include(p => p.TrashCollections).First(p => p.ProfileId == profileId);
            var userProfile = db.Profiles
                .Include(p => p.Addresses)
                .Include("Addresses.City")
                .Include("Addresses.State")
                .Include("Addresses.ZipCode")
                .Include("Addresses.TrashCollection")
                .First(p => p.ProfileId == profileId);
            return View(userProfile);
        }

        // GET: Profile/Details
        public ActionResult Details()
        {
            //This is where the user's account information will go.
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "User");
            }
            else if (User.IsInRole("Employee"))
            {
                //change this!
                return RedirectToAction("Pickups", "Profile");
            }
            else
            {
                return View();
            }
        }

        // GET: Profile/Addresses
        public ActionResult Addresses()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            int profileId = Convert.ToInt32(User.Identity.GetProfileId());
            var userProfile = db.Profiles
                .Include(p => p.Addresses)
                .Include("Addresses.City")
                .Include("Addresses.State")
                .Include("Addresses.ZipCode")
                .Include("Addresses.TrashCollection")
                .First(p => p.ProfileId == profileId);

            //test data
            //StringBuilder testData = new StringBuilder();
            //end of test data

            //return Content( userProfile.ToString() );
            return View(userProfile);
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

        public ActionResult Pickups()
        {
            if (User.IsInRole("Customer"))
            {
                return HttpNotFound();
            }

            //get all addresses that have a start date and are in the user's zipcodes
            int profileId = Convert.ToInt32(User.Identity.GetProfileId());
            var profile = db.Profiles.Find(profileId);

            string[] employeeZipcodes = profile.ZipCodes.Split(Convert.ToChar(","));

            var addresses = db.Addresses
                .Include(a => a.TrashCollection)
                .Where( a => employeeZipcodes.Contains(a.ZipCode.Number) )
                .Where( a => a.TrashCollection.StartDate <= DateTime.Now )
                .ToList();

            //remove addresses from the list where the customer is on vacay
            for (int i=0; i<addresses.Count; i++)
            {
                if (addresses[i].TrashCollection.VacationStartDate != null)
                {
                    DateTime vacationStart = (DateTime)addresses[i].TrashCollection.VacationStartDate;
                    DateTime vacationEnd = (DateTime)addresses[i].TrashCollection.VacationEndDate;
                    if (vacationStart < DateTime.Now)
                    {
                        if (vacationEnd > DateTime.Now)
                        {
                            addresses.RemoveAt(i);
                        }
                    }
                }
            }

            return View(addresses);
        }
    }
}
