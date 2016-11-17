//*************************************************************
//  File: MainActivity.cs
//  Date created: 10/21/2016
//  Date edited: 11/16/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: The main page/area of the app
//*************************************************************

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Android.App;
using Android.Content.Res;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using DWL.Utility;

namespace App
{
	[Activity(Label = "Knowledgebase", MainLauncher = true, Icon = "@drawable/Logo", Theme = "@android:style/Theme.NoTitleBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
		private int m_iCount = 1;

		private bool m_bBrowserInitialized;
		private WebView m_pWebView;

		private Dictionary<string, string> m_dLoadedPages;
		private Dictionary<string, int> m_dPageScrollPoints;
		private string m_sCurrentPage;

		//private string[] m_aNavTitles;
		private List<string> m_lNavTitles;
		private DrawerLayout m_pDrawerLayout;
		private ListView m_pDrawerList;

		private string m_sCSS;
		private string m_sHead;
		private string m_sHome;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			WebCommunications.AuthKey = "54003c32a190b6063fe06a528bc230ce151b589512db4a39ecf8ac01be393dafa154f39bde1f56e690f4c3c2870323972240d4d02fc4fa2f3349dc7ef4c7dc09";

			m_bBrowserInitialized = false;
			m_dLoadedPages = new Dictionary<string, string>();
			m_dPageScrollPoints = new Dictionary<string, int>();

			// load important stuff
			this.LoadCSS();
			this.LoadHeaderHtml();
			this.LoadHomeHTML();

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			m_lNavTitles = new List<string>() { "Query", "New Snippet", "Close Current", "Random Tag" };

			m_pDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.appDrawerLayout);
			m_pDrawerList = FindViewById<ListView>(Resource.Id.appDrawerList);

			this.RefreshDrawer();

			m_pDrawerList.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
			{
				int iChoice = e.Position;
				string sChoice = m_lNavTitles[iChoice];
				if (sChoice == "Query")
				{
					Intent pIntent = new Intent(this, (new QueryActivity()).Class);
					pIntent.PutExtra("num", m_iCount);
					StartActivityForResult(pIntent, 0);
				}
				else if (sChoice == "New Snippet")
				{
					Intent pIntent = new Intent(this, (new PageActivity()).Class);
					pIntent.PutExtra("Type", "new");
					StartActivity(pIntent);
				}
				else if (sChoice == "Close Current") { this.ClosePage(); }
				else if (sChoice == "Random Tag") { this.RandomTag(); }
				else { this.Query(sChoice); }

				m_pDrawerLayout.CloseDrawer(m_pDrawerList);
			};

			global::Xamarin.Forms.Forms.Init(this, bundle);
			DisplayHome();
		}

		private void DisplayHome()
		{
			this.InitBrowser();
			m_pWebView.LoadUrl("about:blank");
			string sHomeHTML = "<html><head><style>" + m_sCSS + "</style></head>" + m_sHome + "</html>";
			m_pWebView.LoadData(sHomeHTML, "text/html", "UTF-8");
		}

		private void InitBrowser()
		{
			if (!m_bBrowserInitialized)
			{
				m_pWebView = FindViewById<WebView>(Resource.Id.webview);
				m_pWebView.SetWebChromeClient(new WebChromeClient());
				//m_pWebView.SetWebViewClient(new CustomWebViewClient());
				
				// setting stuff
				m_pWebView.Settings.JavaScriptEnabled = true;
				m_pWebView.Settings.AllowFileAccessFromFileURLs = true;
				m_pWebView.Settings.AllowUniversalAccessFromFileURLs = true;
				m_pWebView.Settings.DomStorageEnabled = true;
				m_pWebView.Settings.SetPluginState(WebSettings.PluginState.On);
				m_pWebView.SetLayerType(LayerType.Hardware, null); // make it go faster?

				// attach our javascript interface thing
				m_pWebView.AddJavascriptInterface(new JSInterface(this), "CSharp");

				m_bBrowserInitialized = true;
			}
		}

		private void Query(string sQuery)
		{
			this.InitBrowser();

			// save current point
			if (m_sCurrentPage != "" && m_sCurrentPage != null) { m_dPageScrollPoints[m_sCurrentPage] = m_pWebView.ScrollY; }

			int iScrollPoint = 0;

			string sHTML = "";
			if (!m_dLoadedPages.ContainsKey(sQuery))
			{
				string sFixedQuery = HttpUtility.UrlEncode(sQuery);
				string sResponse = WebCommunications.SendGetRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/ConstructPage?squery=" + sFixedQuery, true);

				// fix response and add to it
				sResponse = Master.CleanResponse(sResponse);

				Console.WriteLine(m_sHead);
				sHTML = "<html><head>" + m_sHead + "<style>" + m_sCSS + "</style></head>" + sResponse + "</html>";
				
				m_dLoadedPages.Add(sQuery, sHTML);
				m_dPageScrollPoints.Add(sQuery, 0);
				iScrollPoint = 0;
				m_lNavTitles.Add(sQuery);
				this.RefreshDrawer();
			}
			else 
			{ 
				sHTML = m_dLoadedPages[sQuery];
				iScrollPoint = m_dPageScrollPoints[sQuery];
			}
			m_sCurrentPage = sQuery;

			//m_pWebView.StopLoading();
			m_pWebView.LoadUrl("about:blank");
			m_pWebView.LoadData(sHTML, "text/html", "UTF-8");
			m_pWebView.ScrollY = iScrollPoint;
			//m_pWebView.Reload();
		}

		private void RefreshDrawer() { m_pDrawerList.Adapter = new DrawerItemCustomAdapter(this, Resource.Layout.ListViewItemRow, m_lNavTitles.ToArray()); }
		
		private void ClosePage()
		{
			if (m_sCurrentPage == "" || m_sCurrentPage == null) return;
			
			m_dLoadedPages.Remove(m_sCurrentPage);
			m_dPageScrollPoints.Remove(m_sCurrentPage);
			m_lNavTitles.Remove(m_sCurrentPage);

			//m_pWebView.StopLoading();
			//m_pWebView.LoadData("", "text/html", "UTF-8");
			//m_pWebView.Reload();
			this.DisplayHome();
			m_sCurrentPage = "";
			this.RefreshDrawer();
		}

		private void RandomTag()
		{
			string sResponse = WebCommunications.SendGetRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/RandomTag", true);
			this.Query(Master.CleanResponse(sResponse));
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

		private void LoadHomeHTML()
		{
			AssetManager pManager = this.Assets;
		
			var pStream = pManager.Open("Home.html");
			StreamReader pStreamReader = new StreamReader(pStream);
			m_sHome = pStreamReader.ReadToEnd();
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == Result.Ok)
			{
				this.Query(data.GetStringExtra("Query"));
			}
		}
	}
}

