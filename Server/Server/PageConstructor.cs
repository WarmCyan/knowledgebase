using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


// TODO: expression for finding meta data:
// <meta name='([a-zA-Z]*)' content='([a-zA-Z]*)'>
// (capture group 1 is the name, capture group 2 is the content)

namespace KnowledgeBaseServer
{
	public class Page
	{
		// static variables
		private static List<string> s_lGenericTags = new List<string>() { "Definition", "Theory", "Argument", "Wisdom", "Note", "Depth", "Important" };
		
		// member variables
		private List<string> m_lQueryTags = new List<string>();
		private string m_sInterestTag = "";
		private List<Snippet> m_lOriginalSnippets = new List<Snippet>();
		private List<PageSnippet> m_lPageSnippets = new List<PageSnippet>();

		//private List
		private Dictionary<string, List<PageSnippet>> m_dDepthSections = new Dictionary<string, List<PageSnippet>>();

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
			this.Notes();
			this.HandleTheDepths();

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
			if (lNotes.Count > 0) { sHTML += "<div class='templatepart'><h3>Notes</h3></div>"; }
			// TODO: don't forget can make other sections too 
			foreach (PageSnippet pSnippet in lNotes) { sHTML += pSnippet.Build(); }
			
			if (lNormal.Count > 0 || lNotes.Count > 0) { sHTML += "<div class='separator'></div>"; }
			
			// depth
			// TODO: make other sections based on unique (but similar) tags
			//--------------------------------------------------------------------------------	
			/*if (lDepth.Count > 0) { sHTML += "<div class='templatepart'><h2>Depth</h2></div>"; }
			foreach (PageSnippet pSnippet in lDepth) { sHTML += pSnippet.Build(); }*/
			//--------------------------------------------------------------------------------	
			if (m_dDepthSections.Count > 0) { sHTML += "<div class='templatepart'><h2>Depth</h2></div>"; }
			if (m_dDepthSections.ContainsKey("Normal")) // print non-section specific stuff FIRST
			{
				foreach (PageSnippet pSnippet in m_dDepthSections["Normal"]) { sHTML += pSnippet.Build(); }
			}
			foreach (string sSectionKey in m_dDepthSections.Keys) // print all other section stuff
			{
				if (sSectionKey == "Normal") { continue; }
				sHTML += "<div class='templatepart'><h3>" + sSectionKey + "</h3></div>";
				foreach (PageSnippet pSnippet in m_dDepthSections[sSectionKey]) { sHTML += pSnippet.Build(); }
			}
				
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
				if (!pSnippet.Tags.Contains("Theory") && !pSnippet.Tags.Contains("Definition") && !pSnippet.Tags.Contains("Note") && !pSnippet.Tags.Contains("Depth") && !pSnippet.Tags.Contains("Argument"))
				{
					PageSnippet pPageSnippet = new PageSnippet(pSnippet, 0, PageSection.Normal);
					m_lPageSnippets.Add(pPageSnippet);
				}
			}
		}
		private void Notes()
		{
			foreach (Snippet pSnippet in m_lOriginalSnippets)
			{
				if (pSnippet.Tags.Contains("Note") && !pSnippet.Tags.Contains("Depth"))
				{
					PageSnippet pPageSnippet = new PageSnippet(pSnippet, 0, PageSection.Notes);
					m_lPageSnippets.Add(pPageSnippet);
				}
			}
		}

		private void HandleTheDepths()
		{
			// get all the snippets that are in depth
			List<Snippet> lDeepSnippets = new List<Snippet>();
			foreach (Snippet pSnippet in m_lOriginalSnippets)
			{
				if (pSnippet.Tags.Contains("Depth")) { lDeepSnippets.Add(pSnippet); }
			}

			// get tagged snippet lists (IGNORING SOURCE TAGS [use meta])
			Dictionary<string, List<Snippet>> dTagOrganizedSnippets = new Dictionary<string, List<Snippet>>();
			foreach (Snippet pSnippet in lDeepSnippets)
			{
				foreach (string sTag in pSnippet.Tags)
				{
					if (sTag == "Depth" || sTag == "Important" || sTag == pSnippet.SourceTag || sTag == m_sInterestTag) { continue; } 
					if (dTagOrganizedSnippets.ContainsKey(sTag)) { dTagOrganizedSnippets[sTag].Add(pSnippet); }
					else { dTagOrganizedSnippets.Add(sTag, new List<Snippet>() { pSnippet }); }
				}
			}

			// TODO: SCALE this, like with importance level
			foreach (string sKey in dTagOrganizedSnippets.Keys)
			{
				if (dTagOrganizedSnippets[sKey].Count > 1)
				{
					m_dDepthSections.Add(sKey, new List<PageSnippet>());
					foreach (Snippet pSnippet in dTagOrganizedSnippets[sKey])
					{
						PageSnippet pPageSnippet = new PageSnippet(pSnippet, 0, PageSection.Depth);
						m_dDepthSections[sKey].Add(pPageSnippet);
					}
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
