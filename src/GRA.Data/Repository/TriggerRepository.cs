using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;
using System;

namespace GRA.Data.Repository
{
    public class TriggerRepository : AuditingRepository<Model.Trigger, Trigger>, ITriggerRepository
    {
        public TriggerRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<TriggerRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task AddTriggerActivationAsync(int userId, int triggerId)
        {
            _context.UserTriggers.Add(new Model.UserTrigger
            {
                UserId = userId,
                TriggerId = triggerId,
                CreatedAt = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }

        // honors site id, skip, and take
        public async Task<int> CountAsync(Filter filter)
        {
            return await DbSet.AsNoTracking().Where(_ => _.SiteId == filter.SiteId).CountAsync();
        }

        public async Task<ICollection<Trigger>> GetTriggersAsync(int userId)
        {
            // get user details for filtering triggers
            var user = await _context.Users
                .AsNoTracking()
                .Where(_ => _.Id == userId)
                .SingleOrDefaultAsync();

            // already earned triggers to exclude
            var alreadyEarnedTriggerIds = _context.UserTriggers
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .Select(_ => _.TriggerId);

            // monster trigger query
            var triggers = await DbSet
                .AsNoTracking()
                .Include("RequiredBadges")
                .Include("RequiredChallenges")
                .Where(_ => _.SiteId == user.SiteId
                    && !alreadyEarnedTriggerIds.Contains(_.Id)
                    && (_.LimitToSystemId == null || _.LimitToSystemId == user.SystemId)
                    && (_.LimitToBranchId == null || _.LimitToBranchId == user.BranchId)
                    && (_.LimitToProgramId == null || _.LimitToProgramId == user.ProgramId)
                    && (_.Points == 0 || _.Points <= user.PointsEarned)
                    && string.IsNullOrEmpty(_.SecretCode))
                .OrderBy(_ => _.Points)
                .ThenBy(_ => _.AwardPoints)
                .ToListAsync();

            // create a list of triggers to remove based on badge and challenge earnings
            var itemsToRemove = new List<Model.Trigger>();

            // get a list of triggers that fire based on badge earnings
            var badgeTriggers = triggers
                .Where(_ => _.RequiredBadges != null && _.RequiredBadges.Count > 0);

            if (badgeTriggers.Count() > 0) {
                // get the user's badges
                var userBadgeIds = _context.UserBadges
                    .AsNoTracking()
                    .Where(_ => _.UserId == userId)
                    .Select(_ => _.BadgeId);

                foreach (var eligibleTrigger in badgeTriggers)
                {
                    var requiredBadges = eligibleTrigger.RequiredBadges.Select(_ => _.BadgeId);
                    // check for badge eligibility
                    if (requiredBadges.Except(userBadgeIds).Any())
                    {
                        // requires badges the user doesn't have
                        itemsToRemove.Add(eligibleTrigger);
                    }
                }
            }

            // get a list of triggers that fire based on challenge earnings
            var challengeTriggers = triggers
                .Where(_ => _.RequiredChallenges != null && _.RequiredChallenges.Count > 0);

            if(challengeTriggers.Count() > 0)
            {
                // user's challenges
                var userChallengeIds = _context.UserLogs
                    .AsNoTracking()
                    .Where(_ => _.UserId == userId && _.ChallengeId != null)
                    .Select(_ => _.ChallengeId.Value);

                foreach(var eligibleTrigger in challengeTriggers)
                {
                    var requiredChallenges = eligibleTrigger.RequiredChallenges.Select(_ => _.ChallengeId);
                    if(requiredChallenges.Except(userChallengeIds).Any())
                    {
                        // requires challenges the user doesn't have
                        itemsToRemove.Add(eligibleTrigger);
                    }
                }
            }
            
            // return all the triggers that should be awarded to the user
            return _mapper.Map<ICollection<Trigger>>(triggers.Except(itemsToRemove));
        }

        // honors site id, skip, and take
        public async Task<ICollection<Trigger>> PageAsync(Filter filter)
        {
            return await ApplySiteIdPagination(filter)
                .ProjectTo<Trigger>()
                .ToListAsync();
        }

        private IQueryable<Model.Trigger> ApplySiteIdPagination(Filter filter)
        {
            var filteredData = DbSet.AsNoTracking().Where(_ => _.SiteId == filter.SiteId);
            return ApplyPagination(filteredData, filter);
        }
    }
}
