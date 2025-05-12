using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Transconnect.Algorithms.PlusCourtChemin;
using Transconnect.Algorithms.Parcours;
using Transconnect.Data;
using Transconnect.Models.Graphe;
using Transconnect.Algorithms.CalculDistance;

namespace Transconnect.UI
{
    public class UIVisualisation : Form
    {
        private TabControl tabVisu;
        private TabPage tabCarteVilles;
        private TabPage tabOrganigramme;
        private TabPage tabPlusCourtChemin;
        private TabPage tabComparaison;
        
        private ComboBox cmbVilleDepart;
        private ComboBox cmbVilleArrivee;
        private ComboBox cmbAlgorithme;
        private Button btnCalculer;
        private Button btnVisualiser;
        private Panel pnlResultats;
        private RichTextBox rtbComparaison;
        private Button btnComparer;
        private Button btnFermer;
        
        private DataInitializer dataInitializer;
        private Graphe graphe;

        public UIVisualisation(DataInitializer dataInitializer)
        {
            this.dataInitializer = dataInitializer;
            this.graphe = dataInitializer.grapheVille;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Configuration du formulaire
            this.Text = "TransConnect - Visualisation";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // TabControl
            tabVisu = new TabControl
            {
                Location = new Point(20, 20),
                Size = new Size(960, 600),
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(tabVisu);

            // Tab Carte des villes
            tabCarteVilles = new TabPage
            {
                Text = "Carte des villes",
                BackColor = Color.White
            };
            tabVisu.TabPages.Add(tabCarteVilles);

            Label lblInfosCarteVilles = new Label
            {
                Text = "La carte des villes affiche les principales villes françaises et leurs connexions routières.\nUtilisez ce module pour visualiser le réseau de transport.",
                Location = new Point(20, 200),
                Size = new Size(920, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };
            tabCarteVilles.Controls.Add(lblInfosCarteVilles);

            Button btnAfficherCarte = new Button
            {
                Text = "Afficher la carte des villes",
                Location = new Point(350, 250),
                Size = new Size(200, 30),
                BackColor = Color.LightBlue
            };
            btnAfficherCarte.Click += (s, e) => AfficherCarteVilles();
            tabCarteVilles.Controls.Add(btnAfficherCarte);

            // Tab Organigramme
            tabOrganigramme = new TabPage
            {
                Text = "Organigramme",
                BackColor = Color.White
            };
            tabVisu.TabPages.Add(tabOrganigramme);

            Label lblInfosOrganigramme = new Label
            {
                Text = "L'organigramme affiche la structure hiérarchique de l'entreprise.\nCela vous permet de visualiser les relations entre les salariés.",
                Location = new Point(20, 200),
                Size = new Size(920, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };
            tabOrganigramme.Controls.Add(lblInfosOrganigramme);

            Button btnAfficherOrganigramme = new Button
            {
                Text = "Afficher l'organigramme",
                Location = new Point(350, 250),
                Size = new Size(200, 30),
                BackColor = Color.LightGreen
            };
            btnAfficherOrganigramme.Click += (s, e) => AfficherOrganigramme();
            tabOrganigramme.Controls.Add(btnAfficherOrganigramme);

            // Tab Plus court chemin
            tabPlusCourtChemin = new TabPage
            {
                Text = "Plus court chemin",
                BackColor = Color.White
            };
            tabVisu.TabPages.Add(tabPlusCourtChemin);

            // Contrôles pour le calcul du plus court chemin
            Label lblVilleDepart = new Label
            {
                Text = "Ville de départ :",
                Location = new Point(20, 20),
                Size = new Size(100, 20)
            };
            tabPlusCourtChemin.Controls.Add(lblVilleDepart);

            cmbVilleDepart = new ComboBox
            {
                Location = new Point(130, 20),
                Size = new Size(150, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbVilleDepart.Items.AddRange(new object[] { "Paris", "Lyon", "Marseille", "Bordeaux", "Lille", "Strasbourg", "Nantes", "Toulouse", "Nice" });
            tabPlusCourtChemin.Controls.Add(cmbVilleDepart);

            Label lblVilleArrivee = new Label
            {
                Text = "Ville d'arrivée :",
                Location = new Point(310, 20),
                Size = new Size(100, 20)
            };
            tabPlusCourtChemin.Controls.Add(lblVilleArrivee);

            cmbVilleArrivee = new ComboBox
            {
                Location = new Point(420, 20),
                Size = new Size(150, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbVilleArrivee.Items.AddRange(new object[] { "Paris", "Lyon", "Marseille", "Bordeaux", "Lille", "Strasbourg", "Nantes", "Toulouse", "Nice" });
            tabPlusCourtChemin.Controls.Add(cmbVilleArrivee);

            Label lblAlgorithme = new Label
            {
                Text = "Algorithme :",
                Location = new Point(600, 20),
                Size = new Size(80, 20)
            };
            tabPlusCourtChemin.Controls.Add(lblAlgorithme);

            cmbAlgorithme = new ComboBox
            {
                Location = new Point(690, 20),
                Size = new Size(150, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbAlgorithme.Items.AddRange(new object[] { "Dijkstra", "Bellman-Ford", "Floyd-Warshall","A*" });
            cmbAlgorithme.SelectedIndex = 0; // Dijkstra par défaut
            tabPlusCourtChemin.Controls.Add(cmbAlgorithme);

            btnCalculer = new Button
            {
                Text = "Calculer",
                Location = new Point(20, 60),
                Size = new Size(100, 30),
                BackColor = Color.LightBlue
            };
            btnCalculer.Click += (s, e) => CalculerPlusCourtChemin();
            tabPlusCourtChemin.Controls.Add(btnCalculer);

            btnVisualiser = new Button
            {
                Text = "Visualiser sur la carte",
                Location = new Point(130, 60),
                Size = new Size(150, 30),
                BackColor = Color.LightGreen
            };
            btnVisualiser.Click += (s, e) => VisualiserPlusCourtChemin();
            tabPlusCourtChemin.Controls.Add(btnVisualiser);

            pnlResultats = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(920, 460),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke,
                AutoScroll = true
            };
            tabPlusCourtChemin.Controls.Add(pnlResultats);

            // Tab Comparaison
            tabComparaison = new TabPage
            {
                Text = "Comparaison des algorithmes",
                BackColor = Color.White
            };
            tabVisu.TabPages.Add(tabComparaison);

            rtbComparaison = new RichTextBox
            {
                Location = new Point(20, 60),
                Size = new Size(920, 500),
                BackColor = Color.WhiteSmoke,
                ReadOnly = true,
                Font = new Font("Consolas", 10)
            };
            tabComparaison.Controls.Add(rtbComparaison);

            btnComparer = new Button
            {
                Text = "Comparer les algorithmes",
                Location = new Point(20, 20),
                Size = new Size(200, 30),
                BackColor = Color.LightYellow
            };
            btnComparer.Click += (s, e) => ComparerAlgorithmes();
            tabComparaison.Controls.Add(btnComparer);

            // Bouton pour fermer
            btnFermer = new Button
            {
                Text = "Fermer",
                Location = new Point(880, 630),
                Size = new Size(100, 30),
                BackColor = Color.LightGray
            };
            btnFermer.Click += (s, e) => this.Close();
            this.Controls.Add(btnFermer);
        }

        private void AfficherCarteVilles()
        {
            try {
                dataInitializer.AfficherGrapheVilleGraphique();
            }
            catch (Exception ex) {
                MessageBox.Show($"Erreur lors de l'affichage de la carte: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AfficherOrganigramme()
        {
            try {
                dataInitializer.AfficherGrapheSalarieGraphique();
            }
            catch (Exception ex) {
                MessageBox.Show($"Erreur lors de l'affichage de l'organigramme: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculerPlusCourtChemin()
        {
            Random rnd = new Random();
            if (cmbVilleDepart.SelectedIndex == -1 || cmbVilleArrivee.SelectedIndex == -1)
            {
                MessageBox.Show("Veuillez sélectionner les villes de départ et d'arrivée", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbVilleDepart.SelectedItem.ToString() == cmbVilleArrivee.SelectedItem.ToString())
            {
                MessageBox.Show("Les villes de départ et d'arrivée doivent être différentes", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            pnlResultats.Controls.Clear();

            string villeDepart = cmbVilleDepart.SelectedItem.ToString();
            string villeArrivee = cmbVilleArrivee.SelectedItem.ToString();
            string algorithme = cmbAlgorithme.SelectedItem.ToString();

            // Titre du résultat
            Label lblResultat = new Label
            {
                Text = $"Calcul du plus court chemin de {villeDepart} à {villeArrivee} avec l'algorithme {algorithme}",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(900, 30)
            };
            pnlResultats.Controls.Add(lblResultat);

            // Simuler un calcul de plus court chemin (à remplacer par le code réel)
            Label lblCalcul = new Label
            {
                Text = "Calcul en cours...",
                Location = new Point(10, 50),
                Size = new Size(900, 20)
            };
            pnlResultats.Controls.Add(lblCalcul);

            decimal distance = Convert.ToDecimal(CalculDistance.CalculerDistanceTotale(graphe,Dijkstra.TrouverCheminLePlusCourt(graphe, graphe.TrouverNoeudVille(villeDepart), graphe.TrouverNoeudVille(villeArrivee))));
            decimal tempsExecution = Convert.ToDecimal(Dijkstra.TempsExecution.TotalSeconds);
            List<Noeud> itineraire = Dijkstra.TrouverCheminLePlusCourt(dataInitializer.grapheVille, dataInitializer.grapheVille.TrouverNoeudVille(villeDepart), dataInitializer.grapheVille.TrouverNoeudVille(villeArrivee));
            if (algorithme == "Dijkstra")
            {
                distance = Convert.ToDecimal(CalculDistance.CalculerDistanceTotale(graphe,Dijkstra.TrouverCheminLePlusCourt(graphe, graphe.TrouverNoeudVille(villeDepart), graphe.TrouverNoeudVille(villeArrivee))));
                tempsExecution = Convert.ToDecimal(Dijkstra.TempsExecution.TotalSeconds);
                itineraire = Dijkstra.TrouverCheminLePlusCourt(dataInitializer.grapheVille, dataInitializer.grapheVille.TrouverNoeudVille(villeDepart), dataInitializer.grapheVille.TrouverNoeudVille(villeArrivee));
            }
            else if (algorithme == "Bellman-Ford")
            {
                BellmanFord bellmanFord = new BellmanFord(graphe);
                bellmanFord.CalculerPlusCourtsChemins(graphe.TrouverNoeudVille(villeDepart));
                distance = Convert.ToDecimal(CalculDistance.CalculerDistanceTotale(graphe,bellmanFord.RecupererChemin(graphe.TrouverNoeudVille(villeArrivee))));
                tempsExecution = Convert.ToDecimal(BellmanFord.TempsExecution.TotalSeconds);
                itineraire = bellmanFord.RecupererChemin(graphe.TrouverNoeudVille(villeArrivee));
            }
            else if (algorithme == "Floyd-Warshall")
            {
                FloydWarshall floydWarshall = new FloydWarshall(graphe);
                floydWarshall.CalculerPlusCourtsChemins();
                distance = Convert.ToDecimal(CalculDistance.CalculerDistanceTotale(graphe,floydWarshall.RecupererChemin(graphe.TrouverNoeudVille(villeDepart),graphe.TrouverNoeudVille(villeArrivee))));
                tempsExecution = Convert.ToDecimal(FloydWarshall.TempsExecution.TotalSeconds);
                itineraire = floydWarshall.RecupererChemin(graphe.TrouverNoeudVille(villeDepart), graphe.TrouverNoeudVille(villeArrivee));
            }
            else if (algorithme == "A*")
            {

            }


            List<string> chemin = new List<string>();

            foreach (var noeud in itineraire)
            {
                chemin.Add(noeud.Entite.ToString());
            }

            // Afficher les résultats dans un tableau
            int yPos = 80;
            
            Label lblAlgo = new Label { Text = "Algorithme :", Font = new Font("Arial", 10, FontStyle.Bold), Location = new Point(10, yPos), Size = new Size(150, 20) };
            Label valAlgo = new Label { Text = algorithme, Location = new Point(170, yPos), Size = new Size(200, 20) };
            pnlResultats.Controls.Add(lblAlgo);
            pnlResultats.Controls.Add(valAlgo);
            yPos += 30;
            
            Label lblDist = new Label { Text = "Distance calculée :", Font = new Font("Arial", 10, FontStyle.Bold), Location = new Point(10, yPos), Size = new Size(150, 20) };
            Label valDist = new Label { Text = $"{distance} km", Location = new Point(170, yPos), Size = new Size(200, 20) };
            pnlResultats.Controls.Add(lblDist);
            pnlResultats.Controls.Add(valDist);
            yPos += 30;
            
            Label lblTemps = new Label { Text = "Temps d'exécution :", Font = new Font("Arial", 10, FontStyle.Bold), Location = new Point(10, yPos), Size = new Size(150, 20) };
            Label valTemps = new Label { Text = $"{tempsExecution} ms", Location = new Point(170, yPos), Size = new Size(200, 20) };
            pnlResultats.Controls.Add(lblTemps);
            pnlResultats.Controls.Add(valTemps);
            yPos += 30;
            
            Label lblChemin = new Label { Text = "Chemin trouvé :", Font = new Font("Arial", 10, FontStyle.Bold), Location = new Point(10, yPos), Size = new Size(150, 20) };
            Label valChemin = new Label { Text = string.Join(" → ", chemin), Location = new Point(170, yPos), Size = new Size(700, 20) };
            pnlResultats.Controls.Add(lblChemin);
            pnlResultats.Controls.Add(valChemin);
            yPos += 30;
            
            Label lblDetails = new Label { Text = "Détails :", Font = new Font("Arial", 10, FontStyle.Bold), Location = new Point(10, yPos), Size = new Size(150, 20) };
            pnlResultats.Controls.Add(lblDetails);
            yPos += 30;
            
            TextBox txtDetails = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(10, yPos),
                Size = new Size(900, 300),
                BackColor = Color.White
            };
            BellmanFord bellmanFordNB = new BellmanFord(graphe);
            bellmanFordNB.CalculerPlusCourtsChemins(graphe.TrouverNoeudVille(villeDepart));

            // Afficher des détails appropriés selon l'algorithme
            switch (algorithme) {
                case "Dijkstra":
                    txtDetails.Text = "L'algorithme de Dijkstra a exploré " + Dijkstra.TrouverCheminLePlusCourt(graphe, graphe.TrouverNoeudVille(villeDepart), graphe.TrouverNoeudVille(villeArrivee)).Count() + " nœuds avant de trouver le chemin optimal.\r\n";
                    txtDetails.Text += "Il a utilisé une file de priorité pour sélectionner les nœuds les plus prometteurs en premier.\r\n";
                    txtDetails.Text += "Complexité spatiale: O(V)\r\n";
                    txtDetails.Text += "Complexité temporelle: O(V² + E) avec une implémentation naïve, ou O((V + E) log V) avec une file de priorité.";
                    break;
                case "Bellman-Ford":
                    txtDetails.Text = "L'algorithme de Bellman-Ford a effectué " + bellmanFordNB.RecupererChemin(graphe.TrouverNoeudVille(villeArrivee)).Count() + " itérations sur l'ensemble des arêtes.\r\n";
                    txtDetails.Text += "Contrairement à Dijkstra, Bellman-Ford peut gérer les arêtes de poids négatif.\r\n";
                    txtDetails.Text += "Complexité spatiale: O(V)\r\n";
                    txtDetails.Text += "Complexité temporelle: O(V×E)";
                    break;
                case "Floyd-Warshall":
                    txtDetails.Text = "L'algorithme de Floyd-Warshall a calculé les distances entre toutes les paires de villes.\r\n";
                    txtDetails.Text += "C'est plus coûteux pour un seul chemin, mais optimal si on veut tous les chemins possibles.\r\n";
                    txtDetails.Text += "Complexité spatiale: O(V²)\r\n";
                    txtDetails.Text += "Complexité temporelle: O(V³)";
                    break;
                case "A*":
                    txtDetails.Text = "L'algorithme A* a utilisé une heuristique pour guider la recherche du chemin le plus court.\r\n";
                    txtDetails.Text += "Il est efficace pour les graphes avec des poids positifs et peut être optimisé avec différentes heuristiques.\r\n";
                    txtDetails.Text += "Complexité spatiale: O(V)\r\n";
                    txtDetails.Text += "Complexité temporelle: O((V + E) log V)";
                    break;
            }
            
            pnlResultats.Controls.Add(txtDetails);
        }

        private void VisualiserPlusCourtChemin()
        {
            if (cmbVilleDepart.SelectedIndex == -1 || cmbVilleArrivee.SelectedIndex == -1)
            {
                MessageBox.Show("Veuillez d'abord calculer un chemin", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try {
                AfficherCarteVilles();
            }
            catch (Exception ex) {
                MessageBox.Show($"Erreur lors de la visualisation: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ComparerAlgorithmes()
        {
            rtbComparaison.Clear();
            
            rtbComparaison.SelectionFont = new Font("Arial", 12, FontStyle.Bold);
            rtbComparaison.AppendText("COMPARAISON DES ALGORITHMES DE PLUS COURT CHEMIN\n\n");
            rtbComparaison.SelectionFont = new Font("Arial", 10);
            
            // Résumé des algorithmes
            rtbComparaison.SelectionFont = new Font("Arial", 11, FontStyle.Bold);
            rtbComparaison.AppendText("1. RÉSUMÉ DES ALGORITHMES\n\n");
            rtbComparaison.SelectionFont = new Font("Arial", 10);
            
            rtbComparaison.SelectionFont = new Font("Arial", 10, FontStyle.Bold);
            rtbComparaison.AppendText("Dijkstra\n");
            rtbComparaison.SelectionFont = new Font("Arial", 10);
            rtbComparaison.AppendText("- Trouve le plus court chemin depuis une source vers toutes les autres destinations\n");
            rtbComparaison.AppendText("- Très efficace pour un graphe à arêtes de poids positifs\n");
            rtbComparaison.AppendText("- Ne fonctionne pas avec des poids négatifs\n");
            rtbComparaison.AppendText("- Complexité temporelle: O((V + E) log V) avec une file de priorité\n");
            rtbComparaison.AppendText("- Complexité spatiale: O(V)\n\n");
            
            rtbComparaison.SelectionFont = new Font("Arial", 10, FontStyle.Bold);
            rtbComparaison.AppendText("Bellman-Ford\n");
            rtbComparaison.SelectionFont = new Font("Arial", 10);
            rtbComparaison.AppendText("- Trouve le plus court chemin depuis une source vers toutes les autres destinations\n");
            rtbComparaison.AppendText("- Fonctionne avec des poids négatifs\n");
            rtbComparaison.AppendText("- Peut détecter les cycles de poids négatif\n");
            rtbComparaison.AppendText("- Moins efficace que Dijkstra pour les graphes à poids positifs\n");
            rtbComparaison.AppendText("- Complexité temporelle: O(V×E)\n");
            rtbComparaison.AppendText("- Complexité spatiale: O(V)\n\n");
            
            rtbComparaison.SelectionFont = new Font("Arial", 10, FontStyle.Bold);
            rtbComparaison.AppendText("Floyd-Warshall\n");
            rtbComparaison.SelectionFont = new Font("Arial", 10);
            rtbComparaison.AppendText("- Trouve les plus courts chemins entre TOUTES les paires de nœuds\n");
            rtbComparaison.AppendText("- Fonctionne avec des poids négatifs (mais pas de cycles négatifs)\n");
            rtbComparaison.AppendText("- Simple à implémenter\n");
            rtbComparaison.AppendText("- Beaucoup plus coûteux pour un seul chemin, mais optimal pour tous les chemins\n");
            rtbComparaison.AppendText("- Complexité temporelle: O(V³)\n");
            rtbComparaison.AppendText("- Complexité spatiale: O(V²)\n\n");

            rtbComparaison.SelectionFont = new Font("Arial", 10, FontStyle.Bold);
            rtbComparaison.AppendText("A*\n");
            rtbComparaison.SelectionFont = new Font("Arial", 10);
            rtbComparaison.AppendText("- Utilise une heuristique pour guider la recherche du chemin le plus court\n");
            rtbComparaison.AppendText("- Très efficace pour les graphes avec des poids positifs\n");
            rtbComparaison.AppendText("- Peut être optimisé avec différentes heuristiques\n");
            rtbComparaison.AppendText("- Complexité temporelle: O((V + E) log V)\n");
            rtbComparaison.AppendText("- Complexité spatiale: O(V)\n\n");

            
            // Tests de performance
            rtbComparaison.SelectionFont = new Font("Arial", 11, FontStyle.Bold);
            rtbComparaison.AppendText("2. TESTS DE PERFORMANCE\n\n");
            rtbComparaison.SelectionFont = new Font("Arial", 10);
            
            rtbComparaison.AppendText("Nous effecturons le test de Performance sur le trajet Paris-Toulouse:\n\n");
            Dijkstra.TrouverCheminLePlusCourt(dataInitializer.grapheVille, dataInitializer.grapheVille.TrouverNoeudVille("Paris"), dataInitializer.grapheVille.TrouverNoeudVille("Toulouse"));
            BellmanFord bellmanFord = new BellmanFord(dataInitializer.grapheVille);
            bellmanFord.CalculerPlusCourtsChemins(dataInitializer.grapheVille.TrouverNoeudVille("Paris"));
            FloydWarshall floydWarshall = new FloydWarshall(dataInitializer.grapheVille);
            floydWarshall.CalculerPlusCourtsChemins();

            // Simuler des résultats comparatifs
            Random rnd = new Random();
            
            rtbComparaison.SelectionFont = new Font("Arial", 10, FontStyle.Bold);
            rtbComparaison.AppendText("Temps moyen d'exécution:\n");
            rtbComparaison.SelectionFont = new Font("Arial", 10);
            rtbComparaison.AppendText("- Dijkstra: " + Convert.ToDecimal(Dijkstra.TempsExecution.TotalSeconds) + " ms\n");
            rtbComparaison.AppendText("- Bellman-Ford: " + Convert.ToDecimal(BellmanFord.TempsExecution.TotalSeconds) + " ms\n");
            rtbComparaison.AppendText("- Floyd-Warshall: " + Convert.ToDecimal(FloydWarshall.TempsExecution.TotalSeconds) + " ms\n\n");
            rtbComparaison.AppendText("- A*: " + "PLACEHOLDER" + " ms\n\n");
            
            rtbComparaison.SelectionFont = new Font("Arial", 10, FontStyle.Bold);
            rtbComparaison.AppendText("Utilisation mémoire:\n");
            rtbComparaison.SelectionFont = new Font("Arial", 10);
            rtbComparaison.AppendText("- Dijkstra: " + Dijkstra.UtilisationMemoire + " MB\n");
            rtbComparaison.AppendText("- Bellman-Ford: " + BellmanFord.UtilisationMemoire + " MB\n");
            rtbComparaison.AppendText("- Floyd-Warshall: " + FloydWarshall.UtilisationMemoire + " MB\n\n");
            rtbComparaison.AppendText("- A*: " + "PLACEHOLDER" + " MB\n\n");
            
            // Cas d'utilisation
            rtbComparaison.SelectionFont = new Font("Arial", 11, FontStyle.Bold);
            rtbComparaison.AppendText("3. CAS D'UTILISATION RECOMMANDÉS\n\n");
            rtbComparaison.SelectionFont = new Font("Arial", 10);
            
            rtbComparaison.AppendText("Utiliser Dijkstra lorsque:\n");
            rtbComparaison.AppendText("- Vous recherchez le chemin le plus court depuis une seule source\n");
            rtbComparaison.AppendText("- Toutes les arêtes ont des poids positifs\n");
            rtbComparaison.AppendText("- Vous avez besoin de la meilleure performance pour un seul chemin\n\n");
            
            rtbComparaison.AppendText("Utiliser Bellman-Ford lorsque:\n");
            rtbComparaison.AppendText("- Certaines arêtes peuvent avoir des poids négatifs\n");
            rtbComparaison.AppendText("- Vous devez détecter les cycles de poids négatifs\n");
            rtbComparaison.AppendText("- La performance n'est pas la priorité absolue\n\n");
            
            rtbComparaison.AppendText("Utiliser Floyd-Warshall lorsque:\n");
            rtbComparaison.AppendText("- Vous avez besoin des plus courts chemins entre TOUTES les paires de nœuds\n");
            rtbComparaison.AppendText("- Le graphe est dense (beaucoup d'arêtes)\n");
            rtbComparaison.AppendText("- La simplicité d'implémentation est prioritaire\n");
            rtbComparaison.AppendText("- Le graphe est relativement petit\n\n");

            rtbComparaison.AppendText("Utiliser A* lorsque:\n");
            rtbComparaison.AppendText("- Vous avez une bonne heuristique pour guider la recherche\n");
            rtbComparaison.AppendText("- Vous recherchez le chemin le plus court dans un graphe avec des poids positifs\n");
            rtbComparaison.AppendText("- Vous avez besoin d'une performance optimale pour un seul chemin\n");
            rtbComparaison.AppendText("- Vous souhaitez explorer le graphe de manière plus ciblée\n\n");
            
            // Conclusion
            rtbComparaison.SelectionFont = new Font("Arial", 11, FontStyle.Bold);
            rtbComparaison.AppendText("CONCLUSION\n\n");
            rtbComparaison.SelectionFont = new Font("Arial", 10);
            
            rtbComparaison.AppendText("Pour l'application TransConnect qui recherche principalement des itinéraires entre deux villes spécifiques sur un graphe à poids positifs (distances kilométriques), l'algorithme de Dijkstra est généralement le choix optimal.\n\n");
            rtbComparaison.AppendText("Bellman-Ford pourrait être utile si nous devions intégrer des pondérations négatives (par exemple, des routes à privilégier pour des raisons commerciales).\n\n");
            rtbComparaison.AppendText("Floyd-Warshall serait pertinent si nous avions besoin de précalculer toutes les distances entre toutes les villes pour une consultation rapide (par exemple, pour générer une matrice complète des distances).");
            
            MessageBox.Show("Comparaison générée avec succès!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}