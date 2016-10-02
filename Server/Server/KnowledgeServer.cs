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
		public List<string> QuerySnippetList(List<string> lQueryTags)
		{
			List<string> lTagIDs = this.GetTagIDs(lQueryTags);
		}







		// turn the list of tag NAMES into a list of tag IDS
		public List<string> GetTagIDs(List<string> lQueryTags)
		{
			// construct the query
			// TODO: dynamically adding to query filters (just loop through?)
			//TableQuery<TagTableEntity> pQuery = new TableQuery<TagTableEntity>().Where(
				//TableQuery.CombineFilters(
					//TableQuery));

		
			return null;
		}

		

		private void Initialize()
		{
			m_pStorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
			m_pTableClient = m_pStorageAccount.CreateCloudTableClient();
		}

		
		/*private Dictionary<string, List<string>> m_dTagLists;

		private void FillTagLists()
		{
			// read in file		
			string[] aLines = File.ReadAllLines("_tags.txt");
			foreach (string sLine in aLines)
			{
				int iEqualsIndex = sLine.IndexOf("=");
				string sKey = sLine.Substring(0, iEqualsIndex);
				string sList = sLine.Substring(iEqualsIndex + 1);
				//List<string> lSnippets = sList.Split(",");
			}
		}*/

		private CloudTable GetTagsTable()
		{
			CloudTable pTable = m_pTableClient.GetTableReference("KnowledgeBaseTags");

			// make sure it exists
			pTable.CreateIfNotExists();
			return pTable;
		}
	}
}
