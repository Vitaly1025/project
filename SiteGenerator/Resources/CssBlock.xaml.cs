using mshtml;
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
using System.IO;

namespace SiteGenerator
{
    /// <summary>
    /// Логика взаимодействия для CssBlock.xaml
    /// </summary>
    public partial class CssBlock : UserControl
    {
        public CssBlock(string name)
        {
            InitializeComponent();
            using (SiteGeneratorContext context = new SiteGeneratorContext())
            {

                var linq = from t in context.CSS_HTML
                           where t.TAG_NAME == name.ToLower()
                           select t;

                
                 if (linq.Count() > 0)
                {

                    
                    
                  List<string> list=  ReadingCss();
                   

                    foreach (var item in linq)
                    {

                           
                            var linqobj = (from t in context.CSS_GEN
                                           where t.PROPERTY_NAME == item.PROPERTY_NAME
                                           select t).Take(1).Single();
                            string buf = "";
                            foreach (var param in list)
                            {

                                if (param.Contains(linqobj.PROPERTY_NAME))
                                {
                                buf = param.Remove(0, param.IndexOf(':', 0)+1); ;
                                }
                            }
                         
                            if (linqobj.TYPE_ROW == 0)
                            {
                                Label label = new Label() { Content = linqobj.PROPERTY_NAME, ToolTip=linqobj.DISCRIPTION_CSS };
                                TextBox text = new TextBox() { Text = buf, DataContext = linqobj.PROPERTY_NAME };
                                CssController.Children.Add(label);
                                CssController.Children.Add(text);

                            }
                            else if (linqobj.TYPE_ROW == 1)
                            {
                                Label label = new Label() { Content = linqobj.PROPERTY_NAME, ToolTip = linqobj.DISCRIPTION_CSS };
                                ComboBox combo = new ComboBox() { Items = { "red", "blue","black" },DataContext= linqobj.PROPERTY_NAME };
                                CssController.Children.Add(label);
                                CssController.Children.Add(combo);
                            }
                            
                     

                       

                    }
                    Button button = new Button() { Content = "Применить свойства" };
                    button.Click += Button_Click;
                    CssController.Children.Add(button);
                }
              
            }

        }

        public MainWindow MainWindow
        {
            get => default;
            set
            {
            }
        }

        private List<string> ReadingCss()
        {
            List<string> list = new List<string>();
            using (StreamReader sr =new StreamReader(Settings.CssPath,true))
            {
                list.Add(sr.ReadToEnd());
            }
            IHTMLElement selectitem = (IHTMLElement)MainWindow.window.SelectTreeViewItem.DataContext;
            IHTMLElement parent = ((IHTMLElement)MainWindow.window.SelectTreeViewItem.DataContext).parentElement;
            string buf = null;
            foreach (var value in list)
            {
                if (value.Contains("#" + (selectitem.id == null ? "" : selectitem.id) + Environment.NewLine))
                {
                    int lenght = ("#" + (selectitem.id == null ? "" : selectitem.id) + Environment.NewLine).Length + 1;
                    int firstposition = value.IndexOf("#" + (selectitem.id == null ? "" : selectitem.id) + Environment.NewLine);
                    buf=value.Substring(firstposition + lenght, value.IndexOf('}', firstposition) - firstposition - lenght);
                }

                else if (value.Contains(((parent != null ? parent.tagName + " > " : "") + selectitem.tagName + Environment.NewLine)))
                {
                    int lenght = ((parent != null ? parent.tagName + " > " : "") + selectitem.tagName + Environment.NewLine).Length + 1;
                    int firstposition = value.IndexOf((parent != null ? parent.tagName + " > " : "") + selectitem.tagName + Environment.NewLine);
                    buf=value.Substring(firstposition + lenght, value.IndexOf('}', firstposition) - firstposition - lenght);
                }
            }
            list.Clear();
            if (buf != null)
            { 
              
            foreach (var item in buf.Split(';'))
            {
                list.Add(item);
            }
            }
            return list;
        }

        string ChildrenSelector(List<string> list,IHTMLElement selectitem,ref string text)
        {
            
            for (int i = list.Count - 1; i >= 0; i--)
            {
                text += list[i] + " > ";

            }

            text += selectitem.tagName + Environment.NewLine + "{" + Environment.NewLine;

            foreach (var item in CssController.Children)
            {
                var x = item as TextBox;
                if (x != null)
                {
                    if (!(x.Text == "" || x.Text == null))
                        text += x.DataContext.ToString() + ":" + x.Text + ";" + Environment.NewLine;
                }

                var y = item as ComboBox;
                if (y != null)
                {
                    if (!(y.Text == "" || y.Text == null))
                        text += y.DataContext.ToString() + ":" + y.SelectedItem + ";" + Environment.NewLine;
                }

            }
            text += "}";
            return text;
        }

        string IdSelector(List<string> list, IHTMLElement selectitem, ref string text)
        {

            for (int i = 0; i <list.Count; i++)
            {
                text += "#"+list[i];

            }

            text +=  Environment.NewLine + "{" + Environment.NewLine;

            foreach (var item in CssController.Children)
            {
                var x = item as TextBox;
                if (x != null)
                {
                    if (!(x.Text == "" || x.Text == null))
                        text += x.DataContext.ToString() + ":" + x.Text + ";" + Environment.NewLine;
                }

                var y = item as ComboBox;
                if (y != null)
                {
                    if (!(y.Text == "" || y.Text == null))
                        text += y.DataContext.ToString() + ":" + y.SelectedItem + ";" + Environment.NewLine;
                }

            }
            text += "}";
            return text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            IHTMLElement selectitem = (IHTMLElement)MainWindow.window.SelectTreeViewItem.DataContext;
            IHTMLElement parent = ((IHTMLElement)MainWindow.window.SelectTreeViewItem.DataContext).parentElement;
            List<string> list = new List<string>();
            list.Clear();
            string text = "";
           

            if (selectitem.id != null)
            {
                list.Add(selectitem.id.ToString());
                IdSelector(list, selectitem, ref text);
            }
            else
            {
                while (parent != null)
                {
                    list.Add(parent.tagName);
                    parent = parent.parentElement;
                }
                ChildrenSelector(list,selectitem,ref text);
            }
            if (text.IndexOf('{') + 3 != text.IndexOf('}'))
            {
                using (StreamWriter sw = new StreamWriter(Settings.CssPath, true))
                {
                    sw.WriteLine(text);
                }
            }
            else
            {
                MessageBox.Show("Вы не ввели никаких данных,","Внимание",MessageBoxButton.OK,MessageBoxImage.Warning);
            }
        }
        
    }
}
