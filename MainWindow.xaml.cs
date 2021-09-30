using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.IO;

namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        Timer DateTimer = new Timer();
        Timer RollingTimer = new Timer();
        Timer PageTimer = new Timer();
        static int PageNum = 1;
        ImageBrush bg = new ImageBrush();

        public MainWindow()
        {
            InitializeComponent();

            Cursor = System.Windows.Input.Cursors.None;

            // 현재 시각
            DateTimer.Interval = 1000;  // 1초
            DateTimer.Tick += GetDateTimeNow;
            DateTimer.Start();

            // 인터넷 최근 기사 롤링
            Rolling.Content = new RollingControl();
            RollingTimer.Interval = 33000;
            RollingTimer.Tick += LoadRolling;
            RollingTimer.Start();

            // 페이지 전환
            Main.Content = new Views.PageViews();
            PageTimer.Interval = 40000;
            PageTimer.Tick += LoadPage;
            PageTimer.Start();
        }

        private void GetDateTimeNow(object sender, EventArgs e)
        {
            string dt = DateTime.Now.ToString("M월 d일(ddd) HH:mm:ss");

            DateTimeNow.Text = dt;
            SaveLog(dt.Substring(dt.Length - 8));

            if(dt.Substring(dt.Length - 8) == "07:00:00")
            {
                System.Windows.Forms.Application.Restart();
                System.Windows.Application.Current.Shutdown();
                SaveLog("@Application Restart...");
            }
        }

        private void LoadRolling(object sender, EventArgs e)
        {
            Rolling.Content = new RollingControl();
        }

        // 페이스북 넣는 경우. PageTimer.Interval = 60000;
        //private void LoadPage(object sender, EventArgs e)
        //{
        //    switch (PageNum)
        //    {
        //        case 0:     // PV 현황
        //            bg.ImageSource = new BitmapImage(new Uri(@"./Images/bg1.jpg", UriKind.Relative));
        //            Background = bg;
        //            PageTimer.Interval = 60000;
        //            Main.Content = new Views.PageViews();
        //            break;
        //        case 1:     // 인터넷 최근 기사
        //            PageTimer.Interval = 30000;
        //            Main.Content = new Views.InternetRecent();
        //            break;
        //        case 2:     // 페이스북 최근 게시
        //            Main.Content = new Views.FacebookRecent();
        //            break;
        //        case 3:     // 조회순 - 오늘
        //            bg.ImageSource = new BitmapImage(new Uri(@"./Images/bg2.jpg", UriKind.Relative));
        //            Background = bg;
        //            Main.Content = new Views.PVRankToday();
        //            break;
        //        case 4:     // 조회순 - 어제~오늘
        //            Main.Content = new Views.PVRankYesterdayToday();
        //            break;
        //        case 5:     // 페이스북 좋아요순
        //            Main.Content = new Views.FacebookRank();
        //            break;
        //    }

        //    SaveLog("Load Page " + PageNum + "... " + String.Format("{0:#,##0}", GC.GetTotalMemory(false)) + "bytes");

        //    if (PageNum == 5)
        //        PageNum = 0;
        //    else
        //        PageNum++;
        //}

        // 페이스북 빼는 경우. PageTimer.Interval = 40000;
        private void LoadPage(object sender, EventArgs e)
        {
            switch (PageNum)
            {
                case 0:     // PV 현황
                    bg.ImageSource = new BitmapImage(new Uri(@"./Images/bg1.jpg", UriKind.Relative));
                    Background = bg;
                    PageTimer.Interval = 60000;
                    Main.Content = new Views.PageViews();
                    break;
                case 1:     // 인터넷 최근 기사
                    PageTimer.Interval = 30000;
                    Main.Content = new Views.InternetRecent();
                    break;
                case 2:     // 조회순 - 오늘
                    bg.ImageSource = new BitmapImage(new Uri(@"./Images/bg2.jpg", UriKind.Relative));
                    Background = bg;
                    Main.Content = new Views.PVRankToday();
                    break;
                case 3:     // 조회순 - 어제~오늘
                    Main.Content = new Views.PVRankYesterdayToday();
                    break;
            }

            SaveLog("Load Page " + PageNum + "... " + String.Format("{0:#,##0}", GC.GetTotalMemory(false)) + "bytes");

            if (PageNum == 3)
                PageNum = 0;
            else
                PageNum++;
        }

        public static void SaveLog(string LogMsg)
        {
            FileStream fo = null;
            StreamWriter sw = null;

            try
            {
                if (Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\log") == false)
                    Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\log");

                string strFileName = System.Windows.Forms.Application.StartupPath + "\\log\\datalmgr_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

                fo = new FileStream(strFileName, FileMode.Append);
                sw = new StreamWriter(fo);

                sw.WriteLine(LogMsg);
                sw.Close();
                fo.Close();
            }
            catch (Exception)
            {
                if (sw != null)
                    sw.Close();

                if (fo != null)
                    fo.Close();
            }
        }
    }
}
