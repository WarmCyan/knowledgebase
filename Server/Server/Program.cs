using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DWL.Utility;

namespace KnowledgeBaseServer
{
	class Program
	{
		public static void Main(string[] args)
		{
			Console.Write("Hello world!");


			WebCommunications.AuthKey = "54003c32a";
			//string sResponse = WebCommunications.SendGetRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/AddSnippet?sSnippet=");
			
			Console.Read();
		}
	}
}
