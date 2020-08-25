using Bloggsystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Bloggsystem.Controllers
{
    public class DefaultController : Controller
    {

        BlogContext db = new BlogContext();

        // GET: Default
        public ActionResult Index(int page = 0)
        {

            string user = (string)Session["user"];
            ViewBag.user = user;

            // Number of items to be shown per page:
            int totalpages = 10;
            var items = from e in db.Entries orderby e.date descending select e;
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


        /// <summary>
        /// Handle the view to show a specific entry with belonging comments.
        /// </summary>
        /// <returns></returns>
        public ActionResult EntryComments(int? id)
        {

            string user = (string)Session["user"];
            ViewBag.user = user;

            // Fetch view id (entryID) into a session to be catched and used later in populate-method:
            int eID = (int)id;
            string eID2 = Convert.ToString(eID);
            Session["eID"] = eID2;

            // Populate-method needs to be called here to keep model state valid:
            populateEntryDropdownlist();

            if (id == null)
            {
                return RedirectToAction("Index");
            }

            // Filter out the specific entry with belonging comments:
            var entryquery = from e in db.Entries where eID == e.entryID select e;
            var commentquery = from c in db.Comments where c.entryID == eID orderby c.date descending select c;

            // Check number of comments to print alternative text for an entry that has no comments:
            var countexistingcomments = commentquery.Count();
            ViewBag.existingcomments = countexistingcomments;

            // Loop through entries and categories to get wanted metadata for the specific entry:
            foreach (var entry in db.Entries.ToList())
            {

                if (entry.entryID == eID)
                {

                    // Declare viewbags to reach metadata in view:
                    ViewBag.dctitle = entry.title;
                    string date = entry.date.ToString();
                    ViewBag.dcdate = date.Substring(0, 10);
                    ViewBag.dckeywords = entry.keywords;
                    ViewBag.dcdescription = entry.description;

                    // Fetch username for the entry:
                    foreach (var u in db.Users.ToList())
                    {
                        if (entry.userID == u.userID)
                            ViewBag.username = u.username;
                    }

                    // Fetch category name for the entry:
                    foreach (var cat in db.Categories.ToList())
                    {
                        if (entry.categoryID == cat.categoryID)
                            ViewBag.dccategory = cat.category1;
                    }

                }

            }

            // Create sessions with entries and comments to be catched in post view:
            Session["entries"] = entryquery;
            Session["comments"] = commentquery;

            // Declare viewbags to be able to loop out the filtered entry with belonging comments in view:
            ViewBag.entries = entryquery;
            ViewBag.comments = commentquery;

            // Catch feedback after a comment has been posted:
            ViewBag.successmsg = TempData["successmsg"];

            return View();

        }


        /// <summary>
        /// Handle the view to be able to post a new comment. Also, show details for a specific entry with belonging comments.
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EntryComments([Bind(Include = "commentID,name,mail,date,website,comment1,entryID")] Comment item)
        {

            string user = (string)Session["user"];
            ViewBag.user = user;

            // Set current date as default value to the item's date attribute:
            item.date = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    db.Comments.Add(item);
                    db.SaveChanges();

                    // Call method to repopulate list including binded attribute to keep model state valid:
                    populateEntryDropdownlist(item.entryID);

                    TempData["successmsg"] = "Tack för din kommentar!";

                    return RedirectToAction("EntryComments", new { @id = item.entryID });
                }
                catch
                {
                    ViewBag.errormsg = "Något blev fel, försök igen!";
                }

            }

            // The repopulation is necessary here too:
            populateEntryDropdownlist(item.entryID);

            // Resume sessions to prevent NullReferenceException:
            ViewBag.entries = Session["entries"];
            ViewBag.comments = Session["comments"];

            return View(item);

        }


        /// <summary>
        /// Action method to handle the search view to be able to search and filter blog entries.
        /// </summary>
        /// <param name="sortorder"></param>
        /// <param name="currentfilter"></param>
        /// <param name="searchstring"></param>
        /// <returns></returns>
        public ActionResult Search(string sortorder, string currentfilter, string searchstring, int page = 0)
        {

            string user = (string)Session["user"];
            ViewBag.user = user;

            IEnumerable<Entry> entries = searchEntries(sortorder, currentfilter, searchstring);

            // Number of items to be shown per page:
            int totalpages = 10;
            // Fetch total numbers of items in db:
            var rowcount = entries.Count();
            // Read limited number of items per page and enable navigation to next page:
            var entriesperpage = entries.Skip(page * totalpages).Take(totalpages).ToList();
            int entrymaxpage = (rowcount / totalpages) - (rowcount % totalpages == 0 ? 1 : 0);

            if (page > entrymaxpage)
                page = entrymaxpage;

            ViewBag.maxpage = entrymaxpage;
            ViewBag.page = page;

            // Return search result as listed entries to the view:
            return View(entriesperpage.ToList());

        }


        /// <summary>
        /// Get form to send new password to user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SendPassword()
        {
            return View();
        }


        /// <summary>
        /// Post form to send new password to user.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SendPassword(FormCollection fc)
        {

            string username = "";
            string usermail = "";
            // Fetch username and mail for user:
            foreach (var user in db.Users.ToList())
            {
                username = user.username;
                usermail = user.mail;
            }

            MailMessage mail = new MailMessage();

            // Mail receiver address:
            mail.To.Add(usermail);
            string sender = ConfigurationManager.AppSettings["MailSender"].ToString();
            // Mail will be sent from:
            mail.From = new MailAddress(sender, "Webbansvarig för bloggsystem", System.Text.Encoding.UTF8);
            mail.Subject = "Nytt lösenord!";
            // Encode to UTF8 to make sure that Swedish letters are read properly:
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            string newpassword = new Models.BL().randomString();
            mail.Body = "Ditt nya lösenord: <b>" + newpassword + "</b>";
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            // Send the mail through a specific mail account based on SMTP properties below:
            SmtpClient client = new SmtpClient();
            // Fetch password from webconfig for the used account:
            string password = ConfigurationManager.AppSettings["MailSenderPass"].ToString();
            string port = ConfigurationManager.AppSettings["MailSenderPort"].ToString();
            string host = ConfigurationManager.AppSettings["MailSenderHost"].ToString();
            client.Credentials = new System.Net.NetworkCredential(sender, password);
            client.Port = Convert.ToInt32(port);
            client.Host = host;
            client.EnableSsl = true;

            try
            {
                // Catch form input:
                string inputname = fc["userinput"];

                if (username == inputname)
                {
                    // Update user table with new password:
                    new Models.PassThrough().updatePassword(newpassword, username);
                    // Send new password to user mail:
                    client.Send(mail);
                    ViewBag.successmsg = "Ett nytt lösenord har nu skickats till den mejladress som är kopplad till ditt användarnamn!<br>"
                        + "(Obs! Om lösenordet inte skickats till din inkorg så kontrollera din skräppost.)";
                    return View();
                }
                else
                {
                    ViewBag.errormsg = "Användarnamnet kunde ej hittas!";
                    return View();
                }

            }
            catch (Exception e)
            {
                ViewBag.errormsg = e.Message;
                return View();
            }

        }


        /// <summary>
        /// Get login form.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        /// <summary>
        /// Post login form.
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {
            try
            {
                // Fetch input values from loginform in view:
                string user = fc["username"];
                string pass = fc["password"];

                // Call model method to check if login is ok:
                if (new Models.PassThrough().checkLogin(user, pass))
                {
                    Session["user"] = user;

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["loginfail"] = "Felaktiga inloggningsuppgifter!";
                    return RedirectToAction("Index");
                }

            }
            catch (FormatException)
            {
                return RedirectToAction("Index");
            }

        }


        /// <summary>
        /// Clear all sessions when logging out.
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }


        /// <summary>
        /// Method that handles a search and filter function applied on entries.
        /// </summary>
        /// <param name="sortorder"></param>
        /// <param name="currentfilter"></param>
        /// <param name="searchstring"></param>
        public IEnumerable<Entry> searchEntries(string sortorder, string currentfilter, string searchstring)
        {
            ViewBag.sorttitle = String.IsNullOrEmpty(sortorder) ? "title_desc" : "";
            ViewBag.sortcategory = sortorder == "category" ? "category_desc" : "category";
            ViewBag.sortdate = sortorder == "date" ? "date_desc" : "date";

            // Fetch entries ordered by entry titles:
            var dbEntry = db.Entries.AsEnumerable().OrderBy(s => s.title, StringComparer.CurrentCulture);
            var entries = from s in dbEntry select s;

            if (searchstring == null)
                searchstring = currentfilter;

            // Variable to reach search string in view:
            ViewBag.currentfilter = searchstring;

            if (!String.IsNullOrEmpty(searchstring))
            {
                // Check if db contains the search word applied on title, entry and category:
                entries = entries.Where(s => s.title.ToUpper().Contains(searchstring.ToUpper()) ||
                Convert.ToString(s.Category.category1.ToUpper()).Contains(searchstring.ToUpper()) ||
                Convert.ToString(s.entry1.ToUpper()).Contains(searchstring.ToUpper()));
            }

            // Sort result attributes in descending order:
            switch (sortorder)
            {
                case "title_desc":
                    entries = entries.OrderByDescending(s => s.title);
                    break;
                case "category":
                    entries = entries.OrderBy(s => s.Category.category1);
                    break;
                case "category_desc":
                    entries = entries.OrderByDescending(s => s.Category.category1);
                    break;
                case "date":
                    entries = entries.OrderBy(s => s.date);
                    break;
                case "date_desc":
                    entries = entries.OrderByDescending(s => s.date);
                    break;
                default:
                    entries = entries.OrderByDescending(s => s.date);
                    break;
            }

            return entries.ToList();
        }


        /// <summary>
        /// Method to repopulate a list, including entryID, to be able to create and edit a comment.
        /// </summary>
        /// <param name="selectedDirector"></param>
        private void populateEntryDropdownlist(object selectedEntry = null)
        {
            string id = (string)Session["eID"];
            int id2 = Convert.ToInt32(id);

            // Make sure that a new comment only will be posted for the chosen entry (entryID):
            var query = from e in db.Entries where id2 == e.entryID orderby e.entryID select e;

            ViewBag.entryID = new SelectList(query, "entryID", "entryID", selectedEntry);
        }

    }
}