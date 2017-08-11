using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VisionAPIHelper
{
    public class VisionWorkflowMessage
    {
        private List<string> _messages = new List<string>();

        private bool _isError = false;
        public void AddWarning(string message)
        {
            _messages.Add(message);
        }

        public void AddError(string message)
        {
            _messages.Add(message);
            _isError = true;
        }

        public void AddError(string message, VisionMessage info)
        {
            System.Text.StringBuilder msg = new System.Text.StringBuilder();
            msg.AppendLine(message);
            msg.AppendFormat("Vision Message Error: Return Code = {0}, Description = {1}, Detail = {2}, Errors = {3}", info.ReturnCode, info.ReturnDesc, info.ReturnedData, info.GetErrors());
            _messages.Add(msg.ToString());
            _isError = true;
        }

        public VisionWorkflowMessage()
        {
            this.Clear();
        }

        public void Clear()
        {
            _messages = new List<string>();
            _isError = false;
        }

        public override string ToString()
        {
            XElement retval = new XElement("errors");

            if (!_isError)
            {
                retval.Add(new XAttribute("warning", "y"));
            }

            foreach (var message in _messages)
            {
                XElement xmlerr = new XElement("error", message);
                retval.Add(xmlerr);
            }

            return retval.ToString();
        }

        public bool HasErrors()
        {
            return _isError;
        }

        public string GetErrorText()
        {
            System.Text.StringBuilder retval = new System.Text.StringBuilder();

            if (_isError)
                retval.AppendLine("Message contains errors!");

            foreach (var msg in _messages)
            {
                retval.AppendLine(msg);
            }

            return retval.ToString();
        }

        public string GetErrorTextHtml()
        {
            System.Text.StringBuilder retval = new System.Text.StringBuilder();

            if (_isError)
                retval.AppendLine("<h3>Message contains errors!</h3>");

            foreach (var msg in _messages)
            {
                retval.AppendLine("<p>" + msg + "</p>");
            }

            return retval.ToString();
        }

        public static VisionWorkflowMessage FromException(Exception ex)
        {
            VisionWorkflowMessage retval = new VisionWorkflowMessage();

            Exception inner = ex;
            while (ex != null)
            {
                retval.AddError(ex.Message);
                inner = ex.InnerException;
            }

            return retval;
        }
    }

}
