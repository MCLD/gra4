﻿using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class UserService : Abstract.BaseUserService<UserService>
    {
        private readonly GRA.Abstract.IPasswordValidator _passwordValidator;
        private readonly IAuthorizationCodeRepository _authorizationCodeRepository;
        private readonly IBadgeRepository _badgeRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IDrawingRepository _drawingRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IStaticAvatarRepository _staticAvatarRepository;
        private readonly IUserLogRepository _userLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly SampleDataService _configurationService;
        public UserService(ILogger<UserService> logger,
            IUserContextProvider userContextProvider,
            GRA.Abstract.IPasswordValidator passwordValidator,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IBadgeRepository badgeRepository,
            IBookRepository bookRepository,
            IDrawingRepository drawingRepository,
            INotificationRepository notificationRepository,
            IProgramRepository programRepository,
            IRoleRepository roleRepository,
            ISiteRepository siteRepository,
            IStaticAvatarRepository staticAvatarRepository,
            IUserLogRepository userLogRepository,
            IUserRepository userRepository,
            SampleDataService configurationService)
            : base(logger, userContextProvider)
        {
            _passwordValidator = Require.IsNotNull(passwordValidator, nameof(passwordValidator));
            _authorizationCodeRepository = Require.IsNotNull(authorizationCodeRepository,
                nameof(authorizationCodeRepository));
            _badgeRepository = Require.IsNotNull(badgeRepository, nameof(badgeRepository));
            _bookRepository = Require.IsNotNull(bookRepository, nameof(bookRepository));
            _drawingRepository = Require.IsNotNull(drawingRepository, nameof(drawingRepository));
            _notificationRepository = Require.IsNotNull(notificationRepository,
                nameof(notificationRepository));
            _programRepository = Require.IsNotNull(programRepository, nameof(programRepository));
            _roleRepository = Require.IsNotNull(roleRepository, nameof(roleRepository));
            _siteRepository = Require.IsNotNull(siteRepository, nameof(siteRepository));
            _staticAvatarRepository = Require.IsNotNull(staticAvatarRepository,
                nameof(staticAvatarRepository));
            _userLogRepository = Require.IsNotNull(userLogRepository, nameof(userLogRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _configurationService = Require.IsNotNull(configurationService,
                nameof(configurationService));
        }

        public async Task<User> RegisterUserAsync(User user, string password)
        {
            VerifyCanRegister();
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser != null)
            {
                throw new GraException("Someone has already chosen that username, please try another.");
            }

            _passwordValidator.Validate(password);

            user.CanBeDeleted = true;
            user.IsLockedOut = false;
            var registeredUser = await _userRepository.AddSaveAsync(0, user);
            await _userRepository
                .SetUserPasswordAsync(registeredUser.Id, registeredUser.Id, password);

            await JoinedProgramNotificationBadge(registeredUser);

            return registeredUser;
        }

        public async Task<DataWithCount<IEnumerable<User>>> GetPaginatedUserListAsync(int skip,
            int take,
            string search = null,
            SortUsersBy sortBy = SortUsersBy.LastName)
        {
            int siteId = GetClaimId(ClaimType.SiteId);
            if (HasPermission(Permission.ViewParticipantList))
            {
                return new DataWithCount<IEnumerable<User>>
                {
                    Data = await _userRepository.PageAllAsync(siteId, skip, take, search, sortBy),
                    Count = await _userRepository.GetCountAsync(siteId, search)
                };
            }
            else
            {
                int userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to view all participants.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<User>>>
            GetPaginatedFamilyListAsync(
            int householdHeadUserId,
            int skip,
            int take)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            var authUser = await _userRepository.GetByIdAsync(authUserId);
            if (authUserId == householdHeadUserId
                || authUser.HouseholdHeadUserId == householdHeadUserId
                || HasPermission(Permission.ViewParticipantList))
            {
                return new DataWithCount<IEnumerable<User>>
                {
                    Data = await _userRepository
                        .PageHouseholdAsync(householdHeadUserId, skip, take),
                    Count = await _userRepository.GetHouseholdCountAsync(householdHeadUserId)
                };
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to view household participants.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<int>
            FamilyMemberCountAsync(int householdHeadUserId)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            var authUser = await _userRepository.GetByIdAsync(authUserId);
            if (authUserId == householdHeadUserId
                || authUser.HouseholdHeadUserId == householdHeadUserId
                || HasPermission(Permission.ViewParticipantList))
            {
                return await _userRepository.GetHouseholdCountAsync(householdHeadUserId);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to get a count of household participants.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<User> GetDetails(int userId)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            var authUser = await _userRepository.GetByIdAsync(authUserId);
            var requestedUser = await _userRepository.GetByIdAsync(userId);
            if (requestedUser == null)
            {
                throw new GraException("The requested participant could not be accessed or does not exist.");
            }
            if (authUserId == userId
                || requestedUser.HouseholdHeadUserId == authUserId
                || authUser.HouseholdHeadUserId == userId
                || HasPermission(Permission.ViewParticipantDetails))
            {
                if (requestedUser.AvatarId != null)
                {
                    var avatar = await _staticAvatarRepository
                        .GetByIdAsync((int)requestedUser.AvatarId);
                    if (avatar != null)
                    {
                        requestedUser.StaticAvatarFilename = avatar.Filename;
                    }
                }
                return requestedUser;
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to view participant details.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<User> Update(User userToUpdate)
        {
            int requestingUserId = GetActiveUserId();

            if (requestingUserId == userToUpdate.Id)
            {
                // users can only update some of their own fields
                var currentEntity = await _userRepository.GetByIdAsync(userToUpdate.Id);
                currentEntity.IsAdmin = await UserHasRoles(userToUpdate.Id);
                currentEntity.AvatarId = userToUpdate.AvatarId;
                currentEntity.BranchId = userToUpdate.BranchId;
                currentEntity.BranchName = null;
                currentEntity.CardNumber = userToUpdate.CardNumber;
                currentEntity.Email = userToUpdate.Email;
                currentEntity.FirstName = userToUpdate.FirstName;
                currentEntity.LastName = userToUpdate.LastName;
                currentEntity.PhoneNumber = userToUpdate.PhoneNumber;
                currentEntity.PostalCode = userToUpdate.PostalCode;
                currentEntity.ProgramId = userToUpdate.ProgramId;
                currentEntity.ProgramName = null;
                currentEntity.SystemId = userToUpdate.SystemId;
                currentEntity.SystemName = null;
                //currentEntity.Username = userToUpdate.Username;
                return await _userRepository.UpdateSaveAsync(requestingUserId, currentEntity);
            }
            else
            {
                _logger.LogError($"User {requestingUserId} doesn't have permission to update user {userToUpdate.Id}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<User> MCUpdate(User userToUpdate)
        {
            int requestedByUserId = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.EditParticipants))
            {
                // admin users can update anything except siteid
                var currentEntity = await _userRepository.GetByIdAsync(userToUpdate.Id);
                userToUpdate.SiteId = currentEntity.SiteId;
                userToUpdate.IsAdmin = await UserHasRoles(userToUpdate.Id);
                return await _userRepository.UpdateSaveAsync(requestedByUserId, userToUpdate);
            }
            else
            {
                _logger.LogError($"User {requestedByUserId} doesn't have permission to update user {userToUpdate.Id}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task Remove(int userIdToRemove)
        {
            int requestedByUserId = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.DeleteParticipants))
            {
                var userLookup = await _userRepository.GetByIdAsync(userIdToRemove);
                if (!userLookup.CanBeDeleted)
                {
                    throw new GraException($"User {userIdToRemove} cannot be deleted.");
                }
                var familyCount = await _userRepository.GetHouseholdCountAsync(userIdToRemove);
                if (familyCount > 0)
                {
                    throw new GraException($"User {userIdToRemove} is the head of a family. Please remove all family members first.");
                }
                await _userRepository.RemoveSaveAsync(requestedByUserId, userIdToRemove);
            }
            else
            {
                _logger.LogError($"User {requestedByUserId} doesn't have permission to remove user {userIdToRemove}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<UserLog>>>
            GetPaginatedUserHistoryAsync(int userId,
            int skip,
            int take)
        {
            int requestedByUserId = GetActiveUserId();
            if (requestedByUserId == userId
               || HasPermission(Permission.ViewParticipantDetails))
            {
                return new DataWithCount<IEnumerable<UserLog>>
                {
                    Data = await _userLogRepository.PageHistoryAsync(userId, skip, take),
                    Count = await _userLogRepository.GetHistoryItemCountAsync(userId)
                };
            }
            else
            {
                _logger.LogError($"User {requestedByUserId} doesn't have permission to view details for {userId}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DataWithCount<ICollection<Book>>>
            GetPaginatedUserBookListAsync(int userId, int skip, int take)
        {
            int requestedByUserId = GetActiveUserId();
            if (requestedByUserId == userId
               || HasPermission(Permission.ViewParticipantDetails))
            {
                return await _bookRepository.GetPaginatedListForUserAsync(userId, skip, take);
            }
            else
            {
                _logger.LogError($"User {requestedByUserId} doesn't have permission to view details for {userId}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<DrawingWinner>>>
            GetPaginatedUserDrawingListAsync(int userId,
            int skip,
            int take)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.ViewUserDrawings))
            {
                return new DataWithCount<IEnumerable<DrawingWinner>>
                {
                    Data = await _drawingRepository.PageUserAsync(userId, skip, take),
                    Count = await _drawingRepository.GetUserWinCountAsync(userId)
                };
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to view drawinsg for {userId}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<string>
            ActivateAuthorizationCode(string authorizationCode)
        {
            string fixedCode = authorizationCode.Trim().ToLower();
            int siteId = GetClaimId(ClaimType.SiteId);
            var authCode
                = await _authorizationCodeRepository.GetByCodeAsync(siteId, fixedCode);

            if (authCode == null)
            {
                return null;
            }
            int userId = GetClaimId(ClaimType.UserId);
            await _userRepository.AddRoleAsync(userId, userId, authCode.RoleId);
            if (authCode.IsSingleUse)
            {
                await _authorizationCodeRepository.RemoveSaveAsync(userId, authCode.Id);
            }
            else
            {
                authCode.Uses++;
                await _authorizationCodeRepository.UpdateSaveAsync(userId, authCode);
            }

            // if the program doesn't have an email address assigned, perform that action here
            // TODO in the future this should be replaced with the initial setup process
            var user = await _userRepository.GetByIdAsync(userId);
            var program = await _programRepository.GetByIdAsync(user.ProgramId);
            if (string.IsNullOrEmpty(program.FromEmailAddress))
            {
                program.FromEmailAddress = user.Email;
                program.FromEmailName = user.FullName;
                await _programRepository.UpdateSaveAsync(userId, program);
            }

            user.IsAdmin = true;
            await _userRepository.UpdateSaveAsync(userId, user);

            return authCode.RoleName;
        }

        public async Task AddHouseholdMemberAsync(int householdHeadUserId, User memberToAdd)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            var householdHead = await _userRepository.GetByIdAsync(householdHeadUserId);

            if (householdHead.HouseholdHeadUserId != null)
            {
                _logger.LogError($"User {authUserId} cannot add a household member for {householdHeadUserId} who isn't a head of household.");
                throw new GraException("Cannot add a household member to someone who isn't a head of household.");
            }

            if (authUserId == householdHeadUserId
               || HasPermission(Permission.EditParticipants))
            {
                memberToAdd.HouseholdHeadUserId = householdHeadUserId;
                memberToAdd.SiteId = householdHead.SiteId;
                memberToAdd.CanBeDeleted = true;
                memberToAdd.IsLockedOut = false;
                var registeredUser = await _userRepository.AddSaveAsync(authUserId, memberToAdd);
                await JoinedProgramNotificationBadge(registeredUser);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to add a household member to {householdHeadUserId}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task RegisterHouseholdMemberAsync(User memberToRegister, string password)
        {
            VerifyCanRegister();
            int authUserId = GetClaimId(ClaimType.UserId);

            if (authUserId == (int)memberToRegister.HouseholdHeadUserId
               || HasPermission(Permission.EditParticipants))
            {
                var user = await GetDetails(memberToRegister.Id);
                if (!string.IsNullOrWhiteSpace(user.Username))
                {
                    _logger.LogError($"User {authUserId} cannot register household member {memberToRegister.Id} who is already registered.");
                    throw new GraException("Household member is already registered");
                }

                var existingUser = await _userRepository.GetByUsernameAsync(memberToRegister.Username);
                if (existingUser != null)
                {
                    throw new GraException("Someone has already chosen that username, please try another.");
                }

                _passwordValidator.Validate(password);

                user.Username = memberToRegister.Username;
                var registeredUser = await _userRepository.UpdateSaveAsync(authUserId, user);
                await _userRepository
                    .SetUserPasswordAsync(authUserId, user.Id, password);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to register household member {memberToRegister.Id}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<Badge>>>
            GetPaginatedBadges(int userId, int skip, int take)
        {
            int activeUserId = GetActiveUserId();

            if (userId == activeUserId
                || HasPermission(Permission.ViewParticipantDetails))
            {
                return new DataWithCount<IEnumerable<Badge>>
                {
                    Data = await _badgeRepository.PageForUserAsync(userId, skip, take),
                    Count = await _badgeRepository.GetCountForUserAsync(userId)
                };
            }
            else
            {
                _logger.LogError($"User {activeUserId} doesn't have permission to view details for {userId}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<IEnumerable<Notification>> GetNotificationsForUser()
        {
            return await _notificationRepository.GetByUserIdAsync(GetActiveUserId());
        }

        public async Task ClearNotificationsForUser()
        {
            await _notificationRepository.RemoveByUserId(GetActiveUserId());
        }

        private async Task<bool> UserHasRoles(int userId)
        {
            var roles = await _roleRepository.GetPermisisonNamesForUserAsync(userId);
            return roles != null && roles.Count() > 0;
        }

        private async Task JoinedProgramNotificationBadge(User registeredUser)
        {
            var program = await _programRepository.GetByIdAsync(registeredUser.ProgramId);
            var site = await _siteRepository.GetByIdAsync(registeredUser.SiteId);

            var notification = new Notification
            {
                PointsEarned = 0,
                Text = $"<span class=\"fa fa-thumbs-o-up\"></span> You've successfully joined <strong>{site.Name}</strong>!",
                UserId = registeredUser.Id,
                IsJoining = true
            };

            if (program.JoinBadgeId != null)
            {
                var badge = await _badgeRepository.GetByIdAsync((int)program.JoinBadgeId);
                await _badgeRepository.AddUserBadge(registeredUser.Id, badge.Id);
                await _userLogRepository.AddAsync(registeredUser.Id, new UserLog
                {
                    UserId = registeredUser.Id,
                    PointsEarned = 0,
                    IsDeleted = false,
                    BadgeId = badge.Id,
                    Description = $"Joined {site.Name}!"
                });
                notification.BadgeId = badge.Id;
                notification.BadgeFilename = badge.Filename;
            }
            await _notificationRepository.AddSaveAsync(registeredUser.Id, notification);
        }
    }
}