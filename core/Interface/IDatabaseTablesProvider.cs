using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Interface
{
    interface IDatabaseAnalyzer
    {
        Dictionary<string, List<Tuple<string, List<string>>>> GetTableAdjacencyList(string connectionString);
    }
}
