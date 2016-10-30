using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Widget;
using Android.OS;

namespace App
{
	[Activity(Label = "App", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		int count = 1;

		private string[] m_aNavTitles;
		private DrawerLayout m_pDrawerLayout;
		private ListView m_pDrawerList;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			//Button button = FindViewById<Button>(Resource.Id.MyButton);

			//button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };

			string[] m_aNavTitles = new string[] { "thing1", "thing2" };

			m_pDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.appDrawerLayout);
			m_pDrawerList = FindViewById<ListView>(Resource.Id.appDrawerList);

			m_pDrawerList.Adapter = new DrawerItemCustomAdapter(this, Resource.Layout.ListViewItemRow, m_aNavTitles);
		}
	}
}

