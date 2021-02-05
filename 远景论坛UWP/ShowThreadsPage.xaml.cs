using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
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
    public sealed partial class ShowThreadsPage : Page
    {
        public static ShowThreadsPage PageTrans;
        public ShowThreadsPage()
        {
            this.InitializeComponent();
            PageTrans = this;
        }
        string ThreadTags;
        Uri pageURL = null;
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            pageURL = MainPage.CurrentPage.WebView1.Source;
            try
            {
                滚动条.Visibility = Visibility.Visible;
                滚动条.IsActive = true;

                if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
                {
                    滚动条.Visibility = (Visibility)1;
                    滚动条.IsActive = false;
                    return;
                }

                RefreshMessage refresh = new RefreshMessage();
                refresh.CheckNewMessage();

                ThreadList.Items.Clear();

                BlockNText.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { "document.querySelectorAll(\".xs2 a[href *= 'bbs.pcbeta.com/forum-']\")[0].innerText;" });
                if (await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { "document.querySelectorAll('h1.xs2 .y').length.toString();" }) == "0")
                {
                    BZText.Text = "该板块暂无版主";
                }
                else
                {
                    BZText.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { "document.querySelectorAll('h1.xs2 .y')[0].innerText;" });
                }
                TodayText.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { "document.getElementsByClassName('xs1 xw0 i')[0].querySelector('.xi1').innerText;" });

                var ckCount = string.Format(@"document.querySelectorAll('.icn').length.toString();");
                string ckResult = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { ckCount });
                int count = Convert.ToInt32(ckResult);

                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { "document.getElementById('separatorline').remove(); " });

                for (int i = 0; i < count; i++)
                {
                    ThreadTags = "";

                    var getTag = string.Format(@"document.querySelectorAll('.icn')[{0}].innerHTML;", i);
                    string thTag = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getTag });

                    if (thTag.Contains("公告"))
                    {
                        ThreadTags += "*全站公告*\n";
                        ThreadTags += "【已高亮】\n";
                        ThreadTags += "【已加粗】\n";
                    }
                    if (thTag.Contains("全局置顶"))
                    {
                        ThreadTags += "*全局置顶*\n";
                    }
                    if (thTag.Contains("本版置顶"))
                    {
                        ThreadTags += "*本版置顶*\n";
                    }
                    if (thTag.Contains("分类置顶"))
                    {
                        ThreadTags += "*分类置顶*\n";
                    }
                    if (thTag.Contains("关闭的"))
                    {
                        ThreadTags += "[已关闭]\n";
                    }
                    if (thTag.Contains("投票"))
                    {
                        ThreadTags += "[投票帖]\n";
                    }
                    if (thTag.Contains("辩论"))
                    {
                        ThreadTags += "[辩论帖]\n";
                    }
                    if (thTag.Contains("悬赏"))
                    {
                        ThreadTags += "[悬赏帖]\n";
                    }

                    var getTittle = string.Format("document.querySelectorAll(\"table[summary*='forum']\")[0].querySelectorAll('th')[{0}].innerText;", i);
                    string ThreadTittle = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getTittle });

                    var getPost = string.Format("document.querySelectorAll(\"table[summary*='forum']\")[0].querySelectorAll('tbody')[{0}].querySelectorAll('.by')[0].innerText;", i);
                    string TPostby = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getPost });

                    var getCV = string.Format("document.querySelectorAll(\"table[summary*='forum']\")[0].querySelectorAll('tbody')[{0}].querySelectorAll('td')[2].innerText;", i);
                    string ThreadCV = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getCV });

                    var getLastPost = string.Format("document.querySelectorAll(\"table[summary*='forum']\")[0].querySelectorAll('tbody')[{0}].querySelectorAll('.by')[1].innerText;", i);
                    string TLastpost = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getLastPost });

                    Uri ThreadURL = null;
                    if(i==0)
                    {
                        ThreadURL = new Uri("http://bbs.pcbeta.com/viewthread-1079227-1-1.html");
                    }
                    else
                    {
                        var getURL = string.Format(@"document.querySelectorAll('.xst')[{0}].href;", i);
                        ThreadURL = new Uri(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getURL }));

                        var getHL = string.Format(@"document.querySelectorAll('.xst')[{0}].style.color;", i);
                        string thHighlight = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getHL });
                        if(thHighlight.Contains("rgb"))
                        {
                            ThreadTags += "【已高亮】\n";
                        }
                        var getBL = string.Format(@"document.querySelectorAll('.xst')[{0}].style.fontWeight;", i);
                        string thBold = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getBL });
                        if(thBold.Contains("bold"))
                        {
                            ThreadTags += "【已加粗】\n";
                        }

                    }


                    ThreadList.Items.Add(new ThreadsIfm { Tags = ThreadTags, Tittle = ThreadTittle, Postby = TPostby, CheckViewNum = ThreadCV, LastReply = TLastpost, ThrUri = ThreadURL });
                }

            }
            catch(Exception)
            {

            }

            滚动条.IsActive = false;
            滚动条.Visibility = (Visibility)1;
        }      
        private async void 发布_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var get = string.Format(@"document.getElementById('newspecial').click();");
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { get });  
            }
            catch(Exception)
            {
                MainPage.CurrentPage.ShowNotifi("加载失败，请稍后再试!");
                return;
            }

            发布.IsEnabled = false;

            if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
            {
                发布.IsEnabled = true;
                return;
            }

            try
            {
                var check = string.Format(@"document.querySelectorAll('#messagetext').length.toString();");
                string msg = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check });

                if (msg == "1")
                {
                    var chkAccess = string.Format(@"document.querySelector('#messagetext').innerText;");
                    msg = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { chkAccess });

                    if (msg.Contains("未实名用户不能使用该功能"))
                    {
                        MainPage.CurrentPage.ShowNotifi("请登录您的帐号，若已登录请先实名认证!");
                    }
                    else if (msg.Contains("没有权限"))
                    {
                        MainPage.CurrentPage.ShowNotifi("您没有权限在此板块发帖！如有疑问请联系管理员.");
                    }
                    else if (msg.Contains("管理员设置"))
                    {
                        MainPage.CurrentPage.ShowNotifi("论坛在北京时间 03:00--05:00 之间不能发帖，请在此时间段外发帖！");
                    }
                    else
                    {
                        MainPage.CurrentPage.ShowNotifi("发生错误，请稍后再试!");
                    }

                    MainPage.CurrentPage.WebView1.Source = pageURL;
                    发布.IsEnabled = true;
                    return;
                }

            }
            catch
            {
                MainPage.CurrentPage.ShowNotifi("加载失败，请稍后再试!");
                发布.IsEnabled = true;
                return;
            }

            TPageContent.Visibility = (Visibility)1;
            CloseWV.Visibility = 0;
            MainPage.CurrentPage.WebView1.Visibility = 0;

            /*
                         try
            {
                var getCount = string.Format(@"document.querySelectorAll('.sltm li').length.toString();");
                string thFL = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getCount });
                int count = Convert.ToInt32(thFL);

                for (int i = 1; i < count; i++)
                {
                    var getName = string.Format(@"document.querySelectorAll('.sltm li')[{0}].innerText;", count);
                    string FLName = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getName });

                    ThreadFL.Items.Add(FLName);
                }
            }
            catch (Exception)
            {
                MainPage.CurrentPage.WebView1.Source = pageURL;
                await new MessageDialog("加载板块信息失败，请刷新页面", "提示信息").ShowAsync();
                发布.IsEnabled = true;
                return;
            }
             */

            //await 发帖页.ShowAsync();
            发布.IsEnabled = true;

            MainPage.CurrentPage.WebView1.NavigationStarting += WebView1_NavigationStarting;
        }
        public void CloseWV_Click(object sender, RoutedEventArgs e)
        {
            //跳转到帖子
            MainPage.CurrentPage.WebView1.Source = pageURL;
            ThreadCollapse();
        }
        public async void ThreadCollapse(string webviewURL="")
        {
            CloseWV.Visibility = (Visibility)1;
            if(webviewURL.Contains("bbs.pcbeta.com/forum.php?mod=post&action=newthread"))
            {
                await Task.Delay(1000);
                MainPage.CurrentPage.WebView1.Source = pageURL;
            }
            MainPage.CurrentPage.WebView1.Visibility = (Visibility)1;
            TPageContent.Visibility = 0;
        }
        private void WebView1_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            ThreadCollapse(MainPage.CurrentPage.WebView1.Source.ToString());
            MainPage.CurrentPage.WebView1.NavigationStarting -= WebView1_NavigationStarting;
        }
        private void 刷新_Click(object sender, RoutedEventArgs e)
        {
            MainPage.CurrentPage.WebView1.Source = pageURL;
            Page_Loaded(sender, e);
        }

        private async void 发帖页_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                var sendType = string.Format(@"document.querySelectorAll('#typeid_ctrl_menu li')[{0}].click();", ThreadFL.SelectedIndex+1);
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { sendType });
                var sendTitle = string.Format(@"document.querySelector('#subject').value='{0}';", 发帖标题.Text);
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { sendTitle });

                内容.Document.GetText(Windows.UI.Text.TextGetOptions.UseLf, out string aaa);
                var sendContent = string.Format(@"document.querySelector('#e_iframe').contentWindow.document.querySelector('body').innerText='{0}';", aaa);
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { sendContent });

                var sendSubmit = string.Format(@"document.querySelector('#postsubmit').click();");
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { sendSubmit });

                await Task.Delay(2000);

                if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
                {
                    MainPage.CurrentPage.ShowNotifi("sorry 发送请求超时,请稍后重试");
                    return;
                }

                var getResult = string.Format(@"document.getElementById('ct').innerText;");
                string submitResult = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getResult });

                if (submitResult.Contains("您的主题已发布"))
                {
                    MainPage.CurrentPage.ShowNotifi("发帖成功!即将跳转到帖子页面");
                }
                else if (submitResult.Contains("未实名用户不能使用该功能"))
                {
                    MainPage.CurrentPage.ShowNotifi("发帖失败！可能是以下原因：未登录账号/未实名认证/无权限发帖/禁止发贴");
                }
                else if (submitResult.Contains("需要审核"))
                {
                    MainPage.CurrentPage.ShowNotifi("Oh!这令人厌恶的审核，您的帖子已进入审核池，请等待通过");
                }
                else
                {
                    MainPage.CurrentPage.ShowNotifi("发帖失败!");
                }

                //跳转到帖子
                //MainPage.CurrentPage.WebView1.Source = new Uri("http://bbs.pcbeta.com");
                发帖页.Hide();
            }
            catch(Exception)
            {

            }
        }

        private void 发帖页_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            MainPage.CurrentPage.WebView1.Source = pageURL;
        }
        private void FTFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string fontData = "";
            switch (FTFont.SelectedIndex)
            {
                case 0:
                    fontData = "仿宋";
                    break;
                case 1:
                    fontData = "黑体";
                    break;
                case 2:
                    fontData = "楷体";
                    break;
                case 3:
                    fontData = "宋体";
                    break;
                case 4:
                    fontData = "新宋体";
                    break;
                case 5:
                    fontData = "微软雅黑";
                    break;
            }

            if (内容.Document.Selection.Length != 0)
            {
                内容.Document.Selection.Text = "[font=" + fontData + "]" + 内容.Document.Selection.Text + "[/font]";
            }
            else
            {
                内容.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[font=" + fontData + "]   [/font]");
            }
            FTFont.ClearValue(ListView.SelectedIndexProperty);
        }
        private void FtFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string fontsizeData = "";
            switch (FTFontSize.SelectedIndex)
            {
                case 0:
                    fontsizeData = "1";
                    break;
                case 1:
                    fontsizeData = "2";
                    break;
                case 2:
                    fontsizeData = "3";
                    break;
                case 3:
                    fontsizeData = "4";
                    break;
                case 4:
                    fontsizeData = "5";
                    break;
                case 5:
                    fontsizeData = "6";
                    break;
                case 6:
                    fontsizeData = "7";
                    break;
            }

            if (内容.Document.Selection.Length != 0)
            {
                内容.Document.Selection.Text = "[size=" + fontsizeData + "]" + 内容.Document.Selection.Text + "[/size]";
            }
            else
            {
                内容.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[size=" + fontsizeData + "]   [/size]");
            }
            FTFontSize.ClearValue(ListView.SelectedIndexProperty);
        }
        private void FTBlod_Click(object sender, RoutedEventArgs e)
        {
            if (内容.Document.Selection.Length != 0)
            {
                内容.Document.Selection.Text = "[b]" + 内容.Document.Selection.Text + "[/b]";
            }
            else
            {
                内容.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[b]   [/b]");
            }
        }

        private void FTFi_Click(object sender, RoutedEventArgs e)
        {
            if (内容.Document.Selection.Length != 0)
            {
                内容.Document.Selection.Text = "[i]" + 内容.Document.Selection.Text + "[/i]";
            }
            else
            {
                内容.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[i]   [/i]");
            }
        }

        private void FTLine_Click(object sender, RoutedEventArgs e)
        {
            if (内容.Document.Selection.Length != 0)
            {
                内容.Document.Selection.Text = "[u]" + 内容.Document.Selection.Text + "[/u]";
            }
            else
            {
                内容.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[u]   [/u]");
            }
        }
        private void FTColor_Click(object sender, RoutedEventArgs e)
        {
            if (内容.Document.Selection.Length != 0)
            {
                内容.Document.Selection.Text = "[color=请输入颜色英文或编码]" + 内容.Document.Selection.Text + "[/color]";
            }
            else
            {
                内容.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[color=请输入颜色英文或编码]   [/color]");
            }
        }

        private void FTBgcolor_Click(object sender, RoutedEventArgs e)
        {
            if (内容.Document.Selection.Length != 0)
            {
                内容.Document.Selection.Text = "[backcolor=请输入颜色英文或编码]" + 内容.Document.Selection.Text + "[/backcolor]";
            }
            else
            {
                内容.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[backcolor=请输入颜色英文或编码]   [/backcolor]");
            }
        }

        private void FTLink_Click(object sender, RoutedEventArgs e)
        {
            if (内容.Document.Selection.Length != 0)
            {
                内容.Document.Selection.Text = "[url=超链接]" + 内容.Document.Selection.Text + "[/url]";
            }
            else
            {
                内容.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[url=超链接]   [/url]");
            }
        }
        private void FTCode_Click(object sender, RoutedEventArgs e)
        {
            if (内容.Document.Selection.Length != 0)
            {
                内容.Document.Selection.Text = "[code]" + 内容.Document.Selection.Text + "[/code]";
            }
            else
            {
                内容.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[code]   [/code]");
            }
        }

        private void FTQuote_Click(object sender, RoutedEventArgs e)
        {
            if (内容.Document.Selection.Length != 0)
            {
                内容.Document.Selection.Text = "[quote]" + 内容.Document.Selection.Text + "[/quote]";
            }
            else
            {
                内容.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[quote]   [/quote]");
            }
        }

        private void FTLeft_Click(object sender, RoutedEventArgs e)
        {
            if (内容.Document.Selection.Length != 0)
            {
                内容.Document.Selection.Text = "[align=left]" + 内容.Document.Selection.Text + "[/align]";
            }
            else
            {
                内容.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[align=left]   [/align]");
            }
        }

        private void FTCenter_Click(object sender, RoutedEventArgs e)
        {
            if (内容.Document.Selection.Length != 0)
            {
                内容.Document.Selection.Text = "[align=center]" + 内容.Document.Selection.Text + "[/align]";
            }
            else
            {
                内容.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[align=center]   [/align]");
            }
        }

        private void FTRight_Click(object sender, RoutedEventArgs e)
        {
            if (内容.Document.Selection.Length != 0)
            {
                内容.Document.Selection.Text = "[align=right]" + 内容.Document.Selection.Text + "[/align]";
            }
            else
            {
                内容.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[align=right]   [/align]");
            }
        }
        private void BQ_Loaded(object sender, RoutedEventArgs e)
        {
            BQ1.Text = "{:5_260:}至{:5_299:}；{:5_586:}至{:5_597:}";
            BQ2.Text = "{:9_348:}至{:9_422:}；{:9_598:}至{:9_640:}";
            BQ3.Text = "{:7_423:}至{:7_507:}";
            BQ4.Text = "{:8_508:}至{:8_555:}";
        }
        private async void 上传_Click(object sender, RoutedEventArgs e)
        {
            SelectIMGCount.ClearValue(TextBlock.TextProperty);
            try
            {
                var getCount = string.Format(@"document.querySelectorAll('.filedata').length.toString();");
                string reNum = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getCount });
                int count = Convert.ToInt32(reNum)-1;

                var sendComm = string.Format(@"document.querySelectorAll('.filedata')[{0}].click();", count);
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { sendComm });

                await Task.Delay(1000);
                var checkVaild = string.Format(@"document.querySelectorAll('.alert_error').length.toString();");
                string result= await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { checkVaild });
                if(result=="1")
                {
                    SelectIMGCount.Text = "请选择图片文件！(jpg, gif, png)";
                    return;
                }

                SelectIMGCount.Text = "您已选择了" + count + "张图片,点击上传按钮上传您的图片";
            }
            catch (Exception)
            {
                SelectIMGCount.Text = "操作已取消";
            }

        }

        private async void 开始_Click(object sender, RoutedEventArgs e)
        {
            SelectIMGCount.ClearValue(TextBlock.TextProperty);
            UploadIMGCount.ClearValue(TextBlock.TextProperty);
            try
            {
                var getCount = string.Format(@"document.querySelectorAll('.filedata').length.toString();");
                string reNum = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getCount });
                if(reNum=="2")
                {
                    SelectIMGCount.Text = "您还没有选择图片";
                    return;
                }

                var get = string.Format(@"document.getElementsByClassName('pn pnc vm')[0].click();");
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { get });

                a1();
            }
            catch(Exception)
            {

            }
        }
        public async void a1()
        {
            UploadIMGRing.Visibility = 0;
            UploadIMGRing.IsActive = true;
            UploadIMGCount.Text = "正在上传...";

            try
            {
                UploadIMGList.Items.Clear();

                for (int i = 0; i < 60; i++)
                {
                    var get = string.Format(@"document.querySelectorAll('.flb').length.toString();");
                    string result = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { get });
                    if (result == "1")
                    {
                        break;
                    }

                    var checkVaild = string.Format(@"document.querySelectorAll('.alert_error').length.toString();");
                    string checkResult = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { checkVaild });
                    if (checkResult == "1")
                    {
                        checkVaild = string.Format(@"document.querySelectorAll('.alert_error')[0].innerText;");
                        checkResult = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { checkVaild });
                        if(checkResult.Contains("用户组限制"))
                        {
                            UploadIMGCount.Text = "用户组限制无法上传那么大的图片";
                        }
                        else if (checkResult.Contains("内部错误"))
                        {
                            UploadIMGCount.Text = "论坛服务器内部错误，请稍后重试";
                        }
                        else
                        {
                            UploadIMGCount.Text = "上传错误！";
                        }

                        UploadIMGRing.Visibility = (Visibility)1;
                        UploadIMGRing.IsActive = false;

                        return;
                    }

                    if (i == 59)
                    {
                        UploadIMGCount.Text = "上传超时";

                        UploadIMGRing.Visibility = (Visibility)1;
                        UploadIMGRing.IsActive = false;

                        return;
                    }

                    await Task.Delay(1000);
                }
            }
            catch(Exception)
            {
                UploadIMGRing.Visibility = (Visibility)1;
                UploadIMGRing.IsActive = false;
                return;
            }

            try
            {
                UploadIMGCount.Text = "上传成功，即将显示您的图片";
                await Task.Delay(2000);

                int count = 0;
                for(int i=0;i<60;i++)
                {
                    string get1 = string.Format(@"document.querySelectorAll('.imgl img').length.toString();");
                    string result1 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { get1 });
                    count = Convert.ToInt32(result1);
                    if(count!=0)
                    {
                        UploadIMGCount.Text = "您已上传" + count + "张图片：";
                        break;
                    }
                    if(i==59)
                    {
                        UploadIMGCount.Text = "已全部成功上传，但加载您的图片超时";
                    }
                    await Task.Delay(1000);
                }

                UploadIMGRing.Visibility = (Visibility)1;
                UploadIMGRing.IsActive = false;

                for (int i = 0; i < count; i++)
                {
                    var get2 = string.Format(@"document.querySelectorAll('.imgl img')[{0}].src", i);
                    Uri result2 = new Uri(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { get2 }));

                    var get3 = string.Format(@"document.querySelectorAll('.imgl img')[{0}].id", i);
                    string result3 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { get3 });
                    result3 = "delImgAttach(" + result3.Replace("image_", "")+")";
                    string result4 = "[attachimg]" + result3.Replace("image_", "") + "[/attachimg]";

                    var bitmapImage = new BitmapImage(result2);

                    UploadIMGList.Items.Add(new UploadImage { PreviewIMG=bitmapImage, DeleteIMG =result3,InsertIMG=result4});
                }

            }
            catch(Exception)
            {
                MainPage.CurrentPage.ShowNotifi("连接服务器失败,请稍后重试!");
            }

        }
        private void 添加_Click(object sender, RoutedEventArgs e)
        {
            var abcde = UploadIMGList.SelectedItem as UploadImage;
            if (内容.Document.Selection.Length != 0)
            {
                内容.Document.Selection.Text = abcde.InsertIMG;
            }
            else
            {
                内容.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, abcde.InsertIMG);
            }
        }
        private async void 删除1_Click(object sender, RoutedEventArgs e)
        {
            var abcde = UploadIMGList.SelectedItem as UploadImage;
            await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { abcde.DeleteIMG });
            UploadIMGList.Items.Remove(UploadIMGList.SelectedItem);
        }
        private void UploadIMGList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(UploadIMGList.SelectedIndex<0)
            {
                操作1.Visibility = Visibility.Collapsed;
            }
            else
            {
                操作1.Visibility = Visibility.Visible;
            }
        }

        private async void 上传2_Click(object sender, RoutedEventArgs e)
        {
            SelectFileCount.ClearValue(TextBlock.TextProperty);
            try
            {
                var getCount = string.Format(@"document.querySelector('#attachbtn').childElementCount.toString();");
                string reNum = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getCount });
                int count = Convert.ToInt32(reNum);

                var sendComm = string.Format(@"document.querySelector('#attachbtn').querySelectorAll('.fldt')[{0}].click();", count-1);
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { sendComm });

                await Task.Delay(1000);
                var checkVaild = string.Format(@"document.querySelectorAll('.alert_error').length.toString();");
                string result = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { checkVaild });
                if (result == "1")
                {
                    SelectFileCount.Text = "不支持上传此类附件！";
                    return;
                }

                SelectFileCount.Text = "您已选择了" + count + "个文件,点击上传按钮上传您的文件";
            }
            catch (Exception)
            {
                SelectFileCount.Text = "操作已取消";
            }
        }

        private async void 开始2_Click(object sender, RoutedEventArgs e)
        {
            SelectFileCount.ClearValue(TextBlock.TextProperty);
            UploadFileCount.ClearValue(TextBlock.TextProperty);
            try
            {
                var getCount = string.Format(@"document.querySelector('#attachbtn').childElementCount.toString();");
                string reNum = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getCount });
                if(reNum=="1")
                {
                    SelectFileCount.Text = "您还没有选择文件";
                    return;
                }

                var get = string.Format(@"document.getElementsByClassName('pn pnc vm')[1].click();");
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { get });

                a2();
            }
            catch (Exception)
            {

            }
        }
        public async void a2()
        {
            UploadFileRing.Visibility = 0;
            UploadFileRing.IsActive = true;
            UploadFileCount.Text = "正在上传...";

            try
            {
                SCFJList.Items.Clear();

                for (int i = 0; i < 120; i++)
                {
                    var get = string.Format(@"document.querySelectorAll('.flb').length.toString();");
                    string result = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { get });
                    if (result == "1")
                    {
                        break;
                    }

                    var checkVaild = string.Format(@"document.querySelectorAll('.alert_error').length.toString();");
                    string checkResult = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { checkVaild });
                    if (checkResult == "1")
                    {
                        checkVaild = string.Format(@"document.querySelectorAll('.alert_error')[0].innerText;");
                        checkResult = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { checkVaild });
                        if (checkResult.Contains("用户组限制"))
                        {
                            UploadFileCount.Text = "用户组限制无法上传那么大的附件";
                        }
                        else if (checkResult.Contains("内部错误"))
                        {
                            UploadFileCount.Text = "论坛服务器内部错误，请稍后重试";
                        }
                        else
                        {
                            UploadFileCount.Text = "上传错误！";
                        }

                        UploadFileRing.Visibility = (Visibility)1;
                        UploadFileRing.IsActive = false;

                        return;
                    }

                    if (i == 119)
                    {
                        UploadFileCount.Text = "上传超时";

                        UploadFileRing.Visibility = (Visibility)1;
                        UploadFileRing.IsActive = false;

                        return;
                    }

                    await Task.Delay(1000);
                }
            }
            catch(Exception)
            {
                UploadFileRing.Visibility = (Visibility)1;
                UploadFileRing.IsActive = false;
                return;
            }

            try
            {
                UploadFileCount.Text = "附件上传成功，即将显示您的附件";
                await Task.Delay(2000);

                int count = 0;
                for (int i = 0; i < 60; i++)
                {
                    string get1 = string.Format(@"document.querySelectorAll('#attachlist tbody').length.toString();");
                    string result1 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { get1 });
                    count = Convert.ToInt32(result1);
                    if (count != 0)
                    {
                        UploadFileCount.Text = "您已上传了" + count + "个附件：";
                        break;
                    }
                    if (i == 59)
                    {
                        UploadFileCount.Text = "已全部成功上传附件，但加载您的附件超时";
                    }
                    await Task.Delay(1000);
                }

                UploadFileRing.Visibility = (Visibility)1;
                UploadFileRing.IsActive = false;

                for (int i = 0; i < count; i++)
                {
                    var getAttID = string.Format(@"document.querySelectorAll('#attachlist tbody')[{0}].id.replace('attach_','');", i);
                    string attID = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getAttID });

                    var getAttTitle = string.Format(@"document.querySelector('#attachname{0}').innerText", attID);
                    string FJTitle = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getAttTitle });

                    var getReadAcc = string.Format(@"document.querySelectorAll('#attachlist tbody #readperm')[{0}].value;", i);
                    string FJRead = "权限:" + await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getReadAcc });
                    if(FJRead=="权限:")
                    {
                        FJRead += "0";
                    }

                    var getPrice = string.Format(@"document.querySelectorAll('#attachlist tbody .attpr .px')[{0}].value;", i);
                    string FJPrice = "售价:" + await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getPrice });
                    if (FJPrice == "售价:")
                    {
                        FJPrice += "0";
                    }

                    string FJIns = "[attach]" + attID + "[/attach]";
                    string FJRemove = "delAttach(" + attID + ")";

                    SCFJList.Items.Add(new ThrFiles { FileName = FJTitle, ViewLVL = FJRead, Price = FJPrice, Insert = FJIns, Delete = FJRemove, FilesID = attID });
                }

            }
            catch (Exception)
            {
                MainPage.CurrentPage.ShowNotifi("连接服务器失败,请稍后重试!");
            }

        }
        private void 添加2_Click(object sender, RoutedEventArgs e)
        {
            var abcde = SCFJList.SelectedItem as ThrFiles;
            if (内容.Document.Selection.Length != 0)
            {
                内容.Document.Selection.Text = abcde.Insert;
            }
            else
            {
                内容.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, abcde.Insert);
            }
        }

        private async void 删除2_Click(object sender, RoutedEventArgs e)
        {
            var abcde = SCFJList.SelectedItem as ThrFiles;
            await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { abcde.Delete });
            SCFJList.Items.Remove(SCFJList.SelectedItem);
        }

        private void SCFJList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SCFJList.SelectedIndex<0)
            {
                操作2.Visibility = (Visibility)1;
                操作2b.Visibility = (Visibility)1;
            }
            else
            {
                设定1.Text = (SCFJList.SelectedItem as ThrFiles).ViewLVL.Replace("权限:","");
                设定2.Text = (SCFJList.SelectedItem as ThrFiles).Price.Replace("售价:","");
                操作2.Visibility = 0;
                操作2b.Visibility = 0;
            }
        }
        int[] qxCount = { 0,9,10,20,30,40,50,70,100,120,140,160,200,205,210,220,225,235,250,255};
        private async void 更新_Click(object sender, RoutedEventArgs e)
        {
            SDError.ClearValue(TextBlock.TextProperty);
            if (string.IsNullOrEmpty(设定1.Text))
            {
                设定1.Text = "0";
            }
            if (string.IsNullOrEmpty(设定2.Text))
            {
                设定2.Text = "0";
            }

            try
            {
                int sd1 = Convert.ToInt16(设定1.Text);
                int sd2 = Convert.ToInt16(设定2.Text);

                var aaa = from bbb in qxCount where sd1 == bbb select bbb;
                if (aaa.ToList().Count == 0)
                {
                    SDError.Text += "设定的阅读权限值不正确";
                    return;
                }
                if (sd2 < 0 || sd2 > 4)
                {
                    SDError.Text += "单个附件最多只能售价4PB";
                    return;
                }

                var abcde = (SCFJList.SelectedItem as ThrFiles).FilesID;
                var sent1 = string.Format(@"document.getElementsByName('attachnew[{0}][readperm]')[0].value='{1}';", abcde, 设定1.Text);
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { sent1 });
                var sent2 = string.Format(@"document.getElementsByName('attachnew[{0}][price]')[0].value='{1}';", abcde, 设定2.Text);
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { sent2 });

            }
            catch(Exception)
            {

            }

            SCFJList.Items.Clear();

            string getCount = string.Format(@"document.querySelectorAll('#attachlist tbody').length.toString();");
            string result = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getCount });
            int count = Convert.ToInt32(result);
            for (int i = 0; i < count; i++)
            {
                var getAttID = string.Format(@"document.querySelectorAll('#attachlist tbody')[{0}].id.replace('attach_','');", i);
                string attID = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getAttID });

                var getAttTitle = string.Format(@"document.querySelector('#attachname{0}').innerText", attID);
                string FJTitle = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getAttTitle });

                var getReadAcc = string.Format(@"document.querySelectorAll('#attachlist tbody #readperm')[{0}].value;", i);
                string FJRead = "权限:" + await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getReadAcc });
                if (FJRead == "权限:")
                {
                    FJRead += "0";
                }

                var getPrice = string.Format(@"document.querySelectorAll('#attachlist tbody .attpr .px')[{0}].value;", i);
                string FJPrice = "售价:" + await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getPrice });
                if (FJPrice == "售价:")
                {
                    FJPrice += "0";
                }

                string FJIns = "[attach]" + attID + "[/attach]";
                string FJRemove = "delAttach(" + attID + ")";

                SCFJList.Items.Add(new ThrFiles { FileName = FJTitle, ViewLVL = FJRead, Price = FJPrice, Insert = FJIns, Delete = FJRemove, FilesID = attID });
            }

            UploadFileCount.Text = "已成功更新";
        }

        private void ThreadList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                MainPage.CurrentPage.WebView1.Source = (ThreadList.SelectedItem as ThreadsIfm).ThrUri;
                //await Task.Delay(1000);
                this.Frame.Navigate(typeof(ThreadContentPage));
            }
            catch(Exception)
            {
                MainPage.CurrentPage.ShowNotifi("打开帖子时出了点小问题，请重试");
            }
        }
    }
}
