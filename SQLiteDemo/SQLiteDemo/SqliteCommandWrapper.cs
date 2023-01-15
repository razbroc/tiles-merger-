using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDemo
{
    internal class SqliteCommandWrapper : ISqliteCommand
    {
        private readonly SQLiteCommand sqliteCommand;

        public SqliteCommandWrapper(SQLiteCommand sqliteCommand)
        {
            this.sqliteCommand = sqliteCommand;
        }

        public SQLiteDataReader ExecuteReader()
        {
            return this.sqliteCommand.ExecuteReader();
            throw new NotImplementedException();
        }
    }
}
