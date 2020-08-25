using Bloggsystem.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Bloggsystem.Controllers
{
    public class UserController : Controller
    {

        BlogContext db = new BlogContext();

        /// <summary>
        /// Edit a user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {

            string user = (string)Session["user"];
            ViewBag.user = user;

            if (user != null)
            {
                User item = db.Users.Find(id);

                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                if (item == null)
                {
                    return HttpNotFound();
                }

                // Catch tempdata to show feedback if password is successfully changed:
                ViewBag.success = TempData["successpass"];

                return View(item);
            }
            else
            {
                return RedirectToAction("Index", "Default");
            }

        }


        /// <summary>
        /// Edit a user based on EF and bind with related entity.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(User item)
        {

            string user = (string)Session["user"];
            ViewBag.user = user;

            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    ViewBag.successmsg = "Användaruppgifterna är nu uppdaterade!";

                    return View(item);
                }
                else
                {
                    ViewBag.errormsg = "Användaruppgifterna kunde ej uppdateras!";
                    return View(item);
                }
            }
            catch
            {
                return View(item);
            }

        }


        /// <summary>
        /// Get form to change a user password.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdatePassword()
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
        /// Post form to change a user password.
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdatePassword(FormCollection fc)
        {

            string user = (string)Session["user"];
            ViewBag.user = user;

            // Fetch input values from password form in view:
            string newpassword = fc["password2"];
            string username = "";

            if (String.IsNullOrEmpty(newpassword))
            {
                return RedirectToAction("Edit", new { id = 1 });
            }
            else
            {
                try
                {
                    // Fetch existing username:
                    foreach (var u in db.Users.ToList())
                    {
                        username = u.username;
                    }

                    // Update user table with new password:
                    new Models.PassThrough().updatePassword(newpassword, username);
                    TempData["successpass"] = "Lösenordet är nu uppdaterat!";
                    
                    return RedirectToAction("Edit", "User", new { id = 1 });
                }
                catch
                {
                    return RedirectToAction("Edit", "User", new { id = 1 });
                }

            }

        }


    }
}