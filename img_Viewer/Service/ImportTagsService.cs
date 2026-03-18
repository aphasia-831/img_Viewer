using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace img_Viewer.Service
{
    public class ImportTagsService
    {

        private readonly string _connectionString;

        public ImportTagsService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task ImportAsync(string jsonPath, string csvPath)
        {
            var tags = await LoadTagsFromJson(jsonPath);
            var translations = LoadTranslationsFromCsv(csvPath);

            SaveToDatabase(tags, translations);
        }

        private async Task<List<string>> LoadTagsFromJson(string path)
        {
            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<List<string>>(json)!;
        }

        private Dictionary<string, string> LoadTranslationsFromCsv(string path)
        {
            var dict = new Dictionary<string, string>();

            foreach (var line in File.ReadLines(path))
            {
                var parts = line.Split(',');
                if (parts.Length >= 2)
                {
                    var tag = parts[0].Trim();
                    var zh = parts[1].Trim();
                    dict[tag] = zh;
                }
            }

            return dict;
        }

        private void SaveToDatabase(
            List<string> tags,
            Dictionary<string, string> translations)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var pragma = connection.CreateCommand();
            pragma.CommandText = "PRAGMA foreign_keys = ON;";
            pragma.ExecuteNonQuery();

            using var transaction = connection.BeginTransaction();

            foreach (var tag in tags)
            {
                var command = connection.CreateCommand();
                command.CommandText =
                    @"INSERT OR IGNORE INTO Tags (Name)
                  VALUES ($name);";

                command.Parameters.AddWithValue("$name", tag);
                command.ExecuteNonQuery();

                var getId = connection.CreateCommand();
                getId.CommandText =
                    @"SELECT Id FROM Tags WHERE Name = $name;";
                getId.Parameters.AddWithValue("$name", tag);

                var tagId = Convert.ToInt32(getId.ExecuteScalar());

                if (translations.TryGetValue(tag, out var zh))
                {
                    var insertZh = connection.CreateCommand();
                    insertZh.CommandText =
                        @"INSERT OR REPLACE INTO TagTranslations
                      (TagId, LanguageCode, DisplayName)
                      VALUES ($tagId, 'zh', $zh);";

                    insertZh.Parameters.AddWithValue("$tagId", tagId);
                    insertZh.Parameters.AddWithValue("$zh", zh);

                    insertZh.ExecuteNonQuery();
                }
            }

            transaction.Commit();
        }

    }
    
    }