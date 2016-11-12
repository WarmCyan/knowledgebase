using System;
using System.Collections.Generic;
using System.Xml.Linq;
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
	[Activity(Label = "Snippet")]
	public class PageActivity : Activity
	{
		// member variables
		private string m_sBaseDir;

		private EditText m_pTags;
		private EditText m_pSources;
		
		private static List<string> s_lGenericTags = new List<string>() { "Wisdom", "Theory", "Note", "Important", "Depth", "Definition", "Argument" };

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Page);

			//update local tag cache
			m_sBaseDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			this.UpdateTagCache();
			

			m_pTags = FindViewById<EditText>(Resource.Id.txtTags);
			m_pSources = FindViewById<EditText>(Resource.Id.txtSource);

			Button pTagsButton = FindViewById<Button>(Resource.Id.btnTagsList);
			pTagsButton.Click += delegate
			{
				this.DisplayList("tags");
			};
			Button pSourceButton = FindViewById<Button>(Resource.Id.btnSourceList);
			pSourceButton.Click += delegate
			{
				this.DisplayList("sources");
			};
			
			//AssetManager pManager = this.Assets;
			//var pStream = pManager.Open("Style.css");
			//StreamReader pStreamReader = new StreamReader(pStream);
			//m_sCSS = pStreamReader.ReadToEnd();
		}

		private void UpdateTagCache()
		{
			// get the xml list of tags from the server
			string sResponse = WebCommunications.SendGetRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/ListTags", true);
			sResponse = Master.CleanResponse(sResponse);
			//txtSnippetContent.Text = m_sHomeFolder;

			List<string> lFileLines = new List<string>();
			List<string> lSourceLines = new List<string>();

			// get data from xml
			XElement pTagsXml = XElement.Parse(sResponse);
			foreach (XElement pTagXml in pTagsXml.Elements("Tag"))
			{
				if (pTagXml.Attribute("Source").Value == "true") { lSourceLines.Add(pTagXml.Value); }
				else { lFileLines.Add(pTagXml.Value); }
			}

			// save the lines into files
			File.WriteAllLines(m_sBaseDir + "_tagcache.dat", lFileLines.ToArray());
			File.WriteAllLines(m_sBaseDir + "_sourcecache.dat", lSourceLines.ToArray());
		}

		private void DisplayList(string sListName)
		{
			// make sure needed files exist
			//if (!File.Exists(m_sBaseDir + "_tagcache.dat")) { File.WriteAllText(m_sBaseDir + "_tagcache.dat", ""); }
			//if (!File.Exists(m_sBaseDir + "_sourcecache.dat")) { File.WriteAllText(m_sBaseDir + "_sourcecache.dat", ""); }

			List<string> lStringList = new List<string>();
			if (sListName == "tags")
			{
				lStringList = File.ReadAllLines(m_sBaseDir + "_tagcache.dat").ToList();

				foreach (string sGeneric in s_lGenericTags)
				{
					if (lStringList.Contains(sGeneric)) lStringList.Remove(sGeneric);
					lStringList.Insert(0, sGeneric);
				}
			}
			else if (sListName == "sources")
			{
				lStringList = File.ReadAllLines(m_sBaseDir + "_sourcecache.dat").ToList();


				for (int i = 0; i < lStringList.Count; i++)
				{
					lStringList[i] = lStringList[i].Remove(lStringList[i].IndexOf("source:"), "source:".Length);
				}
			}
			
			LayoutInflater pLayoutInflater = LayoutInflater.From(this);
			View pPromptView = pLayoutInflater.Inflate(Resource.Layout.ListDialog, null);
			AlertDialog.Builder pAlertBuilder = new AlertDialog.Builder(this);
			pAlertBuilder.SetView(pPromptView);

			ScrollView pScrollView = (ScrollView)pPromptView.FindViewById(Resource.Id.listDialog);
			ListView pListView = (ListView)pPromptView.FindViewById(Resource.Id.listList);
			//pListView.Adapter = new DrawerItemCustomAdapter(this, Resource.Layout.ListViewItemRowSmaller, lStringList.ToArray());
			pListView.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, lStringList.ToArray());

			// setup a dialog window
			pAlertBuilder.SetCancelable(true);
			AlertDialog pAlert = pAlertBuilder.Create();

			pListView.ItemClick += (sender, args) =>
			{
				int iChoice = args.Position;
				string sChoice = lStringList[iChoice];

				if (sListName == "tags")
				{
					if (m_pTags.Text != "") { m_pTags.Text += ","; }
					m_pTags.Text += sChoice;
					pAlert.Dismiss();
				}
				else if (sListName == "sources")
				{
					m_pSources.Text = sChoice;
					pAlert.Dismiss();
				}
			};

			// create an alert dialog
			pAlert.Show();
			pAlert.Window.SetLayout(600, 1000);
			pScrollView.LayoutParameters.Height = 1000;
			pListView.RequestLayout();
		}
	}
}