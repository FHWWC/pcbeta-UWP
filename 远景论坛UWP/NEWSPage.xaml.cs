using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 远景论坛UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NEWSPage : Page
    {
        public NEWSPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadNEWS(false);
        }
        public async void LoadNEWS(bool noclear)
        {
            滚动条.Visibility = 0;
            滚动条.IsActive = true;
            if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
            {
                滚动条.Visibility = (Visibility)1;
                滚动条.IsActive = false;
                JumpBtn.IsEnabled = true;
                return;
            }

            RefreshMessage refresh = new RefreshMessage();
            refresh.CheckNewMessage();

            if (!noclear)
            {
                NewsList.Items.Clear();
            }

            try
            {
                for (int i = 0; i < 45; i++)
                {
                    var newsImg = string.Format(@"document.getElementById('pt_list').getElementsByTagName('img')[{0}].src;", i);
                    string get1 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { newsImg });
                    var bitmap = new BitmapImage(new Uri(get1));
                    var newsTitle = string.Format(@"document.getElementById('pt_list').getElementsByTagName('h3')[{0}].innerText;", i);
                    string get2 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { newsTitle });
                    var newsNotes = string.Format(@"document.getElementById('pt_list').getElementsByTagName('p')[{0}].innerText;", i);
                    string get3 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { newsNotes });
                    var newsPost = string.Format(@"document.getElementById('pt_list').getElementsByTagName('span')[{0}].innerText;", i);
                    string get4 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { newsPost });
                    var newsLink = string.Format(@"document.getElementsByClassName('thumb')[{0}].href;", i);
                    Uri get5 = new Uri(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { newsLink }));

                    NewsList.Items.Add(new NEWSIfm { Tittle = get2, Preview = get3, Information = get4, NEWSICON = bitmap, NEWSUri = get5 });
                }
            }
            catch (Exception)
            {
                MainPage.CurrentPage.ShowNotifi("新闻加载失败，请稍后重试");
            }

            滚动条.Visibility = (Visibility)1;
            滚动条.IsActive = false;
            JumpBtn.IsEnabled = true;
        }
        private void NewsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if(e.ClickedItem as NEWSIfm == null)
            {
                return;
            }
            MainPage.CurrentPage.WebView1.Source = (e.ClickedItem as NEWSIfm).NEWSUri;
            this.Frame.Navigate(typeof(ShowNEWSPage));
        }

        private void JumpBtn_Click(object sender, RoutedEventArgs e)
        {
            int index;
            if (!int.TryParse(PageIndex.Text, out index))
            {
                MainPage.CurrentPage.ShowNotifi("请输入有效的数值！");
                return;
            }
            if (index < 1 || index > 1000)
            {
                MainPage.CurrentPage.ShowNotifi("允许跳转页面的范围：1 -- 1000！");
                return;
            }
            JumpBtn.IsEnabled = false;

            MainPage.CurrentPage.WebView1.Source = new Uri("http://www.pcbeta.com/news/?page="+index);
            LoadNEWS(false);
        }

        private void ListScroll_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if(ListScroll.VerticalOffset==ListScroll.ScrollableHeight)
            {
                LoadMore();
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           ListScroll.Height = Window.Current.Bounds.Height - 180;
        }
        /*
           var t = NewsList.TransformToVisual(Window.Current.Content);
            Point screenCoords = t.TransformPoint(new Point(0, 0));
            await new MessageDialog(screenCoords.ToString()).ShowAsync();         
         */
        public async void LoadMore()
        {
            if(滚动条.IsActive)
            {
                return;
            }
            try
            {
                var send = string.Format(@"document.getElementsByClassName('nxt')[0].href");
                string CKvaild = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                MainPage.CurrentPage.WebView1.Source = new Uri(CKvaild);
                LoadNEWS(true);
            }
            catch(Exception)
            {
                MainPage.CurrentPage.ShowNotifi("新闻加载失败，请稍后重试");
            }

        }
    }
}
