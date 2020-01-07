using System.Collections.Generic;

namespace GTDFundAllocatorService.Repository.Shared
{
    public partial class Status : EntityBase
    {
        public Status()
        {
            Fund = new HashSet<Fund>();
        }

        public string Name { get; set; }

        public virtual ICollection<Fund> Fund { get; set; }
    }
}
