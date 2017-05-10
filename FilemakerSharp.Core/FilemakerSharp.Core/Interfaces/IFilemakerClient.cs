using FilemakerSharp.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FilemakerSharp.Core.Interfaces
{
    public class IFilemakerClient
    {
        public interface Filemaker
        {
            int TimeOut { get; set; }

            /// <summary>
            /// Get databases
            /// </summary>
            List<string> Databases { get; }

            /// <summary>
            /// Layout names
            /// </summary>
            List<string> LayoutNames { get; }

            /// <summary>
            /// Set database
            /// </summary>
            /// <param name="databaseName"></param>
            void SetDatabase(string databaseName);

            /// <summary>
            /// Create find command
            /// </summary>
            /// <param name="layout">The layout</param>
            /// <param name="throwError">Throw error when not found</param>
            /// <returns></returns>
            IFindCommand CreateFindCommand(string layout, bool throwError = true);

            /// <summary>
            /// Create record command
            /// </summary>
            /// <param name="layout">Layout name</param>
            /// <returns></returns>
            ICreateRecordCommand CreateRecordCommand(string layout);

            /// <summary>
            /// Create record command
            /// </summary>
            /// <param name="layout">Layout name</param>
            /// <param name="fields">Fields</param>
            /// <returns></returns>
            Task<int> CreateRecord(string layout, Dictionary<string, string> fields);

            /// <summary>
            /// Get field from layout
            /// </summary>
            /// <param name="layoutName">Layoutname</param>
            /// <returns></returns>
            Task<LayoutInfo> GetLayoutInfo(string layoutName);

            /// <summary>
            /// Delete record from layout
            /// </summary>
            /// <param name="layoutname">Layout name</param>
            /// <param name="recordID">Record ID</param>
            void DeleteRecord(string layoutname, int recordID);


            /// <summary>
            /// Edit record from layout
            /// </summary>
            /// <param name="layoutname">Layout name</param>
            /// <param name="recordID">Record ID</param>
            IEditRecordCommand EditRecordCommand(string layoutname, int recordID);

            /// <summary>
            /// Get layout names
            /// </summary>
            /// <returns>The layout names</returns>
            Task<List<string>> getLayouts();

            /// <summary>
            /// Download file from filemaker
            /// </summary>
            /// <param name="layoutname"></param>
            /// <param name="filename"></param>
            /// <param name="fieldName"></param>
            /// <param name="recordID"></param>
            /// <returns></returns>
            Task<byte[]> DownloadFile(string layoutname, string filename, string fieldName, int recordID);
        }
    }
}
