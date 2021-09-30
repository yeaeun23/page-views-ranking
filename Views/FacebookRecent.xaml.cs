using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace WpfApplication1.Views
{
    public partial class FacebookRecent : Page
    {
        public FacebookRecent()
        {
            InitializeComponent();

            ((MainWindow)Application.Current.MainWindow).PageTitle.Text = "페이스북 최근 게시";
            GetJsonData(RequestJson());
        }

        private string RequestJson()
        {
            string result = "";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://appsvr2.seoul.co.kr/baljae/service.aspx?FN=FACEBOOKNEW");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                result = reader.ReadToEnd();
                stream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                ErrMsg.Text = ex.Message;
                return "";
            }

            return result;
        }

        private void GetJsonData(string result)
        {
            if (result == "")
                return;

            List<ArticleInfo> items = new List<ArticleInfo>();

            try
            {
                JObject obj = JObject.Parse(result);
                JArray array = JArray.Parse(obj["feed"]["data"].ToString());

                foreach (JObject jObj in array)
                {
                    if (jObj["name"] == null)
                        continue;

                    if (!jObj["link"].ToString().Contains("seoul.co.kr"))
                        continue;

                    items.Add(new ArticleInfo()
                    {
                        title = jObj["name"].ToString().Trim(),
                        create_time = ConvertTime(double.Parse(jObj["created_time"].ToString())),
                        likes = String.Format("{0:#,##0}", jObj["likes"]["summary"]["total_count"]) + "♡"
                    });
                                        
                    if (items.Count == 8)
                        break;
                }

                ArticleList.ItemsSource = items;
                obj.RemoveAll();
                array.Clear();
            }
            catch (Exception ex)
            {
                ErrMsg.Text = ex.Message;
            }
        }
        
        private string ConvertTime(double timestamp)
        {
            // 시간으로 변환 - UNIX Timestamp to DateTime, GMT+9
            //return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp).AddHours(9).ToString("HH:mm");

            // 날짜로 변환
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp).AddHours(9).ToString("MM-dd");
        }
    }
}
