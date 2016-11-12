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
	[Activity(Label = "App", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.NoTitleBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
		private int count = 1;

		private bool m_bBrowserInitialized;
		private WebView m_pWebView;

		private Dictionary<string, string> m_dLoadedPages;
		private string m_sCurrentPage;

		//private string[] m_aNavTitles;
		private List<string> m_lNavTitles;
		private DrawerLayout m_pDrawerLayout;
		private ListView m_pDrawerList;

		private string m_sCSS;
		private string m_sHead;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			WebCommunications.AuthKey = "54003c32a190b6063fe06a528bc230ce151b589512db4a39ecf8ac01be393dafa154f39bde1f56e690f4c3c2870323972240d4d02fc4fa2f3349dc7ef4c7dc09";

			m_bBrowserInitialized = false;
			m_dLoadedPages = new Dictionary<string, string>();

			// load important stuff
			this.LoadCSS();
			this.LoadHeaderHtml();

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			//Button button = FindViewById<Button>(Resource.Id.MyButton);

			//button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };

			//string[] m_aNavTitles = new string[] { "thing1", "thing2" };
			m_lNavTitles = new List<string>() { "Query", "New Snippet", "Close Current" };

			m_pDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.appDrawerLayout);
			m_pDrawerList = FindViewById<ListView>(Resource.Id.appDrawerList);

			this.RefreshDrawer();

			m_pDrawerList.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
			{
				int iChoice = e.Position;
				string sChoice = m_lNavTitles[iChoice];
				if (sChoice == "Query") { this.ShowInputDialog(); }
				else if (sChoice == "New Snippet") 
				{
					Intent pIntent = new Intent(this, (new PageActivity()).Class);
					StartActivity(pIntent);
				}
				else if (sChoice == "Close Current") { this.ClosePage(); }
				else { this.Query(sChoice); }

				m_pDrawerLayout.CloseDrawer(m_pDrawerList);
			};

			//m_pWebView.SetWebViewClient(new CustomWebViewClient());

			//m_pWebView.SetWebChromeClient
			//WebView.SetWebContentsDebuggingEnabled(true); // doesn't seem to do anything?? How do you debug it?

			//m_pWebView.LoadUrl("http://www.google.com");
			//m_pWebView.Load
			//Query("Genetic_Algorithm");
			//Query("Test");
			Console.WriteLine("Hello world!");
		}

		private void InitBrowser()
		{
			if (!m_bBrowserInitialized)
			{
				m_pWebView = FindViewById<WebView>(Resource.Id.webview);
				m_pWebView.SetWebChromeClient(new WebChromeClient());
				m_pWebView.Settings.JavaScriptEnabled = true;
				m_pWebView.Settings.AllowFileAccessFromFileURLs = true;
				m_pWebView.Settings.AllowUniversalAccessFromFileURLs = true;
				m_pWebView.Settings.DomStorageEnabled = true;
				m_pWebView.Settings.SetPluginState(WebSettings.PluginState.On);
				m_pWebView.SetLayerType(LayerType.Hardware, null); // make it go faster?
				m_bBrowserInitialized = true;
			}
		}

		private void Query(string sQuery)
		{
			this.InitBrowser();

			string sHTML = "";
			if (!m_dLoadedPages.ContainsKey(sQuery))
			{
				string sFixedQuery = HttpUtility.UrlEncode(sQuery);
				string sResponse = WebCommunications.SendGetRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/ConstructPage?squery=" + sFixedQuery, true);


				// fix response and add to it
				sResponse = Master.CleanResponse(sResponse);

				sHTML = "<html><head>" + m_sHead + "<style>" + m_sCSS + "</style></head>" + sResponse;

				m_dLoadedPages.Add(sQuery, sHTML);
				m_lNavTitles.Add(sQuery);
				this.RefreshDrawer();
			}
			else { sHTML = m_dLoadedPages[sQuery]; }
			m_sCurrentPage = sQuery;

			m_pWebView.StopLoading();
			m_pWebView.LoadData(sHTML, "text/html", "UTF-8");
			m_pWebView.Reload();
		}

		private void RefreshDrawer() { m_pDrawerList.Adapter = new DrawerItemCustomAdapter(this, Resource.Layout.ListViewItemRow, m_lNavTitles.ToArray()); }
		
		private void ClosePage()
		{
			if (m_sCurrentPage == "" || m_sCurrentPage == null) return;
			
			m_dLoadedPages.Remove(m_sCurrentPage);
			m_lNavTitles.Remove(m_sCurrentPage);

			m_pWebView.StopLoading();
			m_pWebView.LoadData("", "text/html", "UTF-8");
			m_pWebView.Reload();
			m_sCurrentPage = "";
			this.RefreshDrawer();
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

		protected void ShowInputDialog()
		{
			LayoutInflater pLayoutInflater = LayoutInflater.From(this);
			View pPromptView = pLayoutInflater.Inflate(Resource.Layout.InputDialog, null);
			AlertDialog.Builder pAlertBuilder = new AlertDialog.Builder(this);
			pAlertBuilder.SetView(pPromptView);

			EditText pEditText = (EditText)pPromptView.FindViewById(Resource.Id.txtQuery);
			ProgressBar pProgressBar = (ProgressBar)pPromptView.FindViewById(Resource.Id.indicator);
			pProgressBar.Visibility = ViewStates.Invisible;

			// setup a dialog window
			//pAlertBuilder.SetCancelable(true).SetPositiveButton("OK", delegate
			pAlertBuilder.SetPositiveButton("Query", delegate
			{
				Console.WriteLine("Querying: " + pEditText.Text);
				pProgressBar.Visibility = ViewStates.Visible;
				Query(pEditText.Text);
			});

			// create an alert dialog
			AlertDialog alert = pAlertBuilder.Create();
			alert.Show();
		}
	}
}

