using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
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
            this.Text = "TransConnect - Statistiques";
            this.Size = new Size(1000, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

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

            tabStats = new TabControl
            {
                Location = new Point(20, 80),
                Size = new Size(960, 550),
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(tabStats);

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

            Label lblChartTitle = new Label
            {
                Text = "Évolution des revenus",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(0, 10),
                Size = new Size(930, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlRevenueChart.Controls.Add(lblChartTitle);

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
            Panel pnlRevenueChart = (Panel)tabRevenue.Controls["pnlRevenueChart"];
            if (pnlRevenueChart == null)
            {
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

            pnlRevenueChart.Controls.Clear();

            Label lblTitle = new Label
            {
                Text = $"Revenus mensuels ({dtpDebut.Value:MMM yy} - {dtpFin.Value:MMM yy})",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(0, 10),
                Size = new Size(930, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlRevenueChart.Controls.Add(lblTitle);

            DateTime debut = dtpDebut.Value;
            DateTime fin = dtpFin.Value;

            List<DateTime> mois = new List<DateTime>();
            List<decimal> revenus = new List<decimal>();

            for (DateTime date = new DateTime(debut.Year, debut.Month, 1); date <= fin; date = date.AddMonths(1))
            {
                if (date > fin) break;

                decimal revenuMensuel = commandes
                    .Where(c => c.Date.Year == date.Year && c.Date.Month == date.Month)
                    .Sum(c => c.Prix);

                mois.Add(date);
                revenus.Add(revenuMensuel);
            }

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


            decimal maxRevenu = revenus.Count > 0 ? revenus.Max() : 0;
            maxRevenu = maxRevenu > 0 ? maxRevenu : 1;

            int barWidth = Math.Min(80, (pnlRevenueChart.Width - 100) / Math.Max(1, mois.Count));
            int maxBarHeight = 350;
            int startY = 60;
            int startX = 50;

            for (int i = 0; i < mois.Count; i++)
            {
                string labelText = mois[i].ToString("MMM yy");
                decimal revenu = revenus[i];


                int barHeight = (int)(revenu / maxRevenu * maxBarHeight);
                if (barHeight < 1 && revenu > 0) barHeight = 1; 

                Panel barPanel = new Panel
                {
                    BackColor = Color.SteelBlue,
                    Location = new Point(startX + i * (barWidth + 20), startY + (maxBarHeight - barHeight)),
                    Size = new Size(barWidth, Math.Max(1, barHeight)),
                    BorderStyle = BorderStyle.None
                };
                pnlRevenueChart.Controls.Add(barPanel);

                Label lblMonth = new Label
                {
                    Text = labelText,
                    Font = new Font("Arial", 8),
                    Location = new Point(startX + i * (barWidth + 20), startY + maxBarHeight + 5),
                    Size = new Size(barWidth, 20),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                pnlRevenueChart.Controls.Add(lblMonth);

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

            Label lblLegend = new Label
            {
                Text = "Ce graphique montre les revenus mensuels sur la période sélectionnée",
                Font = new Font("Arial", 9),
                Location = new Point(20, startY + maxBarHeight + 30),
                Size = new Size(890, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlRevenueChart.Controls.Add(lblLegend);

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
        }

        private void ExporterStatistiques()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Fichier CSV (*.csv)|*.csv";
                saveFileDialog.Title = "Exporter les statistiques";
                saveFileDialog.FileName = "statistiques.csv";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(saveFileDialog.FileName, false, System.Text.Encoding.UTF8))
                        {

                            writer.WriteLine("Statistiques par chauffeur");
                            for (int i = 0; i < dgvStatsChauffeurs.Columns.Count; i++)
                            {
                                writer.Write(dgvStatsChauffeurs.Columns[i].HeaderText);
                                if (i < dgvStatsChauffeurs.Columns.Count - 1) writer.Write(";");
                            }
                            writer.WriteLine();
                            foreach (DataGridViewRow row in dgvStatsChauffeurs.Rows)
                            {
                                for (int i = 0; i < dgvStatsChauffeurs.Columns.Count; i++)
                                {
                                    writer.Write(row.Cells[i].Value?.ToString());
                                    if (i < dgvStatsChauffeurs.Columns.Count - 1) writer.Write(";");
                                }
                                writer.WriteLine();
                            }
                            writer.WriteLine();


                            writer.WriteLine("Statistiques des commandes");
                            for (int i = 0; i < dgvStatsCommandes.Columns.Count; i++)
                            {
                                writer.Write(dgvStatsCommandes.Columns[i].HeaderText);
                                if (i < dgvStatsCommandes.Columns.Count - 1) writer.Write(";");
                            }
                            writer.WriteLine();
                            foreach (DataGridViewRow row in dgvStatsCommandes.Rows)
                            {
                                for (int i = 0; i < dgvStatsCommandes.Columns.Count; i++)
                                {
                                    writer.Write(row.Cells[i].Value?.ToString());
                                    if (i < dgvStatsCommandes.Columns.Count - 1) writer.Write(";");
                                }
                                writer.WriteLine();
                            }
                            writer.WriteLine();

                            writer.WriteLine("Statistiques par client");
                            for (int i = 0; i < dgvStatsClients.Columns.Count; i++)
                            {
                                writer.Write(dgvStatsClients.Columns[i].HeaderText);
                                if (i < dgvStatsClients.Columns.Count - 1) writer.Write(";");
                            }
                            writer.WriteLine();
                            foreach (DataGridViewRow row in dgvStatsClients.Rows)
                            {
                                for (int i = 0; i < dgvStatsClients.Columns.Count; i++)
                                {
                                    writer.Write(row.Cells[i].Value?.ToString());
                                    if (i < dgvStatsClients.Columns.Count - 1) writer.Write(";");
                                }
                                writer.WriteLine();
                            }
                        }
                        MessageBox.Show("Statistiques exportées avec succès !", "Exportation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur lors de l'exportation : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}