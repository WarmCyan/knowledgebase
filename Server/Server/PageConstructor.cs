using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeBaseServer
{
	public class PageConstructor
	{
		private static List<string> s_lTags;
	
		// return page HTML
		public static string Construct(string sOriginalQuery, List<Snippet> lSnippets)
		{
			s_lTags = new List<string>();
		
			string sHTML = "<body>";
			sHTML += TitleHtml(sOriginalQuery);
			sHTML += "<div class='separator'></div>";

			return sHTML;
		}

		private static string TitleHtml(string sOriginalQuery)
		{
			s_lTags = sOriginalQuery.Split(',').ToList();

			string sTagString = "";
			foreach (string sTag in s_lTags) { sTagString += sTag.Replace('_', ' ') + " "; }
			sTagString = sTagString.Trim();

			string sHTML = "<div class='snippet title'><h1>" + sTagString + "</h1></div>";
			return sHTML;
		}
	}
}
