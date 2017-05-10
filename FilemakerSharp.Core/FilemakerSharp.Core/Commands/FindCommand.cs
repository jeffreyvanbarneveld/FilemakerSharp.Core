using FilemakerSharp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FilemakerSharp.Core.Commands
{
    public class FindCommand: IFindCommand
    {
        private Filemaker m_fm;
        private string m_layout;

        private string m_command = "";

        private int m_max = 0;
        private int m_skip = -1;

        private bool m_throwErrorOnNotFound;

        private List<FilemakerRecord> m_records;

        private List<FindItem> m_items = new List<FindItem>();

        /// <summary>
        /// New find command
        /// </summary>
        /// <param name="fm">FM instance</param>
        /// <param name="layout">Layout</param>
        /// <param name="throwError">Throw error when not found</param>
        internal FindCommand(Filemaker fm, string layout, bool throwError = true)
        {
            m_fm = fm;
            m_layout = layout;
            m_throwErrorOnNotFound = throwError;
        }

        /// <summary>
        /// Clear all items
        /// </summary>
        public void Clear()
        {
            m_items.Clear();
        }

        /// <summary>
        /// Set type
        /// </summary>
        /// <param name="type"></param>
        public void SetType(SearchType type)
        {
            if (type == SearchType.Find)
            {
                m_command = "-find";
            }
            else
                m_items.Clear();


            if (type == SearchType.FindAll)
                m_command = "-findall";
            else if (type == SearchType.FindAny)
                m_command = "-findany";
        }


        /// <summary>
        /// This parameter specifies the number of records to skip in the beginning of the found set. The default value is 0.
        /// </summary>
        /// <remarks>If the skip value is greater than the number of records found, then no record is returned.</remarks>
        /// <param name="howMany">How many records to skip.</param>
        public void SetSkip(int howMany)
        {
            m_skip = howMany;
        }

        /// <summary>
        /// Specifies the maximum number of records returned. By default, FileMaker returns all records.
        /// </summary>
        /// <param name="max">The max.</param>
        public void SetMax(int max)
        {

            m_max = max;
        }

        /// <summary>
        /// Execute find
        /// </summary>
        /// <returns></returns>
        public async Task<List<FilemakerRecord>> Execute()
        {
            string search = "";

            if (m_command == "-find")
            {
                foreach (var item in m_items)
                    search += "&" + Uri.EscapeDataString(item.Name) + "=" + Uri.EscapeDataString(item.Value) + "&" + Uri.EscapeDataString(item.Name) + ".op=" + RealSearchCriterium(item.Operator);
            }

            if (m_max != 0)
                search += "&-max=" + m_max;

            if (m_skip != -1)
                search += "&-skip=" + m_skip;

            XmlNode xmlResponse = await m_fm.Communicate("-lay=" + m_layout + "&" + m_command + search, true);



            m_records = new List<FilemakerRecord>();

            foreach (XmlNode rootNode in xmlResponse.ChildNodes)
            {
                switch (rootNode.Name)
                {
                    case "error":
                        string theError = rootNode.Attributes.GetNamedItem("code").Value;
                        if (theError != "0")
                        {
                            if (!m_throwErrorOnNotFound && theError == "401")
                                return null;

                            throw new FilemakerException("FileMaker Server returned an error in retrieving the XML.  Error: " + theError);
                        }
                        break;

                    case "resultset":
                        HandleRecords(rootNode);
                        break;

                    case "metadata":
                        // TODO: parse meta data
                        break;
                }
            }

            return m_records;
        }

        /// <summary>
        /// Add criterium
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        /// <param name="op">operator</param>
        public void AddCriterium(string name, string value, SearchOperator op)
        {
            SetType(SearchType.Find);

            m_items.Add(new FindItem
            {
                Name = name,
                Value = value,
                Operator = op
            });
        }


        /// <summary>
        /// Translates the searchOption enumerator into the search operator string FMSA expects.
        /// </summary>
        /// <param name="o">SearchOption enumerator value</param>
        /// <returns>string</returns>
        private static String RealSearchCriterium(SearchOperator o)
        {
            string temp = "";

            switch (o)
            {
                case SearchOperator.beginsWith:
                    temp = "bw";
                    break;
                case SearchOperator.biggerOrEqualThan:
                    temp = "gte";
                    break;
                case SearchOperator.biggerThan:
                    temp = "gt";
                    break;
                case SearchOperator.contains:
                    temp = "cn";
                    break;
                case SearchOperator.endsWith:
                    temp = "ew";
                    break;
                case SearchOperator.equals:
                    temp = "eq";
                    break;
                case SearchOperator.omit:
                    temp = "neq";
                    break;
                case SearchOperator.lessOrEqualThan:
                    temp = "lte";
                    break;
                case SearchOperator.lessThan:
                    temp = "lt";
                    break;
            }

            return temp;
        }

        private void HandleRecords(XmlNode rootNode)
        {
            foreach (XmlNode record in rootNode.ChildNodes)
            {
                string recordIDText = record.Attributes.GetNamedItem("record-id")?.Value;

                int recordID = (recordIDText != null) ? int.Parse(recordIDText) : 0;

                if (recordID == 0)
                    continue;

                FilemakerRecord recordObj = new FilemakerRecord { RecordID = recordID, Fields = new Dictionary<string, string>(), RelatedSets= new Dictionary<string, FilemakerRelatedTable>() };

                foreach (XmlNode item in record.ChildNodes)
                {
                    if (item.Name != "field")
                        continue;

                    string name = item.Attributes.GetNamedItem("name")?.Value;

                    string value = item.FirstChild?.FirstChild?.Value;

                    if (value == null)
                        value = "";

                    string test = "";
                    if (!recordObj.Fields.TryGetValue(name, out test))
                        recordObj.Fields.Add(name, value);
                }

                foreach (XmlNode item in record.ChildNodes)
                {
                    if (item.Name != "relatedset")
                        continue;

                    int count = int.Parse(item.Attributes.GetNamedItem("count")?.Value);
                    string table = item.Attributes.GetNamedItem("table")?.Value;

                    var relatedTable = new FilemakerRelatedTable { Count = count, Records = new List<Dictionary<string, string>>() };

                    foreach(XmlNode rec in item.ChildNodes)
                    {
                        Dictionary<string, string> fields = new Dictionary<string, string>();

                        foreach (XmlNode fld in rec.ChildNodes)
                        {
                            if (fld.Name != "field")
                                continue;

                            string name = fld.Attributes.GetNamedItem("name")?.Value;

                            string value = fld.FirstChild?.FirstChild?.Value;

                            if (value == null)
                                value = "";

                            string valTest;
                            if (!fields.TryGetValue(name, out valTest))
                                fields.Add(name, value);
                        }

                        relatedTable.Records.Add(fields);
                    }

                    recordObj.RelatedSets.Add(table, relatedTable);
                }


                m_records.Add(recordObj);
            }
        }
    }
}
