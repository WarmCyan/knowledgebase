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


namespace Client
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			
			// make rendering not suck
			CefSettings pSettings = new CefSettings();
			pSettings.SetOffScreenRenderingBestPerformanceArgs();
			Cef.Initialize(pSettings);


			// read in sample HTML
			string sSample = File.ReadAllText(@"C:\dwl\lab\KnowledgeBase\Client\sample.html");

			//stkDocContainer.Children.Add(Block.ConstructBlock(@"<h1>Hello World!</h1><p>\(\alpha = \beta\)"));
			Page pPage = new Page();
			pPage.Height = 800;
			pPage.FillHtml(sSample);
			stkDocContainer.Children.Add(pPage);
		}
	}
}
