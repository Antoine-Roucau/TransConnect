using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Transconnect.Data;

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
        
        private Chart chartRevenues;
        private Button btnExporter;
        private Button btnFermer;
        
        private DateTimePicker dtpDebut;
        private DateTimePicker dtpFin;
        private Button btnFiltrer;
        
        private DataInitializer dataInitializer;

        public UIStatistique(DataInitializer dataInitializer)
        {
            this.dataInitializer = dataInitializer;
            InitializeComponents();
            ChargerStatistiques();
        }

        private void InitializeComponents()
        {
            // Configuration du formulaire
            this.Text = "TransConnect - Statistiques";
            this.Size = new Size(1000, 700);
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

            chartRevenues = new Chart
            {
                Location = new Point(10, 10),
                Size = new Size(930, 500),
                BackColor = Color.WhiteSmoke,
                BorderlineWidth = 1,
                BorderlineColor = Color.LightGray,
                BorderlineDashStyle = ChartDashStyle.Solid
            };
            
            // Configuration du graphique
            chartRevenues.Titles.Add(new Title("Évolution des revenus", Docking.Top, new Font("Arial", 14, FontStyle.Bold), Color.Black));
            chartRevenues.ChartAreas.Add(new ChartArea("Main"));
            chartRevenues.ChartAreas[0].AxisX.Title = "Mois";
            chartRevenues.ChartAreas[0].AxisY.Title = "Revenus (€)";
            chartRevenues.ChartAreas[0].BackColor = Color.White;
            chartRevenues.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            chartRevenues.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            
            tabRevenue.Controls.Add(chartRevenues);

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
            // Placeholder - à remplacer par le code réel
            DataTable dtStatsChauffeurs = new DataTable();
            dtStatsChauffeurs.Columns.Add("Nom", typeof(string));
            dtStatsChauffeurs.Columns.Add("Prénom", typeof(string));
            dtStatsChauffeurs.Columns.Add("Nombre de livraisons", typeof(int));
            dtStatsChauffeurs.Columns.Add("Distance totale (km)", typeof(int));
            dtStatsChauffeurs.Columns.Add("Durée totale (h)", typeof(double));
            dtStatsChauffeurs.Columns.Add("Revenus générés (€)", typeof(decimal));
            dtStatsChauffeurs.Columns.Add("Note moyenne", typeof(double));

            // Simuler quelques données
            dtStatsChauffeurs.Rows.Add("Romu", "David", 42, 12500, 156.5, 36750.50m, 4.8);
            dtStatsChauffeurs.Rows.Add("Romi", "Claire", 38, 10200, 133.0, 29580.25m, 4.9);
            dtStatsChauffeurs.Rows.Add("Roma", "Nicolas", 45, 13800, 172.5, 41400.75m, 4.7);

            dgvStatsChauffeurs.DataSource = dtStatsChauffeurs;
        }

        private void ChargerStatistiquesCommandes()
        {
            // Placeholder - à remplacer par le code réel
            DataTable dtStatsCommandes = new DataTable();
            dtStatsCommandes.Columns.Add("Période", typeof(string));
            dtStatsCommandes.Columns.Add("Nombre de commandes", typeof(int));
            dtStatsCommandes.Columns.Add("Prix moyen (€)", typeof(decimal));
            dtStatsCommandes.Columns.Add("Distance moyenne (km)", typeof(int));
            dtStatsCommandes.Columns.Add("Revenu total (€)", typeof(decimal));
            dtStatsCommandes.Columns.Add("Commandes en attente", typeof(int));
            dtStatsCommandes.Columns.Add("Commandes annulées", typeof(int));

            // Simuler quelques données
            DateTime debut = dtpDebut.Value;
            DateTime fin = dtpFin.Value;
            
            // Par mois
            for (DateTime date = new DateTime(debut.Year, debut.Month, 1); date <= fin; date = date.AddMonths(1))
            {
                if (date > fin) break;
                
                string periode = date.ToString("MMMM yyyy");
                int nbCommandes = new Random().Next(10, 50);
                decimal prixMoyen = new Random().Next(300, 800);
                int distanceMoyenne = new Random().Next(200, 600);
                decimal revenuTotal = nbCommandes * prixMoyen;
                int commandesEnAttente = new Random().Next(0, 5);
                int commandesAnnulees = new Random().Next(0, 3);
                
                dtStatsCommandes.Rows.Add(periode, nbCommandes, prixMoyen, distanceMoyenne, revenuTotal, commandesEnAttente, commandesAnnulees);
            }

            dgvStatsCommandes.DataSource = dtStatsCommandes;
        }

        private void ChargerStatistiquesClients()
        {
            // Placeholder - à remplacer par le code réel
            DataTable dtStatsClients = new DataTable();
            dtStatsClients.Columns.Add("Nom", typeof(string));
            dtStatsClients.Columns.Add("Prénom", typeof(string));
            dtStatsClients.Columns.Add("Nombre de commandes", typeof(int));
            dtStatsClients.Columns.Add("Montant total (€)", typeof(decimal));
            dtStatsClients.Columns.Add("Panier moyen (€)", typeof(decimal));
            dtStatsClients.Columns.Add("Dernière commande", typeof(DateTime));
            dtStatsClients.Columns.Add("Statut", typeof(string));

            // Simuler quelques données
            dtStatsClients.Rows.Add("Dupont", "Jean", 15, 8520.75m, 568.05m, DateTime.Now.AddDays(-3), "Client régulier");
            dtStatsClients.Rows.Add("Martin", "Sophie", 8, 4260.50m, 532.56m, DateTime.Now.AddDays(-12), "Client occasionnel");
            dtStatsClients.Rows.Add("Bernard", "Paul", 22, 12980.25m, 590.01m, DateTime.Now.AddDays(-1), "Client premium");
            dtStatsClients.Rows.Add("Leroy", "Marie", 5, 2870.30m, 574.06m, DateTime.Now.AddDays(-30), "Client inactif");

            dgvStatsClients.DataSource = dtStatsClients;
        }

        private void ChargerGraphiqueRevenues()
        {
            // Placeholder - à remplacer par le code réel
            chartRevenues.Series.Clear();
            
            // Créer les séries de données
            Series serieRevenues = new Series("Revenus mensuels");
            serieRevenues.ChartType = SeriesChartType.Column;
            serieRevenues.Color = Color.SteelBlue;
            
            Series serieTendance = new Series("Tendance");
            serieTendance.ChartType = SeriesChartType.Line;
            serieTendance.Color = Color.Firebrick;
            serieTendance.BorderWidth = 3;
            
            // Simuler des données
            DateTime debut = dtpDebut.Value;
            DateTime fin = dtpFin.Value;
            
            // Par mois
            for (DateTime date = new DateTime(debut.Year, debut.Month, 1); date <= fin; date = date.AddMonths(1))
            {
                if (date > fin) break;
                
                string periode = date.ToString("MMM yy");
                decimal revenu = new Random().Next(15000, 45000);
                
                serieRevenues.Points.AddXY(periode, revenu);
                
                // Ajouter à la tendance avec une légère croissance
                if (serieTendance.Points.Count > 0)
                {
                    double dernierPoint = serieTendance.Points[serieTendance.Points.Count - 1].YValues[0];
                    double tendance = dernierPoint * (1 + (new Random().Next(-5, 8) / 100.0));
                    serieTendance.Points.AddXY(periode, tendance);
                }
                else
                {
                    serieTendance.Points.AddXY(periode, revenu);
                }
            }
            
            chartRevenues.Series.Add(serieRevenues);
            chartRevenues.Series.Add(serieTendance);
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