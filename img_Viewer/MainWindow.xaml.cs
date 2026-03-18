using img_Viewer.Data;
using img_Viewer.Models;
using img_Viewer.Service;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Devices.Display.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;



namespace img_Viewer
{
    public class ImageItem
    {
        public BitmapImage Bitmap { get; set; }
        public string Path { get; set; }
    }
    public sealed partial class MainWindow : Window
    {
        private List<string> _folderPaths = new();
        private List<BitmapImage> _images = new();
        public ObservableCollection<DisplayBigImage> ImagesToDisplay { get; set; } = new();
        public MainWindow()
        {
            this.InitializeComponent();
            Title = "img_Viewer";
            
            _folderPaths = LoadFolderPaths();
            RefreshFolderList();
            var service = new DisplayService("\"C:\\Users\\runliu\\AppData\\Local\\Packages\\ad047355-ce8a-4940-9ae8-d39d80d292b1_y2xh6pxtv8r9m\\LocalCache\\Local\\Img\\tags.db\"");
            var list = service.LoadImagesWithTags();
            foreach (var img in list)
            {
                ImagesToDisplay.Add(img);
            }
            ImageGrid.ItemsSource = ImagesToDisplay;
        }

        void SaveFolderPaths(List<string> paths)
        {
            string json = JsonSerializer.Serialize(paths);
            ApplicationData.Current.LocalSettings.Values["FolderPaths"] = json;
        }

        List<string> LoadFolderPaths()
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue("FolderPaths", out object value))
            {
                try
                {
                    return JsonSerializer.Deserialize<List<string>>(value as string);
                }
                catch
                {
                    return new List<string>();
                }
            }
            return new List<string>();
        }
        private void RefreshFolderList()
        {
            FolderList.ItemsSource = null;
            FolderList.ItemsSource = _folderPaths;
        }
        private async void OnPickFolderClicked(object sender, RoutedEventArgs e)
        {
           var picker = new FolderPicker();

            
            var hwnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(picker, hwnd);

            picker.FileTypeFilter.Add("*");

            StorageFolder folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {   if(_folderPaths.Contains(folder.Path))
                {
                    ContentDialog warningDialog = new ContentDialog
                    {
                        Title = "提示",
                        Content="该文件夹已经存在",
                        CloseButtonText = "确定",
                        XamlRoot = this.Content.XamlRoot
                    };

                    await warningDialog.ShowAsync();
                    return;
                }
                _folderPaths.Add(folder.Path);        
                SaveFolderPaths(_folderPaths);
                RefreshFolderList();
            }
        }

        private void OnDeleteFolderClicked(object sender, RoutedEventArgs e)
        {
            
            if (FolderList.SelectedItem is string selectedPath)
            {
                _folderPaths.Remove(selectedPath);
                SaveFolderPaths(_folderPaths);
                RefreshFolderList();
            }
        }
  
        
        private async void FolderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FolderList.SelectedItem == null)
                return;

            
            string folderPath = FolderList.SelectedItem.ToString();

            if (!Directory.Exists(folderPath))
                return;

            await LoadImagesFromPath(folderPath);
        }
        private async Task LoadImagesFromPath(string folderPath)
        {
            ImagesToDisplay.Clear();

            //    StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            //    var files = await folder.GetFilesAsync();
            //
            //    foreach (var file in files)
            //    {
            //        if (!IsImageFile(file.FileType))
            //            continue;
            //
            //        using var stream = await file.OpenReadAsync();
            //        var bitmap = new BitmapImage();
            //        await bitmap.SetSourceAsync(stream);
            //
            //        Images.Add(new ImageItem
            //        {
            //            Bitmap = bitmap,
            //            Path = file.Path
            //        });
            //    }
            var service = new DisplayService("\"C:\\Users\\runliu\\AppData\\Local\\Packages\\ad047355-ce8a-4940-9ae8-d39d80d292b1_y2xh6pxtv8r9m\\LocalCache\\Local\\Img\\tags.db\"");
            var dbList = service.LoadImagesWithTags();
            var dbDict = dbList.ToDictionary(x => x.FilePath, x => x);
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            var files = await folder.GetFilesAsync();

            foreach (var file in files)
            {
                if (!IsImageFile(file.FileType))
                    continue;

                try
                {
                    using var stream = await file.OpenReadAsync();

                    var bitmap = new BitmapImage();
                    await bitmap.SetSourceAsync(stream);

                    
                    List<string> tags = new();

                    if (dbDict.TryGetValue(file.Path, out var dbImg))
                    {
                        tags = dbImg.Tags;
                    }

                    ImagesToDisplay.Add(new DisplayBigImage
                    {
                        FilePath = file.Path,
                        Bitmap = bitmap,
                        Tags = tags
                    });
                }
                catch
                {
                    
                }
            }
        }
        private bool IsImageFile(string ext)
        {
            ext = ext.ToLower();
            return ext == ".jpg" ||
                   ext == ".jpeg" ||
                   ext == ".png" ||
                   ext == ".bmp" ||
                   ext == ".gif" ||
                   ext == ".webp" ||
                   ext ==".jfif";
        }

        private async void ImportTagButton_Click(object sender, RoutedEventArgs e)
        {
            var db = new DatabaseService();
            var importer = new ImportTagsService(db.GetConnectionString());

            string jsonPath = @"D:\self study\tempFile\tag_list.json";
            string csvPath = @"D:\self study\tempFile\danbooru-0-zh.csv";

            await importer.ImportAsync(jsonPath, csvPath);

            ContentDialog dialog = new ContentDialog
            {
                Title = "完成",
                Content = "标签导入完成！",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }
        private async void ImportImages_Click(object sender, RoutedEventArgs e)
        {
            var db = new DatabaseService();
            var imageImpoter = new ImportImagesService(db.GetConnectionString());


            if (FolderList.SelectedItem is string folderPath)
            {
                var result = await imageImpoter.ImportImagesFromFolder(folderPath);
                ContentDialog dialog = new ContentDialog
                {
                    Title = "导入完成",
                    Content = $"成功导入 {result.successCount} 张\n重复图片 {result.duplicateCount} 张",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };

                await dialog.ShowAsync();
            }
        }

        private async void StartAnalysis_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = FolderList.SelectedItem as string;

            if (folderPath == null)
            {
                return;
            }
            string jsonPath = Path.Combine(
                AppContext.BaseDirectory,
                "temp",
                "result.json"
            );
            Directory.CreateDirectory(Path.GetDirectoryName(jsonPath)!);
            await ResultService.RunPythonScript(
                @"C:\Users\runliu\source\repos\img_Viewer\img_Viewer\AI\PythonEnv\2.py",
                @"C:\Users\runliu\source\repos\img_Viewer\img_Viewer\AI\PythonEnv\Scripts\python.exe",
                folderPath,
                jsonPath
            );
            string json = await File.ReadAllTextAsync(jsonPath);
            Debug.WriteLine(json);
            var results = JsonSerializer.Deserialize<List<Models.TagResult>>(json);
            
            DatabaseService db = new DatabaseService();
            using var conn = new SqliteConnection(db.GetConnectionString());
            conn.Open();

            ImageTagWriteService.SaveTags(results, conn);
        }
        private void TogglePane_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void ThumbnailGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("点击了");
            System.Diagnostics.Debug.WriteLine(e.ClickedItem.GetType().Name);
            var img = e.ClickedItem as DisplayBigImage;
            if (img != null)
            {
                LargeImage.Source = new BitmapImage(new Uri(img.FilePath));
                ImageTags.Text = string.Join(", ", img.Tags);
            }
        }
    }
        //下面一行是边界
    }

