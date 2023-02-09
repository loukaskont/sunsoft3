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
    public class BankCustomerController : ControllerBase
    {

        private readonly ILogger<BankCustomerController> _logger;

        public BankCustomerController(ILogger<BankCustomerController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public ResponseATM CreateNewCustomer(Customer Customer1)
        {
            ResponseATM ResponseATM1 = new ResponseATM();
            if (Customer1.key.Trim() == "12345678910")
            {
                try
                {
                    MyGlobal myGlobal = new MyGlobal();
                    bool afmValid = myGlobal.ValidateAFM(Customer1.CustomerVat);
                    if (!afmValid)
                    {
                        ResponseATM1.Code = -1;
                        ResponseATM1.Description = "the VAT number is incorrect";
                        ResponseATM1.Status = "error";
                    }
                    else
                    {
                        if (myGlobal.cnn.State == ConnectionState.Closed) { myGlobal.cnn.Open(); }
                        String selectCustomer = "SELECT a.CustomerID from [wf].Customer a where a.CustomerVat = '" + Customer1.CustomerVat + "'";
                        SqlCommand commandSelectCustomer = new SqlCommand(selectCustomer, myGlobal.cnn);
                        SqlDataReader readerSelectCustomer = commandSelectCustomer.ExecuteReader();
                        if (readerSelectCustomer.Read())
                        {
                            ResponseATM1.Code = -1;
                            ResponseATM1.Description = "The Customer Exist";
                            ResponseATM1.Status = "error";
                        }
                        else
                        {
                            String sqlInsertCustomer = "insert into [wf].Customer(CustomerName, CustomerSurname, CustomerVat, CustomerPhone) values('" + Customer1.CustomerName + "', '" + Customer1.CustomerSurname + "', '" + Customer1.CustomerVat + "', '" + Customer1.CustomerPhone + "')";
                            SqlCommand command = new SqlCommand(sqlInsertCustomer, myGlobal.cnn);
                            int CustomerID = Convert.ToInt32(command.ExecuteScalar());
                            ResponseATM1.Code = 1;
                            ResponseATM1.Description = "The Customer was registered successfully";
                            ResponseATM1.Status = "success";
                            myGlobal.cnn.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ResponseATM1.Code = -1;
                    ResponseATM1.Description = ex.Message;
                    ResponseATM1.Status = "error";
                }
            }
            return ResponseATM1;
        }

        /*[HttpGet]
        public Deposit GetAccountBalance()
        {
            MyGlobal myGlobal = new MyGlobal();
            String sql = "";
            if (myGlobal.cnn.State == ConnectionState.Closed) { myGlobal.cnn.Open(); }
            SqlCommand command = new SqlCommand(sql, myGlobal.cnn);

            return new Deposit
            {
                DepositDateTime = DateTime.Now.AddDays(1),
                Available = "170",
                Fifty = "1",
                Twenty = "3",
                Ten = "0"
            };
        }*/

    }
}
