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
	[Activity(Label = "Query")]
	public class QueryActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.QueryLayout);

			//int iQueryNum = this.Intent.GetIntExtra("num", 0);

			// Create your application here

			TextView pTextView = FindViewById<TextView>(Resource.Id.txtStatus);
			EditText pEditText = FindViewById<EditText>(Resource.Id.txtQueryField);
			Button pButton = FindViewById<Button>(Resource.Id.btnQuery);

			this.Window.SetSoftInputMode(SoftInput.StateVisible);
			pEditText.RequestFocus();

			pTextView.Visibility = ViewStates.Invisible;

			pButton.Click += delegate
			{
				pTextView.Visibility = ViewStates.Visible;
				//Master.SetQuery(pEditText.Text);
				//Master.SetQueryNumber(iQueryNum);
				Intent pOriginalIntent = new Intent(this, typeof(MainActivity));
				pOriginalIntent.PutExtra("Query", pEditText.Text);
				SetResult(Result.Ok, pOriginalIntent);
				this.Finish();
			};
			
		}
	}
}