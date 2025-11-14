using System.Collections.Generic;

namespace com.InnovaMD.Provider.Models.Common
{
    public class SearchResponseModel<T>
    {
        public IEnumerable<T> Results { get; set; }
        public int TotalRows { get; set; }
    }
}
