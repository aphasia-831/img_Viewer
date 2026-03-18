using img_Viewer.Models;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Windows.AI.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace img_Viewer.Service
{
    
    public class DisplayService
    {
        private readonly string _connectionString;
        public DisplayService(string connectionString  )
        {
            _connectionString = connectionString;
        }
        public List<DisplayBigImage> LoadImagesWithTags()
        {
            var dict = new Dictionary<int, DisplayBigImage>();

            using var conn = new SqliteConnection($"Data Source={_connectionString}");
            conn.Open();

            var cmd = conn.CreateCommand();
            //           cmd.CommandText = @"
            //           SELECT i.Id, i.FilePath, t.Name
            //           FROM Images i
            //           LEFT JOIN ImageTags it ON i.Id = it.ImageId
            //           LEFT JOIN Tags t ON it.TagId = t.Id
            //           ORDER BY i.Id
            //       ";
                        cmd.CommandText = @"
                        SELECT
                            i.Id, 
                            i.FilePath, 
                            COALESCE(tt.DisplayName, t.Name) AS TagName
                        FROM Images i
                        LEFT JOIN ImageTags it ON i.Id = it.ImageId
                        LEFT JOIN Tags t ON it.TagId = t.Id
                        LEFT JOIN TagTranslations tt
                            ON t.Id = tt.TagId AND tt.LanguageCode = 'zh'
                        ORDER BY i.Id
                            ";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string path = reader.GetString(1);
                if (!dict.ContainsKey(id))
                {
                    dict[id] = new DisplayBigImage
                    {
                        Id = id,
                        FilePath = path
                    };
                }
                if (!reader.IsDBNull(2))
                {
                    string tag = reader.GetString(2);
                    dict[id].Tags.Add(tag);
                }
            }

            return dict.Values.ToList();
         }

    }
    
}
