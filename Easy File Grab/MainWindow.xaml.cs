using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using WinForms = System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace Easy_File_Grab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] fileExtensions;
        private DirectoryInfo selectedDirectory;

        public MainWindow()
        {
            InitializeComponent();

            ExtractFilesButton.IsEnabled = false;

            string jsonData = File.ReadAllText(@"D:\Visual Studio Projects\Repos\Easy-File-Grab\Easy File Grab\FileExtensions.json");
            fileExtensions = JsonConvert.DeserializeObject<string[]>(jsonData);
        }

        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            WinForms.FolderBrowserDialog selectFolderDialog = new WinForms.FolderBrowserDialog
            {
                ShowNewFolderButton = false,
                SelectedPath = System.AppDomain.CurrentDomain.BaseDirectory
            };
            WinForms.DialogResult result = selectFolderDialog.ShowDialog();

            if (result == WinForms.DialogResult.OK)
            {
                FilesListBox.Items.Clear();
                string selectedPath = selectFolderDialog.SelectedPath;
                selectedDirectory = new DirectoryInfo(selectedPath);

                FileInfo[] allFiles = selectedDirectory.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo file in allFiles)
                {
                    if (fileExtensions.Contains(file.Extension))
                    {
                        FilesListBox.Items.Add(file.FullName);
                    }
                }

                ExtractFilesButton.IsEnabled = true;
            }
        }

        private void ExtractFilesButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDirectory.Exists)
            {
                DirectoryInfo desktopCopyDirectory = new DirectoryInfo(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Easy File Grab - Archivos", selectedDirectory.Name));
                if (!desktopCopyDirectory.Exists)
                {
                    DirectoryCopy(selectedDirectory.FullName, desktopCopyDirectory.FullName, true);
                }

                FileInfo[] allFiles = desktopCopyDirectory.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo file in allFiles)
                {
                    if (!fileExtensions.Contains(file.Extension))
                    {
                        file.Delete();
                    }
                }
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = System.IO.Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
