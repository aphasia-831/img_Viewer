using img_Viewer.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static img_Viewer.Models.TagResult;

namespace img_Viewer.Data
{
    public static class ImageTagWriteService
    {
        public static void SaveTags(List<TagResult> results, SqliteConnection conn)
        {
            foreach (var img in results)
            {
                long imageId = GetOrCreateImage(conn, img.file_path);

                InsertTags(conn, imageId, img.general, TagCategory.General);
                InsertTags(conn, imageId, img.character, TagCategory.Character);
                InsertTags(conn, imageId, img.copyright, TagCategory.Copyright);
                InsertTags(conn, imageId, img.rating, TagCategory.Rating);
            }
        }
        static long GetOrCreateImage(SqliteConnection conn, string path)
        {
            var check = conn.CreateCommand();
            check.CommandText = "SELECT Id FROM Images WHERE FilePath=$path";
            check.Parameters.AddWithValue("$path", path);

            var id = check.ExecuteScalar();

            if (id != null)
                return (long)id;

            var insert = conn.CreateCommand();
            insert.CommandText =
            """
        INSERT INTO Images (FilePath)
        VALUES ($path);
        SELECT last_insert_rowid();
        """;

            insert.Parameters.AddWithValue("$path", path);

            return (long)insert.ExecuteScalar();
        }

        static void InsertTags(SqliteConnection conn, long imageId, List<TagItem> tags, TagCategory category)
        {
            if (tags == null)
                return;

            foreach (var tag in tags)
            {
                long tagId = GetOrCreateTag(conn, tag.tag, category);

                var cmd = conn.CreateCommand();
                cmd.CommandText =
                """
            INSERT OR IGNORE INTO ImageTags (ImageId, TagId)
            VALUES ($img,$tag)
            """;

                cmd.Parameters.AddWithValue("$img", imageId);
                cmd.Parameters.AddWithValue("$tag", tagId);

                cmd.ExecuteNonQuery();
            }
        }

        static long GetOrCreateTag(SqliteConnection conn, string name, TagCategory category)
        {
            var check = conn.CreateCommand();
            check.CommandText = "SELECT Id FROM Tags WHERE Name=$name";
            check.Parameters.AddWithValue("$name", name);

            var id = check.ExecuteScalar();

            if (id != null)
                return (long)id;

            var insert = conn.CreateCommand();
            insert.CommandText =
            """
        INSERT INTO Tags (Name, Category)
        VALUES ($name,$category);
        SELECT last_insert_rowid();
        """;

            insert.Parameters.AddWithValue("$name", name);
            insert.Parameters.AddWithValue("$category", (int)category);

            return (long)insert.ExecuteScalar();
        }

    }
}
