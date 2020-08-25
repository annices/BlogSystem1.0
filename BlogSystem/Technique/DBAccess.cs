using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Bloggsystem.Technique
{
    public class DBAccess
    {

        // Use a connection string based on webconfig:
        public string conn = ConfigurationManager.ConnectionStrings["TechniqueContext"].ConnectionString;


        /// <summary>
        /// Get values from user table and check if there's a user with matching username and password.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public Boolean checkLogin(string user, string pass)
        {

            SqlConnection dbconn = new SqlConnection(conn);

            // Check if username matches a password in members table through a sql query:
            string sql = "SELECT username, password FROM Users WHERE username='"
                + user + "' AND password=HASHBYTES('md5', '" + pass + "')";

            SqlCommand dbcommand = new SqlCommand(sql, dbconn);

            dbconn.Open();
            SqlDataReader reader = dbcommand.ExecuteReader();

            int count = 0;

            // Loop through table rows:
            while (reader.Read())
            {
                count = count + 1;
            }

            // If there's a matching row we have a valid login:
            if (count == 1)
            {
                return true;
            }

            dbconn.Close();
            return false;

        }


        /// <summary>
        /// Update user table with a new password.
        /// </summary>
        /// <param name="newpassword"></param>
        /// <param name="mail"></param>
        public void updatePassword(string newpassword, string username)
        {

            // Update user table with a new, hashed password:
            string sql = "UPDATE Users SET password=HASHBYTES('md5', '" + newpassword + "') WHERE username='" + username+"'";

            SqlConnection dbconn = new SqlConnection(conn);

            dbconn.Open();

            SqlCommand dbcommand = new SqlCommand(sql, dbconn);
            dbcommand.ExecuteNonQuery();

            dbconn.Close();

        }

    }
}