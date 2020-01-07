using System;
using System.Collections.Generic;

namespace GTDFundAllocatorService.Repository.Shared
{
    public class SearchCriteria
    {
        public SearchCriteria()
        {
            ThenBy = new List<Tuple<string, bool>>();
        }

        public string SortBy { get; set; }

        public bool IsDescending { get; set; }

        //Tuple<fieldName, IsDescending>
        public IList<Tuple<string, bool>> ThenBy { get; set; }

        public int? Take { get; set; }

        public int? Skip { get; set; }
    }
}
