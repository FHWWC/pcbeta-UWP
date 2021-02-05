using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class MyAlertPage : Page
    {
        public static MyAlertPage CurrentPage;
        public MyAlertPage()
        {
            this.InitializeComponent();
            CurrentPage = this;
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var ckNew = string.Format(@"document.getElementById('myprompt').innerText;");
                string ckResult = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { ckNew });

                if (ckResult.Contains("("))
                {
                    NewMessage_Click(sender, e);
                }
                else
                {
                    InboxMessage_Click(sender, e);
                }
            }
            catch (Exception)
            {
                MainPage.CurrentPage.ShowNotifi("获取信息异常，请检查您的网络");
            }
        }
        public async void GetMessage()
        {
            BackBtn.Visibility = 0;
            NextBtn.Visibility = 0;
            Pageindex.Visibility = 0;
            JumpBtn.Visibility = 0;
            NoMessage.Text = "";
            MessageList.Items.Clear();

            if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
            {
                NoMessage.Text = "加载消息失败";
                MainPage.CurrentPage.ShowNotifi("获取信息异常，请检查您的网络");
                return;
            }

            var check = string.Format(@"document.getElementsByClassName('bm bw0')[0].innerText;");
            string isvd = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check });
            if (isvd.Contains("暂时没有新提醒")) 
            { 
                NoMessage.Text = "暂时没有新提醒";
                BackBtn.Visibility = (Visibility)1;
                NextBtn.Visibility = (Visibility)1;
                Pageindex.Visibility = (Visibility)1;
                JumpBtn.Visibility = (Visibility)1;

                return; 
            }

            try
            {
                var send = string.Format(@"'Count'+document.getElementsByClassName('nts')[0].childElementCount");
                string get = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                int item = Convert.ToInt32(get.Replace("Count", ""));

                for (int i = 0; i < item; i++)
                {
                    var avatar = string.Format(@"document.getElementsByClassName('m avt mbn')[{0}].getElementsByTagName('img')[0].src;", i);
                    string get1 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { avatar });

                    string get2 = "";
                    if (!get1.Contains("system"))
                    {
                        var space = string.Format(@"document.getElementsByClassName('m avt mbn')[{0}].getElementsByTagName('a')[0].href;", i);
                        get2 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { space });
                    }
                    var dtime = string.Format(@"document.getElementsByClassName('nts')[0].getElementsByClassName('cl')[{0}].getElementsByClassName('xg1 xw0')[0].innerText;", i);
                    string get3 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { dtime });
                    var content = string.Format(@"document.getElementsByClassName('ntc_body')[{0}].innerText;", i);
                    string get4 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { content });

                    var linkcount = string.Format(@"'Count'+document.getElementsByClassName('ntc_body')[{0}].getElementsByTagName('a').length", i);
                    string get5 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { linkcount });
                    int count = Convert.ToInt32(get5.Replace("Count", ""));
                    for (int j = 0; j < count; j++)
                    {
                        var directlink = string.Format(@"document.getElementsByClassName('ntc_body')[{0}].getElementsByTagName('a')[{1}].href;", i, j);
                        get5 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { directlink });
                        if (get5.Contains("viewthread") || get5.Contains("redirect"))
                        {
                            break;
                        }
                        else
                        {
                            get5 = "";
                        }
                    }

                    MessageList.Items.Add(
                        new AlertInformation { UserImage = new BitmapImage(new Uri(get1)), UserSpaceID = get2, Datetime = get3, AlertContent = get4, Directlink = get5 , BtnName =i.ToString()});

                }


                send = string.Format(@"'Count'+document.getElementsByClassName('prev').length");
                string CKvaild = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                int count1 = Convert.ToInt32(CKvaild.Replace("Count", ""));
                if (count1 == 0)
                {
                    BackBtn.Visibility = (Visibility)1;
                }
                send = string.Format(@"'Count'+document.getElementsByClassName('nxt').length");
                CKvaild = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                int count2 = Convert.ToInt32(CKvaild.Replace("Count", ""));
                if (count2 == 0)
                {
                    NextBtn.Visibility = (Visibility)1;
                }
                if(count1==0 && count2==0)
                {
                    Pageindex.Visibility = (Visibility)1;
                    JumpBtn.Visibility = (Visibility)1;
                }
                if(MainPage.CurrentPage.WebView1.Source.ToString().Contains("page=50"))
                {
                    NextBtn.Visibility = (Visibility)1;
                }
                int abc = MainPage.CurrentPage.WebView1.Source.ToString().IndexOf("page");
                PageNum.Text = MainPage.CurrentPage.WebView1.Source.ToString().Substring(abc).Replace("page=", "");
            }
            catch (Exception)
            {

            }
        }

        private void NewMessage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NoMessage.ClearValue(TextBlock.TextProperty);
                MainPage.CurrentPage.WebView1.Source = new Uri("http://i.pcbeta.com/home.php?mod=space&do=notice&page=1");
                NewMessage.FontSize = 26; InboxMessage.FontSize = 22; NewMessage.Foreground = new SolidColorBrush(Colors.Yellow); InboxMessage.Foreground = new SolidColorBrush(Colors.Black);

            }
            catch (Exception)
            {
                MainPage.CurrentPage.ShowNotifi("非常抱歉，我们这里出了点小问题，请您刷新页面重试");
            }

            GetMessage();
        }

        private void InboxMessage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NoMessage.ClearValue(TextBlock.TextProperty);
                MainPage.CurrentPage.WebView1.Source = new Uri("http://i.pcbeta.com/home.php?mod=space&do=notice&isread=1%page=1");
                NewMessage.FontSize = 22; InboxMessage.FontSize = 26; NewMessage.Foreground = new SolidColorBrush(Colors.Black); InboxMessage.Foreground = new SolidColorBrush(Colors.Cyan);
            }
            catch (Exception)
            {
                MainPage.CurrentPage.ShowNotifi("非常抱歉，我们这里出了点小问题，请您刷新页面重试");
            }

            GetMessage();
        }
        private void MessageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(MessageList.SelectedIndex<0)
            {
                return;
            }
            string url = ((AlertInformation)MessageList.SelectedItem).Directlink;
            MainPage.CurrentPage.WebView1.Source = new Uri(url);
            this.Frame.Navigate(typeof(ThreadContentPage));
        }
        private void TempleBtn_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int index = Convert.ToInt32(button.Tag);
            var link = ((AlertInformation)MessageList.Items[index]).UserSpaceID;
            //等待处理url
            MainPage.CurrentPage.ShowNotifi(link);

        }
        private async void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var send = string.Format(@"'Count'+document.getElementsByClassName('prev').length");
                string CKvaild = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                int count = Convert.ToInt32(CKvaild.Replace("Count", ""));
                if(count==0)
                {
                    MainPage.CurrentPage.ShowNotifi("没有上一页了");

                    return;
                }
                send = string.Format(@"document.getElementsByClassName('prev')[0].href");
                CKvaild= await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                MainPage.CurrentPage.WebView1.Source = new Uri(CKvaild);
                GetMessage();
            }
            catch(Exception)
            {
                MainPage.CurrentPage.ShowNotifi("加载失败");
            }
        }

        private void JumpBtn_Click(object sender, RoutedEventArgs e)
        {
            int index=-1;
            if(!Int32.TryParse(Pageindex.Text,out index))
            {
                MainPage.CurrentPage.ShowNotifi("请输入有效的数值!");
                return;
            }
            if(index<1||index>50)
            {
                MainPage.CurrentPage.ShowNotifi("允许跳转页面的范围：1-- 50！");
                return;
            }
            if (MainPage.CurrentPage.WebView1.Source.ToString().Contains("isread"))
            {
                MainPage.CurrentPage.WebView1.Source = new Uri("http://i.pcbeta.com/home.php?mod=space&do=notice&isread=1&page=" + index);
            }
            else
            {
                MainPage.CurrentPage.WebView1.Source = new Uri("http://i.pcbeta.com/home.php?mod=space&do=notice&page=" + index);
            }

            GetMessage();
        }

        private async void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var send = string.Format(@"'Count'+document.getElementsByClassName('nxt').length");
                string CKvaild = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                int count = Convert.ToInt32(CKvaild.Replace("Count", ""));
                if (count == 0)
                {
                    MainPage.CurrentPage.ShowNotifi("没有下一页了");

                    return;
                }
                send = string.Format(@"document.getElementsByClassName('nxt')[0].href");
                CKvaild = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                MainPage.CurrentPage.WebView1.Source = new Uri(CKvaild);
                GetMessage();
            }
            catch (Exception)
            {
                MainPage.CurrentPage.ShowNotifi("加载失败");
            }
        }
    }
}
