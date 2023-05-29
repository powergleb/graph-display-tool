using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Entity
{
    public class Table
    {
        public string TableName { get; set; }
        public string PrimaryKey { get; set; }
        public List<Tuple<String, Table>> ForeignKeys { get; set; }
        public Table()
        {
            ForeignKeys = new List<Tuple<string, Table>>();
        }


    }
}
