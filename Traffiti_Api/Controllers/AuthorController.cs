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
                MySqlCommand cmd = new MySqlCommand(@"select author_id, author_name, email, created_date, facebook_id, google_id, profile_pic 
                                    from author where author_login = @login and author_password = @password", cn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@login", MySqlDbType.VarChar).Value = incomeAuthor.author_login;
                cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = incomeAuthor.author_password;
                MySqlDataAdapter ad = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                ad.Fill(ds);
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
    }
}
