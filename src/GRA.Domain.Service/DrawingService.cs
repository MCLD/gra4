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
            return new DataWithCount<IEnumerable<Drawing>>
            {
                Data = new List<Drawing>(),
                Count = 0
            };
        }

        public async Task<DataWithCount<IEnumerable<DrawingCriterion>>> 
            GetPaginatedCriterionListAsync(int skip, int take)
        {
            return new DataWithCount<IEnumerable<DrawingCriterion>>
            {
                Data = new List<DrawingCriterion>(),
                Count = 0
            };
        }
    }
}
