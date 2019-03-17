using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace GUI_RDF
{
    public class Program_RDF
    {
        public List<string> Abstract_result = new List<string>();

        public Program_RDF() { }

        public Program_RDF(List<string> Abstract_result)
        {

        }

        public List<string> Abstract_results { get { return Abstract_result; } set { Abstract_result = value; } } 
        /*To permit to use what the user writes on the console and then on a sparql query, we have to up the first letter and skip the space
      */
        public static string GoodFormat(string mot)
        {
            char[] word = mot.ToCharArray();
            word[0] = char.ToUpper(word[0]);

            var i = 1;
            for (i = 1; i <= word.Length - 1; i++)
            {
                if (word[i] == ' ' && i != word.Length - 1)
                {
                    word[i] = '_';
                    word[i + 1] = char.ToUpper(word[i + 1]);
                }
            }

            string m = new string(word);
            Console.WriteLine("New Format : " + m);
            return m;
        }
        static void LoadData()
        { //Define a remote endpoint
            //Use the DBPedia SPARQL endpoint with the default Graph set to DBPedia
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org")
            {
                RdfAcceptHeader = "application/turtle"
            };
            //Make a SELECT query against the Endpoint
            string query = "SELECT ?titre WHERE { ?film rdf:type <http://dbpedia.org/ontology/Film> . ?film rdfs:label ?titre FILTER langMatches(lang(?titre), 'en') }LIMIT 100";
            SparqlResultSet results = endpoint.QueryWithResultSet(query);
            foreach (SparqlResult result in results)
            {
                Console.WriteLine(result.ToString());
            }

            //Make a DESCRIBE query against the Enpoint
            IGraph g = endpoint.QueryWithResultGraph(query);
            foreach (Triple t in g.Triples)
            {
                Console.WriteLine(t.ToString());
            }

        }
        /* COMPLETE :Function 1 : I want to navigate into a graph
         Currently : Done. we output the node in and out 
        */
        public static void Item1()
        {
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org");
            //endpoint.RdfAcceptHeader = "application/turtle";
            //Make a SELECT query against the Endpoint
            string word = "";

            Uri uri = new Uri("http://dbpedia.org/resource/");
            Console.WriteLine("From which node do you want to start?");
            //We put the history of node selected in a List
            List<string> history = new List<string>();
            word = Console.ReadLine();
            while (word != "quit")
            {
                history.Add(word);
                word = GoodFormat(word);

                string query = "select \"" + word + "\", ?final_property, ?final_domain WHERE {{ ?domain ?property  <" + uri + word + ">  BIND (strafter(str(?property),'http://dbpedia.org/') as ?prop) BIND (strafter(str(?prop),'/') as ?final_property )  BIND (strafter(str(?domain),'http://dbpedia.org/') as ?dom) BIND (strafter(str(?dom),'/') as ?final_domain )}} LIMIT 100";
                SparqlResultSet results = endpoint.QueryWithResultSet(query);
                if (results.LongCount() == 0)
                { Console.WriteLine("No results"); }
                foreach (SparqlResult result in results)
                {
                    Console.WriteLine(result[2] + " " + result[1] + "--->" + word);
                }
                query = "select ?prop, ?final_domain WHERE { <" + uri + word + "> ?property ?final_domain BIND (strafter(str(?property),'http://dbpedia.org/ontology/') as ?prop) FILTER regex(?property, 'http://dbpedia.org/ontology/', 'i' )}";
                results = endpoint.QueryWithResultSet(query);
                if (results.LongCount() == 0)
                { Console.WriteLine("No results"); }
                foreach (SparqlResult result in results)
                {
                    Console.WriteLine(word + "--->" + result[0] + " " + result[1]);
                }


                Console.WriteLine("Quit to leave. Tap prec to go to the precedent node");
                word = Console.ReadLine();
                if (word == "prec")
                    word = history.Last();
            }
        }
        /*Function 4 : Searching informations : COMPLETE
        */
        /* Function 2 We want to know if two nodes are connected. We suppose 
         */
        /*public static void Item4(string subject)
        { // The user must choose a subject to get the abstract
            //Console.WriteLine("About what subject do you want an abstract ?");
           .// string subject = Console.ReadLine();
            subject = GoodFormat(subject);
            //Console.WriteLine("You chose a abstract about " + subject);

            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org")
            {
                RdfAcceptHeader = "application/turtle"
            };
            //Make a SELECT query against the Endpoint

            Uri uriResource = new Uri("http://dbpedia.org/resource/");
            //Query Sparql
            string query = "select * where { <" + uriResource + subject + "> <http://dbpedia.org/ontology/abstract>  ?value FILTER langMatches(lang(?value), 'en') }  ";
            //Stockage of results
            SparqlResultSet results = endpoint.QueryWithResultSet(query);
            //Output on console
         
            foreach (SparqlResult result in results)
            {
                // Console.WriteLine(result.ToString());
                abstract_result.Add(result.ToString());
            }


        }*/
        /* Function 5 : Know informations about something" : COULD BE BETTER
        Exemple : I want to know where Brad Pitt is involved 
        */
        public static void Item5()
        {
            Console.WriteLine("Item 5");
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org")
            {
                RdfAcceptHeader = "application/turtle"
            };
            //Make a SELECT query against the Endpoint
            string end = "";
            Uri uri = new Uri("http://dbpedia.org/resource/");
            while (end == "")
            {
                Console.WriteLine("What informations are you looking for ?");
                string word = Console.ReadLine();
                word = GoodFormat(word);
                //< "+uri+word+ " > ? property ? value ||
                //To focus on what object the user asks (and focus the query), we have to know the type of the object.
                //GUI : Do buttons to select if the type of the object
                string reponse = "";
                //The goal is to propose a lot of "person"
                //select replace(str(?object),'http://dbpedia.org/resource/',''), ?final_domain where { <http://dbpedia.org/resource/Interstellar(film)> ?domain ?object FILTER regex(?object, 'http://dbpedia.org/') FILTER (regex(?domain, 'http://dbpedia.org/ontology') || regex(?domain, 'http://dbpedia.org/property') ) BIND (strafter(str(?domain),'http://dbpedia.org/') as ?dom) BIND (strafter(str(?dom),'/') as ?final_domain )}
                Console.WriteLine("It is an person ? Tape to yes");
                string query = "";
                reponse = Console.ReadLine();
                if (reponse == "yes" || reponse == "YES")
                { //For a actor
                    query = "select replace(str(?property),'http://dbpedia.org/ontology/',''), replace(str(?domain), 'http://dbpedia.org/resource/', '') where {  ?domain ?property  <" + uri + word + ">  FILTER regex(?property, 'http://dbpedia.org/ontology/') FILTER regex(?property, 'http://dbpedia.org/ontology/')  } ";
                }
                else
                {
                    //For a film 
                    query = "select replace(str(?object), 'http://dbpedia.org/resource/', ''), ?final_domain where { < ' + uri + word + ' > ?domain ?object FILTER regex(?object, 'http://dbpedia.org/') FILTER (regex(?domain, 'http://dbpedia.org/ontology') || regex(?domain, 'http://dbpedia.org/property') ) BIND (strafter(str(?domain),'http://dbpedia.org/') as ?dom) BIND (strafter(str(?dom),'/') as ?final_domain )}";
                }


                //For a city
                // query= select ?final_domain, ?object where { < "+uri+word+ " > ?domain ?object FILTER (regex(?domain, 'http://dbpedia.org/ontology') || regex(?domain, 'http://dbpedia.org/property') ) BIND (strafter(str(?domain),'http://dbpedia.org/') as ?dom) BIND (strafter(str(?dom),'/') as ?final_domain ) } 

                SparqlResultSet results = endpoint.QueryWithResultSet(query);
                foreach (SparqlResult result in results)
                {
                    Console.WriteLine(result[0].ToString() + " " + result[1].ToString());
                }
                //Make a DESCRIBE query against the Endpoint
                IGraph g = endpoint.QueryWithResultGraph(query);
                /*  int i = 0;
                  foreach (Triple t in g.Triples)
                  {
                      Console.WriteLine(i + t.Object.ToString());
                      i++;
                  }
                  */

            }
        }
        static IGraph Create_graph(string namefile)
        {
            IGraph graph = new Graph();
            FileLoader.Load(graph, namefile);
            if (graph != null)
                Console.WriteLine("Graph load");
            else
                Console.WriteLine("ERROR");
            return graph;
        }
        static void Read_graph(IGraph g)
        {
            foreach (Triple t in g.Triples)
            {
                Console.WriteLine(t.ToString());

            }
        }

    }
}
