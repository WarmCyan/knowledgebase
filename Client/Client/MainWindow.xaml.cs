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

			
			/*Page pPage = new Page();
			pPage.Height = 800;
			pPage.FillHtml(sSample);
			stkDocContainer.Children.Add(pPage);*/
			this.Query("AI");
		}

		private void Query(string sQuery)
		{
			sQuery = HttpUtility.UrlEncode(sQuery);
			string sResponse = WebCommunications.SendGetRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/ConstructPage?squery=" + sQuery, true);

			// sanitize
			sResponse = sResponse.Trim('\"');
			sResponse = sResponse.Replace("\\\"", "\"");
			
			Page pPage = new Page();
			pPage.Height = 800;
			pPage.FillHtml(sResponse);
			stkDocContainer.Children.Add(pPage);
		}
	}
}
