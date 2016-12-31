﻿using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IDrawingRepository : IRepository<Drawing>
    {
        Task<IEnumerable<Drawing>> PageAllAsync(int siteId, int skip, int take);
        Task<int> GetCountAsync(int siteId);
        Task<Drawing> GetByIdAsync(int id, int skip, int take);
        Task<int> GetWinnerCountAsync(int id);
        Task RemoveWinnerAsync(int drawingId, int userId);
        Task RedeemWinnerAsync(int drawingId, int userId);
        Task<IEnumerable<DrawingWinner>> PageUserAsync(int userId, int skip, int take);
        Task<int> GetUserWinCountAsync(int userId);
    }
}
