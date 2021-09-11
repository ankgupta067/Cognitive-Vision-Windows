using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Design;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VisionAPI_WPF_Samples
{
    public static class SqlHelper
    {
        public static SQLiteConnection sqlite_conn { get;  }
        static SqlHelper()
        {
            sqlite_conn = new SQLiteConnection("Data Source=ImageTags.db; Version = 3;");
            sqlite_conn.Open();
        }


        public static Tuple<bool, string> SaveImage(string url , List<Tag> tags)
        {
            if (!ImageExists(url))
            {
                var json = JsonConvert.SerializeObject(tags);
                var sqliteCmd = SqlHelper.sqlite_conn.CreateCommand();
                sqliteCmd.CommandText = $"INSERT INTO Image (Url, Tags) VALUES('{url.Trim()}', '{json}'); ";
                sqliteCmd.ExecuteNonQuery();
                return new Tuple<bool, string>(true, null);
            }
            else
            {
                return new Tuple<bool, string>(false, "Image Already exists");
            }
        }

        public static bool ImageExists(string ImageDataUrl)
        {
            var sqliteCmd = SqlHelper.sqlite_conn.CreateCommand();
            sqliteCmd.CommandText = $"select count(*) from Image WHERE Url = '{ImageDataUrl.Trim()}' ";
            var result = sqliteCmd.ExecuteScalar();
            return  (long)result != 0;
        }

        public static List<ImageData> GetImages(List<string> Tags)
        {
            var sqliteCmd = SqlHelper.sqlite_conn.CreateCommand();
            var tagString = string.Join(",", Tags);
            sqliteCmd.CommandText = $"select * from Image";
            var reader = sqliteCmd.ExecuteReader();
            var result = new List<ImageData>();
            while (reader.Read())
            {
                ImageData ImageData = new ImageData {imageId = reader["Id"].ToString(), imageUrl = reader["Url"].ToString()};

                string jsonArray = reader["Tags"].ToString();
                var taglist = JsonConvert.DeserializeObject<List<Tag>>(jsonArray);
                foreach (var tag in taglist)
                {
                    ImageData.confidenceByTag.Add(tag.TagValue,tag.Confidence);
                }

                ImageData.score = 0;
                result.Add(ImageData);
            }
            reader.Close();

            return result;
        }
    }
}
