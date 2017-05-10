using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FilemakerSharp.Core.Commands
{
    class LayoutInfoCommand
    {
        private Filemaker m_fm;
        private string m_layout;

        internal LayoutInfoCommand(Filemaker fm, string layout)
        {
            m_fm = fm;
            m_layout = layout;
        }


        /// <summary>
        /// Execute command
        /// </summary>
        /// <returns>Available items</returns>
        public async Task<LayoutInfo> Execute()
        {
            LayoutInfo info = new LayoutInfo();

            List<LayoutField> fields = new List<LayoutField>();

            XmlNode xmlResponse = await m_fm.Communicate("-lay=" + m_layout + "&-view", true);

            foreach (XmlNode rootNode in xmlResponse.ChildNodes)
            {
                switch (rootNode.Name)
                {
                    case "datasource":
                        foreach (XmlAttribute a in rootNode.Attributes)
                        {
                            switch (a.Name)
                            {
                                case "table":
                                    info.TableName = a.Value;
                                    break;
                            }
                        }
                        break;

                    case "error":
                        string theError = rootNode.Attributes.GetNamedItem("code").Value;
                        if (theError != "0")
                        {
                            throw new FilemakerException("FileMaker Server returned an error in retrieving the XML.  Error: " + theError);
                        }
                        break;

                    case "metadata":
                        foreach (XmlNode fileNode in rootNode.ChildNodes)
                        {
                            string fieldname = fileNode.Attributes["name"].InnerText;
                            string type = fileNode.Attributes["type"].InnerText;
                            string result = fileNode.Attributes["result"].InnerText;
                            bool AutoEnter = fileNode.Attributes["auto-enter"].InnerText == "yes";

                            fields.Add(new LayoutField
                            {
                                Name = fieldname,
                                Type = type,
                                Result = result,
                                AutoEnter = AutoEnter
                            });
                        }

                        break;

                }
            }

            info.Fields = fields;

            return info;
        }
    }
}
