using System;
using System.Drawing;
using System.Windows.Forms;
using TransConnect.Data;
using TransConnect.UI;

namespace TransConnect.UI
{
    public class MenuPrincipal : Form
    {
        private Button btnClients;
        private Button btnSalaries;
        private Button btnCommandes;
        private Button btnStatistiques;
        private Button btnVisualisation;
        private Button btnQuitter;
        private Label lblTitre;
        private DataInitializer dataInitializer;

        public MenuPrincipal(DataInitializer dataInitializer)
        {
            this.dataInitializer = dataInitializer;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Configuration du formulaire
            this.Text = "TransConnect - Gestion de Transport Routier";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Titre
            lblTitre = new Label
            {
                Text = "TransConnect",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(400, 50),
                Location = new Point((this.ClientSize.Width - 400) / 2, 30)
            };
            this.Controls.Add(lblTitre);

            // Création des boutons avec une taille et un espacement uniformes
            int btnWidth = 200;
            int btnHeight = 60;
            int startY = 120;
            int spacing = 80;

            // Bouton Clients
            btnClients = new Button
            {
                Text = "Gestion des Clients",
                Size = new Size(btnWidth, btnHeight),
                Location = new Point((this.ClientSize.Width - btnWidth) / 2, startY),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 12)
            };
            btnClients.Click += (s, e) => OuvrirGestionClients();
            this.Controls.Add(btnClients);

            // Bouton Salariés
            btnSalaries = new Button
            {
                Text = "Gestion des Salariés",
                Size = new Size(btnWidth, btnHeight),
                Location = new Point((this.ClientSize.Width - btnWidth) / 2, startY + spacing),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 12)
            };
            btnSalaries.Click += (s, e) => OuvrirGestionSalaries();
            this.Controls.Add(btnSalaries);

            // Bouton Commandes
            btnCommandes = new Button
            {
                Text = "Gestion des Commandes",
                Size = new Size(btnWidth, btnHeight),
                Location = new Point((this.ClientSize.Width - btnWidth) / 2, startY + spacing * 2),
                BackColor = Color.LightSalmon,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 12)
            };
            btnCommandes.Click += (s, e) => OuvrirGestionCommandes();
            this.Controls.Add(btnCommandes);

            // Bouton Statistiques
            btnStatistiques = new Button
            {
                Text = "Statistiques",
                Size = new Size(btnWidth, btnHeight),
                Location = new Point((this.ClientSize.Width - btnWidth) / 2, startY + spacing * 3),
                BackColor = Color.LightYellow,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 12)
            };
            btnStatistiques.Click += (s, e) => OuvrirStatistiques();
            this.Controls.Add(btnStatistiques);

            // Bouton Visualisation
            btnVisualisation = new Button
            {
                Text = "Visualisation",
                Size = new Size(btnWidth, btnHeight),
                Location = new Point((this.ClientSize.Width - btnWidth) / 2, startY + spacing * 4),
                BackColor = Color.Lavender,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 12)
            };
            btnVisualisation.Click += (s, e) => OuvrirVisualisation();
            this.Controls.Add(btnVisualisation);

            // Bouton Quitter
            btnQuitter = new Button
            {
                Text = "Quitter",
                Size = new Size(btnWidth, btnHeight),
                Location = new Point((this.ClientSize.Width - btnWidth) / 2, startY + spacing * 5),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 12)
            };
            btnQuitter.Click += (s, e) => this.Close();
            this.Controls.Add(btnQuitter);
        }

        private void OuvrirGestionClients()
        {
            var formClient = new UIClient(dataInitializer);
            formClient.ShowDialog();
        }

        private void OuvrirGestionSalaries()
        {
            var formSalarie = new UISalarie(dataInitializer);
            formSalarie.ShowDialog();
        }

        private void OuvrirGestionCommandes()
        {
            var formCommande = new UICommande(dataInitializer);
            formCommande.ShowDialog();
        }

        private void OuvrirStatistiques()
        {
            var formStats = new UIStatistique(dataInitializer);
            formStats.ShowDialog();
        }

        private void OuvrirVisualisation()
        {
            var formVisu = new UIVisualisation(dataInitializer);
            formVisu.ShowDialog();
        }
    }
}