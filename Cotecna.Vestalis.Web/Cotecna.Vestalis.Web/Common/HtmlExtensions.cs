
using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Cotecna.Vestalis.Core;

namespace Cotecna.Vestalis.Web.Common
{
    /// <summary>
    /// This class contains some extension methods for personalizing the views
    /// </summary>
    public static class HtmlExtensions
    {

        #region LocalValidationSummary
        /// <summary>
        /// Generate my validation summary to adapt to our desings
        /// </summary>
        /// <param name="htmlHelper">htmlHelper</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString LocalValidationSummary(this HtmlHelper htmlHelper)
        {
            MvcHtmlString result = null;
            if (htmlHelper.ViewData.ModelState.IsValid) return result;

            int errors = 0;

            foreach (ModelState modelState in htmlHelper.ViewData.ModelState.Values)
            {
                errors += modelState.Errors.Count;
            }

            if (errors == 1)
                result = LocalValidationSummary(htmlHelper, Resources.Common.OneError);
            else if (errors > 1)
                result = LocalValidationSummary(htmlHelper, string.Format(Resources.Common.SomeErrors, errors));

            return result;
        }
        #endregion

        #region LocalValidationSummary
        /// <summary>
        /// Generate my validation summary to adapt to our desings
        /// </summary>
        /// <param name="htmlHelper">htmlHelper</param>
        /// <param name="message">message for summary</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString LocalValidationSummary(this HtmlHelper htmlHelper, string message)
        {
            // Nothing to do if there aren't any errors
            if (htmlHelper.ViewData.ModelState.IsValid)
            {
                return null;
            }

            TagBuilder spanTag = new TagBuilder("h5");
            if (!string.IsNullOrEmpty(message))
                spanTag.SetInnerText(message);

            string messageSpan = spanTag.ToString(TagRenderMode.Normal) + Environment.NewLine;

            StringBuilder htmlSummary = new StringBuilder();
            TagBuilder unorderedList = new TagBuilder("ul");

            foreach (ModelState modelState in htmlHelper.ViewData.ModelState.Values)
            {
                foreach (ModelError modelError in modelState.Errors)
                {
                    string errorText = GetUserErrorMessageOrDefault(modelError, null /* modelState */);
                    if (!String.IsNullOrEmpty(errorText))
                    {
                        TagBuilder listItem = new TagBuilder("li");
                        listItem.SetInnerText(errorText);
                        htmlSummary.AppendLine(listItem.ToString(TagRenderMode.Normal));
                    }
                }
            }

            unorderedList.InnerHtml = htmlSummary.ToString();

            TagBuilder divContent = new TagBuilder("div");
            divContent.MergeAttribute("class", "error");
            divContent.InnerHtml = messageSpan + unorderedList.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(divContent.ToString(TagRenderMode.Normal));

        }
        #endregion

        #region GetUserErrorMessageOrDefault
        private static string GetUserErrorMessageOrDefault(ModelError error, ModelState modelState)
        {
            if (!String.IsNullOrEmpty(error.ErrorMessage))
            {
                return error.ErrorMessage;
            }
            if (modelState == null)
            {
                return null;
            }

            string attemptedValue = (modelState.Value != null) ? modelState.Value.AttemptedValue : null;
            return String.Format(CultureInfo.CurrentCulture, Resources.Common.NotValidValue, attemptedValue);
        }
        #endregion

        #region SortActionLink<T>

        /// <summary>
        /// Builds an ajax action link with a visual indication if the list is sorted
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="linkText"></param>
        /// <param name="action">The HttpPost action that returns the PartialViewResult</param>
        /// <param name="sortedColumn">The sorted column that represents this link</param>
        /// <param name="model">The properties fot sorting are preserved</param>
        /// <param name="function">The script function that replaces the actual content with the ajax content retrieved.</param>
        /// <param name="currentPage">Selected page</param>
        /// <param name="routeValues">Additional routeValues to be preserved</param>
        /// <returns></returns>
        public static MvcHtmlString SortActionLink<T>(this AjaxHelper helper, string linkText, string action, string sortedColumn, PaginatedList<T> model,
                                                        string function, int currentPage, RouteValueDictionary routeValues = null)
        {

            System.Web.UI.WebControls.SortDirection direction = System.Web.UI.WebControls.SortDirection.Descending;
            bool isSameColumn = false;
            if (model.SortedColumn == sortedColumn)
            {
                if (model.SortDirection == System.Web.UI.WebControls.SortDirection.Descending)
                    direction = System.Web.UI.WebControls.SortDirection.Ascending;
                isSameColumn = true;
            }

            RouteValueDictionary staticRouteValues = new RouteValueDictionary();
            staticRouteValues.Add("sortDirection", (isSameColumn ? direction : System.Web.UI.WebControls.SortDirection.Ascending));
            staticRouteValues.Add("sortedColumn", sortedColumn);
            staticRouteValues.Add("page", currentPage);
            if (routeValues != null)
            {
                foreach (var item in routeValues)
                {
                    staticRouteValues.Add(item.Key, item.Value);
                }
            }

            string html = helper.JqueryAjaxLink(linkText, action, function, staticRouteValues);
            //string html = helper.ActionLink(linkText, action, staticRouteValues, ajaxOptions).ToHtmlString();

            //Define image
            TagBuilder tagBuilder = new TagBuilder("img");
            string path = VirtualPathUtility.ToAbsolute("~/Images/");
            if (sortedColumn == model.SortedColumn)
            {
                if (model.SortDirection == System.Web.UI.WebControls.SortDirection.Ascending)
                    path += "arrow_navy_3.png";
                else
                    path += "arrow_navy_2.png";
            }
            else
                path += "arrow_navy.png";

            tagBuilder.MergeAttribute("src", path);
            tagBuilder.MergeAttribute("border", "0");

            html += tagBuilder.ToString();
            return MvcHtmlString.Create(html);

        }
        #endregion

        #region SortActionLink

        /// <summary>
        /// Builds an ajax action link with a visual indication if the list is sorted
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="linkText"></param>
        /// <param name="action">The HttpPost action that returns the PartialViewResult</param>
        /// <param name="sortedColumn">The sorted column that represents this link</param>
        /// <param name="model">The properties fot sorting are preserved</param>
        /// <param name="function">The script function that replaces the actual content with the ajax content retrieved.</param>
        /// <param name="currentPage">The selected page</param>
        /// <param name="routeValues">Additional routeValues to be preserved</param>
        /// <returns></returns>
        public static MvcHtmlString SortActionLink(this AjaxHelper helper, string linkText, string action, string sortedColumn, PaginatedGridModel model,
                                                        string function, int currentPage, RouteValueDictionary routeValues = null)
        {

            System.Web.UI.WebControls.SortDirection direction = System.Web.UI.WebControls.SortDirection.Descending;
            bool isSameColumn = false;
            if (model.SortedColumn == sortedColumn)
            {
                if (model.SortDirection == System.Web.UI.WebControls.SortDirection.Descending)
                    direction = System.Web.UI.WebControls.SortDirection.Ascending;
                isSameColumn = true;
            }

            RouteValueDictionary staticRouteValues = new RouteValueDictionary();
            staticRouteValues.Add("sortDirection", (isSameColumn ? direction : System.Web.UI.WebControls.SortDirection.Ascending));
            staticRouteValues.Add("sortedColumn", sortedColumn);
            staticRouteValues.Add("page", currentPage);
            if (routeValues != null)
            {
                foreach (var item in routeValues)
                {
                    staticRouteValues.Add(item.Key, item.Value);
                }
            }

            string html = helper.JqueryAjaxLink(linkText, action, function, staticRouteValues);
            //string html = helper.ActionLink(linkText, action, staticRouteValues, ajaxOptions).ToHtmlString();

            //Define image
            TagBuilder tagBuilder = new TagBuilder("img");
            string path = VirtualPathUtility.ToAbsolute("~/Images/");
            if (sortedColumn == model.SortedColumn)
            {
                if (model.SortDirection == System.Web.UI.WebControls.SortDirection.Ascending)
                    path += "arrow_navy_3.png";
                else
                    path += "arrow_navy_2.png";
            }
            else
                path += "arrow_navy.png";

            tagBuilder.MergeAttribute("src", path);
            tagBuilder.MergeAttribute("border", "0");

            html += tagBuilder.ToString();
            return MvcHtmlString.Create(html);

        }
        #endregion

        #region JqueryAjaxLink
        /// <summary>
        /// Creates a Jquery link that makes an AJAX post expecting Html data.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="text">The link text</param>
        /// <param name="action">The action to be executed on click</param>
        /// <param name="function">The script function to replace the content</param>
        /// <param name="routeValues">Additional route values to be added to the action</param>
        /// <returns>An anchor tag</returns>
        public static string JqueryAjaxLink(this AjaxHelper helper, string text, string action, string function, RouteValueDictionary routeValues = null)
        {
            string url;
            if (routeValues != null)
                url = new UrlHelper(helper.ViewContext.RequestContext).Action(action, routeValues);
            else
                url = new UrlHelper(helper.ViewContext.RequestContext).Action(action);

            string jqueryPost = "$.post('" + url + "', null, " + function + ", 'html');";
            string html = "<a style='cursor:pointer;'  onclick=\"" + jqueryPost + "\">" + text + "</a>";
            return html;
        }
        #endregion
    }

}