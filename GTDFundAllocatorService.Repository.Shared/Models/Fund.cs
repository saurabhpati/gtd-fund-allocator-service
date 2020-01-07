using System;

namespace GTDFundAllocatorService.Repository.Shared
{
    public partial class Fund : EntityBase
    {
        public int Amount { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public int StatusId { get; set; }
        public bool Disabled { get; set; }

        public virtual User Approver { get; set; }
        public virtual User Creator { get; set; }
        public virtual Status Status { get; set; }
        public virtual User Updater { get; set; }
    }
}
