﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using GRA.Domain.Service.Abstract;
using GRA.Domain.Repository;
using GRA.Domain.Model;

namespace GRA.Domain.Service
{
    public class DynamicAvatarService : BaseUserService<DynamicAvatarService>
    {
        private readonly IDynamicAvatarElementRepository _dynamicAvatarElementRepository;
        private readonly IDynamicAvatarLayerRepository _dynamicAvatarLayerRepository;
        public DynamicAvatarService(ILogger<DynamicAvatarService> logger,
            IUserContextProvider userContextProvider,
            IDynamicAvatarElementRepository dynamicAvatarElementRepository,
            IDynamicAvatarLayerRepository dynamicAvatarLayerRepository)
            : base(logger, userContextProvider)
        {
            _dynamicAvatarElementRepository = Require.IsNotNull(dynamicAvatarElementRepository,
                nameof(dynamicAvatarElementRepository));
            _dynamicAvatarLayerRepository = Require.IsNotNull(dynamicAvatarLayerRepository,
                nameof(dynamicAvatarLayerRepository));
        }
        public async Task<Dictionary<int, int>> GetDefaultAvatarAsync()
        {
            var avatarParts = new Dictionary<int, int>();
            var layerIds = await _dynamicAvatarLayerRepository.GetLayerIdsAsync();
            foreach (int layerId in layerIds)
            {
                var elementId = await _dynamicAvatarElementRepository.GetIdByLayerIdAsync(layerId);
                avatarParts.Add(layerId, elementId);
            }
            return avatarParts;
        }

        public async Task<Dictionary<int, int>> ReturnValidated(IEnumerable<int> elementIds)
        {
            var avatarParts = new Dictionary<int, int>();
            var layerIds = await _dynamicAvatarLayerRepository.GetLayerIdsAsync();

            if (layerIds.Count() != elementIds.Count())
            {
                return null;
            }

            var layerElements = layerIds
                .Zip(elementIds, (LayerId, ElementId) => new { LayerId, ElementId })
                .ToDictionary(_ => _.LayerId, _ => _.ElementId);

            foreach (var layerId in layerElements.Keys)
            {
                if (!await _dynamicAvatarElementRepository.ExistsAsync(layerId, layerElements[layerId]))
                {
                    return null;
                }
            }
            return layerElements;
        }
    }
}
