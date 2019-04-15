using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord_bot.SelectTable.Models;
using Discord_bot.SelectTable.Models.Enum;

namespace Discord_bot.SelectTable.Sql.MiniDatabase {
    public class MiniDatabase<T> {
        public IList<Table<T>> Tables { get; set; }
        public Func<string, Task<IQueryable<T>>> MissingFunction { get; set; }

        public MiniDatabase() {
            Tables = new List<Table<T>>();
        }

        public async Task<Table<T>> FetchMissingTable(string identifier) {
            var list = await MissingFunction.Invoke(identifier);
            var table = new Table<T>() {
                Identifier = identifier,
                Data = list.ToList()
            };
            return table;
        }

        public async Task<IQueryable<T>> GetTable(string identifier) {
            var table = Tables.FirstOrDefault(x =>
                            x.Identifier.Equals(identifier, StringComparison.CurrentCultureIgnoreCase)) ??
                        await FetchMissingTable(identifier);

            return table.FetchTable();
        }

        public async Task<string[][]> Query(string query) {
            var tokens = Tokenizer.Tokenize(query);
            var command = tokens[0];
            if (command.Type != TokenType.Command || command.Value.ToLower() != "select") {
                throw new Exception("There is an error in your SQL syntax: You can only perform select queries.");
            }

            var index = 0;

            var columns = new List<ColumnData<T>>();
            var tables = new List<string>();
            var whereCondition = new Expression<T>();
            var groupBy = new List<string>();
            var havingCondition = new Expression<T>();
            var orderBy = new List<string>();

            // 1. Separate
            // Column names
            if (tokens.Where(x => x.Type == TokenType.Expression).Any(x => x.Value == "*")) {
                columns.AddRange(typeof(T).GetProperties().Select(p => new ColumnData<T>(p.Name)));
                index = tokens.IndexOf(tokens.FirstOrDefault(x => x.Type == TokenType.From)) + 1;
            } else {
                for (var i = index; i < tokens.Count; i++) {
                    var token = tokens[i];
                    if (token.Type == TokenType.Expression) {
                        var rename = "";
                        for (var j = i + 1; j < tokens.Count; j++) {
                            if (tokens[j].Type != TokenType.Comma && tokens[j].Type != TokenType.From) {
                                if (!string.IsNullOrWhiteSpace(rename)) rename += " ";
                                rename += tokens[j].Value;
                            } else break;
                        }

                        columns.Add(string.IsNullOrWhiteSpace(rename)
                            ? new ColumnData<T>(token)
                            : new ColumnData<T>(token, rename));
                    } else if (token.Type == TokenType.From) {
                        index = i + 1;
                        break;
                    }
                }
            }


            // Table names
            for (var i = index; i < tokens.Count; i++) {
                var token = tokens[i];
                if (token.Type == TokenType.TableName) {
                    tables.Add(token.Value);
                } else if (token.Type != TokenType.Comma) {
                    index = i;
                    break;
                }
            }

            // Where conditions
            // Group by
            // Having conditions
            // Order by

            // 2. Query
            var results = new List<T>();

            foreach (var tableName in tables) {
                var tableData = await GetTable(tableName);
                results.AddRange(tableData);
            }

            // 3. Apply where condition
            // 4. Apply grouping
            // 5. Apply Having condition
            // 6. Apply order by
            // 7. Build to result string

            var resultTable = new string[results.Count + 1][];
            resultTable[0] = new string[columns.Count];
            for (var i = 0; i < columns.Count; i++) {
                resultTable[0][i] = columns[i].DisplayName;
            }

            for (var i = 0; i < results.Count; i++) {
                resultTable[i + 1] = new string[columns.Count];
                for (var j = 0; j < columns.Count; j++) {
                    resultTable[i + 1][j] = columns[j].Get(results[i]);
                }
            }

            return resultTable;
        }
    }
}