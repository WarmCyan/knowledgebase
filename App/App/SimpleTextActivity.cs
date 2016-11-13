using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App
{
	[Activity(Label = "SimpleTextActivity")]
	public class SimpleTextActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.SimpleText);

			string sTitle = this.Intent.GetStringExtra("Title");
			string sContent = this.Intent.GetStringExtra("Content");

			this.Title = sTitle;
			TextView pTextView = FindViewById<TextView>(Resource.Id.lblSimple);
			pTextView.Text = sContent;
		}
	}
}