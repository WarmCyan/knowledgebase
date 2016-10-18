﻿using System;
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

		public static bool IsCacheRefreshed() { return s_bCacheRefreshed; }
		public static void SetCacheRefreshed(bool sRefreshed) { s_bCacheRefreshed = sRefreshed; }

		public static string CleanResponse(string sResponse) { return sResponse.Trim('\"').Replace("\\\"", "\"").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\\\", "\\"); }
		public static string EncodeXML(string sXML) { return sXML.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;"); }
	}
}
