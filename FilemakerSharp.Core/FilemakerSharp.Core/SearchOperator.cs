using System;
using System.Collections.Generic;
using System.Text;

namespace FilemakerSharp.Core
{
    /// <summary>
    /// The different search options you can use.
    /// </summary>
    public enum SearchOperator
    {
        /// <summary>
        /// Field matches search value exactly.
        /// </summary>
        equals,
        /// <summary>
        /// Field contains search value.
        /// </summary>
        contains,
        /// <summary>
        /// Field starts with search value.
        /// </summary>
        beginsWith,
        /// <summary>
        /// Field ends with search value.
        /// </summary>
        endsWith,
        /// <summary>
        /// Field is greater than search value.
        /// </summary>
        biggerThan,
        /// <summary>
        /// Field is greater than or equal to search value.
        /// </summary>
        biggerOrEqualThan,
        /// <summary>
        /// Field is less than search value.
        /// </summary>
        lessThan,
        /// <summary>
        /// Field is less than or equal to search value.
        /// </summary>
        lessOrEqualThan,
        /// <summary>
        /// Exclude records that match search value.
        /// </summary>
        omit
    }
}
