using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KOCR_Web.Infrastructure {
    public class DataTableParameters {
        public Dictionary<int, DataTableColumn> Columns;
        public int Draw;
        public int Length;
        public Dictionary<int, DataTableOrder> Order;
        public bool SearchRegex;
        public string SearchValue;
        public int Start;

        private DataTableParameters() {
        }

        /// <summary>
        /// Retrieve DataTable parameters from WebMethod parameter, sanitized against parameter spoofing
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DataTableParameters Get(object input) {
            return Get(JObject.FromObject(input));
        }

        /// <summary>
        /// Retrieve DataTable parameters from JSON, sanitized against parameter spoofing
        /// </summary>
        /// <param name="input">JToken object</param>
        /// <returns>parameters</returns>
        public static DataTableParameters Get(JToken input) {
            return new DataTableParameters {
                Columns = DataTableColumn.Get(input),
                Order = DataTableOrder.Get(input),
                Draw = (int)input["draw"],
                Start = (int)input["start"],
                Length = (int)input["length"],
                SearchValue =
                    new string(
                        ((string)input["search"]["value"]).Where(
                            c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-').ToArray()),
                SearchRegex = (bool)input["search"]["regex"]
            };
        }
    }

    public class DataTableColumn {
        public int Data;
        public string Name;
        public bool Orderable;
        public bool Searchable;
        public bool SearchRegex;
        public string SearchValue;

        private DataTableColumn() {
        }

        /// <summary>
        /// Retrieve the DataTables Columns dictionary from a JSON parameter list
        /// </summary>
        /// <param name="input">JToken object</param>
        /// <returns>Dictionary of Column elements</returns>
        public static Dictionary<int, DataTableColumn> Get(JToken input) {
            return (
                (JArray)input["columns"])
                .Select(col => new DataTableColumn {
                    Data = (int)col["data"],
                    Name =
                        new string(
                            ((string)col["name"]).Where(
                                c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-').ToArray()),
                    Searchable = (bool)col["searchable"],
                    Orderable = (bool)col["orderable"],
                    SearchValue =
                        new string(
                            ((string)col["search"]["value"]).Where(
                                c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-').ToArray()),
                    SearchRegex = (bool)col["search"]["regex"]
                })
                .ToDictionary(c => c.Data);
        }
    }

    public class DataTableOrder {
        public int Column;
        public string Direction;

        private DataTableOrder() {
        }

        /// <summary>
        /// Retrieve the DataTables order dictionary from a JSON parameter list
        /// </summary>
        /// <param name="input">JToken object</param>
        /// <returns>Dictionary of Order elements</returns>
        public static Dictionary<int, DataTableOrder> Get(JToken input) {
            return (
                (JArray)input["order"])
                .Select(col => new DataTableOrder {
                    Column = (int)col["column"],
                    Direction =
                        ((string)col["dir"]).StartsWith("desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC"
                })
                .ToDictionary(c => c.Column);
        }
    }
}
