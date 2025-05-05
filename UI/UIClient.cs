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
        private List<Commande> commandes; // A rajouter
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
            this.Size = new Size(1500, 700);
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
            cmbTri.Items.AddRange(new object[] { "Nom", "Ville", "Montant des achats","Date de Naissance" });
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
                Size = new Size(1460, 400),
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
                Size = new Size(1460, 150),
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

            btnFermer = new Button
            {
                Text = "Fermer",
                Location = new Point(1380, 630),
                Size = new Size(100, 30),
                BackColor = Color.LightGray
            };
            btnFermer.Click += (s, e) => this.Close();
            this.Controls.Add(btnFermer);
        }

        private void ChargerClients()
        {
            dtClients = new DataTable();
            dtClients.Columns.Add("NumeroSS", typeof(string));
            dtClients.Columns.Add("Nom", typeof(string));
            dtClients.Columns.Add("Prenom", typeof(string));
            dtClients.Columns.Add("Date_de_Naissance", typeof(DateTime));
            dtClients.Columns.Add("Telephone", typeof(string));
            dtClients.Columns.Add("Adresse_Mail", typeof(string));
            dtClients.Columns.Add("Adresse_Postale", typeof(string));
            dtClients.Columns.Add("Montant_Total_Achats", typeof(decimal));

            foreach (var client in clients)
            {
                dtClients.Rows.Add(client.NumeroSS, client.Nom, client.Prenom, client.DateNaissance, client.Telephone, client.AdresseMail, client.AdressePostale, client.MontantTotalAchats);
            }
            dgvClients.DataSource = dtClients;
        }

        private void TrierClients()
        {
            if (cmbTri.SelectedIndex == 0) // Nom
            {
                dtClients = clientService.TrierClientsParNom(dtClients);
            }
            else if (cmbTri.SelectedIndex == 1) // Ville
            {
                dtClients = clientService.TrierClientsParVille(dtClients);
            }
            else if (cmbTri.SelectedIndex == 2) // Montant
            {
                dtClients = clientService.TrierClientsParMontant(dtClients);
            }
            else if (cmbTri.SelectedIndex == 3) // Date de Naissance
            {
                dtClients = clientService.TrierClientsParDateNaissance(dtClients);
            }
            
            // Mettre à jour le DataGridView avec les données triées
            dgvClients.DataSource = dtClients;
        }

        private void RechercherClients()
        {
            dtClients = clientService.TrierClientsParNom(dtClients);
            string recherche = txtRecherche.Text.ToLower();
            if (string.IsNullOrWhiteSpace(recherche))
            {
                ChargerClients();
                return;
            }

            dtClients.DefaultView.RowFilter = $"NumeroSS LIKE '%{recherche}%' OR Nom LIKE '%{recherche}%' OR Prenom LIKE '%{recherche}%' OR Adresse_Postale LIKE '%{recherche}%' OR Adresse_Mail LIKE '%{recherche}%' OR Telephone LIKE '%{recherche}%'";
            dgvClients.DataSource = dtClients;
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

            DataGridView dgvCommandes = new DataGridView
            {
                Location = new Point(10, 35),
                Size = new Size(1440, 105),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            DataTable dtCommandes = new DataTable();
            dtCommandes.Columns.Add("ID", typeof(int));
            dtCommandes.Columns.Add("villeDepart", typeof(string));
            dtCommandes.Columns.Add("villeArrivee", typeof(string));
            dtCommandes.Columns.Add("date", typeof(DateTime));
            dtCommandes.Columns.Add("prix", typeof(decimal));
            dtCommandes.Columns.Add("Statut", typeof(string));


            dgvCommandes.DataSource = dtCommandes;
            pnlCommandes.Controls.Add(dgvCommandes);
        }

        private void AjouterClient()
        {
            UIAddClient uiAddClient = new UIAddClient(dataInitializer);
            uiAddClient.ShowDialog();
            Client nouveauClient = uiAddClient.AjouterClient();
            if (nouveauClient == null) return; // Si l'utilisateur a annulé l'ajout
            clientService.AjouterClient(nouveauClient, clients);
            ChargerClients();
        }

        private void ModifierClient()
        {
            Client clientAModifier = clients[dgvClients.SelectedRows[0].Index];
            UIModifClient uiModifClient = new UIModifClient(dataInitializer,clientAModifier);
            uiModifClient.ShowDialog();
            Client clientModifie = uiModifClient.ModifierClient();
            if (clientModifie == null) return; // Si l'utilisateur a annulé la modification
            clientService.ModifierClient(clientAModifier, clientModifie, clients);
            ChargerClients();

        }

        private void SupprimerClient()
        {
            clientService.SupprimerClient(clients[dgvClients.SelectedRows[0].Index], clients);
            ChargerClients();
        }

    }

    public class UIAddClient : Form
    {
        private TextBox txtNumeroSS;
        private TextBox txtNom;
        private TextBox txtPrenom;
        private TextBox txtDateNaissance;
        private TextBox txtAdressePostale;
        private TextBox txtAdresseMail;
        private TextBox txtTelephone;
        private Button btnAjouterClient;
        private Button btnAnnuler;
        private DataInitializer dataInitializer;

        public UIAddClient(DataInitializer dataInitializer)
        {
            this.dataInitializer = dataInitializer;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Configuration du formulaire
            this.Text = "Ajouter un Client";
            this.Size = new Size(400, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Champs de saisie
            Label numeroSSLabel = new Label { Text = "Numéro de Sécurité Sociale:", Location = new Point(20, 20) };
            txtNumeroSS = new TextBox { Location = new Point(150, 20), Width = 200 };
            this.Controls.Add(numeroSSLabel);
            this.Controls.Add(txtNumeroSS);

            Label lblNom = new Label { Text = "Nom:", Location = new Point(20, 60) };
            txtNom = new TextBox { Location = new Point(150, 60), Width = 200 };
            this.Controls.Add(lblNom);
            this.Controls.Add(txtNom);

            Label lblPrenom = new Label { Text = "Prénom:", Location = new Point(20, 100) };
            txtPrenom = new TextBox { Location = new Point(150, 100), Width = 200 };
            this.Controls.Add(lblPrenom);
            this.Controls.Add(txtPrenom);
            

            Label lblDateNaissance = new Label { Text = "Date de Naissance:", Location = new Point(20, 140) };
            txtDateNaissance = new TextBox { Location = new Point(150, 140), Width = 200 };
            this.Controls.Add(lblDateNaissance);
            this.Controls.Add(txtDateNaissance);

            Label lblAdressePostale = new Label { Text = "Adresse Postale:", Location = new Point(20, 180) };
            txtAdressePostale = new TextBox { Location = new Point(150, 180), Width = 200 };
            this.Controls.Add(lblAdressePostale);
            this.Controls.Add(txtAdressePostale);

            Label lblAdresseMail = new Label { Text = "Adresse Email:", Location = new Point(20, 220) };
            txtAdresseMail = new TextBox { Location = new Point(150, 220), Width = 200 };
            this.Controls.Add(lblAdresseMail);
            this.Controls.Add(txtAdresseMail);

            Label lblTelephone = new Label { Text = "Telephone :", Location = new Point(20, 260) };
            txtTelephone = new TextBox { Location = new Point(150, 260), Width = 200 };
            this.Controls.Add(lblTelephone);
            this.Controls.Add(txtTelephone);

            // Boutons
            btnAjouterClient = new Button
            {
                Text = "Ajouter Client",
                Location = new Point(80, 300),
                Size = new Size(120,30),
                BackColor = Color.LightGreen
            };
            btnAjouterClient.Click += (s, e) => AjouterClient();
            this.Controls.Add(btnAjouterClient);
            
            btnAnnuler = new Button
            {
                Text = "Annuler",
                Location = new Point(220, 300),
                Size = new Size(120,30),
                BackColor = Color.LightCoral
            };
            btnAnnuler.Click += (s, e) => this.Close();
            this.Controls.Add(btnAnnuler);
        }

        public Client AjouterClient()
        {
            if (string.IsNullOrWhiteSpace(txtNumeroSS.Text) || string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtPrenom.Text))
            {
                return null;
            }
            string numeroSS = txtNumeroSS.Text;
            string nom = txtNom.Text;
            string prenom = txtPrenom.Text;
            DateTime dateNaissance = DateTime.Now;
            if (txtDateNaissance.Text !="")
            { 
                dateNaissance = DateTime.Parse(txtDateNaissance.Text);
            }
            string adressePostale = txtAdressePostale.Text;
            string adresseMail = txtAdresseMail.Text;
            string telephone = txtTelephone.Text;

            Client client = new Client(numeroSS,nom, prenom, dateNaissance, adressePostale, adresseMail, telephone);
            
            this.Close();
            return client;
        }
    
    }

    public class UIModifClient : Form 
    {
        private TextBox txtNumeroSS;
        private TextBox txtNom;
        private TextBox txtPrenom;
        private TextBox txtDateNaissance;
        private TextBox txtAdressePostale;
        private TextBox txtAdresseMail;
        private TextBox txtTelephone;
        private Button btnModifierClient;
        private Button btnAnnuler;
        private DataInitializer dataInitializer;

        public UIModifClient(DataInitializer dataInitializer, Client clientAModifier)
        {
            this.dataInitializer = dataInitializer;
            InitializeComponents(clientAModifier);
        }

        private void InitializeComponents(Client clientAModifier)
        {
        
            // Configuration du formulaire
            this.Text = "Modifier un Client";
            this.Size = new Size(400, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Champs de saisie
            Label numeroSSLabel = new Label { Text = "Numéro de Sécurité Sociale:", Location = new Point(20, 20) };
            txtNumeroSS = new TextBox { Location = new Point(150, 20), Width = 200 };
            this.Controls.Add(numeroSSLabel);
            txtNumeroSS.Text = clientAModifier.NumeroSS;
            this.Controls.Add(txtNumeroSS);

            Label lblNom = new Label { Text = "Nom:", Location = new Point(20, 60) };
            txtNom = new TextBox { Location = new Point(150, 60), Width = 200 };
            this.Controls.Add(lblNom);
            txtNom.Text = clientAModifier.Nom;
            this.Controls.Add(txtNom);

            Label lblPrenom = new Label { Text = "Prénom:", Location = new Point(20, 100) };
            txtPrenom = new TextBox { Location = new Point(150, 100), Width = 200 };
            this.Controls.Add(lblPrenom);
            txtPrenom.Text = clientAModifier.Prenom;
            this.Controls.Add(txtPrenom);


            Label lblDateNaissance = new Label { Text = "Date de Naissance:", Location = new Point(20, 140) };
            txtDateNaissance = new TextBox { Location = new Point(150, 140), Width = 200 };
            this.Controls.Add(lblDateNaissance);
            txtDateNaissance.Text = clientAModifier.DateNaissance.ToString("yyyy-MM-dd");
            this.Controls.Add(txtDateNaissance);

            Label lblAdressePostale = new Label { Text = "Adresse Postale:", Location = new Point(20, 180) };
            txtAdressePostale = new TextBox { Location = new Point(150, 180), Width = 200 };
            this.Controls.Add(lblAdressePostale);
            txtAdressePostale.Text = clientAModifier.AdressePostale;
            this.Controls.Add(txtAdressePostale);

            Label lblAdresseMail = new Label { Text = "Adresse Email:", Location = new Point(20, 220) };
            txtAdresseMail = new TextBox { Location = new Point(150, 220), Width = 200 };
            this.Controls.Add(lblAdresseMail);
            txtAdresseMail.Text = clientAModifier.AdresseMail;
            this.Controls.Add(txtAdresseMail);

            Label lblTelephone = new Label { Text = "Telephone :", Location = new Point(20, 260) };
            txtTelephone = new TextBox { Location = new Point(150, 260), Width = 200 };
            this.Controls.Add(lblTelephone);
            txtTelephone.Text = clientAModifier.Telephone;
            this.Controls.Add(txtTelephone);

            // Boutons
            btnModifierClient = new Button
            {
                Text = "Modifier Client",
                Location = new Point(80, 300),
                Size = new Size(120,30),
                BackColor = Color.LightGreen
            };
            btnModifierClient.Click += (s, e) => ModifierClient();
            this.Controls.Add(btnModifierClient);
            
            btnAnnuler = new Button
            {
                Text = "Annuler",
                Location = new Point(220, 300),
                Size = new Size(120,30),
                BackColor = Color.LightCoral
            };
            btnAnnuler.Click += (s, e) => this.Close();
            this.Controls.Add(btnAnnuler);

        }

        public Client ModifierClient()
        {
            if (string.IsNullOrWhiteSpace(txtNumeroSS.Text) || string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtPrenom.Text))
            {
                return null;
            }
            string numeroSS = txtNumeroSS.Text;
            string nom = txtNom.Text;
            string prenom = txtPrenom.Text;
            DateTime dateNaissance = DateTime.Now;
            if (txtDateNaissance.Text !="")
            { 
                dateNaissance = DateTime.Parse(txtDateNaissance.Text);
            }
            string adressePostale = txtAdressePostale.Text;
            string adresseMail = txtAdresseMail.Text;
            string telephone = txtTelephone.Text;

            Client client = new Client(numeroSS,nom, prenom, dateNaissance, adressePostale, adresseMail, telephone);
            
            this.Close();
            return client;
        }
    }
}