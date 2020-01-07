using System.Collections.Generic;

namespace GTDFundAllocatorService.Repository.Shared
{
    public partial class User : EntityBase
    {
        public User()
        {
            ApprovedFund = new HashSet<Fund>();
            CreatedFund = new HashSet<Fund>();
            UpdatedFund = new HashSet<Fund>();
            UserRole = new HashSet<UserRole>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public virtual ICollection<Fund> ApprovedFund { get; set; }
        public virtual ICollection<Fund> CreatedFund { get; set; }
        public virtual ICollection<Fund> UpdatedFund { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; }
    }
}
