using System;
using Bloggsystem.Technique;

namespace Bloggsystem.Models
{
    public class PassThrough
    {

        /// <summary>
        /// Method to pass username and password from controllers to technique layer to check user login.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public Boolean checkLogin(string username, string password)
        {
            return new DBAccess().checkLogin(username, password);
        }


        /// <summary>
        /// Method to pass username and password from controllers to technique layer to update a user password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public void updatePassword(string password, string username)
        {
            new DBAccess().updatePassword(password, username);
        }

    }
}