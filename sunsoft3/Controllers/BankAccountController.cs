using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using sunsoft3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace sunsoft3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankAccountController : ControllerBase
    {
        private readonly ILogger<BankAccountController> _logger;

        public BankAccountController(ILogger<BankAccountController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public ResponseATM CreateNewAccount(BankAccount account1)
        {
            ResponseATM ResponseATM1 = new ResponseATM();
            if (account1.key.Trim() == "12345678910")
            {
                try
                {
                    MyGlobal myGlobal = new MyGlobal();
                    if (myGlobal.cnn.State == ConnectionState.Closed) { myGlobal.cnn.Open(); }
                    String sqlInsertAccount = "insert into [wf].Account(Balance,accountType,CreationDate,Currency,CustomerID,Password) values(" + account1.Balance + ", '" + account1.accountType + "', getdate(), '" + account1.Currency + "', " + account1.CustomerID + " , '" + account1.key + "')";
                    SqlCommand command = new SqlCommand(sqlInsertAccount, myGlobal.cnn);
                    int AccountID = Convert.ToInt32(command.ExecuteScalar());
                    ResponseATM1.Code = 1;
                    ResponseATM1.Description = "The Account was registered successfully";
                    ResponseATM1.Status = "success";
                    myGlobal.cnn.Close();
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
