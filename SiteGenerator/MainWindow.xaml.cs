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
using mshtml;

namespace SiteGenerator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       // public static CssBlock css { get; set; }
        public static MainWindow window { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

        }   

        #region CodeFirst

        private void CreateProject_Click(object sender, RoutedEventArgs e)
        {
            CreateProjectPage projectPage = new CreateProjectPage();
            projectPage.ShowDialog();
            if(projectPage.Success)
           
            if (Settings.Uri != null)
            {
                    Toglebar();
                    web.Source = new Uri(Settings.Uri);
                System.Windows.Forms.MessageBox.Show(web.Document.ToString());
                FirstInizial();
            }
           
            using (SiteGeneratorContext context=new SiteGeneratorContext())
            {
                var linqobj = (from b in context.HTML_ATTR
                               where b.TAG == "a"
                               select b).ToList();
       //         MessageBox.Show(linqobj.Count.ToString());
            }

        }

        public void Toglebar()
        {
            List<UIElement> objects = new List<UIElement>();
            objects.Add(Stack);
            objects.Add(Web_browser);
            objects.Add(Grid);
            objects.Add(Tool);
            objects.Add(ElementsControlFirst);
            objects.Add(Editor);
            foreach (UIElement a in objects)
            {
                if (a.Visibility == Visibility.Hidden)
                {
                    a.Visibility = Visibility.Visible;
                }
            }
            ElementsControlSecond.Visibility = Visibility.Visible;
            //ManagmentProject.Items.Add("Cоздать проект");
            //ManagmentProject.Items.Add("Открыть проект");
        }

        private void AddElementsClick(object sender, RoutedEventArgs e)
        {
            MenuItem menu = (MenuItem)sender;
            Tool.Items.Clear();
            Tool.DataContext = menu;
            Tool.Items.Add(new CssBlock(menu.Header.ToString()));
        }

        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            Settings.OpenFile();
            if (Settings.Path != null)
            {
                Toglebar();
                web.Navigate(new Uri(Settings.Uri));
                System.Windows.Forms.MessageBox.Show(web.Document.ToString());
                Inizial();
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Разработано для курсовой работы","ИСИТ-2 Курзенков",MessageBoxButton.OK,MessageBoxImage.Information);
        }
    
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
     
        #region CodeSecond

       
        private void FirstInizial()
        {
            dom = (HTMLDocument)web.Document;

            if (dom.hasChildNodes())
            {

                var x = dom.createElement("HTML");
                TreeViewItem nodes = new TreeViewItem() { Header = "<" + x.tagName + ">" };
                nodes.Style = (Style)this.TryFindResource("TreeViewItem");
                SelectTreeViewItem = nodes;
                ProcessElement(dom.documentElement, ref nodes);
                var link = dom.createElement("Link");
                link.setAttribute("rel", "stylesheet");
                link.setAttribute("href", Settings.CssPath);
                ((TreeViewItem)nodes.Items[0]).Items.Add(new TreeViewItem { Header = "<" + link.tagName + ">", DataContext = link });
                nodes.DataContext = dom;
                node.Items.Add(nodes);
                buf.DataContext = node;

            }
        }
        public HTMLDocument dom;
        private void Inizial()
        {
            dom = (HTMLDocument)web.Document;
            
            if (dom.hasChildNodes())
            {
                var x = dom.createElement("HTML");
                TreeViewItem nodes = new TreeViewItem() { Header = "<" + x.tagName + ">" };
                nodes.Style = (Style)this.TryFindResource("TreeViewItem"); 
                SelectTreeViewItem = nodes;
                nodes.DataContext = dom;
                ProcessElement(dom.documentElement, ref nodes);
                node.Items.Add(nodes);
            }
        
        }

        private void ProcessElement(IHTMLElement parentElement, ref TreeViewItem parent)
        {

            foreach (IHTMLElement element in parentElement.children)
            {
                TreeViewItem child = new TreeViewItem { Header = "<" + element.tagName + ">" };
                child.PreviewMouseLeftButtonUp += SelectItem;
                child.PreviewMouseRightButtonUp += MenuGen;
                child.DataContext = element;
                if (child.Header.ToString() == "<LINK>")
                {
                  Settings.CssPath= element.getAttribute("href");
                  Settings.CssPath= Settings.CssPath.Replace("file:///", "");
                }
                parent.Items.Add(child);
                
                if (element.children.length != 0)
                    ProcessElement(element, ref child);
                else if (element.innerHTML != null)
                {
                    TreeViewItem n = new TreeViewItem { Header = "<TEXT>", DataContext = element.innerHTML };
                    n.PreviewMouseLeftButtonUp += SelectItem;
                    child.Items.Add(n);
                    Tool.Items.Add(new CssBlock((n.Header.ToString())));
                }

            }
        }

        public TreeViewItem SelectTreeViewItem = new TreeViewItem();

        private void SelectItem(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem select = sender as TreeViewItem;
            SelectTreeViewItem = select;
            window = this;
            if (select.Header.ToString() == "<TEXT>")
            {
                MessageBox.Show("Текст");
            }

            else
            {
                List<string> list = new List<string>();
                var c = ((IHTMLElement)select.DataContext).tagName;
                using (SiteGeneratorContext context = new SiteGeneratorContext())
                {
                    var b = (from w in context.HTML_ATTR
                             where w.TAG == c.ToLower()
                             select w.NAME_ATTR).ToList();
                    if (b.Count>0)
                    {
                        foreach (var item in b)
                        {
                            dynamic a = ((IHTMLElement)select.DataContext).getAttribute(item.ToString());
                            string buf = " " + a;
                            list.Add(buf);

                            if (list != null)
                            {
                            attributes.Children.Clear();
                            }
                        }
                        attributes.Children.Add(new AttrController(((IHTMLElement)select.DataContext).tagName, list));
                    }
                        
                    if (b.Count == 0)
                    {
                        attributes.Children.Clear();
                    }
                }
                Tool.Items.Clear();
                Tool.Items.Add(new CssBlock((((IHTMLElement)select.DataContext).tagName)));
                if (select.Items.Count <= 1)
                {
                    var a = select.DataContext as IHTMLElement;
                    
                    if (a.innerText != null)
                        EditInner(a.innerText);
                }
                else
                {
                    Editor.Text = "";
                }
            }
        }

       

        #endregion

        public string invalid(TreeViewItem items)
        {
            if (items == null) return"";   
            string html = "";
         
            foreach (TreeViewItem item in items.Items)
            {
                if (item.Header.ToString() == "<TEXT>")
                {
                        
                    html += item.DataContext;

                    continue;
                }
                var x = dom.createElement(((IHTMLElement)item.DataContext).tagName);
                string newHTML = x.outerHTML;
               
                    using (SiteGeneratorContext context = new SiteGeneratorContext { })
                    {
                        var attr = (from at in context.HTML_ATTR
                                    where ((IHTMLElement)item.DataContext).tagName == at.TAG
                                    select at.NAME_ATTR);

                        foreach (var query in attr)
                        {
                        dynamic a = ((IHTMLElement)item.DataContext).getAttribute(query.ToString(), -1);
                            string buf = "" + a;
                            if (buf.Length > 0)
                            {
                                newHTML = newHTML.Insert(newHTML.IndexOf('>'), " " + query + "=" + '"' + a + '"');
                            }
                        }

                    }
          
                html += newHTML.Insert(newHTML.IndexOf('>') + 1,
                    item.Items.Count <= 0 ?
                    (((IHTMLElement)item.DataContext).innerHTML == null ? "" : ((IHTMLElement)item.DataContext).innerHTML + item.DataContext)
                    : invalid(item));

            }
            return html;
        }


        private void AddClick(object sender, RoutedEventArgs e)
        {
            var value=((MenuItem)Tool.DataContext).Header.ToString();
            var x = dom.createElement(value);

            TreeViewItem child = new TreeViewItem { Header = "<" + x.tagName + ">"};

            child.PreviewMouseLeftButtonUp += SelectItem;
            child.DataContext = x;
            SelectTreeViewItem.Items.Add(child);
           
        }

        private void RemoveClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы хотите безвозратно удалить элемент и все вложенные элементы?", "Предупреждение",
         MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                foreach (var item in node.Items)
                {
                    IndexNode((TreeViewItem)item, SelectTreeViewItem);
                }
            }
        }

        void IndexNode(TreeViewItem parent , TreeViewItem item)
        {
            int x = parent.Items.IndexOf(item);
            if (x >= 0)
            {
                parent.Items.RemoveAt(x);
            }
            else
            {
                if (parent.Items.Count > 0)
                foreach (var i in parent.Items)
                {
                    IndexNode((TreeViewItem)i, item);
                }
            }
        }


        private void ChangeClick(object sender, RoutedEventArgs e)
        {
            System.Threading.Thread.Sleep(1000);
            web.Visibility = Visibility.Hidden;
            web.Source = new Uri("file:///D:/"); 
            foreach (var item in node.Items)
            {
                string s = invalid(item as TreeViewItem).Insert(0, "<HTML>") + "</HTML>";
                MessageBox.Show("Успешно!");
                using(StreamWriter sw =new StreamWriter(Settings.Path,false))
                { 
                    sw.WriteLine(s);
                    sw.Flush();
                }
            }

            web.Visibility = Visibility.Visible;
            web.Navigate( new Uri(Settings.Uri));
       
        }

        private string decorations(TreeViewItem item)
        {
            var s = (IHTMLElement)item.DataContext;
            var x = dom.createElement(s.tagName);
            string newHTML = x.outerHTML;
            return newHTML.Insert(x.outerHTML.IndexOf('>') + 1, s.innerHTML == null ? "" : s.innerHTML);
        }

        private void DomElement(IHTMLElement parentElement, ref TreeViewItem parent)
        {

            foreach (IHTMLElement element in parentElement.children)
            {
                TreeViewItem child = new TreeViewItem { Header = "<" + element.tagName + ">" };
                child.PreviewMouseLeftButtonUp += SelectItem;
                child.DataContext = element;
                parent.Items.Add(child);
                if (!((element.children.length == 0) && (element.innerHTML != null)))
                    ProcessElement(element, ref child);

            }
        }

        private void InnerText(object sender, RoutedEventArgs e)
        {
            var a = SelectTreeViewItem.DataContext as IHTMLElement;
            TreeViewItem n = new TreeViewItem { Header = "<TEXT>", DataContext =Editor.Text };
            n.PreviewMouseLeftButtonUp += SelectItem;
            SelectTreeViewItem.Items.Clear();
            if (a.tagName == "TITLE")
            {
                if (a.innerText != null)
                {
                    a.innerText = null;
                }
                a.title = Editor.Text;
                SelectTreeViewItem.Items.Add(n);
            }
            else
            {
                if (a.innerText != null)
                {
                    a.innerText = null;
                }
                a.innerText = Editor.Text;
                SelectTreeViewItem.Items.Add(n);
            }
           

         
        }

        private void EditInner(string inner)
        {
            Editor.Text = inner;
        }

        private void Browser_Open(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process process= new System.Diagnostics.Process();
            process.StartInfo.FileName = "chrome.exe";
            process.StartInfo.Arguments = Settings.Path;
            process.Start();
        }

        private void MenuGen(object sender, MouseButtonEventArgs e)
        {
            var a = (TreeViewItem)sender;
           
 
            using (var context = new SiteGeneratorContext())
            {
                var blogs = (from t in context.HTML_GEN
                             where t.POSITION == (((a.Header.ToString()).Remove(a.Header.ToString().Length - 1, 1)).Remove(0, 1)).ToLower()
                             select t.TAG).ToList();
                if (blogs.Count != 0)
                    AddElements.Items.Clear();
                foreach (var item in blogs)
                {
                    MenuItem items = new MenuItem { Header =item.ToString() };
                    items.Click += AddElementsClick;
                    AddElements.Items.Add(items);
                }


            }
        }

        public AttrController AttrController
        {
            get => default;
            set
            {
            }
        }

        public CssBlock CssBlock
        {
            get => default;
            set
            {
            }
        }
    }
}