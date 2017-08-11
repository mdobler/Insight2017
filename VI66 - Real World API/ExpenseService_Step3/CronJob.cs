using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using VisionAPIHelper;

namespace ExpenseService_Step3
{
    public class CronJob
    {
        private VisionAPIRepository repository;

        public CronJob()
        {
            //initialize the VisionAPIRepository class to execute commands to the Vision API
            //does not initialize for HTTPS - need additional settings...
            repository = new VisionAPIRepository(
                Properties.Settings.Default.VisionWSUrl,
                Properties.Settings.Default.VisionDatabase,
                Properties.Settings.Default.VisionUser,
                Properties.Settings.Default.VisionPassword);
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
                    records = parseExpenseFile(file);
                    postToVision(records);
                }
                catch (Exception)
                {
                    //do something here
                }
            }
        }

        public List<ExpenseRecord> parseExpenseFile(string filename)
        {
            List<ExpenseRecord> records = new List<ExpenseRecord>();
            string[] fields;
            char[] delimiter = { '|' };

            //throw exception if file cannot be found
            if (System.IO.File.Exists(filename) == false) { throw new FileNotFoundException("Cannot find expense file", filename); }

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

        public VisionMessage postToVision(List<ExpenseRecord> records)
        {
            string _lastEmployee = "";
            int _detailSeq = 0;
            int _masterSeq = 0
            int _period = 0;
            string _company = "";

            XElement recs = new XElement("RECS");
            List<XElement> detailRows = new List<XElement>();
            List<XElement> masterRows = new List<XElement>();
            List<XElement> controlRows = new List<XElement>();

            //creates a new posting batch id based on a guid
            string _batchId = $"EX_{DateTime.Now:yyyymmddhhMMss}";
            string _masterPKey = "";

            try
            {

                //goes through the records ordered by employee and transdate
                foreach (var record in records.OrderBy(x => x.Employee).ThenBy(x => x.TransDate))
                {
                    //if the employee changes we will start a new master record (report)
                    if (_lastEmployee != record.Employee)
                    {
                        _detailSeq = 0;
                        _masterSeq += 1;
                        _masterPKey = $"{_batchId}{_masterSeq:00}";

                        masterRows.Add(new XElement("ROW",
                                            new XAttribute("tranType", "INSERT"),
                                            new XElement("Batch", _batchId),
                                            new XElement("MasterPKey", _masterPKey),
                                            new XElement("Employee", record.Employee),
                                            new XElement("ReportDate", record.TransDate),   //we simply apply the first date as report date
                                            new XElement("ReportName", $"Expenses {record.Employee} for {record.TransDate.ToString()}"), //dummy report name
                                            new XElement("Posted", "N"),
                                            new XElement("Seq", _masterSeq),
                                            new XElement("AdvanceAmount", 0),
                                            new XElement("BarCode", ""),
                                            new XElement("DefaultCurrencyCode", " ")
                                        )
                                    );
                    }

                    _detailSeq += 1;

                    //assumes that all period and company values are the same in one file
                    _period = record.Period;
                    _company = record.Company;
                    //for non-multi company the value must be a [space] character
                    if (_company == "") { _company = " "; }

                    detailRows.Add(new XElement("ROW",
                                            new XAttribute("tranType", "INSERT"),
                                            new XElement("Batch", _batchId),
                                            new XElement("MasterPKey", _masterPKey),
                                            new XElement("PKey", GetGuid()),
                                            new XElement("Seq", _detailSeq),
                                            new XElement("TransDate", record.TransDate),
                                            new XElement("WBS1", record.WBS1),
                                            new XElement("WBS2", record.WBS2),
                                            new XElement("WBS3", record.WBS3),
                                            new XElement("Account", record.Account),
                                            new XElement("Amount", record.Amount.ToString("#.00")),
                                            new XElement("Description", record.Description),
                                            new XElement("SuppressBill", record.Billable == true ? "N" : "Y"),
                                            new XElement("NetAmount", record.Amount.ToString("#.00")),
                                            new XElement("PaymentExchangeRate", "1.00"),
                                            new XElement("PaymentAmount", record.Amount.ToString("#.00")),
                                            new XElement("PaymentExchangeInfo", 0),
                                            new XElement("CurrencyExchangeOverrideRate", 0),
                                            new XElement("CurrencyCode", " "),
                                            new XElement("OriginatingVendor", "")
                                        )
                                    );

                    _lastEmployee = record.Employee;
                }

                //add a control record
                controlRows.Add(new XElement("ROW",
                                            new XAttribute("tranType", "INSERT"),
                                            new XElement("Batch", _batchId),
                                            new XElement("Recurring", "N"),
                                            new XElement("Posted", "N"),
                                            new XElement("Creator", "DELTEKAPI"),
                                            new XElement("Period", _period),
                                            new XElement("EndDate", detailRows.Min(x => x.TryGetElementDateValue("TransDate").ToString("s"))),
                                            new XElement("Total", detailRows.Sum(x => x.TryGetElementDecimalValue("Amount"))),
                                            new XElement("AdvanceAmount", 0),
                                            new XElement("Company", _company),
                                            new XElement("DefaultCurrencyCode", " ")
                                        )
                                    );

                //create full posting message
                foreach (var ctrl in controlRows)
                {
                    XElement exControl = new XElement("exControl",
                                new XAttribute("name", "exControl"),
                                new XAttribute("alias", "exControl"),
                                new XAttribute("keys", "Batch"),
                                new XElement(ctrl)
                                );
                    XElement exMaster = new XElement("exMaster",
                                    new XAttribute("name", "exMaster"),
                                    new XAttribute("alias", "exMaster"),
                                    new XAttribute("keys", "Batch,Employee"),
                                    masterRows.Where(x => x.Element("Batch").Value == ctrl.Element("Batch").Value)
                                    );
                    XElement exDetail = new XElement("exDetail",
                                    new XAttribute("name", "exDetail"),
                                    new XAttribute("alias", "exDetail"),
                                    new XAttribute("keys", "Batch,MasterPKey,PKey"),
                                    detailRows.Where(x => x.Element("Batch").Value == ctrl.Element("Batch").Value)
                                    );

                    recs.Add(
                        new XElement("REC",
                            exControl,
                            exMaster,
                            exDetail
                            )
                    );
                }

                //check if there are any rows to submit
                VisionMessage retval = new VisionMessage();
                if (detailRows.Count > 0)
                {
                    recs.ApplyVisionDefaultNamespace();
                    retval = repository.AddTransaction(TransactionType.EX, recs);
                }
                else
                {
                    retval = new VisionMessage("0", "No Data To Process", "");
                }

                //auto posting
                if (retval.ReturnCode == "1")
                {
                    if (Properties.Settings.Default.AutoPost == true)
                    {
                        retval = repository.PostTransaction(
                            TransactionType.LA,
                            _batchId,
                            _period);
                    }
                }

                return retval;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception adding Expense posting", ex);
            }

        }

        private string GetGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
