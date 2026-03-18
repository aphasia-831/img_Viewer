using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace img_Viewer.Service
{
    public class ImportImagesService
    {
        private readonly string _connectionString;

        public ImportImagesService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task <(int successCount, int duplicateCount)> ImportImagesFromFolder(string folderPath)
        {
            var imageFiles = Directory
                .GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Where(f =>
                    f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    f.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
                .ToList();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            var command = connection.CreateCommand();

            int duplicateCount = 0;
            int successCount = 0;

            foreach (var file in imageFiles)
            {
                command.CommandText =
                    @"INSERT OR IGNORE INTO Images (FilePath)
          VALUES ($path);";

                command.Parameters.Clear();
                command.Parameters.AddWithValue("$path", file);

                int rows = command.ExecuteNonQuery();

                if (rows == 0)
                {
                    duplicateCount++;
                }
                else
                {
                    successCount++;
                }
            }

            transaction.Commit();
            return (successCount, duplicateCount);
            

        }
    }
}
