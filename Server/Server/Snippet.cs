using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KnowledgeBaseServer
{
	public class Snippet
	{

		// the content should have the name of the blob file for "source" (and then just a piece of text for if there's a page number associated with it or whatever)

		private static string s_sMetaPattern = @"<meta name='([a-zA-Z]*)' content='([a-zA-Z]*)'>";

	
		// member variables
		private string m_sFileName;
		private List<string> m_lTags;
		private string m_sContent;
		private string m_sSourceTag;

		// construction
		public Snippet() { }
		public Snippet(string sFileName, List<string> lTags)
		{
			this.FileName = sFileName;
			this.Tags = lTags;
		}

		// properties
		public string FileName { get { return m_sFileName; } set { m_sFileName = value; } }
		public List<string> Tags { get { return m_lTags; } set { m_lTags = value; } }
		public string Content { get { return m_sContent; } set { m_sContent = value; } }
		public string SourceTag { get { return m_sSourceTag; } set { m_sSourceTag = value; } }

		// functions
		public void ParseContent(string sContent) 
		{ 
			this.Content = sContent;
			this.FindMetaSource();
		}

		private void FindMetaSource()
		{
			MatchCollection pMatches = Regex.Matches(this.Content, s_sMetaPattern);
			foreach (Match pMatch in pMatches)
			{
				if (pMatch.Groups[1].Value == "sourceTag") { this.SourceTag = pMatch.Groups[2].Value; }
			}
		}
	}
}
