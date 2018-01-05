using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TrashCollector.Models;
using TrashCollector.Extensions;


namespace TrashCollector.Controllers
{
    public class AddressController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Address
        public ActionResult Index()
        {
            var addresses = db.Addresses.Include(a => a.City).Include(a => a.State).Include(a => a.ZipCode);
            return View(addresses.ToList());
        }

        // GET: Address/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = db.Addresses.Find(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        // GET: Address/Create
        public ActionResult Create()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.StateId = new SelectList(db.States, "StateId", "Abbreviation");
            return View();
        }

        public ActionResult DuplicateAddress()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: Address/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "AddressId,StreetOne,StreetTwo,City,State,ZipCode")] Address address)
        public ActionResult Create(string StreetOne, string StreetTwo, string City_Name, string StateId, string ZipCode_Number)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                Address address = GetAddress(StreetOne, StreetTwo, City_Name, StateId, ZipCode_Number);
                if(address.AddressId == 0)
                {
                    return RedirectToAction("DuplicateAddress","Address");
                }

                int profileId = Convert.ToInt32(User.Identity.GetProfileId());
                var profile = db.Profiles.First(p=> p.ProfileId == profileId);
                if ( profile.Addresses == null )
                {
                    profile.Addresses = new List<Address>();
                }
                profile.Addresses.Add(address);
                db.SaveChanges();
                return RedirectToAction("Addresses", "Profile");
            }

            ViewBag.State = new SelectList(db.States, "StateId", "Abbreviation");
            return View();
        }

        private Address GetAddress(string StreetOne, string StreetTwo, string City_Name, string StateId, string ZipCode_Number)
        {
            int stateIdNumber = Convert.ToInt32(StateId);
            if (db.Addresses.Any(a => a.StreetOne == StreetOne && a.StreetTwo == StreetTwo && a.City.Name == City_Name && a.State.StateId == stateIdNumber && a.ZipCode.Number == ZipCode_Number))
            {
                var addressFound = db.Addresses.First(a => a.StreetOne == StreetOne && a.StreetTwo == StreetTwo && a.City.Name == City_Name && a.State.StateId == stateIdNumber && a.ZipCode.Number == ZipCode_Number);
                return new Address();
            }
            int trashCollectionId = CreateEmptyTrashCollection();
            Address address = null;
            address = new Address();
            address.StreetOne = StreetOne;
            address.StreetTwo = StreetTwo;
            address.CityId = GetCityID(City_Name);
            address.StateId = GetStateID(StateId);
            address.ZipCodeId = GetZipCodeID(ZipCode_Number);
            address.TrashCollectionId = trashCollectionId;
            db.Addresses.Add(address);
            db.SaveChanges();
            
            return address;
        }

        int CreateEmptyTrashCollection()
        {
            TrashCollection trashCollection = new TrashCollection();
            ApplicationDbContext db = new ApplicationDbContext();
            db.TrashCollections.Add(trashCollection);
            return (db.SaveChanges());
        }

        private int GetStateID(string StateId)
        {
            int stateIdNumber = Convert.ToInt32(StateId);
            var stateFound = db.States.First(s => s.StateId == stateIdNumber);
            return stateFound.StateId;
        }

        private int GetZipCodeID(string ZipCode_Number)
        {
            if (db.ZipCodes.Any(z => z.Number == ZipCode_Number))
            {
                var zipCodeFound = db.ZipCodes.First(z => z.Number == ZipCode_Number);
                return zipCodeFound.ZipCodeId;
            }
            ZipCode zipCode = new ZipCode();
            zipCode.Number = ZipCode_Number;
            db.ZipCodes.Add(zipCode);
            return db.SaveChanges();
        }

        private int GetCityID(string City_Name)
        {
            if (db.Cities.Any(c => c.Name.ToLower() == City_Name.ToLower()))
            {
                var cityFound = db.Cities.First(c => c.Name == City_Name);
                return cityFound.CityId;
            }
            City city = new City();
            string formattedName = char.ToUpper(City_Name[0]) + City_Name.Substring(1).ToLower();
            city.Name = formattedName;
            db.Cities.Add(city);
            return db.SaveChanges();
        }

        // GET: Address/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = db.Addresses.Find(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", address.CityId);
            ViewBag.StateId = new SelectList(db.States, "StateId", "Name", address.StateId);
            ViewBag.ZipCodeId = new SelectList(db.ZipCodes, "ZipCodeId", "Number", address.ZipCodeId);
            return View(address);
        }

        // POST: Address/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AddressId,StreetOne,StreetTwo,CityId,StateId,ZipCodeId")] Address address)
        {
            if (ModelState.IsValid)
            {
                db.Entry(address).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", address.CityId);
            ViewBag.StateId = new SelectList(db.States, "StateId", "Name", address.StateId);
            ViewBag.ZipCodeId = new SelectList(db.ZipCodes, "ZipCodeId", "Number", address.ZipCodeId);
            return View(address);
        }

        // GET: Address/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = db.Addresses.Find(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        // POST: Address/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Address address = db.Addresses.Find(id);
            db.Addresses.Remove(address);
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

        public ActionResult ScheduleCollection(int addressId)
        {
            if ( !(addressId > 0) )
            {
                return HttpNotFound();
            }
            var address = db.Addresses.Include(a => a.City).Include(a => a.State).Include(a => a.ZipCode).First(a => a.AddressId == addressId);
            return View(address);
        }

        public ActionResult EditCollection(int addressId)
        {
            return Content("Edit a trash collection! Address Id: " + addressId);
            //return View();
        }

    }
}
