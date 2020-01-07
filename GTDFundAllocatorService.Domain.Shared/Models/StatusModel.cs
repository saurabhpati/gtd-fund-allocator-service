using System.Collections.Generic;

namespace GTDFundAllocatorService.Domain.Shared
{
    public partial class StatusModel : Key
    {
        public StatusModel()
        {
            Fund = new HashSet<FundModel>();
        }

        public string Name { get; set; }

        public virtual ICollection<FundModel> Fund { get; set; }
    }
}
