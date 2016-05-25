using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTestHelpers
{
    public interface IDataService
    {
        Task<IEnumerable<string>> LoadData();
    }
}
