using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FilemakerSharp.Core.Interfaces
{
    public interface IFindCommand
    {

        /// <summary>
        /// Clear all items
        /// </summary>
        void Clear();

        /// <summary>
        /// Set type
        /// </summary>
        /// <param name="type"></param>
        void SetType(SearchType type);


        /// <summary>
        /// This parameter specifies the number of records to skip in the beginning of the found set. The default value is 0.
        /// </summary>
        /// <remarks>If the skip value is greater than the number of records found, then no record is returned.</remarks>
        /// <param name="howMany">How many records to skip.</param>
        void SetSkip(int howMany);

        /// <summary>
        /// Specifies the maximum number of records returned. By default, FileMaker returns all records.
        /// </summary>
        /// <param name="max">The max.</param>
        void SetMax(int max);

        /// <summary>
        /// Execute find
        /// </summary>
        /// <returns></returns>
        Task<List<FilemakerRecord>> Execute();

        /// <summary>
        /// Add criterium
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        /// <param name="op">operator</param>
        void AddCriterium(string name, string value, SearchOperator op);
    }
}
