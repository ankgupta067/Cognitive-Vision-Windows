using System;
using System.Collections.Generic;
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
            sqlite_conn = new SQLiteConnection("Data Source=imageTags.db; Version = 3;");
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

        public static bool ImageExists(string imageUrl)
        {
            var sqliteCmd = SqlHelper.sqlite_conn.CreateCommand();
            sqliteCmd.CommandText = $"select count(*) from Image WHERE Url = '{imageUrl.Trim()}' ";
            var result = sqliteCmd.ExecuteScalar();
            return  (long)result != 0;
        }
    }
}
