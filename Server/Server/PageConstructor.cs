using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KnowledgeBaseServer
{
	public class PageConstructor
	{
		private static List<string> s_lQueryTags;

		private static List<string> s_lGenericTags = new List<string>() { "Definition", "Theory", "Argument", "Wisdom", "Note", "Depth" };

		private static string s_sInterestTag = "";

		// return page HTML
		public static string Construct(string sOriginalQuery, List<Snippet> lSnippets)
		{
			s_lQueryTags = new List<string>();
		
			// title/query list
			string sHTML = "<body>";
			sHTML += TitleHtml(sOriginalQuery);
			sHTML += "<div class='separator'></div>";

			// find tag of interest
			DetermineInterestTag();

			// find definition or theory related directly to tag of interest
			sHTML += InterestTagHtml(lSnippets);

			sHTML += "</body>";
			return sHTML;
		}

		private static string InterestTagHtml(List<Snippet> lSnippets)
		{
			string sHTML = "";
			// find a snippet with a tag of InterestTag_Definition or InterestTag_Theory
			foreach (Snippet pSnippet in lSnippets)
			{
				if (pSnippet.Tags.Contains(s_sInterestTag + "_Definition") || pSnippet.Tags.Contains(s_sInterestTag + "_Theory"))
				{
					sHTML += "<div class='snippet primary'>";

					// get the body of the snippet (ignore source for now)
					XElement pXML = XElement.Parse(pSnippet.Content);
					sHTML += pXML.Element("snippet").Value;
				
					sHTML += "</div>";
				}
			}
			return sHTML;
		}

		private static void DetermineInterestTag()
		{
			// go backwards and find first non generic tag
			for (int i = s_lQueryTags.Count - 1; i >= 0; i--)
			{
				string sTag = s_lQueryTags[i];
				if (s_lGenericTags.Contains(sTag)) { continue; }
				s_sInterestTag = sTag;
				return;
			}
		}

		private static string TitleHtml(string sOriginalQuery)
		{
			s_lQueryTags = sOriginalQuery.Split(',').ToList();

			string sTagString = "";
			foreach (string sTag in s_lQueryTags) { sTagString += sTag.Replace('_', ' ') + " "; }
			sTagString = sTagString.Trim();

			string sHTML = "<div class='snippet title'><h1>" + sTagString + "</h1></div>";
			return sHTML;
		}
	}
}
