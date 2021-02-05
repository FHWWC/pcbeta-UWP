using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Printing.Workflow;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 远景论坛UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MyPostPage : Page
    {
        public MyPostPage()
        {
            this.InitializeComponent();
        }
        Uri thLink;
        int LoadType = 1;
        bool firstload = true;
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
            {
                return;
            }

            RefreshMessage refresh = new RefreshMessage();
            refresh.CheckNewMessage();

            LoadMyThread();
        }
        public async void LoadMyThread()
        {
            if(!await LoadCheck())
            {
                return;
            }

            TestList.Items.Add(new MyThreads { Tittle = "标题", Block = "所属板块", ReplyViewNum = "回复数 浏览量", LastReply = "最后回复", TUri = null });

            try
            {
                var send = string.Format(@"document.getElementsByTagName('table')[1].getElementsByTagName('tr').length.toString();");
                string count = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                int index = Convert.ToInt32(count);

                for (int i = 1; i < index; i++)
                {
                    var send1 = string.Format(@"document.getElementsByTagName('table')[1].getElementsByTagName('tr')[{0}].getElementsByTagName('a')[1].innerText;", i);
                    string thTittle = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send1 });
                    var send2 = string.Format(@"document.getElementsByTagName('table')[1].getElementsByTagName('tr')[{0}].getElementsByTagName('a')[1].href;", i);
                    thLink = new Uri(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send2 }));
                    var send3 = string.Format(@"document.getElementsByTagName('table')[1].getElementsByTagName('tr')[{0}].getElementsByTagName('td')[1].innerText;", i);
                    string thBlock = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send3 });
                    var send4 = string.Format(@"document.getElementsByTagName('table')[1].getElementsByTagName('tr')[{0}].getElementsByTagName('td')[2].innerText;", i);
                    string thCheck = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send4 });
                    var send5 = string.Format(@"document.getElementsByTagName('table')[1].getElementsByClassName('by')[{0}].innerText;", i);
                    string lastrpl = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send5 });
                    TestList.Items.Add(new MyThreads { Tittle = thTittle, Block = thBlock, ReplyViewNum = thCheck, LastReply = lastrpl, TUri = thLink });
                }

                CheckPage();
            }
            catch (Exception)
            {

            }
        }
        public async void LoadMyReply()
        {
            if (!await LoadCheck())
            {
                return;
            }
            TestList.Items.Add(new MyReply { Tittle = "标题", Block = "所属板块", ReplyViewNum = "回复数 浏览量",ReplyContent="回帖内容", LastReply = "最后回复", TUri = null });

            try
            {
                var send = string.Format(@"document.getElementsByTagName('table')[1].getElementsByClassName('bw0_all').length.toString();");
                string count = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                int index = Convert.ToInt16(count);

                for (int i = 0; i < index; i++)
                {
                    var send1 = string.Format(@"document.getElementsByTagName('table')[1].getElementsByClassName('bw0_all')[{0}].getElementsByTagName('a')[1].innerText;", i);
                    string thTittle = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send1 });
                    var send2 = string.Format(@"document.getElementsByTagName('table')[1].getElementsByClassName('bw0_all')[{0}].getElementsByTagName('a')[1].href;", i);
                    thLink = new Uri(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send2 }));
                    var send3 = string.Format(@"document.getElementsByTagName('table')[1].getElementsByClassName('bw0_all')[{0}].getElementsByTagName('td')[1].innerText;", i);
                    string thBlock = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send3 });
                    var send4 = string.Format(@"document.getElementsByTagName('table')[1].getElementsByClassName('bw0_all')[{0}].getElementsByTagName('td')[2].innerText;", i);
                    string thCheck = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send4 });
                    var send5 = string.Format(@"document.getElementsByTagName('table')[1].getElementsByClassName('bw0_all')[{0}].getElementsByClassName('by')[0].innerText;", i);
                    string lastrpl = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send5 });
                    var send6 = string.Format(@"document.querySelectorAll('td.xg1')[{0}].innerText;", i);
                    string content = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send6 });

                    TestList.Items.Add(new MyReply { Tittle = thTittle, Block = thBlock, ReplyViewNum = thCheck, LastReply = lastrpl,ReplyContent=content, TUri = thLink });
                }

                CheckPage();
            }
            catch (Exception)
            {

            }
        }
        public async void LoadMyDP()
        {
            if (!await LoadCheck())
            {
                return;
            }
            TestList.Items.Add(new MyDP { Tittle = "标题", Block = "所属板块",DPContent="点评内容",TUri = null });

            try
            {
                var send = string.Format(@"document.getElementsByClassName('bw0_all').length.toString();");
                string count = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                int index = Convert.ToInt16(count);

                for (int i = 0; i < index; i++)
                {
                    var send1 = string.Format(@"document.getElementsByClassName('bw0_all')[{0}].getElementsByTagName('a')[1].innerText;", i);
                    string thTittle = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send1 });
                    var send2 = string.Format(@"document.getElementsByClassName('bw0_all')[{0}].getElementsByTagName('a')[1].href;", i);
                    thLink = new Uri(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send2 }));
                    var send3 = string.Format(@"document.getElementsByClassName('bw0_all')[{0}].getElementsByTagName('td')[1].innerText;", i);
                    string thBlock = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send3 });
                    var send4 = string.Format(@"document.querySelectorAll('td.xg1')[{0}].innerText;", i);
                    string content = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send4 });

                    TestList.Items.Add(new MyDP { Tittle = thTittle, Block = thBlock, DPContent = content, TUri = thLink });
                }

                CheckPage();
            }
            catch (Exception)
            {

            }
        }
        public async Task<bool> LoadCheck()
        {
            BackBtn.Visibility = 0;
            NextBtn.Visibility = 0;
            Pageindex.Visibility = 0;
            JumpBtn.Visibility = 0;
            NoMessage.Text = "";
            TestList.Items.Clear();

            if(!firstload)
            {
                if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
                {
                    NoMessage.Text = "加载失败";
                    MainPage.CurrentPage.ShowNotifi("获取信息异常，请检查您的网络");
                    return false;
                }
            }
            firstload = false;

            try
            {
                var check = string.Format(@"document.getElementsByClassName('bm bw0')[0].innerText;");
                string isvd = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check });
                if (isvd.Contains("还没有相关的帖子"))
                {
                    NoMessage.Text = "您还没有发布的内容";
                    BackBtn.Visibility = (Visibility)1;
                    NextBtn.Visibility = (Visibility)1;
                    Pageindex.Visibility = (Visibility)1;
                    JumpBtn.Visibility = (Visibility)1;

                    return false;
                }
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }
        public async void CheckPage()
        {
            var send = string.Format(@"document.getElementsByClassName('pgb').length.toString();");
            string CKvaild = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
            int count1 = Convert.ToInt32(CKvaild);
            if (count1 == 0)
            {
                BackBtn.Visibility = (Visibility)1;
            }
            send = string.Format(@"document.getElementsByClassName('nxt').length.toString();");
            CKvaild = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
            int count2 = Convert.ToInt32(CKvaild);
            if (count2 == 0)
            {
                NextBtn.Visibility = (Visibility)1;
            }
            if (count1 == 0 && count2 == 0)
            {
                Pageindex.Visibility = (Visibility)1;
                JumpBtn.Visibility = (Visibility)1;
            }
            if (MainPage.CurrentPage.WebView1.Source.ToString().Contains("page=50"))
            {
                NextBtn.Visibility = (Visibility)1;
            }
            int abc = MainPage.CurrentPage.WebView1.Source.ToString().IndexOf("page");
            PageNum.Text = MainPage.CurrentPage.WebView1.Source.ToString().Substring(abc).Replace("page=", "");
        }
        private void TestList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if(LoadType==1)
            {
                var abcde = (e.ClickedItem) as MyThreads;
                if (abcde.TUri == null)
                {
                    return;
                }
                MainPage.CurrentPage.WebView1.Source = abcde.TUri;
            }
            else if (LoadType==2)
            {
                var abcde = (e.ClickedItem) as MyReply;
                if (abcde.TUri == null)
                {
                    return;
                }
                MainPage.CurrentPage.WebView1.Source = abcde.TUri;
            }
            else
            {
                var abcde = (e.ClickedItem) as MyDP;
                if (abcde.TUri == null)
                {
                    return;
                }
                MainPage.CurrentPage.WebView1.Source = abcde.TUri;
            }

            this.Frame.Navigate(typeof(ThreadContentPage));
        }

        private async void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var send = string.Format(@"'Count'+document.getElementsByClassName('pgb').length");
                string CKvaild = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                int count = Convert.ToInt32(CKvaild.Replace("Count", ""));
                if (count == 0)
                {
                    MainPage.CurrentPage.ShowNotifi("没有上一页了");

                    return;
                }
                send = string.Format(@"document.getElementsByClassName('pgb')[0].getElementsByTagName('a')[0].href");
                CKvaild = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                MainPage.CurrentPage.WebView1.Source = new Uri(CKvaild);

                if(LoadType==1)
                {
                    LoadMyThread();
                }
                else if (LoadType == 2)
                {
                    LoadMyReply();
                }
                else
                {
                    LoadMyDP();
                }
            }
            catch (Exception)
            {
                MainPage.CurrentPage.ShowNotifi("加载失败");
            }
        }

        private void JumpBtn_Click(object sender, RoutedEventArgs e)
        {
            int index = -1;
            if (!Int32.TryParse(Pageindex.Text, out index))
            {
                MainPage.CurrentPage.ShowNotifi("请输入有效的数值！");
                return;
            }
            if (index < 1 || index > 50)
            {
                MainPage.CurrentPage.ShowNotifi("允许跳转页面的范围：1 -- 50！");
                return;
            }

            if (LoadType == 1)
            {
                MainPage.CurrentPage.WebView1.Source = new Uri("http://i.pcbeta.com/home.php?mod=space&do=thread&view=me&type=thread&page="+index);
                LoadMyThread();
            }
            else if (LoadType == 2)
            {
                MainPage.CurrentPage.WebView1.Source = new Uri("http://i.pcbeta.com/home.php?mod=space&do=thread&view=me&type=reply&page="+index);
                LoadMyReply();
            }
            else
            {
                MainPage.CurrentPage.WebView1.Source = new Uri("http://i.pcbeta.com/home.php?mod=space&do=thread&view=me&type=postcomment&page="+index);
                LoadMyDP();
            }
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

                if (LoadType == 1)
                {
                    LoadMyThread();
                }
                else if (LoadType == 2)
                {
                    LoadMyReply();
                }
                else
                {
                    LoadMyDP();
                }
            }
            catch (Exception)
            {
                MainPage.CurrentPage.ShowNotifi("加载失败");
            }
        }

        private void TopBtn_Click(object sender, RoutedEventArgs e)
        {
            TestList.Items.Clear();
            Button button = sender as Button;
            if (button.Name == "MyThBtn")
            {
                MainPage.CurrentPage.WebView1.Source = new Uri("http://i.pcbeta.com/home.php?mod=space&do=thread&view=me&type=thread&page=1");

                LoadType = 1;
                TestList.ItemTemplate = DT1;
                LoadMyThread();
            }
            else if (button.Name == "MyReBtn")
            {
                MainPage.CurrentPage.WebView1.Source = new Uri("http://i.pcbeta.com/home.php?mod=space&do=thread&view=me&type=reply&page=1");

                LoadType = 2;
                TestList.ItemTemplate = DT2;
                LoadMyReply();
            }
            else
            {
                MainPage.CurrentPage.WebView1.Source = new Uri("http://i.pcbeta.com/home.php?mod=space&do=thread&view=me&type=postcomment&page=1");

                LoadType = 3;
                TestList.ItemTemplate = DT3;
                LoadMyDP();
            }
        }
    }
}
