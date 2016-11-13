using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Java.Interop;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Webkit;

namespace App
{
	class JSInterface : Java.Lang.Object
	{
		// member variables
		private Context m_pContext;

		// construction
		public JSInterface(Context pContext)
		{
			m_pContext = pContext;
		}

		[Export]
		[JavascriptInterface]
		public void DisplaySource(string sSourceName, string sSourceText)
		{
			// show source stuff here, in a message box
		}

		[Export]
		[JavascriptInterface]
		public void DisplayEdit(string sSnippetName, string sSnippetSourceName, string sSnippetSourceText, string sSnippetTags)
		{
			Intent pIntent = new Intent(m_pContext, (new PageActivity()).Class);
			pIntent.PutExtra("Type", "Edit");
			pIntent.PutExtra("SnippetName", sSnippetName);
			pIntent.PutExtra("SourceName", sSnippetSourceName);
			pIntent.PutExtra("SourceText", sSnippetSourceText);
			pIntent.PutExtra("SnippetTags", sSnippetTags);
			m_pContext.StartActivity(pIntent);
		}
	}
}