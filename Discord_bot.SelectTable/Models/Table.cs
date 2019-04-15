using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord_bot.SelectTable.Models {
    public class Table<T> {
        public List<T> Data { get; set; }

        private List<Column<T>> Columns { get; }

        public Table() {
            Data = new List<T>();
            Columns = new List<Column<T>>();
        }

        public void AddColumn(string header, Func<T, string> mapFunc) {
            Columns.Add(new Column<T>(header, mapFunc));
        }

        public void RemoveColumn(string header) {
            Columns.RemoveAll(x => x.Header == header);
        }

        public string BuildTable() {
            var lengths = new int[Columns.Count];
            for (var i = 0; i < Columns.Count; i++) {
                lengths[i] = Columns[i].SetMaxLength(Data);
            }

            var result = GetSeparator(lengths) + "\n";

            foreach (var column in Columns) {
                result += "| " + PadToLength(column.MaxLength, column.Header) + " ";
            }
            result += "|\n" + GetSeparator(lengths) + "\n";

            foreach (var data in Data) {
                foreach (var column in Columns) {
                    result += "| " + PadToLength(column.MaxLength, column.MapFunc.Invoke(data)) + " ";
                }
                result += "|\n" + GetSeparator(lengths) + "\n";
            }

            return result;
        }

        private static string PadToLength(int length, string s) {
            while (s.Length < length) {
                s += " ";
            }

            return s;
        }

        private static string GetSeparator(IEnumerable<int> lengths) {
            var result = "";
            foreach (var length in lengths) {
                if (result != "") result += "-";
                result += "+-";
                for (var i = 0; i < length; i++) {
                    result += "-";
                }
            }

            return result + "-+";
        }
    }

    public class Column<T> {
        public string Header { get; set; }
        public Func<T, string> MapFunc { get; set; }
        public int MaxLength { get; private set; }

        public Column(string header, Func<T, string> mapFunc) {
            Header = header;
            MapFunc = mapFunc;
        }

        public int SetMaxLength(List<T> data) {
            MaxLength = data.Select(x => MapFunc.Invoke(x).Length).Max();
            if (Header.Length > MaxLength) MaxLength = Header.Length;
            return MaxLength;
        }
    }
}