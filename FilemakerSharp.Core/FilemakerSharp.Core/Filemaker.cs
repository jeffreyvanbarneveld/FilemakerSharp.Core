using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using FilemakerSharp.Core.Commands;
using System.Net.Http;
using System.Threading.Tasks;
using FilemakerSharp.Core.Interfaces;

namespace FilemakerSharp.Core
{
    public class Filemaker: IFilemakerClient
    {
        private int m_port;
        private string m_host;
        private string m_username;
        private string m_password;
        private string m_protocol;

        private List<string> m_databases;
        private List<string> m_layoutNames;

        private string m_selectedDatabase;
        private bool m_validateDatabase;

        private string m_siteurl;
        private string m_baseurl;

        public static int TimeOut
        {
            get
            {
                return FMHttpHelper.Timeout;
            }

            set
            {
                FMHttpHelper.Timeout = TimeOut;
            }
        }

        /// <summary>
        /// Get databases
        /// </summary>
        public List<string> Databases
        {
            get { return m_databases; }
        }

        /// <summary>
        /// Layout names
        /// </summary>
        public List<string> LayoutNames
        {
            get { return m_layoutNames; }
        }

        /// <summary>
        /// Filemaker instance
        /// </summary>
        /// <param name="host"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public Filemaker(string host, string username, string password)
            : this(host, username, password, "http", 80) { }

        /// <summary>
        /// Filemaker instance
        /// </summary>
        /// <param name="host"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="port"></param>
        public Filemaker(string host, string username, string password, int port)
            : this(host, username, password, "http", port) { }

        /// <summary>
        /// Filemaker instance
        /// </summary>
        /// <param name="host">Hostname</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="protocol">Protocol</param>
        /// <param name="port">Port</param>
        public Filemaker(string host, string username, string password, string protocol = "http", int port = 80, bool validateDatabase = true)
        {
            m_host = host;
            m_port = port;

            m_username = username;
            m_password = password;

            m_protocol = protocol;

            m_siteurl = m_protocol + "://" + m_host + ":" + port;
            m_baseurl = m_siteurl + "/fmi/xml/fmresultset.xml";

            m_validateDatabase = validateDatabase;

            if (validateDatabase)
                m_databases = getDatabases().Result;
        }

        /// <summary>
        /// Set database
        /// </summary>
        /// <param name="databaseName"></param>
        public void SetDatabase(string databaseName)
        {
            if (m_validateDatabase && m_databases.Where(p => p == databaseName).Count() == 0)
                throw new FilemakerException("Database \"" + databaseName + "\" not found on server");

            m_selectedDatabase = databaseName;

            if (m_validateDatabase)
                m_layoutNames = getLayouts().Result;
        }

        /// <summary>
        /// Communicate with FM server
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns></returns>
        internal async Task<XmlNode> Communicate(string data, bool withDB = false)
        {
            if (withDB && string.IsNullOrEmpty(m_selectedDatabase))
                throw new FilemakerException("No database not selected!");

            if (withDB)
                return await FMHttpHelper.PostXML(m_baseurl, "-db=" + m_selectedDatabase + "&" + data, m_username, m_password);
            else
                return await FMHttpHelper.PostXML(m_baseurl, data, m_username, m_password);
        }

        /// <summary>
        /// Create find command
        /// </summary>
        /// <param name="layout">The layout</param>
        /// <param name="throwError">Throw error when not found</param>
        /// <returns></returns>
        public IFindCommand CreateFindCommand(string layout, bool throwError = true)
        {
            return new FindCommand(this, layout, throwError);
        }

        /// <summary>
        /// Create record command
        /// </summary>
        /// <param name="layout">Layout name</param>
        /// <returns></returns>
        public ICreateRecordCommand CreateRecordCommand(string layout)
        {
            return new CreateRecordCommand(this, layout);
        }

        /// <summary>
        /// Create record command
        /// </summary>
        /// <param name="layout">Layout name</param>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public async Task<int> CreateRecord(string layout, Dictionary<string, string> fields)
        {
            CreateRecordCommand command = new CreateRecordCommand(this, layout);
            command.AddFields(fields);

            return await command.Execute();
        }

        /// <summary>
        /// Get databases
        /// </summary>
        /// <returns>Database list</returns>
        private async Task<List<string>> getDatabases()
        {
            AvailableDBCommand command = new AvailableDBCommand(this);

            return await command.Execute();
        }

        /// <summary>
        /// Get field from layout
        /// </summary>
        /// <param name="layoutName">Layoutname</param>
        /// <returns></returns>
        public async Task<LayoutInfo> GetLayoutInfo(string layoutName)
        {
            if (m_validateDatabase && m_layoutNames.Where(p => p == layoutName).Count() == 0)
                throw new FilemakerException("Layout " + layoutName + " not found");

            LayoutInfoCommand command = new LayoutInfoCommand(this, layoutName);

            return await command.Execute();
        }

        /// <summary>
        /// Delete record from layout
        /// </summary>
        /// <param name="layoutname">Layout name</param>
        /// <param name="recordID">Record ID</param>
        public async void DeleteRecord(string layoutname, int recordID)
        {
            DeleteRecordCommand command = new DeleteRecordCommand(this, layoutname, recordID);
            await command.Execute();
        }


        /// <summary>
        /// Edit record from layout
        /// </summary>
        /// <param name="layoutname">Layout name</param>
        /// <param name="recordID">Record ID</param>
        public EditRecordCommand EditRecordCommand(string layoutname, int recordID)
        {
            EditRecordCommand command = new EditRecordCommand(this, layoutname, recordID);

            return command;
        }

        /// <summary>
        /// Get layout names
        /// </summary>
        /// <returns>The layout names</returns>
        private async Task<List<string>> getLayouts()
        {
            AvailableLayoutsCommand command = new AvailableLayoutsCommand(this);

            return (await command.Execute()).Except(new List<string> { "" }).ToList();
        }

        /// <summary>
        /// Download file from filemaker
        /// </summary>
        /// <param name="layoutname"></param>
        /// <param name="filename"></param>
        /// <param name="fieldName"></param>
        /// <param name="recordID"></param>
        /// <returns></returns>
        public async Task<byte[]> DownloadFile(string layoutname, string filename, string fieldName, int recordID)
        {
            string url = m_siteurl + "/fmi/xml/cnt/" + filename + "?-db=" + m_selectedDatabase + "&-lay=" + layoutname + "&-recid=" + recordID + "&-field=" + fieldName;
            

            var webClient = new HttpClient();
            webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(m_username + ":" + m_password)));

            return await webClient.GetByteArrayAsync(url);
        }
    }
}
