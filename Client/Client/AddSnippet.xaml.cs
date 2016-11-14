using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Configuration;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using DWL.Utility;

namespace Client
{
	/// <summary>
	/// Interaction logic for AddSnippet.xaml
	/// </summary>
	public partial class AddSnippet : Window
	{
		private static string s_sMetaPattern = @"<meta name='([a-zA-Z]*)' content='([^\']*)'>";
		private static List<string> s_lGenericTags = new List<string>() { "Wisdom", "Theory", "Note", "Important", "Depth", "Definition", "Argument" };

		// member variables
		private string m_sBaseDir = AppDomain.CurrentDomain.BaseDirectory;
		private bool m_bEditing = false;
		private string m_sEditingSnippet = "";
	
		public AddSnippet()
		{
			InitializeComponent();

			// update local tag cache
			if (!Master.IsCacheRefreshed()) { this.UpdateTagCache(); }
			ListTags();
			
		}

		public void MakeEditingSnippet(string sSnippetName, string sSnippetSourceName, string sSnippetSourceText, string sSnippetTags)
		{
			m_bEditing = true;
			m_sEditingSnippet = sSnippetName;

			// get the actual snippet content from the server
			string sSnippetContent = WebCommunications.SendGetRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/GetSnippet?sfilename=" + sSnippetName, true);
			sSnippetContent = Master.CleanResponseForEditor(sSnippetContent);

			// remove the meta tags (since they are merely added back in on submit)
			Regex pRegex = new Regex(s_sMetaPattern);
			sSnippetContent = pRegex.Replace(sSnippetContent, "");

			// fill fields
			txtSnippetContent.Text = sSnippetContent;
			txtSourceName.Text = sSnippetSourceName;
			txtSourceText.Text = sSnippetSourceText;
			txtTagList.Text = sSnippetTags;

			this.Title = "Edit Snippet";
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

		private void ListTags()
		{
			// read tag and source files
			List<string> lTags = File.ReadAllLines(m_sBaseDir + "_tagcache.dat").ToList();
			List<string> lSources = File.ReadAllLines(m_sBaseDir + "_sourcecache.dat").ToList();

			// take care of special tags (put them at top of list)
			foreach (string sGeneric in s_lGenericTags)
			{
				if (lTags.Contains(sGeneric)) lTags.Remove(sGeneric);
				lTags.Insert(0, sGeneric);
			}
			
			// tags
			foreach (string sTag in lTags)
			{
				// border container
				Border pBorder = new Border();
				pBorder.BorderThickness = new Thickness(0, 0, 0, 1);
				pBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(100, 70, 70, 70));
				pBorder.Background = new SolidColorBrush(Color.FromArgb(100, 40, 40, 40));
				pBorder.MouseEnter += delegate { pBorder.Background = new SolidColorBrush(Color.FromArgb(100, 60, 60, 60)); };
				pBorder.MouseLeave += delegate { pBorder.Background = new SolidColorBrush(Color.FromArgb(100, 40, 40, 40)); };
				pBorder.MouseUp += delegate
				{
					if (txtTagList.Text != "") { txtTagList.Text += ","; }
					txtTagList.Text += sTag;
				};
				
				// tag label
				TextBlock pTxtLabel = new TextBlock();
				pTxtLabel.Text = sTag;
				pTxtLabel.Foreground = new SolidColorBrush(Colors.White);
				if (s_lGenericTags.Contains(sTag)) pTxtLabel.Foreground = new SolidColorBrush(Color.FromRgb(69, 186, 255));
				pTxtLabel.Padding = new Thickness(10);
				pTxtLabel.HorizontalAlignment = HorizontalAlignment.Stretch;

				// add all the things!
				pBorder.Child = pTxtLabel;
				stkTags.Children.Add(pBorder);
			}
			
			// sources
			foreach (string sSource in lSources)
			{
				// fix text
				string sFixedSource = sSource.Remove(sSource.IndexOf("source:"), "source:".Length);
			
				// border container
				Border pBorder = new Border();
				pBorder.BorderThickness = new Thickness(0, 0, 0, 1);
				pBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(100, 70, 70, 70));
				pBorder.Background = new SolidColorBrush(Color.FromArgb(100, 40, 40, 40));
				pBorder.MouseEnter += delegate { pBorder.Background = new SolidColorBrush(Color.FromArgb(100, 60, 60, 60)); };
				pBorder.MouseLeave += delegate { pBorder.Background = new SolidColorBrush(Color.FromArgb(100, 40, 40, 40)); };
				//pBorder.MouseUp += delegate { lblSourceName.Content = sFixedSource; };
				pBorder.MouseUp += delegate { txtSourceName.Text = sFixedSource; };
				
				// tag label
				TextBlock pTxtLabel = new TextBlock();
				pTxtLabel.Text = sFixedSource;
				pTxtLabel.Foreground = new SolidColorBrush(Colors.White);
				pTxtLabel.Padding = new Thickness(10);
				pTxtLabel.HorizontalAlignment = HorizontalAlignment.Stretch;

				// add all the things!
				pBorder.Child = pTxtLabel;
				stkSources.Children.Add(pBorder);
			}
		}

		private void btnSubmit_MouseLeave(object sender, MouseEventArgs e) { btnSubmit.Background = new SolidColorBrush(Color.FromRgb(21, 21, 21)); }

		private void btnSubmit_MouseEnter(object sender, MouseEventArgs e) { btnSubmit.Background = new SolidColorBrush(Color.FromRgb(69, 186, 255)); }

		private void btnSubmit_MouseUp(object sender, MouseButtonEventArgs e)
		{
			// add snippet code here
			lblStatus.Content = "Submitting...";

			string sContent = txtSnippetContent.Text;
			//string sTags = txtTagList.Text + ",source:" + lblSourceName.Content;
			string sTags = txtTagList.Text + ",source:" + txtSourceName.Text;

			// add meta tags
			//sContent = "<meta name='sourceTag' content='" + lblSourceName.Content + "'><meta name='source' content='" + txtSourceText.Text + "'>" + sContent;
			sContent = "<meta name='sourceTag' content='" + txtSourceName.Text + "'><meta name='source' content='" + txtSourceText.Text + "'>" + sContent;

			// make the xml request body
			string sBody = "<params>";
			sBody += "<param name='sTagList'>" + sTags + "</param><param name='sSnippet'>" + Master.EncodeXML(sContent) + "</param>";
			if (m_bEditing) { sBody += "<param name='sFileName'>" + m_sEditingSnippet + "</param>"; }
			sBody += "</params>";

			string sResponse = "";
			if (m_bEditing) { sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/EditSnippet", sBody, true); }
			else { sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/AddSnippet", sBody, true); }

			txtSnippetContent.Text = "";

			if (sResponse != "") { lblStatus.Content = "Error!"; File.WriteAllText(m_sBaseDir + "errordump.txt", sResponse); return; }
			lblStatus.Content = "Submitted!";
			if (m_bEditing) 
			{
				Master.GetMainWindow().ShowPage(Master.GetMainWindow().ActiveQuery, true);
				this.Close(); 
			}
		}

		private void txtSnippetContent_TextChanged(object sender, TextChangedEventArgs e)
		{
			lblStatus.Content = "";
		}

		private void btnDelete_MouseLeave(object sender, MouseEventArgs e)
		{
			btnDelete.Background = new SolidColorBrush(Color.FromRgb(170, 37, 37));
		}

		private void btnDelete_MouseEnter(object sender, MouseEventArgs e)
		{
			btnDelete.Background = new SolidColorBrush(Color.FromRgb(200, 80, 80));
		}

		private void btnDelete_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (!m_bEditing) { return; }

			string sResponse = "";
			
			// msgbox verify
			if (MessageBox.Show("Are you sure you want to delete this snippet?", "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				sResponse = WebCommunications.SendGetRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/DeleteSnippet?sfilename=" + m_sEditingSnippet, true);
				if (sResponse != "") { lblStatus.Content = "Error!"; File.WriteAllText(m_sBaseDir + "errordump.txt", sResponse); return; }
				
				Master.GetMainWindow().ShowPage(Master.GetMainWindow().ActiveQuery, true);
				this.Close(); 
			}
			else { return; }
		}
	}
}
