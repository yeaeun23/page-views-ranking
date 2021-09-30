using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace WpfApplication1.Views
{
    public partial class FacebookRank : Page
    {
        public FacebookRank()
        {
            InitializeComponent();

            ((MainWindow)Application.Current.MainWindow).PageTitle.Text = "페이스북 좋아요순";
            GetJsonData(RequestJson());
        }

        private string RequestJson()
        {
            string result = "";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://appsvr2.seoul.co.kr/baljae/service.aspx?FN=FACEBOOKLIKE");
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
                JArray array = JArray.Parse(obj["data"].ToString());

                foreach (JObject jObj in array)
                {
                    if (jObj["TITLE"] == null)
                        continue;

                    items.Add(new ArticleInfo()
                    {
                        title = jObj["TITLE"].ToString().Trim(),
                        create_time = jObj["DATE"].ToString().Substring(5, 5),
                        likes = String.Format("{0:#,##0}", Convert.ToInt32(jObj["HIT"])) + "♡"
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
    }
}
