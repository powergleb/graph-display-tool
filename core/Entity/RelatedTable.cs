using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Entity
{
    public class RelatedTable
    {
        public string TableName { get; set; }
        public string ForeignKey { get; set; }

        public RelatedTable(string tableName, string foreignKey)
        {
            TableName = tableName;
            ForeignKey = foreignKey;
        }
    }
}
