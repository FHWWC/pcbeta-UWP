using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class BBS2Page : Page
    {
        public static BBS2Page PageTrans;
        public BBS2Page()
        {
            this.InitializeComponent();
            PageTrans = this;
        }
        BitmapImage bitmapImage;
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(1000);
            try
            {
                bitmapImage = new BitmapImage(new Uri("http://bbs.pcbeta.com/templates/forumicons/1_1/nb.png"));
                Z1P1.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://bbs.pcbeta.com/templates/forumicons/1_1/acer.png"));
                Z1P2.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://bbs.pcbeta.com/templates/forumicons/1_1/asus.png"));
                Z1P3.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://bbs.pcbeta.com/templates/forumicons/1_1/dell.png"));
                Z1P4.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://bbs.pcbeta.com/templates/forumicons/1_1/hp.png"));
                Z1P5.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://bbs.pcbeta.com/templates/forumicons/1_1/lenovo.png"));
                Z1P6.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://bbs.pcbeta.com/templates/forumicons/1_1/sony.png"));
                Z1P7.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://bbs.pcbeta.com/templates/forumicons/1_1/toshiba.png"));
                Z1P8.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://www.pcbeta.com///static/image/pcbeta/forum.gif"));
                Z2P1.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://mac.pcbeta.com/images/forumicons/screen.png"));
                Z2P2.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://mac.pcbeta.com/images/forumicons/bootcamp.png"));
                Z2P3.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://mac.pcbeta.com/images/forumicons/app.png"));
                Z2P4.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://bbs.pcbeta.com/templates/forumicons/1_1/help.png"));
                Z3P1.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://bbs.pcbeta.com/templates/forumicons/1_1/complaint.png"));
                Z3P2.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://bbs.pcbeta.com/templates/forumicons/1_1/warning.png"));
                Z3P3.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://bbs.pcbeta.com/templates/forumicons/1_1/banuser.png"));
                Z3P4.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://bbs.pcbeta.com/templates/forumicons/1_1/manager.png"));
                Z4P1.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://bbs.pcbeta.com/templates/forumicons/1_1/special.png"));
                Z4P2.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://bbs.pcbeta.com/templates/forumicons/1_1/group.png"));
                Z4P3.Source = bitmapImage;
                bitmapImage = new BitmapImage(new Uri("http://bbs.pcbeta.com/templates/forumicons/1_1/link.png"));
                Z4P4.Source = bitmapImage;
            }
            catch (Exception)
            {

            }
        }
        private void 子版1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void 子版2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void 子版3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
  
        }

        private void 子版4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
