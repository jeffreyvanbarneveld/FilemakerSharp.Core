using System;
using System.Collections.Generic;
using System.Text;

namespace FilemakerSharp.Core
{
    class FindItem
    {
        /// <summary>
        /// Field name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Search operator
        /// </summary>
        public SearchOperator Operator { get; set; }

        /// <summary>
        /// Search value
        /// </summary>
        public string Value { get; set; }
    }
}
