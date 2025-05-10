using System;
using System.Collections.Generic;
using System.Drawing;
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
        
        // Dimensions réduites pour un affichage plus compact
        private const int LARGEUR_NOEUD = 150;
        private const int HAUTEUR_NOEUD = 50;
        private const int ESPACEMENT_H = 30;
        private const int ESPACEMENT_V = 70;
        private const int MARGE = 40;
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
            { "Controleur de Gestion", "Finance" }
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
            { "Controleur de Gestion", Color.LightPink }
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

            // Panel pour contenir l'organigramme (pour permettre le défilement)
            Panel panel = new Panel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill,
                AutoScrollMinSize = new Size(_maxX + MARGE, _maxY + MARGE)
            };
            
            panel.Paint += (sender, e) => DessinerOrganigramme(e.Graphics);

            Form formulaire = new Form
            {
                Text = "Organigramme TransConnect",
                Size = new Size(LARGEUR, HAUTEUR),
                BackColor = Color.White,
                StartPosition = FormStartPosition.CenterScreen
            };
            
            formulaire.Controls.Add(panel);
            
            // Utilisation de ShowDialog() au lieu de Application.Run()
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
                var noeud = _graphe.Noeuds.First(n => n.Id == idNoeud);
                
                if (noeud.Entite is Salarie salarie)
                {
                    // Déterminer le département
                    string departement = "Autre";
                    if (_departementParPoste.ContainsKey(salarie.Poste))
                        departement = _departementParPoste[salarie.Poste];
                    
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

            // Calculer les positions horizontales et verticales
            for (int niveau = 0; niveau <= maxNiveau; niveau++)
            {
                if (!noeudsByNiveauEtDepartement.ContainsKey(niveau))
                    continue;

                // Collecter tous les noeuds pour ce niveau, triés par département
                List<Noeud> noeudsNiveau = new List<Noeud>();
                
                foreach (string departement in ordreDepartements)
                {
                    if (noeudsByNiveauEtDepartement[niveau].ContainsKey(departement))
                    {
                        // Trier par poste dans le département pour un affichage cohérent
                        var noeudsParDepartement = noeudsByNiveauEtDepartement[niveau][departement]
                            .OrderBy(n => {
                                var salarie = (Salarie)n.Entite;
                                // Directeur en premier, puis autres postes
                                return salarie.Poste.Contains("Directeur") || salarie.Poste.Contains("Directrice") 
                                    ? 0 : (salarie.Poste.Contains("Chef") ? 1 : 2);
                            })
                            .ToList();
                        
                        noeudsNiveau.AddRange(noeudsParDepartement);
                    }
                }
                
                // Calculer la largeur totale nécessaire pour ce niveau
                int nombreNoeuds = noeudsNiveau.Count;
                int largeurTotale = nombreNoeuds * LARGEUR_NOEUD + (nombreNoeuds - 1) * ESPACEMENT_H;
                int startX = Math.Max(MARGE, (LARGEUR - largeurTotale) / 2);
                
                for (int i = 0; i < nombreNoeuds; i++)
                {
                    var noeud = noeudsNiveau[i];
                    int posX = startX + i * (LARGEUR_NOEUD + ESPACEMENT_H);
                    int posY = MARGE + niveau * (HAUTEUR_NOEUD + ESPACEMENT_V);
                    
                    _positionsNoeuds[noeud.Id] = new Point(posX, posY);
                    
                    // Mettre à jour les dimensions maximales
                    _maxX = Math.Max(_maxX, posX + LARGEUR_NOEUD);
                    _maxY = Math.Max(_maxY, posY + HAUTEUR_NOEUD);
                }
            }
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

        private void DessinerOrganigramme(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            // Dessiner les liens d'abord
            DessinerLiens(g);
            
            // Puis dessiner les noeuds des salariés
            DessinerNoeudsSalaries(g);
            
            // Ajouter une légende
            DessinerLegende(g);
        }

        private void DessinerLiens(Graphics g)
        {
            foreach (var lien in _graphe.Liens)
            {
                if (!_positionsNoeuds.ContainsKey(lien.Noeud1.Id) || 
                    !_positionsNoeuds.ContainsKey(lien.Noeud2.Id))
                    continue;

                PointF posSup, posSub;
                
                // Déterminer qui est le supérieur et qui est le subordonné
                if (lien.Oriente == true)
                {
                    posSup = _positionsNoeuds[lien.Noeud1.Id];
                    posSub = _positionsNoeuds[lien.Noeud2.Id];
                }
                else
                {
                    posSup = _positionsNoeuds[lien.Noeud2.Id];
                    posSub = _positionsNoeuds[lien.Noeud1.Id];
                }

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

                // Dessiner la ligne hiérarchique avec une flèche
                using (Pen pen = new Pen(Color.Gray, 1.5f))
                {
                    // Configuration de la flèche
                    pen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(5, 5);
                    g.DrawLine(pen, debut, fin);
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
                    using (var path = RoundedRectangle(rect, 10))
                    {
                        using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                            rect, 
                            couleurFond, 
                            Color.FromArgb(255, Math.Max(0, couleurFond.R - 30), Math.Max(0, couleurFond.G - 30), Math.Max(0, couleurFond.B - 30)), 
                            System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                        {
                            g.FillPath(brush, path);
                        }
                        
                        using (var pen = new Pen(Color.DarkGray, 1))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                    
                    // Texte: Nom et Prénom
                    using (Font fontNom = new Font("Arial", 9, FontStyle.Bold))
                    using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
                    {
                        g.DrawString($"{salarie.Prenom} {salarie.Nom}", fontNom, Brushes.Black, 
                            new RectangleF(position.X, position.Y + 5, LARGEUR_NOEUD, 18), sf);
                    }
                    
                    // Texte: Poste
                    using (Font fontPoste = new Font("Arial", 8))
                    using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
                    {
                        g.DrawString(salarie.Poste, fontPoste, Brushes.Black, 
                            new RectangleF(position.X, position.Y + 23, LARGEUR_NOEUD, 16), sf);
                    }
                    
                    // Texte: Salaire
                    using (Font fontSalaire = new Font("Arial", 8))
                    using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
                    {
                        g.DrawString($"{salarie.Salaire:C0}", fontSalaire, Brushes.DarkBlue, 
                            new RectangleF(position.X, position.Y + 35, LARGEUR_NOEUD, 15), sf);
                    }
                }
            }
        }

        private System.Drawing.Drawing2D.GraphicsPath RoundedRectangle(Rectangle rect, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            
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
            int x = 20;
            int y = 20;
            int largeurCarre = 12;
            int hauteurCarre = 12;
            int espacementY = 18;
            
            // Titre
            using (Font fontTitre = new Font("Arial", 11, FontStyle.Bold))
            {
                g.DrawString("Légende des départements", fontTitre, Brushes.Black, x, y);
            }
            
            y += 25;
            
            // Liste des postes pour la légende, dans l'ordre
            string[] postesLegende = {
                "Directeur General",
                "Directrice Commerciale",
                "Commercial",
                "Directeur des Operations",
                "Opérations",
                "Directrice des RH",
                "Ressources Humaines",
                "Directeur Financier",
                "Finance"
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
                    else if (poste == "Opérations")
                        couleur = Color.LightSkyBlue;
                    else if (poste == "Ressources Humaines")
                        couleur = Color.PaleGreen;
                    else if (poste == "Finance")
                        couleur = Color.Pink;
                    else
                        couleur = Color.LightGray;
                    
                    // Rectangle de couleur
                    g.FillRectangle(new SolidBrush(couleur), x, y, largeurCarre, hauteurCarre);
                    g.DrawRectangle(Pens.DarkGray, x, y, largeurCarre, hauteurCarre);
                    
                    // Nom du département
                    g.DrawString(poste, fontLegende, Brushes.Black, x + largeurCarre + 5, y);
                    
                    y += espacementY;
                }
            }
        }
    }
}