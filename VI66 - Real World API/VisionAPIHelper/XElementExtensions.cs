using System;
using System.Xml.Linq;


namespace VisionAPIHelper
{
    /// <summary>
    /// provides extension methods to the XElement class
    /// </summary>
    public static class XElementExtensions
    {
        /// <summary>
        /// returns empty if item is null
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string EmptyIfNull(this string item)
        {
            if (string.IsNullOrEmpty(item)) { return ""; } else { return item; }
        }

        /// <summary>
        /// checks if a certain element contains a child element by name
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static bool ElementExists(this XElement parentElement, string elementName)
        {
            try
            {
                var foundElement = parentElement.Element(elementName);
                if (foundElement != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// checks if a document contains a certain element
        /// </summary>
        /// <param name="parentDocument"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static bool ElementExists(this XDocument parentDocument, string elementName)
        {
            try
            {
                var foundElement = parentDocument.Element(elementName);
                if (foundElement != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// tries to return an element value or a default value if not exists
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="elementName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string TryGetElementValue(this XElement parentElement, string elementName, string defaultValue = null, int maxLength = 0)
        {
            var foundElement = parentElement.Element(elementName);
            if (foundElement != null)
            {
                if (maxLength > 0 && foundElement.Value.Length > maxLength)
                {
                    return foundElement.Value.Substring(0, maxLength);
                }
                else
                {
                    return foundElement.Value;
                }

            }

            return defaultValue;
        }

        /// <summary>
        /// returns the content of the element (including all XML tags. I need this for the error message return...)
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="elementName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string TryGetElementContent(this XElement parentElement, string elementName, string defaultValue = null)
        {
            var foundElement = parentElement.Element(elementName);
            if (foundElement != null)
            {
                return foundElement.ToString();
            }

            return defaultValue;
        }

        /// <summary>
        /// returns data as int
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="elementName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int TryGetElementIntValue(this XElement parentElement, string elementName, int defaultValue = 0)
        {
            string _strVal = TryGetElementValue(parentElement, elementName);
            if (_strVal != null)
            {
                int _val;
                if (int.TryParse(_strVal, out _val))
                {
                    return _val;
                }
                else
                {
                    return defaultValue;
                }

            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// returns data as double
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="elementName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal TryGetElementDecimalValue(this XElement parentElement, string elementName, decimal defaultValue = 0)
        {
            string _strVal = TryGetElementValue(parentElement, elementName);
            if (_strVal != null)
            {
                decimal _val;
                if (decimal.TryParse(_strVal, out _val))
                {
                    return decimal.Round(_val, 4);
                }
                else
                {
                    return decimal.Round(defaultValue, 4);
                }

            }
            else
            {
                return decimal.Round(defaultValue, 4);
            }
        }


        /// <summary>
        /// returns data as double
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="elementName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double TryGetElementDoubleValue(this XElement parentElement, string elementName, double defaultValue = 0)
        {
            string _strVal = TryGetElementValue(parentElement, elementName);
            if (_strVal != null)
            {
                double _val;
                if (double.TryParse(_strVal, out _val))
                {
                    return _val;
                }
                else
                {
                    return defaultValue;
                }

            }
            else
            {
                return defaultValue;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static DateTime TryGetElementDateValue(this XElement parentElement, string elementName)
        {
            var foundElement = parentElement.Element(elementName);
            if (foundElement != null && string.IsNullOrEmpty(foundElement.Value) == false)
            {
                //return DateTime.Parse(foundElement.Value);
                return ToUtcDateOnly(foundElement.Value);
            }

            return DateTime.MinValue;

        }

        public static DateTime TryGetElementDateValue(this XElement parentElement, string elementName, DateTime defaultValue)
        {
            var foundElement = parentElement.Element(elementName);
            if (foundElement != null && string.IsNullOrEmpty(foundElement.Value) == false)
            {
                //return DateTime.Parse(foundElement.Value);
                return ToUtcDateOnly(foundElement.Value, defaultValue);
            }

            return defaultValue;
        }

        /// <summary>
        /// returns "Y" or "N" (Vision bool values) for true/false, 0/1, Y/N value pairs. if unknown then N
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="elementName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string TryGetElementYesNoValue(this XElement parentElement, string elementName, string defaultValue = "N")
        {
            string _strVal = TryGetElementValue(parentElement, elementName);
            if (_strVal != null)
            {
                if (_strVal.ToLower() == "true" || _strVal == "1" || _strVal.ToLower() == "y")
                {
                    return "Y";
                }
                else
                {
                    return "N";
                }

            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="xmlNamespace"></param>
        /// <param name="namespaceId"></param>
        /// <returns></returns>
        public static XElement ApplyNamespace(this XElement parentElement, string xmlNamespace = "", string namespaceId = "")
        {
            XNamespace _xn = xmlNamespace;
            if (!string.IsNullOrEmpty(namespaceId))
            {
                parentElement.Add(new XAttribute(XNamespace.Xmlns + namespaceId, _xn.NamespaceName));
            }

            var _xdoc = new XDocument(parentElement);

            foreach (XElement el in _xdoc.Descendants())
            {
                el.Name = _xn + el.Name.LocalName;
            }

            return _xdoc.Root;
        }

        public static XElement ApplyVisionDefaultNamespace(this XElement parentElement)
        {
            return ApplyNamespace(parentElement, "http://deltek.vision.com/XMLSchema", "");
        }

        /// <summary>
        /// returns a UTD date value (no time component)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToUtcDateOnly(string value)
        {
            try
            {
                //strip time component from date string value
                if (value.Contains("T"))
                {
                    value = value.Substring(0, value.IndexOf('T'));
                }

                DateTime utcDateTime = DateTime.SpecifyKind(DateTime.Parse(value), DateTimeKind.Utc);
                return utcDateTime;
            }
            catch (Exception)
            {
                //throw;
                return DateTime.MinValue;
            }

        }

        /// <summary>
        /// returns a UTD date value (no time component)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime ToUtcDateOnly(string value, DateTime defaultValue)
        {
            try
            {
                //strip time component from date string value
                if (value.Contains("T"))
                {
                    value = value.Substring(0, value.IndexOf('T'));
                }

                DateTime utcDateTime = DateTime.SpecifyKind(DateTime.Parse(value), DateTimeKind.Utc);
                return utcDateTime;
            }
            catch (Exception)
            {
                //throw;
                return defaultValue;
            }

        }
    }
}
