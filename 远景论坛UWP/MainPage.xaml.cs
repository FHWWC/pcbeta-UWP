using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml.Documents;
using Windows.ApplicationModel.DataTransfer;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace 远景论坛UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage CurrentPage;
        public MainPage()
        {
            this.InitializeComponent();
            CurrentPage = this;

        }
        int isFinished = -1;
        //bool canNavi = true;
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            WebView1.Source = new Uri("http://i.pcbeta.com/home.php?mod=space&do=profile&mobile=no");

            if (await CheckLoadState(true) != 0)
            {
                return;
            }

            FirstLoad();
            加载界面.Visibility = (Visibility)1;
        }
        public async void FirstLoad()
        {
            await Task.Delay(1000);
            try
            {
                var login = string.Format(@"document.getElementById('toptb').innerText;");
                string result = await WebView1.InvokeScriptAsync("eval", new string[] { login });
                if (result.Contains("登录"))
                {
                    TopBtn.Visibility = 0;
                    return;
                }
            }
            catch (Exception)
            {
                TopBtn.Visibility = 0;
                ShowNotifi("网络连接异常！请检查您的网络.     错误");
                return;
            }

            ShowNotifi("正在登陆...请稍后");
            await Task.Delay(1000);
            LoginEvent();

            TopBtn.Visibility = 0;
            退出.Visibility = 0;
            LPLogin.Visibility = (Visibility)1;
            if (APPLeftPanel.IsPaneOpen)
            {
                LPBtn.Visibility = 0;
            }
            获取列表1.Visibility = 0;

        }
        private async void WebView1_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            await Task.Delay(1000);
            if (args.IsSuccess)
            {
                //canNavi = true;
                isFinished = 0;
            }
            else
            {
                isFinished = 1;
            }

        }
        public async Task<int> CheckLoadState(bool notifi)
        {
            await Task.Delay(1500);
            //新项目不需要，如要判断是否可跳转除外。
            //canNavi = false;

            int num = 0;
            while (true)
            {
                if (isFinished == 0)
                {
                    isFinished = -1;
                    break;
                }
                if (isFinished == 1)
                {
                    if (notifi)
                    {
                        ShowNotifi("连接服务器失败！请检查网络连接并重试！\n如果一直出现则可能是服务器暂时无法访问.");
                    }
                    isFinished = -1;
                    return 1;
                }
                if (num == 120)
                {
                    if (notifi)
                    {
                        ShowNotifi("连接服务器超时！请稍后重试！");
                    }
                    isFinished = -1;
                    return 2;
                }
                num++;
                await Task.Delay(1000);
            }
            return 0;
        }

        private void PanelControlBtn_Click(object sender, RoutedEventArgs e)
        {
            APPLeftPanel.IsPaneOpen = !APPLeftPanel.IsPaneOpen;
            if (APPLeftPanel.IsPaneOpen)
            {
                LPAvatar.Height = LPAvatar.Width = 120;
                LPAvatar.HorizontalAlignment = HorizontalAlignment.Center;
                LPAvatar.Margin = new Thickness(0, 20, 0, 10);
                UserLVL.Visibility = 0;

                if (UserName.Text != "游客")
                {
                    LPBtn.Visibility = 0;
                }

                SmallNew.Visibility = (Visibility)1;


            }
        }
        private void APPLeftPanel_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            APPLeftPanel.IsPaneOpen = false;

            LPAvatar.Height = LPAvatar.Width = 40;
            LPAvatar.HorizontalAlignment = HorizontalAlignment.Left;
            LPAvatar.Margin = new Thickness(5);
            UserLVL.Visibility = (Visibility)1;
            LPBtn.Visibility = (Visibility)1;

            if (新消息.Text!="")
            {
                SmallNew.Visibility = 0;
            }
        }
        private async void NEWSPage_Click(object sender, RoutedEventArgs e)
        {
            TopFunc();

            /*
            if (!canNavi)
            {
                await new MessageDialog("论坛服务器不稳定，数据未读取完，请稍后再试", "提示信息").ShowAsync();
                return;
            }
            */

            WebView1.Source = new Uri("http://www.pcbeta.com/news/");
            TheAPPFrame.Navigate(typeof(NEWSPage));
        }
        private void 论坛_Click(object sender, RoutedEventArgs e)
        {
            //TheAPPFrame.Navigate(typeof(ThreadContentPage));
            //return;
            TopFunc();

            WebView1.Source = new Uri("http://bbs.pcbeta.com/");
            TheAPPFrame.Navigate(typeof(BBSMainPage));
        }
        private async void CheckAndSearch_Click(object sender, RoutedEventArgs e)
        {
            TopFunc();

            WebView1.Source = new Uri("http://bbs.pcbeta.com/search.php?mod=forum&srchfrom=3600&searchsubmit=yes");
            // http://bbs.pcbeta.com/search.php?mod=forum&searchid=3600&orderby=lastpost&ascdesc=desc&searchsubmit=yes
        }
        private async void MoreFun_Click(object sender, RoutedEventArgs e)
        {
            TopFunc();


            //待编写...
        }
        public void TopFunc()
        {
            RegLoginPage.Visibility = (Visibility)1;
            if (UserName.Text == "游客")
            {
                LPAvatar.Visibility = 0;
                LPLogin.Visibility = 0;
            }
        }
        private async void ForgotPass_Click(object sender, RoutedEventArgs e)
        {
            var zh = string.Format(@"document.getElementsByClassName('tipcol')[1].getElementsByTagName('a')[0].onclick();");
            await WebView1.InvokeScriptAsync("eval", new string[] { zh });

            p1.Visibility = (Visibility)1;
            p2.Visibility = 0;
        }
        private async void Z返回_Click(object sender, RoutedEventArgs e)
        {
            var zh = string.Format(@"document.getElementsByClassName('tipcol')[1].getElementsByTagName('a')[0].onclick();");
            await WebView1.InvokeScriptAsync("eval", new string[] { zh });

            p1.Visibility = 0;
            p2.Visibility = (Visibility)1;
        }
        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            Error3.Text = "";

            Error1.Visibility = (Visibility)1;
            Error2.Visibility = (Visibility)1;

            if (string.IsNullOrEmpty(Username.Text) | string.IsNullOrWhiteSpace(Username.Text))
            {
                Error1.Visibility = 0;
                return;
            }
            if (string.IsNullOrEmpty(UPassword.Password) | string.IsNullOrWhiteSpace(UPassword.Password))
            {
                Error2.Visibility = 0;
                return;
            }

            LoginBtn.IsEnabled = false;
            try
            {
                var checkLogin = string.Format(@"document.getElementById('ct').innerText;");
                string result = await WebView1.InvokeScriptAsync("eval", new string[] { checkLogin });
                if (result.Contains("多次输入密码错误"))
                {
                    ShowNotifi("因为多次输入错误的密码，您的IP已被封禁30分钟，请之后再尝试登陆");
                    LoginBtn.IsEnabled = true;

                    RegLoginPage.Visibility = (Visibility)1;
                    LPAvatar.Visibility = 0;
                    LPLogin.Visibility = 0;
                    return;
                }

                var send1 = string.Format(@"document.getElementsByTagName('input')[2].value='{0}';", Username.Text);
                var send2 = string.Format(@"document.getElementsByTagName('input')[3].value='{0}';", UPassword.Password);
                var send3 = "";
                if (RememberPass.IsChecked == true) { send3 = string.Format(@"document.getElementsByName('cookietime')[0].checked=1;"); }
                else { send3 = string.Format(@"document.getElementsByName('cookietime')[0].checked=0;"); }
                var send4 = "";
                if(QuestionCheck.IsChecked==true)
                {
                    send4 = string.Format(@"document.getElementsByName('questionid')[0].selectedIndex={0};", QuestionBox.SelectedIndex) + string.Format(@"document.getElementsByName('answer')[0].innerText='{0}';", QuestionAnswer.Text);
                }

                var send5 = string.Format(@"document.getElementsByName('loginsubmit')[0].click();");
                await WebView1.InvokeScriptAsync("eval", new string[] { send1 + send2 + send3 + send4 + send5 });

                await LoginFun();

            }
            catch (Exception)
            {

            }

            LoginBtn.IsEnabled = true;
        }
        public async Task<string> LoginFun()
        {
            await Task.Delay(1000);

            var checkStatus = string.Format(@"document.querySelectorAll('.alert_right').length.toString();");
            string status = await WebView1.InvokeScriptAsync("eval", new string[] { checkStatus });
            if(status!="0")
            {
                RegLoginPage.Visibility = (Visibility)1;
                退出.Visibility = 0;
                LPLogin.Visibility = (Visibility)1;

                WebView1.Source = new Uri("http://i.pcbeta.com/home.php?mod=space&do=profile&mobile=no");
                if (await CheckLoadState(true) != 0)
                {
                    LoginBtn.IsEnabled = true;
                    return "";
                }

                await Task.Delay(2000);
                获取列表1.Visibility = 0;
                ShowNotifi("登陆成功!欢迎您!");
                LoginEvent();
                LPAvatar.Visibility = 0;
            }
            else
            {
                checkStatus = string.Format(@"document.querySelectorAll('.alert_error').length.toString();");
                status = await WebView1.InvokeScriptAsync("eval", new string[] { checkStatus });
                if(status!="0")
                {
                    var xxxx = string.Format(@"document.querySelector('.alert_error').innerText;");
                    Error3.Text = await WebView1.InvokeScriptAsync("eval", new string[] { xxxx });
                }
                else
                {
                    checkStatus = string.Format(@"document.querySelectorAll('.alert_info').length.toString();");
                    status = await WebView1.InvokeScriptAsync("eval", new string[] { checkStatus });
                    if (status != "0")
                    {
                        var xxxx = string.Format(@"document.querySelector('.alert_info').innerText;");
                        ShowNotifi(await WebView1.InvokeScriptAsync("eval", new string[] { xxxx }));
                        //
                    }
                    else
                    {
                        await LoginFun();
                    }
                }
            }

            return "";
        }
        public async void LoginEvent()
        {
            await Task.Delay(1000);

            try
            {
                var value1 = string.Format(@"document.getElementsByClassName('xw1')[0].innerText;");
                UserName.Text = await WebView1.InvokeScriptAsync("eval", new string[] { value1 });
            }
            catch(Exception)
            {

            }

            for(int i=0;i<30;i++)
            {
                try
                {
                    var txicon = string.Format(@"document.getElementById('pcd').getElementsByTagName('img')[0].src;");
                    TempAvatarURL = await WebView1.InvokeScriptAsync("eval", new string[] { txicon });
                    var gicon = string.Format(@"document.getElementsByClassName('vm')[1].src;");
                    TempLVLURL = await WebView1.InvokeScriptAsync("eval", new string[] { gicon });

                    RefreshMessage gx = new RefreshMessage();
                    gx.CheckNewMessage();

                    G1();
                    break;
                }
                catch (Exception)
                {

                }

                if (i==29)
                {
                    ShowNotifi("头像获取失败,请稍后再试");
                }
                await Task.Delay(1000);
            }

            ShowNotifi("登陆成功，欢迎您回来！");
        }
        public string TempAvatarURL;
        public string TempLVLURL;
        public void G1()
        {
            try
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri(TempAvatarURL));
                Avatar.ImageSource = bitmapImage;
                bitmapImage = new BitmapImage(new Uri(TempLVLURL));
                UserLVL.Source = bitmapImage;
            }
            catch(Exception)
            {
                
            }
        }
        private async void LPLogin_Click(object sender, RoutedEventArgs e)
        {
            APPLeftPanel.IsPaneOpen = false;
            try
            {
                if (WebView1.Source != new Uri("http://i.pcbeta.com/home.php?mod=space&do=profile&mobile=no"))
                {
                    WebView1.Source = new Uri("http://i.pcbeta.com/home.php?mod=space&do=profile&mobile=no");
                    if (await CheckLoadState(true) != 0)
                    {
                        return;
                    }
                }

                var send = string.Format(@"document.getElementById('toptb').innerText;");
                string ardy = await WebView1.InvokeScriptAsync("eval", new string[] { send });
                if (ardy.Contains("退出"))
                {
                    LoginEvent();

                    LPLogin.Visibility = (Visibility)1;
                    退出.Visibility = 0;
                    if(APPLeftPanel.IsPaneOpen)
                    {
                        LPBtn.Visibility = 0;
                    }
                    获取列表1.Visibility = 0;
                }
                else
                {
                    RegLoginPage.Visibility = 0;
                    LPAvatar.Visibility = (Visibility)1;
                    LPLogin.Visibility = (Visibility)1;
                }
            }
            catch (Exception)
            {
                WebView1.Source = new Uri("http://i.pcbeta.com/home.php?mod=space&do=profile&mobile=no");

                ShowNotifi("请稍后再试");
            }

        }
        private async void 退出_Click(object sender, RoutedEventArgs e)
        {
            var exit = string.Format(@"document.querySelectorAll('a[href*=""action=logout""]')[0].href;");
            string result = await WebView1.InvokeScriptAsync("eval", new string[] { exit });
            WebView1.Source = new Uri(result);

            BitmapImage bitmapImage = new BitmapImage(new Uri(this.BaseUri, "TopbarICON/默认头像.jpg"));
            Avatar.ImageSource = bitmapImage;
            SmallNew.Visibility = (Visibility)1;
            UserLVL.ClearValue(Image.SourceProperty);
            UserName.Text = "游客";
            LPBtn.Visibility = (Visibility)1;
            获取列表1.Visibility = (Visibility)1;

            TheAPPFrame.Content = null;
            if (await CheckLoadState(true) == 0)
            {
                ShowNotifi("您已成功退出，现在返回主界面");
            }

            await Task.Delay(1000);
            退出.Visibility = (Visibility)1;
            await Task.Delay(1000);
            LPLogin.Visibility = 0;
        }

        private async void Z发送_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var zh1 = string.Format(@"document.getElementById('lostpw_username').value='{0}';", z用户名.Text);
                var zh2 = string.Format(@"document.getElementById('lostpw_email').value='{0}';", z邮箱.Text);
                var zh3 = string.Format(@"document.getElementsByName('lostpwsubmit')[0].click();");
                await WebView1.InvokeScriptAsync("eval", new string[] { zh1 + zh2 + zh3 });
                await Task.Delay(1000);

                await FindResult();
            }
            catch (Exception)
            {
                ShowNotifi("发送失败，请稍后重试");
            }
        }

        public async Task<string> FindResult()
        {
            await Task.Delay(1000);
            var zhc = string.Format(@"document.getElementById('append_parent').innerText;");
            string zhStr= await WebView1.InvokeScriptAsync("eval", new string[] { zhc });

            if (zhStr.Contains("取回密码的方法"))
            {
                ShowNotifi("找回密码的邮件已发送至 " + z邮箱.Text + " 请注意查收.     找回密码");
            }
            else if (zhStr.Contains("您填写的账户资料不匹配"))
            {
                ShowNotifi("您填写的账户资料不匹配,请仔细检查后重试.     找回密码");
            }
            else
            {
                await FindResult();
            }
            return "";
        }
        private void 获取列表1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (获取列表1.SelectedIndex == 0)
            {
                WebView1.Source = new Uri("http://i.pcbeta.com/home.php?mod=space&do=thread&view=me&type=thread&page=1");
                TheAPPFrame.Navigate(typeof(MyPostPage));
            }
            else if (获取列表1.SelectedIndex == 1)
            {
                TheAPPFrame.Navigate(typeof(AccountSePage));
            }
        }
        public bool noCheck = false;
        private void LPAvatar_Click(object sender, RoutedEventArgs e)
        {
            if (UserName.Text != "游客")
            {
                if (WebView1.Source != new Uri("http://i.pcbeta.com/home.php?mod=space&do=profile&mobile=no"))
                {
                    WebView1.Source = new Uri("http://i.pcbeta.com/home.php?mod=space&do=profile&mobile=no");
                }
                else
                {
                    noCheck = true;
                }

                TheAPPFrame.Navigate(typeof(UserIfmPage));
            }
            else
            {
                ShowNotifi("请先登录账号再查看个人信息！");
            }
        }

        private void 提醒_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TheAPPFrame.Navigate(typeof(MyAlertPage));
                新消息.ClearValue(TextBlock.TextProperty);
            }
            catch (Exception)
            {

            }
        }

        private void FrameBackBtn_Click(object sender, RoutedEventArgs e)
        {
            if(TheAPPFrame.CanGoBack)
            {
                TheAPPFrame.GoBack();
                WebView1.GoBack();
            }
        }

        private void TheAPPFrame_Navigated(object sender, NavigationEventArgs e)
        {
            Frame frame = sender as Frame;
            if (frame != null)
            {
                if (frame.CanGoBack)
                {
                    FrameBackBtn.Visibility = 0;
                }
                else
                {
                    FrameBackBtn.Visibility = (Visibility)1;
                }
            }
        }

        public async void ShowNotifi(string content)
        {
            NotifiMessage.Text = content;

            for (double i = 0; i <=1; i += 0.2)
            {
                NotifiBlock.Opacity = i;
                await Task.Delay(100);
            }

            await Task.Delay(5000);

            for (double i = 1; i >= 0; i -= 0.2)
            {
                NotifiBlock.Opacity = i;
                await Task.Delay(100);
            }
        }

        private void WebView1_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {

        }
    }
}
