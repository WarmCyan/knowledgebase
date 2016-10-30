using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Webkit;

namespace App
{
	class CustomWebViewClient : WebViewClient
	{
		public override bool ShouldOverrideUrlLoading(WebView view, string url)
		{
			view.LoadUrl(url);
			return true;
		}
	}
}