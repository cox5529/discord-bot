using System;
using System.Collections.Generic;
using System.Text;

namespace Discord_bot.SelectTable.Models.Enum {
    public enum TokenType {
        Command, Distinct, Expression, Rename, From, Comma, TableName, Where, GroupBy, Having, Undefined, OrderBy, Literal, Number, LessThan, GreaterThan, Equal, And, Or, OpenParenthesis, CloseParenthesis
    }
}