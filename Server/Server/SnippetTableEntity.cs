using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace KnowledgeBaseServer
{
	class SnippetTableEntity : TableEntity 
	{
		public SnippetTableEntity(string sSnippetName) 
		{
			this.PartitionKey = "SNIPPET";
			this.RowKey = sSnippetName; 
		}
		public SnippetTableEntity() { }

		public string TagList { get; set; }
	}
}
