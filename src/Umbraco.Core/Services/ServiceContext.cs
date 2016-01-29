using System;
using System.IO;
using System.Linq;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.UnitOfWork;
using Umbraco.Core.Events;

namespace Umbraco.Core.Services
{
    /// <summary>
    /// These are used currently to return the temporary 'operation' interfaces for services
    /// which are used to return a status from operational methods so we can determine if things are
    /// cancelled, etc...
    /// 
    /// These will be obsoleted in v8 since all real services methods will be changed to have the correct result.
    /// </summary>
    public static class ServiceWithResultExtensions
    {
        public static IContentServiceOperations WithResult(this IContentService contentService)
        {
            return (IContentServiceOperations)contentService;
        }
        public static IMediaServiceOperations WithResult(this IMediaService mediaService)
        {
            return (IMediaServiceOperations)mediaService;
        }
    }

    /// <summary>
    /// The Umbraco ServiceContext, which provides access to the following services:
    /// <see cref="IContentService"/>, <see cref="IContentTypeService"/>, <see cref="IDataTypeService"/>,
    /// <see cref="IFileService"/>, <see cref="ILocalizationService"/> and <see cref="IMediaService"/>.
    /// </summary>
    public class ServiceContext
    {
        private Lazy<IMigrationEntryService> _migrationEntryService; 
        private Lazy<IPublicAccessService> _publicAccessService; 
        private Lazy<ITaskService> _taskService; 
        private Lazy<IDomainService> _domainService; 
        private Lazy<IAuditService> _auditService; 
        private Lazy<ILocalizedTextService> _localizedTextService;
        private Lazy<ITagService> _tagService;
        private Lazy<Tuple<IContentService, IContentTypeService>> _contentServices;
        private Lazy<IUserService> _userService;
        private Lazy<Tuple<IMemberService, IMemberTypeService>> _memberServices;
        private Lazy<Tuple<IMediaService, IMediaTypeService>> _mediaServices;
        private Lazy<IDataTypeService> _dataTypeService;
        private Lazy<IFileService> _fileService;
        private Lazy<ILocalizationService> _localizationService;
        private Lazy<IPackagingService> _packagingService;
        private Lazy<IServerRegistrationService> _serverRegistrationService;
        private Lazy<IEntityService> _entityService;
        private Lazy<IRelationService> _relationService;
        private Lazy<IApplicationTreeService> _treeService;
        private Lazy<ISectionService> _sectionService;
        private Lazy<IMacroService> _macroService;
        private Lazy<IMemberGroupService> _memberGroupService;
        private Lazy<INotificationService> _notificationService;
        private Lazy<IExternalLoginService> _externalLoginService;

        /// <summary>
        /// public ctor - will generally just be used for unit testing all items are optional and if not specified, the defaults will be used
        /// </summary>
        /// <param name="contentServices"></param>
        /// <param name="mediaServices"></param>
        /// <param name="dataTypeService"></param>
        /// <param name="fileService"></param>
        /// <param name="localizationService"></param>
        /// <param name="packagingService"></param>
        /// <param name="entityService"></param>
        /// <param name="relationService"></param>
        /// <param name="memberGroupService"></param>
        /// <param name="memberServices"></param>
        /// <param name="userService"></param>
        /// <param name="sectionService"></param>
        /// <param name="treeService"></param>
        /// <param name="tagService"></param>
        /// <param name="notificationService"></param>
        /// <param name="localizedTextService"></param>
        /// <param name="auditService"></param>
        /// <param name="domainService"></param>
        /// <param name="taskService"></param>
        /// <param name="macroService"></param>
        /// <param name="publicAccessService"></param>
        /// <param name="externalLoginService"></param>
        /// <param name="migrationEntryService"></param>
        public ServiceContext(
            Tuple<IContentService, IContentTypeService> contentServices = null,
            Tuple<IMediaService, IMediaTypeService> mediaServices = null,
            Tuple<IMemberService, IMemberTypeService> memberServices = null,
            IDataTypeService dataTypeService = null,
            IFileService fileService = null,
            ILocalizationService localizationService = null,
            IPackagingService packagingService = null,
            IEntityService entityService = null,
            IRelationService relationService = null,
            IMemberGroupService memberGroupService = null,
            IUserService userService = null,
            ISectionService sectionService = null,
            IApplicationTreeService treeService = null,
            ITagService tagService = null,
            INotificationService notificationService = null,
            ILocalizedTextService localizedTextService = null,
            IAuditService auditService = null,
            IDomainService domainService = null,
            ITaskService taskService = null,
            IMacroService macroService = null,
            IPublicAccessService publicAccessService = null,
            IExternalLoginService externalLoginService = null,
            IMigrationEntryService migrationEntryService = null)
        {
            if (migrationEntryService != null) _migrationEntryService = new Lazy<IMigrationEntryService>(() => migrationEntryService);
            if (externalLoginService != null) _externalLoginService = new Lazy<IExternalLoginService>(() => externalLoginService);
            if (auditService != null) _auditService = new Lazy<IAuditService>(() => auditService);
            if (localizedTextService != null) _localizedTextService = new Lazy<ILocalizedTextService>(() => localizedTextService);
            if (tagService != null) _tagService = new Lazy<ITagService>(() => tagService);
            if (contentServices != null) _contentServices = new Lazy<Tuple<IContentService, IContentTypeService>>(() => contentServices);
            if (mediaServices != null) _mediaServices = new Lazy<Tuple<IMediaService, IMediaTypeService>>(() => mediaServices);
            if (dataTypeService != null) _dataTypeService = new Lazy<IDataTypeService>(() => dataTypeService);
            if (fileService != null) _fileService = new Lazy<IFileService>(() => fileService);
            if (localizationService != null) _localizationService = new Lazy<ILocalizationService>(() => localizationService);
            if (packagingService != null) _packagingService = new Lazy<IPackagingService>(() => packagingService);
            if (entityService != null) _entityService = new Lazy<IEntityService>(() => entityService);
            if (relationService != null) _relationService = new Lazy<IRelationService>(() => relationService);
            if (sectionService != null) _sectionService = new Lazy<ISectionService>(() => sectionService);
            if (memberGroupService != null) _memberGroupService = new Lazy<IMemberGroupService>(() => memberGroupService);
            if (treeService != null) _treeService = new Lazy<IApplicationTreeService>(() => treeService);
            if (memberServices != null) _memberServices = new Lazy<Tuple<IMemberService, IMemberTypeService>>(() => memberServices);
            if (userService != null) _userService = new Lazy<IUserService>(() => userService);
            if (notificationService != null) _notificationService = new Lazy<INotificationService>(() => notificationService);
            if (domainService != null) _domainService = new Lazy<IDomainService>(() => domainService);
            if (taskService != null) _taskService = new Lazy<ITaskService>(() => taskService);
            if (macroService != null) _macroService = new Lazy<IMacroService>(() => macroService);
            if (publicAccessService != null) _publicAccessService = new Lazy<IPublicAccessService>(() => publicAccessService);
        }

        /// <summary>
        /// Creates a service context with a RepositoryFactory which is used to construct Services
        /// </summary>
        /// <param name="repositoryFactory"></param>
        /// <param name="dbUnitOfWorkProvider"></param>
        /// <param name="fileUnitOfWorkProvider"></param>
        /// <param name="cache"></param>
        /// <param name="logger"></param>
        /// <param name="eventMessagesFactory"></param>
        public ServiceContext(
            RepositoryFactory repositoryFactory,
            IDatabaseUnitOfWorkProvider dbUnitOfWorkProvider, 
            IUnitOfWorkProvider fileUnitOfWorkProvider,
            CacheHelper cache, 
            ILogger logger,
            IEventMessagesFactory eventMessagesFactory)
        {
            if (repositoryFactory == null) throw new ArgumentNullException("repositoryFactory");
            if (dbUnitOfWorkProvider == null) throw new ArgumentNullException("dbUnitOfWorkProvider");
            if (fileUnitOfWorkProvider == null) throw new ArgumentNullException("fileUnitOfWorkProvider");
            if (cache == null) throw new ArgumentNullException("cache");
            if (logger == null) throw new ArgumentNullException("logger");
            if (eventMessagesFactory == null) throw new ArgumentNullException("eventMessagesFactory");

            BuildServiceCache(dbUnitOfWorkProvider, fileUnitOfWorkProvider, cache,
                              repositoryFactory,
                              logger, eventMessagesFactory);
        }

        /// <summary>
        /// Builds the various services
        /// </summary>
        private void BuildServiceCache(
            IDatabaseUnitOfWorkProvider dbUnitOfWorkProvider,
            IUnitOfWorkProvider fileUnitOfWorkProvider,
            CacheHelper cache,
            RepositoryFactory repositoryFactory,
            ILogger logger,
            IEventMessagesFactory eventMessagesFactory)
        {
            var provider = dbUnitOfWorkProvider;
            var fileProvider = fileUnitOfWorkProvider;

            if (_migrationEntryService == null)
                _migrationEntryService = new Lazy<IMigrationEntryService>(() => new MigrationEntryService(provider, repositoryFactory, logger, eventMessagesFactory));

            if (_externalLoginService == null)
                _externalLoginService = new Lazy<IExternalLoginService>(() => new ExternalLoginService(provider, repositoryFactory, logger, eventMessagesFactory));

            if (_publicAccessService == null)
                _publicAccessService = new Lazy<IPublicAccessService>(() => new PublicAccessService(provider, repositoryFactory, logger, eventMessagesFactory));

            if (_taskService == null)
                _taskService = new Lazy<ITaskService>(() => new TaskService(provider, repositoryFactory, logger, eventMessagesFactory));

            if (_domainService == null)
                _domainService = new Lazy<IDomainService>(() => new DomainService(provider, repositoryFactory, logger, eventMessagesFactory));

            if (_auditService == null)
                _auditService = new Lazy<IAuditService>(() => new AuditService(provider, repositoryFactory, logger, eventMessagesFactory));

            if (_localizedTextService == null)
            {
                
                _localizedTextService = new Lazy<ILocalizedTextService>(() => new LocalizedTextService(
                    new Lazy<LocalizedTextServiceFileSources>(() =>
                    {
                        var mainLangFolder = new DirectoryInfo(IOHelper.MapPath(SystemDirectories.Umbraco + "/config/lang/"));
                        var appPlugins = new DirectoryInfo(IOHelper.MapPath(SystemDirectories.AppPlugins));
                        var configLangFolder = new DirectoryInfo(IOHelper.MapPath(SystemDirectories.Config + "/lang/"));

                        var pluginLangFolders = appPlugins.Exists == false
                            ? Enumerable.Empty<LocalizedTextServiceSupplementaryFileSource>()
                            : appPlugins.GetDirectories()
                                .SelectMany(x => x.GetDirectories("Lang"))
                                .SelectMany(x => x.GetFiles("*.xml", SearchOption.TopDirectoryOnly))
                                .Where(x => Path.GetFileNameWithoutExtension(x.FullName).Length == 5)
                                .Select(x => new LocalizedTextServiceSupplementaryFileSource(x, false));

                        //user defined langs that overwrite the default, these should not be used by plugin creators
                        var userLangFolders = configLangFolder.Exists == false
                            ? Enumerable.Empty<LocalizedTextServiceSupplementaryFileSource>()
                            : configLangFolder
                                .GetFiles("*.user.xml", SearchOption.TopDirectoryOnly)
                                .Where(x => Path.GetFileNameWithoutExtension(x.FullName).Length == 10)
                                .Select(x => new LocalizedTextServiceSupplementaryFileSource(x, true));

                        return new LocalizedTextServiceFileSources(
                            logger,
                            cache.RuntimeCache,
                            mainLangFolder,
                            pluginLangFolders.Concat(userLangFolders));

                    }),
                    logger));
            }
                

            if (_notificationService == null)
                _notificationService = new Lazy<INotificationService>(() => new NotificationService(provider, _userService.Value, ContentService, logger));

            if (_serverRegistrationService == null)
                _serverRegistrationService = new Lazy<IServerRegistrationService>(() => new ServerRegistrationService(provider, repositoryFactory, logger, eventMessagesFactory));

            if (_userService == null)
                _userService = new Lazy<IUserService>(() => new UserService(provider, repositoryFactory, logger, eventMessagesFactory));

            if (_memberServices == null)
                _memberServices = new Lazy<Tuple<IMemberService, IMemberTypeService>>(() =>
                {
                    // need this before both services cross-reference each other
                    var memberService = new MemberService(provider, repositoryFactory, logger, eventMessagesFactory, _memberGroupService.Value, _dataTypeService.Value);
                    var memberTypeService = new MemberTypeService(provider, repositoryFactory, logger, eventMessagesFactory);
                    memberService.MemberTypeService = memberTypeService;
                    memberTypeService.MemberService = memberService;
                    return Tuple.Create((IMemberService) memberService, (IMemberTypeService) memberTypeService);
                });

            // that one is distinct from the two preview services (no cross-reference)
            if (_memberGroupService == null)
                _memberGroupService = new Lazy<IMemberGroupService>(() => new MemberGroupService(provider, repositoryFactory, logger, eventMessagesFactory));

            if (_contentServices == null)
                _contentServices = new Lazy<Tuple<IContentService, IContentTypeService>>(() =>
                {
                    // need this before both services cross-reference each other
                    var contentService = new ContentService(provider, repositoryFactory, logger, eventMessagesFactory, _dataTypeService.Value, _userService.Value);
                    var contentTypeService = new ContentTypeService(provider, repositoryFactory, logger, eventMessagesFactory);
                    contentService.ContentTypeService = contentTypeService;
                    contentTypeService.ContentService = contentService;
                    return Tuple.Create((IContentService)contentService, (IContentTypeService) contentTypeService);
                });

            if (_mediaServices == null)
                _mediaServices = new Lazy<Tuple<IMediaService, IMediaTypeService>>(() =>
                {
                    // need this before both services cross-reference each other
                    var mediaService = new MediaService(provider, repositoryFactory, logger, eventMessagesFactory, _dataTypeService.Value, _userService.Value);
                    var mediaTypeService = new MediaTypeService(provider, repositoryFactory, logger, eventMessagesFactory);
                    mediaService.MediaTypeService = mediaTypeService;
                    mediaTypeService.MediaService = mediaService;
                    return Tuple.Create((IMediaService) mediaService, (IMediaTypeService) mediaTypeService);
                });

            if (_dataTypeService == null)
                _dataTypeService = new Lazy<IDataTypeService>(() => new DataTypeService(provider, repositoryFactory, logger, eventMessagesFactory));

            if (_fileService == null)
                _fileService = new Lazy<IFileService>(() => new FileService(fileProvider, provider, repositoryFactory, logger, eventMessagesFactory));

            if (_localizationService == null)
                _localizationService = new Lazy<ILocalizationService>(() => new LocalizationService(provider, repositoryFactory, logger, eventMessagesFactory));

            if (_entityService == null)
                _entityService = new Lazy<IEntityService>(() => new EntityService(
                    provider, repositoryFactory, logger, eventMessagesFactory,
                    ContentService, ContentTypeService, MediaService, MediaTypeService, MemberService, MemberTypeService, _dataTypeService.Value,
                    //TODO: Consider making this an isolated cache instead of using the global one
                    cache.RuntimeCache));
            
            if (_packagingService == null)
                _packagingService = new Lazy<IPackagingService>(() => new PackagingService(logger, _contentService.Value, _contentTypeService.Value, _mediaService.Value, _macroService.Value, _dataTypeService.Value, _fileService.Value, _localizationService.Value, _entityService.Value, _userService.Value, repositoryFactory, provider));

            if (_relationService == null)
                _relationService = new Lazy<IRelationService>(() => new RelationService(provider, repositoryFactory, logger, eventMessagesFactory, _entityService.Value));

            if (_treeService == null)
                _treeService = new Lazy<IApplicationTreeService>(() => new ApplicationTreeService(logger, cache));

            if (_sectionService == null)
                _sectionService = new Lazy<ISectionService>(() => new SectionService(_userService.Value, _treeService.Value, provider, cache));

            if (_macroService == null)
                _macroService = new Lazy<IMacroService>(() => new MacroService(provider, repositoryFactory, logger, eventMessagesFactory));

            if (_tagService == null)
                _tagService = new Lazy<ITagService>(() => new TagService(provider, repositoryFactory, logger, eventMessagesFactory));
        }

        /// <summary>
        /// Gets the <see cref="IMigrationEntryService"/>
        /// </summary>
        public IMigrationEntryService MigrationEntryService
        {
            get { return _migrationEntryService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IPublicAccessService"/>
        /// </summary>
        public IPublicAccessService PublicAccessService
        {
            get { return _publicAccessService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="ITaskService"/>
        /// </summary>
        public ITaskService TaskService
        {
            get { return _taskService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IDomainService"/>
        /// </summary>
        public IDomainService DomainService
        {
            get { return _domainService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IAuditService"/>
        /// </summary>
        public IAuditService AuditService
        {
            get { return _auditService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="ILocalizedTextService"/>
        /// </summary>
        public ILocalizedTextService TextService
        {
            get { return _localizedTextService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="INotificationService"/>
        /// </summary>
        public INotificationService NotificationService
        {
            get { return _notificationService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="ServerRegistrationService"/>
        /// </summary>
        public IServerRegistrationService ServerRegistrationService
        {
            get { return _serverRegistrationService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="ITagService"/>
        /// </summary>
        public ITagService TagService
        {
            get { return _tagService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IMacroService"/>
        /// </summary>
        public IMacroService MacroService
        {
            get { return _macroService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IEntityService"/>
        /// </summary>
        public IEntityService EntityService
        {
            get { return _entityService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IRelationService"/>
        /// </summary>
        public IRelationService RelationService
        {
            get { return _relationService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IContentService"/>
        /// </summary>
        public IContentService ContentService
        {
            get { return _contentServices.Value.Item1; }
        }

        /// <summary>
        /// Gets the <see cref="IContentTypeService"/>
        /// </summary>
        public IContentTypeService ContentTypeService
        {
            get { return _contentServices.Value.Item2; }
        }

        /// <summary>
        /// Gets the <see cref="IMediaTypeService"/>
        /// </summary>
        public IMediaTypeService MediaTypeService
        {
            get { return _mediaServices.Value.Item2; }
        }

        /// <summary>
        /// Gets the <see cref="IDataTypeService"/>
        /// </summary>
        public IDataTypeService DataTypeService
        {
            get { return _dataTypeService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IFileService"/>
        /// </summary>
        public IFileService FileService
        {
            get { return _fileService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="ILocalizationService"/>
        /// </summary>
        public ILocalizationService LocalizationService
        {
            get { return _localizationService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IMediaService"/>
        /// </summary>
        public IMediaService MediaService
        {
            get { return _mediaServices.Value.Item1; }
        }

        /// <summary>
        /// Gets the <see cref="PackagingService"/>
        /// </summary>
        public IPackagingService PackagingService
        {
            get { return _packagingService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="UserService"/>
        /// </summary>
        public IUserService UserService
        {
            get { return _userService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="MemberService"/>
        /// </summary>
        public IMemberService MemberService
        {
            get { return _memberServices.Value.Item1; }
        }

        /// <summary>
        /// Gets the <see cref="SectionService"/>
        /// </summary>
        public ISectionService SectionService
        {
            get { return _sectionService.Value; }
        }

        /// <summary>
        /// Gets the <see cref="ApplicationTreeService"/>
        /// </summary>
        public IApplicationTreeService ApplicationTreeService
        {
            get { return _treeService.Value; }
        }

        /// <summary>
        /// Gets the MemberTypeService
        /// </summary>
        public IMemberTypeService MemberTypeService
        {
            get { return _memberServices.Value.Item2; }
        }

        /// <summary>
        /// Gets the MemberGroupService
        /// </summary>
        public IMemberGroupService MemberGroupService
        {
            get { return _memberGroupService.Value; }
        }

        public IExternalLoginService ExternalLoginService
        {
            get { return _externalLoginService.Value; }
        }
    }
}