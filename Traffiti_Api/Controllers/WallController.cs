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
    public class WallController : ApiController
    {
        // GET api/wall
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/wall/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/wall
        public void Post([FromBody]string value)
        {
        }

        // PUT api/wall/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/wall/5
        public void Delete(int id)
        {
        }

        [HttpPost]
        public List<Wall> GetWall(WallComing page)
        {
            CommonController cc = new CommonController();
            List<Wall> wList = new List<Wall>();
             MySqlConnection cn = new MySqlConnection(ConfigurationManager.ConnectionStrings["sq_traffiti"].ConnectionString);
            try
            {
                int pageSize = 20;
                int offset = (page.pageNum - 1) * pageSize;
                cn.Open();
                MySqlCommand cmd = new MySqlCommand(@"select s.wall_id, s.content, s.photo_path, s.author_id, a.author_name,  a.profile_pic, 
                                                l.location_en as location, s.created_date, w.fav_count, w.like_count
                                                from wall_snap s, author a, location l, wall w
                                                where s.author_id = a.author_id and s.location_id = l.location_id
                                                and s.wall_id = w.wall_id
                                                order by s.created_date desc limit @pageSize offset @offset", cn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@pageSize", MySqlDbType.Int32).Value = pageSize;
                cmd.Parameters.Add("@offset", MySqlDbType.Int32).Value = offset;
                cmd.Parameters.Add("@lang_id", MySqlDbType.Int32).Value = page.lang_id;
                MySqlDataAdapter ad = new MySqlDataAdapter(cmd);

                DataSet ds = new DataSet();
                ad.Fill(ds);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Wall w = new Wall();
                    w.wall_id = Convert.ToInt32(dr["wall_id"]);
                    w.content = dr["content"].ToString();
                    w.photo_path = dr["photo_path"].ToString();
                    w.author_id = Convert.ToInt32(dr["author_id"]);
                    w.author_name = dr["author_name"].ToString();
                    w.profile_pic = dr["profile_pic"].ToString();
                    w.location = dr["location"].ToString();
                    w.date_text = cc.CalculateDateTime(Convert.ToDateTime(dr["created_date"]), page.lang_id);
                    w.fav_count = Convert.ToInt32(dr["fav_count"]);
                    w.like_count = Convert.ToInt32(dr["like_count"]);
                    wList.Add(w);
                }

            }
            catch (Exception ex)
            { }
            finally
            {
                cn.Close();
            }
            return wList;
        }

        [HttpPost]
        public WallDetail GetWallDetail(WallComing wc)
        {
            CommonController cc = new CommonController();
            WallDetail wd = new WallDetail();
            MySqlConnection cn = new MySqlConnection(ConfigurationManager.ConnectionStrings["sq_traffiti"].ConnectionString);
            try
            {
                cn.Open();
                MySqlCommand cmd = new MySqlCommand("select photo_path from photo where wall_id = @wall_id order by is_default desc", cn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@wall_id", MySqlDbType.Int32).Value = wc.wall_id;
                DataSet ds = new DataSet();
                MySqlDataAdapter ad = new MySqlDataAdapter(cmd);
                ad.Fill(ds);

                wd.photoList = new List<string>();
                foreach(DataRow dr in ds.Tables[0].Rows)
                {
                    if (dr["photo_path"] != DBNull.Value && !string.IsNullOrEmpty(dr["photo_path"].ToString()))
                    {
                        wd.photoList.Add(dr["photo_path"].ToString());
                    }
                }

                MySqlCommand contentCmd = new MySqlCommand("select content from wall_section where wall_id = @wall_id order by display_order", cn);
                contentCmd.CommandType = CommandType.Text;
                contentCmd.Parameters.Add("@wall_id", MySqlDbType.Int32).Value = wc.wall_id;
                ds = new DataSet();
                ad = new MySqlDataAdapter(contentCmd);
                ad.Fill(ds);
                wd.contentList = new List<string>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    wd.contentList.Add(dr["content"].ToString());
                }

                cmd = new MySqlCommand(@"select w.author_id, w.created_date, w.like_count, w.fav_count, w.location_id, 
                                        a.author_name, a.profile_pic, 
                                        case @lang when 1 then l.location_en when 2 then l.location_tc when 3 then l.location_sc end as location 
                                         from wall w, author a, location l where w.author_id = a.author_id and w.location_id = l.location_id
                                        and w.wall_id = @wall_id", cn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@wall_id", MySqlDbType.Int32).Value = wc.wall_id;
                cmd.Parameters.Add("@lang", MySqlDbType.Int32).Value = wc.lang_id;
                MySqlDataReader detailDr = cmd.ExecuteReader();
                if (detailDr.Read())
                { 
                    wd.wall_id = wc.wall_id;
                    wd.location = detailDr["location"].ToString();
                    wd.author_id = Convert.ToInt32(detailDr["author_id"]);
                    wd.author_name = detailDr["author_name"].ToString();
                    wd.profile_pic = detailDr["profile_pic"].ToString();
                    wd.date_text = cc.CalculateDateTime(Convert.ToDateTime(detailDr["created_date"]), wc.lang_id);
                    wd.fav_count = Convert.ToInt32(detailDr["fav_count"]);
                    wd.like_count = Convert.ToInt32(detailDr["like_count"]);
                }
                detailDr.Close();

            }
            catch (Exception ex)
            { }
            finally
            {
                cn.Close();
            }
            return wd;
        }

        [HttpPost]
        public List<WallDetail> GetProfileWallList(WallComing wc)
        {
            CommonController cc = new CommonController();
            List<WallDetail> wdList = new List<WallDetail>();
            MySqlConnection cn = new MySqlConnection(ConfigurationManager.ConnectionStrings["sq_traffiti"].ConnectionString);
            try
            {
                cn.Open();

               
                MySqlCommand cmd = new MySqlCommand(@"select w.wall_id, w.author_id, w.created_date, w.like_count, w.fav_count, w.location_id, 
                                        a.author_name, a.profile_pic, 
                                        case @lang when 1 then l.location_en when 2 then l.location_tc when 3 then l.location_sc end as location 
                                         from wall w, author a, location l where w.author_id = a.author_id and w.location_id = l.location_id
                                        and w.author_id = @authorID order by w.created_date desc", cn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@wall_id", MySqlDbType.Int32).Value = wc.wall_id;
                cmd.Parameters.Add("@lang", MySqlDbType.Int32).Value = wc.lang_id;
                cmd.Parameters.Add("@authorID", MySqlDbType.Int32).Value = wc.authorID;
                MySqlDataReader detailDr = cmd.ExecuteReader();
                while (detailDr.Read())
                {
                    WallDetail wd = new WallDetail();
                    wd.wall_id = Convert.ToInt32(detailDr["wall_id"]);
                    wd.location = detailDr["location"].ToString();
                    wd.author_id = Convert.ToInt32(detailDr["author_id"]);
                    wd.author_name = detailDr["author_name"].ToString();
                    wd.profile_pic = detailDr["profile_pic"].ToString();
                    wd.date_text = cc.CalculateDateTime(Convert.ToDateTime(detailDr["created_date"]), wc.lang_id);
                    wd.fav_count = Convert.ToInt32(detailDr["fav_count"]);
                    wd.like_count = Convert.ToInt32(detailDr["like_count"]);
                    wdList.Add(wd);
                }
                detailDr.Close();
                
                for (int i = 0; i < wdList.Count; i++)
                {
                    cmd = new MySqlCommand("select photo_path from photo where wall_id = @wall_id order by is_default desc", cn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@wall_id", MySqlDbType.Int32).Value = wdList[i].wall_id;
                    DataSet ds = new DataSet();
                    MySqlDataAdapter ad = new MySqlDataAdapter(cmd);
                    ad.Fill(ds);

                    wdList[i].photoList = new List<string>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (dr["photo_path"] != DBNull.Value && !string.IsNullOrEmpty(dr["photo_path"].ToString()))
                        {
                            wdList[i].photoList.Add(dr["photo_path"].ToString());
                        }
                    }
                }



                //MySqlCommand contentCmd = new MySqlCommand("select content from wall_section where wall_id = @wall_id order by display_order", cn);
                //contentCmd.CommandType = CommandType.Text;
                //contentCmd.Parameters.Add("@wall_id", MySqlDbType.Int32).Value = wc.wall_id;
                //ds = new DataSet();
                //ad = new MySqlDataAdapter(contentCmd);
                //ad.Fill(ds);
                //wd.contentList = new List<string>();
                //foreach (DataRow dr in ds.Tables[0].Rows)
                //{
                //    wd.contentList.Add(dr["content"].ToString());
                //}



            }
            catch (Exception ex)
            { }
            finally
            {
                cn.Close();
            }
            return wdList;
        }

        [HttpPost]
        public void SaveTempPhoto(ComingTempPhoto tempPhoto)
        {
            MySqlConnection cn = new MySqlConnection(ConfigurationManager.ConnectionStrings["sq_traffiti"].ConnectionString);
            try
            {
                cn.Open();
                MySqlCommand cmd = new MySqlCommand("insert into temp_photo (author_id, photo_path) values (@author_id, @photo_path)", cn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@author_id", MySqlDbType.Int32).Value = tempPhoto.authorID;
                cmd.Parameters.Add("@photo_path", MySqlDbType.VarChar).Value = tempPhoto.photoPath;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            { }
            finally
            {
                cn.Close();
            }
        }

        [HttpPost]
        public List<TempPhoto> GetTempPhoto(ComingTempPhoto tempPhoto)
        {
            List<TempPhoto> tempPhotoList = new List<TempPhoto>();
            MySqlConnection cn = new MySqlConnection(ConfigurationManager.ConnectionStrings["sq_traffiti"].ConnectionString);
            try
            {
                cn.Open();
                MySqlCommand cmd = new MySqlCommand("select id, photo_path from temp_photo where author_id = @author_id", cn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@author_id", MySqlDbType.Int32).Value = tempPhoto.authorID;
                MySqlDataAdapter ad = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                ad.Fill(ds);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    TempPhoto tp = new TempPhoto();
                    tp.id = Convert.ToInt32(dr["id"]);
                    tp.photo_path = dr["photo_path"].ToString();
                    tempPhotoList.Add(tp);
                }
            }
            catch (Exception ex)
            { }
            finally
            {
                cn.Close();
            }
            return tempPhotoList;
        }

        [HttpPost]
        public void DeleteTempPhoto(ComingTempPhoto tempPhoto)
        {
            List<TempPhoto> tempPhotoList = new List<TempPhoto>();
            MySqlConnection cn = new MySqlConnection(ConfigurationManager.ConnectionStrings["sq_traffiti"].ConnectionString);
            try
            {
                cn.Open();
                MySqlCommand cmd = new MySqlCommand("delete from temp_photo where id = @id", cn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = tempPhoto.id;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            { }
            finally
            {
                cn.Close();
            }
        }
    }
}
