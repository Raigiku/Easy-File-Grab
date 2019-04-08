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
using System.Text.RegularExpressions;

namespace Easy_File_Grab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string[] fileExtensions;
        private DirectoryInfo selectedDirectory;
        private readonly List<FileInfo> filteredFiles;

        public MainWindow()
        {
            InitializeComponent();

            ExtractFilesButton.IsEnabled = false;
            filteredFiles = new List<FileInfo>();

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
                        filteredFiles.Add(file);
                    }
                }
                if (FilesListBox.Items.IsEmpty)
                {
                    ExtractFilesButton.IsEnabled = false;
                    EmptyFolderLabel.Visibility = Visibility.Visible;
                }
                else
                {
                    ExtractFilesButton.IsEnabled = true;
                    EmptyFolderLabel.Visibility = Visibility.Hidden;
                }
            }
        }

        private void ExtractFilesButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDirectory.Exists)
            {
                DirectoryInfo desktopCopyDirectory = new DirectoryInfo(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Easy File Grab - Archivos"));

                foreach (FileInfo file in filteredFiles)
                {
                    int i = 0;
                    foreach (var cutPath in Regex.Split(file.Directory.FullName, @"(?<=" + selectedDirectory.Parent.Name + @"\\(?=[^\\]+))"))
                    {
                        ++i;
                        if (i % 2 == 0)
                        {
                            Directory.CreateDirectory(System.IO.Path.Combine(desktopCopyDirectory.FullName, cutPath));
                            File.Copy(file.FullName, System.IO.Path.Combine(desktopCopyDirectory.FullName, cutPath, file.Name), true);
                        }
                    }
                }

                MessageBox.Show("Su carpeta y archivos han sido filtrados y extraidos a su escritorio dento de la carpeta \"Easy File Grab - Archivos\"", "Carpeta Extraída!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Autor: Diego Castillo\nTecnologia: Windows Presentation Foundation\nLicencia: MIT", "Acerca de", MessageBoxButton.OK, MessageBoxImage.Question);
        }
    }
}
