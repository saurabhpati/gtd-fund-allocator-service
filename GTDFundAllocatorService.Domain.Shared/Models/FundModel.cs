using System;

namespace GTDFundAllocatorService.Domain.Shared
{
    public partial class FundModel : Key
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

        public virtual UserModel Approver { get; set; }
        public virtual UserModel Creator { get; set; }
        public virtual StatusModel Status { get; set; }
        public virtual UserModel Updater { get; set; }
    }

    public enum FundStatus : int
    {
        Pending = 1,
        Approved = 2,
        Denied = 3
    }
}
