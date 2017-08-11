using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionAPIHelper
{
    public enum TransactionType
    {
        AP,
        CD,
        CR,
        CV,
        ER,
        EX,
        IN,
        JE,
        LA,
        MI,
        PR,
        TS,
        UN,
        UP
    }

    public static class TransactionTypeExtensions
    {
        public static string GetValueName(this TransactionType transType)
        {
            string _retval = Enum.GetName(typeof(TransactionType), transType);
            return _retval;
        }

    }
}
