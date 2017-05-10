using System;
using System.Collections.Generic;
using System.Text;

namespace FilemakerSharp.Core
{
    public class FilemakerRecord
    {

        /// <summary>
        /// Record ID
        /// </summary>
        public int RecordID { get; set; }

        /// <summary>
        /// Record fields
        /// </summary>
        public Dictionary<string, string> Fields { get; set; }

        public Dictionary<string, FilemakerRelatedTable> RelatedSets { get; set; }
    }

    public class FilemakerRelatedTable
    {
        public int Count { get; set; }

        public List<Dictionary<string, string>> Records { get; set; }
    }
}
