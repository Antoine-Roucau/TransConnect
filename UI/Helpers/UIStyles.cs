using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Transconnect.UI.Helpers
{
    /// <summary>
    /// Classe statique pour appliquer des styles cohérents aux contrôles de l'interface utilisateur
    /// </summary>
    public static class UIStyles
    {
        /// <summary>
        /// Applique un style moderne à un formulaire
        /// </summary>
        public static void ApplyFormStyle(Form form)
        {
            form.BackColor = UIColors.BackgroundColor;
            form.Font = UIFonts.Body;
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.MaximizeBox = false;
            form.StartPosition = FormStartPosition.CenterScreen;
        }
        
        /// <summary>
        /// Applique un style moderne à un DataGridView
        /// </summary>
        public static void ApplyDataGridViewStyle(DataGridView dgv)
        {
            // Styles de base
            dgv.BackgroundColor = UIColors.CardColor;
            dgv.BorderStyle = BorderStyle.None;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowTemplate.Height = 30;
            
            // Styles d'en-tête
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = UIColors.LightGrayColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = UIColors.MediumGrayColor;
            dgv.ColumnHeadersDefaultCellStyle.Font = UIFonts.SecondaryText;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(10);
            dgv.ColumnHeadersHeight = 40;
            
            // Styles des cellules
            dgv.DefaultCellStyle.Font = UIFonts.Body;
            dgv.DefaultCellStyle.SelectionBackColor = UIColors.WithAlpha(UIColors.PrimaryColor, 50);
            dgv.DefaultCellStyle.SelectionForeColor = UIColors.DarkGrayColor;
            
            // Alternance de couleurs pour les lignes
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
        }
        
        /// <summary>
        /// Crée un panel d'en-tête pour un formulaire avec une couleur spécifiée
        /// </summary>
        public static Panel CreateHeaderPanel(string title, Color backgroundColor, EventHandler returnButtonHandler = null)
        {
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = backgroundColor
            };
            
            Label lblTitle = new Label
            {
                Text = "TransConnect | " + title,
                Font = UIFonts.Subtitle,
                ForeColor = Color.White,
                Location = new Point(20, 18),
                AutoSize = true
            };
            
            headerPanel.Controls.Add(lblTitle);
            
            if (returnButtonHandler != null)
            {
                Button btnRetour = new Button
                {
                    Text = "Retour au menu",
                    BackColor = UIColors.Darken(backgroundColor),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(130, 30),
                    Location = new Point(headerPanel.Width - 150, 15),
                    Font = UIFonts.SmallText
                };
                
                btnRetour.FlatAppearance.BorderSize = 0;
                btnRetour.Click += returnButtonHandler;
                
                headerPanel.Controls.Add(btnRetour);
            }
            
            return headerPanel;
        }
        
        /// <summary>
        /// Crée un panel de pied de page standard
        /// </summary>
        public static Panel CreateFooterPanel()
        {
            Panel footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                BackColor = UIColors.LightGrayColor
            };
            
            Label lblFooter = new Label
            {
                Text = "TransConnect - Projet Étudiant - 2025",
                Font = UIFonts.TinyText,
                ForeColor = UIColors.MediumGrayColor,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            footerPanel.Controls.Add(lblFooter);
            
            return footerPanel;
        }
        
        /// <summary>
        /// Crée un bouton moderne avec le style spécifié
        /// </summary>
        public static Button CreateButton(string text, Color backgroundColor, bool isPrimary = true)
        {
            Button btn = new Button
            {
                Text = text,
                BackColor = backgroundColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = isPrimary ? new Size(150, 30) : new Size(120, 30),
                Font = UIFonts.SmallText
            };
            
            btn.FlatAppearance.BorderSize = 0;
            
            return btn;
        }
        
        /// <summary>
        /// Crée un panel de carte avec un style moderne
        /// </summary>
        public static Panel CreateCard(Padding? padding = null)
        {
            Panel card = new Panel
            {
                BackColor = UIColors.CardColor,
                BorderStyle = BorderStyle.None,
                Padding = padding ?? new Padding(15)
            };
            
            // Ajouter ombre et bordure arrondie
            card.Paint += (sender, e) => 
            {
                Control control = sender as Control;
                Graphics g = e.Graphics;
                
                Rectangle rect = new Rectangle(0, 0, control.Width - 1, control.Height - 1);
                using (Pen pen = new Pen(Color.FromArgb(20, 0, 0, 0), 1))
                {
                    g.DrawRectangle(pen, rect);
                }
                
                // Dessiner les pixels d'ombre aux coins
                g.DrawLine(new Pen(Color.FromArgb(10, 0, 0, 0), 1), 1, control.Height - 1, control.Width - 1, control.Height - 1);
                g.DrawLine(new Pen(Color.FromArgb(10, 0, 0, 0), 1), control.Width - 1, 1, control.Width - 1, control.Height - 1);
            };
            
            return card;
        }
        
        /// <summary>
        /// Crée un groupe d'onglets personnalisés
        /// </summary>
        public static Panel CreateTabPanel(string[] tabNames, EventHandler<string> tabChangedHandler)
        {
            Panel tabContainer = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = UIColors.CardColor,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            // Créer les onglets
            int tabWidth = 150;
            int x = 0;
            
            foreach (string tabName in tabNames)
            {
                Button tabButton = new Button
                {
                    Text = tabName,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.White,
                    ForeColor = UIColors.MediumGrayColor,
                    Size = new Size(tabWidth, 48),
                    Location = new Point(x, 0),
                    Font = UIFonts.Body,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                
                tabButton.FlatAppearance.BorderSize = 0;
                tabButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
                
                // Gestionnaire d'événements
                tabButton.Click += (sender, e) => 
                {
                    // Réinitialiser tous les onglets
                    foreach (Control c in tabContainer.Controls)
                    {
                        if (c is Button btn)
                        {
                            btn.BackColor = Color.White;
                            btn.ForeColor = UIColors.MediumGrayColor;
                            btn.Font = UIFonts.Body;
                            
                            // Supprimer indicateur actif si présent
                            foreach (Control child in btn.Controls)
                            {
                                if (child is Panel)
                                {
                                    btn.Controls.Remove(child);
                                    child.Dispose();
                                }
                            }
                        }
                    }
                    
                    // Activer l'onglet sélectionné
                    Button selectedTab = (Button)sender;
                    selectedTab.ForeColor = UIColors.PrimaryColor;
                    selectedTab.Font = UIFonts.CreateFont(UIFonts.BodySize, FontStyle.Bold);
                    
                    // Ajouter indicateur actif
                    Panel activeIndicator = new Panel
                    {
                        BackColor = UIColors.PrimaryColor,
                        Size = new Size(tabWidth, 3),
                        Location = new Point(0, 45)
                    };
                    
                    selectedTab.Controls.Add(activeIndicator);
                    
                    // Notifier du changement d'onglet
                    tabChangedHandler?.Invoke(selectedTab, tabButton.Text);
                };
                
                tabContainer.Controls.Add(tabButton);
                x += tabWidth;
            }
            
            // Activer le premier onglet par défaut
            if (tabContainer.Controls.Count > 0 && tabContainer.Controls[0] is Button firstTab)
            {
                firstTab.PerformClick();
            }
            
            return tabContainer;
        }
        
        /// <summary>
        /// Applique un style de recherche moderne à une zone de recherche
        /// </summary>
        public static void ApplySearchBoxStyle(TextBox textBox)
        {
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = UIFonts.Body;
            
            Panel searchIconPanel = new Panel
            {
                Size = new Size(30, textBox.Height),
                Location = new Point(0, 0),
                BackColor = Color.Transparent
            };
            
            // Dessin de l'icône de recherche
            searchIconPanel.Paint += (sender, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Dessiner la loupe
                using (Pen p = new Pen(UIColors.MediumGrayColor, 1.5f))
                {
                    g.DrawEllipse(p, 8, 6, 12, 12);
                    g.DrawLine(p, 17, 17, 22, 22);
                }
            };
            
            textBox.Controls.Add(searchIconPanel);
            textBox.Padding = new Padding(30, 0, 0, 0);
        }
    }
}