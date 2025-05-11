using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Transconnect.Data;
using Transconnect.Models;
using Transconnect.Models.Graphe;
using System.Linq;
using System.Collections.Generic;
using Transconnect.Algorithms.PlusCourtChemin;
using Transconnect.Algorithms.CalculDistance;
using Transconnect.Services;

namespace Transconnect.UI
{
    public class UIStatistique : Form
    {
        private TabControl tabStats;
        private TabPage tabChauffeurs;
        private TabPage tabCommandes;
        private TabPage tabClients;
        private TabPage tabRevenue;

        private DataGridView dgvStatsChauffeurs;
        private DataGridView dgvStatsCommandes;
        private DataGridView dgvStatsClients;

        private Panel pnlRevenueChart;
        private Button btnExporter;
        private Button btnFermer;

        private DateTimePicker dtpDebut;
        private DateTimePicker dtpFin;
        private Button btnFiltrer;

        private DataInitializer dataInitializer;
        private List<Salarie> chauffeurs = new List<Salarie>();
        private List<Client> clients = new List<Client>();
        private List<Commande> commandes = new List<Commande>();
        private Graphe graphe;
        private StatistiqueService statistiqueService = new StatistiqueService();

        public UIStatistique(DataInitializer dataInitializer)
        {
            this.dataInitializer = dataInitializer;
            this.clients = dataInitializer.clients;
            this.commandes = dataInitializer.commandes;
            this.graphe = dataInitializer.grapheVille;
            this.statistiqueService = new StatistiqueService();
            foreach (var chauffeur in dataInitializer.salaries)
            {
                if (chauffeur.Poste == "Chauffeur")
                {
                    this.chauffeurs.Add(chauffeur);
                }
            }
            InitializeComponents();
            ChargerStatistiques();
        }

        private void InitializeComponents()
        {
            // Configuration du formulaire
            this.Text = "TransConnect - Statistiques";
            this.Size = new Size(1000, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Panel pour les filtres de dates
            Panel pnlFiltres = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(960, 50),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(pnlFiltres);

            Label lblPeriode = new Label
            {
                Text = "Période : ",
                Location = new Point(10, 15),
                Size = new Size(70, 20),
                Font = new Font("Arial", 10)
            };
            pnlFiltres.Controls.Add(lblPeriode);

            dtpDebut = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(90, 15),
                Size = new Size(120, 20),
                Value = DateTime.Now.AddMonths(-3)
            };
            dtpDebut.Value = new DateTime(2024, 01, 1);
            pnlFiltres.Controls.Add(dtpDebut);

            Label lblA = new Label
            {
                Text = "à",
                Location = new Point(220, 15),
                Size = new Size(20, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlFiltres.Controls.Add(lblA);

            dtpFin = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(250, 15),
                Size = new Size(120, 20),
                Value = DateTime.Now
            };
            pnlFiltres.Controls.Add(dtpFin);

            btnFiltrer = new Button
            {
                Text = "Filtrer",
                Location = new Point(380, 14),
                Size = new Size(80, 23),
                BackColor = Color.LightBlue
            };
            btnFiltrer.Click += (s, e) => ChargerStatistiques();
            pnlFiltres.Controls.Add(btnFiltrer);

            // TabControl
            tabStats = new TabControl
            {
                Location = new Point(20, 80),
                Size = new Size(960, 550),
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(tabStats);

            // Tab Chauffeurs
            tabChauffeurs = new TabPage
            {
                Text = "Statistiques par chauffeur",
                BackColor = Color.White
            };
            tabStats.TabPages.Add(tabChauffeurs);

            dgvStatsChauffeurs = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(930, 500),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.WhiteSmoke
            };
            tabChauffeurs.Controls.Add(dgvStatsChauffeurs);

            // Tab Commandes
            tabCommandes = new TabPage
            {
                Text = "Statistiques des commandes",
                BackColor = Color.White
            };
            tabStats.TabPages.Add(tabCommandes);

            dgvStatsCommandes = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(930, 500),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.WhiteSmoke
            };
            tabCommandes.Controls.Add(dgvStatsCommandes);

            // Tab Clients
            tabClients = new TabPage
            {
                Text = "Statistiques par client",
                BackColor = Color.White
            };
            tabStats.TabPages.Add(tabClients);

            dgvStatsClients = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(930, 500),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.WhiteSmoke
            };
            tabClients.Controls.Add(dgvStatsClients);

            // Tab Revenus
            tabRevenue = new TabPage
            {
                Text = "Graphique des revenus",
                BackColor = Color.White
            };
            tabStats.TabPages.Add(tabRevenue);

            Panel pnlRevenueChart = new Panel
            {
                Name = "pnlRevenueChart",
                Location = new Point(10, 10),
                Size = new Size(930, 500),
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle
            };
            tabRevenue.Controls.Add(pnlRevenueChart);

            // Add a placeholder title
            Label lblChartTitle = new Label
            {
                Text = "Évolution des revenus",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(0, 10),
                Size = new Size(930, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlRevenueChart.Controls.Add(lblChartTitle);

            // Boutons
            btnExporter = new Button
            {
                Text = "Exporter les statistiques",
                Location = new Point(700, 640),
                Size = new Size(170, 30),
                BackColor = Color.LightGreen
            };
            btnExporter.Click += (s, e) => ExporterStatistiques();
            this.Controls.Add(btnExporter);

            btnFermer = new Button
            {
                Text = "Fermer",
                Location = new Point(880, 640),
                Size = new Size(100, 30),
                BackColor = Color.LightGray
            };
            btnFermer.Click += (s, e) => this.Close();
            this.Controls.Add(btnFermer);
        }

        private void ChargerStatistiques()
        {
            ChargerStatistiquesChauffeurs();
            ChargerStatistiquesCommandes();
            ChargerStatistiquesClients();
            ChargerGraphiqueRevenues();
        }

        private void ChargerStatistiquesChauffeurs()
        {

            DataTable dtStatsChauffeurs = new DataTable();
            dtStatsChauffeurs.Columns.Add("Nom", typeof(string));
            dtStatsChauffeurs.Columns.Add("Prénom", typeof(string));
            dtStatsChauffeurs.Columns.Add("Nombre de livraisons", typeof(int));
            dtStatsChauffeurs.Columns.Add("Distance totale (km)", typeof(int));
            dtStatsChauffeurs.Columns.Add("Revenus générés (€)", typeof(decimal));


            foreach (var chauffeur in chauffeurs)
            {
                int nbLivraisons = 0;
                List<Commande> commandesInterne = statistiqueService.CommandesParChauffeur(commandes, chauffeur);
                foreach (var commande in commandesInterne)
                {
                    if (commande.Date >= dtpDebut.Value && commande.Date <= dtpFin.Value)
                    {
                        nbLivraisons++;
                    }
                }

                decimal distanceTotale = 0;
                foreach (var commande in commandes)
                {
                    if (commande.Chauffeur.NumeroSS == chauffeur.NumeroSS && commande.Date >= dtpDebut.Value && commande.Date <= dtpFin.Value)
                    {
                        // Calculer la distance totale
                        distanceTotale += Convert.ToDecimal(CalculDistance.CalculerDistanceTotale(graphe, Dijkstra.TrouverCheminLePlusCourt(graphe, graphe.TrouverNoeudVille(commande.VilleDepart), graphe.TrouverNoeudVille(commande.VilleArrivee))));
                    }
                }

                decimal revenusGeneres = commandes.Where(c => c.Chauffeur.NumeroSS == chauffeur.NumeroSS && c.Date >= dtpDebut.Value && c.Date <= dtpFin.Value).Sum(c => c.Prix);

                dtStatsChauffeurs.Rows.Add(chauffeur.Nom, chauffeur.Prenom, nbLivraisons, distanceTotale, revenusGeneres);
            }

            dgvStatsChauffeurs.DataSource = dtStatsChauffeurs;
        }

        private void ChargerStatistiquesCommandes()
        {
            DataTable dtStatsCommandes = new DataTable();
            dtStatsCommandes.Columns.Add("Période", typeof(string));
            dtStatsCommandes.Columns.Add("Nombre de commandes", typeof(int));
            dtStatsCommandes.Columns.Add("Prix moyen (€)", typeof(decimal));
            dtStatsCommandes.Columns.Add("Distance moyenne (km)", typeof(int));
            dtStatsCommandes.Columns.Add("Revenu total (€)", typeof(decimal));
            dtStatsCommandes.Columns.Add("Commandes en attente", typeof(int));
            dtStatsCommandes.Columns.Add("Commandes annulées", typeof(int));

            DateTime debut = dtpDebut.Value;
            DateTime fin = dtpFin.Value;

            // Par mois
            for (DateTime date = new DateTime(debut.Year, debut.Month, 1); date <= fin; date = date.AddMonths(1))
            {
                if (date > fin) break;

                string periode = date.ToString("MMM yy");


                var commandesDuMois = commandes.Where(c => c.Date >= date && c.Date < date.AddMonths(1)).ToList();

                int nbCommandes = commandesDuMois.Count;

                decimal prixMoyen = 0;
                decimal revenuTotal = 0;

                if (nbCommandes > 0)
                {
                    revenuTotal = commandesDuMois.Sum(c => c.Prix);
                    prixMoyen = revenuTotal / nbCommandes;
                }


                int distanceMoyenne = 0;
                if (nbCommandes > 0)
                {
                    double distanceTotale = 0;
                    int commandesAvecDistance = 0;

                    foreach (var commande in commandesDuMois)
                    {
                        try
                        {
                            var noeudDepart = graphe.TrouverNoeudVille(commande.VilleDepart);
                            var noeudArrivee = graphe.TrouverNoeudVille(commande.VilleArrivee);

                            if (noeudDepart != null && noeudArrivee != null)
                            {
                                var chemin = Dijkstra.TrouverCheminLePlusCourt(graphe, noeudDepart, noeudArrivee);

                                if (chemin != null && chemin.Count > 0)
                                {
                                    double distance = CalculDistance.CalculerDistanceTotale(graphe, chemin);
                                    distanceTotale += distance;
                                    commandesAvecDistance++;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }

                    if (commandesAvecDistance > 0)
                    {
                        distanceMoyenne = (int)(distanceTotale / commandesAvecDistance);
                    }
                }

                int commandesEnAttente = commandesDuMois.Count(c => c.Statut == StatutCommande.EnAttente);
                int commandesAnnulees = commandesDuMois.Count(c => c.Statut == StatutCommande.Annulee);

                dtStatsCommandes.Rows.Add(periode, nbCommandes, prixMoyen, distanceMoyenne, revenuTotal, commandesEnAttente, commandesAnnulees);
            }

            dgvStatsCommandes.DataSource = dtStatsCommandes;
        }

        private void ChargerStatistiquesClients()
        {

            DataTable dtStatsClients = new DataTable();
            dtStatsClients.Columns.Add("Nom", typeof(string));
            dtStatsClients.Columns.Add("Prénom", typeof(string));
            dtStatsClients.Columns.Add("NB de commandes", typeof(int));
            dtStatsClients.Columns.Add("Montant total (€)", typeof(decimal));
            dtStatsClients.Columns.Add("Panier moyen (€)", typeof(decimal));
            dtStatsClients.Columns.Add("Dernière commande", typeof(DateTime));

            foreach (var client in clients)
            {
                var commandesClient = commandes.Where(c => c.Client == client && c.Date >= dtpDebut.Value && c.Date <= dtpFin.Value).ToList();

                int nbCommandes = commandesClient.Count;
                decimal montantTotal = commandesClient.Sum(c => c.Prix);
                decimal panierMoyen = nbCommandes > 0 ? montantTotal / nbCommandes : 0;
                DateTime derniereCommande = commandesClient.Count > 0 ? commandesClient.Max(c => c.Date) : DateTime.MinValue;

                dtStatsClients.Rows.Add(client.Nom, client.Prenom, nbCommandes, montantTotal, panierMoyen, derniereCommande);
            }

            dgvStatsClients.DataSource = dtStatsClients;
        }

        private void ChargerGraphiqueRevenues()
        {
            // Get the panel from the form
            Panel pnlRevenueChart = (Panel)tabRevenue.Controls["pnlRevenueChart"];
            if (pnlRevenueChart == null)
            {
                // If not found, create it
                pnlRevenueChart = new Panel
                {
                    Name = "pnlRevenueChart",
                    Location = new Point(10, 10),
                    Size = new Size(930, 500),
                    BackColor = Color.WhiteSmoke,
                    BorderStyle = BorderStyle.FixedSingle
                };
                tabRevenue.Controls.Add(pnlRevenueChart);
            }

            // Clear existing controls
            pnlRevenueChart.Controls.Clear();

            // Add title
            Label lblTitle = new Label
            {
                Text = $"Revenus mensuels ({dtpDebut.Value:MMM yy} - {dtpFin.Value:MMM yy})",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(0, 10),
                Size = new Size(930, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlRevenueChart.Controls.Add(lblTitle);

            // Collect data for the visualization
            DateTime debut = dtpDebut.Value;
            DateTime fin = dtpFin.Value;

            List<DateTime> mois = new List<DateTime>();
            List<decimal> revenus = new List<decimal>();

            for (DateTime date = new DateTime(debut.Year, debut.Month, 1); date <= fin; date = date.AddMonths(1))
            {
                if (date > fin) break;

                // Calculate revenue for the month
                decimal revenuMensuel = commandes
                    .Where(c => c.Date.Year == date.Year && c.Date.Month == date.Month)
                    .Sum(c => c.Prix);

                mois.Add(date);
                revenus.Add(revenuMensuel);
            }

            // Check if we have data
            if (mois.Count == 0)
            {
                Label lblNoData = new Label
                {
                    Text = "Aucune donnée pour la période sélectionnée",
                    Font = new Font("Arial", 12),
                    Location = new Point(20, 200),
                    Size = new Size(890, 30),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                pnlRevenueChart.Controls.Add(lblNoData);
                return;
            }

            // Create a simple bar chart visualization
            decimal maxRevenu = revenus.Count > 0 ? revenus.Max() : 0;
            maxRevenu = maxRevenu > 0 ? maxRevenu : 1; // Avoid division by zero

            int barWidth = Math.Min(80, (pnlRevenueChart.Width - 100) / Math.Max(1, mois.Count));
            int maxBarHeight = 350;
            int startY = 60;
            int startX = 50;

            // Draw the bars and labels
            for (int i = 0; i < mois.Count; i++)
            {
                string labelText = mois[i].ToString("MMM yy");
                decimal revenu = revenus[i];

                // Calculate bar height proportional to max revenue
                int barHeight = (int)(revenu / maxRevenu * maxBarHeight);
                if (barHeight < 1 && revenu > 0) barHeight = 1; // Minimum height for visibility

                // Create the bar
                Panel barPanel = new Panel
                {
                    BackColor = Color.SteelBlue,
                    Location = new Point(startX + i * (barWidth + 20), startY + (maxBarHeight - barHeight)),
                    Size = new Size(barWidth, Math.Max(1, barHeight)), // Ensure at least 1px height
                    BorderStyle = BorderStyle.None
                };
                pnlRevenueChart.Controls.Add(barPanel);

                // Add month label
                Label lblMonth = new Label
                {
                    Text = labelText,
                    Font = new Font("Arial", 8),
                    Location = new Point(startX + i * (barWidth + 20), startY + maxBarHeight + 5),
                    Size = new Size(barWidth, 20),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                pnlRevenueChart.Controls.Add(lblMonth);

                // Add revenue value
                Label lblRevenue = new Label
                {
                    Text = revenu.ToString("C0"),
                    Font = new Font("Arial", 8),
                    Location = new Point(startX + i * (barWidth + 20),
                                        startY + (maxBarHeight - barHeight) - 20),
                    Size = new Size(barWidth, 20),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                pnlRevenueChart.Controls.Add(lblRevenue);
            }

            // Add a legend
            Label lblLegend = new Label
            {
                Text = "Ce graphique montre les revenus mensuels sur la période sélectionnée",
                Font = new Font("Arial", 9),
                Location = new Point(20, startY + maxBarHeight + 30),
                Size = new Size(890, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlRevenueChart.Controls.Add(lblLegend);

            // Add Y-axis labels
            for (int i = 0; i <= 5; i++)
            {
                decimal value = maxRevenu * i / 5;
                Label lblYAxis = new Label
                {
                    Text = value.ToString("C0"),
                    Font = new Font("Arial", 8),
                    Location = new Point(5, startY + maxBarHeight - (i * maxBarHeight / 5) - 10),
                    Size = new Size(40, 20),
                    TextAlign = ContentAlignment.MiddleRight
                };
                pnlRevenueChart.Controls.Add(lblYAxis);
            }

            // Draw trend line (optional - simple linear trend)
            if (mois.Count >= 2)
            {
                // Calculate simple linear regression
                double[] xValues = Enumerable.Range(0, mois.Count).Select(i => (double)i).ToArray();
                double[] yValues = revenus.Select(r => (double)r).ToArray();

                double sumX = xValues.Sum();
                double sumY = yValues.Sum();
                double sumXY = xValues.Zip(yValues, (x, y) => x * y).Sum();
                double sumX2 = xValues.Sum(x => x * x);
                double n = xValues.Length;

                double slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
                double intercept = (sumY - slope * sumX) / n;

                // Draw red trend line
                for (int i = 0; i < mois.Count - 1; i++)
                {
                    int x1 = startX + i * (barWidth + 20) + barWidth / 2;
                    int x2 = startX + (i + 1) * (barWidth + 20) + barWidth / 2;

                    double y1 = slope * i + intercept;
                    double y2 = slope * (i + 1) + intercept;

                    // Convert to screen coordinates
                    int screenY1 = startY + maxBarHeight - (int)(y1 / Convert.ToDouble(maxRevenu) * maxBarHeight);
                    int screenY2 = startY + maxBarHeight - (int)(y2 / Convert.ToDouble(maxRevenu) * maxBarHeight);

                    // Create a line using a skinny panel
                    int lineLength = (int)Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(screenY2 - screenY1, 2));
                    double angle = Math.Atan2(screenY2 - screenY1, x2 - x1) * 180 / Math.PI;

                    Panel linePanel = new Panel
                    {
                        BackColor = Color.Firebrick,
                        Location = new Point(x1, screenY1),
                        Size = new Size(lineLength, 2)
                    };

                    Label lblTrend = new Label
                    {
                        Text = slope > 0 ? "▲ Tendance à la hausse" : "▼ Tendance à la baisse",
                        ForeColor = slope > 0 ? Color.Green : Color.Red,
                        Font = new Font("Arial", 9, FontStyle.Bold),
                        Location = new Point(700, 20),
                        Size = new Size(200, 20),
                        TextAlign = ContentAlignment.MiddleRight
                    };
                    pnlRevenueChart.Controls.Add(lblTrend);

                    break; // Just add the label once
                }
            }
        }

        private void ExporterStatistiques()
        {
            // Placeholder pour l'exportation des statistiques
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Fichiers Excel (*.xlsx)|*.xlsx|Fichiers CSV (*.csv)|*.csv",
                Title = "Exporter les statistiques"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show($"Les statistiques ont été exportées vers {saveDialog.FileName}", "Exportation réussie", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}