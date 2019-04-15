using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discord_bot.SelectTable.Sql.MiniDatabase.Tables
{
    public class DefinedTable<T> : ITable<T> {

        public string Identifier { get; set; }
        public IList<T> Table { get; set; }

        public DefinedTable() {
            Table = new List<T>();
        }

        public IQueryable<T> FetchTable() {
            return Table.AsQueryable();
        }
    }
}
