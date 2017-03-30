using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using Umbraco.Core;

namespace Umbraco.Core
{
    /// <summary>
    /// Manages any assembly binding redirects that cannot be done via config
    /// </summary>
    internal class BindingRedirects
    {
        public static void Initialize()
        {
            // this only gets called when an assembly can't be resolved
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static readonly Regex Log4NetAssemblyPattern = new Regex("log4net, Version=([\\d\\.]+?), Culture=neutral, PublicKeyToken=null");
        private const string Log4NetReplacement = "log4net, Version=1.2.15.0, Culture=neutral, PublicKeyToken=669e0ddf0bb1aa2a";

        private static readonly Regex HtmlAgilityAssemblyPattern = new Regex("HtmlAgilityPack, Version=([\\d\\.]+?), Culture=neutral, PublicKeyToken=bd319b19eaf3b43a");
        private const string HtmlAgilityReplacement = "HtmlAgilityPack, Version=1.4.9.5, Culture=neutral, PublicKeyToken=bd319b19eaf3b43a";

        /// <summary>
        /// This is used to do an assembly binding redirect via code - normally required due to signature changes in assemblies
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //log4net:            
            if (Log4NetAssemblyPattern.IsMatch(args.Name) && args.Name != Log4NetReplacement)
            {
                return Assembly.Load(Log4NetAssemblyPattern.Replace(args.Name, Log4NetReplacement));
            }

            //HtmlAgility:            
            if (HtmlAgilityAssemblyPattern.IsMatch(args.Name) && args.Name != HtmlAgilityReplacement)
            {
                return Assembly.Load(HtmlAgilityAssemblyPattern.Replace(args.Name, HtmlAgilityReplacement));
            }

            //AutoMapper:
            // ensure the assembly is indeed AutoMapper and that the PublicKeyToken is null before trying to Load again
            // do NOT just replace this with 'return Assembly', as it will cause an infinite loop -> stackoverflow
            if (args.Name.StartsWith("AutoMapper") && args.Name.EndsWith("PublicKeyToken=null"))
                return Assembly.Load(args.Name.Replace(", PublicKeyToken=null", ", PublicKeyToken=be96cd2c38ef1005"));

            return null;
            
        }
    }
}