using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeBaseServer
{
	public class PageSnippet
	{
		// static variables
		public static int s_sSnippetID = 0;
	
		// member variables
		private int m_iLevel = 0; //0 = normal, 1 = quaternary, 2 = tertiary, 3 = secondary, 4 = primary
		private PageSection m_eSection = PageSection.Normal;
		private Snippet m_pSnippet = null;
		private int m_iID = 0;
		//private string m_sContent = "";

		// construction
		public PageSnippet() { }
		public PageSnippet(Snippet pSnippet, int iLevel, PageSection eSection)
		{
			this.Snippet = pSnippet;
			this.Level = iLevel;
			this.Section = eSection;
			this.m_iID = s_sSnippetID;
			s_sSnippetID++;
		}
		
		// properties
		public int Level { get { return m_iLevel; } set { m_iLevel = value; } }
		public PageSection Section { get { return m_eSection; } set { m_eSection = value; } }
		public Snippet Snippet { get { return m_pSnippet; } set { m_pSnippet = value; } }
		public int ID { get { return m_iID; } set { m_iID = value; } }

		// methods
		public string Build(int iMaxTagCount)
		{
			// adjust level based on relative tag count
			if (this.Level == 0)
			{
				float pPercentage = (float)(this.Snippet.Tags.Count) / (float)(iMaxTagCount);
				if (pPercentage <= .5) { this.Level = 0; }
				else if (pPercentage <= .625) { this.Level = 1; }
				else if (pPercentage <= .75) { this.Level = 2; }
				else if (pPercentage <= .875) { this.Level = 3; }
				else { this.Level = 4; }
			}
			
			// adjust level if has an important tag
			if (this.Snippet.Tags.Contains("Important") && this.Level < 4) { this.Level++; }
		
			string sHTML = "<div id='snippet" + this.ID.ToString() + "' class='snippet";
			// determine level of importance
			switch (this.Level)
			{
				case 1:
					sHTML += " quaternary";
					break;
				case 2:
					sHTML += " tertiary";
					break;
				case 3:
					sHTML += " secondary";
					break;
				case 4:
					sHTML += " primary";
					break;
			}
			//sHTML += "' onmouseover='SnippetOnMouseOver(" + this.ID.ToString() + ")' onclick='SnippetOnClick(" + this.ID.ToString() + ")'>";
			sHTML += "'>";
			sHTML += "<meta name='snippetFileName' content='" + this.Snippet.FileName + "'>";
			sHTML += this.Snippet.Content;
			sHTML += "</div>";
			//sHTML += "<p class='sourcetext' id='snippetsource" + this.ID.ToString() + "' onclick='SnippetSourceOnClick(" + this.ID.ToString() + ")' onmouseover='SnippetSourceOnMouseOver(" + this.ID.ToString() + ")'>source</p></div>";

			return sHTML;
		}
	}

	public enum PageSection
	{
		Interest = 0,
		Definitions = 1,
		Theories = 2,
		Normal = 3,
		Notes = 4,
		Depth = 5,
		Arguments = 6,
		Related = 7
	}
}
