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
		public SnippetTableEntity() 
		{
			this.PartitionKey = "SNIPPET";
			this.RowKey = DateTime.UtcNow.Ticks.ToString();
		}

		public string TagRowID { get; set; }
		public string Name { get; set; }
	}
}
