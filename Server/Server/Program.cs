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


			WebCommunications.AuthKey = "54003c32a190b6063fe06a528bc230ce151b589512db4a39ecf8ac01be393dafa154f39bde1f56e690f4c3c2870323972240d4d02fc4fa2f3349dc7ef4c7dc09";
			//string sResponse = WebCommunications.SendGetRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/AddSnippet?sSnippet=");


			string sBody = @"<params>
					<param name='sTagList'>Definition,AI,Theory,Genetic_Algorithm</param>
					<param name='sSnippet'>
						<Snippet>This is the definition of a genetic algorithm! Yes, genetic algorithm! How exciting! Are you not entertained??</Snippet>
						<Source>http://www.wikipedia.com</Source>
					</param>
				</params>";

			string sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/AddSnippet", sBody, true);
			//string sResponse = WebCommunications.SendGetRequest("http://dwlapi.azurewebsites.net/api/reflection/Priorities/PriorityManager/PriorityManager/ListPriorities", true);

			Console.WriteLine(sResponse);
			Console.Read();
		}
	}
}
