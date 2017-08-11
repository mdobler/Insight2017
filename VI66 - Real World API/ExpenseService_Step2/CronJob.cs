using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionAPIHelper;

namespace ExpenseService_Step2
{
    public class CronJob
    {

        public CronJob()
        {
        }

        /// <summary>
        /// this method is called by the timer in the service and checks for files in a specific directory
        /// then it reads the files and processes them accordingly
        /// </summary>
        /// <param name="stateInfo"></param>
        public void Run(object stateInfo)
        {
            List<ExpenseRecord> records = new List<ExpenseRecord>();
            
            //looks for all files in the specified directory that end in .expense (custom file type for this excercise)
            foreach (var file in Directory.GetFiles(Properties.Settings.Default.ExportLocation, "*.expense"))
            {
                try
                {
                    //reads the text file into a specific object list
                    records = parseExpenseFile(file);
                    postToVision(records);
                }
                catch (Exception)
                {
                    //do something here
                }
            }
        }

        private List<ExpenseRecord> parseExpenseFile(string filename)
        {
            List<ExpenseRecord> records = new List<ExpenseRecord>();
            string[] fields;
            char[] delimiter = { '|' };

            //throw exception if file cannot be found
            if (System.IO.File.Exists(filename) == false )
            { throw new FileNotFoundException("Cannot find expense file", filename); }

            //parse the content of the text file
            using (StreamReader reader = new StreamReader(filename))
            {
                string line;
                int count = 0;

                while (reader.EndOfStream == false)
                {
                    count += 1;
                    line = reader.ReadLine();
                    //jump over first record -> contains header
                    if (count != 1)
                    {
                        fields = line.Split(delimiter);

                        var record = new ExpenseRecord();
                        record.Employee = fields[0];
                        record.TransDate = Convert.ToDateTime(fields[1]);
                        record.Description = fields[2];
                        record.WBS1 = fields[3];
                        record.WBS2 = fields[4];
                        record.WBS3 = fields[5];
                        record.Account = fields[6];
                        record.Amount = Convert.ToDecimal(fields[7]);
                        if (fields[8] == "X") { record.Billable = true; } else { record.Billable = false; }
                        record.Period = Convert.ToInt32(fields[9]);
                        record.Company = fields[10];
                        //add to list
                        records.Add(record);
                    }
                };
            }

            //return list
            return records;
        }

        private bool postToVision(List<ExpenseRecord> records)
        {
            //TODO: to be implemented in phase 3
            return true;
        }
    }
}
