using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace WpfApplication1.Views
{
    public partial class InternetRecent : Page
    {
        public InternetRecent()
        {
            InitializeComponent();

            ((MainWindow)Application.Current.MainWindow).PageTitle.Text = "인터넷 최근 기사";
            GetXmlData();
        }

        private void GetXmlData()
        {
            XmlDocument xml = new XmlDocument();
            List<ArticleInfo> items = new List<ArticleInfo>();
            
            try
            {
                xml.Load(@"http://www.seoul.co.kr/rss/getHitArticle.php?type=new&count=30&output=xml");
                XmlNodeList xnList = xml.SelectNodes("/news/channel/item");

                foreach (XmlNode xn in xnList)
                {
                    if (xn["title"].InnerText.Trim() == "")
                        continue;

                    items.Add(new ArticleInfo()
                    {
                        title = xn["title"].InnerText.Trim(),
                        writer = xn["writer"].InnerText.Trim(),
                        create_time = xn["create_time"].InnerText.Trim().Substring(11, 5)
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
