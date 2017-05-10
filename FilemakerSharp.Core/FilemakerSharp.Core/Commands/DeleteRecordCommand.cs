using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FilemakerSharp.Core.Commands
{
    class DeleteRecordCommand
    {
        private Filemaker m_fm;
        private string m_layout;
        private int m_recordID;

        internal DeleteRecordCommand(Filemaker fm, string layout, int recordID)
        {
            m_fm = fm;
            m_layout = layout;
            m_recordID = recordID;
        }

        public async Task<string> Execute()
        {
            XmlNode xmlResponse = await m_fm.Communicate("-lay=" + m_layout + "&-delete&-recid=" + m_recordID, true);


            string errorcode = "0";
            foreach (XmlNode rootNode in xmlResponse.ChildNodes)
            {
                switch (rootNode.Name)
                {

                    case "error":
                        errorcode = rootNode.Attributes.GetNamedItem("code").Value;
                        break;

                }
            }

            return errorcode;
        }
    }
}
