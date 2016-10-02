using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeBaseServer
{
	public class Snippet
	{
		// member variables
		private string m_sFileName;
		private List<string> m_lTags;
		private string m_sContent;

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
	}
}
