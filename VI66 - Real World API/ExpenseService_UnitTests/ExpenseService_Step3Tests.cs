using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpenseService_Step3;

namespace ExpenseService_UnitTests
{
    [TestClass]
    public class ExpenseService_Step3Tests
    {
        [TestMethod]
        public void TestParseExpenseFile()
        {
            CronJob job = new CronJob();

            var result = job.parseExpenseFile("c:\\temp\\SampleImportData.expense");
            Assert.IsTrue(result.Count > 0);
            Assert.IsTrue(result[0].WBS1 == "1998001.00");
        }

        [TestMethod]
        public void TestPostExpense()
        {
            CronJob job = new CronJob();

            var records = job.parseExpenseFile("c:\\temp\\SampleImportData.expense");
            var result = job.postToVision(records);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Errors.Count == 0);
        }
    }
}
