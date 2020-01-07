using System.Collections.Generic;

namespace GTDFundAllocatorService.Domain.Shared
{
    public partial class UserModel : Key
    {
        public UserModel()
        {
            ApprovedFund = new HashSet<FundModel>();
            CreatedFund = new HashSet<FundModel>();
            UpdatedFund = new HashSet<FundModel>();
            UserRole = new HashSet<UserRoleModel>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public virtual ICollection<FundModel> ApprovedFund { get; set; }
        public virtual ICollection<FundModel> CreatedFund { get; set; }
        public virtual ICollection<FundModel> UpdatedFund { get; set; }
        public virtual ICollection<UserRoleModel> UserRole { get; set; }
    }
}
