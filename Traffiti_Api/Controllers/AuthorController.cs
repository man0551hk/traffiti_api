using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Traffiti_Api.Models;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace Traffiti_Api.Controllers
{
    public class AuthorController : ApiController
    {
        [HttpPost]
        public Author AuthorLogin(ComingAuthor incomeAuthor)
        {
            Author author = new Author();
            MySqlConnection cn = new MySqlConnection(ConfigurationManager.ConnectionStrings["sq_traffiti"].ConnectionString);
            try
            {
                string sql = @"select author_id, author_name, email, created_date, facebook_id, google_id, profile_pic, cover_pic, access_key, fans, follows from author where ";
                if (!string.IsNullOrEmpty(incomeAuthor.authorLogin) && !string.IsNullOrEmpty(incomeAuthor.authorPassword))
                {
                    sql += "author_login = @login and author_password = @password";
                }
                else if (incomeAuthor.facebookID > 0)
                {
                    sql += "facebook_id = @facebookID";
                }
                else if (incomeAuthor.googleID > 0)
                {
                    sql += "google_id = @googleID";
                }
                else if (!string.IsNullOrEmpty(incomeAuthor.accessKey))
                {
                    sql += "access_key = @accessKey";
                }
                cn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, cn);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(incomeAuthor.authorLogin) && !string.IsNullOrEmpty(incomeAuthor.authorPassword))
                {
                    cmd.Parameters.Add("@login", MySqlDbType.VarChar).Value = incomeAuthor.authorLogin;
                    cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = incomeAuthor.authorPassword;
                }
                else if (incomeAuthor.facebookID > 0)
                {
                    cmd.Parameters.Add("@facebookID", MySqlDbType.Int32).Value = incomeAuthor.facebookID;
                }
                else if (incomeAuthor.googleID > 0)
                {
                    cmd.Parameters.Add("@goolgeID", MySqlDbType.Int32).Value = incomeAuthor.googleID;
                }
                else if (!string.IsNullOrEmpty(incomeAuthor.accessKey))
                {
                    cmd.Parameters.Add("@accessKey", MySqlDbType.VarChar).Value = incomeAuthor.accessKey;
                }

                MySqlDataAdapter ad = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                ad.Fill(ds);
                string accessKey = RandomAccessKey();
                if (ds.Tables[0].Rows.Count == 1)
                { 
                    DataRow dr = ds.Tables[0].Rows[0];
                    author.author_id = Convert.ToInt32(dr["author_id"]);
                    author.author_name = dr["author_name"].ToString();
                    author.email = dr["email"].ToString();
                    author.created_date = DateTime.Parse(dr["created_date"].ToString());
                    author.facebook_id = Convert.ToInt32(dr["facebook_id"]);
                    author.google_id = Convert.ToInt32(dr["google_id"]);
                    author.profile_pic = dr["profile_pic"].ToString();
                    author.cover_pic = dr["cover_pic"].ToString();
                    author.access_key = dr["access_key"].ToString();
                    author.fans = Convert.ToInt32(dr["fans"]);
                    author.follows = Convert.ToInt32(dr["follows"]);
                }

                if (author.author_id > 0 && author.access_key == "")
                {
                    sql = "update author set access_key = @accessKey where author_id = @author_id";
                    MySqlCommand uCmd = new MySqlCommand(sql, cn);
                    uCmd.CommandType = CommandType.Text;
                    uCmd.Parameters.Add("@accessKey", MySqlDbType.VarChar).Value = author.author_id + accessKey;
                    uCmd.Parameters.Add("@author_id", MySqlDbType.Int32).Value = author.author_id;
                    uCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            { }
            finally
            {
                cn.Close();
            }
            return author;
        }

        private string RandomAccessKey()
        {
            string accessKey = "";
            Random rnd = new Random();
            string[] chars = new string[]{"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", 
                "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
            for (int i = 0; i < 16; i++)
            {
                if (rnd.Next(1, 9) % 2 == 0)
                {
                    accessKey += chars[rnd.Next(0, 25)];
                }
                else
                {
                    accessKey += rnd.Next(0, 9).ToString();
                }
            }
            return accessKey;
        }
    }
}
