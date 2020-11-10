using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Threading;
using System.Text.RegularExpressions;
using Path = System.IO.Path;
using System.Threading.Tasks;

namespace File_Searching
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string current_Directory = null;
        public string pattern = null;
        public TreeViewItem current_TreeIteam = null;
        public TreeViewItem Item = new TreeViewItem();
        public TreeViewItem subItem = new TreeViewItem();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Butt_Begin_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "/options.txt"))
            {
                StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "/options.txt");

                current_Directory = sr.ReadLine();
                pattern = sr.ReadLine();
                sr.Close();
                sr.Dispose();
            }
            else
            {
                if ((pattern == null) & (current_Directory == null))
                {
                    if (Regex_pattern == null)
                    {
                        MessageBox.Show("Введите Regex выражение");
                    }
                    else
                    {
                        pattern = @"" + Regex_pattern.Text;
                    }

                    if (current_Directory == null)
                    {
                        MessageBox.Show("Выберете директорию");
                    }
                }
                else
                {
                    pattern = @"" + Regex_pattern.Text;

                    //Thread myThread = new Thread(new ThreadStart(Searching_DrawTreeView));
                    //myThread.Start();

                    Task task = new Task(Searching_DrawTreeView);
                    task.Start();

                }

                if (File.Exists(Directory.GetCurrentDirectory() + "/options.txt"))
                {
                    StreamWriter sr = new StreamWriter(Directory.GetCurrentDirectory() + "/options.txt");

                    sr.WriteLine(current_Directory);
                    sr.WriteLine(pattern);
                    sr.Close();
                    sr.Dispose();
                }
                else
                {
                    FileStream fs = File.Create(Directory.GetCurrentDirectory() + "/options.txt");
                    fs.Close();
                    fs.Dispose();
                    StreamWriter sr = new StreamWriter(Directory.GetCurrentDirectory() + "/options.txt");

                    sr.WriteLine(current_Directory);
                    sr.WriteLine(pattern);
                    sr.Close();
                    sr.Dispose();
                }

            }
        }

        private void Butt_Stop_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Choose_directory_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;
            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
               current_Directory = dlg.FileName;
               Start_Directory.Text = dlg.FileName;
            }
        }

        public void Searching_DrawTreeView ()
        {
            //TreeViewItem treeViewItem = new TreeViewItem();
            //treeViewItem.Header = current_Directory;
            //TreeView.Items.Add(treeViewItem);

            Directorys(current_Directory);
            Files(current_Directory);
        }

        public void Directorys(string targetDirectory)
        {
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            if (subdirectoryEntries.Length != 0)
            {
                foreach (string subdirectory in subdirectoryEntries)
                {
                    TreeDirectory(subdirectory);
                    Files(subdirectory);
                    Directorys(subdirectory);
                }

            }
            try
            {
                TreeViewItem parent = (TreeViewItem)current_TreeIteam.Parent;
                current_TreeIteam = parent;
            }
            catch
            { }

        }

        public void Files(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);

            foreach (string fileName in fileEntries)
            {
                if (Regex.IsMatch(Path.GetFileName(fileName), pattern))
                    TreeFile(fileName);
            }
        }

        public void TreeDirectory(string targetDirectory)
        {

            Item.Header = Path.GetFileName(targetDirectory);

            current_TreeIteam.Items.Add(Item);
            current_TreeIteam = Item;
        }

        public void TreeFile(string targetDirectory)
        {
            subItem.Header = Path.GetFileName(targetDirectory);

            current_TreeIteam.Items.Add(subItem);
        }
    }
}
