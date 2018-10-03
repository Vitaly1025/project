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

namespace SiteGenerator
{
    /// <summary>
    /// Логика взаимодействия для AttrController.xaml
    /// </summary>
    public partial class AttrController : UserControl
    {
        // List<string> resultquerry { get; set; }
        List<string> resultquerry = new List<string>();
        public TextBox text;
        public AttrController(string name,List<string> a)
        {
            InitializeComponent();
            using (SiteGeneratorContext context = new SiteGeneratorContext())
            {

                var linq = from t in context.HTML_ATTR
                           where t.TAG == name.ToLower()
                           select t;

    
                if (linq.Count() > 0)
                {
                    var count = 0;
                    foreach (var item in linq)
                    {
                        
                            
                            var linqobj = (from b in context.HTML_ATTR
                                           where b.TAG == item.TAG
                                           select b).ToArray();
                        if (linqobj[count].NAME_ATTR != "src")
                        {

                            Label label = new Label() { Content = linqobj[count].NAME_ATTR, ToolTip = linqobj[count].TAG };
                            text = new TextBox() { Text = a[count], DataContext = linqobj[count].NAME_ATTR };
                            resultquerry.Add(linqobj[count].NAME_ATTR);
                            Attr.Children.Add(label);
                            Attr.Children.Add(text);
                            count++;
                        }

                        else
                        {
                            Label label = new Label() { Content = linqobj[count].NAME_ATTR, ToolTip = linqobj[count].TAG };
                            text = new TextBox() { Text = a[count], DataContext = linqobj[count].NAME_ATTR };
                            resultquerry.Add(linqobj[count].NAME_ATTR);
                            Attr.Children.Add(label);
                            Attr.Children.Add(text);
                            count++;
                        }

                    }
                    Button button = new Button() { Content = "Применить аттрибут" };
                    button.Click += Button_Click;
                    Attr.Children.Add(button);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var a = MainWindow.window.SelectTreeViewItem.DataContext as IHTMLElement;

            int buf = 0;
            foreach (var item in Attr.Children)
            {

                var x = item as TextBox;

                if (x!=null)
                {
                    if (x.Text != " ")
                    {
                        if(x.Text!=null)
                        a.setAttribute(resultquerry[buf].ToString(), x.Text.Remove(0,1));
                        buf++;
                    }
                }
              
            }
                       
        }

    }
}