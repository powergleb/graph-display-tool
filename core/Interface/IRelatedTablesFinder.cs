using core.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Interface
{
    public interface IRelatedTablesFinder
    {
        List<RelatedTable> GetRelatedTables(SqlConnection connection, string tableName);
    }
}
