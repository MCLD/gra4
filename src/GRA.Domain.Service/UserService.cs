﻿using System;
using Microsoft.Extensions.Logging;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;

namespace GRA.Domain.Service
{
    public class UserService : Abstract.BaseService<UserService>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        public UserService(ILogger<UserService> logger,
            IUserRepository userRepository,
            IRoleRepository roleRepository)
            : base(logger)
        {
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _roleRepository = Require.IsNotNull(roleRepository, nameof(roleRepository));
        }

        public async Task<AuthenticationResult> AuthenticateUserAsync(string username,
            string password)
        {
            var authResult = await _userRepository.AuthenticateUserAsync(username, password);

            if (!authResult.FoundUser)
            {
                authResult.AuthenticationMessage = $"Could not find username '{username}'";
            }
            else if (!authResult.PasswordIsValid)
            {
                authResult.AuthenticationMessage = "The provided password is incorrect.";
            }
            else
            {
                authResult.PermissionNames
                    = await _roleRepository.GetPermisisonNamesForUserAsync(authResult.User.Id);
            }
            return authResult;
        }

        public async Task<User> RegisterUserAsync(User user, string password)
        {
            //todo: handle validation (username isn't already in use, etc)
            user.BranchId = 1;
            user.ProgramId = 1;
            user.SiteId = 1;
            var registeredUser = await _userRepository.AddSaveAsync(0, user);
            await _userRepository.SetUserPasswordAsync(registeredUser.Id, password);
            return registeredUser;
        }

        public async Task<DataWithCount<IEnumerable<User>>>
           GetPaginatedUserListAsync(ClaimsPrincipal user,
           int skip,
           int take)
        {
            if (UserHasPermission(user, Permission.ViewParticipantList))
            {
                var dataTask = _userRepository.PageAllAsync(skip, take);
                var countTask = _userRepository.GetCountAsync();
                await Task.WhenAll(dataTask, countTask);
                return new DataWithCount<IEnumerable<User>>
                {
                    Data = dataTask.Result,
                    Count = countTask.Result
                };
            } else
            {
                int userId = GetId(user, ClaimType.UserId);
                logger.LogError($"User {userId} doesn't have permission to view all participants.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<User> GetDetails(ClaimsPrincipal user, int userId)
        {
            if (UserHasPermission(user, Permission.ViewParticipantDetails))
            {
                return await _userRepository.GetByIdAsync(userId);
            }
            else
            {
                int requestedByUserId = GetId(user, ClaimType.UserId);
                logger.LogError($"User {requestedByUserId} doesn't have permission to view participant details.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<User> Update(ClaimsPrincipal user, User userToUpdate)
        {
            int requestedByUserId = GetId(user, ClaimType.UserId);

            if (UserHasPermission(user, Permission.EditParticipants)
                || requestedByUserId == userToUpdate.Id)
            {
                var currentEntity = await _userRepository.GetByIdAsync(userToUpdate.Id);
                userToUpdate.SiteId = currentEntity.SiteId;
                return await _userRepository.UpdateSaveAsync(requestedByUserId, userToUpdate);
            }
            else
            {
                logger.LogError($"User {requestedByUserId} doesn't have permission to update user {userToUpdate.Id}.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task Remove(ClaimsPrincipal user, int userIdToRemove)
        {
            int requestedByUserId = GetId(user, ClaimType.UserId);

            if (UserHasPermission(user, Permission.DeleteParticipants))
            {
                await _userRepository.RemoveSaveAsync(requestedByUserId, userIdToRemove);
            }
            else
            {
                logger.LogError($"User {requestedByUserId} doesn't have permission remove user {userIdToRemove}.");
                throw new Exception("Permission denied.");
            }
        }
    }
}