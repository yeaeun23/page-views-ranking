using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace WpfApplication1.Views
{
    public partial class PVRankYesterdayToday : Page
    {
        public PVRankYesterdayToday()
        {
            InitializeComponent();

            ((MainWindow)Application.Current.MainWindow).PageTitle.Text = "기사 조회순 - 어제~오늘";
            GetXmlData();
        }

        private void GetXmlData()
        {
            XmlDocument xml = new XmlDocument();
            List<ArticleInfo> items = new List<ArticleInfo>();

            try
            {
                xml.Load(@"http://www.seoul.co.kr/rss/getHitArticle.php?type=hit&cdate_from=" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + "&cdate_to=" + DateTime.Now.ToString("yyyy-MM-dd"));
                XmlNodeList xnList = xml.SelectNodes("/news/channel/item");

                foreach (XmlNode xn in xnList)
                {
                    if (xn["title"].InnerText.Trim() == "")
                        continue;

                    items.Add(new ArticleInfo()
                    {
                        title = xn["title"].InnerText.Trim(),
                        writer = xn["writer"].InnerText.Trim(),
                        hits = String.Format("{0:#,##0}", Convert.ToInt32(xn["hit_sum"].InnerText.Trim()))
                    });
                    xn.RemoveAll();

                    if (items.Count == 8)
                        break;
                }

                ArticleList.ItemsSource = items;
                xml.RemoveAll();
            }
            catch (Exception ex)
            {
                ErrMsg.Text = ex.Message;
            }
        }
    }
}
