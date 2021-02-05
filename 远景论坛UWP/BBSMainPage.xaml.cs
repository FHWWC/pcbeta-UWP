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
    public sealed partial class BBSMainPage : Page
    {
        public BBSMainPage()
        {
            this.InitializeComponent();
        }
        BitmapImage bitmapImage;
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRing.Visibility = 0;
            LoadRing.IsActive = true;

            try
            {
                MainPage.CurrentPage.WebView1.Source = new Uri("http://bbs.pcbeta.com");
                if (await MainPage.CurrentPage.CheckLoadState(true) != 0)
                {
                    LoadRing.Visibility = (Visibility)1;
                    LoadRing.IsActive = false;
                    return;
                }

                RefreshMessage refresh = new RefreshMessage();
                refresh.CheckNewMessage();
                /*
                string get1 = "document.querySelectorAll('#category_213 .fl_g').length.toString();";
                string result1 = await ConnToWV(new string[] { get1 });
                int count = Convert.ToInt32(result1);
                */

                //板块1：Windows 10
                //1a Win10区；1b 经典区；1c 设备及硬件；1d MS其他产品
                for (int i = 0; i < 9; i++)
                {
                    GetAndAddItems("category_213",i,List1a);
                    await Task.Delay(100);
                }

                for (int i = 0; i < 2; i++)
                {
                    GetAndAddItems("category_181", i, List1b);
                    await Task.Delay(100);
                }

                GetAndAddItems("category_263", 1, List1b);
                await Task.Delay(100);
                GetAndAddItems("category_263", 3, List1b);
                await Task.Delay(100);
                GetAndAddItems("category_106", 2, List1b);
                await Task.Delay(100);

                for (int i = 0; i < 9; i++)
                {
                    GetAndAddItems("category_287", i, List1c);
                    await Task.Delay(100);
                }

                GetAndAddItems("category_106", 3, List1d);
                await Task.Delay(100);
                GetAndAddItems("category_106", 4, List1d);
                await Task.Delay(100);
                GetAndAddItems("category_533", 2, List1d);
                await Task.Delay(100);


                //板块2：Office
                for (int i = 9; i < 12; i++)
                {
                    GetAndAddItems("category_213", i, List2);
                    await Task.Delay(100);
                }


                //板块3：苹果
                for (int i = 0; i < 6; i++)
                {
                    GetAndAddItems("category_86", i, List3);
                    await Task.Delay(100);
                }


                //板块4：Linux
                GetAndAddItems("category_508", 0, List4);
                await Task.Delay(100);
                GetAndAddItems("category_508", 1, List4);
                await Task.Delay(100);


                //板块5：俱乐部
                GetAndAddItems("category_15", 0, List5);
                await Task.Delay(100);
                GetAndAddItems("category_156", 0, List5);
                await Task.Delay(100);
                GetAndAddItems("category_156", 3, List5);
                await Task.Delay(100);


                //板块6：站务
                for (int i = 0; i < 3; i++)
                {
                    GetAndAddItems("category_7", i, List6);
                    await Task.Delay(100);
                }


            }
            catch (Exception)
            {

            }

            LoadRing.Visibility = Visibility.Collapsed;
            LoadRing.IsActive = false;
        }
        public async void GetAndAddItems(string categoryID,int index,ListView addToListView)
        {
            try
            {
                string r1 = await ConnToWV(new string[] { "document.querySelectorAll('#" + categoryID + " .fl_g img')[" + index + "].src" });
                bitmapImage = new BitmapImage(new Uri(r1));

                string r2 = await ConnToWV(new string[] { "document.querySelectorAll('#" + categoryID + " .fl_g')[" + index + "].querySelector('a').href" });
                Uri u1 = new Uri(r2);

                r2 = await ConnToWV(new string[] { "document.querySelectorAll('#" + categoryID + " .fl_g')[" + index + "].querySelectorAll('a')[1].innerText" });
                string r3 = await ConnToWV(new string[] { "document.querySelectorAll('#" + categoryID + " .fl_g')[" + index + "].getElementsByClassName('xw0 xi1 yellow').length.toString();" });
                if (r3 == "1")
                {
                    r3 = "+" + await ConnToWV(new string[] { "document.querySelectorAll('#" + categoryID + " .fl_g')[" + index + "].querySelector('em').innerText" });
                }
                else
                {
                    r3 = "";
                }

                string r4 = await ConnToWV(new string[] { "document.querySelectorAll('#" + categoryID + " .fl_g')[" + index + "].getElementsByClassName('xg2').length.toString();" });
                if (r4 == "1")
                {
                    r4 = await ConnToWV(new string[] { "document.querySelectorAll('#" + categoryID + " .fl_g')[" + index + "].querySelector('.xg2').innerText" });
                }
                else
                {
                    r4 = "该板块暂无介绍";
                }

                r4 += "\n" + await ConnToWV(new string[] { "document.querySelectorAll('#" + categoryID + " .fl_g')[" + index + "].querySelector('dd').innerText" });

                addToListView.Items.Add(new BBSMainBlock { BlockICON = bitmapImage, uri = u1, BlockTittle = r2, TodayPosts = r3, BlockIfm = r4 });
            }
            catch (Exception)
            {

            }
        }

        public async Task<string> ConnToWV(string[] value)
        {
            return await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", value); ;
        }
        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            switch(listView.Name)
            {
                case "List1a":
                    MainPage.CurrentPage.WebView1.Source= (List1a.SelectedItem as BBSMainBlock).uri;
                    break;
                case "List1b":
                    MainPage.CurrentPage.WebView1.Source = (List1b.SelectedItem as BBSMainBlock).uri;
                    break;
                case "List1c":
                    MainPage.CurrentPage.WebView1.Source = (List1c.SelectedItem as BBSMainBlock).uri;
                    break;
                case "List1d":
                    MainPage.CurrentPage.WebView1.Source = (List1d.SelectedItem as BBSMainBlock).uri;
                    break;
                case "List2":
                    MainPage.CurrentPage.WebView1.Source = (List2.SelectedItem as BBSMainBlock).uri;
                    break;
                case "List3":
                    MainPage.CurrentPage.WebView1.Source = (List3.SelectedItem as BBSMainBlock).uri;
                    break;
                case "List4":
                    MainPage.CurrentPage.WebView1.Source = (List4.SelectedItem as BBSMainBlock).uri;
                    break;
                case "List5":
                    MainPage.CurrentPage.WebView1.Source = (List5.SelectedItem as BBSMainBlock).uri;
                    break;
                case "List6":
                    MainPage.CurrentPage.WebView1.Source = (List6.SelectedItem as BBSMainBlock).uri;
                    break;
            }

            Frame.Navigate(typeof(ShowThreadsPage));
        }
    }
}
