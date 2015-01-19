﻿using System;
using Umbraco.Core;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Services;
using umbraco.interfaces;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.PublishedCache.XmlPublishedCache;

namespace Umbraco.Web.Strategies.Migrations
{
    /// <summary>
    /// This will execute after upgrading to rebuild the xml cache
    /// </summary>
    /// <remarks>
    /// This cannot execute as part of a db migration since we need access to the services/repos.
    /// 
    /// This will execute for specific versions - 
    /// 
    /// * If current is less than or equal to 7.0.0
    /// </remarks>
    public class RebuildMediaXmlCacheAfterUpgrade : IApplicationStartupHandler
    {
        public RebuildMediaXmlCacheAfterUpgrade()
        {
            MigrationRunner.Migrated += MigrationRunner_Migrated;
        }

        void MigrationRunner_Migrated(MigrationRunner sender, Core.Events.MigrationEventArgs e)
        {
            var target70 = new Version(7, 0, 0);

            if (e.ConfiguredVersion <= target70)
            {
                // maintain - for backward compatibility?
                //var mediasvc = (MediaService)ApplicationContext.Current.Services.MediaService;
                //mediasvc.RebuildMediaXml();
                var svc = PublishedCachesServiceResolver.Current.Service as PublishedCachesService;
                if (svc != null)
                    svc.RebuildMediaXml();
            }
        }
    }
}