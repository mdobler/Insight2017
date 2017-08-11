using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VisionAPIHelper
{
    public class VisionKey
    {
        public List<string> SubKeys { get; set; } = new List<string>();

        public VisionKey() {}

        public VisionKey(string key)
        {
            this.SubKeys.Add(key);
        }

        public VisionKey(params string[] subkeys)
        {
            this.SubKeys.AddRange(subkeys);
        }

        public override string ToString()
        {
            XElement _retval = new XElement("KeyValues", 
                new XElement("Keys", 
                    new XAttribute("ID", 1), 
                    new XElement("Key")
                )
            );

            foreach (string subkey in this.SubKeys)
            {
                _retval.Element("Keys").Element("Key").Add(new XElement("Fld", subkey));
            }

            return _retval.ToString();
        }

        public static VisionKey CreateGenericKey(string key)
        {
            return new VisionKey(key);
        }

        public static VisionKey CreateProjectKey(string wbs1, string wbs2 = "", string wbs3 = "")
        {
            return new VisionKey(wbs1, wbs2, wbs3 );
        }
    }

    public class VisionKeyList : List<VisionKey>
    {
        public VisionKeyList() {}

        public VisionKeyList(params VisionKey[] keys)
        {
            this.AddRange(keys);
        }

        public override string ToString()
        {
            XElement _retval = new XElement("KeyValues");
            int _counter = 1;
            foreach (var key in this)
            {
                XElement _key = new XElement("Keys",
                    new XAttribute("ID", _counter),
                    new XElement("Key")
                );

                foreach (string subkey in key.SubKeys)
                {
                    _key.Element("Key").Add(new XElement("Fld", subkey));
                }

                _retval.Add(_key);

                _counter += 1;
            }

            return _retval.ToString();
        }
    }
}
