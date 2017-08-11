using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VisionAPIHelper
{
    public class VisionMessage
    {
        public string ReturnCode { get; set; } 
        public string ReturnDesc { get; set; } 
        public string Detail { get; set; } 
        public XDocument ReturnedData { get; set; }
        public Dictionary<string, string> Errors { get; set; }

        public VisionMessage()
        {
            Errors = new Dictionary<string, string>();
            ReturnCode = "";
            ReturnDesc = "";
            Detail = "";
            ReturnedData = new XDocument();
        }

        public VisionMessage(string code, string desc, string detail) : this()
        {
            ReturnCode = code;
            ReturnDesc = desc;
            Detail = detail;
        }

        public override string ToString()
        {
            XElement _retval = new XElement("DLTKVisionMessage",
                new XElement("ReturnCode", ReturnCode),
                new XElement("ReturnDesc", ReturnDesc),
                new XElement("Detail", Detail));

            return _retval.ToString();
        }

        /// <summary>
        /// appends a message to an existing message if the return code of the added message is not "1" success
        /// </summary>
        /// <param name="message"></param>
        public void AppendToMessage(VisionMessage message)
        {
            if (string.IsNullOrEmpty(ReturnCode))
            {
                ReturnCode = message.ReturnCode;
                ReturnDesc = message.ReturnDesc;
                Detail = message.Detail;
                ReturnedData = message.ReturnedData;
                Errors = message.Errors;
            }
            else
            {
                if (message.ReturnCode != "1")
                {
                    ReturnCode = message.ReturnCode;
                    ReturnDesc = $"{ReturnDesc}, {message.ReturnDesc}";
                    Detail = $"{Detail}{Environment.NewLine}{message.Detail}";
                    ReturnedData.Add(message.ReturnedData.Root);
                    foreach (var itm in message.Errors)
                    {
                        Errors.Add(itm.Key, itm.Value);
                    }
                }
            }
        }

        /// <summary>
        /// returns all errors as text
        /// </summary>
        /// <returns></returns>
        public string GetErrors()
        {
            StringBuilder _retval = new StringBuilder();
            int _count = 0;

            foreach (var item in Errors)
            {
                _count += 1;
                _retval.AppendLine($"Error {_count}: {item.Key}");
                _retval.AppendLine(item.Value);
                _retval.AppendLine();
            }

            return _retval.ToString();
        }

        /// <summary>
        /// produces a VisionMessage from  XML string
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static VisionMessage FromXML(string xml)
        {
            XDocument _doc;
            VisionMessage _retval = new VisionMessage();

            try
            {
                _doc = XDocument.Load(new System.IO.StringReader(xml));
                return FromXML(_doc);
            }
            catch (Exception)
            {
                return _retval;
            }
        }

        /// <summary>
        /// produces a VisionMessage from xml document
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public static VisionMessage FromXML(XDocument xmlDoc)
        {
            VisionMessage _retval = new VisionMessage();
            
            if (xmlDoc.Element("DLTKVisionMessage") != null)
            {
                _retval = new VisionMessage(
                    xmlDoc.Element("DLTKVisionMessage").TryGetElementValue("ReturnCode", ""),
                    xmlDoc.Element("DLTKVisionMessage").TryGetElementValue("ReturnDesc", ""),
                    xmlDoc.Element("DLTKVisionMessage").TryGetElementValue("Detail", "")
                    );

                //MD-2017-01-17: change in returned message. Can now have multiple <ReturnSelect> elements
                //and even returns code 1 if there are errors!!!
                if (xmlDoc.Element("DLTKVisionMessage").ElementExists("ReturnSelect"))
                {
                    _retval.ReturnedData = new XDocument();

                    foreach (var item in xmlDoc.Element("DLTKVisionMessage").Elements("ReturnSelect"))
                    {
                        if (item.ElementExists("MULTIRECS"))
                        {
                            _retval.ReturnedData.Add(item.Element("MULTIRECS").Elements("RECS"));
                        }
                        else if (item.ElementExists("RECS"))
                        {
                            _retval.ReturnedData.Add(item.Elements("RECS"));
                        }
                    }

                    //check if the returned data xml contains error items
                    if (_retval.ReturnedData.ElementExists("RECS"))
                    {
                        foreach (var item in _retval.ReturnedData.Elements("RECS"))
                        {
                            if (item.ElementExists("Error"))
                            {
                                foreach (var err in item.Elements("Error"))
                                {
                                    _retval.Errors.Add(err.TryGetElementValue("ErrorCode"),
                                        err.TryGetElementValue("Message"));
                                }
                            }
                        }
                    }

                    if (_retval.Errors.Count > 0)
                    {
                        _retval.ReturnCode = "-1";
                        _retval.ReturnDesc = _retval.ReturnedData.ToString();
                    }
                }
            }

            //check if it's a value returned from executestoredproc
            if (xmlDoc.Root.Name == "NewDataSet")
            {
                _retval.ReturnCode = "1";
                _retval.ReturnDesc = "stored procedure returned data";
                _retval.ReturnedData = new XDocument(xmlDoc);
            }

            return _retval;
        }


        public static VisionMessage FromException(Exception ex)
        {
            VisionMessage _retval = new VisionMessage();

            _retval.ReturnCode = "-1";
            _retval.ReturnDesc = ex.Message;

            Exception inner = ex;
            while (inner != null)
            {
                _retval.Detail = $"{_retval.Detail}{Environment.NewLine}{inner.Message}";
                inner = ex.InnerException;
            }

            return _retval;
        }
    }

    
}
