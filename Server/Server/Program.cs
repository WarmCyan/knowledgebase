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
						<snippet>This is the definition of a genetic algorithm! Yes, genetic algorithm! How exciting! Are you not entertained??</snippet>
						<source>http://www.wikipedia.com</source>
					</param>
				</params>";

			string sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/AddSnippet", sBody, true);
			//string sResponse = WebCommunications.SendPostRequest("http://localhost:16651/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/AddSnippet", sBody, true);
			////string sResponse = WebCommunications.SendGetRequest("http://dwlapi.azurewebsites.net/api/reflection/Priorities/PriorityManager/PriorityManager/ListPriorities", true);
			//string sResponse = WebCommunications.SendGetRequest("http://localhost:16651/api/reflection/Priorities/PriorityManager/PriorityManager/ListPriorities", true);

			//string sResponse = WebCommunications.SendGetRequest("http://localhost:16651/api/reflection/TestingAssembly/TestingAssembly/Basics/HelloWorld", true);

			//Console.WriteLine(sResponse);

			//KnowledgeServer ks = new KnowledgeServer();
			//ks.AddSnippet("<snippet>This is the testing definition of a genetic algorithm! Yes, genetic algorithm! This is so exciting!!!</snippet><source>http://wikipedia.com</source>", "Definition,AI,Theory,Genetic_Algorithm");

			Console.WriteLine("Finished");
			Console.WriteLine(sResponse);
			Console.Read();
		}
	}
}
