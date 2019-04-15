using System;
using Discord_bot.SelectTable.Models.Enum;
using Discord_bot.SelectTable.Sql;
using Xunit;

namespace Discord_bot.Test {
    public class TokenizerTest {

        [Fact]
        public void SimpleTest() {
            var query = "Select * from table";
            var tokens = Tokenizer.Tokenize(query);

            Assert.Equal(TokenType.Command, tokens[0].Type);
            Assert.Equal(TokenType.Expression, tokens[1].Type);
            Assert.Equal(TokenType.From, tokens[2].Type);
            Assert.Equal(TokenType.TableName, tokens[3].Type);
        }

        [Fact]
        public void MultipleColumnTest() {
            var query = "Select c1, c2 from table";
            var tokens = Tokenizer.Tokenize(query);

            Assert.Equal(TokenType.Command, tokens[0].Type);
            Assert.Equal(TokenType.Expression, tokens[1].Type);
            Assert.Equal(TokenType.Comma, tokens[2].Type);
            Assert.Equal(TokenType.Expression, tokens[3].Type);
            Assert.Equal(TokenType.From, tokens[4].Type);
            Assert.Equal(TokenType.TableName, tokens[5].Type);
        }

        [Fact]
        public void MultipleTableTest() {
            var query = "Select c1 from table1, table2";
            var tokens = Tokenizer.Tokenize(query);

            Assert.Equal(TokenType.Command, tokens[0].Type);
            Assert.Equal(TokenType.Expression, tokens[1].Type);
            Assert.Equal(TokenType.From, tokens[2].Type);
            Assert.Equal(TokenType.TableName, tokens[3].Type);
            Assert.Equal(TokenType.Comma, tokens[4].Type);
            Assert.Equal(TokenType.TableName, tokens[5].Type);
        }

        [Fact]
        public void MultipleColumnTableTest() {
            var query = "Select c1, c2 from table1, table2";
            var tokens = Tokenizer.Tokenize(query);

            Assert.Equal(TokenType.Command, tokens[0].Type);
            Assert.Equal(TokenType.Expression, tokens[1].Type);
            Assert.Equal(TokenType.Comma, tokens[2].Type);
            Assert.Equal(TokenType.Expression, tokens[3].Type);
            Assert.Equal(TokenType.From, tokens[4].Type);
            Assert.Equal(TokenType.TableName, tokens[5].Type);
            Assert.Equal(TokenType.Comma, tokens[6].Type);
            Assert.Equal(TokenType.TableName, tokens[7].Type);
        }

        [Fact]
        public void WhereTest() {
            var query = "Select c1 from table where c1 > 2";
            var tokens = Tokenizer.Tokenize(query);

            Assert.Equal(TokenType.Command, tokens[0].Type);
            Assert.Equal(TokenType.Expression, tokens[1].Type);
            Assert.Equal(TokenType.From, tokens[2].Type);
            Assert.Equal(TokenType.TableName, tokens[3].Type);
            Assert.Equal(TokenType.Where, tokens[4].Type);
            Assert.Equal(TokenType.Expression, tokens[5].Type);
            Assert.Equal(TokenType.GreaterThan, tokens[6].Type);
            Assert.Equal(TokenType.Number, tokens[7].Type);
        }

        [Fact]
        public void LiteralWhereTest() {
            var query = "Select c1 from table where c1 = 'howdy'";
            var tokens = Tokenizer.Tokenize(query);

            Assert.Equal(TokenType.Command, tokens[0].Type);
            Assert.Equal(TokenType.Expression, tokens[1].Type);
            Assert.Equal(TokenType.From, tokens[2].Type);
            Assert.Equal(TokenType.TableName, tokens[3].Type);
            Assert.Equal(TokenType.Where, tokens[4].Type);
            Assert.Equal(TokenType.Expression, tokens[5].Type);
            Assert.Equal(TokenType.Equal, tokens[6].Type);
            Assert.Equal(TokenType.Literal, tokens[7].Type);
        }

        [Fact]
        public void UnterminatedLiteralTest() {
            var query = "Select c1 from table where c1 = 'howdy";
            Assert.Throws<Exception>(() => Tokenizer.Tokenize(query));
        }

        [Fact]
        public void OrderByTest() {
            var query = "Select c1 from table order by c1";
            var tokens = Tokenizer.Tokenize(query);

            Assert.Equal(TokenType.Command, tokens[0].Type);
            Assert.Equal(TokenType.Expression, tokens[1].Type);
            Assert.Equal(TokenType.From, tokens[2].Type);
            Assert.Equal(TokenType.TableName, tokens[3].Type);
            Assert.Equal(TokenType.OrderBy, tokens[4].Type);
            Assert.Equal(TokenType.Expression, tokens[5].Type);
        }

        [Fact]
        public void OrderByMultipleTest() {
            var query = "Select c1 from table order by c1, c2";
            var tokens = Tokenizer.Tokenize(query);

            Assert.Equal(TokenType.Command, tokens[0].Type);
            Assert.Equal(TokenType.Expression, tokens[1].Type);
            Assert.Equal(TokenType.From, tokens[2].Type);
            Assert.Equal(TokenType.TableName, tokens[3].Type);
            Assert.Equal(TokenType.OrderBy, tokens[4].Type);
            Assert.Equal(TokenType.Expression, tokens[5].Type);
            Assert.Equal(TokenType.Comma, tokens[6].Type);
            Assert.Equal(TokenType.Expression, tokens[7].Type);
        }

        [Fact]
        public void GroupByTest() {
            var query = "Select c1 from table group by c1";
            var tokens = Tokenizer.Tokenize(query);

            Assert.Equal(TokenType.Command, tokens[0].Type);
            Assert.Equal(TokenType.Expression, tokens[1].Type);
            Assert.Equal(TokenType.From, tokens[2].Type);
            Assert.Equal(TokenType.TableName, tokens[3].Type);
            Assert.Equal(TokenType.GroupBy, tokens[4].Type);
            Assert.Equal(TokenType.Expression, tokens[5].Type);
        }

        [Fact]
        public void MultipleGroupByTest() {
            var query = "Select c1 from table group by c1, c2";
            var tokens = Tokenizer.Tokenize(query);

            Assert.Equal(TokenType.Command, tokens[0].Type);
            Assert.Equal(TokenType.Expression, tokens[1].Type);
            Assert.Equal(TokenType.From, tokens[2].Type);
            Assert.Equal(TokenType.TableName, tokens[3].Type);
            Assert.Equal(TokenType.GroupBy, tokens[4].Type);
            Assert.Equal(TokenType.Expression, tokens[5].Type);
            Assert.Equal(TokenType.Comma, tokens[6].Type);
            Assert.Equal(TokenType.Expression, tokens[7].Type);
        }

        [Fact]
        public void WhereGroupByTest() {
            var query = "Select c1 from table where c1 > 2 group by c1";
            var tokens = Tokenizer.Tokenize(query);

            Assert.Equal(TokenType.Command, tokens[0].Type);
            Assert.Equal(TokenType.Expression, tokens[1].Type);
            Assert.Equal(TokenType.From, tokens[2].Type);
            Assert.Equal(TokenType.TableName, tokens[3].Type);
            Assert.Equal(TokenType.Where, tokens[4].Type);
            Assert.Equal(TokenType.Expression, tokens[5].Type);
            Assert.Equal(TokenType.GreaterThan, tokens[6].Type);
            Assert.Equal(TokenType.Number, tokens[7].Type);
            Assert.Equal(TokenType.GroupBy, tokens[8].Type);
            Assert.Equal(TokenType.Expression, tokens[9].Type);
        }
        
        [Fact]
        public void GroupByHavingTest() {
            var query = "Select c1 from table group by c1 having c1 > 2";
            var tokens = Tokenizer.Tokenize(query);

            Assert.Equal(TokenType.Command, tokens[0].Type);
            Assert.Equal(TokenType.Expression, tokens[1].Type);
            Assert.Equal(TokenType.From, tokens[2].Type);
            Assert.Equal(TokenType.TableName, tokens[3].Type);
            Assert.Equal(TokenType.GroupBy, tokens[4].Type);
            Assert.Equal(TokenType.Expression, tokens[5].Type);
            Assert.Equal(TokenType.Having, tokens[6].Type);
            Assert.Equal(TokenType.Expression, tokens[7].Type);
            Assert.Equal(TokenType.GreaterThan, tokens[8].Type);
            Assert.Equal(TokenType.Number, tokens[9].Type);
        }

        [Fact]
        public void WhereGroupByHavingTest() {
            var query = "Select c1 from table where c1 > 2 group by c1 having c1 > 2";
            var tokens = Tokenizer.Tokenize(query);

            Assert.Equal(TokenType.Command, tokens[0].Type);
            Assert.Equal(TokenType.Expression, tokens[1].Type);
            Assert.Equal(TokenType.From, tokens[2].Type);
            Assert.Equal(TokenType.TableName, tokens[3].Type);
            Assert.Equal(TokenType.Where, tokens[4].Type);
            Assert.Equal(TokenType.Expression, tokens[5].Type);
            Assert.Equal(TokenType.GreaterThan, tokens[6].Type);
            Assert.Equal(TokenType.Number, tokens[7].Type);
            Assert.Equal(TokenType.GroupBy, tokens[8].Type);
            Assert.Equal(TokenType.Expression, tokens[9].Type);
            Assert.Equal(TokenType.Having, tokens[10].Type);
            Assert.Equal(TokenType.Expression, tokens[11].Type);
            Assert.Equal(TokenType.GreaterThan, tokens[12].Type);
            Assert.Equal(TokenType.Number, tokens[13].Type);
        }

        [Fact]
        public void FullTest() {
            var query = "Select c1, c2 from table1, table2 where c1 > 2 group by c1 having c1 > 2";
            var tokens = Tokenizer.Tokenize(query);

            Assert.Equal(TokenType.Command, tokens[0].Type);
            Assert.Equal(TokenType.Expression, tokens[1].Type);
            Assert.Equal(TokenType.Comma, tokens[2].Type);
            Assert.Equal(TokenType.Expression, tokens[3].Type);
            Assert.Equal(TokenType.From, tokens[4].Type);
            Assert.Equal(TokenType.TableName, tokens[5].Type);
            Assert.Equal(TokenType.Comma, tokens[6].Type);
            Assert.Equal(TokenType.TableName, tokens[7].Type);
            Assert.Equal(TokenType.Where, tokens[8].Type);
            Assert.Equal(TokenType.Expression, tokens[9].Type);
            Assert.Equal(TokenType.GreaterThan, tokens[10].Type);
            Assert.Equal(TokenType.Number, tokens[11].Type);
            Assert.Equal(TokenType.GroupBy, tokens[12].Type);
            Assert.Equal(TokenType.Expression, tokens[13].Type);
            Assert.Equal(TokenType.Having, tokens[14].Type);
            Assert.Equal(TokenType.Expression, tokens[15].Type);
            Assert.Equal(TokenType.GreaterThan, tokens[16].Type);
            Assert.Equal(TokenType.Number, tokens[17].Type);
        }

    }
}