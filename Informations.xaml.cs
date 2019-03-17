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
    /// Logique d'interaction pour Informations.xaml
    /// </summary>
    public partial class Informations : Window
    {
        public Informations()
        {
            InitializeComponent();
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
        private void Information_Method(string word)
        {
            List<string> Informations_result = new List<string>();

            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org")
            {
                RdfAcceptHeader = "application/turtle"
            };
            //Make a SELECT query against the Endpoint
           
            Uri uri = new Uri("http://dbpedia.org/resource/");
             word = GoodFormat(word);
           
                string query = "";
                // reponse = Console.ReadLine();

                query = "select replace(str(?property),'http://dbpedia.org/ontology/',''), replace(str(?domain), 'http://dbpedia.org/resource/', '') where {  ?domain ?property  <" + uri + word + ">  FILTER regex(?property, 'http://dbpedia.org/ontology/') FILTER regex(?property, 'http://dbpedia.org/ontology/')  } ";


                SparqlResultSet results = endpoint.QueryWithResultSet(query);
                foreach (SparqlResult result in results)
                {
                    string final_result = result[0].ToString() + " " + result[1].ToString();
                    Informations_result.Add(final_result);

                }
                //Make a DESCRIBE query against the Endpoint
                IGraph g = endpoint.QueryWithResultGraph(query);

            

            Informations_Result_List.ItemsSource = Informations_result;
        }
        private void Informations_Quit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Informations_Main_Menu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow new_window = new MainWindow();
            this.Close();
            new_window.Show();
        }

        private void Get_informations_Click(object sender, RoutedEventArgs e)
        {
            Information_Method(Informations_text.Text);

            Informations_Result_List.Visibility = Visibility.Visible;

        }
    }
}
