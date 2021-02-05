using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
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
using Windows.UI.Xaml.Shapes;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 远景论坛UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ThreadContentPage : Page
    {
        public ThreadContentPage()
        {
            this.InitializeComponent();
        }
        Uri MainThrURL = null;
        Uri ReplyThrURL,EditURL = null;
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
            {
                return;
            }

            await Task.Delay(2000);
            RefreshMessage refresh = new RefreshMessage();
            refresh.CheckNewMessage();
            MainThrURL = MainPage.CurrentPage.WebView1.Source;

            try
            {
                var getBN = string.Format(@"document.querySelectorAll('.z a')[5].innerText;");
                BlockName.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getBN });
            }
            catch(Exception)
            {

            }

            try
            {
                //var getAvatar = string.Format(@"document.querySelectorAll('.pls')[2].getElementsByClassName('m z')[0].querySelector('img').src;");
                //string AvatarURL = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getAvatar });
                //LZAvatar.Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri(AvatarURL)) };
                var getUserName = string.Format(@"document.querySelector('.authi .xw1').innerText;");
                UserNameBtn.Content = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getUserName });
                getUserName = string.Format(@"document.querySelector('.authi .xw1').href;");
                UserNameBtn.Tag= new Uri(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getUserName }));

                var getUserIfm = string.Format(@"document.getElementsByClassName('i y')[0].innerText.replace('发消息',''); ");
                UserIfm.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getUserIfm });

                var getThrTitle = string.Format(@"document.querySelector('#thread_subject').innerText;");
                ThreadTitle.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getThrTitle });

                var getThrIfm = string.Format(@"document.querySelector('.authi em').innerText
+'        '+document.querySelector('.hm').innerText
+'        帖子ID：'+document.querySelector('#thread_subject').href.replace('http://bbs.pcbeta.com/viewthread-','').replace('.html','');");
                ThreadIfm.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getThrIfm });
            }
            catch (Exception)
            {
                MainPage.CurrentPage.ShowNotifi("抱歉,帖子主体信息获取失败,请尝试刷新页面.");
            }

            try
            {
                var getlink = string.Format(@"document.querySelectorAll('.fastre').length.toString();");
                string canuse = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getlink });
                if(canuse=="0")
                {
                    ReplyBtn.IsEnabled = false;
                    ReplyStatus.Visibility = 0;
                }
                else
                {
                    getlink = string.Format(@"document.querySelector('.fastre').href;");
                    string replylink = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getlink });
                    ReplyThrURL = new Uri(replylink);
                }

                getlink = string.Format(@"document.querySelectorAll('.editp').length.toString();");
                canuse = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getlink });
                if (canuse == "0")
                {
                    EditBtn.Visibility = (Visibility)1;
                }
                else
                {
                    getlink = string.Format(@"document.querySelector('.editp').href;");
                    string editlink = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getlink });
                    EditURL = new Uri(editlink);
                }

                getlink = string.Format(@"document.querySelector('#p_btn').innerText;");
                canuse = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getlink });
                if (!canuse.Contains("举报"))
                {
                    ReportBtn.Visibility = (Visibility)1;
                }

            }
            catch(Exception)
            {

            }

            try
            {
                var getThrContent = string.Format(@"document.querySelector('.t_f').innerText;");
                ThrContent.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getThrContent });

                var getCount = string.Format(@"document.querySelector('.t_f').querySelectorAll('ignore_js_op').length.toString();");
                string result = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getCount });
                int count = Convert.ToInt32(result);

                for(int i=0;i<count;i++)
                {
                    var getFJType = string.Format(@"document.querySelector('.t_f').querySelectorAll('ignore_js_op')[{0}].querySelectorAll('em').length.toString();", i);
                    string isIMG = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getFJType });
                    if (isIMG == "0")
                    {
                        var getIMGIfm = string.Format(@"document.querySelector('.t_f').querySelectorAll('ignore_js_op')[{0}].querySelector('img').title;", i);
                        string filename = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getIMGIfm });
                        getIMGIfm = string.Format(@"document.getElementsByClassName('tip_c xs0')[{0}].querySelector('a').href;", i);
                        Uri IMGURL = new Uri(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getIMGIfm }));

                        StackPanel stackPanel = new StackPanel();

                        stackPanel.Children.Add(new HyperlinkButton() { Content = filename, NavigateUri = IMGURL, HorizontalAlignment = HorizontalAlignment.Center });
                        stackPanel.Children.Add(new Image() { Source = new BitmapImage(IMGURL) });

                        ThrFJ.Children.Add(stackPanel);
                    }
                    else
                    {
                        var getFJIfm = string.Format(@"document.querySelector('.t_f').querySelectorAll('ignore_js_op')[{0}].querySelector('a').innerText;", i);
                        string filename = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getFJIfm });
                        getFJIfm = string.Format(@"document.querySelector('.t_f').querySelectorAll('ignore_js_op')[{0}].querySelector('a').href;", i);
                        string downloadlink = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getFJIfm });
                        getFJIfm = string.Format(@"document.querySelector('.t_f').querySelectorAll('ignore_js_op')[{0}].querySelector('em').innerText;", i);
                        string fileIfm = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getFJIfm });

                        StackPanel stackPanel = new StackPanel();
                        stackPanel.Orientation = Orientation.Horizontal;

                        stackPanel.Children.Add(new Image() { Source = new BitmapImage(new Uri("ms-appx:///SmallICON/File.png")) });
                        stackPanel.Children.Add(new HyperlinkButton() { NavigateUri = new Uri(downloadlink), Content = filename });
                        stackPanel.Children.Add(new TextBlock() { Text = fileIfm, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center }); ;

                        ThrFJ.Children.Add(stackPanel);
                    }
                }


                string checkFJ = string.Format(@"document.querySelector('.pcb').querySelectorAll('.pattl').length.toString();");
                string ishaveFJ = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { checkFJ });
                if(ishaveFJ=="1")
                {
                    getCount = string.Format(@"document.querySelector('.pattl').querySelectorAll('ignore_js_op').length.toString();");
                    result = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getCount });
                    count = Convert.ToInt32(result);

                    for (int i = 0; i < count; i++)
                    {
                        var getFJType = string.Format(@"document.querySelector('.pattl').querySelectorAll('ignore_js_op')[{0}].querySelectorAll('em').length.toString();", i);
                        string isIMG = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getFJType });
                        if (isIMG == "0")
                        {
                            var getFJIfm = string.Format(@"document.querySelector('.pattl').querySelectorAll('ignore_js_op')[{0}].querySelector('a').innerText;", i);
                            string filename = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getFJIfm });
                            getFJIfm = string.Format(@"document.querySelector('.pattl').querySelectorAll('ignore_js_op')[{0}].querySelector('a').href;", i);
                            string downloadlink = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getFJIfm });
                            getFJIfm = string.Format(@"document.querySelector('.pattl').querySelectorAll('ignore_js_op')[{0}].querySelector('.tip_c').innerText;", i);
                            string fileIfm = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getFJIfm });

                            StackPanel stackPanel = new StackPanel();
                            stackPanel.Orientation = Orientation.Horizontal;

                            stackPanel.Children.Add(new Image() { Source = new BitmapImage(new Uri("ms-appx:///SmallICON/File.png")) });
                            stackPanel.Children.Add(new HyperlinkButton() { NavigateUri = new Uri(downloadlink), Content = filename });
                            stackPanel.Children.Add(new TextBlock() { Text = fileIfm, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center }); ;

                            ThrFJ.Children.Add(stackPanel);

                        }
                        else
                        {
                            var getIMGIfm = string.Format(@"document.querySelector('.pattl').querySelectorAll('ignore_js_op')[{0}].querySelector('.mbn').innerText;", i);
                            string filename = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getIMGIfm });
                            getIMGIfm = string.Format(@"document.querySelector('.pattl').querySelectorAll('ignore_js_op')[{0}].querySelector('a').href;", i);
                            Uri IMGURL = new Uri(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getIMGIfm }));

                            StackPanel stackPanel = new StackPanel();

                            stackPanel.Children.Add(new HyperlinkButton() { Content = filename, NavigateUri = IMGURL, HorizontalAlignment = HorizontalAlignment.Center });
                            stackPanel.Children.Add(new Image() { Source = new BitmapImage(IMGURL) });

                            ThrFJ.Children.Add(stackPanel);

                        }
                    }
                }


            }
            catch(Exception)
            {
                MainPage.CurrentPage.ShowNotifi("抱歉,读取帖子内容时出现问题,请尝试刷新页面.");
            }

            try
            {
                var checkDP = string.Format(@"document.querySelector('.pcb').getElementsByClassName('pstl xs1').length.toString();");
                string result = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { checkDP });
                if(result!="0")
                {
                    ThrDP.Children.Add(new TextBlock() { Text = "点评：", FontSize = 16, Foreground = new SolidColorBrush(Colors.OrangeRed) });

                    var getDPCount = string.Format(@"document.querySelector('.pcb').getElementsByClassName('pstl xs1').length.toString();");
                    string DPCount = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getDPCount });
                    int count = Convert.ToInt32(DPCount);

                    for(int i=0;i<count;i++)
                    {
                        var getUsername = string.Format(@"document.querySelector('.pcb').getElementsByClassName('pstl xs1')[{0}].querySelector('.psti a').innerText;",i);
                        string username = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getUsername });
                        var getUserLink = string.Format(@"document.querySelector('.pcb').getElementsByClassName('pstl xs1')[{0}].querySelector('.psti a').href;",i);
                        string userlink = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getUserLink });
                        var getText = string.Format(@"document.querySelector('.pcb').getElementsByClassName('pstl xs1')[{0}].querySelector('.psti').innerText.replace('{1}','');",i,username);
                        string DPText = "："+await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getText });

                        StackPanel stackPanel = new StackPanel();

                        HyperlinkButton hyperlinkButton = new HyperlinkButton();
                        hyperlinkButton.Click += HyperlinkButton_Click;
                        hyperlinkButton.Content = username;
                        //hyperlinkButton.Tag = userlink;
                        stackPanel.Children.Add(hyperlinkButton);
                        stackPanel.Children.Add(new TextBlock() { Text=DPText});

                        ThrDP.Children.Add(stackPanel);
                    }

                }


            }
            catch (Exception ex)
            {
                MainPage.CurrentPage.ShowNotifi("获取点评信息失败！");
            }

            try
            {
                var checkRate = string.Format(@"document.querySelector('.pcb').querySelectorAll('.rate').length.toString();");
                string result = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { checkRate });
                if (result != "0")
                {
                    RateText.Visibility = RatePanel.Visibility = 0;

                    var getUrl = string.Format(@"document.querySelector('.pcb').querySelector('.rate a').href;");
                    RatePageURL = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getUrl });

                    var getpeopleCount = string.Format(@"document.querySelector('.pcb').querySelectorAll('.rate .cl img').length.toString();");
                    string peoplecount = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getpeopleCount });
                    int count = Convert.ToInt32(peoplecount);

                    if(count>10)
                    {
                        RateNote.Text = "等共有"+count+"人觉得很赞并评分";
                        count = 10;
                    }
                    else
                    {
                        RateNote.Text = "已有" + count + "人觉得很赞并评分";
                    }

                    for(int i=0;i<count;i++)
                    {
                        var getIMGURL = string.Format(@"document.querySelector('.pcb').querySelectorAll('.rate .cl img')[{0}].src;",i);
                        string IMGURL = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getIMGURL });

                        StackPanel stackPanel = new StackPanel();

                        stackPanel.Children.Add(new Ellipse()
                        {
                            Width = 50,
                            Height = 50,
                            Stroke=new SolidColorBrush(Colors.White),
                            StrokeThickness = 2,
                            Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri(IMGURL)) },
                            Margin = new Thickness(-20, 0, 0, 0)
                        });

                        RateList.Children.Add(stackPanel);
                    }


                }
            }
            catch(Exception)
            {
                MainPage.CurrentPage.ShowNotifi("获取评分信息失败！");
            }

            try
            {
                var get = string.Format(@"document.querySelector('#recommend_add').href;");
                dzLink = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { get });
                DZCount.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { @"document.querySelector('#recommendv_add').innerText;" });

                var get2 = string.Format(@"document.querySelector('#recommend_subtract').href;");
                fdLink = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { get2 });
                FDCount.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { @"document.querySelector('#recommendv_subtract').innerText;" });

                var get3 = string.Format(@"document.querySelector('#k_favorite').href;");
                scLink = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { get3 });
                scCount.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { @"document.querySelector('#favoritenumber').innerText;" });

                var get4 = string.Format(@"document.querySelector('#k_share').href;");
                shareLink = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { get4 });
                ShareCount.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { @"document.querySelector('#sharenumber').innerText;" });

            }
            catch(Exception)
            {
                MainPage.CurrentPage.ShowNotifi("获取帖子评价信息失败！");
            }

            ThrManagePanel.Visibility = 0;

            try
            {
                var message = string.Format("document.querySelectorAll('a[title=\"帖子模式\"]').length.toString();");
                string result = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { message });
                if(result=="1")
                {
                    message = string.Format("document.querySelector('a[title=\"帖子模式\"]').innerText;");
                    ThreadRecord.Content = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { message });
                }
            }
            catch (Exception)
            {

            }

            GetComm();
        }

        public async void GetComm()
        {
            try
            {
                var getCount = string.Format("document.querySelectorAll('.authi .xw1').length.toString();");
                string commCount = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getCount });
                int count = Convert.ToInt32(commCount);

                for (int i=1;i<count;i++)
                {
                    var getusername = string.Format(@"document.querySelectorAll('.authi .xw1')[{0}].innerText;",i);
                    string username = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getusername });

                    var getuserlink = string.Format(@"document.querySelectorAll('.authi .xw1')[{0}].href;", i);
                    string commuserlink = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getuserlink });

                    var getuserIfm = string.Format(@"document.getElementsByClassName('i y')[{0}].innerText;",i);
                    string userIfm = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getuserIfm });

                    var getFlooIfm = string.Format(@"document.querySelectorAll('.pi strong')[{0}].innerText+'          '+document.querySelectorAll('.authi em')[{0}].innerText;", i);
                    string FlooIfm = (await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getFlooIfm })).Replace("\n","");

                    var getContent = string.Format(@"document.querySelectorAll('.t_f')[{0}].innerText;", i);
                    string commContent = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getContent });

                    Visibility FJvisibility = (Visibility)1;
                    var getFJCount = string.Format(@"document.querySelectorAll('.t_f')[{0}].querySelectorAll('ignore_js_op').length.toString();",i);
                    string result = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getFJCount });
                    int FJcount = Convert.ToInt32(result);
                    if(FJcount!=0)
                    {
                        FJvisibility = 0;
                    }

                    StackPanel ThrFJPanel = new StackPanel();
                    for (int j = 0; j < FJcount; j++)
                    {
                        var getFJType = string.Format(@"document.querySelectorAll('.t_f')[{0}].querySelectorAll('ignore_js_op')[{1}].querySelectorAll('em').length.toString();", i,j);
                        string isIMG = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getFJType });
                        if (isIMG == "0")
                        {
                            var getIMGIfm = string.Format(@"document.querySelectorAll('.t_f')[{0}].querySelectorAll('ignore_js_op')[{1}].querySelector('img').title;", i,j);
                            string filename = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getIMGIfm });
                            getIMGIfm = string.Format(@"document.getElementsByClassName('tip_c xs0')[{0}].querySelector('a').href;", j);
                            Uri IMGURL = new Uri(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getIMGIfm }));

                            StackPanel stackPanel = new StackPanel();

                            stackPanel.Children.Add(new HyperlinkButton() { Content = filename, NavigateUri = IMGURL, HorizontalAlignment = HorizontalAlignment.Center });
                            stackPanel.Children.Add(new Image() { Source = new BitmapImage(IMGURL) });
                            
                            ThrFJPanel.Children.Add(stackPanel);
                        }
                        else
                        {
                            var getFJIfm = string.Format(@"document.querySelectorAll('.t_f')[{0}].querySelectorAll('ignore_js_op')[{1}].querySelector('a').innerText;", i,j);
                            string filename = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getFJIfm });
                            getFJIfm = string.Format(@"document.querySelectorAll('.t_f')[{0}].querySelectorAll('ignore_js_op')[{1}].querySelector('a').href;", i,j);
                            string downloadlink = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getFJIfm });
                            getFJIfm = string.Format(@"document.querySelectorAll('.t_f')[{0}].querySelectorAll('ignore_js_op')[{1}].querySelector('em').innerText;", i,j);
                            string fileIfm = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getFJIfm });

                            StackPanel stackPanel = new StackPanel();
                            stackPanel.Orientation = Orientation.Horizontal;

                            stackPanel.Children.Add(new Image() { Source = new BitmapImage(new Uri("ms-appx:///SmallICON/File.png")) });
                            stackPanel.Children.Add(new HyperlinkButton() { NavigateUri = new Uri(downloadlink), Content = filename });
                            stackPanel.Children.Add(new TextBlock() { Text = fileIfm, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center }); ;

                            ThrFJPanel.Children.Add(stackPanel);
                        }
                    }

                    string checkFJ = string.Format(@"document.querySelectorAll('.pcb')[{0}].querySelectorAll('.pattl').length.toString();",i);
                    string ishaveFJ = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { checkFJ });
                    if (ishaveFJ == "1")
                    {
                        getCount = string.Format(@"document.querySelectorAll('.pcb')[{0}].querySelector('.pattl').querySelectorAll('ignore_js_op').length.toString();",i);
                        result = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getCount });
                        count = Convert.ToInt32(result);

                        for (int k = 0; k < count; k++)
                        {
                            var getFJType = string.Format(@"document.querySelectorAll('.pcb')[{0}].querySelector('.pattl').querySelectorAll('ignore_js_op')[{1}].querySelectorAll('em').length.toString();", i,k);
                            string isIMG = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getFJType });
                            if (isIMG == "0")
                            {
                                var getFJIfm = string.Format(@"document.querySelectorAll('.pcb')[{0}].querySelector('.pattl').querySelectorAll('ignore_js_op')[{1}].querySelector('a').innerText;", i,k);
                                string filename = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getFJIfm });
                                getFJIfm = string.Format(@"document.querySelectorAll('.pcb')[{0}].querySelector('.pattl').querySelectorAll('ignore_js_op')[{1}].querySelector('a').href;", i,k);
                                string downloadlink = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getFJIfm });
                                getFJIfm = string.Format(@"document.querySelectorAll('.pcb')[{0}].querySelector('.pattl').querySelectorAll('ignore_js_op')[{1}].querySelector('.tip_c').innerText;", i,k);
                                string fileIfm = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getFJIfm });

                                StackPanel stackPanel = new StackPanel();
                                stackPanel.Orientation = Orientation.Horizontal;

                                stackPanel.Children.Add(new Image() { Source = new BitmapImage(new Uri("ms-appx:///SmallICON/File.png")) });
                                stackPanel.Children.Add(new HyperlinkButton() { NavigateUri = new Uri(downloadlink), Content = filename });
                                stackPanel.Children.Add(new TextBlock() { Text = fileIfm, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center }); ;

                                ThrFJPanel.Children.Add(stackPanel);

                            }
                            else
                            {
                                var getIMGIfm = string.Format(@"document.querySelectorAll('.pcb')[{0}].querySelector('.pattl').querySelectorAll('ignore_js_op')[{1}].querySelector('.mbn').innerText;", i,k);
                                string filename = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getIMGIfm });
                                getIMGIfm = string.Format(@"document.querySelectorAll('.pcb')[{0}].querySelector('.pattl').querySelectorAll('ignore_js_op')[{1}].querySelector('a').href;", i,k);
                                Uri IMGURL = new Uri(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getIMGIfm }));

                                StackPanel stackPanel = new StackPanel();

                                stackPanel.Children.Add(new HyperlinkButton() { Content = filename, NavigateUri = IMGURL, HorizontalAlignment = HorizontalAlignment.Center });
                                stackPanel.Children.Add(new Image() { Source = new BitmapImage(IMGURL) });

                                ThrFJPanel.Children.Add(stackPanel);

                            }
                        }
                    }

                    Visibility DPvisibility = (Visibility)1;
                    StackPanel ThrDPPanel = new StackPanel();
                    var checkDP = string.Format(@"document.querySelectorAll('.pcb')[{0}].querySelector('.cm').childElementCount.toString();",i);
                    string ckresult = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { checkDP });
                    if (ckresult != "0")
                    {
                        DPvisibility = 0;
                        ThrDPPanel.Children.Add(new TextBlock() { Text = "点评：", FontSize = 16, Foreground = new SolidColorBrush(Colors.OrangeRed) });

                        var getDPCount = string.Format(@"document.querySelectorAll('.pcb')[{0}].getElementsByClassName('pstl xs1').length.toString();", i);
                        string DPCount = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getDPCount });
                        int DPcount = Convert.ToInt32(DPCount);

                        for (int j = 0; j < DPcount; j++)
                        {
                            var getUsername = string.Format(@"document.querySelectorAll('.pcb')[{0}].getElementsByClassName('pstl xs1')[{1}].querySelector('.psti a').innerText;", i,j);
                            string DPusername = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getUsername });
                            var getUserLink = string.Format(@"document.querySelectorAll('.pcb')[{0}].getElementsByClassName('pstl xs1')[{1}].querySelector('.psti a').href;", i,j);
                            string userlink = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getUserLink });
                            var getText = string.Format(@"document.querySelectorAll('.pcb')[{0}].getElementsByClassName('pstl xs1')[{1}].querySelector('.psti').innerText.replace('{2}','');", i,j, username);
                            string DPText = "：" + await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getText });

                            StackPanel stackPanel = new StackPanel();

                            HyperlinkButton hyperlinkButton = new HyperlinkButton();
                            hyperlinkButton.Click += HyperlinkButton_Click;
                            hyperlinkButton.Content = username;
                            //hyperlinkButton.Tag = userlink;
                            stackPanel.Children.Add(hyperlinkButton);
                            stackPanel.Children.Add(new TextBlock() { Text = DPText });

                            ThrDPPanel.Children.Add(stackPanel);
                        }

                    }

                    Visibility visibility = (Visibility)1;
                    string ratenote = "";
                    Uri pageURL = null;
                    StackPanel ThrRatePanel = new StackPanel() { Orientation=Orientation.Horizontal};
                    var checkRate = string.Format(@"document.querySelectorAll('.pcb')[{0}].querySelectorAll('.rate').length.toString();",i);
                    string Rateresult = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { checkRate });
                    if (Rateresult != "0")
                    {
                        visibility = 0;

                        var getUrl = string.Format(@"document.querySelectorAll('.pcb')[{0}].querySelector('.rate a').href;",i);
                        pageURL = new Uri(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getUrl }));

                        var getpeopleCount = string.Format(@"document.querySelectorAll('.pcb')[{0}].querySelectorAll('.rate .cl img').length.toString();",i);
                        string peoplecount = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getpeopleCount });
                        int Ratecount = Convert.ToInt32(peoplecount);

                        if (Ratecount > 10)
                        {
                            ratenote = "等共有" + Ratecount + "人觉得很赞并评分";
                            Ratecount = 10;
                        }
                        else
                        {
                            ratenote = "已有" + Ratecount + "人觉得很赞并评分";
                        }

                        for (int j = 0; j < Ratecount; j++)
                        {
                            var getIMGURL = string.Format(@"document.querySelectorAll('.pcb')[{0}].querySelectorAll('.rate .cl img')[{1}].src;", i,j);
                            string IMGURL = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getIMGURL });

                            StackPanel stackPanel = new StackPanel();

                            stackPanel.Children.Add(new Ellipse()
                            {
                                Width = 50,
                                Height = 50,
                                Stroke = new SolidColorBrush(Colors.White),
                                StrokeThickness = 2,
                                Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri(IMGURL)) },
                                Margin = new Thickness(-20, 0, 0, 0)
                            });

                            ThrRatePanel.Children.Add(stackPanel);
                        }


                    }



                    CommentList.Items.Add(new ThreadComm
                    {
                        CommUsername = username,
                        CommUserlink = commuserlink,
                        CommUserIfm = userIfm,
                        CommIfm = FlooIfm,
                        CommContent = commContent,
                        FJVisibility = FJvisibility,
                        DPVisibility = DPvisibility,
                        ThreadFJ = ThrFJPanel,
                        ThreadDP = ThrDPPanel,
                        RateVisibility = visibility,
                        RateNote = ratenote,
                        RateList = ThrRatePanel,
                        RateURL=pageURL
                    });
                }
            }
            catch (Exception)
            {
                MainPage.CurrentPage.ShowNotifi("获取评论出现了问题");
            }
        }
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string link = (sender as HyperlinkButton).Tag.ToString();
                link = link.Replace("http://i.pcbeta.com/space-uid-", "http://i.pcbeta.com/home.php?mod=space&uid=").Replace(".html", "&do=profile");
                MainPage.CurrentPage.WebView1.Source = new Uri(link);
                this.Frame.Navigate(typeof(UserIfmPage));
            }
            catch(Exception)
            {

            }

        }
        string RatePageURL = "";
        private async void CheckRateBtn_Click(object sender, RoutedEventArgs e)
        {
            MainPage.CurrentPage.WebView1.Source = new Uri(RatePageURL);
            await Task.Delay(1000);
            if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
            {
                MainPage.CurrentPage.ShowNotifi("获取评分信息超时,请稍后再试");
                MainPage.CurrentPage.WebView1.GoBack();

                return;
            }

            try
            {
                var getRateIfm = string.Format(@"document.querySelector('.list').innerText;");
                RateIfm.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getRateIfm });
                var getRateTotal = string.Format(@"document.getElementsByClassName('o pns')[0].innerText;");
                RateTotal.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getRateTotal });
            }
            catch(Exception)
            {
                MainPage.CurrentPage.ShowNotifi("抱歉,获取评分信息失败,请稍后再试");
                MainPage.CurrentPage.WebView1.GoBack();

                return;
            }

            await CheckRate.ShowAsync();
            MainPage.CurrentPage.WebView1.GoBack();

        }
        private void RateList_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
        string dzLink, fdLink, scLink, shareLink = "";
        private async void LeftFourBtn_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Uri link = null;

            switch(button.Name)
            {
                case "DZBtn":
                    link = new Uri(dzLink);
                    break;
                case "FDBtn":
                    link = new Uri(fdLink);
                    break;
                case "SCBtn":
                    link = new Uri(scLink);
                    break;
                case "ShareBtn":
                    link = new Uri(shareLink);
                    break;
            }

            MainPage.CurrentPage.WebView1.Source = link;
            await Task.Delay(1000);
            if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
            {
                MainPage.CurrentPage.ShowNotifi("操作失败,请稍后再试！");
                MainPage.CurrentPage.WebView1.GoBack();
                return;
            }

            if(button.Name== "DZBtn"||button.Name== "FDBtn")
            {
                //var isSuccess = string.Format(@"document.querySelector('#messagetext').className;");
                //string isSuccessR = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { isSuccess });
                var message = string.Format(@"document.querySelector('#messagetext p').innerText;");
                MainPage.CurrentPage.ShowNotifi(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { message }));

                MainPage.CurrentPage.WebView1.GoBack();
                if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
                {
                    return;
                }

                try
                {
                    DZCount.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { @"document.querySelector('#recommendv_add').innerText;" });
                    FDCount.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { @"document.querySelector('#recommendv_subtract').innerText;" });
                    scCount.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { @"document.querySelector('#favoritenumber').innerText;" });
                    ShareCount.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { @"document.querySelector('#sharenumber').innerText;" });
                }
                catch (Exception)
                {

                }
            }
            else
            {
                var check = string.Format(@"document.querySelectorAll('#messagetext').length.toString();");
                string result = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check });
                if (result == "1")
                {
                    var message = string.Format(@"document.querySelector('#messagetext p').innerText;");
                    MainPage.CurrentPage.ShowNotifi(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { message }));

                    MainPage.CurrentPage.WebView1.GoBack();
                    return;
                }

                SCFXPanel.Tag = button.Name;
                await SCFXPanel.ShowAsync();
            }


        }
        private async void RightFourBtn_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch(button.Name)
            {
                case "EditBtn":
                    MainPage.CurrentPage.WebView1.Source = EditURL;
                    if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
                    {
                        MainPage.CurrentPage.WebView1.GoBack();
                        MainPage.CurrentPage.ShowNotifi("加载失败，请稍后再试！");
                        return;
                    }

                    var check = string.Format(@"document.querySelectorAll('#messagetext').length.toString();");
                    string result = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check });
                    if (result == "1")
                    {
                        var message = string.Format(@"document.querySelector('#messagetext p').innerText;");
                        MainPage.CurrentPage.ShowNotifi(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { message }));

                        MainPage.CurrentPage.WebView1.GoBack();
                        return;
                    }

                    PostTitle.Header = "帖子标题：";
                    PostCommPage.PrimaryButtonText = "保存";
                    await PostCommPage.ShowAsync();

                    break;
                case "RateBtn":
                    break;
                case "ToolBtn":
                    break;
                case "ReportBtn":
                    break;
            }

        }

        private void CommLeftAvatar_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ReplyBtn_Click(object sender, RoutedEventArgs e)
        {
            MainPage.CurrentPage.WebView1.Source = ReplyThrURL;
            if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
            {
                MainPage.CurrentPage.WebView1.GoBack();
                MainPage.CurrentPage.ShowNotifi("加载失败，请稍后重试！");
                return;
            }

            var check = string.Format(@"document.querySelectorAll('#messagetext').length.toString();");
            string result = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { check });
            if (result == "1")
            {
                var message = string.Format(@"document.querySelector('#messagetext p').innerText;");
                MainPage.CurrentPage.ShowNotifi(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { message }));

                MainPage.CurrentPage.WebView1.GoBack();
                return;
            }

            var getContent = string.Format(@"document.querySelector('#e_iframe').contentWindow.document.querySelector('body').innerText;");
            PostContent.Document.SetText(Windows.UI.Text.TextSetOptions.None, await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getContent }));

            PostTitle.Header = "副标题(如不需要可不填，默认值为空)：";
            PostCommPage.PrimaryButtonText = "发表回帖";
            await PostCommPage.ShowAsync();
        }

        private async void CommCheckRate_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            MainPage.CurrentPage.WebView1.Source = new Uri(button.Tag.ToString());
            await Task.Delay(1000);
            if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
            {
                MainPage.CurrentPage.ShowNotifi("获取评分信息超时,请稍后再试");
                MainPage.CurrentPage.WebView1.GoBack();

                return;
            }

            try
            {
                var getRateIfm = string.Format(@"document.querySelector('.list').innerText;");
                RateIfm.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getRateIfm });
                var getRateTotal = string.Format(@"document.getElementsByClassName('o pns')[0].innerText;");
                RateTotal.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getRateTotal });
            }
            catch (Exception)
            {
                MainPage.CurrentPage.ShowNotifi("抱歉,获取评分信息失败,请稍后再试");
                MainPage.CurrentPage.WebView1.GoBack();

                return;
            }

            await CheckRate.ShowAsync();
            MainPage.CurrentPage.WebView1.GoBack();

        }
        private async void SCFXPanel_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {            
                if (SCFXPanel.Tag.ToString() == "SCBtn")
                {
                    var send = string.Format(@"document.querySelector('textarea').innerText='{0}';", SCFXContent.Text);
                    await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                    send = string.Format(@"document.querySelector('#favoritesubmit_btn').click();");
                    await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                }
                else
                {
                    var send = string.Format(@"document.querySelector('textarea').innerText='{0}';", SCFXContent.Text);
                    await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                    send = string.Format(@"document.querySelector('#sharesubmit_btn').click();");
                    await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { send });
                }
            }
            catch(Exception)
            {

            }

            if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
            {
                MainPage.CurrentPage.ShowNotifi("操作失败,请稍后再试！");
            }
            else
            {
                var message = string.Format(@"document.querySelector('#messagetext p').innerText;");
                MainPage.CurrentPage.ShowNotifi(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { message }));
            }

            MainPage.CurrentPage.WebView1.Source = MainThrURL;
            await Task.Delay(1000);
            if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
            {
                return;
            }

            try
            {
                DZCount.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { @"document.querySelector('#recommendv_add').innerText;" });
                FDCount.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { @"document.querySelector('#recommendv_subtract').innerText;" });
                scCount.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { @"document.querySelector('#favoritenumber').innerText;" });
                ShareCount.Text = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { @"document.querySelector('#sharenumber').innerText;" });
            }
            catch (Exception)
            {

            }
        }

        private void SCFXPanel_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            MainPage.CurrentPage.WebView1.GoBack();
            SCFXPanel.Hide();
        }

        private async void PostCommPage_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string posttext;
            PostContent.Document.GetText(Windows.UI.Text.TextGetOptions.None,out posttext);
            if(posttext.Length<4)
            {
                MainPage.CurrentPage.ShowNotifi("评论太短");
                return;
            }
            if(PostTitle.Header.ToString()== "帖子标题：")
            {              
                if (PostTitle.Text.Length < 4)
                {
                    MainPage.CurrentPage.ShowNotifi("帖子标题不能少于4个字符");
                    return;
                }
            }

            try
            {
                var sendTitle = string.Format(@"document.querySelector('#subject').value='{0}';", PostTitle.Text);
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { sendTitle });

                PostContent.Document.GetText(Windows.UI.Text.TextGetOptions.UseLf, out string aaa);
                var sendContent = string.Format(@"document.querySelector('#e_iframe').contentWindow.document.querySelector('body').innerText='{0}';", aaa);
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { sendContent });

                var sendSubmit = string.Format(@"document.querySelector('#postsubmit').click();");
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { sendSubmit });

                await Task.Delay(2000);

                if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
                {
                    MainPage.CurrentPage.WebView1.GoBack();
                    MainPage.CurrentPage.ShowNotifi("sorry 发送请求超时,请稍后重试");
                    return;
                }

                var message = string.Format(@"document.querySelector('#messagetext p').innerText;");
                MainPage.CurrentPage.ShowNotifi(await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { message }));

                MainPage.CurrentPage.WebView1.Source = MainThrURL;
                this.Frame.Navigate(typeof(ThreadContentPage));

            }
            catch (Exception)
            {

            }
        }

        private void PostCommPage_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            MainPage.CurrentPage.WebView1.GoBack();
            PostCommPage.Hide();
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

            if (PostContent.Document.Selection.Length != 0)
            {
                PostContent.Document.Selection.Text = "[font=" + fontData + "]" + PostContent.Document.Selection.Text + "[/font]";
            }
            else
            {
                PostContent.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[font=" + fontData + "]   [/font]");
            }
            FTFont.ClearValue(ListView.SelectedIndexProperty);
        }

        private void FTFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

            if (PostContent.Document.Selection.Length != 0)
            {
                PostContent.Document.Selection.Text = "[size=" + fontsizeData + "]" + PostContent.Document.Selection.Text + "[/size]";
            }
            else
            {
                PostContent.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[size=" + fontsizeData + "]   [/size]");
            }
            FTFontSize.ClearValue(ListView.SelectedIndexProperty);
        }

        private void FirstBtn_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch(button.Name)
            {
                case "FTBlod":

                    if (PostContent.Document.Selection.Length != 0)
                    {
                        PostContent.Document.Selection.Text = "[b]" + PostContent.Document.Selection.Text + "[/b]";
                    }
                    else
                    {
                        PostContent.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[b]   [/b]");
                    }

                    break;
                case "FTFi":

                    if (PostContent.Document.Selection.Length != 0)
                    {
                        PostContent.Document.Selection.Text = "[i]" + PostContent.Document.Selection.Text + "[/i]";
                    }
                    else
                    {
                        PostContent.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[i]   [/i]");
                    }

                    break;
                case "FTLine":

                    if (PostContent.Document.Selection.Length != 0)
                    {
                        PostContent.Document.Selection.Text = "[u]" + PostContent.Document.Selection.Text + "[/u]";
                    }
                    else
                    {
                        PostContent.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[u]   [/u]");
                    }

                    break;
                case "FTColor":

                    if (PostContent.Document.Selection.Length != 0)
                    {
                        PostContent.Document.Selection.Text = "[color=请输入颜色英文或编码]" + PostContent.Document.Selection.Text + "[/color]";
                    }
                    else
                    {
                        PostContent.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[color=请输入颜色英文或编码]   [/color]");
                    }

                    break;
                case "FTBgcolor":

                    if (PostContent.Document.Selection.Length != 0)
                    {
                        PostContent.Document.Selection.Text = "[backcolor=请输入颜色英文或编码]" + PostContent.Document.Selection.Text + "[/backcolor]";
                    }
                    else
                    {
                        PostContent.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[backcolor=请输入颜色英文或编码]   [/backcolor]");
                    }

                    break;
                case "FTLink":

                    if (PostContent.Document.Selection.Length != 0)
                    {
                        PostContent.Document.Selection.Text = "[url=超链接]" + PostContent.Document.Selection.Text + "[/url]";
                    }
                    else
                    {
                        PostContent.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[url=超链接]   [/url]");
                    }

                    break;
                case "FTCode":

                    if (PostContent.Document.Selection.Length != 0)
                    {
                        PostContent.Document.Selection.Text = "[code]" + PostContent.Document.Selection.Text + "[/code]";
                    }
                    else
                    {
                        PostContent.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[code]   [/code]");
                    }

                    break;
                case "FTQuote":

                    if (PostContent.Document.Selection.Length != 0)
                    {
                        PostContent.Document.Selection.Text = "[quote]" + PostContent.Document.Selection.Text + "[/quote]";
                    }
                    else
                    {
                        PostContent.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[quote]   [/quote]");
                    }

                    break;
                case "FTLeft":

                    if (PostContent.Document.Selection.Length != 0)
                    {
                        PostContent.Document.Selection.Text = "[align=left]" + PostContent.Document.Selection.Text + "[/align]";
                    }
                    else
                    {
                        PostContent.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[align=left]   [/align]");
                    }

                    break;
                case "FTCenter":

                    if (PostContent.Document.Selection.Length != 0)
                    {
                        PostContent.Document.Selection.Text = "[align=center]" + PostContent.Document.Selection.Text + "[/align]";
                    }
                    else
                    {
                        PostContent.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[align=center]   [/align]");
                    }

                    break;
                case "FTRight":

                    if (PostContent.Document.Selection.Length != 0)
                    {
                        PostContent.Document.Selection.Text = "[align=right]" + PostContent.Document.Selection.Text + "[/align]";
                    }
                    else
                    {
                        PostContent.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, "[align=right]   [/align]");
                    }

                    break;
            }
        }
        private void SecondBtn_Click(object sender, RoutedEventArgs e)
        {
            BQ1.Text = "{:5_260:}至{:5_299:}；{:5_586:}至{:5_597:}";
            BQ2.Text = "{:9_348:}至{:9_422:}；{:9_598:}至{:9_640:}";
            BQ3.Text = "{:7_423:}至{:7_507:}";
            BQ4.Text = "{:8_508:}至{:8_555:}";
        }

        private async void SelectIMGUpload_Click(object sender, RoutedEventArgs e)
        {
            SelectIMGCount.ClearValue(TextBlock.TextProperty);
            try
            {
                var getCount = string.Format(@"document.querySelectorAll('.filedata').length.toString();");
                string reNum = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getCount });
                int count = Convert.ToInt32(reNum) - 1;

                var sendComm = string.Format(@"document.querySelectorAll('.filedata')[{0}].click();", count);
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { sendComm });

                await Task.Delay(1000);
                var checkVaild = string.Format(@"document.querySelectorAll('.alert_error').length.toString();");
                string result = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { checkVaild });
                if (result == "1")
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

        private async void StartIMGUpload_Click(object sender, RoutedEventArgs e)
        {
            SelectIMGCount.ClearValue(TextBlock.TextProperty);
            UploadIMGCount.ClearValue(TextBlock.TextProperty);
            try
            {
                var getCount = string.Format(@"document.querySelectorAll('.filedata').length.toString();");
                string reNum = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getCount });
                if (reNum == "2")
                {
                    SelectIMGCount.Text = "您还没有选择图片";
                    return;
                }

                var get = string.Format(@"document.getElementsByClassName('pn pnc vm')[0].click();");
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { get });

                a1();
            }
            catch (Exception)
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
                        if (checkResult.Contains("用户组限制"))
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
            catch (Exception)
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
                for (int i = 0; i < 60; i++)
                {
                    string get1 = string.Format(@"document.querySelectorAll('.imgl img').length.toString();");
                    string result1 = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { get1 });
                    count = Convert.ToInt32(result1);
                    if (count != 0)
                    {
                        UploadIMGCount.Text = "您已上传" + count + "张图片：";
                        break;
                    }
                    if (i == 59)
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
                    result3 = "delImgAttach(" + result3.Replace("image_", "") + ")";
                    string result4 = "[attachimg]" + result3.Replace("image_", "") + "[/attachimg]";

                    var bitmapImage = new BitmapImage(result2);

                    UploadIMGList.Items.Add(new UploadImage { PreviewIMG = bitmapImage, DeleteIMG = result3, InsertIMG = result4 });
                }

            }
            catch (Exception)
            {
                MainPage.CurrentPage.ShowNotifi("连接服务器失败,请稍后重试!");
            }

        }

        private void InsertIMG_Click(object sender, RoutedEventArgs e)
        {
            var abcde = UploadIMGList.SelectedItem as UploadImage;
            if (PostContent.Document.Selection.Length != 0)
            {
                PostContent.Document.Selection.Text = abcde.InsertIMG;
            }
            else
            {
                PostContent.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, abcde.InsertIMG);
            }
        }

        private async void DeleteIMG_Click(object sender, RoutedEventArgs e)
        {
            var abcde = UploadIMGList.SelectedItem as UploadImage;
            await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { abcde.DeleteIMG });
            UploadIMGList.Items.Remove(UploadIMGList.SelectedItem);
        }

        private async void SelectFileUpload_Click(object sender, RoutedEventArgs e)
        {
            SelectFileCount.ClearValue(TextBlock.TextProperty);
            try
            {
                var getCount = string.Format(@"document.querySelector('#attachbtn').childElementCount.toString();");
                string reNum = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getCount });
                int count = Convert.ToInt32(reNum);

                var sendComm = string.Format(@"document.querySelector('#attachbtn').querySelectorAll('.fldt')[{0}].click();", count - 1);
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

        private async void StartFileUpload_Click(object sender, RoutedEventArgs e)
        {
            SelectFileCount.ClearValue(TextBlock.TextProperty);
            UploadFileCount.ClearValue(TextBlock.TextProperty);
            try
            {
                var getCount = string.Format(@"document.querySelector('#attachbtn').childElementCount.toString();");
                string reNum = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { getCount });
                if (reNum == "1")
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
            catch (Exception)
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

            }
            catch (Exception)
            {
                MainPage.CurrentPage.ShowNotifi("连接服务器失败,请稍后重试!");
            }

        }
        private void InsertFile_Click(object sender, RoutedEventArgs e)
        {
            var abcde = SCFJList.SelectedItem as ThrFiles;
            if (PostContent.Document.Selection.Length != 0)
            {
                PostContent.Document.Selection.Text = abcde.Insert;
            }
            else
            {
                PostContent.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, abcde.Insert);
            }
        }

        private async void DeleteFile_Click(object sender, RoutedEventArgs e)
        {
            var abcde = SCFJList.SelectedItem as ThrFiles;
            await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { abcde.Delete });
            SCFJList.Items.Remove(SCFJList.SelectedItem);
        }
        int[] qxCount = { 0, 9, 10, 20, 30, 40, 50, 70, 100, 120, 140, 160, 200, 205, 210, 220, 225, 235, 250, 255 };
        private async void UpdateFile_Click(object sender, RoutedEventArgs e)
        {
            SDError.ClearValue(TextBlock.TextProperty);
            if (string.IsNullOrEmpty(SetFileRead.Text))
            {
                SetFileRead.Text = "0";
            }
            if (string.IsNullOrEmpty(SetFilePrice.Text))
            {
                SetFilePrice.Text = "0";
            }

            try
            {
                int sd1 = Convert.ToInt16(SetFileRead.Text);
                int sd2 = Convert.ToInt16(SetFilePrice.Text);

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
                var sent1 = string.Format(@"document.getElementsByName('attachnew[{0}][readperm]')[0].value='{1}';", abcde, SetFileRead.Text);
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { sent1 });
                var sent2 = string.Format(@"document.getElementsByName('attachnew[{0}][price]')[0].value='{1}';", abcde, SetFilePrice.Text);
                await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { sent2 });

            }
            catch (Exception)
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

        private void UploadIMGList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UploadIMGList.SelectedIndex < 0)
            {
                IMGPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                IMGPanel.Visibility = Visibility.Visible;
            }
        }

        private void SCFJList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SCFJList.SelectedIndex < 0)
            {
                FilePanel.Visibility = (Visibility)1;
                FilePanel2.Visibility = (Visibility)1;
            }
            else
            {
                SetFileRead.Text = (SCFJList.SelectedItem as ThrFiles).ViewLVL.Replace("权限:", "");
                SetFilePrice.Text = (SCFJList.SelectedItem as ThrFiles).Price.Replace("售价:", "");
                FilePanel.Visibility = 0;
                FilePanel2.Visibility = 0;
            }
        }

        private void CommentList_ItemClick(object sender, ItemClickEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void CommFlyout_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
        private void CheckRate_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            CheckRate.Hide();
        }
    }
}
