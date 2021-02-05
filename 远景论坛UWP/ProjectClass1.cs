using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace 远景论坛UWP
{
    public class MyThreads
    {
        public string Tittle { get; set; }
        public string Block { get; set; }
        public string ReplyViewNum { get; set; }
        public string LastReply { get; set; }
        public Uri TUri { get; set; }
    }
    public class MyReply
    {
        public string Tittle { get; set; }
        public string ReplyContent { get; set; }
        public string Block { get; set; }
        public string ReplyViewNum { get; set; }
        public string LastReply { get; set; }
        public Uri TUri { get; set; }
    }
    public class MyDP
    {
        public string Tittle { get; set; }
        public string Block { get; set; }
        public string DPContent { get; set; }
        public Uri TUri { get; set; }
    }
    public class RefreshMessage
    {
        public async void CheckNewMessage()
        {
            var ckNew = string.Format(@"document.getElementById('myprompt').innerText;");
            string ckResult = await MainPage.CurrentPage.WebView1.InvokeScriptAsync("eval", new string[] { ckNew });

            if (ckResult.Contains("("))
            {
                MainPage.CurrentPage.新消息.Text = ckResult.Replace("提醒(","").Replace(")","");
                if(!MainPage.CurrentPage.APPLeftPanel.IsPaneOpen)
                {
                    MainPage.CurrentPage.SmallNew.Visibility = 0;
                }
            }
            else
            {
                MainPage.CurrentPage.新消息.ClearValue(TextBlock.TextProperty);
                if (!MainPage.CurrentPage.APPLeftPanel.IsPaneOpen)
                {
                    MainPage.CurrentPage.SmallNew.Visibility = (Visibility)1;
                }
            }
        }
    }
    public class AlertInformation
    {
        public BitmapImage UserImage { get; set; }
        public string UserSpaceID { get; set; }
        public string Datetime { get; set; }
        public string AlertContent { get; set; }
        public string Directlink {get;set;}
        public string BtnName { get; set; }
    }
    public class NEWSIfm
    {
        public string Tittle { get; set; }
        public string Preview { get; set; }
        public string Information { get; set; }
        public BitmapImage NEWSICON { get; set; }
        public Uri NEWSUri { get; set; }
    }
    public class ShowNEWSc
    {
        public BitmapImage Images { get; set; }
        public string CommentsIfm { get; set; }
        public string Quote { get; set; }
        public string Comments { get; set; }
        public string DeleteComm { get; set; }
    }
    public class ThreadsIfm
    {
        public string Tags { get; set; }
        public string Tittle { get; set; }
        public string Postby { get; set; }
        public string CheckViewNum { get; set; }
        public string LastReply { get; set; }
        public Uri ThrUri { get; set; }
    }
    public class UploadImage
    {
        public BitmapImage PreviewIMG { get; set; }
        public string DeleteIMG { get; set; }
        public string InsertIMG { get; set; }
    }
    public class ThrFiles
    {
        public string FileName { get; set; }
        public string ViewLVL { get; set; }
        public string Price { get; set; }
        public string Delete { get; set; }
        public string Insert { get; set; }
        public string FilesID { get; set; }
    }
    public class BBSMainBlock
    {
        public BitmapImage BlockICON { get; set; }
        public string BlockTittle { get; set; }
        public string TodayPosts { get; set; }
        public string BlockIfm { get; set; }
        public Uri uri { get; set; }
    }
    public class ThreadComm
    {
        public string CommUsername { get; set; }
        public string CommUserlink { get; set; }
        public string CommUserIfm { get; set; }
        public string CommIfm { get; set; }
        public string CommContent { get; set; }
        public StackPanel ThreadFJ { get; set; }
        public StackPanel ThreadDP { get; set; }
        public Visibility FJVisibility { get; set; }
        public Visibility DPVisibility { get; set; }
        public Visibility RateVisibility { get; set; }
        public Uri RateURL { get; set; }
        public StackPanel RateList { get; set; }
        public string RateNote { get; set; }
        public Uri DPLink { get; set; }
        public Uri ReplyLink { get; set; }
        public Uri EditLink { get; set; }
        public Uri ReportLink { get; set; }

    }
}
