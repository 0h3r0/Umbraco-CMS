using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Services.Protocols;
using System.Xml;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.Sync;
using umbraco.BusinessLogic;
using umbraco.interfaces;

namespace Umbraco.Web.Cache
{
    //public class CacheUpdatedEventArgs : EventArgs
    //{
        
    //}

    /// <summary>
    /// DistributedCache is used to invalidate cache throughout the application which also takes in to account load balancing environments automatically
    /// </summary>
    /// <remarks>
    /// Distributing calls to all registered load balanced servers, ensuring that content are synced and cached on all servers.
    /// Dispatcher is exendable, so 3rd party services can easily be integrated into the workflow, using the interfaces.ICacheRefresher interface.
    /// 
    /// Dispatcher can refresh/remove content, templates and macros.
    /// </remarks>
    public sealed class DistributedCache
    {

        #region Public constants/Ids

        public const string ApplicationTreeCacheRefresherId = "0AC6C028-9860-4EA4-958D-14D39F45886E";
        public const string ApplicationCacheRefresherId = "B15F34A1-BC1D-4F8B-8369-3222728AB4C8";
        public const string TemplateRefresherId = "DD12B6A0-14B9-46e8-8800-C154F74047C8";
        public const string PageCacheRefresherId = "27AB3022-3DFA-47b6-9119-5945BC88FD66";
        public const string UnpublishedPageCacheRefresherId = "55698352-DFC5-4DBE-96BD-A4A0F6F77145";
        public const string MemberCacheRefresherId = "E285DF34-ACDC-4226-AE32-C0CB5CF388DA";
        public const string MemberGroupCacheRefresherId = "187F236B-BD21-4C85-8A7C-29FBA3D6C00C";
        public const string MediaCacheRefresherId = "B29286DD-2D40-4DDB-B325-681226589FEC";
        public const string MacroCacheRefresherId = "7B1E683C-5F34-43dd-803D-9699EA1E98CA";
        public const string UserCacheRefresherId = "E057AF6D-2EE6-41F4-8045-3694010F0AA6";
        public const string UserPermissionsCacheRefresherId = "840AB9C5-5C0B-48DB-A77E-29FE4B80CD3A";
        public const string UserTypeCacheRefresherId = "7E707E21-0195-4522-9A3C-658CC1761BD4";
        public const string ContentTypeCacheRefresherId = "6902E22C-9C10-483C-91F3-66B7CAE9E2F5";
        public const string LanguageCacheRefresherId = "3E0F95D8-0BE5-44B8-8394-2B8750B62654";
        public const string DomainCacheRefresherId = "11290A79-4B57-4C99-AD72-7748A3CF38AF";
        public const string StylesheetCacheRefresherId = "E0633648-0DEB-44AE-9A48-75C3A55CB670";
        public const string StylesheetPropertyCacheRefresherId = "2BC7A3A4-6EB1-4FBC-BAA3-C9E7B6D36D38";
        public const string DataTypeCacheRefresherId = "35B16C25-A17E-45D7-BC8F-EDAB1DCC28D2";
        public const string DictionaryCacheRefresherId = "D1D7E227-F817-4816-BFE9-6C39B6152884";
        public const string PublicAccessCacheRefresherId = "1DB08769-B104-4F8B-850E-169CAC1DF2EC";
        public const string ContentCacheRefresherId = "900A4FBE-DF3C-41E6-BB77-BE896CD158EA";

        // note - use these and stop creating GUIDs all the time
        public static readonly Guid ApplicationTreeCacheRefresherGuid = new Guid(ApplicationTreeCacheRefresherId);
        public static readonly Guid ApplicationCacheRefresherGuid = new Guid(ApplicationCacheRefresherId);
        public static readonly Guid TemplateRefresherGuid = new Guid(TemplateRefresherId);
        public static readonly Guid PageCacheRefresherGuid = new Guid(PageCacheRefresherId);
        public static readonly Guid UnpublishedPageCacheRefresherGuid = new Guid(UnpublishedPageCacheRefresherId);
        public static readonly Guid MemberCacheRefresherGuid = new Guid(MemberCacheRefresherId);
        public static readonly Guid MemberGroupCacheRefresherGuid = new Guid(MemberGroupCacheRefresherId);
        public static readonly Guid MediaCacheRefresherGuid = new Guid(MediaCacheRefresherId);
        public static readonly Guid MacroCacheRefresherGuid = new Guid(MacroCacheRefresherId);
        public static readonly Guid UserCacheRefresherGuid = new Guid(UserCacheRefresherId);
        public static readonly Guid UserPermissionsCacheRefresherGuid = new Guid(UserPermissionsCacheRefresherId);
        public static readonly Guid UserTypeCacheRefresherGuid = new Guid(UserTypeCacheRefresherId);
        public static readonly Guid ContentTypeCacheRefresherGuid = new Guid(ContentTypeCacheRefresherId);
        public static readonly Guid LanguageCacheRefresherGuid = new Guid(LanguageCacheRefresherId);
        public static readonly Guid DomainCacheRefresherGuid = new Guid(DomainCacheRefresherId);
        public static readonly Guid StylesheetCacheRefresherGuid = new Guid(StylesheetCacheRefresherId);
        public static readonly Guid StylesheetPropertyCacheRefresherGuid = new Guid(StylesheetPropertyCacheRefresherId);
        public static readonly Guid DataTypeCacheRefresherGuid = new Guid(DataTypeCacheRefresherId);
        public static readonly Guid DictionaryCacheRefresherGuid = new Guid(DictionaryCacheRefresherId);
        public static readonly Guid PublicAccessCacheRefresherGuid = new Guid(PublicAccessCacheRefresherId);
        public static readonly Guid ContentCacheRefresherGuid = new Guid(ContentCacheRefresherId);

        #endregion

        private static readonly DistributedCache InstanceObject = new DistributedCache();

        /// <summary>
        /// Constructor
        /// </summary>
        private DistributedCache()
        {                 
        }
        
        /// <summary>
        /// Singleton
        /// </summary>
        /// <returns></returns>
        public static DistributedCache Instance
        {
            get
            {
                return InstanceObject;    
            }
        }

        /// <summary>
        /// Sends a request to all registered load-balanced servers to refresh node with the specified Id
        /// using the specified ICacheRefresher with the guid factoryGuid.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factoryGuid"></param>
        /// <param name="getNumericId">The callback method to retrieve the ID from an instance</param>
        /// <param name="instances">The instances containing Ids</param>
        /// <remarks>
        /// This method is much better for performance because it does not need to re-lookup an object instance
        /// </remarks>
        public void Refresh<T>(Guid factoryGuid, Func<T, int> getNumericId, params T[] instances)
        {
            if (factoryGuid == Guid.Empty || instances.Length == 0 || getNumericId == null) return;

            ServerMessengerResolver.Current.Messenger.PerformRefresh<T>(
                ServerRegistrarResolver.Current.Registrar.Registrations,
                GetRefresherById(factoryGuid),
                getNumericId,
                instances);
        }

        /// <summary>
        /// Sends a request to all registered load-balanced servers to refresh node with the specified Id
        /// using the specified ICacheRefresher with the guid factoryGuid.
        /// </summary>
        /// <param name="factoryGuid">The unique identifier of the ICacheRefresher used to refresh the node.</param>
        /// <param name="id">The id of the node.</param>
        public void Refresh(Guid factoryGuid, int id)
        {
            if (factoryGuid == Guid.Empty || id == default(int)) return;

            ServerMessengerResolver.Current.Messenger.PerformRefresh(
                ServerRegistrarResolver.Current.Registrar.Registrations, 
                GetRefresherById(factoryGuid), 
                id);
        }

        /// <summary>
        /// Sends a request to all registered load-balanced servers to refresh the node with the specified guid
        /// using the specified ICacheRefresher with the guid factoryGuid.
        /// </summary>
        /// <param name="factoryGuid">The unique identifier of the ICacheRefresher used to refresh the node.</param>
        /// <param name="id">The guid of the node.</param>
        public void Refresh(Guid factoryGuid, Guid id)
        {
            if (factoryGuid == Guid.Empty || id == Guid.Empty) return;

            ServerMessengerResolver.Current.Messenger.PerformRefresh(
                ServerRegistrarResolver.Current.Registrar.Registrations,
                GetRefresherById(factoryGuid),
                id);
        }

        /// <summary>
        /// Sends a request to all registered load-balanced servers to refresh data based on the custom json payload
        /// using the specified ICacheRefresher with the guid factoryGuid.
        /// </summary>
        /// <param name="factoryGuid"></param>
        /// <param name="jsonPayload"></param>
        public void RefreshByJson(Guid factoryGuid, string jsonPayload)
        {
            if (factoryGuid == Guid.Empty || jsonPayload.IsNullOrWhiteSpace()) return;

            ServerMessengerResolver.Current.Messenger.PerformRefresh(
                ServerRegistrarResolver.Current.Registrar.Registrations,
                GetRefresherById(factoryGuid),
                jsonPayload);
        }

        /// <summary>
        /// Sends a request to all registered load-balanced servers to refresh all nodes
        /// using the specified ICacheRefresher with the guid factoryGuid.
        /// </summary>
        /// <param name="factoryGuid">The unique identifier.</param>
        public void RefreshAll(Guid factoryGuid)
        {
            if (factoryGuid == Guid.Empty) return;

            RefreshAll(factoryGuid, true);
        }

        /// <summary>
        /// Sends a request to all registered load-balanced servers to refresh all nodes
        /// using the specified ICacheRefresher with the guid factoryGuid.
        /// </summary>
        /// <param name="factoryGuid">The unique identifier.</param>
        /// <param name="allServers">
        /// If true will send the request out to all registered LB servers, if false will only execute the current server
        /// </param>
        public void RefreshAll(Guid factoryGuid, bool allServers)
        {
            if (factoryGuid == Guid.Empty) return;

            ServerMessengerResolver.Current.Messenger.PerformRefreshAll(
                allServers 
                    ? ServerRegistrarResolver.Current.Registrar.Registrations
                    : Enumerable.Empty<IServerAddress>(), //this ensures it will only execute against the current server
                GetRefresherById(factoryGuid));
        }

        /// <summary>
        /// Sends a request to all registered load-balanced servers to remove the node with the specified id
        /// using the specified ICacheRefresher with the guid factoryGuid.
        /// </summary>
        /// <param name="factoryGuid">The unique identifier.</param>
        /// <param name="id">The id.</param>
        public void Remove(Guid factoryGuid, int id)
        {
            if (factoryGuid == Guid.Empty || id == default(int)) return;

            ServerMessengerResolver.Current.Messenger.PerformRemove(
                ServerRegistrarResolver.Current.Registrar.Registrations,
                GetRefresherById(factoryGuid),
                id);
        }
        
        /// <summary>
        /// Sends a request to all registered load-balanced servers to remove the node specified
        /// using the specified ICacheRefresher with the guid factoryGuid.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factoryGuid"></param>
        /// <param name="getNumericId"></param>
        /// <param name="instances"></param>
        public void Remove<T>(Guid factoryGuid, Func<T, int> getNumericId, params T[] instances)
        {
            ServerMessengerResolver.Current.Messenger.PerformRemove<T>(
                ServerRegistrarResolver.Current.Registrar.Registrations,
                GetRefresherById(factoryGuid),
                getNumericId,
                instances);
        }       

        private static ICacheRefresher GetRefresherById(Guid uniqueIdentifier)
        {
            var refresher = CacheRefreshersResolver.Current.GetById(uniqueIdentifier);
            if (refresher == null)
                throw new ArgumentException("Not a registered cache refresher UID: {0}".FormatWith(uniqueIdentifier));
            return refresher;
        }

    }
}
