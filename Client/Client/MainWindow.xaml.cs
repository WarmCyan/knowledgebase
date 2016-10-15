using CefSharp;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DWL.Utility;
using System.Web;

namespace Client
{
	public partial class MainWindow : Window
	{
		// static variables
		//private static int s_iTitleBarOffset = 40;

		// member variables
		private Page m_pActivePage;
		private bool m_bPageRendered = false;

		public MainWindow()
		{
			InitializeComponent();
			WebCommunications.AuthKey = "54003c32a190b6063fe06a528bc230ce151b589512db4a39ecf8ac01be393dafa154f39bde1f56e690f4c3c2870323972240d4d02fc4fa2f3349dc7ef4c7dc09";

			// make rendering not suck
			CefSettings pSettings = new CefSettings();
			pSettings.SetOffScreenRenderingBestPerformanceArgs();
			Cef.Initialize(pSettings);

			// read in sample HTML
			//string sSample = File.ReadAllText(@"C:\dwl\lab\KnowledgeBase\Client\sample.html");

			this.Query("Genetic_Algorithm");
		}

		private void Query(string sQuery)
		{
			sQuery = HttpUtility.UrlEncode(sQuery);
			string sResponse = WebCommunications.SendGetRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/ConstructPage?squery=" + sQuery, true);

			// sanitize
			sResponse = sResponse.Trim('\"');
			sResponse = sResponse.Replace("\\\"", "\"");

			if(m_bPageRendered) { cnvsMain.Children.Remove(m_pActivePage); }
			m_pActivePage = new Page();
			m_pActivePage.FillHtml(sResponse);
			//stkDocContainer.Children.Add(m_pActivePage);
			cnvsMain.Children.Add(m_pActivePage); 
			m_bPageRendered = true;
			this.UpdatePageSize();
		}

		//private void UpdatePageSize() { m_pActivePage.Height = this.ActualHeight - s_iTitleBarOffset; }
		private void UpdatePageSize() 
		{ 
			m_pActivePage.Height = cnvsMain.ActualHeight/* - s_iTitleBarOffset*/;
			m_pActivePage.Width = cnvsMain.ActualWidth;
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e) { UpdatePageSize(); }

		private void TextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				this.Query(txtQueryBox.Text);
			}
		}
	}
}
