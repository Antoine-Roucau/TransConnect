using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Transconnect.Data;
using Transconnect.Models;
using Transconnect.Models.Graphe;
using Transconnect.Services;

namespace Transconnect.UI
{
    public class UISalarie : Form
    {
        private DataGridView dgvSalaries;
        private Button btnAjouter;
        private Button btnModifier;
        private Button btnLicencier;
        private Button btnAjouterSubordonnes;
        private Button btnAfficherOrganigramme;
        private Button btnFermer;
        private Panel pnlInfos;
        private Label lblTitre;
        private DataInitializer dataInitializer;
        private List<Salarie> salaries;
        private SalarieService salarieService;

        public UISalarie(DataInitializer dataInitializer)
        {
            this.dataInitializer = dataInitializer;
            this.salarieService = new SalarieService();
            this.salaries = dataInitializer.salaries;

            InitializeComponents();
            ChargerSalaries();
        }

        private void InitializeComponents()
        {
            // Configuration du formulaire
            this.Text = "TransConnect - Gestion des Salariés";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Titre
            lblTitre = new Label
            {
                Text = "Gestion des Salariés",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(300, 30)
            };
            this.Controls.Add(lblTitre);

            // DataGridView pour afficher les salariés
            dgvSalaries = new DataGridView
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
            dgvSalaries.SelectionChanged += (s, e) => AfficherInfosSalarie();
            this.Controls.Add(dgvSalaries);

            // Panel pour afficher les infos détaillées du salarié
            pnlInfos = new Panel
            {
                Location = new Point(20, 470),
                Size = new Size(960, 150),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke
            };
            this.Controls.Add(pnlInfos);

            // Boutons d'action
            btnAjouter = new Button
            {
                Text = "Ajouter un salarié",
                Location = new Point(20, 630),
                Size = new Size(150, 30),
                BackColor = Color.LightGreen
            };
            btnAjouter.Click += (s, e) => AjouterSalarie();
            this.Controls.Add(btnAjouter);

            btnModifier = new Button
            {
                Text = "Modifier",
                Location = new Point(190, 630),
                Size = new Size(100, 30),
                BackColor = Color.LightBlue
            };
            btnModifier.Click += (s, e) => ModifierSalarie();
            this.Controls.Add(btnModifier);

            btnLicencier = new Button
            {
                Text = "Licencier",
                Location = new Point(310, 630),
                Size = new Size(100, 30),
                BackColor = Color.Salmon
            };
            btnLicencier.Click += (s, e) => LicencierSalarie();
            this.Controls.Add(btnLicencier);

            btnAjouterSubordonnes = new Button
            {
                Text = "Gérer les subordonnés",
                Location = new Point(620, 630),
                Size = new Size(180, 30),
                BackColor = Color.LightYellow
            };
            btnAjouterSubordonnes.Click += (s, e) => AjouterSubordonnes();
            this.Controls.Add(btnAjouterSubordonnes);

            btnAfficherOrganigramme = new Button
            {
                Text = "Afficher l'organigramme",
                Location = new Point(430, 630),
                Size = new Size(180, 30),
                BackColor = Color.LightYellow
            };
            btnAfficherOrganigramme.Click += (s, e) => AfficherOrganigramme();
            this.Controls.Add(btnAfficherOrganigramme);

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

        private void ChargerSalaries()
        {
            DataTable dtSalaries = new DataTable();
            dtSalaries.Columns.Add("NumeroSS", typeof(string));
            dtSalaries.Columns.Add("Nom", typeof(string));
            dtSalaries.Columns.Add("Prenom", typeof(string));
            dtSalaries.Columns.Add("Poste", typeof(string));
            dtSalaries.Columns.Add("Date_d'entree", typeof(DateTime));
            dtSalaries.Columns.Add("Salaire", typeof(decimal));

            foreach (var salarie in salaries)
            {
                dtSalaries.Rows.Add(salarie.NumeroSS, salarie.Nom, salarie.Prenom, salarie.Poste, salarie.DateEntree, salarie.Salaire);
            }

            dgvSalaries.DataSource = dtSalaries;
        }

        private void AfficherInfosSalarie()
        {
            if (dgvSalaries.SelectedRows.Count == 0) return;

            pnlInfos.Controls.Clear();
            Salarie salarie = salaries[dgvSalaries.SelectedRows[0].Index];

            Label lblInfos = new Label
            {
                Text = $"Informations détaillées pour {salarie.Prenom} {salarie.Nom} - {salarie.Poste}",
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(500, 20)
            };
            pnlInfos.Controls.Add(lblInfos);

            TableLayoutPanel tableInfos = new TableLayoutPanel
            {
                Location = new Point(10, 40),
                Size = new Size(940, 100),
                ColumnCount = 2,
                RowCount = 4
            };

            tableInfos.Controls.Add(new Label { Text = "Adresse :", Font = new Font("Arial", 9, FontStyle.Bold) }, 0, 0);
            tableInfos.Controls.Add(new Label { Text = salarie.AdressePostale, AutoSize = true }, 1, 0);

            tableInfos.Controls.Add(new Label { Text = "Email :", Font = new Font("Arial", 9, FontStyle.Bold) }, 0, 1);
            tableInfos.Controls.Add(new Label { Text = salarie.AdresseMail, AutoSize = true }, 1, 1);

            tableInfos.Controls.Add(new Label { Text = "Téléphone :", Font = new Font("Arial", 9, FontStyle.Bold) }, 0, 2);
            tableInfos.Controls.Add(new Label { Text = salarie.Telephone, AutoSize = true }, 1, 2);

            string line = "";
            foreach (var subordinate in salarie.Subordonnes)
            {
                line += $"|{subordinate.Nom} {subordinate.Prenom}/{subordinate.Poste}|  ";
            }
            tableInfos.Controls.Add(new Label { Text = "Subordonnés :", Font = new Font("Arial", 9, FontStyle.Bold) }, 0, 3);
            tableInfos.Controls.Add(new Label { Text = line, AutoSize = true }, 1, 3);


            pnlInfos.Controls.Add(tableInfos);
        }

        private void AjouterSalarie()
        {
            UIAddSalarie uiAddSalarie = new UIAddSalarie(dataInitializer);
            uiAddSalarie.ShowDialog();
            Salarie nouveauSalarie = uiAddSalarie.AjouterSalarie();
            if (nouveauSalarie == null) return; // Si l'utilisateur a annulé l'ajout
            salarieService.AjouterSalarie(nouveauSalarie, salaries);
            ChargerSalaries();
            DataPersistenceService.SaveSalaries(salaries);
        }

        private void ModifierSalarie()
        {
            Salarie salarieAModifier = salaries[dgvSalaries.SelectedRows[0].Index];
            UIModifSalarie uiModifSalarie = new UIModifSalarie(dataInitializer, salarieAModifier);
            uiModifSalarie.ShowDialog();
            Salarie salarieModifie = uiModifSalarie.ModifierSalarie();
            if (salarieModifie == null) return; // Si l'utilisateur a annulé la modification
            salarieService.ModifierSalarie(salarieAModifier, salarieModifie.NumeroSS, salarieModifie.Nom, salarieModifie.Prenom, salarieModifie.DateNaissance, salarieModifie.AdressePostale, salarieModifie.AdresseMail, salarieModifie.Telephone, salarieModifie.DateEntree, salarieModifie.Poste, salarieModifie.Salaire, new List<Salarie>());
            ChargerSalaries();
            DataPersistenceService.SaveSalaries(salaries);
        }

        private void LicencierSalarie()
        {

            Salarie salarieALicencier = salaries[dgvSalaries.SelectedRows[0].Index];
            if (salarieALicencier == null) return; // Si l'utilisateur a annulé la sélection

            ChargerSalaries();
            DataPersistenceService.SaveSalaries(salaries);
            DataPersistenceService.SaveHierarchie(salaries);
        }

        private void AjouterSubordonnes()
        {
            if (dgvSalaries.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez d'abord sélectionner un employé qui sera le manager",
                    "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int selectedIndex = dgvSalaries.SelectedRows[0].Index;
            Salarie manager = salaries[selectedIndex];

            UISelectionSubordonnes formSelection = new UISelectionSubordonnes(dataInitializer, manager);
            formSelection.ShowDialog();

            // Rafraîchir l'affichage après l'ajout de subordonnés
            ChargerSalaries();
            AfficherInfosSalarie();
        }
        private void AfficherOrganigramme()
        {
            dataInitializer.AfficherGrapheSalarieGraphique();
        }
    }

    public class UIAddSalarie : Form
    {
        private TextBox txtNumeroSS;
        private TextBox txtNom;
        private TextBox txtPrenom;
        private TextBox txtDateNaissance;
        private TextBox txtAdressePostale;
        private TextBox txtAdresseMail;
        private TextBox txtTelephone;
        private TextBox txtDateEntree;
        private TextBox txtPoste;
        private TextBox txtSalaire;
        private Button btnAjouterSalarie;
        private Button btnAnnuler;
        private DataInitializer dataInitializer;

        public UIAddSalarie(DataInitializer dataInitializer)
        {
            this.dataInitializer = dataInitializer;
            InitializeComponents();
        }

        private void InitializeComponents()
        {

            this.Text = "Ajouter un Salarie";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;


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

            Label lblDateEntree = new Label { Text = "Date d'entrée:", Location = new Point(20, 300) };
            txtDateEntree = new TextBox { Location = new Point(150, 300), Width = 200 };
            this.Controls.Add(lblDateEntree);
            this.Controls.Add(txtDateEntree);

            Label lblPoste = new Label { Text = "Poste:", Location = new Point(20, 340) };
            txtPoste = new TextBox { Location = new Point(150, 340), Width = 200 };
            this.Controls.Add(lblPoste);
            this.Controls.Add(txtPoste);

            Label lblSalaire = new Label { Text = "Salaire:", Location = new Point(20, 380) };
            txtSalaire = new TextBox { Location = new Point(150, 380), Width = 200 };
            this.Controls.Add(lblSalaire);
            this.Controls.Add(txtSalaire);

            // Boutons
            btnAjouterSalarie = new Button
            {
                Text = "Ajouter Salarie",
                Location = new Point(80, 420),
                Size = new Size(120, 30),
                BackColor = Color.LightGreen
            };
            btnAjouterSalarie.Click += (s, e) => AjouterSalarie();
            this.Controls.Add(btnAjouterSalarie);

            btnAnnuler = new Button
            {
                Text = "Annuler",
                Location = new Point(220, 420),
                Size = new Size(120, 30),
                BackColor = Color.LightCoral
            };
            btnAnnuler.Click += (s, e) => this.Close();
            this.Controls.Add(btnAnnuler);
        }

        public Salarie AjouterSalarie()
        {
            if (string.IsNullOrWhiteSpace(txtNumeroSS.Text) || string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtPrenom.Text))
            {
                return null;
            }
            string numeroSS = txtNumeroSS.Text;
            string nom = txtNom.Text;
            string prenom = txtPrenom.Text;
            DateTime dateNaissance = DateTime.Now;
            if (txtDateNaissance.Text != "")
            {
                dateNaissance = DateTime.Parse(txtDateNaissance.Text);
            }
            string adressePostale = txtAdressePostale.Text;
            string adresseMail = txtAdresseMail.Text;
            string telephone = txtTelephone.Text;
            DateTime dateEntree = DateTime.Now;
            if (txtDateEntree.Text != "")
            {
                dateEntree = DateTime.Parse(txtDateEntree.Text);
            }
            string poste = txtPoste.Text;
            decimal salaire = decimal.Parse(txtSalaire.Text);

            Salarie salarie = new Salarie(numeroSS, nom, prenom, dateNaissance, adressePostale, adresseMail, telephone, dateEntree, poste, salaire);

            this.Close();
            return salarie;
        }

    }

    public class UIModifSalarie : Form
    {
        private TextBox txtNumeroSS;
        private TextBox txtNom;
        private TextBox txtPrenom;
        private TextBox txtDateNaissance;
        private TextBox txtAdressePostale;
        private TextBox txtAdresseMail;
        private TextBox txtTelephone;
        private TextBox txtDateEntree;
        private TextBox txtPoste;
        private TextBox txtSalaire;
        private Button btnModifierSalarie;
        private Button btnAnnuler;
        private DataInitializer dataInitializer;

        public UIModifSalarie(DataInitializer dataInitializer, Salarie salarieAModifier)
        {
            this.dataInitializer = dataInitializer;
            InitializeComponents(salarieAModifier);
        }


        private void InitializeComponents(Salarie salarieAModifier)
        {

            // Configuration du formulaire
            this.Text = "Modifier un Salarie";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Champs de saisie
            Label numeroSSLabel = new Label { Text = "Numéro de Sécurité Sociale:", Location = new Point(20, 20) };
            txtNumeroSS = new TextBox { Location = new Point(150, 20), Width = 200 };
            this.Controls.Add(numeroSSLabel);
            txtNumeroSS.Text = salarieAModifier.NumeroSS;
            this.Controls.Add(txtNumeroSS);

            Label lblNom = new Label { Text = "Nom:", Location = new Point(20, 60) };
            txtNom = new TextBox { Location = new Point(150, 60), Width = 200 };
            this.Controls.Add(lblNom);
            txtNom.Text = salarieAModifier.Nom;
            this.Controls.Add(txtNom);

            Label lblPrenom = new Label { Text = "Prénom:", Location = new Point(20, 100) };
            txtPrenom = new TextBox { Location = new Point(150, 100), Width = 200 };
            this.Controls.Add(lblPrenom);
            txtPrenom.Text = salarieAModifier.Prenom;
            this.Controls.Add(txtPrenom);


            Label lblDateNaissance = new Label { Text = "Date de Naissance:", Location = new Point(20, 140) };
            txtDateNaissance = new TextBox { Location = new Point(150, 140), Width = 200 };
            this.Controls.Add(lblDateNaissance);
            txtDateNaissance.Text = salarieAModifier.DateNaissance.ToString("yyyy-MM-dd");
            this.Controls.Add(txtDateNaissance);

            Label lblAdressePostale = new Label { Text = "Adresse Postale:", Location = new Point(20, 180) };
            txtAdressePostale = new TextBox { Location = new Point(150, 180), Width = 200 };
            this.Controls.Add(lblAdressePostale);
            txtAdressePostale.Text = salarieAModifier.AdressePostale;
            this.Controls.Add(txtAdressePostale);

            Label lblAdresseMail = new Label { Text = "Adresse Email:", Location = new Point(20, 220) };
            txtAdresseMail = new TextBox { Location = new Point(150, 220), Width = 200 };
            this.Controls.Add(lblAdresseMail);
            txtAdresseMail.Text = salarieAModifier.AdresseMail;
            this.Controls.Add(txtAdresseMail);

            Label lblTelephone = new Label { Text = "Telephone :", Location = new Point(20, 260) };
            txtTelephone = new TextBox { Location = new Point(150, 260), Width = 200 };
            this.Controls.Add(lblTelephone);
            txtTelephone.Text = salarieAModifier.Telephone;
            this.Controls.Add(txtTelephone);

            Label lblDateEntree = new Label { Text = "Date d'entrée:", Location = new Point(20, 300) };
            txtDateEntree = new TextBox { Location = new Point(150, 300), Width = 200 };
            this.Controls.Add(lblDateEntree);
            txtDateEntree.Text = salarieAModifier.DateEntree.ToString("yyyy-MM-dd");
            this.Controls.Add(txtDateEntree);

            Label lblPoste = new Label { Text = "Poste:", Location = new Point(20, 340) };
            txtPoste = new TextBox { Location = new Point(150, 340), Width = 200 };
            this.Controls.Add(lblPoste);
            txtPoste.Text = salarieAModifier.Poste;
            this.Controls.Add(txtPoste);

            Label lblSalaire = new Label { Text = "Salaire:", Location = new Point(20, 380) };
            txtSalaire = new TextBox { Location = new Point(150, 380), Width = 200 };
            this.Controls.Add(lblSalaire);
            txtSalaire.Text = salarieAModifier.Salaire.ToString();
            this.Controls.Add(txtSalaire);


            // Boutons
            btnModifierSalarie = new Button
            {
                Text = "Modifier Salarie",
                Location = new Point(80, 420),
                Size = new Size(120, 30),
                BackColor = Color.LightGreen
            };
            btnModifierSalarie.Click += (s, e) => ModifierSalarie();
            this.Controls.Add(btnModifierSalarie);

            btnAnnuler = new Button
            {
                Text = "Annuler",
                Location = new Point(220, 420),
                Size = new Size(120, 30),
                BackColor = Color.LightCoral
            };
            btnAnnuler.Click += (s, e) => this.Close();
            this.Controls.Add(btnAnnuler);

        }

        public Salarie ModifierSalarie()
        {
            if (string.IsNullOrWhiteSpace(txtNumeroSS.Text) || string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtPrenom.Text))
            {
                return null;
            }
            string numeroSS = txtNumeroSS.Text;
            string nom = txtNom.Text;
            string prenom = txtPrenom.Text;
            DateTime dateNaissance = DateTime.Now;
            if (txtDateNaissance.Text != "")
            {
                dateNaissance = DateTime.Parse(txtDateNaissance.Text);
            }
            string adressePostale = txtAdressePostale.Text;
            string adresseMail = txtAdresseMail.Text;
            string telephone = txtTelephone.Text;
            DateTime dateEntree = DateTime.Now;
            if (txtDateEntree.Text != "")
            {
                dateEntree = DateTime.Parse(txtDateEntree.Text);
            }
            string poste = txtPoste.Text;
            decimal salaire = decimal.Parse(txtSalaire.Text);

            Salarie salarie = new Salarie(numeroSS, nom, prenom, dateNaissance, adressePostale, adresseMail, telephone, dateEntree, poste, salaire);

            this.Close();
            return salarie;
        }
    }

    public class UISelectionSubordonnes : Form
    {
        private DataGridView dgvEmployes;
        private Button btnAjouter;
        private Button btnSupprimer;
        private Button btnAnnuler;
        private Label lblInfo;

        private DataInitializer dataInitializer;
        private Salarie manager;
        private List<Salarie> salaries;

        public UISelectionSubordonnes(DataInitializer dataInitializer, Salarie manager)
        {
            this.dataInitializer = dataInitializer;
            this.manager = manager;
            this.salaries = dataInitializer.salaries;

            InitializeComponents();
            ChargerEmployes();
        }

        private void InitializeComponents()
        {
            // Configuration du formulaire
            this.Text = "Sélection des subordonnés";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            // Information
            lblInfo = new Label
            {
                Text = $"Sélectionnez les employés qui seront subordonnés à {manager.Prenom} {manager.Nom}",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(560, 20)
            };
            this.Controls.Add(lblInfo);

            // DataGridView pour afficher les employés
            dgvEmployes = new DataGridView
            {
                Location = new Point(20, 50),
                Size = new Size(560, 350),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.WhiteSmoke
            };
            this.Controls.Add(dgvEmployes);

            // Boutons
            btnAjouter = new Button
            {
                Text = "Ajouter comme subordonnés",
                Location = new Point(20, 420),
                Size = new Size(200, 30),
                BackColor = Color.LightGreen
            };
            btnAjouter.Click += (s, e) => AjouterSubordonnes();
            this.Controls.Add(btnAjouter);

            btnSupprimer = new Button
            {
                Text = "Supprimer",
                Location = new Point(230, 420),
                Size = new Size(100, 30),
                BackColor = Color.Red
            };
            btnSupprimer.Click += (s, e) => SupprimerSubordonnes();
            this.Controls.Add(btnSupprimer);


            btnAnnuler = new Button
            {
                Text = "Annuler",
                Location = new Point(480, 420),
                Size = new Size(100, 30),
                BackColor = Color.LightCoral
            };
            btnAnnuler.Click += (s, e) => this.Close();
            this.Controls.Add(btnAnnuler);
        }

        private void ChargerEmployes()
        {
            DataTable dtEmployes = new DataTable();
            dtEmployes.Columns.Add("NumeroSS", typeof(string));
            dtEmployes.Columns.Add("Nom", typeof(string));
            dtEmployes.Columns.Add("Prenom", typeof(string));
            dtEmployes.Columns.Add("Poste", typeof(string));

            // Filtrer pour ne pas montrer le manager lui-même ni ses subordonnés actuels
            foreach (var salarie in salaries)
            {
                if (salarie.NumeroSS != manager.NumeroSS)
                {
                    dtEmployes.Rows.Add(salarie.NumeroSS, salarie.Nom, salarie.Prenom, salarie.Poste);
                }
            }

            dgvEmployes.DataSource = dtEmployes;
        }

        private void AjouterSubordonnes()
        {
            if (dgvEmployes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner au moins un employé",
                    "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<Salarie> nouveauxSubordonnes = new List<Salarie>();

            foreach (DataGridViewRow row in dgvEmployes.SelectedRows)
            {
                string numeroSS = row.Cells["NumeroSS"].Value.ToString();
                Salarie subordonne = salaries.Find(s => s.NumeroSS == numeroSS);

                if (subordonne != null)
                {
                    nouveauxSubordonnes.Add(subordonne);
                }
            }

            if (nouveauxSubordonnes.Count > 0)
            {
                // Mettre à jour la relation dans le modèle et le graphe
                foreach (var subordonne in nouveauxSubordonnes)
                {
                    // Ajouter à la liste des subordonnés du manager
                    manager.AddSubordonnes(subordonne);

                    // Mettre à jour le graphe
                    Noeud noeudManager = dataInitializer.grapheSalarie.TrouverNoeudParSalarieNumeroSS(manager.NumeroSS);
                    Noeud noeudSubordonne = dataInitializer.grapheSalarie.TrouverNoeudParSalarieNumeroSS(subordonne.NumeroSS);

                    if (noeudManager != null && noeudSubordonne != null)
                    {
                        // Vérifier si le lien existe déjà
                        bool lienExiste = false;
                        foreach (var lien in dataInitializer.grapheSalarie.Liens)
                        {
                            if ((lien.Noeud1 == noeudManager && lien.Noeud2 == noeudSubordonne && lien.Oriente == true) ||
                                (lien.Noeud2 == noeudManager && lien.Noeud1 == noeudSubordonne && lien.Oriente == false))
                            {
                                lienExiste = true;
                                break;
                            }
                        }

                        if (!lienExiste)
                        {
                            // Ajouter un nouveau lien orienté (manager -> subordonné)
                            Lien nouveauLien = new Lien(noeudManager, noeudSubordonne, null, true);
                            dataInitializer.grapheSalarie.AjouterLien(nouveauLien);
                        }
                    }
                }
                DataPersistenceService.SaveHierarchie(dataInitializer.salaries);
                this.Close();
            }
        }

        private void SupprimerSubordonnes()
        {
            if (dgvEmployes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner au moins un employé",
                    "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<Salarie> subordonnesASupprimer = new List<Salarie>();

            foreach (DataGridViewRow row in dgvEmployes.SelectedRows)
            {
                string numeroSS = row.Cells["NumeroSS"].Value.ToString();
                Salarie subordonne = salaries.Find(s => s.NumeroSS == numeroSS);

                if (subordonne != null)
                {
                    subordonnesASupprimer.Add(subordonne);
                }
            }

            if (subordonnesASupprimer.Count > 0)
            {
                // Mettre à jour la relation dans le modèle et le graphe
                foreach (var subordonne in subordonnesASupprimer)
                {
                    // Supprimer de la liste des subordonnés du manager
                    manager.SupSubordonnes(subordonne);

                    // Mettre à jour le graphe
                    Noeud noeudManager = dataInitializer.grapheSalarie.TrouverNoeudParSalarieNumeroSS(manager.NumeroSS);
                    Noeud noeudSubordonne = dataInitializer.grapheSalarie.TrouverNoeudParSalarieNumeroSS(subordonne.NumeroSS);

                    if (noeudManager != null && noeudSubordonne != null)
                    {
                        // Supprimer le lien entre le manager et le subordonné
                        Lien lienASupprimer = dataInitializer.grapheSalarie.Liens.Find(l => l.Noeud1 == noeudManager && l.Noeud2 == noeudSubordonne);
                        if (lienASupprimer != null)
                        {
                            dataInitializer.grapheSalarie.SupprimerLien(lienASupprimer);
                        }
                    }
                }
                DataPersistenceService.SaveHierarchie(dataInitializer.salaries);
                this.Close();
            }
        }
    }



}