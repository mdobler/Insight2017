using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Xml.Linq;
using System.Xml;

namespace VisionAPIHelper
{
    public class VisionAPIRepository : HelperBase
    {
        //NLOG reference
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private  VisionAPI.DeltekVisionOpenAPIWebServiceSoapClient service;

        private string database = "";
        private string username = "";
        private string password = "";

        private bool useSession = true;
        private string sessionId = "";
        private DateTime sessionStart;
        private double maxSessionTimeout = 10;

        /// <summary>
        /// constructor. creates vision service reference. The web.config or app.config must have corresponding
        /// configuration sections for both "DeltekVisionOpenAPIWebServiceSoap" and "DeltekVisionOpenAPIWebServiceSoapSSL"
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="database"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="useHTTPS"></param>
        /// <param name="httpsUser"></param>
        /// <param name="httpsPassword"></param>
        /// <param name="httpsDomain"></param>
        /// <param name="useSession"></param>
        public VisionAPIRepository(string uri, string database, string username, string password,
            bool useHTTPS = false, string httpsUser = "", string httpsPassword = "", string httpsDomain = "",
            bool useSession = true)
        {

            try
            {
                service = GetService(uri, useHTTPS, httpsUser, httpsPassword, httpsDomain);

                this.database = database;
                this.username = username;
                this.password = password;
                this.useSession = useSession;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Connection to service failed with URI {uri}");
                throw new ApplicationException($"Connection to service failed with URI {uri}", ex);
            }

        }

        /// <summary>
        /// returns connection info (including session if selected)
        /// </summary>
        /// <returns></returns>
        private string GetConnectionInfo()
        {
            string _retval = GetVisionConnInfoXML(database, username, password);

            try
            {
                if (useSession)
                {
                    if (string.IsNullOrEmpty(sessionId) || (DateTime.Now - sessionStart).TotalMinutes > maxSessionTimeout)
                    {
                        sessionId = service.ValidateLogin(GetVisionConnInfoXML(database, username, password));
                        sessionStart = DateTime.Now;
                    }

                    _retval = GetVisionConnInfoXML(database, username, password, sessionId);
                }

                return _retval;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Connection failed with database {database} and user {username}");
                throw new ApplicationException($"Connection failed with database {database} and user {username}", ex);
            }
        }

        /// <summary>
        /// checks service info
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="useHTTPS"></param>
        /// <param name="httpsUser"></param>
        /// <param name="httpsPassword"></param>
        /// <param name="httpsDomain"></param>
        /// <returns></returns>
        public static VisionAPI.DeltekVisionOpenAPIWebServiceSoapClient GetService(string uri, bool useHTTPS = false, string httpsUser = "", string httpsPassword = "", string httpsDomain = "")
        {
            VisionAPI.DeltekVisionOpenAPIWebServiceSoapClient _service;

            if (useHTTPS)
            {
                _service = new VisionAPI.DeltekVisionOpenAPIWebServiceSoapClient("DeltekVisionOpenAPIWebServiceSoapSSL");
            }
            else
            {
                _service = new VisionAPI.DeltekVisionOpenAPIWebServiceSoapClient("DeltekVisionOpenAPIWebServiceSoap");
            }

            _service.Endpoint.Address = new System.ServiceModel.EndpointAddress(uri);

            if (useHTTPS && string.IsNullOrEmpty(httpsUser) == false)
            {
                _service.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential(httpsUser, httpsPassword, httpsDomain);
                _service.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Identification;
            }

            return _service;
        }

        /// <summary>
        /// checks if web service is active at location
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="useHTTPS"></param>
        /// <param name="httpsUser"></param>
        /// <param name="httpsPassword"></param>
        /// <param name="httpsDomain"></param>
        /// <returns></returns>
        public static bool IsURLCorrect(string uri, bool useHTTPS = false, string httpsUser = "", string httpsPassword = "", string httpsDomain = "")
        {
            try
            {
                var _service = GetService(uri, useHTTPS, httpsUser, httpsPassword, httpsDomain);
                var _retval = _service.MyTest();

                return (string.IsNullOrEmpty(_retval) ? false : true);
            }
            catch (Exception ex)
            {
                //logger.Error(ex, $"Connection failed with uri {uri}");
                return false;
            }
        }

        /// <summary>
        /// checks for valid database, username, password
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="database"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="useHTTPS"></param>
        /// <param name="httpsUser"></param>
        /// <param name="httpsPassword"></param>
        /// <param name="httpsDomain"></param>
        /// <returns></returns>
        public static bool CanAuthenticate(string uri, string database, string username, string password,
            bool useHTTPS = false, string httpsUser = "", string httpsPassword = "", string httpsDomain = "")
        {
            try
            {
                var _service = GetService(uri, useHTTPS, httpsUser, httpsPassword, httpsDomain);
                var _retval = _service.ValidateLogin(HelperBase.GetVisionConnInfoXML(database, username, password));

                var _message = VisionMessage.FromXML(_retval);

                return (_message.ReturnCode == "ErrLoginVal" ? false : true);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// retrieves a list of all available databases at the uri location
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="useHTTPS"></param>
        /// <param name="httpsUser"></param>
        /// <param name="httpsPassword"></param>
        /// <param name="httpsDomain"></param>
        /// <returns></returns>
        public static List<string> GetAvailableDatabases(string uri, bool useHTTPS = false, string httpsUser = "", string httpsPassword = "", string httpsDomain = "")
        {

            List<string> _retval = new List<string>();

            try
            {
                var _service = GetService(uri, useHTTPS, httpsUser, httpsPassword, httpsDomain);
                var _xml = _service.GetDatabases();
                XDocument _doc = XDocument.Load(new System.IO.StringReader(_xml));

                foreach (var e in _doc.Elements("databases").Elements("desc"))
                {
                    _retval.Add(e.Value);
                }

                return _retval;
            }
            catch (Exception)
            {
                return new List<string>();
            }

            

        }

        /// <summary>
        /// retrieves the system info from the API
        /// </summary>
        /// <returns></returns>
        public string GetSystemInfo()
        {
            string _retval = "";

            try
            {
                _retval = service.GetSystemInfo(GetConnectionInfo());
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error requesting system info from Vision API");
                throw new ApplicationException("Error requesting system info from Vision API", ex);
            }

            return _retval;
        }

        /// <summary>
        /// retrieves the current user info from the API
        /// </summary>
        /// <returns></returns>
        public string GetCurrentUserInfo()
        {
            string _retval = "";

            try
            {
                _retval = service.GetCurrentUserInfo(GetConnectionInfo());
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error requesting current user info from Vision API");
                throw new ApplicationException("Error requesting current user info from Vision API", ex);
            }

            return _retval;
        }

        #region Standard Info Center Methods
        /// <summary>
        /// loops through the data
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="xmlstringdata"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool ReadResult(ref string sessionId, string xmlstringdata, ref VisionMessage message)
        {
            bool _lastChunk = false;

            //create new message if reference is null
            if (message == null) { message = new VisionMessage(); }

            //get existing data from message...
            XDocument _existingData = message.ReturnedData;

            var _xdocResult = XDocument.Load(new System.IO.StringReader(xmlstringdata));
            var _message = VisionMessage.FromXML(_xdocResult);
            if (string.IsNullOrEmpty(_message.ReturnCode) == false && _message.ReturnCode != "1")
            {
                message = _message;
                return true;
            }
            
            //append results
            if (_xdocResult.Elements("RECS") != null)
            {
                sessionId = _xdocResult.Element("RECS").Attribute("SessionID").Value;
                
                if (_xdocResult.Element("RECS").Attribute("LastChunk") == null)
                {
                    _lastChunk = true;
                }
                else
                {
                    _lastChunk = (_xdocResult.Element("RECS").Attribute("LastChunk").Value == "1" ? true : false);
                }

                if (_existingData == null || _existingData.ElementExists("RECS") == false)
                {
                    _existingData = _xdocResult;
                }
                else
                {
                    _existingData.Element("RECS").Add(_xdocResult.Element("RECS").Elements());
                }
            }

            //put together message
            message.ReturnedData = _existingData;
            message.ReturnCode = "1";
            message.ReturnDesc = "Succesful";

            return _lastChunk;
        }

        /// <summary>
        /// get records by key
        /// </summary>
        /// <param name="infoCenter"></param>
        /// <param name="keys"></param>
        /// <param name="recordDetail"></param>
        /// <param name="rowAccess"></param>
        /// <param name="chunkSize"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public VisionMessage GetRecordsByKey(string infoCenter,
            VisionKeyList keys,
            RecordDetail recordDetail = RecordDetail.Empty,
            bool rowAccess = false,
            int chunkSize = 100,
            XElement tableInfo = null)
        {
            try
            {
                VisionMessage _retval = new VisionMessage();
                int _nextChunk = 1;
                bool _lastChunk = false;
                string _sessionId = "";
                string _connInfo = GetConnectionInfo();

                do
                {
                    string _xmlretval = service.GetRecordsByKey(
                        _connInfo,
                        GetInfoCenterXML(infoCenter, rowAccess, _nextChunk, chunkSize, tableInfo),
                        keys.ToString(),
                        recordDetail.GetValueName()
                        );

                    //read loop moved into separate method...
                    _lastChunk = ReadResult(ref _sessionId, _xmlretval, ref _retval);
                    if (string.IsNullOrEmpty(_sessionId) == false) { _connInfo = GetVisionConnInfoXML(database, username, password, _sessionId); }
                    _nextChunk += 1;

                } while (_lastChunk == false);

                return _retval;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"GetRecordsByKey() with keys [{keys.ToString()}] call failed");
                return VisionMessage.FromException(ex);
            }
        }

        /// <summary>
        /// get records by key
        /// </summary>
        /// <param name="infoCenter"></param>
        /// <param name="keys"></param>
        /// <param name="recordDetail"></param>
        /// <param name="rowAccess"></param>
        /// <param name="chunkSize"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public VisionMessage GetRecordsByKey(VisionInfoCenters infoCenter,
            VisionKeyList keys,
            RecordDetail recordDetail = RecordDetail.Empty,
            bool rowAccess = false,
            int chunkSize = 100,
            XElement tableInfo = null)
        {
            return GetRecordsByKey(infoCenter.GetValueName(), keys, recordDetail, rowAccess, chunkSize, tableInfo);
        }

        /// <summary>
        /// get records by key (singular)
        /// </summary>
        /// <param name="infoCenter"></param>
        /// <param name="key"></param>
        /// <param name="recordDetail"></param>
        /// <param name="rowAccess"></param>
        /// <param name="chunkSize"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public VisionMessage GetRecordsByKey(string infoCenter,
            VisionKey key,
            RecordDetail recordDetail = RecordDetail.Empty,
            bool rowAccess = false,
            int chunkSize = 100,
            XElement tableInfo = null)
        {
            return GetRecordsByKey(infoCenter, new VisionKeyList() { key }, recordDetail, rowAccess, chunkSize, tableInfo);
        }

        /// <summary>
        /// get records by key (singular
        /// </summary>
        /// <param name="infoCenter"></param>
        /// <param name="key"></param>
        /// <param name="recordDetail"></param>
        /// <param name="rowAccess"></param>
        /// <param name="chunkSize"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public VisionMessage GetRecordsByKey(VisionInfoCenters infoCenter,
            VisionKey key,
            RecordDetail recordDetail = RecordDetail.Empty,
            bool rowAccess = false,
            int chunkSize = 100,
            XElement tableInfo = null)
        {
            return GetRecordsByKey(infoCenter, new VisionKeyList() { key }, recordDetail, rowAccess, chunkSize, tableInfo);
        }

        /// <summary>
        /// get records by query
        /// </summary>
        /// <param name="infoCenter"></param>
        /// <param name="query"></param>
        /// <param name="recordDetail"></param>
        /// <param name="rowAccess"></param>
        /// <param name="chunkSize"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public VisionMessage GetRecordsByQuery(string infoCenter,
            string query,
            RecordDetail recordDetail = RecordDetail.Empty,
            bool rowAccess = false,
            int chunkSize = 100,
            XElement tableInfo = null)
        {
            try
            {
                VisionMessage _retval = new VisionMessage();
                int _nextChunk = 1;
                bool _lastChunk = false;
                string _sessionId = "";
                string _connInfo = GetConnectionInfo();
                XElement _xQuery = new XElement("Queries", new XElement("Query", query, new XAttribute("ID", 1)));

                do
                {
                    string _xmlretval = service.GetRecordsByQuery(
                        _connInfo,
                        GetInfoCenterXML(infoCenter, rowAccess, _nextChunk, chunkSize, tableInfo),
                        _xQuery.ToString(),
                        recordDetail.GetValueName()
                        );

                    //read loop moved into separate method...
                    _lastChunk = ReadResult(ref _sessionId, _xmlretval, ref _retval);
                    if (string.IsNullOrEmpty(_sessionId) == false) { _connInfo = GetVisionConnInfoXML(database, username, password, _sessionId); }
                    _nextChunk += 1;

                } while (_lastChunk == false);

                return _retval;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"GetRecordsByQuery() with query [{query}] call failed");
                return VisionMessage.FromException(ex);
            }
        }

        /// <summary>
        /// get records by query
        /// </summary>
        /// <param name="infoCenter"></param>
        /// <param name="query"></param>
        /// <param name="recordDetail"></param>
        /// <param name="rowAccess"></param>
        /// <param name="chunkSize"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public VisionMessage GetRecordsByQuery(VisionInfoCenters infoCenter,
            string query,
            RecordDetail recordDetail = RecordDetail.Empty,
            bool rowAccess = false,
            int chunkSize = 100,
            XElement tableInfo = null)
        {
            return GetRecordsByQuery(infoCenter.GetValueName(), query, recordDetail, rowAccess, chunkSize, tableInfo);
        }

        /// <summary>
        /// sends data for processing with return values
        /// </summary>
        /// <param name="infoCenter"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public VisionMessage SendDataToDeltekVisionWithReturn(string infoCenter, XElement data)
        {
            try
            {
                string _strData = data.ToString().Replace("xmlns=\"\"", "");
                string _strMessage = service.SendDataToDeltekVisionWithReturn(infoCenter, GetConnectionInfo(), _strData, "1");
                return VisionMessage.FromXML(_strMessage);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"SendDataToDeltekVisionWithReturn() failed");
                return VisionMessage.FromException(ex);
            }

        }

        public VisionMessage SendDataToDeltekVisionWithReturn(string infoCenter, string data)
        {
            try
            {
                string _strData = data.Replace("xmlns=\"\"", "");
                string _strMessage = service.SendDataToDeltekVisionWithReturn(infoCenter, GetConnectionInfo(), _strData, "1");
                return VisionMessage.FromXML(_strMessage);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"SendDataToDeltekVisionWithReturn() failed");
                return VisionMessage.FromException(ex);
            }

        }

        /// <summary>
        /// sends data for processing with return values
        /// </summary>
        /// <param name="infoCenter"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public VisionMessage SendDataToDeltekVisionWithReturn(VisionInfoCenters infoCenter, XElement data)
        {
            return SendDataToDeltekVisionWithReturn(infoCenter.GetValueName(), data);
        }

        /// <summary>
        /// sends data for processing
        /// </summary>
        /// <param name="infoCenter"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public VisionMessage SendDataToDeltekVision(string infoCenter, XElement data)
        {
            try
            {
                string _strData = data.ToString().Replace("xmlns=\"\"", "");
                string _strMessage = service.SendDataToDeltekVision(infoCenter, GetConnectionInfo(), _strData);
                return VisionMessage.FromXML(_strMessage);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"SendDataToDeltekVision() failed");
                return VisionMessage.FromException(ex);
            }

        }

        public VisionMessage SendDataToDeltekVision(string infoCenter, string data)
        {
            try
            {
                string _strData = data.Replace("xmlns=\"\"", "");
                string _strMessage = service.SendDataToDeltekVision(infoCenter, GetConnectionInfo(), _strData);
                return VisionMessage.FromXML(_strMessage);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"SendDataToDeltekVision() failed");
                return VisionMessage.FromException(ex);
            }

        }

        /// <summary>
        /// sends data for processing
        /// </summary>
        /// <param name="infoCenter"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public VisionMessage SendDataToDeltekVision(VisionInfoCenters infoCenter, XElement data)
        {
            return SendDataToDeltekVision(infoCenter.GetValueName(), data);
        }

        /// <summary>
        /// delete records from Vision
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public VisionMessage DeleteRecords(XElement data)
        {
            try
            {
                string _strData = data.ToString().Replace("xmlns=\"\"", "");
                string _strMessage = service.DeleteRecords(GetConnectionInfo(), _strData);
                return VisionMessage.FromXML(_strMessage);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"DeleteRecords() failed");
                return VisionMessage.FromException(ex);
            }
        }

        public VisionMessage DeleteRecords(string data)
        {
            try
            {
                string _strData = data.Replace("xmlns=\"\"", "");
                string _strMessage = service.DeleteRecords(GetConnectionInfo(), _strData);
                return VisionMessage.FromXML(_strMessage);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"DeleteRecords() failed");
                return VisionMessage.FromException(ex);
            }
        }
        #endregion

        #region Transaction Methods
        /// <summary>
        /// available transaction types
        /// </summary>



        public VisionMessage AddTransaction(TransactionType transType, XElement data)
        {
            try
            {
                string _connInfo = GetConnectionInfo();
                string _strData = data.ToString().Replace("xmlns=\"\"", "");
                string _message = "";

                switch (transType)
                {
                    case TransactionType.AP:
                        _message = service.AddAPVouchersTransaction(_connInfo, _strData);
                        break;
                    case TransactionType.CD:
                        _message = service.AddCashDisbTransaction(_connInfo, _strData);
                        break;
                    case TransactionType.CR:
                        _message = service.AddCashReceiptsTransaction(_connInfo, _strData);
                        break;
                    case TransactionType.CV:
                        _message = service.AddAPDisbursementsTransaction(_connInfo, _strData);
                        break;
                    case TransactionType.ER:
                        _message = service.AddEmpRepaymentTransaction(_connInfo, _strData);
                        break;
                    case TransactionType.EX:
                        _message = service.AddEmpExpenseTransaction(_connInfo, _strData);
                        break;
                    case TransactionType.IN:
                        _message = service.AddInvoiceTransaction(_connInfo, _strData);
                        break;
                    case TransactionType.JE:
                        _message = service.AddJournalEntryTransaction(_connInfo, _strData);
                        break;
                    case TransactionType.LA:
                        _message = service.AddLaborAdjustTransaction(_connInfo, _strData);
                        break;
                    case TransactionType.MI:
                        _message = service.AddMiscTransaction(_connInfo, _strData);
                        break;
                    case TransactionType.PR:
                        _message = service.AddPrintsReproTransaction(_connInfo, _strData);
                        break;
                    case TransactionType.TS:
                        _message = service.AddTimesheetTransaction(_connInfo, _strData);
                        break;
                    case TransactionType.UN:
                        _message = service.AddUnitTransaction(_connInfo, _strData);
                        break;
                    case TransactionType.UP:
                        _message = service.AddUnitByProjectTransaction(_connInfo, _strData);
                        break;
                    default:
                        break;
                }

                return VisionMessage.FromXML(_message);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"AddTransaction() failed for Transaction Type {transType.GetValueName()}");
                return VisionMessage.FromException(ex);
            }
            
        }

        /// <summary>
        /// posts one or more batches into vision
        /// </summary>
        /// <returns></returns>
        public VisionMessage PostTransaction(TransactionType transType, string batchList, int period)
        {
            try
            {
                string _message = service.PostTransaction(GetConnectionInfo(), transType.GetValueName(), batchList, period);

                //catch special case when the return value does not contain any xml
                if (_message.Contains("<") == false)
                {
                    return new VisionMessage("1", "Posting Successful", _message);
                }
                else
                {
                    return VisionMessage.FromXML(_message);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"PostTransaction() failed for Transaction Type {transType.GetValueName()}");
                return VisionMessage.FromException(ex);
            }
        }

        /// <summary>
        /// retrieves transaction details by key
        /// </summary>
        /// <param name="transType"></param>
        /// <param name="keys"></param>
        /// <param name="recordDetail"></param>
        /// <returns></returns>
        public VisionMessage GetTransactionsByKey(TransactionType transType, string keys, RecordDetail recordDetail = RecordDetail.Empty)
        {
            try
            {
                VisionMessage _retval = new VisionMessage();
                int _nextChunk = 1;
                bool _lastChunk = false;
                string _sessionId = "";
                string _connInfo = GetConnectionInfo();

                do
                {
                    string _xmlretval = service.GetTransactionByKey(
                        _connInfo,
                        transType.GetValueName(),
                        keys,
                        recordDetail.GetValueName()
                        );

                    //read loop moved into separate method...
                    _lastChunk = ReadResult(ref _sessionId, _xmlretval, ref _retval);
                    if (string.IsNullOrEmpty(_sessionId) == false) { _connInfo = GetVisionConnInfoXML(database, username, password, _sessionId); }
                    _nextChunk += 1;

                } while (_lastChunk == false);

                return _retval;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"GetTransactionsByKey() with keys [{keys}] call failed");
                return VisionMessage.FromException(ex);
            }
        }

        /// <summary>
        /// returns transaction contents by query
        /// </summary>
        /// <param name="transType"></param>
        /// <param name="query"></param>
        /// <param name="recordDetail"></param>
        /// <returns></returns>
        public VisionMessage GetTransactionsByQuery(TransactionType transType, string query, RecordDetail recordDetail = RecordDetail.Empty)
        {
            try
            {
                VisionMessage _retval = new VisionMessage();
                int _nextChunk = 1;
                bool _lastChunk = false;
                string _sessionId = "";
                string _connInfo = GetConnectionInfo();
                XElement _xQuery = new XElement("Queries", new XElement("Query", query, new XAttribute("ID", 1)));


                do
                {
                    string _xmlretval = service.GetTransactionByQuery(
                        _connInfo,
                        transType.GetValueName(),
                        _xQuery.ToString(),
                        recordDetail.GetValueName()
                        );

                    //read loop moved into separate method...
                    _lastChunk = ReadResult(ref _sessionId, _xmlretval, ref _retval);
                    if (string.IsNullOrEmpty(_sessionId) == false) { _connInfo = GetVisionConnInfoXML(database, username, password, _sessionId); }
                    _nextChunk += 1;

                } while (_lastChunk == false);

                return _retval;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"GetTransactionsByQuery() with query [{query}] call failed");
                return VisionMessage.FromException(ex);
            }
        }
        #endregion

        #region UDIC Methods
        /// <summary>
        /// retrieves record by UID
        /// </summary>
        /// <param name="udicName"></param>
        /// <param name="id"></param>
        /// <param name="recordDetail"></param>
        /// <returns></returns>
        public VisionMessage GetUDICRecordsByKey(string udicName, string id, RecordDetail recordDetail = RecordDetail.Empty)
        {
            try
            {
                VisionMessage _retval = new VisionMessage();
                int _nextChunk = 1;
                bool _lastChunk = false;
                string _sessionId = "";
                string _connInfo = GetConnectionInfo();

                do
                {
                    string _xmlretval = service.GetUDICByKey(
                        _connInfo,
                        udicName,
                        id,
                        recordDetail.GetValueName()
                        );

                    //read loop moved into separate method...
                    _lastChunk = ReadResult(ref _sessionId, _xmlretval, ref _retval);
                    if (string.IsNullOrEmpty(_sessionId) == false) { _connInfo = GetVisionConnInfoXML(database, username, password, _sessionId); }
                    _nextChunk += 1;

                } while (_lastChunk == false);

                return _retval;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"GetUDICRecordsByKey() for {udicName} with key [{id}] call failed");
                return VisionMessage.FromException(ex);
            }
        }


        public VisionMessage GetUDICRecordsByQuery(string udicName, string query, RecordDetail recordDetail = RecordDetail.Empty)
        {
            try
            {
                VisionMessage _retval = new VisionMessage();
                int _nextChunk = 1;
                bool _lastChunk = false;
                string _sessionId = "";
                string _connInfo = GetConnectionInfo();
                XElement _xQuery = new XElement("Queries", new XElement("Query", query, new XAttribute("ID", 1)));

                do
                {
                    string _xmlretval = service.GetUDICByQuery(
                        _connInfo,
                        udicName,
                        _xQuery.ToString(),
                        recordDetail.GetValueName()
                        );

                    //read loop moved into separate method...
                    _lastChunk = ReadResult(ref _sessionId, _xmlretval, ref _retval);
                    if (string.IsNullOrEmpty(_sessionId) == false) { _connInfo = GetVisionConnInfoXML(database, username, password, _sessionId); }
                    _nextChunk += 1;

                } while (_lastChunk == false);

                return _retval;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"GetUDICRecordsByQuery() for {udicName} with query [{query}] call failed");
                return VisionMessage.FromException(ex);
            }
        }

        /// <summary>
        /// adds data to a UDIC
        /// </summary>
        /// <param name="udicName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public VisionMessage AddUDIC(string udicName, XElement data)
        {
            try
            {
                string _strData = data.ToString().Replace("xmlns=\"\"", "");
                string _strMessage = service.AddUDIC(GetConnectionInfo(), udicName, _strData);
                return VisionMessage.FromXML(_strMessage);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"AddUDIC() failed");
                return VisionMessage.FromException(ex);
            }
        }

        /// <summary>
        /// update udic data
        /// </summary>
        /// <param name="udicName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public VisionMessage UpdateUDIC(string udicName, XElement data)
        {
            try
            {
                string _strData = data.ToString().Replace("xmlns=\"\"", "");
                string _strMessage = service.UpdateUDIC(GetConnectionInfo(), udicName, _strData);
                return VisionMessage.FromXML(_strMessage);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"UpdateUDIC() failed");
                return VisionMessage.FromException(ex);
            }
        }

        /// <summary>
        /// deletes records from udic
        /// </summary>
        /// <param name="udicName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public VisionMessage DeleteUDIC(string udicName, XElement data)
        {
            try
            {
                string _strData = data.ToString().Replace("xmlns=\"\"", "");
                string _strMessage = service.DeleteUDIC(GetConnectionInfo(), udicName, _strData);
                return VisionMessage.FromXML(_strMessage);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"DeleteUDIC() failed");
                return VisionMessage.FromException(ex);
            }
        }
        #endregion

        #region Picklist Methods
        /// <summary>
        /// retrtieves pick list values
        /// </summary>
        /// <param name="pickList"></param>
        /// <param name="hierarchical"></param>
        /// <returns></returns>
        public XDocument GetPicklist(PickList pickList, int hierarchical = 0)
        {
            try
            {
                var _xmlresult = service.GetPickList(GetConnectionInfo(), GetPickListRequestXML(pickList, hierarchical));
                var _retval = XDocument.Load(new System.IO.StringReader(_xmlresult));
                var _message = VisionMessage.FromXML(_xmlresult);
                if (string.IsNullOrEmpty(_message.ReturnCode) == false && _message.ReturnCode != "1")
                {
                    throw new ApplicationException(_message.ReturnDesc);
                }

                return _retval;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"GetPicklist() for [{pickList.GetValueName()}] failed");
                return new XDocument(new XElement("RECS"));
            }
        }
        #endregion

        #region Stored Procedures
        /// <summary>
        /// executes a stored procedure in Vision DB
        /// </summary>
        /// <param name="sprocName">stored procedures must be named "DeltekStoredProc_XYZ", only the XYZ portion must be provided!</param>
        /// <param name="parameters">a key/value pair of parameters</param>
        /// <returns></returns>
        public VisionMessage ExecuteStoredProcedure(string sprocName, Dictionary<string, object> parameters = null)
        {
            try
            {
                var _parameterXML = ToParameterXML(parameters);
                var _retval = service.ExecuteStoredProcedure(GetConnectionInfo(), sprocName, _parameterXML.ToString());

                return VisionMessage.FromXML(_retval);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"ExecuteStoredProcedure({sprocName}) failed");
                return VisionMessage.FromException(ex);
            }


        }




        #endregion
    }
}
