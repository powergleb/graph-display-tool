using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Interface
{
    public interface IPrimaryKeyFinder
    {
        string GetPrimaryKey(SqlConnection connection, string tableName);
    }
}
