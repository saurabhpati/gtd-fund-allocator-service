namespace GTDFundAllocatorService.Domain.Shared
{
    public partial class UserRoleModel
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public virtual RoleModel Role { get; set; }
        public virtual UserModel User { get; set; }
    }
}
