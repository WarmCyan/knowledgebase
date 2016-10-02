using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;

namespace KnowledgeBaseServer
{
	public class KnowledgeServer
	{
		// member variables
		private CloudStorageAccount m_pStorageAccount;
		private CloudTableClient m_pTableClient;

		private CloudTable m_pTable = null;
		
		// properties
		public CloudTable Table { 
			get
			{
				if (m_pTable == null) { m_pTable = this.GetTagsTable(); }
				return m_pTable;
			} 
		}

		// methods

		// gets a list of the snippet "file" names for the given list of query tags
		public List<Snippet> QuerySnippetList(List<string> lQueryTags)
		{
			List<TagSnippetTableEntity> lEntityUnionSet = new List<TagSnippetTableEntity>();

			// get a list of snippets for each query tag
			bool bFirst = true;
			foreach (string sTag in lQueryTags)
			{
				// construct the query
				TableQuery<TagSnippetTableEntity> pQuery = new TableQuery<TagSnippetTableEntity>().Where("PartitionKey eq '" + sTag + "'");

				List<TagSnippetTableEntity> pQueryResultSet = this.Table.ExecuteQuery(pQuery).ToList();

				// union each set with preexisting, (union of all of them)
				if (bFirst) { lEntityUnionSet = pQueryResultSet; bFirst = false; }
				else { lEntityUnionSet = lEntityUnionSet.Union(pQueryResultSet).ToList(); }
			}

			// create the list of snippets
			List<Snippet> lSnippets = new List<Snippet>();
			foreach (TagSnippetTableEntity pEntity in lEntityUnionSet) { lSnippets.Add(new Snippet(pEntity.RowKey, pEntity.TagList.Split(',').ToList())); }

			return lSnippets;
		}


		private void Initialize()
		{
			m_pStorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
			m_pTableClient = m_pStorageAccount.CreateCloudTableClient();
		}
		
		private CloudTable GetTagsTable()
		{
			CloudTable pTable = m_pTableClient.GetTableReference("KnowledgeBaseTags");

			// make sure it exists
			pTable.CreateIfNotExists();
			return pTable;
		}
	}
}
