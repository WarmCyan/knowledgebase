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
		
		public Page()
		{
			InitializeComponent();
			this.InitializeBrowser();
			this.GetCSS();
			this.GetHeaderHTML();
		}

		public void FillHtml(string sHtml)
		{
			sHtml = "<html><head>" + m_sHeaderCode + "<style>" + m_sCSS + "</style></head>" + sHtml + "</html>";
			string sFauxURL = "http://page.html";

			m_pBrowser.LoadHtml(sHtml, sFauxURL);
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
