using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TransConnect.Models.Graphe;

namespace TransConnect.UI
{
    public class GrapheVisualiseur
    {
        private Graphe _graphe;
        private Dictionary<int, PointF> _positionsNoeuds;
        private const int RAYON_NOEUD = 20;
        private const int MARGE = 50;
        private const int LARGEUR = 1200;
        private const int HAUTEUR = 800;

        public GrapheVisualiseur(Graphe graphe)
        {
            _graphe = graphe;
            _positionsNoeuds = new Dictionary<int, PointF>();
        }

        public void AfficherGraphe()
        {

            Form formulaire = new Form
            {
                Text = "Visualisation du graphe des villes de France",
                Size = new Size(LARGEUR, HAUTEUR),
                BackColor = Color.White,
                StartPosition = FormStartPosition.CenterScreen
            };

            formulaire.Paint += (sender, e) => DessinerGraphe(e.Graphics);
            
            // Calcul des positions des villes
            CalculerPositionsNoeuds();

            Application.Run(formulaire);
        }


        private void CalculerPositionsNoeuds()
        {
            _positionsNoeuds.Clear();

            // Placement circulaire, avec Paris au centre
            float rayonCercle = Math.Min(LARGEUR, HAUTEUR) / 2 - MARGE * 2;
            float centreX = LARGEUR / 2;
            float centreY = HAUTEUR / 2;

            // Placer Paris au centre s'il existe
            Noeud paris = null;
            foreach (var noeud in _graphe.Noeuds)
            {
                if (noeud.Entite.ToString().Equals("Paris", StringComparison.OrdinalIgnoreCase))
                {
                    paris = noeud;
                    _positionsNoeuds[paris.Id] = new PointF(centreX, centreY);
                    Console.WriteLine($"Position de Paris (ID {paris.Id}) définie au centre: ({centreX}, {centreY})");
                    break;
                }
            }

            // Disposer les autres villes en cercle
            List<Noeud> autresVilles = _graphe.Noeuds
                .Where(n => n != paris)
                .ToList();

            Console.WriteLine($"Nombre d'autres villes à positionner: {autresVilles.Count}");
            
            for (int i = 0; i < autresVilles.Count; i++)
            {
                double angle = 2 * Math.PI * i / autresVilles.Count;
                float x = centreX + (float)(rayonCercle * Math.Cos(angle));
                float y = centreY + (float)(rayonCercle * Math.Sin(angle));
                
                _positionsNoeuds[autresVilles[i].Id] = new PointF(x, y);
                Console.WriteLine($"Position de {autresVilles[i]} (ID {autresVilles[i].Id}) définie à: ({x}, {y})");
            }
        }

        private void DessinerGraphe(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Dessiner les liens d'abord
            DessinerLiens(g);
            
            // Puis dessiner les noeuds
            DessinerNoeuds(g);

            // Ajouter une légende
            DessinerLegende(g);
        }

        private void DessinerLiens(Graphics g)
        {
            // Trier les liens par distance pour une meilleure visibilité
            var liensTriesParDistance = _graphe.Liens
                .OrderByDescending(l => l.Valeur)
                .ToList();

            foreach (var lien in liensTriesParDistance)
            {
                if (!_positionsNoeuds.ContainsKey(lien.Noeud1.Id) || 
                    !_positionsNoeuds.ContainsKey(lien.Noeud2.Id))
                {
                    Console.WriteLine($"Erreur: Lien entre nœuds {lien.Noeud1.Id} et {lien.Noeud2.Id} ignoré car un nœud n'a pas de position définie");
                    continue;
                }

                PointF pos1 = _positionsNoeuds[lien.Noeud1.Id];
                PointF pos2 = _positionsNoeuds[lien.Noeud2.Id];

                // Couleur et épaisseur basées sur la distance
                Color couleur;
                float epaisseur = 2;
                
                if (lien.Valeur.HasValue)
                {
                    double distance = lien.Valeur.Value;
                    
                    // Courts trajets (moins de 300 km)
                    if (distance < 300)
                    {
                        couleur = Color.Red;
                        epaisseur = 4;
                    }
                    // Trajets moyens (300-600 km)
                    else if (distance < 600)
                    {
                        couleur = Color.Purple;
                        epaisseur = 3;
                    }
                    // Longs trajets (plus de 600 km)
                    else
                    {
                        couleur = Color.Blue;
                        epaisseur = 2;
                    }
                }
                else
                {
                    couleur = Color.Gray;
                }

                // Dessiner la ligne du lien
                using (Pen pen = new Pen(couleur, epaisseur))
                {
                    g.DrawLine(pen, pos1, pos2);
                }

                // Afficher la distance
                if (lien.Valeur.HasValue)
                {
                    string distance = $"{lien.Valeur:F0} km";
                    PointF posMilieu = new PointF(
                        (pos1.X + pos2.X) / 2,
                        (pos1.Y + pos2.Y) / 2
                    );

                    // Fond blanc pour le texte
                    using (Brush brush = new SolidBrush(Color.White))
                    {
                        g.FillEllipse(brush, posMilieu.X - 25, posMilieu.Y - 10, 50, 20);
                    }

                    // Texte de distance
                    using (Font font = new Font("Arial", 8))
                    using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(distance, font, Brushes.Black, posMilieu, sf);
                    }
                }
            }
        }

        private void DessinerNoeuds(Graphics g)
        {
            foreach (var noeud in _graphe.Noeuds)
            {
                if (!_positionsNoeuds.ContainsKey(noeud.Id))
                {
                    Console.WriteLine($"Erreur: Nœud {noeud.Id} ({noeud}) ignoré car aucune position n'est définie");
                    continue;
                }

                PointF position = _positionsNoeuds[noeud.Id];
                
                // Vérifier si ce nœud est Paris
                bool estParis = noeud.Entite.ToString().Equals("Paris", StringComparison.OrdinalIgnoreCase);
                
                // Couleur et taille en fonction de la ville
                Brush noeudBrush = estParis ? Brushes.Gold : Brushes.LightBlue;
                int rayon = estParis ? RAYON_NOEUD + 5 : RAYON_NOEUD;
                
                // Dessiner le cercle
                g.FillEllipse(noeudBrush, 
                    position.X - rayon, 
                    position.Y - rayon, 
                    rayon * 2, 
                    rayon * 2);
                
                Pen bordurePen = estParis ? new Pen(Color.DarkGoldenrod, 2) : Pens.Black;
                g.DrawEllipse(bordurePen, 
                    position.X - rayon, 
                    position.Y - rayon, 
                    rayon * 2, 
                    rayon * 2);
                
                if (estParis && bordurePen != Pens.Black)
                    bordurePen.Dispose();

                // Nom de la ville
                string nomVille = noeud.Entite.ToString();
                using (Font font = new Font("Arial", estParis ? 12 : 10, FontStyle.Bold))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(nomVille, font, Brushes.Black, position, sf);
                }
                
                if (estParis)
                {
                    Console.WriteLine($"Paris dessiné à la position: ({position.X}, {position.Y})");
                }
            }
        }

        private void DessinerLegende(Graphics g)
        {
            int y = 20;
            int x = 20;
            
            // Titre
            using (Font titleFont = new Font("Arial", 14, FontStyle.Bold))
            {
                g.DrawString("Carte des distances entre villes françaises", titleFont, Brushes.Black, new PointF(x, y));
            }
            
            y += 30;
            
            // Légende des couleurs
            using (Font font = new Font("Arial", 10))
            {
                g.DrawString("Distances:", font, Brushes.Black, new PointF(x, y));
                y += 20;
                
                // Courte distance (rouge)
                g.DrawLine(new Pen(Color.Red, 4), x, y + 5, x + 30, y + 5);
                g.DrawString("Courte distance", font, Brushes.Black, new PointF(x + 40, y));
                y += 20;
                
                // Distance moyenne (violet)
                g.DrawLine(new Pen(Color.Purple, 3), x, y + 5, x + 30, y + 5);
                g.DrawString("Distance moyenne", font, Brushes.Black, new PointF(x + 40, y));
                y += 20;
                
                // Longue distance (bleu)
                g.DrawLine(new Pen(Color.Blue, 2), x, y + 5, x + 30, y + 5);
                g.DrawString("Longue distance", font, Brushes.Black, new PointF(x + 40, y));
                y += 30;
                
                // Paris
                g.FillEllipse(Brushes.Gold, x, y, RAYON_NOEUD * 2, RAYON_NOEUD * 2);
                g.DrawEllipse(new Pen(Color.DarkGoldenrod, 2), x, y, RAYON_NOEUD * 2, RAYON_NOEUD * 2);
                g.DrawString("Paris (Capitale)", font, Brushes.Black, new PointF(x + 50, y + 10));
                y += 30;
                
                // Autres villes
                g.FillEllipse(Brushes.LightBlue, x, y, RAYON_NOEUD * 2, RAYON_NOEUD * 2);
                g.DrawEllipse(Pens.Black, x, y, RAYON_NOEUD * 2, RAYON_NOEUD * 2);
                g.DrawString("Autres villes", font, Brushes.Black, new PointF(x + 50, y + 10));
            }
        }
    }
}