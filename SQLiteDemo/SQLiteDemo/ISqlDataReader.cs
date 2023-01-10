using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDemo
{
    interface ISqlDataReader 
    {
        Boolean Read();
        double GetDouble(int col);
    }
}
