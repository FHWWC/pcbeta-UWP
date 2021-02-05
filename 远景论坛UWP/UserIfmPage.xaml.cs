using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
    public sealed partial class UserIfmPage : Page
    {
        public UserIfmPage()
        {
            this.InitializeComponent();
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(!MainPage.CurrentPage.noCheck)
            {
                if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
                {
                    return;
                }
            }

            RefreshMessage refresh = new RefreshMessage();
            refresh.CheckNewMessage();

            MainPage.CurrentPage.noCheck = false;

            try
            {
                var getCount = string.Format(@"document.getElementsByClassName('bm_c')[0].querySelectorAll('.mbn').length.toString();");
                string result = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getCount });
                int count = Convert.ToInt32(result);

                for (int i = 0; i < count; i++)
                {
                    var getTittle = string.Format(@"document.getElementsByClassName('bm_c')[0].querySelectorAll('.mbn')[{0}].innerText;",i);
                    string tittle = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getTittle });

                    if(tittle.Contains("勋章"))
                    {
                        var getCount2 = string.Format(@"document.querySelectorAll('.bm_c .md_ctrl img').length.toString();");
                        string result2 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getCount2 });
                        int count2 = Convert.ToInt32(result2);

                        for(int j=0;j<count2;j++)
                        {
                            var send = string.Format(@"document.querySelectorAll('.bm_c .md_ctrl img')[{0}].alt;",j);
                            string get = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                            var send2 = string.Format(@"document.querySelectorAll('.bm_c .md_ctrl img')[{0}].src;", j);
                            string get2 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send2 });

                            Image image = new Image();
                            image.Height = image.Width = 100;
                            image.Source = new BitmapImage(new Uri(get2));

                            StackPanel stackPanel = new StackPanel();
                            stackPanel.Children.Add(image);
                            stackPanel.Children.Add(new TextBlock() { Text = get });
                            stackPanel.Margin = new Thickness(0,0,20,0);

                            XZList.Children.Add(stackPanel);
                        }

                        XZPanel.Visibility = 0;
                    }
                    else if (tittle.Contains("管理以下版块"))
                    {
                        var send = string.Format(@"document.querySelector('.bm_c').children[{0}].innerText.replace('管理以下版块','');", i);
                        userAdmin.Text= await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                        BZPanel.Visibility = 0;
                    }
                    else if (tittle.Contains("活跃概况"))
                    {
                        var send = string.Format(@"document.querySelector('.bm_c').children[{0}].querySelector('ul').querySelectorAll('img').length.toString();", i);
                        string get = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });

                        if(get=="1")
                        {
                            var send2 = string.Format(@"document.querySelector('.bm_c').children[{0}].querySelector('ul').querySelector('li').innerText;", i);
                            var send3 = string.Format(@"document.querySelector('.bm_c').children[{0}].querySelector('ul').querySelector('img').src;", i);
                            string get3 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send3 });

                            UGroupPanel.Children.Add(new TextBlock() { Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send2 }) });
                            UGroupPanel.Children.Add(new Image() { Source = new BitmapImage(new Uri(get3)) });
                        }
                        else
                        {
                            var send2 = string.Format(@"document.querySelector('.bm_c').children[{0}].querySelector('ul').querySelectorAll('li')[0].innerText;", i);
                            var send3 = string.Format(@"document.querySelector('.bm_c').children[{0}].querySelector('ul').querySelectorAll('img')[0].src;", i);
                            string get3 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send3 });
                            UGroupPanel.Children.Add(new TextBlock() { Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send2 }) });
                            UGroupPanel.Children.Add(new Image() { Source = new BitmapImage(new Uri(get3)) });

                            send2 = string.Format(@"document.querySelector('.bm_c').children[{0}].querySelector('ul').querySelectorAll('li')[1].innerText;", i);
                            send3 = string.Format(@"document.querySelector('.bm_c').children[{0}].querySelector('ul').querySelectorAll('img')[1].src;", i);
                            get3 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send3 });
                            UGroupPanel.Children.Add(new TextBlock() { Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send2 }) });
                            UGroupPanel.Children.Add(new Image() { Source = new BitmapImage(new Uri(get3)) });
                        }

                        var send4 = string.Format(@"document.getElementById('pbbs').innerText;");
                        userData.Text= await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send4 });
                    }
                    else if (tittle.Contains("统计信息"))
                    {
                        var send = string.Format(@"document.querySelector('#psts ul').innerText;");
                        userAcc.Text +="\n\n"+ await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                    }
                    else
                    {
                        var ckonline = string.Format(@"document.getElementsByClassName('bm_c')[0].querySelectorAll('.mbn img').length.toString();");
                        string isonline = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { ckonline });

                        var username = string.Format(@"document.getElementsByClassName('bm_c')[0].querySelectorAll('.mbn')[0].innerText;");
                        userAcc.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { username });

                        if (isonline=="1")
                        {
                            userAcc.Text += " 当前在线";
                        }
                        var other = string.Format("document.getElementsByClassName('bm_c')[0].children[0].children[1].innerText;");
                        userAcc.Text += "\n\n"+ await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { other });
                        var mydata = string.Format(@"document.getElementsByClassName('bm_c')[0].children[0].children[2].innerText;");
                        userIfm.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { mydata });

                    }

                }
            }
            catch (Exception)
            {
                MainPage.CurrentPage.ShowNotifi("个人资料加载失败！请稍后重试。");
            }


        }
    }
}
