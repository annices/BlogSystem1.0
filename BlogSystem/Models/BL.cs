using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Bloggsystem.Models
{
    public class BL
    {

        BlogContext db = new BlogContext();

        /// <summary>
        /// Method that clears HTML tags and cuts a string if length transcends 50 characters.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public string clearHtmlAndCutString(string source)
        {

            if (!String.IsNullOrEmpty(source) && source.Length > 50)
            {
                // Cut string:
                source = source.Substring(0, 50) + "...";
                // Clear HTML-tags:
                string clearhtml = Regex.Replace(source, "<.*?>", string.Empty);
                // Keep Swedish letters:
                return HttpUtility.HtmlDecode(clearhtml);
            }
            else
            {
                string clearhtml = Regex.Replace(source, "<.*?>", string.Empty);
                return HttpUtility.HtmlDecode(clearhtml);
            }

        }


        /// <summary>
        /// Method to select a random string, used to randomize a new password.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string randomString()
        {
            // Determine length of randomized string:
            int length = 10;
            Random random = new Random();
            // Characters to pick from:
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(characters[random.Next(characters.Length)]);
            }
            // Return randomized string:
            return result.ToString();
        }

    }
}
