﻿using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.Helpers
{
    [HtmlTargetElement("infolinks")]
    public class InfoLinksTagHelper : TagHelper
    {
        private readonly PageService _pageService;
        private readonly IUrlHelperFactory _urlHelperFactory;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public InfoLinksTagHelper(IUrlHelperFactory urlHelperFactory,
            PageService pageService)
        {
            _urlHelperFactory = Require.IsNotNull(urlHelperFactory, nameof(urlHelperFactory));
            _pageService = Require.IsNotNull(pageService, nameof(pageService));
        }

        public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var pages = await _pageService.GetFooterPagesAsync();
            if (pages.Count() > 0)
            {
                IUrlHelper url = _urlHelperFactory.GetUrlHelper(ViewContext);
                string activeStub = url.ActionContext.RouteData.Values["stub"] as string;
                var pageList = new List<string>();
                foreach (var page in pages)
                {
                    var link = url.Action("Index", "Info", new { stub = page.Stub });
                    if (page.Stub == activeStub)
                    {
                        pageList.Add($"<a class=\"active\" href=\"{link}\">{page.Title}</a>");
                    }
                    else
                    {
                        pageList.Add($"<a href=\"{link}\">{page.Title}</a>");
                    }
                }
                output.TagName = "div";
                output.Attributes.Add("class", "infolinks");
                output.Content.AppendHtml(string.Join(" | ", pageList));
            }
            else
            {
                output.TagName = "";
            }
        }
    }
}
