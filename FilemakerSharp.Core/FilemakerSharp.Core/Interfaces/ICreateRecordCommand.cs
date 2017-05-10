using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FilemakerSharp.Core.Interfaces
{
    public interface ICreateRecordCommand
    {
        /// <summary>
        /// Clear fields
        /// </summary>
        void Clear();

        /// <summary>
        /// Add field to create
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="value">Field value</param>
        void AddField(string name, string value);

        /// <summary>
        /// Add fields to create
        /// </summary>
        /// <param name="fields">The fields</param>
        void AddFields(Dictionary<string, string> fields);

        /// <summary>
        /// Add script after execution
        /// </summary>
        /// <param name="name">Scriptname to execute</param>
        void AddScript(string name);

        /// <summary>
        /// Add script after execution with parameter
        /// </summary>
        /// <param name="name">Scriptname to execute</param>
        /// <param name="parameter">Script parameter</param>
        void AddScript(string name, string parameter);

        /// <summary>
        /// Execute :0
        /// </summary>
        Task<int> Execute();

    }
}
