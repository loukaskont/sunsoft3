using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sunsoft3.Models
{
    public class MyGlobal
    {
        public string Username = "sa";
        public string Password = "Ne@0d0sno2011!!";
        public string Catalog = "NeaOdos_Protocol";
        public string Data_Source = @"10.80.0.77";
        public SqlConnection cnn = new SqlConnection("Data Source=10.80.0.77;Initial Catalog=NeaOdos_Protocol;User ID=sa;Password=Ne@0d0sno2011!!; MultipleActiveResultSets=true; Encrypt=false");


        public bool ValidateAFM(String tin)
        {
            if (tin == "")
            {
                return true;
            }
            string afm = tin;
            int _numAFM = 0;
            if (afm.Length != 9 || !int.TryParse(afm, out _numAFM))
                return false;
            else
            {
                double sum = 0;
                int iter = afm.Length - 1;
                afm.ToCharArray().Take(iter).ToList().ForEach(c =>
                {
                    sum += double.Parse(c.ToString()) * Math.Pow(2, iter);
                    iter--;
                });
                if (sum % 11 == double.Parse(afm.Substring(8)) || double.Parse(afm.Substring(8)) == 0)
                    return true;
                else
                    return false;
            }
        }

    }
}
