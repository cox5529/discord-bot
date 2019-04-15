using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discord_bot.SelectTable.Sql.MiniDatabase.Tables {
    public interface ITable<out T> {
        string Identifier { get; set; }
        IQueryable<T> FetchTable();
    }
}