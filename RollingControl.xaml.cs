using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Xml;

namespace WpfApplication1
{
    public partial class RollingControl : UserControl
    {
        public RollingControl()
        {
            InitializeComponent();

            GetXmlData();
        }
        
        private void GetXmlData()
        {
            XmlDocument xml = new XmlDocument();
            List<ArticleInfo> items = new List<ArticleInfo>();
            string num_char = "";

            try
            {
                xml.Load(@"http://www.seoul.co.kr/rss/getHitArticle.php?type=new&count=30&output=xml");
                XmlNodeList xnList = xml.SelectNodes("/news/channel/item");

                foreach (XmlNode xn in xnList)
                {
                    if (xn["title"].InnerText.Trim() == "")
                        continue;

                    switch (items.Count)
                    {
                        case 0:
                            num_char = "";
                            break;
                        case 1:
                            num_char = "";
                            break;
                        case 2:
                            num_char = "";
                            break;
                        case 3:
                            num_char = "";
                            break;
                        case 4:
                            num_char = "";
                            break;
                        case 5:
                            num_char = "";
                            break;
                        case 6:
                            num_char = "";
                            break;
                        case 7:
                            num_char = "";
                            break;
                    }

                    items.Add(new ArticleInfo()
                    {
                        title = num_char + " " + xn["title"].InnerText.Trim(),
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
            catch (Exception)
            {
            }
        }
    }
}
