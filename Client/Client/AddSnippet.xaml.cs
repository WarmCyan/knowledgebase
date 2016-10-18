﻿using System;
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
using DWL.Utility;

namespace Client
{
	/// <summary>
	/// Interaction logic for AddSnippet.xaml
	/// </summary>
	public partial class AddSnippet : Window
	{

		// member variables
		private string m_sBaseDir = AppDomain.CurrentDomain.BaseDirectory;

	
		public AddSnippet()
		{
			InitializeComponent();

			// update local tag cache
			if (!Master.IsCacheRefreshed()) { this.UpdateTagCache(); }
			ListTags();
			
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

			// take care of special tags
			if (lTags.Contains("Definition")) lTags.Remove("Definition");
			if (lTags.Contains("Theory")) lTags.Remove("Theory");
			if (lTags.Contains("Argument")) lTags.Remove("Argument");
			if (lTags.Contains("Wisdom")) lTags.Remove("Wisdom");
			if (lTags.Contains("Note")) lTags.Remove("Note");
			if (lTags.Contains("Depth")) lTags.Remove("Depth");
			if (lTags.Contains("Important")) lTags.Remove("Important");
			lTags.Insert(0, "Wisdom");
			lTags.Insert(0, "Theory");
			lTags.Insert(0, "Note");
			lTags.Insert(0, "Important");
			lTags.Insert(0, "Depth");
			lTags.Insert(0, "Definition");
			lTags.Insert(0, "Argument");

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
				pBorder.MouseUp += delegate { lblSourceName.Content = sFixedSource; };
				
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

		private void btnSource_MouseLeave(object sender, MouseEventArgs e) { btnAddSource.Background = new SolidColorBrush(Color.FromRgb(21, 21, 21)); }

		private void btnSource_MouseEnter(object sender, MouseEventArgs e) { btnAddSource.Background = new SolidColorBrush(Color.FromRgb(69, 186, 255)); }

		private void btnSource_MouseUp(object sender, MouseButtonEventArgs e)
		{
			// open add source window here
		}

		private void btnSubmit_MouseLeave(object sender, MouseEventArgs e) { btnSubmit.Background = new SolidColorBrush(Color.FromRgb(21, 21, 21)); }

		private void btnSubmit_MouseEnter(object sender, MouseEventArgs e) { btnSubmit.Background = new SolidColorBrush(Color.FromRgb(69, 186, 255)); }

		private void btnSubmit_MouseUp(object sender, MouseButtonEventArgs e)
		{
			// add snippet code here
			lblStatus.Content = "Submitting...";

			string sContent = txtSnippetContent.Text;
			string sTags = txtTagList.Text + ",source:" + lblSourceName.Content;

			// add meta tags
			sContent = "<meta name='sourceTag' content='" + lblSourceName.Content + "'><meta name='source' content='" + txtSourceText.Text + "'>" + sContent;

			// make the xml request body
			string sBody = "<params><param name='sTagList'>" + sTags + "</param><param name='sSnippet'>" + Master.EncodeXML(sContent) + "</param></params>";
			string sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/AddSnippet", sBody, true);

			txtSnippetContent.Text = "";

			if (sResponse != "") { lblStatus.Content = "Error!"; File.WriteAllText(m_sBaseDir + "errordump.txt", sResponse); return; }
			lblStatus.Content = "Submitted!";
		}

		private void txtSnippetContent_TextChanged(object sender, TextChangedEventArgs e)
		{
			lblStatus.Content = "";
		}
	}
}
