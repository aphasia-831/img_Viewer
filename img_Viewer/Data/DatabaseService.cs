using Microsoft.Data.Sqlite;
using Microsoft.Windows.AI.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace img_Viewer.Data
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Img",
                "tags.db");

            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            _connectionString = $"Data Source={dbPath}";

            InitializeDatabase();
            Debug.WriteLine("DatabaseService 被执行了");
        }

        public string GetConnectionString() => _connectionString;
        
        
        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"

            
            CREATE TABLE IF NOT EXISTS Images(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FilePath TEXT NOT NULL UNIQUE,
                CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP
);

            CREATE TABLE IF NOT EXISTS Tags(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE,
                Category INTEGER,
                Rating TEXT
            );

            CREATE TABLE IF NOT EXISTS TagTranslations(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TagId INTEGER NOT NULL,
                LanguageCode TEXT NOT NULL,
                DisplayName TEXT NOT NULL,
                UNIQUE(TagId, LanguageCode),
                FOREIGN KEY(TagId) REFERENCES Tags(Id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS ImageTags(
                ImageId INTEGER NOT NULL,
                TagId INTEGER NOT NULL,
                PRIMARY KEY (ImageId, TagId),
                FOREIGN KEY(ImageId) REFERENCES Images(Id) ON DELETE CASCADE,
                FOREIGN KEY(TagId) REFERENCES Tags(Id) ON DELETE CASCADE
            );

            CREATE INDEX IF NOT EXISTS idx_tags_name ON Tags(Name);
            CREATE INDEX IF NOT EXISTS idx_translations_tagid ON TagTranslations(TagId);
            CREATE INDEX IF NOT EXISTS idx_imagetags_imageid ON ImageTags(ImageId);
            CREATE INDEX IF NOT EXISTS idx_imagetags_tagid ON ImageTags(TagId);
            ";
            command.ExecuteNonQuery();
        }

    }
}
