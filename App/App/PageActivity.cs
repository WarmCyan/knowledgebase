using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

using Android.App;
using Android.Content.Res;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Webkit;
using DWL.Utility;

namespace App
{
	[Activity(Label = "PageActivity")]
	public class PageActivity : Activity
	{
		private string m_sCSS;
		private string m_sHead;

		private WebView m_pWebView;

		private string m_sQuery;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.Page);
			
			WebCommunications.AuthKey = "54003c32a190b6063fe06a528bc230ce151b589512db4a39ecf8ac01be393dafa154f39bde1f56e690f4c3c2870323972240d4d02fc4fa2f3349dc7ef4c7dc09";

			// get query string this intent was created with
			Intent pIntent = this.Intent;
			m_sQuery = pIntent.GetStringExtra("Query");

			// load important stuff
			this.LoadCSS();
			this.LoadHeaderHtml();
			
			m_pWebView = FindViewById<WebView>(Resource.Id.webview);
			m_pWebView.SetWebChromeClient(new WebChromeClient());
			m_pWebView.Settings.JavaScriptEnabled = true;
			m_pWebView.Settings.AllowFileAccessFromFileURLs = true;
			m_pWebView.Settings.AllowUniversalAccessFromFileURLs = true;
			m_pWebView.Settings.DomStorageEnabled = true;
			m_pWebView.Settings.SetPluginState(WebSettings.PluginState.On);

			this.Query(m_sQuery);
		}

		private void Query(string sQuery)
		{
			string sFixedQuery = HttpUtility.UrlEncode(sQuery);
			string sResponse = WebCommunications.SendGetRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/ConstructPage?squery=" + sFixedQuery, true);

			// fix response and add to it
			sResponse = Master.CleanResponse(sResponse);

			string sHTML = "<html><head>" + m_sHead + "<style>" + m_sCSS + "</style></head>" + sResponse;

			m_pWebView.LoadData(sHTML, "text/html", "UTF-8");
		}
		
		private void LoadCSS()
		{
			AssetManager pManager = this.Assets;
		
			var pStream = pManager.Open("Style.css");
			StreamReader pStreamReader = new StreamReader(pStream);
			m_sCSS = pStreamReader.ReadToEnd();
		}

		private void LoadHeaderHtml()
		{
			AssetManager pManager = this.Assets;
		
			var pStream = pManager.Open("Head.html");
			StreamReader pStreamReader = new StreamReader(pStream);
			m_sHead = pStreamReader.ReadToEnd();
		}
	}
}