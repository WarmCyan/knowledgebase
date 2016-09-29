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
