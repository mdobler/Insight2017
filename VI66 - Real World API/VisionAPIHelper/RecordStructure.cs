using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace VisionAPIHelper
{
    /// <summary>
    /// the records structure class builds a REC/RECS/table/ROW structure
    /// and makes it easier to handle insert/update/delete scenarios in Vision API
    /// </summary>
    public class RecordStructure
    {
        public List<TableStructure> Tables { get; set; }

        public RecordStructure()
        {
            Tables = new List<TableStructure>();
        }

        #region helper methods
        /// <summary>
        /// allows for easy creation of a rec structure from within the recs structure
        /// </summary>
        /// <param name="table"></param>
        /// <param name="name"></param>
        /// <param name="alias"></param>
        /// <param name="keys"></param>
        /// <param name="inserts">a list of key value parameters for insert rows</param>
        /// <param name="updates">a list of key value parameters for update rows</param>
        /// <param name="deletes">a list of key value parameters for delete rows</param>
        /// <returns></returns>
        public TableStructure AddTable(string table, string name, string alias, string keys, 
            RowStrucure inserts = null, RowStrucure updates = null, RowStrucure deletes = null)
        {
            TableStructure tbl = new TableStructure() { Table = table, Name = name, Alias = alias, Keys = keys };
            if (inserts != null) { tbl.Inserts = inserts; }
            if (updates != null) { tbl.Updates = updates; }
            if (deletes != null) { tbl.Deletes = deletes; }
            Tables.Add(tbl);

            return tbl;
        }
        #endregion
    }

    /// <summary>
    /// the table structure defines the REC elements within the RECS element
    /// </summary>
    public class TableStructure
    {
        public string Table { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Keys { get; set; }

        public RowStrucure Inserts { get; set; }
        public RowStrucure Updates { get; set; }
        public RowStrucure Deletes { get; set; }

        //sub table structure for additional records attached to main record
        public List<TableStructure> ChildTables { get; set; }

        public TableStructure()
        {
            //sub table structure for additional records attached to main record
            ChildTables = new List<TableStructure>();

            //rows
            Inserts = new RowStrucure();
            Updates = new RowStrucure();
            Deletes = new RowStrucure();
        }

        /// <summary>
        /// allows for easy child table additions
        /// </summary>
        /// <param name="table"></param>
        /// <param name="name"></param>
        /// <param name="alias"></param>
        /// <param name="keys"></param>
        /// <param name="inserts"></param>
        /// <param name="updates"></param>
        /// <param name="deletes"></param>
        /// <returns></returns>
        public TableStructure AddChildTable(string table, string name, string alias, string keys,
            RowStrucure inserts = null, RowStrucure updates = null, RowStrucure deletes = null)
        {
            TableStructure tbl = new TableStructure() { Table = table, Name = name, Alias = alias, Keys = keys };
            if (inserts != null) { tbl.Inserts = inserts; }
            if (updates != null) { tbl.Updates = updates; }
            if (deletes != null) { tbl.Deletes = deletes; }
            ChildTables.Add(tbl);

            return tbl;
        }
    }

    /// <summary>
    /// the rows structure allows a key/pair list to be added that is then reformatted as 
    /// a ROW element
    /// </summary>
    public class RowStrucure : List<Dictionary<string, object>>
    {
        /// <summary>
        /// allows for easy row creation with key value pairs. 
        /// There must be an even number of parameters and the first item of a key value pair must
        /// be a string!
        /// </summary>
        /// <param name="keyvaluepairs"></param>
        public void AddRow(params object[] keyvaluepairs)
        {
            //check for pairs
            if (keyvaluepairs.Length % 2 != 0)
            {
                throw new ApplicationException("key/values must always be in pairs and must be an even number of parameters");
            }

            //check if the first item of each pair is always string
            Dictionary<string, object> dict = new Dictionary<string, object>();
            for (int i = 0; i < keyvaluepairs.Length; i += 2)
            {
                if ((keyvaluepairs[i] is string) == false)
                {
                    throw new ApplicationException("all keys must be of type string");
                }
                else
                {
                    dict.Add((string)keyvaluepairs[i], keyvaluepairs[i + 1]);
                }
            }

            this.Add(dict);
        }

        /// <summary>
        /// adds key/value pairs based on two arrays (names and values). arrays must be of same length
        /// </summary>
        /// <param name="columnNames"></param>
        /// <param name="values"></param>
        public void AddRow(string[] columnNames, object[] values)
        {
            //check for pairs
            if (columnNames.Length != values.Length)
            {
                throw new ApplicationException("column and value arrays must be of same length");
            }

            //check if the first item of each pair is always string
            Dictionary<string, object> dict = new Dictionary<string, object>();
            for (int i = 0; i < columnNames.Length; i++)
            {
                {
                    dict.Add((string)columnNames[i], values[i]);
                }
            }

            this.Add(dict);
        }

        /// <summary>
        /// adds a row of values based on comma separates string lists
        /// like "ColA,ColB,ColC,ColD,ColE,...", "ValA,ValB,ValC,ValD,..."
        /// </summary>
        /// <param name="columnsList"></param>
        /// <param name="valuesList"></param>
        public void AddRow(string columnsList, string valuesList)
        {
            string[] colArray = columnsList.Split(',');
            string[] valArray = valuesList.Split(',');
            AddRow(colArray, valArray);

        }

        /// <summary>
        /// adds a row based on a comma separated list of column names and a list of values
        /// </summary>
        /// <param name="columnsList"></param>
        /// <param name="values"></param>
        public void AddRow(string columnsList, params object[] values)
        {
            string[] colArray = columnsList.Split(',');
            AddRow(colArray, values);
        }

        /// <summary>
        /// allows access to a row
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Dictionary<string, object> Row(int index)
        {
            if (this.Count > index)
            { return null; }
            else
            { return this[index]; }
        }

        /// <summary>
        /// allows access to a column name
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public string ColumnName(int row, int column)
        {
            if (this.Count > row)
            { return null; }
            else
            {
                if (this[row].Count > column)
                { return null; }
                else
                { return this[row].Keys.ElementAt(column); }
            }
        }

        /// <summary>
        /// allows access to a column value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public object ColumnValue(int row, int column)
        {
            if (this.Count > row)
            { return null; }
            else
            {
                if (this[row].Count > column)
                { return null; }
                else
                { return this[row].Values.ElementAt(column); }
            }
        }

        /// <summary>
        /// allows access to a column value by column name
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public object ColumnValue(int row, string columnName)
        {
            if (this.Count > row)
            { return null; }
            else
            {
                if (this[row].ContainsKey(columnName) == false)
                { return null; }
                else
                {
                    return  this.ElementAt(row)[columnName];
                }
            }
        }
    }
}
