using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FilemakerSharp.Core.Commands
{
    class AvailableDBCommand
    {
        private Filemaker m_fm;

        internal AvailableDBCommand(Filemaker fm)
        {
            m_fm = fm;
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <returns>Available databases</returns>
        public async Task<List<string>> Execute()
        {
            List<string> databases = new List<string>();

            XmlNode xmlResponse = await m_fm.Communicate("-dbnames");

            List<string> errors = new List<string>();

            foreach (XmlNode rootNode in xmlResponse.ChildNodes)
            {
                switch (rootNode.Name)
                {
                    case "error":
                        string theError = rootNode.Attributes.GetNamedItem("code").Value;
                        if (theError != "0")
                        {
                            throw new FilemakerException("FileMaker Server returned an error in retrieving the XML.  Error: " + theError);
                        }
                        break;

                    case "resultset":
                        foreach (XmlNode fileNode in rootNode.ChildNodes)
                        {
                            string filename = fileNode.FirstChild.FirstChild.InnerText;

                            databases.Add(filename);
                        }

                        break;

                }
            }

            return databases;
        }
    }
}
