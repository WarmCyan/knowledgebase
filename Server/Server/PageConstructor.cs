using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KnowledgeBaseServer
{
	public class Page
	{
		// static variables
		private static List<string> s_lGenericTags = new List<string>() { "Definition", "Theory", "Argument", "Wisdom", "Note", "Depth" };
		
		// member variables
		private List<string> m_lQueryTags = new List<string>();
		private string m_sInterestTag = "";
		private List<Snippet> m_lOriginalSnippets = new List<Snippet>();
		private List<PageSnippet> m_lPageSnippets = new List<PageSnippet>();

		// construction
		public Page() { }

		// return page HTML
		public string Construct(string sOriginalQuery, List<Snippet> lSnippets)
		{
			m_lOriginalSnippets = lSnippets;

			// get query tags
			m_lQueryTags = sOriginalQuery.Split(',').ToList();
		
			// tag of interest
			this.DetermineInterestTag();
			this.CreateInterestTagSnippet();

			this.HighLevelDefinitions();
			this.HighLevelTheories();
			this.NormalStuff();

			return this.BuildPage(sOriginalQuery);		
		}


		private string BuildPage(string sOriginalQuery)
		{
			string sHTML = "";
			
			// title/query list
			sHTML += "<body>";
			sHTML += TitleHtml(sOriginalQuery);
			sHTML += "<div class='separator'></div>";

			// get all the page snippets
			List<PageSnippet> lInterests = this.FindSectionSnippets(PageSection.Interest);
			List<PageSnippet> lDefinitions = this.FindSectionSnippets(PageSection.Definitions);
			List<PageSnippet> lTheories = this.FindSectionSnippets(PageSection.Theories);
			List<PageSnippet> lNotes = this.FindSectionSnippets(PageSection.Notes);
			List<PageSnippet> lNormal = this.FindSectionSnippets(PageSection.Normal);
			List<PageSnippet> lDepth = this.FindSectionSnippets(PageSection.Depth);
			List<PageSnippet> lArguments = this.FindSectionSnippets(PageSection.Arguments);

			// build all the page snippets 
			// interest
			foreach (PageSnippet pSnippet in lInterests) { sHTML += pSnippet.Build(); }

			// definitions
			if (lDefinitions.Count > 0) { sHTML += "<div class='templatepart'><h2>Definitions</h2></div>"; }
			foreach (PageSnippet pSnippet in lDefinitions) { sHTML += pSnippet.Build(); }

			// theories
			if (lTheories.Count > 0) { sHTML += "<div class='templatepart'><h2>Theories</h2></div>"; }
			foreach (PageSnippet pSnippet in lTheories) { sHTML += pSnippet.Build(); }

			if (lTheories.Count > 0 || lDefinitions.Count > 0) { sHTML += "<div class='separator'></div>"; }

			// other stuff
			foreach (PageSnippet pSnippet in lNormal) { sHTML += pSnippet.Build(); }
			
			// notes
			//if (lNotes.Count > 0) { sHTML += "<div class='templatepart'><h2>Notes</h2></div>"; }
			// TODO: don't forget can make other sections too 
			foreach (PageSnippet pSnippet in lNotes) { sHTML += pSnippet.Build(); }
			
			if (lNormal.Count > 0 || lNotes.Count > 0) { sHTML += "<div class='separator'></div>"; }
			
			// depth
			if (lDepth.Count > 0) { sHTML += "<div class='templatepart'><h2>Depth</h2></div>"; }
			foreach (PageSnippet pSnippet in lDepth) { sHTML += pSnippet.Build(); }
			
			// arguments
			if (lArguments.Count > 0) { sHTML += "<div class='templatepart'><h2>Arguments</h2></div>"; }
			foreach (PageSnippet pSnippet in lArguments) { sHTML += pSnippet.Build(); }

			// things?
			
			sHTML += "</body>";
			return sHTML;
		}

		private void CreateInterestTagSnippet()
		{
			// find a snippet with a tag of InterestTag_Definition or InterestTag_Theory
			foreach (Snippet pSnippet in m_lOriginalSnippets)
			{
				if (pSnippet.Tags.Contains(m_sInterestTag + "_Definition") || pSnippet.Tags.Contains(m_sInterestTag + "_Theory"))
				{
					PageSnippet pPageSnippet = new PageSnippet(pSnippet, 4, PageSection.Interest);
					m_lPageSnippets.Add(pPageSnippet);
				}
			}
		}

		// get html for all high level definitions (ones that don't have depth tag)
		private void HighLevelDefinitions()
		{
			foreach (Snippet pSnippet in m_lOriginalSnippets)
			{
				if (pSnippet.Tags.Contains("Definition") && !pSnippet.Tags.Contains("Depth") && !pSnippet.Tags.Contains(m_sInterestTag + "_Definition") && !pSnippet.Tags.Contains(m_sInterestTag + "_Theory"))
				{
					PageSnippet pPageSnippet = new PageSnippet(pSnippet, 0, PageSection.Definitions);
					m_lPageSnippets.Add(pPageSnippet);
				}
			}
		}
		private void HighLevelTheories()
		{
			foreach (Snippet pSnippet in m_lOriginalSnippets)
			{
				if (pSnippet.Tags.Contains("Theory") && !pSnippet.Tags.Contains("Depth") && !pSnippet.Tags.Contains(m_sInterestTag + "_Theory") && !pSnippet.Tags.Contains(m_sInterestTag + "_Definition"))
				{
					PageSnippet pPageSnippet = new PageSnippet(pSnippet, 0, PageSection.Theories);
					m_lPageSnippets.Add(pPageSnippet);				
				}
			}
		}
		private void NormalStuff()
		{
			foreach (Snippet pSnippet in m_lOriginalSnippets)
			{
				if (!pSnippet.Tags.Contains("Theory") && !pSnippet.Tags.Contains("Definition") && !pSnippet.Tags.Contains("Notes") && !pSnippet.Tags.Contains("Depth") && !pSnippet.Tags.Contains("Argument"))
				{
					PageSnippet pPageSnippet = new PageSnippet(pSnippet, 0, PageSection.Normal);
					m_lPageSnippets.Add(pPageSnippet);
				}
			}
		}

		private void DetermineInterestTag()
		{
			// go backwards and find first non generic tag
			for (int i = m_lQueryTags.Count - 1; i >= 0; i--)
			{
				string sTag = m_lQueryTags[i];
				if (s_lGenericTags.Contains(sTag)) { continue; }
				m_sInterestTag = sTag;
				return;
			}
		}

		private string TitleHtml(string sOriginalQuery)
		{

			string sTagString = "";
			foreach (string sTag in m_lQueryTags) { sTagString += sTag.Replace('_', ' ') + " "; }
			sTagString = sTagString.Trim();

			string sHTML = "<div class='snippet title'><h1>" + sTagString + "</h1></div>";
			return sHTML;
		}

		private List<PageSnippet> FindSectionSnippets(PageSection eSection)
		{
			List<PageSnippet> lSectionSnippets = new List<PageSnippet>();
			foreach (PageSnippet pSnippet in m_lPageSnippets)
			{
				if (pSnippet.Section == eSection) { lSectionSnippets.Add(pSnippet); }
			}
			return lSectionSnippets;
		}
	}
}
