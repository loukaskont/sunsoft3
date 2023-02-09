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
    public class WithdrawController : ControllerBase
    {
        private readonly ILogger<WithdrawController> _logger;

        public WithdrawController(ILogger<WithdrawController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public object NewBankWithdraw(BankWithdraw BankWithdraw1)
        {
            ResponseATM ResponseATM1 = new ResponseATM();
            if (BankWithdraw1.key.Trim() == "12345678910")
            {
                try
                {
                    MyGlobal myGlobal = new MyGlobal();
                    if (myGlobal.cnn.State == ConnectionState.Closed) { myGlobal.cnn.Open(); }
                    String selectReserve = "SELECT r.Fifty, r.Twenty, r.Ten from [wf].Reserve r where r.reserveID = 0";
                    int FiftyCount = 0; int TwentyCount = 0; int TenCount = 0;
                    int FiftyRet = 0; int TwentyRet = 0; int TenRet = 0;
                    double ReserveAmount = 0;
                    SqlCommand commandSelectReserve = new SqlCommand(selectReserve, myGlobal.cnn);
                    SqlDataReader readerSelectReserve = commandSelectReserve.ExecuteReader();
                    if (readerSelectReserve.Read())
                    {
                        FiftyCount = readerSelectReserve.GetInt32(0);
                        TwentyCount = readerSelectReserve.GetInt32(1);
                        TenCount = readerSelectReserve.GetInt32(2);
                    }
                    double tempAmount = BankWithdraw1.Amount;
                    if (BankWithdraw1.Amount >= 100)
                    {
                        while (FiftyCount > 0 && ReserveAmount < BankWithdraw1.Amount) 
                        {
                            ReserveAmount = ReserveAmount + 50;
                            FiftyRet++;
                            tempAmount = tempAmount - 50;
                            if (tempAmount < 50 || (FiftyCount- FiftyRet) <= 0) { break; }
                        }
                    }
                    if (tempAmount >= 20) 
                    {
                        while (TwentyCount > 0 && ReserveAmount < BankWithdraw1.Amount) 
                        {
                            ReserveAmount = ReserveAmount + 20;
                            TwentyRet++;
                            tempAmount = tempAmount - 20;
                            if (tempAmount < 20 || (TwentyCount - TwentyRet) <= 0) { break; }
                        }
                    }
                    if (tempAmount >= 10)
                    {
                        while (TenCount > 0 && ReserveAmount < BankWithdraw1.Amount)
                        {
                            ReserveAmount = ReserveAmount + 10;
                            TenRet++;
                            tempAmount = tempAmount - 10;
                            if (tempAmount < 10 || (TenCount - TenRet) <= 0) { break; }
                        }
                    }
                    if (ReserveAmount == BankWithdraw1.Amount)
                    {
                        ReservedATM ReservedATM1 = new ReservedATM();
                        ReservedATM1.Amount = BankWithdraw1.Amount;
                        ReservedATM1.DepositDateTime = DateTime.Now;
                        ReservedATM1.Fifty = FiftyRet;
                        ReservedATM1.Ten = TenRet;
                        ReservedATM1.Twenty = TwentyRet;
                        String updateReserve = "update [wf].Reserve set Fifty = "+ (FiftyCount - FiftyRet) + ", Twenty = " + (TwentyCount - TwentyRet) + ", Ten = " + (TenCount - TenRet) + " where reserveID = 0";
                        SqlCommand command2 = new SqlCommand(updateReserve, myGlobal.cnn);
                        command2.ExecuteScalar();
                        return ReservedATM1;
                    }
                    else 
                    {
                        ResponseATM1.Code = -1;
                        ResponseATM1.Description = "failure to execute a bank transaction.";
                        ResponseATM1.Status = "error";
                    }
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
