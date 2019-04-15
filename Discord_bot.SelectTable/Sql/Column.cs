using Discord_bot.SelectTable.Models;
using System;
using System.Linq;

namespace Discord_bot.SelectTable.Sql {
    public class ColumnData<T> {
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public ColumnData(string name) {
            Name = name;
            DisplayName = name;
        }

        public ColumnData(Token nameToken) {
            Name = nameToken.Value;
            DisplayName = nameToken.Value;
        }

        public ColumnData(Token nameToken, string displayToken) {
            Name = nameToken.Value;
            DisplayName = displayToken;
        }

        public string Get(T data) {
            var properties = data.GetType().GetProperties();
            return (from p in properties
                where string.Equals(p.Name, Name, StringComparison.CurrentCultureIgnoreCase)
                select p.GetValue(data).ToString()).FirstOrDefault();
        }
    }
}