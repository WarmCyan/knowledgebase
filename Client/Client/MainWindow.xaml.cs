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
using System.Configuration;

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

		private Image m_pStartupImage;
		private bool m_bStartupDisplayed = true;

		// construction
		public MainWindow()
		{
			InitializeComponent();
			WebCommunications.AuthKey = ConfigurationManager.AppSettings["WebCommunicationsAuthKey"];
			m_dPageStack = new Dictionary<string, Page>(); // collection of pages with their associated query strings
			m_dPageStackLabels = new Dictionary<string, Border>(); // collection of the sidebar page labels with associated query strings

			// make rendering not suck
			CefSettings pSettings = new CefSettings();
			pSettings.SetOffScreenRenderingBestPerformanceArgs();
			Cef.Initialize(pSettings);

			Master.AssignMainWindow(this);

			//this.ShowPage("Genetic_Algorithm");
			/*this.ShowPage("Test");
			
			BitmapImage pOrigImage = new BitmapImage(new Uri(@"pack://application:,,,/Logo_256.png"));
			m_pStartupImage = new Image();
			
			m_pStartupImage.Source = pOrigImage;
			m_pStartupImage.Width = 256;
			m_pStartupImage.Height = 256;

			
			cnvsMain.Children.Add(m_pStartupImage);
			Canvas.SetLeft(m_pStartupImage, (this.ActualWidth-250) / 2);
			Canvas.SetTop(m_pStartupImage, cnvsMain.ActualHeight / 2);*/
		}

		// properties
		public string ActiveQuery { get { return m_sActiveQuery; } set { m_sActiveQuery = value; } }

		// functions
		
		private void Query(string sQuery)
		{
			string sFixedQuery = HttpUtility.UrlEncode(sQuery);
			string sResponse = WebCommunications.SendGetRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/ConstructPage?squery=" + sFixedQuery, true);

			if (m_bStartupDisplayed) { cnvsMain.Children.Remove(m_pStartupImage); m_bStartupDisplayed = false; }

			// sanitize
			//sResponse = sResponse.Trim('\"');
			//sResponse = sResponse.Replace("\\\"", "\"");
			sResponse = Master.CleanResponse(sResponse);

			// clear canvas of last page
			this.RemoveActivePage();
			
			// create new page
			m_pActivePage = new Page();
			m_pActivePage.FillHtml(sResponse);
			m_sActiveQuery = sQuery;

			// add new page to things
			this.AddPageToStack(sQuery, m_pActivePage);
			this.DisplayActivePage();
			this.UpdatePageSize();
		}

		private void RemoveActivePage() { if (m_bPageRendered) { cnvsMain.Children.Remove(m_pActivePage); } m_bPageRendered = false; m_sActiveQuery = ""; }
		private void DisplayActivePage() { if (!m_bPageRendered) { cnvsMain.Children.Add(m_pActivePage); m_bPageRendered = true; } }

		public void ShowPage(string sQuery, bool bRefresh = false)
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
				if (m_pActivePage.Empty || bRefresh) 
				{
					// remove the bad page and the label for it
					stkPageStack.Children.Remove(m_dPageStackLabels[sQuery]);
					m_dPageStackLabels.Remove(sQuery);
					m_dPageStack.Remove(sQuery);
					this.Query(sQuery); 
				}
				this.DisplayActivePage();
			}
			// if didn't already have this page, query it
			else { this.Query(sQuery); }

			// highlight the current label in the sidebar
			foreach (Border pBorder in stkPageStack.Children)
			{
				if (pBorder.Child is Grid)
				{
					TextBlock pTxtLabel = (TextBlock)((Grid)pBorder.Child).Children[0];
					if (pTxtLabel.Text == sQuery) { pBorder.Background = new SolidColorBrush(Color.FromArgb(255, 69, 186, 255)); }
					else { pBorder.Background = new SolidColorBrush(Color.FromArgb(100, 40, 40, 40)); }
				}
			}
		}

		// add the page to a list to "store" and add a clickable label for it to the sidebar
		private void AddPageToStack(string sQuery, Page pPage)
		{
			m_dPageStack.Add(sQuery, pPage);

			// border container
			Border pBorder = new Border();
			pBorder.BorderThickness = new Thickness(0, 0, 0, 1);
			pBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(100, 70, 70, 70));
			pBorder.Background = new SolidColorBrush(Color.FromArgb(100, 40, 40, 40));
			pBorder.MouseEnter += delegate { if (sQuery != m_sActiveQuery) { pBorder.Background = new SolidColorBrush(Color.FromArgb(100, 60, 60, 60)); } };
			pBorder.MouseLeave += delegate { if (sQuery != m_sActiveQuery) { pBorder.Background = new SolidColorBrush(Color.FromArgb(100, 40, 40, 40)); } };

			Grid pGrid = new Grid();
			
			// query label
			TextBlock pTxtLabel = new TextBlock();
			pTxtLabel.Text = sQuery;
			pTxtLabel.Foreground = new SolidColorBrush(Colors.White);
			pTxtLabel.Padding = new Thickness(10);
			pTxtLabel.HorizontalAlignment = HorizontalAlignment.Stretch;
			pTxtLabel.MouseUp += delegate { this.ShowPage(sQuery); }; // NOTE: this is here because if on border, and user clicks on exit, it registers for both exit AND border!

			// close query button
			TextBlock pTxtExit = new TextBlock();
			pTxtExit.Text = "x";
			pTxtExit.Foreground = new SolidColorBrush(Colors.White);
			pTxtExit.Padding = new Thickness(10,8,10,0);
			pTxtExit.HorizontalAlignment = HorizontalAlignment.Right;
			pTxtExit.MouseUp += delegate
			{
				if (sQuery == m_sActiveQuery) { this.RemoveActivePage(); } // hide page if it's currently displayed
				m_dPageStack.Remove(sQuery);
				stkPageStack.Children.Remove(m_dPageStackLabels[sQuery]); // remove the label from the sidebar
				m_dPageStackLabels.Remove(sQuery);
			};
			pTxtExit.MouseEnter += delegate { pTxtExit.Foreground = new SolidColorBrush(Colors.Red); };
			pTxtExit.MouseLeave += delegate { pTxtExit.Foreground = new SolidColorBrush(Colors.White); };

			// add all the things!
			pGrid.Children.Add(pTxtLabel);
			pGrid.Children.Add(pTxtExit);
			pBorder.Child = pGrid;
			stkPageStack.Children.Add(pBorder);
			m_dPageStackLabels.Add(sQuery, pBorder);
		}

		// update the size of the browser 
		private void UpdatePageSize() 
		{
			if (m_pActivePage == null) { return; }
			m_pActivePage.Height = cnvsMain.ActualHeight;
			m_pActivePage.Width = cnvsMain.ActualWidth;
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e) { UpdatePageSize(); }

		private void txtQueryBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter) 
			{
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
			else if (e.Key == Key.F5) { if (m_bPageRendered) { m_pActivePage.Refresh(); } }
		}

		private void btnAddSnippet_MouseLeave(object sender, MouseEventArgs e) { btnAddSnippet.Background = new SolidColorBrush(Color.FromRgb(21, 21, 21)); }
		private void btnAddSnippet_MouseEnter(object sender, MouseEventArgs e) { btnAddSnippet.Background = new SolidColorBrush(Color.FromRgb(69, 186, 255)); }

		private void btnAddSnippet_MouseUp(object sender, MouseButtonEventArgs e)
		{
			AddSnippet pSnippetWindow = new AddSnippet();
			pSnippetWindow.Show();
		}
	}
}
