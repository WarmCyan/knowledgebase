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
		private string m_sActiveQuery;
		private bool m_bPageRendered = false;

		private Dictionary<string, Page> m_dPageStack;
		private Dictionary<string, Border> m_dPageStackLabels;

		public MainWindow()
		{
			InitializeComponent();
			WebCommunications.AuthKey = "54003c32a190b6063fe06a528bc230ce151b589512db4a39ecf8ac01be393dafa154f39bde1f56e690f4c3c2870323972240d4d02fc4fa2f3349dc7ef4c7dc09";
			m_dPageStack = new Dictionary<string, Page>();
			m_dPageStackLabels = new Dictionary<string, Border>();

			// make rendering not suck
			CefSettings pSettings = new CefSettings();
			pSettings.SetOffScreenRenderingBestPerformanceArgs();
			Cef.Initialize(pSettings);

			// read in sample HTML
			//string sSample = File.ReadAllText(@"C:\dwl\lab\KnowledgeBase\Client\sample.html");

			this.ShowPage("Genetic_Algorithm");
		}

		private void Query(string sQuery)
		{
			string sFixedQuery = HttpUtility.UrlEncode(sQuery);
			string sResponse = WebCommunications.SendGetRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/ConstructPage?squery=" + sFixedQuery, true);

			// sanitize
			sResponse = sResponse.Trim('\"');
			sResponse = sResponse.Replace("\\\"", "\"");

			// clear canvas of last page
			this.RemoveActivePage();
			
			// create new page
			m_pActivePage = new Page();
			m_pActivePage.FillHtml(sResponse);
			m_sActiveQuery = sQuery;

			// add new page to things
			this.AddPageToStack(sQuery, m_pActivePage);
			this.DisplayActivePage();
			//cnvsMain.Children.Add(m_pActivePage); 
			//m_bPageRendered = true;
			this.UpdatePageSize();
		}

		private void RemoveActivePage() { if (m_bPageRendered) { cnvsMain.Children.Remove(m_pActivePage); } m_bPageRendered = false; }
		private void DisplayActivePage() { cnvsMain.Children.Add(m_pActivePage); m_bPageRendered = true; }

		private void ShowPage(string sQuery)
		{
			this.RemoveActivePage();

			// if the page already exists, just display it
			if (m_dPageStack.ContainsKey(sQuery)) 
			{
				m_pActivePage = m_dPageStack[sQuery];
				this.UpdatePageSize();
				m_sActiveQuery = sQuery;
				if (m_pActivePage.IsBlank()) { m_pActivePage.Refresh(); }

				// if the page didn't load properly, requery it
				if (m_pActivePage.Empty) 
				{
					// remove the bad page and the label for it
					stkPageStack.Children.Remove(m_dPageStackLabels[sQuery]);
					m_dPageStack.Remove(sQuery);
					this.Query(sQuery); 
				}
				this.DisplayActivePage();
			}
			// if didn't alreayd have this page, query it
			else { this.Query(sQuery); }

			// highlight the current one
			foreach (Border pBorder in stkPageStack.Children)
			{
				if (pBorder.Child is TextBlock)
				{
					TextBlock pTxtLabel = (TextBlock)pBorder.Child;
					if (pTxtLabel.Text == sQuery)
					{
						pBorder.Background = new SolidColorBrush(Color.FromArgb(255, 69, 186, 255));
					}
					else 
					{
						pBorder.Background = new SolidColorBrush(Color.FromArgb(100, 40, 40, 40));
					}
				}
			}
		}

		private void AddPageToStack(string sQuery, Page pPage)
		{
			m_dPageStack.Add(sQuery, pPage);

			Border pBorder = new Border();
			pBorder.BorderThickness = new Thickness(0, 0, 0, 1);
			pBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(100, 70, 70, 70));
			pBorder.Background = new SolidColorBrush(Color.FromArgb(100, 40, 40, 40));
			
			TextBlock pTxtLabel = new TextBlock();
			pTxtLabel.Text = sQuery;
			pTxtLabel.Foreground = new SolidColorBrush(Colors.White);
			pTxtLabel.Padding = new Thickness(10);
			pTxtLabel.MouseUp += delegate { this.ShowPage(sQuery); };
			pTxtLabel.MouseEnter += delegate 
			{
				if (sQuery != m_sActiveQuery) { pBorder.Background = new SolidColorBrush(Color.FromArgb(100, 60, 60, 60)); }
			};
			pTxtLabel.MouseLeave += delegate 
			{
				if (sQuery != m_sActiveQuery) { pBorder.Background = new SolidColorBrush(Color.FromArgb(100, 40, 40, 40)); }
			};

			pBorder.Child = pTxtLabel;
			stkPageStack.Children.Add(pBorder);
			m_dPageStackLabels.Add(sQuery, pBorder);
		}

		//private void UpdatePageSize() { m_pActivePage.Height = this.ActualHeight - s_iTitleBarOffset; }
		private void UpdatePageSize() 
		{ 
			m_pActivePage.Height = cnvsMain.ActualHeight/* - s_iTitleBarOffset*/;
			m_pActivePage.Width = cnvsMain.ActualWidth;
		}

		private void HideSearchBox() { txtQueryBox.Opacity = 0; }
		private void ShowSearchBox() { txtQueryBox.Opacity = .8; }

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e) { UpdatePageSize(); }

		private void txtQueryBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter) 
			{
				//this.Query(txtQueryBox.Text);
				this.ShowPage(txtQueryBox.Text);
				Keyboard.ClearFocus();
				txtQueryBox.Focusable = false;
				this.Focus();
			}
		}

		private void txtQueryBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			txtQueryBox.Focusable = true;
			txtQueryBox.Focus();
			txtQueryBox.SelectAll();
			e.Handled = true;
		}

		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.OemQuestion)
			{
				txtQueryBox.Focusable = true;
				txtQueryBox.Focus();
				txtQueryBox.SelectAll();
				e.Handled = true;
			}
		}

	}
}
