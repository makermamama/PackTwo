using System;
using System.IO;
using System.Xml;

namespace PackTwo
{
    class XmlTool
    {

        /// <summary>
        /// 导入XML文件
        /// </summary>
        /// <param name="XMLPath">XML文件路径</param>
        private static XmlDocument XMLLoad(string strPath)
        {
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                string filename = AppDomain.CurrentDomain.BaseDirectory.ToString() + strPath;
                if (File.Exists(filename)) xmldoc.Load(filename);
            }
            catch (Exception e)
            { }
            return xmldoc;
        }
        /// <summary>
        /// 获取节点的值
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string Read(string node)
        {
            string value = "";
            try
            {
                String msg = Directory.GetCurrentDirectory();
                XmlDocument doc = XMLLoad("\\seting.xml");
                XmlNode xn = doc.SelectSingleNode(node);
                value = xn.InnerText;
            }
            catch { }
            return value;
        }
        /// <summary>
        /// 获取节点集合
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string[] ReadNodes(string node)
        {
            string[] values = new string[10];
            try
            {
                String msg = Directory.GetCurrentDirectory();
                XmlDocument doc = XMLLoad("\\seting.xml");
                XmlNodeList xns = doc.SelectNodes(node);

                for(int i = 0; i < 10;i++) {
                    values[i] = xns[i].InnerText;
                }

            }
            catch {

            }
            return values;
        }

        /// <summary>
        /// 修改指定节点的数据
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="value">值</param>
        public static void Update(string node, string value)
        {
            try
            {
                XmlDocument doc = XMLLoad("\\seting.xml");
                XmlNode xn = doc.SelectSingleNode(node);
                xn.InnerText = value;
                doc.Save(AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\seting.xml");
            }
            catch { }
        }


        /// <summary>
        /// 修改指定节点的的属性
        /// </summary>
        /// <param name="node"></param>
        /// <param name="node"></param>
        /// <param name="property"></param>
        public static void UpdateNodeProperty(string node, string property, string value)
        {
            try
            {
                XmlDocument doc = XMLLoad("\\seting.xml");
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                xe.SetAttribute(property, value);
                doc.Save(AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\seting.xml");
            }
            catch { }
        }

        /// <summary>
        /// 获取指定节点的的属性
        /// </summary>
        /// <param name="node"></param>
        /// <param name="node"></param>
        public static String GetNodeProperty(string node, string property)
        {
            try
            {
                XmlDocument doc = XMLLoad("\\seting.xml");
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                return xe.GetAttribute(property);
            }
            catch {
                return "error";
            }

        }


    }
}
