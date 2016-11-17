using System;
using System.IO;
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
			Console.WriteLine("Hello world!");

			WebCommunications.AuthKey = "54003c32a190b6063fe06a528bc230ce151b589512db4a39ecf8ac01be393dafa154f39bde1f56e690f4c3c2870323972240d4d02fc4fa2f3349dc7ef4c7dc09";

			// GENERIC TAGS:
			// - Definition
			// - Theory
			// - Argument
			// - Wisdom
			// - Note
			// - Depth 
			// - Primary? (means yes, this is very important to one of the tags)

			/*string sBody = @"<params>
					<param name='sTagList'>Definition,AI,Theory,Genetic_Algorithm</param>
					<param name='sSnippet'>
						<snippet>This is the definition of a genetic algorithm! Yes, genetic algorithm! How exciting! Are you not entertained??</snippet>
						<source>http://www.wikipedia.com</source>
					</param>
				</params>";*/

			//string sResponse = WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/AddSnippet", sBody, true);

			//AddTestingSnippets();

			//string sPage = Query("Genetic_Algorithm");

			//string sPage = GetTags();

			//EditSnippet("s636125978981349050", "Test,source:NewSource,Depth,Important,OtherThing", "<meta name='sourceTag' content='NewSource'><meta name='source' content='No source for you'><p>Ha! All your evil plans are foiled once more, for I have successfully edited your content!</p>");

			//string sPage = GetSnippet("s636125978981349050");

			//DeleteSnippet("s636146206727119253");

			string sPage = GetRandom();

			Console.WriteLine("Finished");
			Console.WriteLine("\n" + sPage);

			//File.WriteAllText("./output.html", sPage);
			
			Console.Read();
		}

		// TODO: have to make snippet format as actual html, (meta tag inside for source) so don't parse as xml later.

		public static void AddSnippet(string sTagList, string sSnippet)
		{
			string sBody = "<params><param name='sTagList'>" + sTagList + "</param><param name='sSnippet'>" + EncodeXML(sSnippet) + "</param></params>";
			WebCommunications.SendPostRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/AddSnippet", sBody, true);
		}

		public static string Query(string sTagList)
		{
			//string sResponse = WebCommunications.SendGetRequest("http://dwlapi.azurewebsites.net/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/ConstructPage?squery=" + sTagList, true);
			//string sResponse = WebCommunications.SendGetRequest("http://localhost:16651/api/reflection/KnowledgeBaseServer/KnowledgeBaseServer/KnowledgeServer/ConstructPage?squery=" + sTagList, true);
			KnowledgeServer ks = new KnowledgeServer();
			string sResponse = ks.ConstructPage(sTagList);

			// sanitize, because my webservice is still doing weird things
			sResponse = sResponse.Trim('\"');
			sResponse = sResponse.Replace("\\\"", "\"");
			 
			return sResponse;
		}

		public static string GetTags()
		{
			KnowledgeServer ks = new KnowledgeServer();
			string sResponse = ks.ListTags();
			return sResponse;
		}

		public static string EncodeXML(string sXML) { return sXML.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;"); }

		public static void AddTestingSnippets()
		{
			AddSnippet("Definition,AI,Theory,Genetic_Algorithm,source:Wikipedia,Machine_Learning,Genetic_Algorithm_Definition", "<meta name='sourceTag' content='Wikipedia'><meta name='source' content='https://en.wikipedia.org/wiki/Genetic_algorithm'><p><b>Genetic Algorithm</b> - A genetic algorithm is a metaheuristic inspired by the process of natural selection, commonly used to find solutions to optimization and search problems using mutation, crossover, and selection.</p>");
			AddSnippet("Definition,Genetic_Algorithm,source:Wikipedia,Chromosome,Genotype,Chromosome_Definition", "<meta name='sourceTag' content='Wikipedia'><meta name='source' content='https://en.wikipedia.org/wiki/Chromosome_(genetic_algorithm)'><p>Chromosome/Genotype - Set of parameters which define a proposed solution to the problem that the genetic algorithm is trying to solve. Often a binary representation (binary string) or a string representation.</p>");
			AddSnippet("Note,Genetic_Algorithm,source:Wikipedia", "<meta name='sourceTag' content='Wikipedia'><meta name='source' content='https://en.wikipedia.org/wiki/Genetic_algorithm'><p>\"GA's cannot effectively solve problems in which the only fitness measure is a single right/wrong measure (like decision problems), as there is no way to converge on the solution. However, if the situation allows the success/failure trial to be repeated giving different results, then the ratio of successes to failures provides a suitable fitness measure.\"</p>");
			AddSnippet("Depth,Implementation,Genetic_Algorithm,source:Obitko", "<meta name='sourceTag' content='Obitko'><meta name='source' content='http://www.obitko.com/tutorials/genetic-algorithms/ga-basic-description.php'><ol><li>Start - Generate random population of n chromosomes (suitable solutions to problem)</li><li>Fitness - Evaluate the fitness f(x) of each chromosome x in the population</li><li>New population - Create a new population by repeating the following steps until the new population is complete<ol><li>Selection - Select two parent chromosomes from a population according to their fitness (the better fitness, the bigger chance to be selected)</li><li>Crossover - With a crossover probability cross over the parents to form a new offspring (children). If no crossover was performed, offspring is an exact copy of parents.</li><li>Mutation - With a mutation probability mutate new offspring at each locus (position in chromosome)</li><li>Accepting - Place new offspring in a population</li></ol></li><li>Replace - Use new generated population for a further run of algorithm</li><li>Test - If the end condition is satisfied, stop, and return the best soultion in current population.</li><li>Loop - Go to step 2</li></ol>");
			AddSnippet("Depth,Implementation,Note,Genetic_Algorithm,Initial,source:Franklin", "<meta name='sourceTag' content='Franklin'><meta name='source' content='Franklin 177'><p>Generating initial population: \"First forms an initial population by randomly choosing values (alleles) for each of the features (genes)\"</p>");
			AddSnippet("Theory,AI,Church-Turing_Thesis,source:Franklin,Church-Turing_Thesis_Theory", "<meta name='sourceTag' content='Franklin'><meta name='source' content='Franklin 79'><p>Church-Turing Thesis - \"Any effective procedure (algorithm) can be implemented via a Turing machine.\"</p>");
		}

		public static void EditSnippet(string sSnippetName, string sTagList, string sSnippet)
		{
			KnowledgeServer ks = new KnowledgeServer();
			ks.EditSnippet(sSnippetName, sSnippet, sTagList);
		}

		public static void DeleteSnippet(string sSnippetName)
		{
			KnowledgeServer ks = new KnowledgeServer();
			ks.DeleteSnippet(sSnippetName);
		}

		public static string GetSnippet(string sSnippetName)
		{
			KnowledgeServer ks = new KnowledgeServer();
			return ks.GetSnippet(sSnippetName);
		}

		public static string GetRandom()
		{
			KnowledgeServer ks = new KnowledgeServer();
			return ks.RandomTag();
		}
	}
}
