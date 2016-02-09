using System.Web;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Web.Cache;
using Umbraco.Web.Install.Models;
using Umbraco.Web.Security;
using GlobalSettings = umbraco.GlobalSettings;

namespace Umbraco.Web.Install.InstallSteps
{
    [InstallSetupStep(InstallationType.NewInstall | InstallationType.Upgrade,
        "UmbracoVersion", 50, "Installation is complete!, get ready to be redirected to your new CMS.",
        PerformsAppRestart = true)]
    internal class SetUmbracoVersionStep : InstallSetupStep<object>
    {
        private readonly ApplicationContext _applicationContext;
        private readonly HttpContextBase _httpContext;

        public SetUmbracoVersionStep(ApplicationContext applicationContext, HttpContextBase httpContext)
        {
            _applicationContext = applicationContext;
            _httpContext = httpContext;
        }

        public override InstallSetupResult Execute(object model)
        {
            // Some upgrade scripts "may modify the database (cmsContentXml...) tables directly" - not sure
            // that is still true but the idea is that after an upgrade we want to reset the local facade, on
            // all LB nodes of course, so we need to use the distributed cache, and refresh everything.
            DistributedCache.Instance.RefreshAllFacade();

            // Update configurationStatus
            GlobalSettings.ConfigurationStatus = UmbracoVersion.GetSemanticVersion().ToSemanticString();

            // Update ClientDependency version
            var clientDependencyConfig = new ClientDependencyConfiguration(_applicationContext.ProfilingLogger.Logger);
            var clientDependencyUpdated = clientDependencyConfig.IncreaseVersionNumber();
            
            //reports the ended install            
            ih.InstallStatus(true, "");

            return null;
        }

        public override bool RequiresExecution(object model)
        {
            return true;
        }
    }
}