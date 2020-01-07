using System;
using System.Linq;
using GTDFundAllocatorService.Foundation.Common;
using Microsoft.AspNetCore.Mvc;

namespace GTDFundAllocatorService.Controllers
{
    public class BaseController : ControllerBase
    {
        private int userId;

        public int UserId
        {
            get
            {
                if (userId == 0)
                {
                    var userClaimId = User
                                        .Claims
                                        .FirstOrDefault(claim =>
                                            string.Compare(
                                                claim.Type,
                                                Constants.USERID_NAMESPACE,
                                                StringComparison.OrdinalIgnoreCase) == 0);
                    userId = userClaimId != null ? Convert.ToInt32(userClaimId.Value) : 0;
                    return userId;
                }

                return userId;
            }
        }

        private int roleId;

        public int RoleId
        {
            get
            {
                if (roleId == 0)
                {
                    var roleClaimId = User
                                        .Claims
                                        .FirstOrDefault(claim =>
                                            string.Compare(
                                                claim.Type,
                                                Constants.ROLE_NAMESPACE,
                                                StringComparison.OrdinalIgnoreCase) == 0);
                    roleId = roleClaimId != null ? Convert.ToInt32(roleClaimId.Value) : 0;
                    return roleId;
                }

                return roleId;
            }
        }
    }
}
