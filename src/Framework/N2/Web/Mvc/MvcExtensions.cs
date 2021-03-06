﻿using System.Web;
using System.Web.Mvc;
using N2.Definitions;

namespace N2.Web.Mvc
{
	public static class MvcExtensions
	{

		// theming

		private const string ThemeKey = "theme";
		public static string GetTheme(this ControllerContext context)
		{
			return context.HttpContext.GetTheme();
		}

		internal static string GetTheme(this HttpContextBase context)
		{
			return context.Request[ThemeKey] // preview theme via query string
				?? context.Items[ThemeKey] as string // previously initialized theme
				?? "Default"; // fallback
		}

		public static void SetTheme(this ControllerContext context, string theme)
		{
			context.HttpContext.Items[ThemeKey] = theme;
		}

		public static void InitTheme(this ControllerContext context)
		{
			var page = context.RequestContext.CurrentPage<ContentItem>()
				?? RouteExtensions.ResolveService<IUrlParser>(context.RouteData).ResolvePath(context.HttpContext.Request["returnUrl"]).StopItem
				?? RouteExtensions.ResolveService<IUrlParser>(context.RouteData).ResolvePath(context.HttpContext.Request.AppRelativeCurrentExecutionFilePath).StopItem
				?? context.RequestContext.StartPage();

			InitTheme(context, page);
		}

		private static void InitTheme(ControllerContext context, ContentItem page)
		{
			var start = Find.ClosestOf<IThemeable>(page);
			if (start == null)
				return;

			var themeSource = start as IThemeable;
			if (string.IsNullOrEmpty(themeSource.Theme))
				InitTheme(context, start.Parent);
			else
				context.SetTheme(themeSource.Theme);
		}
	}
}
