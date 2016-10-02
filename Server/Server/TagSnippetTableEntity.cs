using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace KnowledgeBaseServer
{
	class TagSnippetTableEntity : TableEntity
	{
		// construction
		public TagSnippetTableEntity(string sTagName)
		{
			this.PartitionKey = sTagName;
		}
		public TagSnippetTableEntity() { }

		public string TagList { get; set; }
	}
}
