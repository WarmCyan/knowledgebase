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
using System.Text.RegularExpressions;
using DWL.Utility;

namespace App
{
	[Activity(Label = "Create Snippet")]
	public class PageActivity : Activity
	{
		private static string s_sMetaPattern = @"<meta name='([a-zA-Z]*)' content='([^\']*)'>";
		private static List<string> s_lGenericTags = new List<string>() { "Wisdom", "Theory", "Note", "Important", "Depth", "Definition", "Argument" };
		
		// member variables
		private string m_sBaseDir;
		private bool m_bEditing = false;
		private string m_sEditingSnippet = "";

		private EditText m_pTags;
		private EditText m_pSources;


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Page);

			//update local tag cache
			m_sBaseDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			this.UpdateTagCache();

			m_pTags = FindViewById<EditText>(Resource.Id.txtTags);
			m_pSources = FindViewById<EditText>(Resource.Id.txtSource);

			EditText pSnippetContent = FindViewById<EditText>(Resource.Id.txtSnippetContent);
			EditText pSourceData = FindViewById<EditText>(Resource.Id.txtSourceData);

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

			Button pSubmitButton = FindViewById<Button>(Resource.Id.btnSubmit);
			pSubmitButton.Click += delegate
			{
				string sContent = pSnippetContent.Text;
				string sTags = m_pTags.Text + ",source:" + m_pSources.Text;

				// add meta tags
				//sContent = "<meta name='sourceTag' content='" + lblSourceName.Content + "'><meta name='source' content='" + txtSourceText.Text + "'>" + sContent;
				sContent = "<meta name='sourceTag' content='" + m_pSources.Text + "'><meta name='source' content='" + pSourceData.Text + "'>" + sContent;

				// make the xml request body
				string sBody = "<params>";
				sBody += "<param name='sTagList'>" + sTags + "</param><param name='sSnippet'>" + Master.EncodeXML(sContent) + "</param>";
				if (m_bEditing) { sBody += "<param name='sFileName'>" + m_sEditingSnippet + "</param>"; }
				sBody += "</params>";

				string sResponse = "";
				if (m_bEditing) { sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/EditSnippet", sBody, true); }
				else { sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/AddSnippet", sBody, true); }

				this.Finish();
			};

			string sType = this.Intent.GetStringExtra("Type");
			if (sType == "new") { m_bEditing = false; }
			else if (sType == "edit") 
			{
				string sSnippetName = this.Intent.GetStringExtra("SnippetName");
				string sSnippetSourceName = this.Intent.GetStringExtra("SourceName");
				string sSnippetSourceText = this.Intent.GetStringExtra("SourceText");
				string sSnippetTags = this.Intent.GetStringExtra("SnippetTags");

				m_bEditing = true;
				m_sEditingSnippet = sSnippetName;

				// get the actual snippet content from the server
				string sSnippetContent = WebCommunications.SendGetRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/GetSnippet?sfilename=" + sSnippetName, true);
				sSnippetContent = Master.CleanResponse(sSnippetContent);

				// remove the meta tags (since they are merely added back in on submit)
				Regex pRegex = new Regex(s_sMetaPattern);
				sSnippetContent = pRegex.Replace(sSnippetContent, "");

				// fill fields
				pSnippetContent.Text = sSnippetContent;
				pSourceData.Text = sSnippetSourceText;
				m_pTags.Text = sSnippetTags;
				m_pSources.Text = sSnippetSourceName;

				this.Title = "Edit Snippet";
			}
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