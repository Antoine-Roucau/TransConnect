using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TransConnect.Data;
using TransConnect.Models;
using Transconnect.Services;

namespace TransConnect.UI
{
    public class UISalarie : Form
    {
        private DataGridView dgvSalaries;
        private Button btnAjouter;
        private Button btnModifier;
        private Button btnLicencier;
        private Button btnAfficherOrganigramme;
        private Button btnFermer;
        private Panel pnlInfos;
        private Label lblTitre;
        private DataInitializer dataInitializer;
        private SalarieService salarieService;

        public UISalarie(DataInitializer dataInitializer)
        {
            this.dataInitializer = dataInitializer;
            this.salarieService = new SalarieService();
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
            // Placeholder - à remplacer par le code réel
            DataTable dtSalaries = new DataTable();
            dtSalaries.Columns.Add("Numéro SS", typeof(string));
            dtSalaries.Columns.Add("Nom", typeof(string));
            dtSalaries.Columns.Add("Prénom", typeof(string));
            dtSalaries.Columns.Add("Poste", typeof(string));
            dtSalaries.Columns.Add("Date d'entrée", typeof(DateTime));
            dtSalaries.Columns.Add("Salaire", typeof(decimal));

            // Simuler quelques données
            dtSalaries.Rows.Add("123456789012345", "Dupond", "Michel", "Directeur Général", new DateTime(2005, 6, 1), 9000.00m);
            dtSalaries.Rows.Add("987654321098765", "Fiesta", "Sophie", "Directrice Commerciale", new DateTime(2010, 9, 15), 7500.00m);
            dtSalaries.Rows.Add("456789123456789", "Fetard", "Julien", "Directeur des Opérations", new DateTime(2008, 3, 10), 7200.00m);
            dtSalaries.Rows.Add("258369741963852", "Romu", "David", "Chauffeur", new DateTime(2017, 3, 10), 3800.00m);

            dgvSalaries.DataSource = dtSalaries;
        }

        private void AfficherInfosSalarie()
        {
            // Placeholder pour afficher les informations détaillées du salarié sélectionné
            if (dgvSalaries.SelectedRows.Count == 0) return;

            pnlInfos.Controls.Clear();

            var row = dgvSalaries.SelectedRows[0];
            string nom = row.Cells["Nom"].Value.ToString();
            string prenom = row.Cells["Prénom"].Value.ToString();
            string poste = row.Cells["Poste"].Value.ToString();

            Label lblInfos = new Label
            {
                Text = $"Informations détaillées pour {prenom} {nom} - {poste}",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(500, 20)
            };
            pnlInfos.Controls.Add(lblInfos);

            // Afficher des infos supplémentaires (adresse, etc.)
            TableLayoutPanel tableInfos = new TableLayoutPanel
            {
                Location = new Point(10, 40),
                Size = new Size(940, 100),
                ColumnCount = 2,
                RowCount = 4
            };
            
            tableInfos.Controls.Add(new Label { Text = "Adresse :", Font = new Font("Arial", 9, FontStyle.Bold) }, 0, 0);
            tableInfos.Controls.Add(new Label { Text = "12 rue de la Paix, 75001 Paris" }, 1, 0);
            
            tableInfos.Controls.Add(new Label { Text = "Email :", Font = new Font("Arial", 9, FontStyle.Bold) }, 0, 1);
            tableInfos.Controls.Add(new Label { Text = $"{prenom.ToLower()}.{nom.ToLower()}@transconnect.fr" }, 1, 1);
            
            tableInfos.Controls.Add(new Label { Text = "Téléphone :", Font = new Font("Arial", 9, FontStyle.Bold) }, 0, 2);
            tableInfos.Controls.Add(new Label { Text = "06 12 34 56 78" }, 1, 2);
            
            tableInfos.Controls.Add(new Label { Text = "Subordonnés :", Font = new Font("Arial", 9, FontStyle.Bold) }, 0, 3);
            
            if (poste.Contains("Directeur") || poste.Contains("Chef"))
            {
                tableInfos.Controls.Add(new Label { Text = "4 subordonnés directs (cliquez sur 'Afficher l'organigramme' pour plus de détails)" }, 1, 3);
            }
            else
            {
                tableInfos.Controls.Add(new Label { Text = "Aucun subordonné" }, 1, 3);
            }
            
            pnlInfos.Controls.Add(tableInfos);
        }

        private void AjouterSalarie()
        {
            // Placeholder pour ajouter un salarié
            MessageBox.Show("Fonctionnalité à implémenter: Ajouter un salarié", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ModifierSalarie()
        {
            // Placeholder pour modifier un salarié
            if (dgvSalaries.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner un salarié à modifier", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Fonctionnalité à implémenter: Modifier un salarié", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LicencierSalarie()
        {
            // Placeholder pour licencier un salarié
            if (dgvSalaries.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner un salarié à licencier", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = dgvSalaries.SelectedRows[0];
            string nom = row.Cells["Nom"].Value.ToString();
            string prenom = row.Cells["Prénom"].Value.ToString();

            DialogResult result = MessageBox.Show($"Êtes-vous sûr de vouloir licencier {prenom} {nom}?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Code de licenciement
                MessageBox.Show("Salarié licencié avec succès!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    }
}