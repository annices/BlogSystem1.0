using Bloggsystem.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Bloggsystem.Controllers
{
    public class CategoryController : Controller
    {

        BlogContext db = new BlogContext();

        // GET: Categories
        public ActionResult Index()
        {

            string user = (string)Session["user"];
            ViewBag.user = user;

            if (user != null)
            {
                // Filter out categories in alphabetic order:
                var categories = from c in db.Categories orderby c.category1 select c;

                return View(categories);
            }
            else
            {
                return RedirectToAction("Index", "Default");
            }

        }


        /// <summary>
        /// If logged in, get form to create a category.
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
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
        /// Create a category based on EF.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Category item)
        {

            string user = (string)Session["user"];
            ViewBag.user = user;

            if (ModelState.IsValid)
            {
                try
                {
                    db.Categories.Add(item);
                    db.SaveChanges();

                    return RedirectToAction("Index");

                }
                catch (Exception)
                {
                    ViewBag.msg = "Kategorin kunde ej skapas, kontrollera att det inte redan existerar!";
                }
            }

            return View(item);
        }


        /// <summary>
        /// Edit a category based on EF.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            string user = (string)Session["user"];
            ViewBag.user = user;

            if (user != null)
            {
                Category item = db.Categories.Find(id);

                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                if (item == null)
                {
                    return HttpNotFound();
                }

                return View(item);
            }
            else
            {
                return RedirectToAction("Index", "Default");
            }

        }


        /// <summary>
        /// Edit a category based on EF.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Category item)
        {

            string user = (string)Session["user"];
            ViewBag.user = user;

            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(item);
            }
            catch
            {
                return View(item);
            }

        }


        /// <summary>
        /// If logged in, get form to delete a category.
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
        /// Delete a category based on EF.
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
                Category item = new Models.Category();

                if (ModelState.IsValid)
                {
                    if (id == null)
                        return RedirectToAction("Index");

                    item = db.Categories.Find(id);

                    if (item == null)
                        return HttpNotFound();

                    db.Categories.Remove(item);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(item);
            }
            catch
            {
                return View();
            }
        }

    }
}