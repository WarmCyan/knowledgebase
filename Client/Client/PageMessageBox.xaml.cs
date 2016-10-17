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

namespace Client
{
	/// <summary>
	/// Interaction logic for MessageBox.xaml
	/// </summary>
	public partial class PageMessageBox : Window
	{

		// member variables
		private double m_dLeft;
		private double m_dTop;
		private bool m_bDragging = false;

		private bool m_bPrepareToClose = false;

	
		public PageMessageBox()
		{
			InitializeComponent();
		}


		public void InformationalTextBox(string sTitle, string sContent)
		{
			lblTitle.Content = sTitle;
			txtContent.Text = sContent;
			this.Show();
		}

		private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e) { }

		private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e) { }

		private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed) { this.DragMove(); }
			if (e.RightButton == MouseButtonState.Pressed) { m_bPrepareToClose = true; }
		}
		private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.RightButton == MouseButtonState.Released && m_bPrepareToClose) { this.Close(); }
		}

		private void txtContent_MouseDown(object sender, MouseButtonEventArgs e)
		{
			// TODO: make this use a regex instead
			if (txtContent.Text.Contains("http"))
			{
				System.Diagnostics.Process.Start(txtContent.Text);
			}
		}

	}
}
