﻿using GRA.Domain.Model;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;

namespace GRA.Domain.Service.Abstract
{
    public abstract class BaseService<T>
    {
        protected readonly ILogger<T> logger;
        public BaseService(ILogger<T> logger)
        {
            this.logger = Require.IsNotNull(logger, nameof(logger));
        }

        protected bool UserHasPermission(ClaimsPrincipal user, Permission permission)
        {
            return new UserClaimLookup(user).UserHasPermission(permission.ToString());
        }

        protected int GetId(ClaimsPrincipal user, string claimType)
        {
            string result = new UserClaimLookup(user).UserClaim(claimType);
            if(string.IsNullOrEmpty(result))
            {
                throw new System.Exception($"Could not find user claim '{claimType}'");
            }
            int id;
            if(int.TryParse(result, out id))
            {
                return id;
            }
            else
            {
                throw new System.Exception($"Could not convert '{claimType}' to a number.");
            }
        }
    }
}
