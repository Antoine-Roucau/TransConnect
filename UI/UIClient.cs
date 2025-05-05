using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TransConnect.Data;
using TransConnect.Models;
using Transconnect.Services;

namespace TransConnect.UI
{
    public class UIClient : Form
    {
        private DataGridView dgvClients;
        private Button btnAjouter;
        private Button btnModifier;
        private Button btnSupprimer;
        private Button btnAfficherHistorique;
        private Button btnFermer;
        private ComboBox cmbTri;
        private TextBox txtRecherche;
        private Button btnRechercher;
        private Label lblTri;
        private Label lblRecherche;
        private Panel pnlCommandes;
        private DataInitializer dataInitializer;
        private ClientService clientService;
        private List<Client> clients;
        private DataTable dtClients;

        public UIClient(DataInitializer dataInitializer)
        {
            this.dataInitializer = dataInitializer;
            this.clientService = new ClientService();
            this.clients = dataInitializer.clients;
            
            InitializeComponents();
            ChargerClients();
        }

        private void InitializeComponents()
        {
            // Configuration du formulaire
            this.Text = "TransConnect - Gestion des Clients";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Labels
            lblTri = new Label
            {
                Text = "Trier par :",
                Location = new Point(20, 20),
                Size = new Size(70, 20)
            };
            this.Controls.Add(lblTri);

            lblRecherche = new Label
            {
                Text = "Rechercher :",
                Location = new Point(500, 20),
                Size = new Size(80, 20)
            };
            this.Controls.Add(lblRecherche);

            // Combobox pour le tri
            cmbTri = new ComboBox
            {
                Location = new Point(100, 20),
                Size = new Size(200, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTri.Items.AddRange(new object[] { "Nom", "Ville", "Montant des achats" });
            cmbTri.SelectedIndex = 0;
            cmbTri.SelectedIndexChanged += (s, e) => TrierClients();
            this.Controls.Add(cmbTri);

            // Champ de recherche
            txtRecherche = new TextBox
            {
                Location = new Point(590, 20),
                Size = new Size(200, 20)
            };
            this.Controls.Add(txtRecherche);

            btnRechercher = new Button
            {
                Text = "Rechercher",
                Location = new Point(800, 20),
                Size = new Size(100, 23)
            };
            btnRechercher.Click += (s, e) => RechercherClients();
            this.Controls.Add(btnRechercher);

            // DataGridView pour afficher les clients
            dgvClients = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(960, 400),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.WhiteSmoke
            };
            dgvClients.SelectionChanged += (s, e) => AfficherCommandesClient();
            this.Controls.Add(dgvClients);

            // Panel pour afficher les commandes du client
            pnlCommandes = new Panel
            {
                Location = new Point(20, 470),
                Size = new Size(960, 150),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke
            };
            this.Controls.Add(pnlCommandes);

            // Boutons d'action
            btnAjouter = new Button
            {
                Text = "Ajouter un client",
                Location = new Point(20, 630),
                Size = new Size(150, 30),
                BackColor = Color.LightGreen
            };
            btnAjouter.Click += (s, e) => AjouterClient();
            this.Controls.Add(btnAjouter);

            btnModifier = new Button
            {
                Text = "Modifier",
                Location = new Point(190, 630),
                Size = new Size(100, 30),
                BackColor = Color.LightBlue
            };
            btnModifier.Click += (s, e) => ModifierClient();
            this.Controls.Add(btnModifier);

            btnSupprimer = new Button
            {
                Text = "Supprimer",
                Location = new Point(310, 630),
                Size = new Size(100, 30),
                BackColor = Color.Salmon
            };
            btnSupprimer.Click += (s, e) => SupprimerClient();
            this.Controls.Add(btnSupprimer);

            btnAfficherHistorique = new Button
            {
                Text = "Historique complet",
                Location = new Point(430, 630),
                Size = new Size(150, 30),
                BackColor = Color.LightYellow
            };
            btnAfficherHistorique.Click += (s, e) => AfficherHistoriqueComplet();
            this.Controls.Add(btnAfficherHistorique);

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

        private void ChargerClients()
        {
            // Placeholder - à remplacer par le code réel
            dtClients = new DataTable();
            dtClients.Columns.Add("Numéro SS", typeof(string));
            dtClients.Columns.Add("Nom", typeof(string));
            dtClients.Columns.Add("Prénom", typeof(string));
            dtClients.Columns.Add("Téléphone", typeof(string));
            dtClients.Columns.Add("Email", typeof(string));
            dtClients.Columns.Add("Adresse", typeof(string));
            dtClients.Columns.Add("Montant Total", typeof(decimal));

            // Simuler quelques données
            dtClients.Rows.Add("123456789012345", "Dupont", "Jean", "0612345678", "jean.dupont@example.com", "Paris", 1250.50m);
            dtClients.Rows.Add("234567890123456", "Martin", "Sophie", "0723456789", "sophie.martin@example.com", "Lyon", 850.75m);
            dtClients.Rows.Add("345678901234567", "Bernard", "Paul", "0634567890", "paul.bernard@example.com", "Marseille", 3200.00m);

            dgvClients.DataSource = dtClients;
        }

        private void TrierClients()
        {
            // Placeholder pour le tri des clients
            if (cmbTri.SelectedIndex == 0) // Nom
            {
                dtClients.DefaultView.Sort = "Nom ASC";
            }
            else if (cmbTri.SelectedIndex == 1) // Ville
            {
                dtClients.DefaultView.Sort = "Adresse ASC";
            }
            else if (cmbTri.SelectedIndex == 2) // Montant
            {
                dtClients.DefaultView.Sort = "Montant Total DESC";
            }
        }

        private void RechercherClients()
        {
            // Placeholder pour la recherche
            string recherche = txtRecherche.Text.ToLower();
            if (string.IsNullOrWhiteSpace(recherche))
            {
                dtClients.DefaultView.RowFilter = "";
                return;
            }

            dtClients.DefaultView.RowFilter = $"Nom LIKE '%{recherche}%' OR Prénom LIKE '%{recherche}%' OR Adresse LIKE '%{recherche}%'";
        }

        private void AfficherCommandesClient()
        {
            // Placeholder pour afficher les commandes du client sélectionné
            if (dgvClients.SelectedRows.Count == 0) return;

            pnlCommandes.Controls.Clear();

            Label lblTitreCommandes = new Label
            {
                Text = "Dernières commandes",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(200, 20)
            };
            pnlCommandes.Controls.Add(lblTitreCommandes);

            // Placeholder: Afficher quelques commandes simulées
            DataGridView dgvCommandes = new DataGridView
            {
                Location = new Point(10, 35),
                Size = new Size(940, 105),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            DataTable dtCommandes = new DataTable();
            dtCommandes.Columns.Add("ID", typeof(int));
            dtCommandes.Columns.Add("Date", typeof(DateTime));
            dtCommandes.Columns.Add("Départ", typeof(string));
            dtCommandes.Columns.Add("Arrivée", typeof(string));
            dtCommandes.Columns.Add("Prix", typeof(decimal));
            dtCommandes.Columns.Add("Statut", typeof(string));

            // Simuler quelques commandes
            dtCommandes.Rows.Add(1, DateTime.Now.AddDays(-5), "Paris", "Lyon", 450.50m, "Payée");
            dtCommandes.Rows.Add(2, DateTime.Now.AddDays(-2), "Lyon", "Marseille", 320.75m, "En cours");
            dtCommandes.Rows.Add(3, DateTime.Now, "Marseille", "Nice", 230.00m, "En attente");

            dgvCommandes.DataSource = dtCommandes;
            pnlCommandes.Controls.Add(dgvCommandes);
        }

        private void AjouterClient()
        {
            // Placeholder pour ajouter un client
            MessageBox.Show("Fonctionnalité à implémenter: Ajouter un client", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ModifierClient()
        {
            // Placeholder pour modifier un client
            if (dgvClients.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner un client à modifier", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Fonctionnalité à implémenter: Modifier un client", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SupprimerClient()
        {
            // Placeholder pour supprimer un client
            if (dgvClients.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner un client à supprimer", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer ce client?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Code de suppression
                MessageBox.Show("Client supprimé avec succès!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void AfficherHistoriqueComplet()
        {
            // Placeholder pour afficher l'historique complet du client
            if (dgvClients.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner un client", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Fonctionnalité à implémenter: Afficher l'historique complet", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}