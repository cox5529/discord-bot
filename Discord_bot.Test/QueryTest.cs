using System.Collections.Generic;
using System.Threading.Tasks;
using Discord_bot.SelectTable.Sql.MiniDatabase;
using Xunit;

namespace Discord_bot.Test {
    public class QueryTest {
        public MiniDatabase<SampleData> SimpleDatabase { get; set; }
        public MiniDatabase<SampleData> GroupingDatabase { get; set; }

        public QueryTest() {
            SimpleDatabase = new MiniDatabase<SampleData>();
            var t1 = new Table<SampleData>();
            var list = new List<SampleData>();
            for (var i = 0; i < 10; i++) {
                list.Add(new SampleData() {
                    Number = i,
                    Literal = "" + i
                });
            }

            t1.Data = list;
            t1.Identifier = "Integers";

            var t2 = new Table<SampleData>();
            list = new List<SampleData>();
            for (var i = 0; i < 10; i++) {
                list.Add(new SampleData() {
                    Number = i,
                    Literal = i / 10.0 + ""
                });
            }

            t2.Data = list;
            t2.Identifier = "Rationals";

            SimpleDatabase.Tables.Add(t1);
            SimpleDatabase.Tables.Add(t2);

            GroupingDatabase = new MiniDatabase<SampleData>();
            t1 = new Table<SampleData>();
            list = new List<SampleData> {
                new SampleData() {Number = 5, Literal = "five"},
                new SampleData() {Number = 5, Literal = "five three"},
                new SampleData() {Number = 5, Literal = "five four"},
                new SampleData() {Number = 2, Literal = "two"},
                new SampleData() {Number = 10, Literal = "ten"}
            };

            t1.Data = list;
            t1.Identifier = "t1";


            t2 = new Table<SampleData>();
            list = new List<SampleData> {
                new SampleData() {Number = 3, Literal = "three"},
                new SampleData() {Number = 3, Literal = "three four"},
                new SampleData() {Number = 2, Literal = "two"},
                new SampleData() {Number = 3, Literal = "three three"},
                new SampleData() {Number = 10, Literal = "ten"}
            };
            t2.Data = list;
            t2.Identifier = "t2";
            GroupingDatabase.Tables.Add(t1);
            GroupingDatabase.Tables.Add(t2);
        }

        [Fact]
        public async Task SimpleTest() {
            var query = "Select * from integers";
            var result = await SimpleDatabase.Query(query);

            Assert.Equal("Number", result[0][0]);
            Assert.Equal("Literal", result[0][1]);
            for (var i = 0; i < 10; i++) {
                Assert.Equal(i + "", result[i + 1][0]);
                Assert.Equal(i + "", result[i + 1][1]);
            }
        }

        [Fact]
        public async void SingleColumnTest() {
            var query = "Select number from integers";
            var result = await SimpleDatabase.Query(query);

            for (var i = 0; i < 10; i++) {
                Assert.Equal(i + "", result[i + 1][0]);
                Assert.Single(result[i + 1]);
            }
        }

        [Fact]
        public async void RenameTest() {
            var query = "Select number N from integers";
            var result = await SimpleDatabase.Query(query);

            Assert.Equal("N", result[0][0]);
            for (var i = 0; i < 10; i++) {
                Assert.Equal(i + "", result[i + 1][0]);
                Assert.Single(result[i + 1]);
            }
        }

        [Fact]
        public async void MultipleTableTest() {
            var query = "Select number from integers, rationals";
            var result = await SimpleDatabase.Query(query);

            for (var i = 0; i < 10; i++) {
                Assert.Equal(i + "", result[i + 1][0]);
                Assert.Single(result[i]);
            }

            for (var i = 10; i < 20; i++) {
                Assert.Equal(i - 10 + "", result[i + 1][0]);
                Assert.Single(result[i + 1]);
            }
        }

        [Fact]
        public async void MultipleColumnTableTest() {
            var query = "Select number, Literal from integers, rationals";
            var result = await SimpleDatabase.Query(query);

            for (var i = 0; i < 10; i++) {
                Assert.Equal(i + "", result[i + 1][0]);
                Assert.Equal(i + "", result[i + 1][1]);
            }

            for (var i = 10; i < 20; i++) {
                Assert.Equal(i - 10 + "", result[i + 1][0]);
                Assert.Equal((i - 10) / 10.0 + "", result[i + 1][1]);
            }
        }

        [Fact]
        public async void WhereTest() {
            var query = "Select number from integers where number > 2";
            var result = await SimpleDatabase.Query(query);

            for (var i = 0; i < 8; i++) {
                Assert.Equal(i + 3 + "", result[i + 1][0]);
                Assert.Single(result[i + 1]);
            }
        }

        [Fact]
        public async void LiteralWhereTest() {
            var query = "Select number from integers where literal = '2'";
            var result = await SimpleDatabase.Query(query);
            
            Assert.Single(result[1]);
            Assert.Equal("2", result[1][0]);
        }

        [Fact]
        public async void OrderByTest() {
            var query = "Select number from t1 order by number";
            var result = await GroupingDatabase.Query(query);

            Assert.Equal("2", result[1][0]);
            Assert.Equal("5", result[2][0]);
            Assert.Equal("5", result[3][0]);
            Assert.Equal("5", result[4][0]);
            Assert.Equal("10", result[5][0]);
        }

        [Fact]
        public async void OrderByMultipleTest() {
            var query = "Select literal from t1 order by number, Literal";
            var result = await GroupingDatabase.Query(query);

            Assert.Equal("two", result[1][0]);
            Assert.Equal("five", result[2][0]);
            Assert.Equal("five four", result[3][0]);
            Assert.Equal("five three", result[4][0]);
            Assert.Equal("ten", result[5][0]);
        }

        [Fact]
        public async void GroupByTest() {
            var query = "Select number from t1 group by number";
            var result = await GroupingDatabase.Query(query);

            Assert.Equal(4, result.Length);
            Assert.Equal("5", result[1][0]);
            Assert.Equal("2", result[2][0]);
            Assert.Equal("10", result[3][0]);
        }

        [Fact]
        public async void MultipleGroupByTest() {
            var query = "Select number from t1 group by number, Literal";
            var result = await GroupingDatabase.Query(query);

            Assert.Equal(6, result.Length);
            Assert.Equal("5", result[1][0]);
            Assert.Equal("5", result[2][0]);
            Assert.Equal("5", result[3][0]);
            Assert.Equal("2", result[4][0]);
            Assert.Equal("10", result[5][0]);
        }

        [Fact]
        public async void WhereGroupByTest() {
            var query = "Select number from t1 where number > 2 group by number";
            var result = await GroupingDatabase.Query(query);

            Assert.Equal(3, result.Length);
            Assert.Equal("5", result[1][0]);
            Assert.Equal("10", result[2][0]);
        }

        [Fact]
        public async void GroupByHavingTest() {
            var query = "Select number from t1 group by number having number > 2";
            var result = await GroupingDatabase.Query(query);

            Assert.Equal(3, result.Length);
            Assert.Equal("5", result[1][0]);
            Assert.Equal("10", result[2][0]);
        }

        [Fact]
        public async void WhereGroupByHavingTest() {
            var query = "Select number from t1 where number > 2 group by number having number > 5";
            var result = await GroupingDatabase.Query(query);

            Assert.Equal(3, result.Length);
            Assert.Equal("5", result[1][0]);
            Assert.Equal("10", result[2][0]);
        }

        [Fact]
        public async void FullTest() {
            var query =
                "Select number, Literal from t1, t2 where number > 2 group by number having number > 2";
            var result = await GroupingDatabase.Query(query);

            Assert.Equal(3, result.Length);
            Assert.Equal("5", result[1][0]);
            Assert.Equal("10", result[2][0]);
        }
    }

    public class SampleData {
        public int Number { get; set; }
        public string Literal { get; set; }
    }
}