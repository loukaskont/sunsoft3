using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using sunsoft3.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace sunsoft3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionHistoryController : ControllerBase
    {
        private readonly ILogger<TransactionHistoryController> _logger;
        public TransactionHistoryController(ILogger<TransactionHistoryController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public object CreateNewTransactionHistory(TransactionHistory TransactionHistory1)
        {
            ResponseATM ResponseATM1 = new ResponseATM();
            int amountTemp = TransactionHistory1.Ten * 10 + TransactionHistory1.Fifty * 50 + TransactionHistory1.Twenty * 20;
            if (amountTemp != TransactionHistory1.Amount) 
            {
                ResponseATM1.Code = -1;
                ResponseATM1.Description = "The amount is not correct";
                ResponseATM1.Status = "error";
                return ResponseATM1;
            }
            if (TransactionHistory1.key.Trim() == "12345678910")
            {
                try
                {
                    MyGlobal myGlobal = new MyGlobal();
                    if (myGlobal.cnn.State == ConnectionState.Closed) { myGlobal.cnn.Open(); }
                    String TransactionDateTime = "getdate()";

                    String insertTran = "insert into [wf].[Transaction](amount, TransactionDate, AccountID) values("+TransactionHistory1.Amount + ", getdate(), " + TransactionHistory1.AccountID + "); SELECT SCOPE_IDENTITY()";
                    SqlCommand command3 = new SqlCommand(insertTran, myGlobal.cnn);
                    int transactionID = Convert.ToInt32(command3.ExecuteScalar());

                    int Fifty = TransactionHistory1.Fifty, Twenty = 0, Ten = 0, Available = 0;
                    Available = 50 * Fifty;
                    if (Available > 0)
                    {
                        String sqlInsertAccount = "INSERT into [wf].TransactionHistory(TransactionDateTime, Available, Fifty, Twenty, Ten,TransactionID) values(" + TransactionDateTime + ", " + Available + ", " + Fifty + ", " + Twenty + ", " + Ten + ", "+ transactionID + ")";
                        SqlCommand command = new SqlCommand(sqlInsertAccount, myGlobal.cnn);
                        command.ExecuteScalar();
                    }
                    Fifty = 0; Twenty = TransactionHistory1.Twenty; Ten = 0; Available = 0;
                    Available = 20 * Twenty;
                    if (Available > 0)
                    {
                        String sqlInsertAccount1 = "INSERT into [wf].TransactionHistory(TransactionDateTime, Available, Fifty, Twenty, Ten,TransactionID) values(" + TransactionDateTime + ", " + Available + ", " + Fifty + ", " + Twenty + ", " + Ten + ", " + transactionID + ")";
                        SqlCommand command1 = new SqlCommand(sqlInsertAccount1, myGlobal.cnn);
                        command1.ExecuteScalar();
                    }
                    Fifty = 0; Twenty = 0; Ten = TransactionHistory1.Ten; Available = 0;
                    Available = 10 * Ten;
                    if (Available > 0)
                    {
                        String sqlInsertAccount2 = "INSERT into [wf].TransactionHistory(TransactionDateTime, Available, Fifty, Twenty, Ten,TransactionID) values(" + TransactionDateTime + ", " + Available + ", " + Fifty + ", " + Twenty + ", " + Ten + ", " + transactionID + ")";
                        SqlCommand command2 = new SqlCommand(sqlInsertAccount2, myGlobal.cnn);
                        command2.ExecuteScalar();
                    }

                    String BalanceSql = "Update [wf].Account set Balance = (Balance + "+ TransactionHistory1.Amount + ") where accountID = " + TransactionHistory1.AccountID + "; select Balance from [wf].Account where accountID = " + TransactionHistory1.AccountID;
                    SqlCommand commandBalance = new SqlCommand(BalanceSql, myGlobal.cnn);
                    double Balance = Convert.ToDouble(commandBalance.ExecuteScalar());

                    Deposit returnDeposit = new Deposit();
                    returnDeposit.DepositDateTime = DateTime.Now;
                    returnDeposit.Available = Convert.ToString(Balance);
                    returnDeposit.Fifty = Convert.ToString(TransactionHistory1.Fifty);
                    returnDeposit.Twenty = Convert.ToString(TransactionHistory1.Twenty);
                    returnDeposit.Ten = Convert.ToString(TransactionHistory1.Ten);
                    myGlobal.cnn.Close();
                    return returnDeposit;
                }
                catch (Exception ex)
                {
                    ResponseATM1.Code = -1;
                    ResponseATM1.Description = ex.Message;
                    ResponseATM1.Status = "error";
                }
            }
            else
            {
                ResponseATM1.Code = -1;
                ResponseATM1.Description = "the key is not correct";
                ResponseATM1.Status = "error";
            }
            return ResponseATM1;
        }

    }
}
