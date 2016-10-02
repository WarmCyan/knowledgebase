using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace KnowledgeBaseServer
{
	class TagTableEntity : TableEntity
	{
		public TagTableEntity(string sRowKey)
		{
			this.PartitionKey = "TAG";
			this.RowKey = sRowKey;
		}
	
		public TagTableEntity() 
		{
			//this.PartitionKey = "TAG";
			//this.RowKey = DateTime.UtcNow.Ticks.ToString();
		}
		
		//public string TagRowID { get; set; }
		public string Name { get; set; }
		public bool IsSource { get; set; }
		public int Count { get; set; }
		public string SourceFileName { get; set; }
	}
}
