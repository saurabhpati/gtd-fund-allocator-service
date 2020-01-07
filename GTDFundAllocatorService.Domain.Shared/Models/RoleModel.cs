using System.Collections.Generic;

namespace GTDFundAllocatorService.Domain.Shared
{
    public partial class RoleModel
    {
        public RoleModel()
        {
            UserRole = new HashSet<UserRoleModel>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UserRoleModel> UserRole { get; set; }
    }
}
