using Collaboration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Collaboration.Helpers
{
    public static class ListHelper
    {
        public static MvcHtmlString List(this HtmlHelper html, ICollection<Countries> countries, object htmlAttributes = null)
        {
            TagBuilder ul = new TagBuilder("ul");
            foreach(Countries c in countries)
            {
                TagBuilder li = new TagBuilder("li");
                li.SetInnerText(c.Name.ToString()+" / "+c.Code.ToString());
                ul.InnerHtml += li.ToString();
            }
            ul.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            return MvcHtmlString.Create(ul.ToString());
        }
    }
}