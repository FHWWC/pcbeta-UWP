using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
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
    public sealed partial class ShowNEWSPage : Page
    {
        public ShowNEWSPage()
        {
            this.InitializeComponent();
        }
        Uri get7a;
        Uri get7b;
        Uri get7c;
        Uri get7d;
        Uri get7e;
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
            {
                return;
            }

            RefreshMessage refresh = new RefreshMessage();
            refresh.CheckNewMessage();

            try
            {
                var check1 = string.Format(@"document.getElementById('article_content').getElementsByTagName('img').length.toString();");
                string result1a = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check1 });
                int result1b = Convert.ToInt16(result1a);

                for (int i = 0; i < result1b; i++)
                {
                    var check2 = string.Format(@"document.getElementsByClassName('vwtb')[0].getElementsByTagName('img')[{0}].src;", i);
                    Uri result2 = new Uri(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check2 }));
                    var btmp = new BitmapImage(result2);

                    图片.Items.Add(new ShowNEWSc { Images = btmp });
                }

                var check3 = string.Format(@"document.getElementsByClassName('ph')[0].innerText;");
                标题.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check3 });
                var check4 = string.Format(@"document.getElementsByClassName('xg1')[0].innerText;");
                信息.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check4 });
                var check5 = string.Format(@"document.getElementById('article_content').innerText;");
                内容.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check5 });

                BTSelect();
                var check7a = string.Format(@"document.getElementsByClassName('atd')[0].getElementsByTagName('a')[0].href");
                get7a = new Uri(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check7a }));
                var check7b = string.Format(@"document.getElementsByClassName('atd')[0].getElementsByTagName('a')[1].href");
                get7b = new Uri(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check7b }));
                var check7c = string.Format(@"document.getElementsByClassName('atd')[0].getElementsByTagName('a')[2].href");
                get7c = new Uri(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check7c }));
                var check7d = string.Format(@"document.getElementsByClassName('atd')[0].getElementsByTagName('a')[3].href");
                get7d = new Uri(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check7d }));
                var check7e = string.Format(@"document.getElementsByClassName('atd')[0].getElementsByTagName('a')[4].href");
                get7e = new Uri(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check7e }));

            }
            catch (Exception)
            {
                MainPage.CurrentPage.ShowNotifi("抱歉，新闻内容加载失败，请稍后重试     加载失败");
            }

            GetComm();
        }
        public async void BTSelect()
        {
            try
            {
                var check6a = string.Format(@"document.getElementsByClassName('mbm xs1')[0].innerText;");
                BTS.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check6a });

                var check6 = string.Format(@"document.getElementsByClassName('atd')[0].getElementsByTagName('a')[0].innerText");
                BT1.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check6 });
                check6 = string.Format(@"document.getElementsByClassName('atd')[0].getElementsByTagName('a')[1].innerText");
                BT2.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check6 });
                check6 = string.Format(@"document.getElementsByClassName('atd')[0].getElementsByTagName('a')[2].innerText");
                BT3.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check6 });
                check6 = string.Format(@"document.getElementsByClassName('atd')[0].getElementsByTagName('a')[3].innerText");
                BT4.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check6 });
                check6 = string.Format(@"document.getElementsByClassName('atd')[0].getElementsByTagName('a')[4].innerText");
                BT5.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check6 });
            }
            catch(Exception)
            {
                MainPage.CurrentPage.ShowNotifi("抱歉，获取表态信息失败，请稍后重试     加载失败");
            }
        }
        public async void GetComm()
        {
            var result2 = "";
            string deleteComm ="";

            try
            {
                var check1 = string.Format(@"document.getElementsByClassName('ptm pbw bbda cl').length.toString();");
                var rs1 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check1 });
                int rs2 = Convert.ToInt16(rs1);

                for(int i=0;i<rs2;i++)
                {
                    var check3 = string.Format(@"document.getElementById('comment_ul').getElementsByClassName('mbm')[{0}].innerText;", i);
                    var rs4 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check3 });
                    string result3;
                    if (rs4.Contains("编辑"))
                    {
                        result3 = rs4.Remove(0, 8);
                        result3 += "（我的评论）";
                        deleteComm = string.Format(@"document.getElementsByClassName('ptm pbw bbda cl')[{0}].getElementsByTagName('a')[2].click();", i);
                    }
                    else
                    {
                        result3 = rs4.Remove(0, 2);
                        deleteComm = "";
                    }


                    var check2 = string.Format(@"document.getElementsByClassName('ptm pbw bbda cl')[{0}].getElementsByTagName('blockquote').length.toString();", i);
                    result2 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check2 });
                    int result2b = Convert.ToInt32(result2);
                    if(result2b>0)
                    {
                        check2 = string.Format(@"document.getElementsByClassName('ptm pbw bbda cl')[{0}].getElementsByTagName('blockquote')[0].innerText;", i);
                        result2 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check2 });
                        check2 = string.Format(@"document.getElementsByClassName('ptm pbw bbda cl')[{0}].getElementsByTagName('blockquote')[0].remove();", i);
                        await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check2 });
                        result2 = "回复了此评论：\n" + result2;
                    }
                    else
                    {
                        result2 = "";
                    }


                    var check4 = string.Format(@"document.getElementById('comment_ul').getElementsByTagName('dd')[{0}].innerText;", i);
                    var result4 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check4 });

                    CommList.Items.Add(new ShowNEWSc { CommentsIfm = result3, Quote = result2,Comments= result4,DeleteComm=deleteComm });   
                }

            }
            catch (Exception)
            {
                MainPage.CurrentPage.ShowNotifi("抱歉，读取评论失败，请重新加载页面     加载失败");
            }


        }
        private async void 表态_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch(button.Name)
            {
                case "表态1":
                    MainPage.CurrentPage.WebView1.Source = get7a;
                    break;
                case "表态2":
                    MainPage.CurrentPage.WebView1.Source = get7b;
                    break;
                case "表态3":
                    MainPage.CurrentPage.WebView1.Source = get7c;
                    break;
                case "表态4":
                    MainPage.CurrentPage.WebView1.Source = get7d;
                    break;
                case "表态5":
                    MainPage.CurrentPage.WebView1.Source = get7e;
                    break;
            }

            if (await MainPage.CurrentPage.CheckLoadState(false) != 0)
            {
                MainPage.CurrentPage.ShowNotifi("表态失败，请稍后再试");
                MainPage.CurrentPage.WebView1.GoBack();
                return;
            }

            try
            {
                var check1 = string.Format(@"document.getElementById('ct').innerText;");
                var result1 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check1 });
                if (result1.Contains("表态成功"))
                {
                    MainPage.CurrentPage.ShowNotifi("表态成功！");
                }
                else if (result1.Contains("您已表过态"))
                {
                    MainPage.CurrentPage.ShowNotifi("您已表过态,请勿重复表态！");
                }
                else if (result1.Contains("您需要先登录才能继续本操作"))
                {
                    MainPage.CurrentPage.ShowNotifi("请您先登陆账号！！");
                }
                else
                {
                    MainPage.CurrentPage.ShowNotifi("表态失败!");
                }
            }
            catch (Exception)
            {

            }

            MainPage.CurrentPage.WebView1.GoBack();
            BTSelect();
        }

        private async void 提交_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(评论.Text))
                {
                    return;
                }
                else if (评论.Text.Length < 2)
                {
                    MainPage.CurrentPage.ShowNotifi("评论太短!");
                    return;
                }

                var send1 = string.Format(@"document.getElementById('message').value='{0}';", 评论.Text);
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send1 });
                var send2 = string.Format(@"document.getElementById('commentsubmit_btn').click();");
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send2 });

                await Task.Delay(1000);
                if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
                {
                    return;
                }

                var send3 = string.Format(@"document.getElementById('ct').innerText;");
                var result3 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send3 });
                if (result3.Contains("操作成功"))
                {
                    评论.Text = "";
                    MainPage.CurrentPage.ShowNotifi("评论成功！");
                }
                else if (result3.Contains("未实名用户不能使用该功能"))
                {
                    MainPage.CurrentPage.ShowNotifi("请先完成实名认证,或者您没登陆账号！！");
                }
                else if (result3.Contains("两次发布操作太快"))
                {
                    MainPage.CurrentPage.ShowNotifi("你手速太快，休息一下吧");
                }
                else
                {
                    MainPage.CurrentPage.ShowNotifi("评论失败！请稍后重试！");
                }

                MainPage.CurrentPage.WebView1.GoBack();
                CommList.Items.Clear();
                if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
                {
                    return;
                }
                await Task.Delay(1000);
                GetComm();
            }
            catch(Exception)
            {
                MainPage.CurrentPage.ShowNotifi("评论失败！请稍后重试！");
            }
        }
        string quoteComm;
        string comments;
        string deleteComm;
        private void CommList_Tapped(object sender, TappedRoutedEventArgs e)
        {
            删除.Visibility = Visibility.Collapsed;

            var items = CommList.SelectedItem as ShowNEWSc;
            quoteComm = items.Quote;
            comments = items.Comments.Replace("\n", "");
            deleteComm = items.DeleteComm;
            string xinxi = items.CommentsIfm;
            if (xinxi.Contains("我的评论"))
            {
                删除.Visibility = Visibility.Visible;
            }


            Point point = e.GetPosition(sender as FrameworkElement);
            CommFlyout.ShowAt(sender as FrameworkElement, point);
        }
        DataPackage dataPackage = new DataPackage();
        private void 复制_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.SetText(quoteComm + "\n" + comments);
            Clipboard.SetContent(dataPackage);
        }
        private void 引用_Click(object sender, RoutedEventArgs e)
        {
            if(评论.Text.Contains("[quote]"))
            {
                评论.Text = "[quote]" + comments + "[/quote]";
            }
            else
            {
                评论.Text = "[quote]" + comments + "[/quote]" + "\\n" + 评论.Text;
            }
        }
        private async void 删除_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { deleteComm });
                await Task.Delay(1500);
                var send = string.Format(@"document.getElementsByName('deletesubmitbtn').length.toString();");
                var result= await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                if(result=="0")
                {
                    MainPage.CurrentPage.ShowNotifi("删除评论失败，请稍后重试！");
                    return;
                }

                var send2 = string.Format(@"document.getElementsByName('deletesubmitbtn')[0].click();");
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send2 });
                if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
                {
                    return;
                }

                var send3 = string.Format(@"document.getElementById('ct').innerText;");
                var result3 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send3 });
                if (result3.Contains("操作成功"))
                {
                    MainPage.CurrentPage.ShowNotifi("已删除");
                }
                else if (result3.Contains("您要删除的评论不存在"))
                {
                    MainPage.CurrentPage.ShowNotifi("该评论已被删除或不存在!");
                }
                else
                {
                    MainPage.CurrentPage.ShowNotifi("删除失败");
                }

                MainPage.CurrentPage.WebView1.GoBack();
                CommList.Items.Clear();
                if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
                {
                    return;
                }
                await Task.Delay(1000);
                GetComm();
            }
            catch(Exception)
            {
                MainPage.CurrentPage.ShowNotifi("删除评论失败，请稍后重试！");
            }
        }

    }
}
