using System;
using System.Collections.Generic;
using System.Text;

namespace FilemakerSharp.Core
{
    public class LayoutInfo
    {
        public string TableName { get; set; }

        public List<LayoutField> Fields { get; set; }
    }

    public class LayoutField
    {
        /// <summary>
        /// Field name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Field type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Field result
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Field auto enter?
        /// </summary>
        public bool AutoEnter { get; set; }
    }
}
