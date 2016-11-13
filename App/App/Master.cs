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
	class Master
	{
		// variables
		private static bool s_bCacheRefreshed = false;
		private static string s_sQuery = "";
		private static int s_iQueryNum = 0; // using this as a "verifier" that sQuery was actually changed

		// functions
		public static bool IsCacheRefreshed() { return s_bCacheRefreshed; }
		public static void SetCacheRefreshed(bool sRefreshed) { s_bCacheRefreshed = sRefreshed; }

		public static string CleanResponse(string sResponse) { return sResponse.Trim('\"').Replace("\\\"", "\"").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\\\", "\\"); }
		public static string EncodeXML(string sXML) { return sXML.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;"); }

		public static void SetQuery(string sQuery) { s_sQuery = sQuery; }
		public static string GetQuery() { return s_sQuery; }
		public static void SetQueryNumber(int iQueryNum) { s_iQueryNum = iQueryNum; }
		public static int GetQueryNumber() { return s_iQueryNum; }
	}
}