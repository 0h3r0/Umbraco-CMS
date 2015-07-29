﻿using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Services;
using Umbraco.Web.Cache;

namespace Umbraco.Web.PublishedCache.XmlPublishedCache
{
    /// <summary>
    /// Implements a facade service.
    /// </summary>
    class FacadeService : FacadeServiceBase
    {
        private readonly XmlStore _xmlStore;
        private readonly RoutesCache _routesCache;
        private readonly PublishedContentTypeCache _contentTypeCache;
        private readonly IDomainService _domainService;
        private readonly IMemberService _memberService;
        private readonly IMediaService _mediaService;
        private readonly ICacheProvider _requestCache;

        #region Constructors

        // used in StandaloneBootManager only, should get rid of that one eventually
        internal FacadeService(ServiceContext serviceContext, DatabaseContext databaseContext, ICacheProvider requestCache)
            : this(serviceContext, databaseContext, requestCache, null, false, true)
        { }

        // used in some tests + in WebBootManager
        internal FacadeService(ServiceContext serviceContext, DatabaseContext databaseContext, ICacheProvider requestCache,
            bool testing, bool enableRepositoryEvents)
            : this(serviceContext, databaseContext, requestCache, null, testing, enableRepositoryEvents)
        { }

        // used in some tests
        internal FacadeService(ServiceContext serviceContext, DatabaseContext databaseContext, ICacheProvider requestCache,
            PublishedContentTypeCache contentTypeCache, bool testing, bool enableRepositoryEvents)
        {
            _routesCache = new RoutesCache();
            _contentTypeCache = contentTypeCache
                ?? new PublishedContentTypeCache(serviceContext.ContentTypeService, serviceContext.MediaTypeService, serviceContext.MemberTypeService);

            _xmlStore = new XmlStore(serviceContext, databaseContext, _routesCache, _contentTypeCache, testing, enableRepositoryEvents);

            _domainService = serviceContext.DomainService;
            _memberService = serviceContext.MemberService;
            _mediaService = serviceContext.MediaService;
            _requestCache = requestCache;
        }

        public override void Dispose()
        {
            _xmlStore.Dispose();
        }

        #endregion

        #region PublishedCachesService Caches

        public override IFacade CreateFacade(string previewToken)
        {
            // use _requestCache to store recursive properties lookup, etc. both in content
            // and media cache. Life span should be the current request. Or, ideally
            // the current caches, but that would mean creating an extra cache (StaticCache
            // probably) so better use RequestCache.

            var domainCache = new DomainCache(_domainService);

            return new Facade(
                new PublishedContentCache(_xmlStore, domainCache, _requestCache, _contentTypeCache, _routesCache, previewToken),
                new PublishedMediaCache(_xmlStore, _mediaService, _requestCache, _contentTypeCache),
                new PublishedMemberCache(_xmlStore, _requestCache, _memberService, _contentTypeCache),
                domainCache);
        }

        #endregion

        #region PublishedCachesService Preview

        public override string EnterPreview(IUser user, int contentId)
        {
            var previewContent = new PreviewContent(_xmlStore, user.Id);
            previewContent.CreatePreviewSet(contentId, true); // preview branch below that content
            return previewContent.Token;
            //previewContent.ActivatePreviewCookie();
        }

        public override void RefreshPreview(string previewToken, int contentId)
        {
            if (previewToken.IsNullOrWhiteSpace()) return;
            var previewContent = new PreviewContent(_xmlStore, previewToken);
            previewContent.CreatePreviewSet(contentId, true); // preview branch below that content
        }

        public override void ExitPreview(string previewToken)
        {
            if (previewToken.IsNullOrWhiteSpace()) return;
            var previewContent = new PreviewContent(_xmlStore, previewToken);
            previewContent.ClearPreviewSet();
        }

        #endregion

        #region Xml specific

        /// <summary>
        /// Gets the underlying XML store.
        /// </summary>
        public XmlStore XmlStore { get { return _xmlStore; } }

        /// <summary>
        /// Gets the underlying RoutesCache.
        /// </summary>
        public RoutesCache RoutesCache { get { return _routesCache; } }

        public bool VerifyContentAndPreviewXml()
        {
            return XmlStore.VerifyContentAndPreviewXml();
        }

        public void RebuildContentAndPreviewXml()
        {
            XmlStore.RebuildContentAndPreviewXml();
        }

        public bool VerifyMediaXml()
        {
            return XmlStore.VerifyMediaXml();
        }

        public void RebuildMediaXml()
        {
            XmlStore.RebuildMediaXml();
        }

        public bool VerifyMemberXml()
        {
            return XmlStore.VerifyMemberXml();
        }

        public void RebuildMemberXml()
        {
            XmlStore.RebuildMemberXml();
        }

        #endregion

        #region Change management

        public override void Notify(ContentCacheRefresher.JsonPayload[] payloads, out bool draftChanged, out bool publishedChanged)
        {
            _xmlStore.Notify(payloads, out draftChanged, out publishedChanged);
        }

        public override void Notify(MediaCacheRefresher.JsonPayload[] payloads, out bool anythingChanged)
        {
            foreach (var payload in payloads)
                PublishedMediaCache.ClearCache(payload.Id);

            anythingChanged = true;
        }

        public override void Notify(ContentTypeCacheRefresher.JsonPayload[] payloads)
        {
            _xmlStore.Notify(payloads);
            if (payloads.Any(x => x.ItemType == typeof (IContentType).Name))
                _routesCache.Clear();
        }

        public override void Notify(DataTypeCacheRefresher.JsonPayload[] payloads)
        {
            _xmlStore.Notify(payloads);
        }

        public override void Notify(DomainCacheRefresher.JsonPayload[] payloads)
        {
            _routesCache.Clear();
        }

        #endregion
    }
}
