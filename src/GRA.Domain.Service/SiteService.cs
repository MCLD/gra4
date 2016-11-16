
namespace GRA.Domain.Service
{
    using System;
    using System.Collections.Generic;

    using GRA.Domain.Model;
    using GRA.Domain.Repository;

    using Microsoft.Extensions.Logging;

    public class SiteService
    {
        private readonly ILogger<SiteService> logger;

        private readonly ISiteRepository siteRepository;

        public SiteService(ILogger<SiteService> logger, ISiteRepository siteRepository)
        {
            if (logger == null)
            {
                // Use nameof so strings don't get out of sync over time
                throw new ArgumentNullException(nameof(logger));
            }
            this.logger = logger;
            if (siteRepository == null)
            {
                throw new ArgumentNullException(nameof(siteRepository));
            }
            this.siteRepository = siteRepository;
        }

        public IEnumerable<Site> GetSitePaths()
        {
            
            return siteRepository.GetAll();
        }

        public void InitialSetup(User user)
        {
            // todo verify paritipcant is admin?

            // create default site
            siteRepository.Add(0, new Model.Site
            {
                Name = "Default Site",
                Path = "default"
            });
        }
    }
}