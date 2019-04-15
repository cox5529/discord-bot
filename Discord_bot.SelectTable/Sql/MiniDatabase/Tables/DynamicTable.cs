using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discord_bot.SelectTable.Sql.MiniDatabase.Tables {
    public class DynamicTable<T> : ITable<T> {

        public string Identifier { get; set; }
        public Func<IQueryable<T>> FetchFunction { get; set; }

        public IQueryable<T> FetchTable() {
            return FetchFunction.Invoke();
        }
    }
}