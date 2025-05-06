using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TransConnect.Data;
using TransConnect.Models;
using TransConnect.Models.Graphe;

namespace TransConnect.UI
{
    public class UICommande : Form
    {
        private TabControl tabCommandes;
        private TabPage tabListeCommandes;
        private TabPage tabNouvelleCommande;
        private TabPage tabItineraire;
        
        private DataGridView dgvCommandes;
        private Button btnAjouter;
        private Button btnModifier;
        private Button btnAnnuler;
        private Button btnPayer;
        private Button btnItineraire;
        private Button btnFermer;
        
        private ComboBox cmbClient;
        private ComboBox cmbVilleDepart;
        private ComboBox cmbVilleArrivee;
        private DateTimePicker dtpDate;
        private ComboBox cmbVehicule;
        private ComboBox cmbChauffeur;
        private Button btnCalculerItineraire;
        private Button btnEnregistrer;
        private Label lblPrix;
        private Panel pnlItineraire;
        
        private DataInitializer dataInitializer;
        private List<Commande> commandes = new List<Commande>();

        public UICommande(DataInitializer dataInitializer)
        {
            this.dataInitializer = dataInitializer;
            this.commandes = dataInitializer.commandes;
            InitializeComponents();
            ChargerCommandes();
        }

        private void InitializeComponents()
        {
            // Configuration du formulaire
            this.Text = "TransConnect - Gestion des Commandes";
            this.Size = new Size(1500, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // TabControl
            tabCommandes = new TabControl
            {
                Location = new Point(20, 20),
                Size = new Size(1460, 600),
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(tabCommandes);

            // Tab Liste des commandes
            tabListeCommandes = new TabPage
            {
                Text = "Liste des commandes",
                BackColor = Color.White
            };
            tabCommandes.TabPages.Add(tabListeCommandes);

            // DataGridView pour afficher les commandes
            dgvCommandes = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(1430, 500),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.WhiteSmoke
            };
            tabListeCommandes.Controls.Add(dgvCommandes);

            // Boutons d'action pour les commandes
            btnAjouter = new Button
            {
                Text = "Nouvelle commande",
                Location = new Point(10, 520),
                Size = new Size(150, 30),
                BackColor = Color.LightGreen
            };
            btnAjouter.Click += (s, e) => tabCommandes.SelectedTab = tabNouvelleCommande;
            tabListeCommandes.Controls.Add(btnAjouter);

            btnModifier = new Button
            {
                Text = "Modifier",
                Location = new Point(170, 520),
                Size = new Size(100, 30),
                BackColor = Color.LightBlue
            };
            btnModifier.Click += (s, e) => ModifierCommande();
            tabListeCommandes.Controls.Add(btnModifier);

            btnAnnuler = new Button
            {
                Text = "Annuler",
                Location = new Point(280, 520),
                Size = new Size(100, 30),
                BackColor = Color.Salmon
            };
            btnAnnuler.Click += (s, e) => AnnulerCommande();
            tabListeCommandes.Controls.Add(btnAnnuler);

            btnPayer = new Button
            {
                Text = "Marquer comme payée",
                Location = new Point(390, 520),
                Size = new Size(150, 30),
                BackColor = Color.LightYellow
            };
            btnPayer.Click += (s, e) => PayerCommande();
            tabListeCommandes.Controls.Add(btnPayer);

            btnItineraire = new Button
            {
                Text = "Voir itinéraire",
                Location = new Point(550, 520),
                Size = new Size(120, 30),
                BackColor = Color.Lavender
            };
            btnItineraire.Click += (s, e) => AfficherItineraire();
            tabListeCommandes.Controls.Add(btnItineraire);

            // Tab Nouvelle commande
            tabNouvelleCommande = new TabPage
            {
                Text = "Nouvelle commande",
                BackColor = Color.White
            };
            tabCommandes.TabPages.Add(tabNouvelleCommande);

            // Contrôles pour la nouvelle commande
            TableLayoutPanel tableCommande = new TableLayoutPanel
            {
                Location = new Point(10, 10),
                Size = new Size(930, 300),
                ColumnCount = 2,
                RowCount = 7
            };
            tableCommande.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableCommande.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            tabNouvelleCommande.Controls.Add(tableCommande);

            // Client
            tableCommande.Controls.Add(new Label { Text = "Client :", Font = new Font("Arial", 10, FontStyle.Bold) }, 0, 0);
            cmbClient = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 300
            };
            cmbClient.Items.AddRange(new object[] { "Dupont Jean", "Martin Sophie", "Bernard Paul" });
            tableCommande.Controls.Add(cmbClient, 1, 0);

            // Ville de départ
            tableCommande.Controls.Add(new Label { Text = "Ville de départ :", Font = new Font("Arial", 10, FontStyle.Bold) }, 0, 1);
            cmbVilleDepart = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 300
            };
            cmbVilleDepart.Items.AddRange(new object[] { "Paris", "Lyon", "Marseille", "Bordeaux", "Lille", "Strasbourg", "Nantes", "Toulouse", "Nice" });
            tableCommande.Controls.Add(cmbVilleDepart, 1, 1);

            // Ville d'arrivée
            tableCommande.Controls.Add(new Label { Text = "Ville d'arrivée :", Font = new Font("Arial", 10, FontStyle.Bold) }, 0, 2);
            cmbVilleArrivee = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 300
            };
            cmbVilleArrivee.Items.AddRange(new object[] { "Paris", "Lyon", "Marseille", "Bordeaux", "Lille", "Strasbourg", "Nantes", "Toulouse", "Nice" });
            tableCommande.Controls.Add(cmbVilleArrivee, 1, 2);

            // Date
            tableCommande.Controls.Add(new Label { Text = "Date :", Font = new Font("Arial", 10, FontStyle.Bold) }, 0, 3);
            dtpDate = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Width = 150,
                MinDate = DateTime.Now
            };
            tableCommande.Controls.Add(dtpDate, 1, 3);

            // Véhicule
            tableCommande.Controls.Add(new Label { Text = "Véhicule :", Font = new Font("Arial", 10, FontStyle.Bold) }, 0, 4);
            cmbVehicule = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 300
            };
            cmbVehicule.Items.AddRange(new object[] { "Camion-citerne (Renault T520)", "Camion frigorifique (Mercedes Actros)", "Camionnette (Fiat Ducato)" });
            tableCommande.Controls.Add(cmbVehicule, 1, 4);

            // Chauffeur
            tableCommande.Controls.Add(new Label { Text = "Chauffeur :", Font = new Font("Arial", 10, FontStyle.Bold) }, 0, 5);
            cmbChauffeur = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 300
            };
            cmbChauffeur.Items.AddRange(new object[] { "Romu David", "Romi Claire", "Roma Nicolas" });
            tableCommande.Controls.Add(cmbChauffeur, 1, 5);

            // Prix estimé
            tableCommande.Controls.Add(new Label { Text = "Prix estimé :", Font = new Font("Arial", 10, FontStyle.Bold) }, 0, 6);
            lblPrix = new Label
            {
                Text = "- €",
                Font = new Font("Arial", 10)
            };
            tableCommande.Controls.Add(lblPrix, 1, 6);

            // Bouton de calcul d'itinéraire
            btnCalculerItineraire = new Button
            {
                Text = "Calculer l'itinéraire",
                Location = new Point(10, 320),
                Size = new Size(150, 30),
                BackColor = Color.LightBlue
            };
            btnCalculerItineraire.Click += (s, e) => CalculerItineraire();
            tabNouvelleCommande.Controls.Add(btnCalculerItineraire);

            // Bouton d'enregistrement
            btnEnregistrer = new Button
            {
                Text = "Enregistrer la commande",
                Location = new Point(170, 320),
                Size = new Size(180, 30),
                BackColor = Color.LightGreen
            };
            btnEnregistrer.Click += (s, e) => EnregistrerCommande();
            tabNouvelleCommande.Controls.Add(btnEnregistrer);

            // Panel pour l'itinéraire
            pnlItineraire = new Panel
            {
                Location = new Point(10, 360),
                Size = new Size(930, 200),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke
            };
            tabNouvelleCommande.Controls.Add(pnlItineraire);

            // Tab Itinéraire
            tabItineraire = new TabPage
            {
                Text = "Visualisation des itinéraires",
                BackColor = Color.White
            };
            tabCommandes.TabPages.Add(tabItineraire);

            // Placeholder pour la visualisation des itinéraires
            Label lblPlaceholder = new Label
            {
                Text = "Cette page permettra de visualiser les itinéraires sur une carte.",
                Font = new Font("Arial", 12),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 100),
                Size = new Size(930, 30)
            };
            tabItineraire.Controls.Add(lblPlaceholder);

            Button btnAfficherCarte = new Button
            {
                Text = "Afficher la carte des villes",
                Location = new Point(350, 150),
                Size = new Size(200, 30),
                BackColor = Color.LightBlue
            };
            btnAfficherCarte.Click += (s, e) => AfficherCarteVilles();
            tabItineraire.Controls.Add(btnAfficherCarte);

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

        private void ChargerCommandes()
        {
            // Placeholder - à remplacer par le code réel
            DataTable dtCommandes = new DataTable();
            dtCommandes.Columns.Add("ID", typeof(int));
            dtCommandes.Columns.Add("Client", typeof(string));
            dtCommandes.Columns.Add("Date", typeof(DateTime));
            dtCommandes.Columns.Add("Départ", typeof(string));
            dtCommandes.Columns.Add("Arrivée", typeof(string));
            dtCommandes.Columns.Add("Prix", typeof(decimal));
            dtCommandes.Columns.Add("Statut", typeof(string));
            dtCommandes.Columns.Add("Chauffeur", typeof(string));
            dtCommandes.Columns.Add("Véhicule", typeof(string));

            foreach (var commande in commandes)
            {
                dtCommandes.Rows.Add(commande.Id, commande.Client.Nom, commande.Date, commande.VilleDepart,commande.VilleArrivee,commande.Prix, commande.Statut.ToString(),commande.Chauffeur.Nom,commande.Vehicule.Type.ToString());
            }
            dgvCommandes.DataSource = dtCommandes;
        }

        private void ModifierCommande()
        {
            // Placeholder pour modifier une commande
            if (dgvCommandes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner une commande à modifier", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Fonctionnalité à implémenter: Modifier une commande", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AnnulerCommande()
        {
            // Placeholder pour annuler une commande
            if (dgvCommandes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner une commande à annuler", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Êtes-vous sûr de vouloir annuler cette commande?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Code d'annulation
                MessageBox.Show("Commande annulée avec succès!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void PayerCommande()
        {
            if(dgvCommandes.SelectedRows.Count != 0)
            {
                //Appel CommandeSercice pour payer la commande
            }
        }

        private void AfficherItineraire()
        {
            // Placeholder pour afficher l'itinéraire d'une commande
            if (dgvCommandes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner une commande pour voir son itinéraire", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            tabCommandes.SelectedTab = tabItineraire;
        }

        private void CalculerItineraire()
        {
            // Placeholder pour calculer l'itinéraire d'une nouvelle commande
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

            pnlItineraire.Controls.Clear();

            // Simulation de calcul d'itinéraire
            string villeDepart = cmbVilleDepart.SelectedItem.ToString();
            string villeArrivee = cmbVilleArrivee.SelectedItem.ToString();
            
            Label lblItineraire = new Label
            {
                Text = "Itinéraire calculé avec l'algorithme de Dijkstra",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(500, 20)
            };
            pnlItineraire.Controls.Add(lblItineraire);

            // Distance simulée
            int distance = new Random().Next(200, 800);
            
            // Afficher l'itinéraire
            Label lblTrajet = new Label
            {
                Text = $"Trajet: {villeDepart} → {villeArrivee}",
                Font = new Font("Arial", 10),
                Location = new Point(10, 40),
                Size = new Size(500, 20)
            };
            pnlItineraire.Controls.Add(lblTrajet);
            
            Label lblDistance = new Label
            {
                Text = $"Distance: {distance} km",
                Font = new Font("Arial", 10),
                Location = new Point(10, 70),
                Size = new Size(200, 20)
            };
            pnlItineraire.Controls.Add(lblDistance);
            
            // Simuler un prix basé sur la distance et le type de véhicule
            decimal prixKm = 0;
            if (cmbVehicule.SelectedIndex != -1)
            {
                if (cmbVehicule.SelectedItem.ToString().Contains("Camion-citerne"))
                    prixKm = 2.95m;
                else if (cmbVehicule.SelectedItem.ToString().Contains("Camion frigorifique"))
                    prixKm = 3.25m;
                else if (cmbVehicule.SelectedItem.ToString().Contains("Camionnette"))
                    prixKm = 1.95m;
                else
                    prixKm = 2.5m;
            }
            else
            {
                prixKm = 2.5m;
            }
            
            decimal prix = distance * prixKm;
            lblPrix.Text = $"{prix:C}";
            
            Label lblInfosPrix = new Label
            {
                Text = $"Tarif kilométrique: {prixKm:C}/km",
                Font = new Font("Arial", 10),
                Location = new Point(10, 100),
                Size = new Size(200, 20)
            };
            pnlItineraire.Controls.Add(lblInfosPrix);
            
            // Afficher un chemin simulé
            Label lblChemin = new Label
            {
                Text = "Chemin: ",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, 130),
                Size = new Size(100, 20)
            };
            pnlItineraire.Controls.Add(lblChemin);
            
            // Simuler un chemin
            string[] villes = { "Paris", "Lyon", "Marseille", "Bordeaux", "Lille", "Strasbourg", "Nantes", "Toulouse", "Nice" };
            List<string> etapes = new List<string>();
            etapes.Add(villeDepart);
            
            // Ajouter des étapes aléatoires si la distance est grande
            if (distance > 400)
            {
                int nbEtapes = new Random().Next(1, 3);
                for (int i = 0; i < nbEtapes; i++)
                {
                    string etape = villes[new Random().Next(villes.Length)];
                    if (etape != villeDepart && etape != villeArrivee && !etapes.Contains(etape))
                        etapes.Add(etape);
                }
            }
            
            etapes.Add(villeArrivee);
            
            Label lblEtapes = new Label
            {
                Text = string.Join(" → ", etapes),
                Font = new Font("Arial", 10),
                Location = new Point(80, 130),
                Size = new Size(500, 20)
            };
            pnlItineraire.Controls.Add(lblEtapes);
        }

        private void EnregistrerCommande()
        {
            // Placeholder pour enregistrer une nouvelle commande
            if (cmbClient.SelectedIndex == -1 || cmbVilleDepart.SelectedIndex == -1 || cmbVilleArrivee.SelectedIndex == -1 ||
                cmbVehicule.SelectedIndex == -1 || cmbChauffeur.SelectedIndex == -1)
            {
                MessageBox.Show("Veuillez remplir tous les champs requis", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Vérifier que le prix a été calculé
            if (lblPrix.Text == "- €")
            {
                MessageBox.Show("Veuillez calculer l'itinéraire pour obtenir le prix", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Commande enregistrée avec succès!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            // Réinitialiser le formulaire et revenir à la liste des commandes
            cmbClient.SelectedIndex = -1;
            cmbVilleDepart.SelectedIndex = -1;
            cmbVilleArrivee.SelectedIndex = -1;
            cmbVehicule.SelectedIndex = -1;
            cmbChauffeur.SelectedIndex = -1;
            lblPrix.Text = "- €";
            pnlItineraire.Controls.Clear();
            
            tabCommandes.SelectedTab = tabListeCommandes;
            ChargerCommandes(); // Recharger les commandes pour voir la nouvelle
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
    }
}