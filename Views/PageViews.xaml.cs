using System;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Windows.Threading;

namespace WpfApplication1.Views
{
    public partial class PageViews : Page
    {
        double[] PVArray = new double[5] { 0, 0, 0, 0, 0 };
        int[] CntArray = new int[5] { 0, 0, 0, 0, 0 };
        DispatcherTimer CountingTimer = new DispatcherTimer();
        
        public PageViews()
        {
            InitializeComponent();

            ((MainWindow)Application.Current.MainWindow).PageTitle.Text = "PV 현황";
            GetJsonData(RequestJson());
        }

        private JObject RequestJson()
        {
            JObject obj = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://wcms.seoul.co.kr/weblog/getSummenryPV.php");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                //StreamReader reader = new StreamReader(@"D:\SEOULADM\My Documents\Downloads\test.json");
                obj = JObject.Parse(reader.ReadToEnd());
                stream.Close();
                response.Close();
            }
            catch (Exception)
            {
                return obj;
            }

            return obj;
        }
        
        private void GetJsonData(JObject obj)
        {
            if (obj == null)
            {
                ShowCountingMsg();
                return;
            }

            SetPVArray(obj);
            SetProgressBarColor();
            SetProgressBarValue();
            
            ShowData(0, TotalToday);
            ShowData(1, SeoulToday);
            ShowData(2, SeoulYesterday);
            ShowData(3, SeoulAvg7);
            ShowData(4, SeoulAvg28);
        }

        private void SetPVArray(JObject obj)
        {
            try
            {
                PVArray[0] = Convert.ToDouble(obj["pv_total_today"]);
                PVArray[1] = Convert.ToDouble(obj["pv_seoul_today"]);
                PVArray[2] = Convert.ToDouble(obj["pv_seoul_yesterday"]);
                PVArray[3] = Convert.ToDouble(obj["pv_seoul_avg_7day"]);
                PVArray[4] = Convert.ToDouble(obj["pv_seoul_avg_28day"]);

                obj.RemoveAll();
            }
            catch (Exception)
            {
            }
        }

        private void SetProgressBarColor()
        {
            if(PVArray[1] != 0 && PVArray[2] != 0)
            {
                if (PVArray[1] < PVArray[2] / 2)
                    ProgressBar2.Style = (Style)FindResource("PVGridBarRed");
                else if(PVArray[1] < PVArray[2])
                    ProgressBar2.Style = (Style)FindResource("PVGridBarYellow");
                else
                    ProgressBar2.Style = (Style)FindResource("PVGridBarGreen");
            }
        }

        private void SetProgressBarValue()
        {
            double PVMax = 0;
            
            for (int i = 1; i < PVArray.Length; i++)
                PVMax = (PVMax < PVArray[i]) ? PVArray[i] : PVMax;

            SeoulTodayBar.To = PVArray[1] / PVMax * 100;
            SeoulYesterdayBar.To = PVArray[2] / PVMax * 100;
            SeoulAvg7Bar.To = PVArray[3] / PVMax * 100;
            SeoulAvg28Bar.To = PVArray[4] / PVMax * 100;
        }
        
        private void ShowData(int index, TextBlock tbName)
        {
            if (PVArray[index] != 0)
            {
                CountingTimer.Interval = TimeSpan.FromMilliseconds(10);
                CountingTimer.Tick += (sender, e) => Counting(sender, e, index, tbName);
                CountingTimer.Start();
            }
            else
            {
                tbName.Text = "집계중..";
            }
        }

        private void Counting(object sender, EventArgs e, int index, TextBlock tbName)
        {
            if (CntArray[index] < 50)
            {
                tbName.Text = String.Format("{0:#,##0}", new Random().Next(100000, 999999));
                CntArray[index]++;
            }
            else
            {
                tbName.Text = String.Format("{0:#,##0}", PVArray[index]);
                CountingTimer.Stop();
            }
        }

        private void ShowCountingMsg()
        {
            TotalToday.Text = "집계중..";
            SeoulToday.Text = "집계중..";
            SeoulYesterday.Text = "집계중..";
            SeoulAvg7.Text = "집계중..";
            SeoulAvg28.Text = "집계중..";
        }
    }
}
