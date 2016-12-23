using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class MailService : Abstract.BaseUserService<MailService>
    {
        private IMailRepository _mailRepository;
        private IMemoryCache _memoryCache;
        public MailService(ILogger<MailService> logger,
            IUserContextProvider userContextProvider,
            IMailRepository mailRepository,
            IMemoryCache memoryCache) : base(logger, userContextProvider)
        {
            _mailRepository = Require.IsNotNull(mailRepository, nameof(mailRepository));
            _memoryCache = Require.IsNotNull(memoryCache, nameof(memoryCache));
        }

        public async Task<int> GetUserUnreadCountAsync()
        {
            var activeUserId = GetActiveUserId();
            var cacheKey = $"{CacheKey.UserUnreadMailCount}?userId={activeUserId}";
            int unreadCount;
            if (!_memoryCache.TryGetValue(cacheKey, out unreadCount))
            {
                unreadCount = await _mailRepository.GetUserUnreadCountAsync(activeUserId);
                _memoryCache.Set(cacheKey, unreadCount, new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return unreadCount;
        }

        public async Task<DataWithCount<IEnumerable<Mail>>> GetUserInboxPaginatedAsync(int skip,
            int take)
        {
            var activeUserId = GetActiveUserId();
            return new DataWithCount<IEnumerable<Mail>>
            {
                Data = await _mailRepository.PageUserInboxAsync(activeUserId, skip, take),
                Count = await _mailRepository.GetUserInboxCountAsync(activeUserId)
            };
        }

        public async Task<DataWithCount<IEnumerable<Mail>>> GetUserPaginatedAsync(
            int getMailForUserId,
            int skip,
            int take)
        {
            if (HasPermission(Permission.ReadAllMail))
            {
                return new DataWithCount<IEnumerable<Mail>>
                {
                    Data = await _mailRepository.PageUserAsync(getMailForUserId, skip, take),
                    Count = await _mailRepository.GetUserCountAsync(getMailForUserId)
                };
            }
            else
            {
                var requestingUser = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {requestingUser} doesn't have permission to view messages for {getMailForUserId}.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<Mail> GetDetails(int mailId)
        {
            var activeUserId = GetActiveUserId();
            bool canReadAll = HasPermission(Permission.ReadAllMail);
            var mail = await _mailRepository.GetByIdAsync(mailId);
            if (mail.FromUserId == activeUserId || mail.ToUserId == activeUserId || canReadAll)
            {
                return mail;
            }
            _logger.LogError($"User {activeUserId} doesn't have permission to view details for message {mailId}.");
            throw new Exception("Permission denied.");
        }

        public async Task<Mail> GetParticipantMailAsync(int mailId)
        {
            var activeUserId = GetActiveUserId();
            try
            {
                var mail = await _mailRepository.GetByIdAsync(mailId);
                if (mail.ToUserId == activeUserId)
                {
                    return mail;
                }
                else
                {
                    throw new GraException("Mail belongs to someone else.");
                }
            }
            catch (Exception)
            {
                throw new GraException("Mail doesn't exist");
            }
        }

        public async Task<DataWithCount<IEnumerable<Mail>>> GetAllPaginatedAsync(int skip,
            int take)
        {
            int siteId = GetClaimId(ClaimType.SiteId);
            if (HasPermission(Permission.ReadAllMail))
            {
                return new DataWithCount<IEnumerable<Mail>>
                {
                    Data = await _mailRepository.PageAllAsync(siteId, skip, take),
                    Count = await _mailRepository.GetAllCountAsync(siteId)
                };
            }
            else
            {
                var userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to get all mails.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<Mail>>> GetAllUnrepliedPaginatedAsync(int skip,
            int take)
        {
            if (HasPermission(Permission.ReadAllMail))
            {
                int siteId = GetClaimId(ClaimType.SiteId);
                return new DataWithCount<IEnumerable<Mail>>
                {
                    Data = await _mailRepository.PageAdminUnrepliedAsync(siteId, skip, take),
                    Count = await _mailRepository.GetAdminUnrepliedCountAsync(siteId)
                };
            }
            else
            {
                var userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to get all unread mails.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<int> GetAdminUnreadCountAsync()
        {
            if (HasPermission(Permission.ReadAllMail))
            {
                int siteId = GetClaimId(ClaimType.SiteId);
                var cacheKey = $"{CacheKey.UnhandledMailCount}?siteId={siteId}";
                int unhandledCount;
                if (!_memoryCache.TryGetValue(cacheKey, out unhandledCount))
                {
                    unhandledCount = await _mailRepository.GetAdminUnrepliedCountAsync(siteId);
                    _memoryCache.Set(cacheKey, unhandledCount, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));
                }
                return unhandledCount;
            }
            else
            {
                var userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to get unread mail count.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task MarkAsReadAsync(int mailId)
        {
            var activeUserId = GetActiveUserId();
            var mail = await _mailRepository.GetByIdAsync(mailId);
            if (mail.ToUserId == activeUserId)
            {
                await _mailRepository.MarkAsReadAsync(mailId);
                _memoryCache.Remove($"{CacheKey.UserUnreadMailCount}?userId={activeUserId}");
                return;
            }
            _logger.LogError($"User {activeUserId} doesn't have permission mark mail {mailId} as read.");
            throw new Exception("Permission denied.");
        }

        public async Task<Mail> SendAsync(Mail mail)
        {
            var authId = GetClaimId(ClaimType.UserId);
            var activeUserId = GetActiveUserId();
            if (mail.ToUserId == null)
            {
                mail.FromUserId = activeUserId;
                mail.IsNew = true;
                mail.IsDeleted = false;
                mail.SiteId = GetClaimId(ClaimType.SiteId);
                return await _mailRepository.AddSaveAsync(authId, mail);
            }
            else
            {
                _logger.LogError($"User {activeUserId} doesn't have permission to send a mail to {mail.ToUserId}.");
                throw new Exception("Permission denied");
            }
        }

        public async Task<Mail> SendReplyAsync(Mail mail)
        {
            var authId = GetClaimId(ClaimType.UserId);
            var activeUserId = GetActiveUserId();
            var inReplyToMail = await _mailRepository.GetByIdAsync(mail.InReplyToId.Value);
            if (inReplyToMail.ToUserId == activeUserId)
            {
                mail.ThreadId = inReplyToMail.ThreadId ?? mail.InReplyToId.Value;
                mail.FromUserId = activeUserId;
                mail.ToUserId = null;
                mail.IsNew = true;
                mail.IsDeleted = false;
                mail.SiteId = GetClaimId(ClaimType.SiteId);
                return await _mailRepository.AddSaveAsync(authId, mail);
            }
            else
            {
                _logger.LogError($"User {activeUserId} doesn't have permission to reply to a mail sent to {mail.ToUserId}.");
                throw new GraException("Permission Denied");
            }
        }

        public async Task<Mail> MCSendAsync(Mail mail)
        {
            if (mail.ToUserId == null
               || HasPermission(Permission.MailParticipants))
            {
                mail.FromUserId = GetClaimId(ClaimType.UserId);
                mail.IsNew = true;
                mail.IsDeleted = false;
                mail.SiteId = GetClaimId(ClaimType.SiteId);
                return await _mailRepository.AddSaveAsync(mail.FromUserId, mail);
            }
            else
            {
                var userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to send a mail to {mail.ToUserId}.");
                throw new Exception("Permission denied");
            }
        }

        public async Task<Mail> MCSendReplyAsync(Mail mail, int inReplyToId)
        {
            if (mail.ToUserId == null
               || HasPermission(Permission.MailParticipants))
            {
                var inReplyToMail = await _mailRepository.GetByIdAsync(inReplyToId);
                mail.InReplyToId = inReplyToId;
                mail.ThreadId = inReplyToMail.ThreadId ?? inReplyToId;
                mail.FromUserId = GetClaimId(ClaimType.UserId);
                mail.IsNew = true;
                mail.IsDeleted = false;
                mail.SiteId = GetClaimId(ClaimType.SiteId);
                return await _mailRepository.AddSaveAsync(mail.FromUserId, mail);
            }
            else
            {
                var userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to send a mail to {mail.ToUserId}.");
                throw new Exception("Permission denied");
            }
        }

        public async Task RemoveAsync(int mailId)
        {
            var userId = GetClaimId(ClaimType.UserId);
            bool canDeleteAll = HasPermission(Permission.DeleteAnyMail);
            var mail = await _mailRepository.GetByIdAsync(mailId);
            if (mail.FromUserId == userId || mail.ToUserId == userId || canDeleteAll)
            {
                await _mailRepository.RemoveSaveAsync(userId, mailId);
                return;
            }
            _logger.LogError($"User {userId} doesn't have permission remove mail {mailId}.");
            throw new Exception("Permission denied.");
        }
    }
}
