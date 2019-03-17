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
    /// Logique d'interaction pour Get_Abstract.xaml
    /// </summary>
    public partial class Get_Abstract : Window

    {

        public Get_Abstract()
        {
            InitializeComponent();
            

        }
        private void Item4(string subject)
        {  // The user must choose a subject to get the abstract
            //Console.WriteLine("About what subject do you want an abstract ?");
           // string subject = Console.ReadLine();
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

            List<string> abstract_result = new List<string>();
            string good_format = "";
            foreach (SparqlResult result in results)
            {
                good_format = result.ToString();
                good_format = good_format.Substring(8, good_format.Length - 11);
                abstract_result.Add(good_format);
            }

            Abstract_Result_List.ItemsSource = abstract_result;

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

        private void Get_the_abstract_Click(object sender, RoutedEventArgs e)
        {

            Item4(Abstract_subject.Text);
            Abstract_Result_List.Visibility = Visibility.Visible;

       


            
        }

        private void Abstract_Quit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Abstract_Main_Menu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow menu_window = new MainWindow();
            menu_window.Show();
            this.Close();
        }


    }
}
