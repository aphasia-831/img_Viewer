using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;


namespace img_Viewer.Data
{
    public class ImageService
    {
        private readonly string _connectionString;
        public ImageService()
        {
            _connectionString = App.database.GetConnectionString();
        }
        public void AddImage(string path)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText =
            "INSERT OR IGNORE INTO Images (FilePath) VALUES (@path)";
            cmd.Parameters.AddWithValue("@path", path);

            cmd.ExecuteNonQuery();
        }




    }
}
