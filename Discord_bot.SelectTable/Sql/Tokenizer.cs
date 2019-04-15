using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord_bot.SelectTable.Models;
using Discord_bot.SelectTable.Models.Enum;

namespace Discord_bot.SelectTable.Sql {
    public class Tokenizer {
        public static IList<Token> Tokenize(string s) {
            var list = new List<Token>();
            var words = Split(" \n\t", ",=<>()&|", "\"'", s);

            var lowerWords = words.Select(x => x.ToLower()).ToArray();

            for (var i = 0; i < words.Count; i++) {
                var last = new Token() {
                    Type = TokenType.Undefined
                };
                if (list.Any()) last = list.Last();

                if (i == 0) {
                    if (lowerWords[i] != "select") {
                        throw new Exception(
                            "There is an error in your SQL syntax: You can only perform select commands.");
                    }

                    list.Add(new Token() {
                        Type = TokenType.Command,
                        Value = words[i]
                    });
                } else if (last.Type == TokenType.Command && lowerWords[i] == "distinct") {
                    list.Add(new Token() {
                        Type = TokenType.Distinct,
                        Value = words[i]
                    });
                } else if (list.All(x => x.Type != TokenType.From)) {
                    if (lowerWords[i] == ",") {
                        list.Add(new Token() {
                            Type = TokenType.Comma,
                            Value = ","
                        });
                    } else if (lowerWords[i] == "from") {
                        list.Add(new Token() {
                            Type = TokenType.From,
                            Value = words[i]
                        });
                    } else if (last.Type == TokenType.Comma || last.Type == TokenType.Command ||
                               last.Type == TokenType.Distinct) {
                        list.Add(new Token() {
                            Type = TokenType.Expression,
                            Value = words[i]
                        });
                    } else if (last.Type == TokenType.Expression || last.Type == TokenType.Rename) {
                        list.Add(new Token() {
                            Type = TokenType.Rename,
                            Value = words[i]
                        });
                    } else {
                        throw new Exception(
                            "There is an error in your SQL syntax: Malformatted column selection statement");
                    }
                } else if (last.Type == TokenType.From || last.Type == TokenType.Comma && !list.Any(x =>
                               x.Type == TokenType.GroupBy || x.Type == TokenType.Having ||
                               x.Type == TokenType.Where || x.Type == TokenType.OrderBy)) {
                    list.Add(new Token() {
                        Type = TokenType.TableName,
                        Value = words[i]
                    });
                } else if (last.Type == TokenType.TableName && lowerWords[i] == ",") {
                    list.Add(new Token() {
                        Type = TokenType.Comma,
                        Value = words[i]
                    });
                } else if (last.Type == TokenType.TableName && lowerWords[i] == "where") {
                    list.Add(new Token() {
                        Type = TokenType.Where,
                        Value = words[i]
                    });

                    var conditionalList = new List<Token>();

                    for (var j = i + 1; j < words.Count; j++) {
                        if ((lowerWords[j] == "group" || lowerWords[j] == "order") && j + 1 < words.Count &&
                            lowerWords[j + 1] == "by") {
                            break;
                        }

                        conditionalList.Add(new Token() {
                            Value = words[j],
                            Type = TokenType.Undefined
                        });
                    }

                    list.AddRange(ParseCondition(conditionalList));

                    i += conditionalList.Count;
                } else if (list.All(x => x.Type != TokenType.Having && x.Type != TokenType.OrderBy) &&
                           words.Count > i + 1 && lowerWords[i] == "group" && lowerWords[i + 1] == "by") {
                    list.Add(new Token() {
                        Type = TokenType.GroupBy,
                        Value = words[i] + " " + words[i + 1]
                    });
                    i++;
                } else if (list.Any(x => x.Type == TokenType.GroupBy) &&
                           list.All(x => x.Type != TokenType.Having && x.Type != TokenType.OrderBy) && !(
                               lowerWords[i] == "having" ||
                               (lowerWords[i] == "order" && i + 1 < words.Count && lowerWords[i + 1] == "by"))) {
                    if (last.Type == TokenType.Expression && lowerWords[i] == ",") {
                        list.Add(new Token() {
                            Type = TokenType.Comma,
                            Value = ","
                        });
                    } else if (last.Type == TokenType.GroupBy || last.Type == TokenType.Comma) {
                        list.Add(new Token() {
                            Type = TokenType.Expression,
                            Value = words[i]
                        });
                    } else {
                        throw new Exception("There is an error in your SQL syntax: Malformatted group by statement");
                    }
                } else if (lowerWords[i] == "having") {
                    list.Add(new Token() {
                        Type = TokenType.Having,
                        Value = words[i]
                    });

                    var conditionalList = new List<Token>();

                    for (var j = i + 1; j < words.Count; j++) {
                        if (j + 1 < words.Count && lowerWords[j] == "order" && lowerWords[j + 1] == "by") {
                            break;
                        }

                        conditionalList.Add(new Token() {
                            Value = words[j],
                            Type = TokenType.Undefined
                        });
                    }

                    list.AddRange(ParseCondition(conditionalList));
                    i += conditionalList.Count;
                } else if (i + 1 < words.Count && lowerWords[i] == "order" && lowerWords[i + 1] == "by") {
                    list.Add(new Token() {
                        Type = TokenType.OrderBy,
                        Value = words[i] + " " + words[i + 1]
                    });
                    i++;
                } else if (list.Any(x => x.Type == TokenType.OrderBy)) {
                    if (last.Type == TokenType.Expression && lowerWords[i] == ",") {
                        list.Add(new Token() {
                            Type = TokenType.Comma,
                            Value = ","
                        });
                    } else if (last.Type == TokenType.OrderBy || last.Type == TokenType.Comma) {
                        list.Add(new Token() {
                            Type = TokenType.Expression,
                            Value = words[i]
                        });
                    } else {
                        throw new Exception("There is an error in your SQL syntax: Malformatted order by statement");
                    }
                } else {
                    throw new Exception("There is an error in your SQL syntax: Unknown symbol '" + words[i] + "'.");
                }
            }

            return list;
        }

        private static IList<string> Split(string splitOnExclude, string splitOnInclude, string literalDelimiter,
            string query) {
            var result = new List<string>();

            var s = "";
            var inLiteral = false;
            var literalStart = '"';
            foreach (var c in query) {
                if (!inLiteral && literalDelimiter.Contains(c)) {
                    inLiteral = true;
                    literalStart = c;
                } else if ((splitOnInclude.Contains(c) || splitOnExclude.Contains(c))) {
                    if (!string.IsNullOrWhiteSpace(s)) {
                        result.Add(s);
                        s = "";
                    }

                    if (splitOnInclude.Contains(c)) {
                        result.Add(c + "");
                    }

                    continue;
                }

                s += c;

                if (!inLiteral || s.Length <= 1 || c != literalStart) continue;

                inLiteral = false;
                result.Add(s);
                s = "";
            }

            if(inLiteral) throw new Exception("There is an error in your SQL syntax: Unterminated literal");

            result.Add(s);

            return result;
        }

        private static IList<Token> ParseCondition(IList<Token> condition) {
            foreach (var token in condition) {
                if (token.Value == "<") {
                    token.Type = TokenType.LessThan;
                } else if (token.Value == ">") {
                    token.Type = TokenType.GreaterThan;
                } else if (token.Value == "=") {
                    token.Type = TokenType.Equal;
                } else if (token.Value.All(x => char.IsDigit(x) || x == '.')) {
                    token.Type = TokenType.Number;
                } else if (token.Value.StartsWith("\"") || token.Value.StartsWith("'")) {
                    if (token.Value.First() == token.Value.Last()) {
                        token.Type = TokenType.Literal;
                        token.Value = token.Value.Substring(1, token.Value.Length - 2);
                    } else {
                        throw new Exception("There is an error in your SQL syntax: Unterminated literal");
                    }
                } else if (char.IsLetter(token.Value.First())) {
                    token.Type = TokenType.Expression;
                } else if (token.Value == "(") {
                    token.Type = TokenType.OpenParenthesis;
                } else if (token.Value == ")") {
                    token.Type = TokenType.CloseParenthesis;
                } else if (token.Value.Equals("and", StringComparison.OrdinalIgnoreCase) || token.Value == "&") {
                    token.Type = TokenType.And;
                } else if (token.Value.Equals("or", StringComparison.OrdinalIgnoreCase) || token.Value == "|") {
                    token.Type = TokenType.Or;
                } else {
                    throw new Exception("There is an error in your SQL syntax: Malformatted condition statement");
                }
            }

            return condition;
        }
    }
}