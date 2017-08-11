using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionAPIHelper
{
    /// <summary>
    /// a sample extension class for the record struct. This can be extended as needed
    /// or other Extensions classes can be implemented for specific use cases
    /// </summary>
    public static class RecordStructureExtensions
    {
        /// <summary>
        /// adds a predefined project table struct
        /// </summary>
        /// <param name="recs"></param>
        /// <param name="inserts"></param>
        /// <param name="updates"></param>
        /// <param name="deletes"></param>
        /// <returns></returns>
        public static TableStructure AddProjectTable(this RecordStructure recs, RowStrucure inserts = null, RowStrucure updates = null, RowStrucure deletes = null)
        {
            return recs.AddTable("PR", "PR", "PR", "WBS1,WBS2,WBS3", inserts, updates, deletes);
        }

        /// <summary>
        /// adds a predefined client table struct
        /// </summary>
        /// <param name="recs"></param>
        /// <param name="inserts"></param>
        /// <param name="updates"></param>
        /// <param name="deletes"></param>
        /// <returns></returns>
        public static TableStructure AddClientTable(this RecordStructure recs, RowStrucure inserts = null, RowStrucure updates = null, RowStrucure deletes = null)
        {
            return recs.AddTable("CL", "CL", "CL", "ClientId", inserts, updates, deletes);
        }

        /// <summary>
        /// adds a predefined employee table struct
        /// </summary>
        /// <param name="recs"></param>
        /// <param name="inserts"></param>
        /// <param name="updates"></param>
        /// <param name="deletes"></param>
        /// <returns></returns>
        public static TableStructure AddEmployeeTable(this RecordStructure recs, RowStrucure inserts = null, RowStrucure updates = null, RowStrucure deletes = null)
        {
            return recs.AddTable("EM", "EM", "EM", "Employee", inserts, updates, deletes);
        }

        /// <summary>
        /// adds a prededfined contacts table struct
        /// </summary>
        /// <param name="recs"></param>
        /// <param name="inserts"></param>
        /// <param name="updates"></param>
        /// <param name="deletes"></param>
        /// <returns></returns>
        public static TableStructure AddContactTable(this RecordStructure recs, RowStrucure inserts = null, RowStrucure updates = null, RowStrucure deletes = null)
        {
            return recs.AddTable("Contacts", "Contacts", "Contacts", "ContactId", inserts, updates, deletes);
        }
    }
}
