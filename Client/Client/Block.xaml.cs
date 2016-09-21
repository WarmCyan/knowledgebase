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
using CefSharp.Wpf;
using System.Threading;
using CefSharp;
using CefSharp.Internals;

namespace Client
{
	/// <summary>
	/// Interaction logic for Block.xaml
	/// </summary>
	public partial class Block : UserControl
	{
		// member variables
		private static SolidColorBrush s_pForegroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BBBBBB"));

		private ChromiumWebBrowser m_pBrowser;
		private string m_sStylesheet = @"
			<style>
				body
				{
					font-family: Arial;
					color: rgb(255,255,255);
				}
			</style>
		";

		private Block()
		{
			InitializeComponent();

			
			m_pBrowser = new ChromiumWebBrowser();
			m_pBrowser.Height = 10;
			m_pBrowser.Width = 800;
			grdContent.Children.Add(m_pBrowser);


			//m_pBrowser.MouseDoubleClick += delegate { Resize(); };
			m_pBrowser.Loaded += delegate { /*Thread.Sleep(1000);*/  Resize(); };
			//m_pBrowser.FrameLoadEnd += delegate { /*Thread.Sleep(1000);*/  Resize(); };
			//m_pBrowser.LoadHandler.OnLoadingStateChange += delegate { Resize(); };
			//m_pBrowser.LoadingStateChanged += delegate { /*Thread.Sleep(1000);*/  Resize(); };
		}

		public async void Resize()
		{

			//Thread.Sleep(100);
			//while (m_pBrowser.IsLoading) { Thread.Sleep(100); }
			//Task<JavascriptResponse> tHeightScript = m_pBrowser.EvaluateScriptAsync("(function() { var body = document.body, html = document.documentElement; return Math.max(body.scrollHeight, body.offsetHeight, html.clientHeight); })();");
			//tHeightScript.Wait();
			while (m_pBrowser.GetBrowser() == null || m_pBrowser.GetBrowser().IsLoading) { Thread.Sleep(10); }
			JavascriptResponse pHeightScript = await m_pBrowser.EvaluateScriptAsync("(function() { var body = document.body, html = document.documentElement; return Math.max(body.scrollHeight, body.offsetHeight, html.clientHeight); })();");

			int iHeight = 0;
			if (pHeightScript.Result != null) { iHeight = Convert.ToInt32(pHeightScript.Result.ToString()); }

			m_pBrowser.Height = iHeight + 1;
		}

		private void FillByText(string sText)
		{
			//string myhtml = @"<html><head>" + m_sStylesheet + @"<script src='https://cdn.mathjax.org/mathjax/latest/MathJax.js?config=TeX-MML-AM_CHTML' async></script></head><body><h1>Hello world</h1><p>\(\alpha = \beta\)</p></body></html>";
			string myhtml = @"<html><head>" + m_sStylesheet + @"<script src='https://cdn.mathjax.org/mathjax/latest/MathJax.js?config=TeX-MML-AM_CHTML' async></script></head><body>" + sText + "</body></html>";
			string myurl = "http://test.html";

			//var handler = m_pBrowser.ResourceHandlerFactory as DefaultResourceHandlerFactory;
			//handler.RegisterHandler(myurl, ResourceHandler.FromString(myhtml));
			//m_pBrowser.Load(myurl);
			//m_pBrowser.Address = myurl;

			m_pBrowser.LoadHtml(myhtml, myurl);
			m_pBrowser.Address = myurl;

			//.m_pBrowser.Measure(new Size(1000,1000));
			
			
			//m_pBrowser.Height = m_pBrowser.DesiredSize.Height;
			//m_pBrowser.Width = m_pBrowser.DesiredSize.Width;

			
			// dealing with some kind of title
			/*if (sText.StartsWith("#"))
			{
				int iIndexOfSpace = sText.IndexOf(" ");
				int iHeaderLevel = iIndexOfSpace;
				string sActualText = sText.Substring(iIndexOfSpace + 1);
				//string sActualText = sText.Trim(new char([' ', '#']));

				Label lblHeaderLevel = new Label();
				//lblHeaderLevel.FontWeight = FontWeights.Bold;
				lblHeaderLevel.Content = sActualText;
				lblHeaderLevel.Foreground = s_pForegroundBrush;
				switch (iHeaderLevel)
				{
					case 1:
						lblHeaderLevel.FontSize = 48;
						break;
					case 2:
						lblHeaderLevel.FontSize = 36;
						break;
					case 3:
						lblHeaderLevel.FontSize = 30;
						break;
					case 4:
						lblHeaderLevel.FontSize = 24;
						break;
					case 5:
						lblHeaderLevel.FontSize = 18;
						break;
					case 6:
						lblHeaderLevel.FontSize = 14;
						break;
				}

				// add stuff to grid
				grdContent.Children.Add(lblHeaderLevel);
			}
			// normal text
			else
			{
				TextBlock lblText = new TextBlock();
				lblText.Text = sText;
				lblText.TextWrapping = TextWrapping.Wrap;
				lblText.FontSize = 12;
				lblText.Foreground = s_pForegroundBrush;
				grdContent.Children.Add(lblText);
			}*/
		}
		
		public static Block ConstructBlock(string sMarkdown)
		{
			Block pBlock = new Block();
			pBlock.FillByText(sMarkdown);
			return pBlock;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
		}
	}

}
