using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        // Yang namanya sama kayak class, dinamakan constructor
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        //Buat get
        public ActionResult Add()
        {
            PopulateSelectedList();
            return View();

            //var entry = new Entry()
            //{
            //    Date = DateTime.Today
            //};
            //return View(entry);
        }
                
        //Buat post
        //[ActionName("Add"), HttpPost] // *
        [HttpPost] // atribut buat menangkap atribut post
        //public ActionResult AddSave() // Pasangannya yang *
        //public ActionResult Add(DateTime? date, int? activityId, double? duration, 
        //    Entry.IntensityLevel? intensity, bool? exclude, string notes) // ? memperbolehkan null, diganti ke bwah
        public ActionResult Add(Entry entry)
        {
            //string date = Request.Form["Date"]; // *

            ////salah satu cara biar ketika di save gak langsung hilang. Nantinya dipake buat validasi
            //ViewBag.Date = date;
            //ViewBag.Activity = activityId;
            //ViewBag.Duration = duration;
            //ViewBag.Intensity = intensity;
            //ViewBag.Exclude = exclude;
            //ViewBag.Notes = notes;

            if(ModelState.IsValid) //
            {
                _entriesRepository.AddEntry(entry);
                return RedirectToAction("Index");
            }

            PopulateSelectedList();
            return View(entry);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Entry entry = _entriesRepository.GetEntry((int)id);

            PopulateSelectedList();

            return View(entry);
        }

        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            if (ModelState.IsValid) //
            {
                _entriesRepository.UpdateEntry(entry);
                return RedirectToAction("Index");
            }

            PopulateSelectedList();
            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Entry entry = _entriesRepository.GetEntry((int)id);            

            return View(entry);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _entriesRepository.DeleteEntry(id);
            return RedirectToAction("Index");
        }

        private void PopulateSelectedList()
        {
            ViewBag.SelectListItem = new SelectList(Data.Data.Activities, "Id", "Name");
        }
    }
}