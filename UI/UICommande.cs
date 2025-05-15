using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Transconnect.Data;
using Transconnect.Models;
using Transconnect.Models.Graphe;
using Transconnect.Services;
using Transconnect.Algorithms.CalculDistance;
using Transconnect.Algorithms.PlusCourtChemin;

namespace Transconnect.UI
{
    public class UICommande : Form
    {
        #region Propriétés
        private TabControl tabCommandes;
        private TabPage tabListeCommandes;
        private TabPage tabNouvelleCommande;
        private TabPage tabItineraire;

        private DataGridView dgvCommandes;
        private Button btnAjouter;
        private Button btnModifier;
        private Button btnAnnuler;
        private Button btnSupprimer;
        private Button btnPayer;
        private Button btnLivrer;
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
        private CommandeService commandeService;
        private List<Commande> commandes = new List<Commande>();
        #endregion

        #region Constructeur
        public UICommande(DataInitializer dataInitializer)
        {
            this.dataInitializer = dataInitializer;
            this.commandes = dataInitializer.commandes;
            this.commandeService = new CommandeService(this.commandes);
            InitializeComponents();
            ChargerCommandes();
        }

        private void InitializeComponents()
        {
            this.Text = "TransConnect - Gestion des Commandes";
            this.Size = new Size(1500, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            tabCommandes = new TabControl
            {
                Location = new Point(20, 20),
                Size = new Size(1460, 600),
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(tabCommandes);

            tabListeCommandes = new TabPage
            {
                Text = "Liste des commandes",
                BackColor = Color.White
            };
            tabCommandes.TabPages.Add(tabListeCommandes);

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
                Text = "Marquer Payée",
                Location = new Point(390, 520),
                Size = new Size(150, 30),
                BackColor = Color.LightYellow
            };
            btnPayer.Click += (s, e) => PayerCommande();
            tabListeCommandes.Controls.Add(btnPayer);

            btnLivrer = new Button
            {
                Text = "Marquer Livrée",
                Location = new Point(550, 520),
                Size = new Size(150, 30),
                BackColor = Color.LightYellow
            };
            btnLivrer.Click += (s, e) => LivrerCommande();
            tabListeCommandes.Controls.Add(btnLivrer);
            
            btnSupprimer = new Button
            {
                Text = "Supprimer",
                Location = new Point(710, 520),
                Size = new Size(120, 30),
                BackColor = Color.Salmon
            };
            btnSupprimer.Click += (s, e) => SupprimerCommande();
            tabListeCommandes.Controls.Add(btnSupprimer);

            btnFermer = new Button
            {
                Text = "Fermer",
                Location = new Point(1350, 625),
                Size = new Size(120, 30),
                BackColor = Color.LightGray
            };
            btnFermer.Click += (s, e) => this.Close();
            this.Controls.Add(btnFermer);

            tabNouvelleCommande = new TabPage
            {
                Text = "Nouvelle commande",
                BackColor = Color.White
            };
            tabCommandes.TabPages.Add(tabNouvelleCommande);

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

            tableCommande.Controls.Add(new Label { Text = "Client :", Font = new Font("Arial", 10, FontStyle.Bold) }, 0, 0);
            cmbClient = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 300
            };
            foreach (Client client in dataInitializer.clients)
            {
                cmbClient.Items.Add($"{client.Nom} - {client.Prenom}");
            }
            tableCommande.Controls.Add(cmbClient, 1, 0);

            tableCommande.Controls.Add(new Label { Text = "Ville de départ :", Font = new Font("Arial", 10, FontStyle.Bold) }, 0, 1);
            cmbVilleDepart = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 300
            };
            foreach (var ville in dataInitializer.villes)
            {
                cmbVilleDepart.Items.Add(ville);
            }
            tableCommande.Controls.Add(cmbVilleDepart, 1, 1);

            tableCommande.Controls.Add(new Label { Text = "Ville d'arrivée :", Font = new Font("Arial", 10, FontStyle.Bold) }, 0, 2);
            cmbVilleArrivee = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 300
            };
            foreach (var ville in dataInitializer.villes)
            {
                cmbVilleArrivee.Items.Add(ville);
            }
            tableCommande.Controls.Add(cmbVilleArrivee, 1, 2);

            tableCommande.Controls.Add(new Label { Text = "Date :", Font = new Font("Arial", 10, FontStyle.Bold) }, 0, 3);
            dtpDate = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Width = 150,
                MinDate = DateTime.Now
            };
            tableCommande.Controls.Add(dtpDate, 1, 3);

            tableCommande.Controls.Add(new Label { Text = "Véhicule :", Font = new Font("Arial", 10, FontStyle.Bold) }, 0, 4);
            cmbVehicule = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 300
            };
            foreach (Vehicule vehicule in dataInitializer.vehicules)
            {
                if (vehicule.EstDisponible)
                {
                    cmbVehicule.Items.Add($"{vehicule.Immatriculation} / {vehicule.Type}");
                }
            }
            tableCommande.Controls.Add(cmbVehicule, 1, 4);

            tableCommande.Controls.Add(new Label { Text = "Chauffeur :", Font = new Font("Arial", 10, FontStyle.Bold) }, 0, 5);
            cmbChauffeur = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 300
            };
            foreach (Salarie chauffeur in dataInitializer.salaries)
            {
                if (chauffeur.Poste == "Chauffeur")
                {
                    cmbChauffeur.Items.Add(chauffeur.Nom);
                }
            }
            tableCommande.Controls.Add(cmbChauffeur, 1, 5);

            tableCommande.Controls.Add(new Label { Text = "Prix estimé :", Font = new Font("Arial", 10, FontStyle.Bold) }, 0, 6);
            lblPrix = new Label
            {
                Text = "- €",
                Font = new Font("Arial", 10)
            };
            tableCommande.Controls.Add(lblPrix, 1, 6);

            btnCalculerItineraire = new Button
            {
                Text = "Calculer l'itinéraire",
                Location = new Point(10, 320),
                Size = new Size(150, 30),
                BackColor = Color.LightBlue
            };
            btnCalculerItineraire.Click += (s, e) => CalculerItineraire();
            tabNouvelleCommande.Controls.Add(btnCalculerItineraire);

            btnEnregistrer = new Button
            {
                Text = "Enregistrer la commande",
                Location = new Point(170, 320),
                Size = new Size(180, 30),
                BackColor = Color.LightGreen
            };
            btnEnregistrer.Click += (s, e) => EnregistrerCommande();
            tabNouvelleCommande.Controls.Add(btnEnregistrer);

            pnlItineraire = new Panel
            {
                Location = new Point(10, 360),
                Size = new Size(930, 200),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke
            };
            tabNouvelleCommande.Controls.Add(pnlItineraire);

            tabItineraire = new TabPage
            {
                Text = "Visualisation des itinéraires",
                BackColor = Color.White
            };
            tabCommandes.TabPages.Add(tabItineraire);

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


        }
        #endregion

        #region Méthodes
        private void ChargerCommandes()
        {
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
            dtCommandes.Columns.Add("Vehicule Type", typeof(string));
            commandes = commandeService.GetCommandes();
            foreach (var commande in commandes)
            {
                dtCommandes.Rows.Add(commande.Id, commande.Client.Nom ?? "None", commande.Date, commande.VilleDepart, commande.VilleArrivee, commande.Prix, commande.Statut.ToString(), commande.Chauffeur.Nom ?? "None", commande.Vehicule.Immatriculation.ToString(), commande.Vehicule.Type.ToString());
            }
            dgvCommandes.DataSource = dtCommandes;
        }

        private void ModifierCommande()
        {
            Commande commandeAModifier = commandes[dgvCommandes.SelectedRows[0].Index];
            UIModifCommande uiModifCommande = new UIModifCommande(dataInitializer, commandeAModifier);
            uiModifCommande.ShowDialog();
            Commande commandeModifie = uiModifCommande.ModifierCommande();
            if (commandeModifie == null) return;
            commandeService.ModifierCommande(commandeAModifier.Id, commandeModifie);
            ChargerCommandes();
            DataPersistenceService.SaveCommandes(commandes);
        }

        private void AnnulerCommande()
        {
            Commande commandeAModifier = commandes[dgvCommandes.SelectedRows[0].Index];
            if (commandeAModifier.Statut != StatutCommande.Annulee && commandeAModifier.Statut != StatutCommande.Livree)
            {
                if (commandeAModifier.Statut == StatutCommande.Payee)
                {
                    MessageBox.Show("La commande ne peut pas être annulée après avoir été payée.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                commandeAModifier.ChangerStatut(StatutCommande.Annulee);
            }
            ChargerCommandes();
            DataPersistenceService.SaveCommandes(commandes);
        }

        private void PayerCommande()
        {
            Commande commandeAModifier = commandes[dgvCommandes.SelectedRows[0].Index];
            if (commandeAModifier.Statut != StatutCommande.Payee)
            {
                commandeAModifier.ChangerStatut(StatutCommande.Payee);
            }
            ChargerCommandes();
            DataPersistenceService.SaveCommandes(commandes);
        }

        private void LivrerCommande()
        {
            Commande commandeAModifier = commandes[dgvCommandes.SelectedRows[0].Index];
            if (commandeAModifier.Statut == StatutCommande.Payee)
            {
                commandeAModifier.ChangerStatut(StatutCommande.Livree);
            }
            else
            {
                MessageBox.Show("La commande doit être payée avant d'être livrée.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ChargerCommandes();
            DataPersistenceService.SaveCommandes(commandes);
        }

        private void SupprimerCommande()
        {
            int index = dgvCommandes.SelectedRows[0].Index;
            Commande commandeASupprimer = commandes[index];
            commandeService.SupprimerCommande(commandeASupprimer.Id);
            ChargerCommandes();
            DataPersistenceService.SaveCommandes(commandes);
        }
        
        private void EnregistrerCommande()
        {

            if (cmbClient.SelectedIndex == -1 || cmbVilleDepart.SelectedIndex == -1 || cmbVilleArrivee.SelectedIndex == -1 ||
                cmbVehicule.SelectedIndex == -1 || cmbChauffeur.SelectedIndex == -1)
            {
                MessageBox.Show("Veuillez remplir tous les champs requis", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Vérifier si le chauffeur est déjà affecté à une commande à la même date
            string chauffeurNom = cmbChauffeur.SelectedItem.ToString();
            DateTime date = dtpDate.Value;
            foreach (var commande in commandes)
            {
                if (commande.Chauffeur.Nom == chauffeurNom && commande.Date.Date == date.Date)
                {
                    string message = $"Le chauffeur {chauffeurNom} est déjà affecté à une commande à cette date.";
                    message += "\nVoici les chauffeurs restants : ";
                    foreach (var s in dataInitializer.salaries)
                    {
                        if (s.Nom != chauffeurNom && s.Poste == "Chauffeur")
                        {
                            message += $"\n- {s.Nom}";
                        }
                    }
                    MessageBox.Show(message, "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            //Verifier si le véhicule est déjà affecté à une commande à la même date
            string vehiculeInfo = cmbVehicule.SelectedItem.ToString().Split('/')[0].Trim();
            foreach (var commande in commandes)
            {
                if (commande.Vehicule.Immatriculation == vehiculeInfo && commande.Date.Date == date.Date)
                {
                    string message = $"Le véhicule {vehiculeInfo} est déjà affecté à une commande à cette date.";
                    message += "\nVoici les véhicules restants : ";
                    foreach (var v in dataInitializer.vehicules)
                    {
                        if (v.Immatriculation != vehiculeInfo && v.EstDisponible)
                        {
                            message += $"\n- {v.Immatriculation} / {v.Type}";
                        }
                    }
                    MessageBox.Show(message, "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }


            string clientNom = cmbClient.SelectedItem.ToString();
            string villeDepart = cmbVilleDepart.SelectedItem.ToString();
            string villeArrivee = cmbVilleArrivee.SelectedItem.ToString();
            string prixText = lblPrix.Text.Replace("€", "");
            decimal prix = decimal.Parse(prixText);

            Client client = dataInitializer.clients.Find(c => $"{c.Nom} - {c.Prenom}" == clientNom);
            Vehicule vehicule = dataInitializer.vehicules.Find(v => v.Immatriculation == vehiculeInfo);
            Salarie chauffeur = dataInitializer.salaries.Find(s => s.Nom == chauffeurNom);


            Commande nouvelleCommande = new Commande(villeDepart, villeArrivee, date, prix);
            nouvelleCommande.Client = client;
            nouvelleCommande.Chauffeur = chauffeur;
            nouvelleCommande.Vehicule = vehicule;
            nouvelleCommande.Statut = StatutCommande.EnAttente;

            commandeService.AjouterCommande(nouvelleCommande);
            client.AddCommande(nouvelleCommande);


            cmbClient.SelectedIndex = -1;
            cmbVilleDepart.SelectedIndex = -1;
            cmbVilleArrivee.SelectedIndex = -1;
            cmbVehicule.SelectedIndex = -1;
            cmbChauffeur.SelectedIndex = -1;
            lblPrix.Text = "- €";
            pnlItineraire.Controls.Clear();

            tabCommandes.SelectedTab = tabListeCommandes;
            ChargerCommandes();
            DataPersistenceService.SaveCommandes(commandes);
            DataPersistenceService.SaveClients(dataInitializer.clients);

        }

        private void CalculerItineraire()
        {
            if (cmbVilleDepart.SelectedItem.ToString() == cmbVilleArrivee.SelectedItem.ToString())
            {
                MessageBox.Show("Les villes de départ et d'arrivée doivent être différentes", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            pnlItineraire.Controls.Clear();

            string villeDepart = cmbVilleDepart.SelectedItem.ToString();
            string villeArrivee = cmbVilleArrivee.SelectedItem.ToString();
            List<Noeud> itineraire = Dijkstra.TrouverCheminLePlusCourt(dataInitializer.grapheVille, dataInitializer.grapheVille.TrouverNoeudVille(villeDepart), dataInitializer.grapheVille.TrouverNoeudVille(villeArrivee));

            Label lblItineraire = new Label
            {
                Text = "Itinéraire calculé avec l'algorithme de Dijkstra",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(500, 20)
            };
            pnlItineraire.Controls.Add(lblItineraire);
            Graphe graphe = dataInitializer.grapheVille;


            decimal distance =Convert.ToDecimal(CalculDistance.CalculerDistanceTotale(graphe,Dijkstra.TrouverCheminLePlusCourt(graphe, graphe.TrouverNoeudVille(villeDepart), graphe.TrouverNoeudVille(villeArrivee)))) ;

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


            decimal prixKm = dataInitializer.vehicules[cmbVehicule.SelectedIndex].TarifKilometrique;
            Salarie chauffeur = dataInitializer.salaries[cmbChauffeur.SelectedIndex];
            decimal primeChauffeur = 0;
            if (DateTime.Now - chauffeur.DateEntree > TimeSpan.FromDays(365))
            {
                primeChauffeur = 100;
            }
            decimal prix = distance * prixKm + primeChauffeur;
            lblPrix.Text = $"{prix:C}";

            Label lblInfosPrix = new Label
            {
                Text = $"Tarif kilométrique: {prixKm:C}/km",
                Font = new Font("Arial", 10),
                Location = new Point(10, 100),
                Size = new Size(200, 20)
            };
            pnlItineraire.Controls.Add(lblInfosPrix);

            Label lblChemin = new Label
            {
                Text = "Chemin : ",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, 130),
                Size = new Size(100, 20)
            };
            pnlItineraire.Controls.Add(lblChemin);


            List<string> etapes = new List<string>();

            foreach (var noeud in itineraire)
            {
                etapes.Add(noeud.Entite.ToString());
            }

            Label lblEtapes = new Label
            {
                Text = string.Join(" → ", etapes),
                Font = new Font("Arial", 10),
                Location = new Point(120, 130),
                Size = new Size(500, 20)
            };
            pnlItineraire.Controls.Add(lblEtapes);
        }

        private void AfficherCarteVilles()
        {
            dataInitializer.AfficherGrapheVilleGraphique();
        }
        #endregion
    }

    public class UIModifCommande : Form
    {
        private TextBox txtId;
        private TextBox txtVilleDepart;
        private TextBox txtVilleArrivee;
        private TextBox txtDate;
        private TextBox txtPrix;
        private ComboBox cmbStatut;
        private TextBox txtClient;
        private TextBox txtChauffeur;
        private TextBox txtVehicule;
        private Button btnModifierClient;
        private Button btnAnnuler;
        private DataInitializer dataInitializer;

        public UIModifCommande(DataInitializer dataInitializer, Commande commandeAModifier)
        {
            this.dataInitializer = dataInitializer;
            InitializeComponents(commandeAModifier);
        }

        private void InitializeComponents(Commande commandeAModifier)
        {

            this.Text = "Modifier une Commande";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            Label IdLabel = new Label { Text = "ID:", Location = new Point(20, 20) };
            txtId = new TextBox { Location = new Point(150, 20), Width = 200 };
            this.Controls.Add(IdLabel);
            txtId.Text = commandeAModifier.Id.ToString();
            this.Controls.Add(txtId);

            Label VilleDepartLabel = new Label { Text = "Ville de départ:", Location = new Point(20, 60) };
            txtVilleDepart = new TextBox { Location = new Point(150, 60), Width = 200 };
            this.Controls.Add(VilleDepartLabel);
            txtVilleDepart.Text = commandeAModifier.VilleDepart;
            this.Controls.Add(txtVilleDepart);

            Label VilleArriveeLabel = new Label { Text = "Ville d'arrivée:", Location = new Point(20, 100) };
            txtVilleArrivee = new TextBox { Location = new Point(150, 100), Width = 200 };
            this.Controls.Add(VilleArriveeLabel);
            txtVilleArrivee.Text = commandeAModifier.VilleArrivee;
            this.Controls.Add(txtVilleArrivee);

            Label DateLabel = new Label { Text = "Date:", Location = new Point(20, 140) };
            txtDate = new TextBox { Location = new Point(150, 140), Width = 200 };
            this.Controls.Add(DateLabel);
            txtDate.Text = commandeAModifier.Date.ToString("yyyy-MM-dd");
            this.Controls.Add(txtDate);

            Label PrixLabel = new Label { Text = "Prix:", Location = new Point(20, 180) };
            txtPrix = new TextBox { Location = new Point(150, 180), Width = 200 };
            this.Controls.Add(PrixLabel);
            txtPrix.Text = commandeAModifier.Prix.ToString();
            this.Controls.Add(txtPrix);

            Label StatutLabel = new Label { Text = "Statut:", Location = new Point(20, 220) };
            cmbStatut = new ComboBox { Location = new Point(150, 220), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(StatutLabel);
            foreach (StatutCommande statut in Enum.GetValues(typeof(StatutCommande)))
            {
                cmbStatut.Items.Add(statut);
            }
            cmbStatut.SelectedItem = commandeAModifier.Statut;
            this.Controls.Add(cmbStatut);

            Label ClientLabel = new Label { Text = "Client:", Location = new Point(20, 260) };
            txtClient = new TextBox { Location = new Point(150, 260), Width = 200 };
            this.Controls.Add(ClientLabel);
            txtClient.Text = commandeAModifier.Client.Nom.ToString();
            this.Controls.Add(txtClient);

            Label ChauffeurLabel = new Label { Text = "Chauffeur:", Location = new Point(20, 300) };
            txtChauffeur = new TextBox { Location = new Point(150, 300), Width = 200 };
            this.Controls.Add(ChauffeurLabel);
            txtChauffeur.Text = commandeAModifier.Chauffeur.Nom.ToString();
            this.Controls.Add(txtChauffeur);

            Label VehiculeLabel = new Label { Text = "Véhicule:", Location = new Point(20, 340) };
            txtVehicule = new TextBox { Location = new Point(150, 340), Width = 200 };
            this.Controls.Add(VehiculeLabel);
            txtVehicule.Text = commandeAModifier.Vehicule.Immatriculation.ToString();
            this.Controls.Add(txtVehicule);

            btnModifierClient = new Button
            {
                Text = "Modifier Commande",
                Location = new Point(80, 400),
                Size = new Size(120, 30),
                BackColor = Color.LightGreen
            };
            btnModifierClient.Click += (s, e) => ModifierCommande();
            this.Controls.Add(btnModifierClient);

            btnAnnuler = new Button
            {
                Text = "Annuler",
                Location = new Point(220, 400),
                Size = new Size(120, 30),
                BackColor = Color.LightCoral
            };
            btnAnnuler.Click += (s, e) => this.Close();
            this.Controls.Add(btnAnnuler);

        }

        public Commande ModifierCommande()
        {
            string Id = txtId.Text;
            string VilleDepart = txtVilleDepart.Text;
            string VilleArrivee = txtVilleArrivee.Text;
            DateTime Date = DateTime.Now;
            if (txtDate.Text != "")
            {
                Date = DateTime.Parse(txtDate.Text);
            }
            decimal Prix = 0;
            if (txtPrix.Text != "")
            {
                Prix = decimal.Parse(txtPrix.Text);
            }
            StatutCommande Statut = (StatutCommande)Enum.Parse(typeof(StatutCommande), cmbStatut.Text);
            Client Client = dataInitializer.clients.Find(c => c.Nom == txtClient.Text);
            Salarie Chauffeur = dataInitializer.salaries.Find(c => c.Nom == txtChauffeur.Text);
            Vehicule Vehicule = dataInitializer.vehicules.Find(c => c.Immatriculation == txtVehicule.Text);
            Commande commande = new Commande(VilleDepart, VilleArrivee, Date, Prix);
            commande.Id = int.Parse(Id);
            commande.Statut = Statut;
            commande.Client = Client;
            commande.Chauffeur = Chauffeur;
            commande.Vehicule = Vehicule;

            this.Close();
            return commande;
        }
    }
}

