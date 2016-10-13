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
		private int m_iMaxTagCount = 0;

		//private List
		private Dictionary<string, List<PageSnippet>> m_dDepthSections = new Dictionary<string, List<PageSnippet>>();

		// construction
		public Page() 
		{
			PageSnippet.s_sSnippetID = 0;
		}

		// return page HTML
		public string Construct(string sOriginalQuery, List<Snippet> lSnippets)
		{
			m_lOriginalSnippets = lSnippets;

			// get query tags
			m_lQueryTags = sOriginalQuery.Split(',').ToList();
		
			// tag of interest
			this.DetermineInterestTag();
			this.CreateInterestTagSnippet();

			// find section snippets
			this.HighLevelDefinitions();
			this.HighLevelTheories();
			this.NormalStuff();
			this.Notes();
			this.HandleTheDepths();
			this.Arguments();

			// "post-processing"
			this.DetermineMaxTagCount();

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
			foreach (PageSnippet pSnippet in lInterests) { sHTML += pSnippet.Build(m_iMaxTagCount); }

			// definitions
			if (lDefinitions.Count > 0) { sHTML += "<div class='templatepart'><h2>Definitions</h2></div>"; }
			foreach (PageSnippet pSnippet in lDefinitions) { sHTML += pSnippet.Build(m_iMaxTagCount); }

			// theories
			if (lTheories.Count > 0) { sHTML += "<div class='templatepart'><h2>Theories</h2></div>"; }
			foreach (PageSnippet pSnippet in lTheories) { sHTML += pSnippet.Build(m_iMaxTagCount); }

			if (lTheories.Count > 0 || lDefinitions.Count > 0) { sHTML += "<div class='separator'></div>"; }

			// other stuff
			foreach (PageSnippet pSnippet in lNormal) { sHTML += pSnippet.Build(m_iMaxTagCount); }
			
			// notes
			if (lNotes.Count > 0) { sHTML += "<div class='templatepart'><h3>Notes</h3></div>"; }
			foreach (PageSnippet pSnippet in lNotes) { sHTML += pSnippet.Build(m_iMaxTagCount); }
			
			if (lNormal.Count > 0 || lNotes.Count > 0) { sHTML += "<div class='separator'></div>"; }
			
			// depth
			// TODO: make other sections based on unique (but similar) tags
			if (lDepth.Count > 0) { sHTML += "<div class='templatepart'><h2>Depth</h2></div>"; }
			if (m_dDepthSections.ContainsKey("Normal")) // print non-section specific stuff FIRST
			{
				foreach (PageSnippet pSnippet in m_dDepthSections["Normal"]) { sHTML += pSnippet.Build(m_iMaxTagCount); }
			}
			foreach (string sSectionKey in m_dDepthSections.Keys) // print all other section stuff
			{
				if (sSectionKey == "Normal") { continue; }
				sHTML += "<div class='templatepart'><h3>" + sSectionKey + "</h3></div>";
				foreach (PageSnippet pSnippet in m_dDepthSections[sSectionKey]) { sHTML += pSnippet.Build(m_iMaxTagCount); }
			}
				
			// arguments
			// TODO: sections for different sides of argument?
			if (lArguments.Count > 0) { sHTML += "<div class='templatepart'><h2>Arguments</h2></div>"; }
			foreach (PageSnippet pSnippet in lArguments) { sHTML += pSnippet.Build(m_iMaxTagCount); }

			// things?
			
			sHTML += "</body>";
			return sHTML;
		}

		private void DetermineMaxTagCount()
		{
			foreach (Snippet pSnippet in m_lOriginalSnippets) { if (pSnippet.Tags.Count > m_iMaxTagCount) { m_iMaxTagCount = pSnippet.Tags.Count; } }
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
		private void Arguments()
		{
			foreach (Snippet pSnippet in m_lOriginalSnippets)
			{
				if (pSnippet.Tags.Contains("Argument"))
				{
					PageSnippet pPageSnippet = new PageSnippet(pSnippet, 0, PageSection.Arguments);
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
				if (pSnippet.Tags.Contains("Depth")) 
				{ 
					lDeepSnippets.Add(pSnippet);

					// adding page snippet to list just so we have a global list of all depth snippets (these should NOT be used for building html of depth snippets)
					PageSnippet pPageSnippet = new PageSnippet(pSnippet, 0, PageSection.Depth);
					m_lPageSnippets.Add(pPageSnippet); 
				}
			}

			// get tag snippet collections (IGNORING SOURCE TAGS [use meta])
			Dictionary<string, List<Snippet>> dTagOrganizedSnippets = new Dictionary<string, List<Snippet>>();
			dTagOrganizedSnippets.Add("Normal", new List<Snippet>());
			foreach (Snippet pSnippet in lDeepSnippets)
			{
				bool bSnippetAdded = false; // if snippet is never added in the conditions, then add it after the loop into the "normal" section
				foreach (string sTag in pSnippet.Tags)
				{
					if (sTag == "Depth" || sTag == "Important" || sTag == pSnippet.MetaData["sourceTag"] || m_lQueryTags.Contains(sTag)) { continue; } 
					if (dTagOrganizedSnippets.ContainsKey(sTag)) { bSnippetAdded = true; dTagOrganizedSnippets[sTag].Add(pSnippet); } // if collection for this tag exists, add snippet to it
					else { bSnippetAdded = true; dTagOrganizedSnippets.Add(sTag, new List<Snippet>() { pSnippet }); } // if collection for tag doesn't exist, make it
				}
				if (!bSnippetAdded) { dTagOrganizedSnippets["Normal"].Add(pSnippet); } // if wasn't added to any other tag collections, add it to normal
			}

			List<PageSnippet> pUnusedSnippets = new List<PageSnippet>();
			
			// TODO: SCALE this, like with importance level NOTE: If certain fraction of number of depth snippets have same tag, then include that tag (but only if above a certain limit, cause this doesn't work for small numbers)
			// for every tag collection of snippets, if meets criteria create specific depth section for them
			m_dDepthSections.Add("Normal", new List<PageSnippet>());
			foreach (string sKey in dTagOrganizedSnippets.Keys)
			{
				if (dTagOrganizedSnippets[sKey].Count > 1 || sKey == "Normal") // if this collection has more than one snippet, give it its own section (change)
				{
					if (sKey != "Normal") { m_dDepthSections.Add(sKey, new List<PageSnippet>()); }
					// make a page snippet for each snippet and add it to the depth section collection
					foreach (Snippet pSnippet in dTagOrganizedSnippets[sKey])
					{
						PageSnippet pPageSnippet = new PageSnippet(pSnippet, 0, PageSection.Depth);
						m_dDepthSections[sKey].Add(pPageSnippet);
					}
				}
				else // otherwise dump in the unusued snippets
				{
					foreach (Snippet pSnippet in dTagOrganizedSnippets[sKey]) 
					{
						PageSnippet pPageSnippet = new PageSnippet(pSnippet, 0, PageSection.Depth);
						pUnusedSnippets.Add(pPageSnippet);
					}
				}
			}

			// anything not yet used (in the unusued snippets list) just throw in "depth normal"
			foreach (PageSnippet pSnippet in pUnusedSnippets)
			{
				if (!CheckIfSnippetUsedInDepth(pSnippet)) { m_dDepthSections["Normal"].Add(pSnippet); }
			}
		}

		// checks if this snippet exists in any of the depth sections (returns true if used, false if not)
		private bool CheckIfSnippetUsedInDepth(PageSnippet pSnippet)
		{
			foreach (string sSectionKey in m_dDepthSections.Keys)
			{
				foreach (PageSnippet pPageSnippet in m_dDepthSections[sSectionKey])
				{
					if (pPageSnippet.Snippet.FileName == pSnippet.Snippet.FileName) { return true; }
				}
			}
		
			return false;
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
