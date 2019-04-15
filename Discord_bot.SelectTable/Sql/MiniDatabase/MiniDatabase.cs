using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord_bot.SelectTable.Sql.MiniDatabase.Tables;

namespace Discord_bot.SelectTable.Sql.MiniDatabase {
    public class MiniDatabase<T> {
        public IList<ITable<T>> Tables { get; set; }
        public Func<string, Task<IQueryable<T>>> MissingFunction { get; set; }

        public MiniDatabase() {
            Tables = new List<ITable<T>>();
        }

        public async Task<DefinedTable<T>> FetchMissingTable(string identifier) {
            var list = await MissingFunction.Invoke(identifier);
            var table = new DefinedTable<T>() {
                Identifier = identifier,
                Table = list.ToList()
            };
            return table;
        }

        public async Task<IQueryable<T>> GetTable(string identifier) {
            var table = Tables.FirstOrDefault(x => x.Identifier == identifier) ?? await FetchMissingTable(identifier);

            return table.FetchTable();
        }

        public async Task<IList<T>> Query(string query) {
            throw new NotImplementedException();
        }
    }
}