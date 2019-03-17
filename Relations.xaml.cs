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
    /// Logique d'interaction pour Relations.xaml
    /// </summary>
    public partial class Relations : Window
    {
        public Relations()
        {
            InitializeComponent();
        }
        public string GoodFormat(string mot)
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
        public string completeQuery(string corequery)
        {
            string completeQuery = "";
            completeQuery += "SELECT * WHERE {" + "\n";
            completeQuery += corequery + "\n";
            completeQuery += "} ";
            return completeQuery;
        }
        public string queryformation(string object1, string object2, int distance)
        {
            object1 = GoodFormat(object1);
            object2 = GoodFormat(object2);
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
            Uri uriResource = new Uri("http://dbpedia.org/resource/");
            object1 = uriResource.ToString() + object1;
            object2 = uriResource.ToString() + object2;
            dictionary["obj"] = new List<string>();
            dictionary["pred"] = new List<string>();
            if (distance == 1)
            {
                string string1 = "<" + object1 + ">" + " ?pf1 " + "<" + object2 + ">";
                dictionary["pred"].Add("?pf1");
                return completeQuery(string1);
            }
            else
            {
                string query2 = "<" + object1 + ">" + " ?pf1 ?of1 " + ".\n";
                dictionary["pred"].Add("?pf1");
                dictionary["obj"].Add("?of1");
                for (int i = 1; i < distance - 1; i++)
                {
                    query2 += "?of" + i + "?pf" + (i + 1) + " ?of" + (i + 1) + ".\n";
                    dictionary["pred"].Add("?pf" + (i + 1));
                    dictionary["obj"].Add("?of" + (i + 1));
                }
                query2 += "?of" + (distance - 1) + " ?pf" + distance + ' ' + "<" + object2 + ">";
                dictionary["pred"].Add("?pf" + distance);
                return completeQuery(query2);
            }

        }

        public void Relations_Method()
        {
            string object1 = GoodFormat((Relation_object1_text.Text));
            string object2 = GoodFormat((Relation_object2_text.Text));

            List<string> Relation_result_text = new List<string>();
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org")
            {
                RdfAcceptHeader = "application/turtle"
            };
            //Pour la partie WPF, il faudra demander les deux noeuds, la limite, et la distance max
            Dictionary<int, List<String>> listQueries = DictionarygetQueries(object1, object2,Convert.ToInt32(Relation_distance_text.Text),Convert.ToInt32(Relation_number_result_text.Text));
            string good_format;
            foreach (KeyValuePair<int, List<string>> entry in listQueries)
            {
                foreach (string i in entry.Value)
                {
                    //Console.WriteLine(i);
                    SparqlResultSet results = endpoint.QueryWithResultSet(i);
                    foreach (SparqlResult result in results)
                    {
                        if (result.LongCount() == 1)
                        {
                            good_format = result.ToString();
                            String[] tab = good_format.Split('/');
                            good_format = tab[tab.Length - 1];
                            Relation_result_text.Add(good_format);
                            //Console.WriteLine(good_format);
                        }
                        //Console.WriteLine(result); 
                        else
                        {
                            string finalnode = "";

                            for (int j = 0; j < result.LongCount(); j++)
                            {
                                string good = "";
                                if (result.ToString().Contains("http://dbpedia") && result.ToString().Contains("lang") == false)
                                {
                                    if (result[j].ToString().Contains("http://dbpedia"))
                                    {
                                        good = result[j].ToString();
                                        String[] tab = good.Split('/');
                                        good = tab[tab.Length - 1];
                                    }

                                    else if (result[j].ToString().Contains("w3"))
                                    {
                                        good = result[j].ToString();
                                        String[] tab = good.Split('#');
                                        good = tab[tab.Length - 1];
                                    }
                                    finalnode += good + " ";
                                }

                                else break;
                            }
                            if (finalnode != "")
                                //Console.WriteLine(finalnode);
                                Relation_result_text.Add(finalnode);

                        }




                    }
                }
            }
            Relation_Result_List.ItemsSource = Relation_result_text;
        }
        public Dictionary<int, List<String>> DictionarygetQueries(string object1, string object2, int maxDistance, int limit)
        {
            Dictionary<int, List<string>> queries = new Dictionary<int, List<string>>();
            for (int distance = 1; distance <= maxDistance; distance++)
            {
                // get direct connection in both directions
                queries[distance] = new List<String>();
                queries[distance].Add(queryformation(object1, object2, distance));
                queries[distance].Add(queryformation(object2, object1, distance));

                for (int a = 1; a <= distance; a++)
                {
                    for (int b = 1; b <= distance; b++)
                    {
                        if ((a + b) == distance)
                        {
                            queries[distance].Add(ConnectedViaAMiddleObject(object1, object2, a, b, true));
                            queries[distance].Add(ConnectedViaAMiddleObject(object1, object2, a, b, false));
                        }
                    }
                }
            }
            return queries;
        }

        public string ConnectedViaAMiddleObject(string object1, string object2, int dist1, int dist2, bool toObject)
        {
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
            dictionary["obj"] = new List<string>();
            dictionary["pred"] = new List<string>();
            dictionary["obj"].Add("?middle");
            Uri uriResource = new Uri("http://dbpedia.org/resource/");
            object1 = "<" + uriResource.ToString() + object1 + ">";
            object2 = "<" + uriResource.ToString() + object2 + ">";
            string fs = "f";
            int tmpdist = dist1;
            int twice = 0;
            string coreQuery = "";
            string obj = object1;
            while (twice < 2)
            {

                if (tmpdist == 1)
                {
                    if (toObject)
                    { coreQuery += obj + ' ' + "?p" + fs + '1' + ' ' + "?middle" + " . \n"; }
                    else
                    { coreQuery += "?middle" + ' ' + "?p" + fs + '1' + ' ' + obj + " . \n"; }

                    dictionary["pred"].Add("?p" + fs + '1');
                }
                else
                {
                    if (toObject)
                    { coreQuery += obj + ' ' + "?p" + fs + '1' + ' ' + "?o" + fs + '1' + " . \n"; }
                    else
                    { coreQuery += obj + ' ' + "?p" + fs + '1' + ' ' + "?middle" + " . \n"; }


                    for (int x = 1; x < tmpdist; x++)
                    {
                        string s = "?o" + fs + x;
                        string p = "?p" + fs + (x + 1);
                        dictionary["obj"].Add(s);
                        dictionary["pred"].Add(p);
                        if ((x + 1) == tmpdist)
                        {
                            if (toObject)
                            {
                                coreQuery += s + ' ' + p + ' ' + "?middle" + " . \n";
                            }
                            else
                            { coreQuery += "?middle" + ' ' + p + ' ' + s + " . \n"; }

                        }
                        else
                        {
                            if (toObject)
                            {
                                coreQuery += s + ' ' + p + ' ' + "?o" + fs + (x + 1) + " . \n";
                            }
                            else
                            {
                                coreQuery += "?o" + fs + (x + 1) + ' ' + p + ' ' + s + " . \n";
                            }
                        }
                    }
                }
                twice++;
                fs = "s";
                tmpdist = dist2;
                obj = object2;

            }//end while
            return completeQuery(coreQuery);
        }
        private void Relation_Button_Click(object sender, RoutedEventArgs e)
        {
            Relations_Method();
            Relation_Result_List.Visibility = Visibility.Visible;

        }

        private void Relations_Quit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Relations_Main_Menu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow new_window = new MainWindow();
            new_window.Show();
            this.Close();
        }
    }
}
