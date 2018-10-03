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
using System.Windows.Shapes;
using System.IO;

namespace SiteGenerator
{
    /// <summary>
    /// Логика взаимодействия для CreateProjectPage.xaml
    /// </summary>
    public partial class CreateProjectPage : Window
    {
        public bool Success { get; set; }

        public CreateProjectPage()
        {
            InitializeComponent();
        }

        private void Path_Click(object sender, RoutedEventArgs e)
        {
            Settings.CreateDirectory();
            FilePath.Content = Settings.Path;
        }

        private void CreateProject(object sender, RoutedEventArgs e)
        {
            if (Project_Name == null)
            {
                MessageBox.Show("Заполните имя проекта");
            }
            else
            {
                Settings.Name = Project_Name.Text;
                FileName.Content = Project_Name.Text;
                if (Project_Name.Text != "")
                {
                    string buf = Settings.Path + Settings.Name + "\\";

                    Directory.CreateDirectory(buf);
                    Directory.CreateDirectory(buf + "\\css");

                    using (FileStream fs = File.Create(buf + Settings.Name + ".html"))
                    {
                        File.Create(buf + "css\\style.css");
                        Settings.CssPath = buf + "css\\style.css";
                    }
                    Success = true;

                    Settings.Path = buf + Settings.Name + ".html";
                    Settings.Uri = "file:///" + Settings.Path;
                    Settings.Uri = Settings.Path.Replace("\\", "/");
                    Close();
                }
                else
                {
                    MessageBox.Show("Вы не ввели информацию", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
             
            }
        }
    }
}
