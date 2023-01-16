using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDemo
{
    public class DataReaderWrapper : ISqlDataReader
    {
        private readonly SqlDataReader sqlDataReader;


        public DataReaderWrapper(SqlDataReader sqlDataReader)
        {
            this.sqlDataReader = sqlDataReader;
        }

        public double GetDouble(int col)
        {
            return this.sqlDataReader.GetDouble(col);
            throw new NotImplementedException();
        }

        public bool Read()
        {
            return this.sqlDataReader.Read();
            throw new NotImplementedException();
        }
    }
}
