using Bloggsystem.Models;
using System.Linq;
using System.Web.Mvc;

namespace Bloggsystem.Controllers
{

    public class CommentController : Controller
    {

        BlogContext db = new BlogContext();

        // GET: Comment
        public ActionResult Index(int page = 0)
        {

            string user = (string)Session["user"];
            ViewBag.user = user;

            if (user != null)
            {
                // Number of items to be shown per page:
                int totalpages = 50;
                var items = from c in db.Comments orderby c.date descending select c;
                // Fetch total numbers of items in db:
                var rowcount = items.Count();
                // Read limited number of items per page and enable navigation to next page:
                var commentsperpage = items.Skip(page * totalpages).Take(totalpages).ToList();
                int commentmaxpage = (rowcount / totalpages) - (rowcount % totalpages == 0 ? 1 : 0);

                if (page > commentmaxpage)
                    page = commentmaxpage;

                ViewBag.maxpage = commentmaxpage;
                ViewBag.page = page;

                return View(commentsperpage.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Default");
            }

        }


        /// <summary>
        /// Edit a comment based on EF.
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

                Comment item = db.Comments.Find(id);

                // Repopulate list with binded attribute to keep model state valid:
                populateEntryDropdownlist(item.entryID);

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
        /// Edit a comment based on EF and bind with related entity.
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit([Bind(Include = "commentID,name,mail,date,website,comment1,answer,entryID")] Comment item)
        {

            string user = (string)Session["user"];
            ViewBag.user = user;

            try
            {
                if (ModelState.IsValid && item.date != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                // Repopulate with binded attribute to keep model state valid:
                populateEntryDropdownlist(item.entryID);
                return View(item);
            }
            catch
            {
                populateEntryDropdownlist(item.entryID);
                return View(item);
            }

        }


        /// <summary>
        /// If logged in, get form to delete a comment.
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
        /// Delete a comment based on EF.
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
                Comment item = new Models.Comment();

                if (ModelState.IsValid)
                {
                    if (id == null)
                        return RedirectToAction("Index");

                    item = db.Comments.Find(id);

                    if (item == null)
                        return HttpNotFound();

                    db.Comments.Remove(item);
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


        /// <summary>
        /// Method to repopulate list, including entryID, to be able to create and edit a comment.
        /// </summary>
        /// <param name="selectedDirector"></param>
        private void populateEntryDropdownlist(object selectedEntry = null)
        {
            var query = from e in db.Entries
                        orderby e.title
                        select e;

            ViewBag.entryID = new SelectList(query, "entryID", "Title", selectedEntry);
        }

    }
}