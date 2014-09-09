﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Text;
using System.Xml;
using System.IO;
using Umbraco.Core;
using Umbraco.Core.IO;

namespace umbraco.presentation.umbraco.developer.Xslt
{
    [WebformsPageTreeAuthorize(Constants.Trees.Xslt)]
    public partial class xsltVisualize : BasePages.UmbracoEnsuredPage
    {
        
		// zb-00004 #29956 : refactor cookies names & handling
		static global::umbraco.BusinessLogic.StateHelper.Cookies.Cookie cookie
			= new global::umbraco.BusinessLogic.StateHelper.Cookies.Cookie("UMB_XSLTVISPG", TimeSpan.FromMinutes(20)); // was "XSLTVisualizerPage"

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if cookie exists in the current request.
				// zb-00004 #29956 : refactor cookies names & handling
				if (cookie.HasValue)
                    contentPicker.Value = cookie.GetValue();
            }            

        }

        protected void visualizeDo_Click(object sender, EventArgs e)
        {
            // get xslt file
            string xslt;
            if (xsltSelection.Value.Contains("<xsl:stylesheet"))
            {
                // assume xslt contains everything we need
                xslt = xsltSelection.Value;
            }
            else
            {
                // read clean xslt, paste selection, and prepare support for XSLT extensions
                xslt = File.ReadAllText(IOHelper.MapPath(SystemDirectories.Umbraco + "/xslt/templates/clean.xslt"));
                xslt = xslt.Replace("<!-- start writing XSLT -->", xsltSelection.Value);
                xslt = Umbraco.Web.Macros.XsltMacroEngine.AddXsltExtensionsToHeader(xslt);
            }

            int pageId;
            if (int.TryParse(contentPicker.Value, out pageId) == false)
                pageId = -1;

            // transform
            string xsltResult;
            try
            {
                xsltResult = Umbraco.Web.Macros.XsltMacroEngine.TestXsltTransform(xslt, pageId);
            }
            catch (Exception ee)
            {
                xsltResult = string.Format(
                    "<div class=\"error\"><h3>Error parsing the XSLT:</h3><p>{0}</p></div>",
                    ee.ToString());
            }

            visualizeContainer.Visible = true;

            // update output
            visualizeArea.Text = !String.IsNullOrEmpty(xsltResult) ? "<div id=\"result\">" + xsltResult + "</Div>" : "<div class=\"notice\"><p>The XSLT didn't generate any output</p></div>";


            // add cookie with current page
			// zb-00004 #29956 : refactor cookies names & handling
			cookie.SetValue(contentPicker.Value);
        }

    }
}
