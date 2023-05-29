using core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Interface
{
    public interface IDatabaseAnalyzer
    {
        List<Table> GetTables(string connectionString);
    }
}
