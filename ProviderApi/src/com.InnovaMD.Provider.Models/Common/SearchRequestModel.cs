using System.ComponentModel.DataAnnotations;

namespace com.InnovaMD.Provider.Models.Common
{
    public class SearchRequestModel
    {
        [Required]
        public int Page { get; set; }
        [Required]
        public int PageSize { get; set; }
        public string SortBy { get; set; }
    }
}
