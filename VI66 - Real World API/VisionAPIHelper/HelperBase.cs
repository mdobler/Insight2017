using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;

namespace VisionAPIHelper
{
    public class HelperBase
    {
        /// <summary>
        /// returns a connection XML string based on a session id
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static string GetVisionConnInfoXML(string sessionId)
        {
            XElement _retval = new XElement("VisionConnInfo", new XElement("SessionID", sessionId));
            return _retval.ToString();
        }

        /// <summary>
        /// returns a connection XML string based on a database and a session id
        /// </summary>
        /// <param name="database"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static string GetVisionConnInfoXML(string database, string sessionId)
        {
            XElement _retval = new XElement("VisionConnInfo",
                new XElement("databaseDescription", database),
                new XElement("SessionID", sessionId)
                );
            return _retval.ToString();
        }

        /// <summary>
        /// returns a connection XML string based on a database, username and password
        /// </summary>
        /// <param name="database"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string GetVisionConnInfoXML(string database, string username, string password)
        {
            XElement _retval = new XElement("VisionConnInfo",
                new XElement("databaseDescription", database),
                new XElement("userName", username),
                new XElement("userPassword", password)
                );
            return _retval.ToString();
        }

        /// <summary>
        /// returns a connection XML string based on a database, username, password and session id
        /// </summary>
        /// <param name="database"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static string GetVisionConnInfoXML(string database, string username, string password, string sessionId)
        {
            XElement _retval = new XElement("VisionConnInfo",
                new XElement("databaseDescription", database),
                new XElement("userName", username),
                new XElement("userPassword", password),
                new XElement("SessionID", sessionId)

                );
            return _retval.ToString();
        }

        /// <summary>
        /// returns an info center XML string
        /// </summary>
        /// <param name="infoCenter"></param>
        /// <param name="rowAccess"></param>
        /// <param name="nextChunk"></param>
        /// <param name="chunkSize"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public static string GetInfoCenterXML(string infoCenter, 
            bool rowAccess = false, 
            int nextChunk = 0,
            int chunkSize = 100,
            XElement tableInfo = null)
        {
            XElement _retval = new XElement("InfoCenters",
                    new XElement("InfoCenter",
                        new XAttribute("ID", 1),
                        new XAttribute("Name", infoCenter),
                        new XAttribute("RowAccess", (rowAccess ? "1" : "0")),
                        new XAttribute("PartialAccess", 1)
                        )
                    );

            if (nextChunk > 0)
            {
                _retval.Element("InfoCenter").Add(
                    new XAttribute("Chunk", nextChunk),
                    new XAttribute("ChunkSize", chunkSize)
                    );
            }

            if (tableInfo != null)
            {
                _retval.Element("InfoCenter").Add(tableInfo);
            }

            return _retval.ToString();
        }

        /// <summary>
        /// returns an info center XML string
        /// </summary>
        /// <param name="infoCenter"></param>
        /// <param name="rowAccess"></param>
        /// <param name="nextChunk"></param>
        /// <param name="chunkSize"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public static string GetInfoCenterXML(VisionInfoCenters infoCenter,
            bool rowAccess = false,
            int nextChunk = 0,
            int chunkSize = 100,
            XElement tableInfo = null)
        {
            return GetInfoCenterXML(infoCenter.GetValueName(),
                rowAccess,
                nextChunk,
                chunkSize,
                tableInfo);
        }

        /// <summary>
        /// returns a picklist xml string
        /// </summary>
        /// <param name="picklist"></param>
        /// <param name="hierarchical"></param>
        /// <returns></returns>
        public static string GetPickListRequestXML(string picklist, int hierarchical = 0)
        {
            XElement _retval = new XElement("PickListRequest",
                new XElement("PickList",
                    new XAttribute("Type", picklist),
                    new XAttribute("Hierarchical", hierarchical)
                    )
                );

            return _retval.ToString();
        }

        /// <summary>
        /// returns a picklist xml string
        /// </summary>
        /// <param name="picklist"></param>
        /// <param name="hierarchical"></param>
        /// <returns></returns>
        public static string GetPickListRequestXML(PickList picklist, int hierarchical = 0)
        {
            return GetPickListRequestXML(picklist.GetValueName(), hierarchical);
        }

        /// <summary>
        /// returns a keys xml string
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string GetKeysXML(params string[] keys)
        {
            XElement _retval = new XElement("KeyValues");
            int _counter = 1;
            foreach (var key in keys)
            {
                _retval.Add(new XElement("Keys",
                    new XAttribute("ID", _counter),
                    new XElement("Key",
                        new XElement("Fld", key))
                    )
                );
                _counter += 1;
            }

            return _retval.ToString();
        }

        /// <summary>
        /// returns a keys xml string
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetKeysXML(VisionKey key)
        {
            return key.ToString();
        }

        /// <summary>
        /// returns a keys xml string
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string GetKeysXML(VisionKeyList keys)
        {
            return keys.ToString();
        }

        /// <summary>
        /// returns the query xml for a single query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string GetQueriesXML(string query)
        {
            var xdoc = new XElement("Queries", new XElement("Query", query, new XAttribute("ID", 1)));

            return xdoc.ToString();

        }

        /// <summary>
        /// returns the query xml for multiple
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string GetQueriesXML(string[] queries)
        {
            int count = 0;
            var xdoc = new XElement("Queries");

            foreach (string query in queries)
            {
                count++;
                xdoc.Add(new XElement("Query", query, new XAttribute("ID", count)));
            }

            return queries.ToString();

        }

        /// <summary>
        /// adds the correct namespace to all elements
        /// </summary>
        /// <param name="data"></param>
        /// <param name="xns"></param>
        /// <returns></returns>
        public static XElement AddNamespace(XElement data, string xns = "http://deltek.vision.com/XMLSchema")
        {
            XDocument _xdoc = new XDocument(data);
            XNamespace _xn = xns;

            foreach (XElement e in _xdoc.Descendants())
            {
                e.Name = _xn + e.Name.LocalName;
            }

            return _xdoc.Root;
        }

        /// <summary>
        /// adds the correct namespace to all elements
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="xns"></param>
        /// <returns></returns>
        public static XDocument AddNamespace(XDocument doc, string xns = "http://deltek.vision.com/XMLSchema")
        {
            XNamespace _xn = xns;

            foreach (XElement e in doc.Descendants())
            {
                e.Name = _xn + e.Name.LocalName;
            }

            return doc;
        }

        #region XML Record Helpers
        public static XElement GetXMLRecordStructure(RecordStructure recs)
        {
            XElement _retval = new XElement("RECS");

            foreach (var rec in recs.Tables)
            {
                _retval.Add(new XElement("REC", GetTableRec(rec)));
            }

            return _retval;
        }

        private static XElement[] GetTableRec(TableStructure table)
        {
            List<XElement> _retval = new List<XElement>();

            //process main table
            XElement _xmltable = new XElement(table.Table,
                            new XAttribute("name", table.Name),
                            new XAttribute("alias", table.Alias),
                            new XAttribute("keys", table.Keys)
                            );

            //process all rows
            List<XElement> _rows = new List<XElement>();
            foreach (var row in table.Inserts)
            {
                _rows.Add(GetRowRec(row, "INSERT"));
            }
            foreach (var row in table.Updates)
            {
                _rows.Add(GetRowRec(row, "UPDATE"));
            }
            foreach (var row in table.Deletes)
            {
                _rows.Add(GetRowRec(row, "DELETE"));
            }
            _xmltable.Add(_rows);

            //add to list
            _retval.Add(_xmltable);

            //process all child tables
            foreach (var subtable in table.ChildTables) 
            {
                //add to list
                _retval.AddRange(GetTableRec(subtable));
            }

            return _retval.ToArray();
        }

        private static XElement GetRowRec(Dictionary<string, object> row, string tranType)
        {
            XElement _retval = new XElement("ROW", new XAttribute("tranType", tranType));
            foreach (var item in row)
            {
                _retval.Add(new XElement(item.Key, item.Value));
            }

            return _retval;
        }
        #endregion

        /// <summary>
        /// creates a parameter list for API
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected static XDocument ToParameterXML(Dictionary<string, object> parameters)
        {
            XDocument _retval = new XDocument(new XElement("data"));
            if (parameters == null) { return _retval; }

            foreach (var item in parameters)
            {
                var xItem = new XElement("param", new XAttribute("name", item.Key));

                //if (item.Value is string)
                //{
                //    xItem.Value = (string)item.Value;
                //}
                //else if (item.Value is DateTime)
                //{
                //    xItem.Value = XmlConvert.ToString(Convert.ToDateTime(item.Value), XmlDateTimeSerializationMode.Utc);
                //    //trim time vlaue - anything after T
                //    //If xItem.Value.Contains("T") Then
                //    //    xItem.Value = xItem.Value.Substring(0, xItem.Value.IndexOf("T"))
                //    //End If
                //}
                //else if (item.Value is decimal)
                //{
                //    xItem.Value = XmlConvert.ToString(Convert.ToDecimal(item.Value));
                //}
                //else if (item.Value is double)
                //{
                //    xItem.Value = XmlConvert.ToString(Convert.ToDouble(item.Value));
                //}
                //else if (item.Value is int)
                //{
                //    xItem.Value = XmlConvert.ToString(Convert.ToInt32(item.Value));
                //}
                //else if (item.Value is bool)
                //{
                //    xItem.Value = XmlConvert.ToString(Convert.ToBoolean(item.Value));
                //}
                //else
                //{
                //    xItem.Value = item.Value.ToString();
                //}

                xItem.Value = ConvertToXMLValue(item.Value);

                _retval.Element("data").Add(xItem);

            }

            return _retval;
        }

        protected static string ConvertToXMLValue(object value)
        {
            string retval = "";

            if (value is string)
            {
                retval = (string)value;
            }
            else if (value is DateTime)
            {
                retval = XmlConvert.ToString(Convert.ToDateTime(value), XmlDateTimeSerializationMode.Utc);
                //trim time vlaue - anything after T
                //If retval.Contains("T") Then
                //    retval = retval.Substring(0, retval.IndexOf("T"))
                //End If
            }
            else if (value is decimal)
            {
                retval = XmlConvert.ToString(Convert.ToDecimal(value));
            }
            else if (value is double)
            {
                retval = XmlConvert.ToString(Convert.ToDouble(value));
            }
            else if (value is int)
            {
                retval = XmlConvert.ToString(Convert.ToInt32(value));
            }
            else if (value is bool)
            {
                retval = XmlConvert.ToString(Convert.ToBoolean(value));
            }
            else
            {
                retval = value.ToString();
            }

            return retval;
        }

        public static string GetParametersXML(Dictionary<string, object> parameters)
        {
            return ToParameterXML(parameters).ToString();
        }

        public static string GetParametersXML(Dictionary<string, string> parameters)
        {
            Dictionary<string, object> oparam = new Dictionary<string, object>();
            foreach (var item in parameters)
            {
                oparam.Add(item.Key, item.Value);
            }

            return ToParameterXML(oparam).ToString();
        }
    }
}
