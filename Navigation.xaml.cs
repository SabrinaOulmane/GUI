using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace GUI_RDF
{
    /// <summary>
    /// Logique d'interaction pour Navigation.xaml
    /// </summary>
    public partial class Navigation : Window
    {
        public Navigation()
        {
            InitializeComponent();
        }

        private void Navigation_Method_Subject_Node(string word)
        {
            List<string> subject_results = new List<string>();

            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org");
            //endpoint.RdfAcceptHeader = "application/turtle";
            //Make a SELECT query against the Endpoint


            Uri uri = new Uri("http://dbpedia.org/resource/");

            //We put the history of node selected in a List
            List<string> history = new List<string>();

                history.Add(word);
                word = GoodFormat(word);

                string query = "select \"" + word + "\", ?final_property, ?final_domain WHERE {{ ?domain ?property  <" + uri + word + ">  BIND (strafter(str(?property),'http://dbpedia.org/') as ?prop) BIND (strafter(str(?prop),'/') as ?final_property )  BIND (strafter(str(?domain),'http://dbpedia.org/') as ?dom) BIND (strafter(str(?dom),'/') as ?final_domain )}} LIMIT 100";
                SparqlResultSet results = endpoint.QueryWithResultSet(query);


                foreach (SparqlResult result in results)
                {
                    string subject_result_text = result[2] + " " + result[1] + "---> \n";
                    subject_results.Add(subject_result_text);
                }
 
            Navigation_Result_List.ItemsSource = subject_results;

        }

        private void Navigation_Method_Object_Node(string word)
        {
            List<string> object_results = new List<string>();

            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org");
            //endpoint.RdfAcceptHeader = "application/turtle";
            //Make a SELECT query against the Endpoint


            Uri uri = new Uri("http://dbpedia.org/resource/");

            //We put the history of node selected in a List
            List<string> history = new List<string>();

                history.Add(word);
                word = GoodFormat(word);

                string query = "select ?prop, ?final_domain WHERE { <" + uri + word + "> ?property ?final_domain BIND (strafter(str(?property),'http://dbpedia.org/ontology/') as ?prop) FILTER regex(?property, 'http://dbpedia.org/ontology/', 'i' )}";
                SparqlResultSet results = endpoint.QueryWithResultSet(query);

                foreach (SparqlResult result in results)
                {
                    string object_result_text = "--->" + result[0] + " " + result[1]+"\n";
                    object_results.Add(object_result_text);
                }

            Navigation_Result_List.ItemsSource = object_results;




        }

        private string GoodFormat(string mot)
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

        private void Navigation_Subject_Node_Click(object sender, RoutedEventArgs e)
        {
            Navigation_Method_Subject_Node(Navigation_node_text.Text);
            Navigation_Result_List.Visibility = Visibility.Visible;
        }

        private void Navigation_Object_Node_Click(object sender, RoutedEventArgs e)
        {
            Navigation_Method_Object_Node(Navigation_node_text.Text);
            Navigation_Result_List.Visibility = Visibility.Visible;
        }

        private void Navigation_Quit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Navigation_Main_Menu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow new_menu = new MainWindow();
            new_menu.Show();
            this.Close();
        }

    }
}