using System;
using System.Collections.Generic;
using System.Linq;
using Discord_bot.SelectTable.Models;

namespace Discord_bot.SelectTable.Sql {
    public class Expression<T> {

        public IList<Token> Tokens { get; set; }

        public Expression() {
            Tokens = new List<Token>();
        }

        public bool Evaluate(T data) {
            return true;
        }

        private static string GetProperty(T data, string property) {
            var properties = data.GetType().GetProperties();
            return (from p in properties
                where string.Equals(p.Name, property, StringComparison.CurrentCultureIgnoreCase)
                select p.GetValue(data).ToString()).FirstOrDefault();
        }
        
    }
}