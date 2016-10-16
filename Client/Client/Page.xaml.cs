using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp.Wpf;
using CefSharp;

namespace Client
{
	/// <summary>
	/// Interaction logic for Page.xaml
	/// </summary>
	public partial class Page : UserControl
	{
		// member variables
		private ChromiumWebBrowser m_pBrowser;
		private string m_sCSS;
		private string m_sHeaderCode;
		private string m_sHtml;
		private bool m_bIsEmpty = false;
		
		public Page()
		{
			InitializeComponent();
			this.InitializeBrowser();
			this.GetCSS();
			this.GetHeaderHTML();
		}

		// properties
		public bool Empty { get { return m_bIsEmpty; } set { m_bIsEmpty = value; } }

		// functions

		public void FillHtml(string sHtml)
		{
			if (sHtml == "") { m_bIsEmpty = true; }
			m_sHtml = "<html><head>" + m_sHeaderCode + "<style>" + m_sCSS + "</style></head>" + sHtml + "</html>";
			string sFauxURL = "http://page.html";

			m_pBrowser.LoadHtml(m_sHtml, sFauxURL);
			m_pBrowser.Address = sFauxURL;
		}

		public bool IsBlank()
		{
			Task<string> pSourceTask = m_pBrowser.GetSourceAsync();
			pSourceTask.Wait();
			if (pSourceTask.Result == "<html><head></head><body></body></html>") { return true; }
			return false;
		}


		public void Refresh()
		{
			string sFauxURL = "http://page.html";

			m_pBrowser.LoadHtml(m_sHtml, sFauxURL);
			m_pBrowser.Address = sFauxURL;
		}


		// load the css
		private void GetCSS() { m_sCSS = File.ReadAllText("./ClientStyle.css"); }

		// load the header html stuff
		private void GetHeaderHTML() { m_sHeaderCode = File.ReadAllText("./HeadStuff.html"); }

		// initialize cefbrowser stuff
		private void InitializeBrowser()
		{
			m_pBrowser = new ChromiumWebBrowser();
			m_pBrowser.Height = this.Height;
			m_pBrowser.Width = this.Width;
			grdContent.Children.Add(m_pBrowser);
		}
	}
}
