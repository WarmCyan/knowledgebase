using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
	public class Master
	{
		// variables 
		private static bool s_bCacheRefreshed = false;
		private static MainWindow s_pWindow = null;

		// properties
		public static MainWindow GetMainWindow() { return s_pWindow; }

		// functions
		public static bool IsCacheRefreshed() { return s_bCacheRefreshed; }
		public static void SetCacheRefreshed(bool sRefreshed) { s_bCacheRefreshed = sRefreshed; }

		public static string CleanResponseForEditor(string sResponse) { return sResponse.Trim('\"').Replace("\\\"", "\"").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\\\", "\\"); }
		public static string CleanResponse(string sResponse) { return sResponse.Trim('\"').Replace("\\\"", "\"").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\\\", "\\").Replace("\\|t", "\\t").Replace("\\|n", "\\n"); }
		public static string EncodeXML(string sXML) { return sXML.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;"); }

		public static void AssignMainWindow(MainWindow pWindow) { s_pWindow = pWindow; }

		//public static void ShowPage(string sQuery) { s_pWindow.ShowPage(sQuery, true); }
	}
}
