using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Transconnect.Models;
using Transconnect.Models.Graphe;

namespace Transconnect.UI
{
    public class OrganigrammeVisualiseur
    {
        private Graphe _graphe;
        private Dictionary<int, PointF> _positionsNoeuds;
        private Dictionary<int, int> _niveauxHierarchiques;
        private Dictionary<string, List<Noeud>> _noeudsParDepartement;
        private int _maxX = 0, _maxY = 0;
        
        // Dimensions ajustées pour une meilleure lisibilité
        private const int LARGEUR_NOEUD = 120;
        private const int HAUTEUR_NOEUD = 40;
        private const int ESPACEMENT_H = 40;
        private const int ESPACEMENT_V = 90;
        private const int MARGE = 50;
        private const int LARGEUR = 1200;
        private const int HAUTEUR = 800;
        
        // Groupes de départements pour un meilleur alignement
        private Dictionary<string, string> _departementParPoste = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Directeur General", "Direction" },
            { "Directrice Commerciale", "Commercial" },
            { "Commercial", "Commercial" },
            { "Commerciale", "Commercial" },
            { "Directeur des Operations", "Operations" },
            { "Chef d'Equipe", "Operations" },
            { "Chauffeur", "Operations" },
            { "Directrice des RH", "RH" },
            { "Formation", "RH" },
            { "Contrats", "RH" },
            { "Directeur Financier", "Finance" },
            { "Direction Comptable", "Finance" },
            { "Comptable", "Finance" },
            { "Controleur de Gestion", "Finance" },
            { "Assistante RH", "RH" }
        };
        
        // Couleurs par département
        private Dictionary<string, Color> _couleursPostes = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase)
        {
            { "Directeur General", Color.Gold },
            { "Directrice Commerciale", Color.LightSalmon },
            { "Commercial", Color.LightSalmon },
            { "Commerciale", Color.LightSalmon },
            { "Directeur des Operations", Color.LightBlue },
            { "Chef d'Equipe", Color.LightSkyBlue },
            { "Chauffeur", Color.PowderBlue },
            { "Directrice des RH", Color.LightGreen },
            { "Formation", Color.PaleGreen },
            { "Contrats", Color.PaleGreen },
            { "Directeur Financier", Color.LightPink },
            { "Direction Comptable", Color.Pink },
            { "Comptable", Color.Pink },
            { "Controleur de Gestion", Color.LightPink },
            { "Assistante RH", Color.PaleGreen }
        };

        // Dictionnaire pour stocker l'état d'expansion des départements
        private Dictionary<string, bool> _departementExpanded = new Dictionary<string, bool>
        {
            { "Direction", true },
            { "Commercial", true },
            { "Operations", true },
            { "RH", true },
            { "Finance", true }
        };

        public OrganigrammeVisualiseur(Graphe graphe)
        {
            _graphe = graphe;
            _positionsNoeuds = new Dictionary<int, PointF>();
            _niveauxHierarchiques = new Dictionary<int, int>();
            _noeudsParDepartement = new Dictionary<string, List<Noeud>>();
        }

        public void AfficherOrganigramme()
        {
            // Calculer les positions des noeuds
            CalculerPositionsNoeuds();

            // Créer le formulaire principal
            Form formulaire = new Form
            {
                Text = "Organigramme TransConnect",
                Size = new Size(LARGEUR, HAUTEUR),
                BackColor = Color.White,
                StartPosition = FormStartPosition.CenterScreen
            };
            
            // Créer un panel de contrôles en haut
            Panel controlPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.WhiteSmoke
            };
            formulaire.Controls.Add(controlPanel);
            
            // Créer un panel personnalisé pour l'organigramme
            OrgPanelWithZoom orgPanel = new OrgPanelWithZoom(this);
            orgPanel.Dock = DockStyle.Fill;
            formulaire.Controls.Add(orgPanel);
            
            // Bouton de zoom +
            Button btnZoomIn = new Button
            {
                Text = "Zoom +",
                Location = new Point(10, 10),
                Size = new Size(80, 30)
            };
            btnZoomIn.Click += (s, e) => 
            {
                orgPanel.ZoomIn();
            };
            controlPanel.Controls.Add(btnZoomIn);
            
            // Bouton de zoom -
            Button btnZoomOut = new Button
            {
                Text = "Zoom -",
                Location = new Point(100, 10),
                Size = new Size(80, 30)
            };
            btnZoomOut.Click += (s, e) => 
            {
                orgPanel.ZoomOut();
            };
            controlPanel.Controls.Add(btnZoomOut);
            
            // Filtres par département
            int xPos = 200;
            foreach (string departement in new[] { "Direction", "Commercial", "Operations", "RH", "Finance" })
            {
                CheckBox chkDept = new CheckBox
                {
                    Text = departement,
                    Checked = _departementExpanded[departement],
                    Location = new Point(xPos, 15),
                    AutoSize = true
                };
                chkDept.CheckedChanged += (s, e) => 
                {
                    _departementExpanded[departement] = chkDept.Checked;
                    CalculerPositionsNoeuds();
                    orgPanel.Refresh();
                };
                controlPanel.Controls.Add(chkDept);
                xPos += 100;
            }
            
            // Bouton pour réinitialiser le zoom et la position
            Button btnReset = new Button
            {
                Text = "Réinitialiser vue",
                Location = new Point(xPos + 20, 10),
                Size = new Size(120, 30)
            };
            btnReset.Click += (s, e) => 
            {
                orgPanel.ResetView();
            };
            controlPanel.Controls.Add(btnReset);
            
            // Ajout des instructions
            Label lblInstructions = new Label
            {
                Text = "Utilisez les boutons pour zoomer ou faire défiler avec la souris",
                Location = new Point(xPos + 150, 15),
                AutoSize = true,
                ForeColor = Color.DarkSlateGray
            };
            controlPanel.Controls.Add(lblInstructions);
            
            formulaire.ShowDialog();
        }

        private void CalculerPositionsNoeuds()
        {
            _positionsNoeuds.Clear();
            _niveauxHierarchiques.Clear();
            _noeudsParDepartement.Clear();
            _maxX = 0;
            _maxY = 0;

            // Déterminer les niveaux hiérarchiques pour chaque noeud
            if (_graphe.Racine != null)
            {
                CalculerNiveauxHierarchiques(_graphe.Racine, 0);
            }

            // Regrouper les noeuds par niveau et par département
            Dictionary<int, Dictionary<string, List<Noeud>>> noeudsByNiveauEtDepartement = new Dictionary<int, Dictionary<string, List<Noeud>>>();
            int maxNiveau = 0;

            foreach (var pair in _niveauxHierarchiques)
            {
                int idNoeud = pair.Key;
                int niveau = pair.Value;
                var noeud = _graphe.Noeuds.FirstOrDefault(n => n.Id == idNoeud);
                
                if (noeud == null) continue; // Sécurité supplémentaire
                
                if (noeud.Entite is Salarie salarie)
                {
                    // Déterminer le département
                    string departement = "Autre";
                    if (_departementParPoste.ContainsKey(salarie.Poste))
                        departement = _departementParPoste[salarie.Poste];
                    
                    // Ignorer ce noeud si son département est masqué
                    if (!_departementExpanded.ContainsKey(departement) || !_departementExpanded[departement])
                        continue;
                    
                    // Organiser par niveau
                    if (!noeudsByNiveauEtDepartement.ContainsKey(niveau))
                        noeudsByNiveauEtDepartement[niveau] = new Dictionary<string, List<Noeud>>();
                    
                    // Puis par département
                    if (!noeudsByNiveauEtDepartement[niveau].ContainsKey(departement))
                        noeudsByNiveauEtDepartement[niveau][departement] = new List<Noeud>();
                    
                    noeudsByNiveauEtDepartement[niveau][departement].Add(noeud);
                    maxNiveau = Math.Max(maxNiveau, niveau);
                    
                    // Stocker également dans la liste globale par département
                    if (!_noeudsParDepartement.ContainsKey(departement))
                        _noeudsParDepartement[departement] = new List<Noeud>();
                    
                    _noeudsParDepartement[departement].Add(noeud);
                }
            }

            // Liste d'ordre de départements pour un affichage cohérent
            string[] ordreDepartements = { "Direction", "Commercial", "Operations", "RH", "Finance", "Autre" };

            // Position verticale de départ
            int totalY = MARGE;
            
            // Placement département par département, chacun dans sa propre section
            foreach (string departement in ordreDepartements)
            {
                if (!_noeudsParDepartement.ContainsKey(departement) || 
                    !_departementExpanded.ContainsKey(departement) || 
                    !_departementExpanded[departement])
                    continue;
                
                var noeudsParNiveau = new Dictionary<int, List<Noeud>>();
                
                // Regrouper par niveau dans ce département
                foreach (var noeud in _noeudsParDepartement[departement])
                {
                    int niveau = _niveauxHierarchiques[noeud.Id];
                    if (!noeudsParNiveau.ContainsKey(niveau))
                        noeudsParNiveau[niveau] = new List<Noeud>();
                    
                    noeudsParNiveau[niveau].Add(noeud);
                }
                
                // En-tête de département
                int departementHeaderHeight = 40;
                totalY += departementHeaderHeight;
                
                // Placement par niveau
                foreach (var niveau in noeudsParNiveau.Keys.OrderBy(k => k))
                {
                    var noeuds = noeudsParNiveau[niveau];
                    
                    // Trier par importance: d'abord directeurs, puis chefs, puis autres
                    noeuds = noeuds.OrderBy(n => 
                    {
                        var salarie = (Salarie)n.Entite;
                        return salarie.Poste.Contains("Directeur") || salarie.Poste.Contains("Directrice") 
                            ? 0 : (salarie.Poste.Contains("Chef") ? 1 : 2);
                    }).ToList();
                    
                    // Calcul de la largeur totale pour ce niveau
                    int nombreNoeuds = noeuds.Count;
                    int largeurDisponible = LARGEUR - (2 * MARGE);
                    int largeurTotale = nombreNoeuds * LARGEUR_NOEUD + (nombreNoeuds - 1) * ESPACEMENT_H;
                    int startX = MARGE + (largeurDisponible - largeurTotale) / 2;
                    
                    if (startX < MARGE) startX = MARGE;
                    
                    // Placement horizontal
                    for (int i = 0; i < nombreNoeuds; i++)
                    {
                        var noeud = noeuds[i];
                        int posX = startX + i * (LARGEUR_NOEUD + ESPACEMENT_H);
                        int posY = totalY;
                        
                        _positionsNoeuds[noeud.Id] = new Point(posX, posY);
                        
                        // Mettre à jour les dimensions maximales
                        _maxX = Math.Max(_maxX, posX + LARGEUR_NOEUD);
                    }
                    
                    totalY += HAUTEUR_NOEUD + ESPACEMENT_V;
                }
                
                // Espacement entre départements
                totalY += 20;
            }
            
            _maxY = totalY;
        }

        private void CalculerNiveauxHierarchiques(Noeud noeud, int niveau)
        {
            _niveauxHierarchiques[noeud.Id] = niveau;
            
            // Trouver tous les subordonnés directs
            var subordonnes = new List<Noeud>();
            
            foreach (var lien in _graphe.Liens)
            {
                // Chef -> Subordonné
                if (lien.Noeud1.Id == noeud.Id && lien.Oriente == true)
                {
                    subordonnes.Add(lien.Noeud2);
                }
                // Subordonné <- Chef (lien inversé)
                else if (lien.Noeud2.Id == noeud.Id && lien.Oriente == false)
                {
                    subordonnes.Add(lien.Noeud1);
                }
            }
            
            // Trier les subordonnés par département puis par poste
            subordonnes = subordonnes
                .OrderBy(s => {
                    var salarie = (Salarie)s.Entite;
                    string departement = "Autre";
                    if (_departementParPoste.ContainsKey(salarie.Poste))
                        departement = _departementParPoste[salarie.Poste];
                    return departement;
                })
                .ThenBy(s => ((Salarie)s.Entite).Poste)
                .ToList();

            // Calculer récursivement pour les subordonnés
            foreach (var subordonne in subordonnes)
            {
                // Éviter les cycles
                if (!_niveauxHierarchiques.ContainsKey(subordonne.Id))
                {
                    CalculerNiveauxHierarchiques(subordonne, niveau + 1);
                }
            }
        }

        public void DessinerOrganigramme(Graphics g, float zoomFactor, Point scrollPosition)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Appliquer la transformation pour le zoom et le défilement
            g.TranslateTransform(-scrollPosition.X, -scrollPosition.Y);
            g.ScaleTransform(zoomFactor, zoomFactor);
            
            // Dessiner les en-têtes de départements
            DessinerEnTetesDepartements(g);
            
            // Dessiner les liens d'abord
            DessinerLiens(g);
            
            // Puis dessiner les noeuds des salariés
            DessinerNoeudsSalaries(g);
            
            // Ajouter une légende
            DessinerLegende(g);
        }

        private void DessinerEnTetesDepartements(Graphics g)
        {
            int y = MARGE;
            
            foreach (string departement in new[] { "Direction", "Commercial", "Operations", "RH", "Finance" })
            {
                if (!_noeudsParDepartement.ContainsKey(departement) || 
                    !_departementExpanded.ContainsKey(departement) || 
                    !_departementExpanded[departement])
                    continue;
                
                // Couleur de fond pour l'en-tête
                Color couleurFond = Color.FromArgb(240, 240, 240);
                switch (departement)
                {
                    case "Direction": couleurFond = Color.FromArgb(255, 250, 240); break;
                    case "Commercial": couleurFond = Color.FromArgb(255, 240, 240); break;
                    case "Operations": couleurFond = Color.FromArgb(240, 240, 255); break;
                    case "RH": couleurFond = Color.FromArgb(240, 255, 240); break;
                    case "Finance": couleurFond = Color.FromArgb(255, 240, 255); break;
                }
                
                // Dessiner l'en-tête
                using (var brush = new SolidBrush(couleurFond))
                {
                    g.FillRectangle(brush, MARGE, y, LARGEUR - (2 * MARGE), 40);
                }
                
                using (var pen = new Pen(Color.Gray))
                {
                    g.DrawRectangle(pen, MARGE, y, LARGEUR - (2 * MARGE), 40);
                }
                
                // Texte de l'en-tête
                using (var font = new Font("Arial", 12, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.Black))
                {
                    g.DrawString(departement, font, brush, MARGE + 10, y + 10);
                }
                
                // Passer à la section suivante
                y += 40;
                
                // Calculer la hauteur de cette section
                int nombreNiveaux = _noeudsParDepartement[departement]
                    .Select(n => _niveauxHierarchiques[n.Id])
                    .Distinct()
                    .Count();
                
                y += nombreNiveaux * (HAUTEUR_NOEUD + ESPACEMENT_V);
                
                // Ajouter un espacement supplémentaire entre les départements
                y += 20;
            }
        }

        private void DessinerLiens(Graphics g)
        {
            // Créer un dictionnaire des liens visibles (les deux nœuds sont visibles)
            var liensVisibles = new List<Tuple<PointF, PointF, bool>>();
            
            foreach (var lien in _graphe.Liens)
            {
                if (!_positionsNoeuds.ContainsKey(lien.Noeud1.Id) || 
                    !_positionsNoeuds.ContainsKey(lien.Noeud2.Id))
                    continue;

                PointF posSup, posSub;
                bool directSupervision;
                
                // Déterminer qui est le supérieur et qui est le subordonné
                if (lien.Oriente == true)
                {
                    posSup = _positionsNoeuds[lien.Noeud1.Id];
                    posSub = _positionsNoeuds[lien.Noeud2.Id];
                    directSupervision = true;
                }
                else
                {
                    posSup = _positionsNoeuds[lien.Noeud2.Id];
                    posSub = _positionsNoeuds[lien.Noeud1.Id];
                    directSupervision = false;
                }
                
                // Ajouter aux liens visibles
                liensVisibles.Add(new Tuple<PointF, PointF, bool>(posSup, posSub, directSupervision));
            }
            
            // Dessiner les liens en fonction de leur type
            foreach (var lienInfo in liensVisibles)
            {
                PointF posSup = lienInfo.Item1;
                PointF posSub = lienInfo.Item2;
                bool isDirect = lienInfo.Item3;
                
                // Points de contrôle pour la courbe
                float midX = (posSup.X + posSub.X) / 2;
                float midY = (posSup.Y + posSub.Y) / 2;
                
                // Point de départ (bas du rectangle du supérieur)
                PointF debut = new PointF(
                    posSup.X + LARGEUR_NOEUD / 2,
                    posSup.Y + HAUTEUR_NOEUD
                );
                
                // Point d'arrivée (haut du rectangle du subordonné)
                PointF fin = new PointF(
                    posSub.X + LARGEUR_NOEUD / 2,
                    posSub.Y
                );
                
                // Utiliser une courbe pour les liens indirects
                if (!isDirect && Math.Abs(posSup.X - posSub.X) > LARGEUR_NOEUD)
                {
                    using (Pen pen = new Pen(Color.FromArgb(150, Color.Gray), 1.5f))
                    {
                        pen.DashStyle = DashStyle.Dot;
                        
                        // Dessiner une courbe de Bézier
                        g.DrawBezier(
                            pen,
                            debut,
                            new PointF(debut.X, debut.Y + 40),
                            new PointF(fin.X, fin.Y - 40),
                            fin
                        );
                    }
                }
                else
                {
                    // Dessiner la ligne hiérarchique avec une flèche
                    using (Pen pen = new Pen(Color.FromArgb(180, Color.Gray), 1.5f))
                    {
                        // Configuration de la flèche
                        pen.CustomEndCap = new AdjustableArrowCap(5, 5);
                        
                        // Dessiner une ligne droite avec un léger offset pour éviter les chevauchements
                        if (Math.Abs(posSup.X - posSub.X) < 10)
                        {
                            // Ligne verticale directe
                            g.DrawLine(pen, debut, fin);
                        }
                        else
                        {
                            // Ligne avec point intermédiaire
                            PointF mid = new PointF(midX, midY);
                            g.DrawLine(pen, debut, mid);
                            g.DrawLine(pen, mid, fin);
                        }
                    }
                }
            }
        }

        private void DessinerNoeudsSalaries(Graphics g)
        {
            foreach (var noeud in _graphe.Noeuds)
            {
                if (!_positionsNoeuds.ContainsKey(noeud.Id))
                    continue;

                PointF position = _positionsNoeuds[noeud.Id];
                
                if (noeud.Entite is Salarie salarie)
                {
                    // Déterminer la couleur en fonction du poste
                    Color couleurFond = Color.LightGray;
                    
                    if (_couleursPostes.ContainsKey(salarie.Poste))
                    {
                        couleurFond = _couleursPostes[salarie.Poste];
                    }
                    
                    // Dessiner le rectangle du salarié
                    Rectangle rect = new Rectangle(
                        (int)position.X, 
                        (int)position.Y, 
                        LARGEUR_NOEUD, 
                        HAUTEUR_NOEUD
                    );
                    
                    // Fond arrondi avec dégradé
                    using (var path = RoundedRectangle(rect, 8))
                    {
                        using (var brush = new LinearGradientBrush(
                            rect, 
                            couleurFond, 
                            Color.FromArgb(255, Math.Max(0, couleurFond.R - 20), Math.Max(0, couleurFond.G - 20), Math.Max(0, couleurFond.B - 20)), 
                            LinearGradientMode.Vertical))
                        {
                            g.FillPath(brush, path);
                        }
                        
                        using (var pen = new Pen(Color.FromArgb(150, Color.Gray), 1))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                    
                    // Texte: Nom et Prénom
                    using (Font fontNom = new Font("Arial", 8, FontStyle.Bold))
                    using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
                    {
                        g.DrawString($"{salarie.Prenom} {salarie.Nom}", fontNom, Brushes.Black, 
                            new RectangleF(position.X, position.Y + 4, LARGEUR_NOEUD, 16), sf);
                    }
                    
                    // Texte: Poste
                    using (Font fontPoste = new Font("Arial", 7))
                    using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
                    {
                        g.DrawString(salarie.Poste, fontPoste, Brushes.Black, 
                            new RectangleF(position.X, position.Y + 22, LARGEUR_NOEUD, 14), sf);
                    }
                }
            }
        }

        private GraphicsPath RoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            
            // Coins arrondis
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            
            return path;
        }

        private void DessinerLegende(Graphics g)
        {
            int x = MARGE;
            int y = _maxY - 130;
            int largeurCarre = 12;
            int hauteurCarre = 12;
            int espacementY = 18;
            
            // Fond de la légende
            using (var brush = new SolidBrush(Color.FromArgb(245, 245, 245)))
            {
                g.FillRectangle(brush, x - 10, y - 10, 250, 140);
            }
            
            using (var pen = new Pen(Color.Gray))
            {
                g.DrawRectangle(pen, x - 10, y - 10, 250, 140);
            }
            
            // Titre
            using (Font fontTitre = new Font("Arial", 10, FontStyle.Bold))
            {
                g.DrawString("Légende des postes", fontTitre, Brushes.Black, x, y);
            }
            
            y += 25;
            
            // Liste des postes pour la légende, dans l'ordre
            string[] postesLegende = {
                "Directeur General",
                "Directrice Commerciale",
                "Commercial/Commerciale",
                "Directeur des Operations",
                "Chef d'Equipe",
                "Chauffeur"
            };
            
            // Afficher la légende dans l'ordre défini
            using (Font fontLegende = new Font("Arial", 8))
            {
                foreach (var poste in postesLegende)
                {
                    Color couleur;
                    
                    // Déterminer la couleur
                    if (_couleursPostes.ContainsKey(poste))
                        couleur = _couleursPostes[poste];
                    else if (poste == "Commercial/Commerciale")
                        couleur = Color.LightSalmon;
                    else
                        couleur = Color.LightGray;
                    
                    // Rectangle de couleur
                    g.FillRectangle(new SolidBrush(couleur), x, y, largeurCarre, hauteurCarre);
                    g.DrawRectangle(Pens.DarkGray, x, y, largeurCarre, hauteurCarre);
                    
                    // Nom du poste
                    g.DrawString(poste, fontLegende, Brushes.Black, x + largeurCarre + 5, y);
                    
                    y += espacementY;
                }
            }
        }
        
        // Accesseurs pour la taille du contenu
        public Size GetContentSize()
        {
            return new Size(_maxX + MARGE, _maxY + MARGE);
        }
    }
    
    // Un panel personnalisé pour gérer le zoom et le défilement
    class OrgPanelWithZoom : Panel
    {
        private OrganigrammeVisualiseur _visualiseur;
        private float _zoomFactor = 1.0f;
        private Point _scrollPosition = new Point(0, 0);
        private Point _dragStart = Point.Empty;
        private bool _isDragging = false;
        
        // Facteurs de zoom min et max
        private const float MIN_ZOOM = 0.3f;
        private const float MAX_ZOOM = 2.0f;
        private const float ZOOM_STEP = 0.1f;
        
        public OrgPanelWithZoom(OrganigrammeVisualiseur visualiseur)
        {
            _visualiseur = visualiseur;
            
            // Configuration du panel
            DoubleBuffered = true;
            BackColor = Color.White;
            AutoScroll = false; // Gérer nous-mêmes le défilement
            
            // Gestionnaires d'événements pour la souris
            MouseDown += Panel_MouseDown;
            MouseMove += Panel_MouseMove;
            MouseUp += Panel_MouseUp;
            MouseWheel += Panel_MouseWheel;
        }
        
        private void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                _dragStart = e.Location;
                Cursor = Cursors.Hand;
            }
        }
        
        private void Panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                // Calculer le déplacement
                int deltaX = e.X - _dragStart.X;
                int deltaY = e.Y - _dragStart.Y;
                
                // Mettre à jour la position de défilement
                _scrollPosition.X = Math.Max(0, _scrollPosition.X - deltaX);
                _scrollPosition.Y = Math.Max(0, _scrollPosition.Y - deltaY);
                
                // Limiter la position de défilement à la taille du contenu
                Size contentSize = GetScaledContentSize();
                _scrollPosition.X = Math.Min(_scrollPosition.X, Math.Max(0, contentSize.Width - ClientSize.Width));
                _scrollPosition.Y = Math.Min(_scrollPosition.Y, Math.Max(0, contentSize.Height - ClientSize.Height));
                
                // Mettre à jour le point de départ pour le prochain mouvement
                _dragStart = e.Location;
                
                // Redessiner
                Invalidate();
            }
        }
        
        private void Panel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = false;
                Cursor = Cursors.Default;
            }
        }
        
        private void Panel_MouseWheel(object sender, MouseEventArgs e)
        {
            // Zoom avec la molette de la souris
            if (ModifierKeys == Keys.Control)
            {
                // Zoom
                if (e.Delta > 0)
                    ZoomIn();
                else
                    ZoomOut();
            }
            else
            {
                // Défilement vertical
                _scrollPosition.Y = Math.Max(0, _scrollPosition.Y - e.Delta);
                
                // Limiter la position de défilement
                Size contentSize = GetScaledContentSize();
                _scrollPosition.Y = Math.Min(_scrollPosition.Y, Math.Max(0, contentSize.Height - ClientSize.Height));
                
                Invalidate();
            }
        }
        
        public void ZoomIn()
        {
            if (_zoomFactor < MAX_ZOOM)
            {
                _zoomFactor += ZOOM_STEP;
                AdjustScrollPositionAfterZoom();
                Invalidate();
            }
        }
        
        public void ZoomOut()
        {
            if (_zoomFactor > MIN_ZOOM)
            {
                _zoomFactor -= ZOOM_STEP;
                AdjustScrollPositionAfterZoom();
                Invalidate();
            }
        }
        
        public void ResetView()
        {
            _zoomFactor = 1.0f;
            _scrollPosition = new Point(0, 0);
            Invalidate();
        }
        
        private void AdjustScrollPositionAfterZoom()
        {
            // Limiter la position de défilement à la taille du contenu après le zoom
            Size contentSize = GetScaledContentSize();
            _scrollPosition.X = Math.Min(_scrollPosition.X, Math.Max(0, contentSize.Width - ClientSize.Width));
            _scrollPosition.Y = Math.Min(_scrollPosition.Y, Math.Max(0, contentSize.Height - ClientSize.Height));
        }
        
        private Size GetScaledContentSize()
        {
            Size contentSize = _visualiseur.GetContentSize();
            return new Size(
                (int)(contentSize.Width * _zoomFactor),
                (int)(contentSize.Height * _zoomFactor)
            );
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            // Dessiner l'organigramme avec le zoom et la position de défilement
            _visualiseur.DessinerOrganigramme(e.Graphics, _zoomFactor, _scrollPosition);
            
            // Afficher des informations de débogage pour le zoom et le défilement si nécessaire
            /*using (Font font = new Font("Arial", 8))
            {
                string debug = $"Zoom: {_zoomFactor:F1}x, Scroll: ({_scrollPosition.X}, {_scrollPosition.Y})";
                e.Graphics.DrawString(debug, font, Brushes.Black, 10, 10);
            }*/
        }
    }
}