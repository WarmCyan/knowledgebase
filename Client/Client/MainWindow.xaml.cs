using CefSharp;
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

			stkDocContainer.Children.Add(Block.ConstructBlock("<h3>Hello World!</h3>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			stkDocContainer.Children.Add(Block.ConstructBlock("<h1>OH YEAH! THIS IS A TITLE!!!</h1>"));
			
			//stkDocContainer.Children.Add(Block.ConstructBlock("This is really just a test to see what happens when I do things...I really don't want to have an issue with text not being very good, or not having proper testing content, so this is my attempt to rectifity that terribly severe issue."));
			/*stkDocContainer.Children.Add(Block.ConstructBlock("## Hello World!"));
			stkDocContainer.Children.Add(Block.ConstructBlock("### Hello World!"));
			stkDocContainer.Children.Add(Block.ConstructBlock("#### Hello World!"));
			stkDocContainer.Children.Add(Block.ConstructBlock("##### Hello World!"));
			stkDocContainer.Children.Add(Block.ConstructBlock("###### Hello World!"));*/
		}
	}
}
