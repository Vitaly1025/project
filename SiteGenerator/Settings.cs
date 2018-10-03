using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;

namespace SiteGenerator
{ 
  static  class Settings
    {
        public static string Path { get; set; }
        public static string Name { get; set; }
        public static bool Style { get; set; }
        public static string Uri { get; set; }
        public static string CssPath { get; set; }


        public static void CreateDirectory()
        {
           FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

            DialogResult result = folderBrowser.ShowDialog();

            if (!string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
            {
                Path = folderBrowser.SelectedPath;
            }
        }

        public static void OpenFile()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "hyper text markup lang (*.html)|*.html";
            DialogResult result = openFileDialog.ShowDialog();

            if (!string.IsNullOrWhiteSpace(openFileDialog.SafeFileName))
            {
                  Path = openFileDialog.FileName;
                  Uri = "file:///"+openFileDialog.FileName;
                  Uri=Uri.Replace("\\","/");
            }

        }
    }
}