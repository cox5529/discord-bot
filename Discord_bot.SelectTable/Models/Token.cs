using System;
using System.Collections.Generic;
using System.Text;
using Discord_bot.SelectTable.Models.Enum;

namespace Discord_bot.SelectTable.Models {
    public class Token {

        public TokenType Type { get; set; }
        public string Value { get; set; }

    }
}