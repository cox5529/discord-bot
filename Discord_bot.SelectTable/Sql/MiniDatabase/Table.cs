using System.Collections.Generic;
using System.Linq;

namespace Discord_bot.SelectTable.Sql.MiniDatabase
{
    public class Table<T> {

        public string Identifier { get; set; }
        public IList<T> Data { get; set; }

        public Table() {
            Data = new List<T>();
        }

        public IQueryable<T> FetchTable() {
            return Data.AsQueryable();
        }
    }
}
