using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionAPIHelper
{
    public enum RecordDetail
    {
        Empty,
        Primary,
        AllPrimary,
        All
    }

    public static class RecordDetailExtensions
    {
        public static string GetValueName(this RecordDetail recordDetail)
        {
            string _retval = Enum.GetName(typeof(RecordDetail), recordDetail);
            if (_retval == "Empty") { _retval = ""; }
            return _retval;
        }

    }
}
