using Bloggsystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Bloggsystem.Controllers
{
    public class EntryController : Controller
    {

        BlogContext db = new BlogContext();

        // GET: Entries
        public ActionResult Index(string sortorder, string currentfilter, string searchstring, int page = 0)
        {

            string user = (string)Session["user"];
            ViewBag.user = user;

            if (user != null)
            {

                IEnumerable<Entry> items = new DefaultController().searchEntries(sortorder, currentfilter, searchstring);

                // Number of items to be shown per page:
                int totalpages = 50;
                // Fetch total numbers of items in db:
                var rowcount = items.Count();
                // Read limited number of items per page and enable navigation to next page:
                var entriesperpage = items.Skip(page * totalpages).Take(totalpages).ToList();
                int entrymaxpage = (rowcount / totalpages) - (rowcount % totalpages == 0 ? 1 : 0);

                if (page > entrymaxpage)
                    page = entrymaxpage;

                ViewBag.maxpage = entrymaxpage;
                ViewBag.page = page;

                return View(entriesperpage.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Default");
            }

        }


        /// <summary>
        /// Show entry details based on EF.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {

            string user = (string)Session["user"];
            ViewBag.user = user;

            if (user != null)
            {
                ViewBag.entryID = id;

                if (id == null)
                    return RedirectToAction("Index");

                Entry e = db.Entries.Find(id);

                // Filter out comments for the specific entry:
                var comments = from c in db.Comments where c.entryID == id orderby c.date descending select c;
                ViewBag.comments = comments;

                if (e == null)
                    return HttpNotFound();

                return View(e);
            }
            else
            {
                return RedirectToAction("Index", "Default");
            }

        }


        /// <summary>
        /// If logged in, get form to create an entry.
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            string user = (string)Session["user"];
            ViewBag.user = user;

            if (user != null)
            {
                populateCatDropdownlist();
                populateUserDropdownlist();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Default");
            }

        }


        /// <summary>
        /// Create an entry based on EF and bind with related entity.
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create([Bind(Include = "entryID,title,date,keywords,description,entry1,categoryID,userID")] Entry e)
        {

            string user = (string)Session["user"];
            ViewBag.user = user;

            if (ModelState.IsValid)
            {
                try
                {

                    // Set default date to the entry:
                    e.date = DateTime.Now;

                    db.Entries.Add(e);
                    db.SaveChanges();

                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    ViewBag.Msg = ex.Message;
                }
            }

            // Call method to repopulate list including binded attributes:
            populateCatDropdownlist(e.categoryID);
            populateUserDropdownlist(e.userID);

            return View(e);
        }


        /// <summary>
        /// Edit an entry.
        /// </summary>
        /// <param name="cat"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {

            string user = (string)Session["user"];
            ViewBag.user = user;

            if (user != null)
            {
                Entry e = db.Entries.Find(id);

                populateCatDropdownlist(e.categoryID);
                populateUserDropdownlist(e.userID);

                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                if (e == null)
                {
                    return HttpNotFound();
                }

                return View(e);
            }
            else
            {
                return RedirectToAction("Index", "Default");
            }

        }


        /// <summary>
        /// Edit an entry based on EF and bind with related entity.
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit([Bind(Include = "entryID,title,date,keywords,description,entry1,categoryID,userID")] Entry e)
        {

            string user = (string)Session["user"];
            ViewBag.user = user;

            try
            {
                if (ModelState.IsValid && e.date != null)
                {
                    db.Entry(e).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.errordate = "Vänligen ange ett giltigt datum (yyyy-mm-dd).";
                }

                populateCatDropdownlist(e.categoryID);
                populateUserDropdownlist(e.userID);
                return View(e);
            }
            catch
            {
                populateCatDropdownlist(e.categoryID);
                populateUserDropdownlist(e.userID);
                return View(e);
            }

        }


        /// <summary>
        /// If logged in, get form to delete an entry.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Delete()
        {
            string user = (string)Session["user"];
            ViewBag.user = user;

            if (user != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Default");
            }

        }


        /// <summary>
        /// Delete an entry based on EF.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int? id)
        {

            string user = (string)Session["user"];
            ViewBag.user = user;

            try
            {
                Entry e = new Models.Entry();

                if (ModelState.IsValid)
                {
                    if (id == null)
                        return RedirectToAction("Index");

                    e = db.Entries.Find(id);

                    if (e == null)
                        return HttpNotFound();

                    db.Entries.Remove(e);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(e);
            }
            catch
            {
                return View();
            }

        }


        /// <summary>
        /// Method to repopulate list, including category, to be able to create and edit a movie.
        /// </summary>
        /// <param name="selectedDirector"></param>
        private void populateCatDropdownlist(object selectedCategory = null)
        {
            var entriesquery = from e in db.Categories
                               orderby e.category1
                               select e;

            ViewBag.categoryID = new SelectList(entriesquery, "categoryID", "category1", selectedCategory);
        }

        /// <summary>
        /// Method to repopulate list, including username, to be able to create and edit a movie.
        /// </summary>
        /// <param name="selectedDirector"></param>
        private void populateUserDropdownlist(object selectedUser = null)
        {
            var entriesquery = from e in db.Users
                               orderby e.firstname
                               select e;

            ViewBag.userID = new SelectList(entriesquery, "userID", "username", selectedUser);
        }

    }
}