using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

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
		private CloudBlobClient m_pBlobClient;

		private CloudTable m_pTable = null;
		private CloudBlobContainer m_pContainer = null;
		
		// properties
		public CloudTable Table 
		{ 
			get
			{
				if (m_pTable == null) { m_pTable = this.GetTagsTable(); }
				return m_pTable;
			} 
		}
		public CloudBlobContainer Container
		{
			get
			{
				if (m_pContainer == null) { m_pContainer = this.GetSnippetsContainer(); }
				return m_pContainer;
			}
		}

		// construction
		public KnowledgeServer()
		{
			this.Initialize();
		}

		// methods

		// PUBLIC FACING ADD SNIPPET METHOD
		// NOTE: adding a new source should be done as a SEPARATE API CALL (as well as separate client action if you think about it)
		//public void AddSnippet(string sSnippet, List<string> lTags)
		public void AddSnippet(string sSnippet, string sTagList)
		{
			List<string> lTags = sTagList.Split(',').ToList();
			Snippet pSnippet = new Snippet(sSnippet, lTags);

			// create the blob
			string sFileName = "s" + DateTime.Now.Ticks.ToString();
			CloudBlockBlob pBlob = this.Container.GetBlockBlobReference(sFileName);
			pBlob.UploadText(sSnippet);

			//TableBatchOperation pBatchOperation = new TableBatchOperation();
			List<TableOperation> lOperations = new List<TableOperation>();
			
			// add the snippet name to each of the tag sets
			/*string sTagList = "";
			foreach (string sTag in lTags) { sTagList += sTag + ","; }
			sTagList = sTagList.Trim(',');*/
			foreach (string sTag in lTags)
			{
				TagSnippetTableEntity pTagSnippetEntity = new TagSnippetTableEntity(sTag, sFileName);
				pTagSnippetEntity.TagList = sTagList;
				lOperations.Add(TableOperation.Insert(pTagSnippetEntity));
				//pBatchOperation.Insert(pTagSnippetEntity);
			}

			// add the raw snippet
			SnippetTableEntity pSnippetEntity = new SnippetTableEntity(sFileName);
			pSnippetEntity.TagList = sTagList;
			lOperations.Add(TableOperation.Insert(pSnippetEntity));
			//pBatchOperation.Insert(pSnippetEntity);

			// try to add the tags (NOTE: This does NOT add source tags)
			foreach (string sTag in lTags)
			{
				TagTableEntity pTagEntity = new TagTableEntity(sTag);
				pTagEntity.IsSource = false;
				if (sTag.StartsWith("source:")) { pTagEntity.IsSource = true; }
				lOperations.Add(TableOperation.InsertOrReplace(pTagEntity));
				//pBatchOperation.InsertOrReplace(pTagEntity);
			}

			// run the batch operation
			//m_pTable.ExecuteBatch(pBatchOperation);
			foreach (TableOperation pOperation in lOperations) { this.Table.Execute(pOperation); }
		}

		// PUBIC FACING QUERY METHOD (should return html)
		public string ConstructPage(string sQuery)
		{
			// split up the query into the list of tags
			List<Snippet> lSnippets = this.QuerySnippetList(sQuery.Split(',').ToList());
			FillSnippetsContent(lSnippets);
			Page pPage = new Page();
			return pPage.Construct(sQuery, lSnippets);
		}

		public string ListTags()
		{
			List<TagTableEntity> lTagList = this.QueryTagList();
			XElement pRoot = new XElement("Tags");

			// construct a tag xml element foreach tag in the queried list
			foreach (TagTableEntity pEntity in lTagList)
			{
				XElement pTagXml = new XElement("Tag");
				pTagXml.SetAttributeValue("Source", pEntity.IsSource);
				pTagXml.Value = pEntity.RowKey;

				pRoot.Add(pTagXml);
			}
			return pRoot.ToString();
		}

		// non public methods

		private List<TagTableEntity> QueryTagList()
		{
			// construct query
			TableQuery<TagTableEntity> pQuery = new TableQuery<TagTableEntity>().Where("PartitionKey eq 'TAG'");
			List<TagTableEntity> pQueryResult = this.Table.ExecuteQuery(pQuery).ToList();
			return pQueryResult;
		} 

		// gets a list of the snippet "file" names for the given list of query tags
		private List<Snippet> QuerySnippetList(List<string> lQueryTags)
		{
			List<TagSnippetTableEntity> lEntityIntersectSet = new List<TagSnippetTableEntity>();

			// get a list of snippets for each query tag
			bool bFirst = true;
			foreach (string sTag in lQueryTags)
			{
				// construct the query
				TableQuery<TagSnippetTableEntity> pQuery = new TableQuery<TagSnippetTableEntity>().Where("PartitionKey eq '" + sTag + "'");

				List<TagSnippetTableEntity> pQueryResultSet = this.Table.ExecuteQuery(pQuery).ToList();

				// union each set with preexisting, (union of all of them)
				if (bFirst) { lEntityIntersectSet = pQueryResultSet; bFirst = false; }
				else { lEntityIntersectSet = this.FindIntersection(lEntityIntersectSet, pQueryResultSet.ToList()); }
			}

			// create the list of snippets
			List<Snippet> lSnippets = new List<Snippet>();
			foreach (TagSnippetTableEntity pEntity in lEntityIntersectSet) { lSnippets.Add(new Snippet(pEntity.RowKey, pEntity.TagList.Split(',').ToList())); }

			return lSnippets;
		}

		private List<TagSnippetTableEntity> FindIntersection(List<TagSnippetTableEntity> l1, List<TagSnippetTableEntity> l2)
		{
			List<TagSnippetTableEntity> lIntersection = new List<TagSnippetTableEntity>();
			
			foreach (TagSnippetTableEntity pEntity in l1)
			{
				string sName = pEntity.RowKey;
				foreach (TagSnippetTableEntity pEntity2 in l2)
				{
					if (pEntity2.RowKey == sName) { lIntersection.Add(pEntity); break; }
				}
			}

			return lIntersection;
		}

		// reads content of snippet blob into the snippet instances
		private void FillSnippetsContent(List<Snippet> lSnippets)
		{
			foreach (Snippet pSnippet in lSnippets)
			{
				CloudBlockBlob pBlob = this.Container.GetBlockBlobReference(pSnippet.FileName);
				string sContent = pBlob.DownloadText();
				pSnippet.ParseContent(sContent);
			}
		}

		private void Initialize()
		{
			m_pStorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
			m_pTableClient = m_pStorageAccount.CreateCloudTableClient();
			m_pBlobClient = m_pStorageAccount.CreateCloudBlobClient();
		}
		
		private CloudTable GetTagsTable()
		{
			CloudTable pTable = m_pTableClient.GetTableReference("knowledgebasedb");

			// make sure it exists
			pTable.CreateIfNotExists();
			return pTable;
		}
		
		private CloudBlobContainer GetSnippetsContainer()
		{
			CloudBlobContainer pContainer = m_pBlobClient.GetContainerReference("knowledgebasesnippets");

			// make sure it exists
			pContainer.CreateIfNotExists();
			return pContainer;
		}
	}
}