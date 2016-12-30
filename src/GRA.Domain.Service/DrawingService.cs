using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class DrawingService : Abstract.BaseUserService<DrawingService>
    {
        private readonly IDrawingRepository _drawingRepository;
        private readonly IDrawingCriterionRepository _drawingCriterionRepository;
        public DrawingService(ILogger<DrawingService> logger,
            IUserContextProvider userContextProvider,
            IDrawingRepository drawingRepository,
            IDrawingCriterionRepository drawingCriterionRepository) : base(logger, userContextProvider)
        {
            _drawingRepository = Require.IsNotNull(drawingRepository, nameof(drawingRepository));
            _drawingCriterionRepository = Require.IsNotNull(drawingCriterionRepository,
                nameof(drawingCriterionRepository));
        }

        public async Task<DataWithCount<IEnumerable<Drawing>>> GetPaginatedDrawingListAsync(int skip, int take)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing))
            {
                int siteId = GetCurrentSiteId();
                return new DataWithCount<IEnumerable<Drawing>>
                {
                    Data = await _drawingRepository.PageAllAsync(siteId, skip, take),
                    Count = await _drawingRepository.GetCountAsync(siteId)
                };
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to view all drawings.");
                throw new GraException("Permission denied.");
            }

        }

        public async Task<Drawing> GetDetails(int id)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing))
            {
                return await _drawingRepository.GetByIdAsync(id);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to view drawing {id}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<DrawingCriterion>>>
            GetPaginatedCriterionListAsync(int skip, int take)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing))
            {
                int siteId = GetCurrentSiteId();
                return new DataWithCount<IEnumerable<DrawingCriterion>>
                {
                    Data = await _drawingCriterionRepository.PageAllAsync(siteId, skip, take),
                    Count = await _drawingCriterionRepository.GetCountAsync(siteId)
                };
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to view all criteria.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DrawingCriterion> GetCriterionDetails(int id)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing))
            {
                return await _drawingCriterionRepository.GetByIdAsync(id);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to view criterion {id}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DrawingCriterion> AddCriterionAsync(DrawingCriterion criterion)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing))
            {
                criterion.SiteId = GetCurrentSiteId();
                return await _drawingCriterionRepository.AddSaveAsync(authUserId, criterion);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to add a criterion.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DrawingCriterion> EditCriterionAsync(DrawingCriterion criterion)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.PerformDrawing))
            {
                var currentCriterion = await _drawingCriterionRepository.GetByIdAsync(criterion.Id);
                criterion.SiteId = currentCriterion.SiteId;
                return await _drawingCriterionRepository.UpdateSaveAsync(authUserId, criterion);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to edit criterion {criterion.Id}.");
                throw new GraException("Permission denied.");
            }
        }
    }
}
