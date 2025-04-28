using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransConnect.Models.Graphe
{
    public class Graphe 
    {
        #region Propriétés
        Noeud racine;
        List<Noeud> noeuds = new List<Noeud>();
        #endregion

        #region Constructeurs
        public Graphe(Object racine)
        {
            this.racine = new Noeud(racine);
        }
        public Graphe(Noeud racine)
        {
            this.racine = racine;
            noeuds.Add(racine);
        }
        public Graphe(Noeud racine, List<Noeud> noeuds)
        {
            this.racine = racine;
            this.noeuds = noeuds;
        }
        #endregion

        #region Getters et Setters
        public Noeud Racine
        {
            get { return racine; }
            set { racine = value; }
        }
        public List<Noeud> Noeuds
        {
            get { return noeuds; }
            set { noeuds = value; }
        }
        #endregion

        #region Méthodes
        public void AjouterNoeud(Noeud noeud)
        {
            noeuds.Add(noeud);
        }
        public void SupprimerNoeud(Noeud noeud)
        {
            noeuds.Remove(noeud);
        }

        public void AfficherGraphe()
        {
            AffichageGrapheConsole affichage = new AffichageGrapheConsole(this);
            affichage.AfficherGraphe();
        }
        #endregion

    }

    //Utilisation D'une IA pour générer la classe AffichageGrapheConsole
    public class AffichageGrapheConsole
    {
        private Graphe _graphe;
        
        public AffichageGrapheConsole(Graphe graphe)
        {
            _graphe = graphe;
        }
        public void AfficherGraphe()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                VISUALISATION DU GRAPHE                    ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            Console.WriteLine($"\nLe graphe contient {_graphe.Noeuds.Count} noeuds.");
            Console.WriteLine($"Noeud racine: {_graphe.Racine.Id} ({_graphe.Racine})");
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                        NOEUDS                             ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            foreach (var noeud in _graphe.Noeuds)
            {
                Console.WriteLine($"• Noeud {noeud.Id}: {noeud}");
            }
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                        LIENS                              ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            var liensAffiches = new HashSet<string>();
            foreach (var noeud in _graphe.Noeuds)
            {
                foreach (var lien in noeud.Liens)
                {
                    string cleLien = GetCleLien(lien);
                    if (!liensAffiches.Contains(cleLien))
                    {
                        liensAffiches.Add(cleLien);
                        AfficherLien(lien);
                    }
                }
            }
            
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║             REPRÉSENTATION HIÉRARCHIQUE                   ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            AfficherRepresentationHierarchique();
        }
        
        private string GetCleLien(Lien lien)
        {
            if (lien.Oriente == null)
            {
                int min = Math.Min(lien.Noeud1.Id, lien.Noeud2.Id);
                int max = Math.Max(lien.Noeud1.Id, lien.Noeud2.Id);
                return $"{min}-{max}";
            }
            else
            {
                int source = lien.Oriente == true ? lien.Noeud1.Id : lien.Noeud2.Id;
                int destination = lien.Oriente == true ? lien.Noeud2.Id : lien.Noeud1.Id;
                return $"{source}->{destination}";
            }
        }

        private void AfficherLien(Lien lien)
        {
            if (lien.Oriente == true)
            {
                Console.Write($"• Noeud {lien.Noeud1.Id} ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"──{(lien.Valeur.HasValue ? lien.Valeur.Value.ToString("F1") + "km" : "")}──> ");
                Console.ResetColor();
                Console.WriteLine($"Noeud {lien.Noeud2.Id}");
            }
            else if (lien.Oriente == false)
            {
                Console.Write($"• Noeud {lien.Noeud2.Id} ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"──{(lien.Valeur.HasValue ? lien.Valeur.Value.ToString("F1") + "km" : "")}──> ");
                Console.ResetColor();
                Console.WriteLine($"Noeud {lien.Noeud1.Id}");
            }
            else
            {
                Console.Write($"• Noeud {lien.Noeud1.Id} ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"────{(lien.Valeur.HasValue ? lien.Valeur.Value.ToString("F1") + "km" : "")}──── ");
                Console.ResetColor();
                Console.WriteLine($"Noeud {lien.Noeud2.Id}");
            }
        }
        
        private void AfficherRepresentationHierarchique()
        {
            var noeudsVisites = new HashSet<int>();
            
            AfficherArbreRecursif(_graphe.Racine, "", true, noeudsVisites);
        }
        
        private void AfficherArbreRecursif(Noeud noeud, string prefixe, bool estDernierFils, HashSet<int> noeudsVisites)
        {
            if (noeudsVisites.Contains(noeud.Id))
            {
                Console.Write(prefixe);
                if (estDernierFils)
                {
                    Console.Write("└── ");
                }
                else
                {
                    Console.Write("├── ");
                }
                
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"Noeud {noeud.Id} (déjà visité)");
                Console.ResetColor();
                return;
            }
            
            // Marque le noeud comme visité
            noeudsVisites.Add(noeud.Id);
            
            // Affiche le noeud actuel
            Console.Write(prefixe);
            if (estDernierFils)
            {
                Console.Write("└── ");
            }
            else
            {
                Console.Write("├── ");
            }
            
            Console.WriteLine($"Noeud {noeud.Id} [{noeud}]");
            
            // Trouve tous les voisins directs du noeud actuel
            List<Tuple<Noeud, Lien>> voisins = new List<Tuple<Noeud, Lien>>();
            foreach (var lien in noeud.Liens)
            {
                // Pour les liens non orientés ou sortants, prend l'autre noeud
                if (lien.Noeud1.Id == noeud.Id && (lien.Oriente == null || lien.Oriente == true))
                {
                    voisins.Add(new Tuple<Noeud, Lien>(lien.Noeud2, lien));
                }
                // Pour les liens entrants, prend le noeud source
                else if (lien.Noeud2.Id == noeud.Id && (lien.Oriente == null || lien.Oriente == false))
                {
                    voisins.Add(new Tuple<Noeud, Lien>(lien.Noeud1, lien));
                }
            }
            
            // Préfixe pour les enfants
            string nouveauPrefixe = prefixe;
            if (estDernierFils)
            {
                nouveauPrefixe += "    ";
            }
            else
            {
                nouveauPrefixe += "│   ";
            }
            
            // Parcourt récursivement les voisins
            for (int i = 0; i < voisins.Count; i++)
            {
                bool estDernier = (i == voisins.Count - 1);
                AfficherArbreRecursif(voisins[i].Item1, nouveauPrefixe, estDernier, new HashSet<int>(noeudsVisites));
            }
        }
        
        public void AfficherChemin(List<Noeud> chemin)
        {
            if (chemin == null || chemin.Count < 2)
            {
                Console.WriteLine("Chemin invalide ou trop court pour être affiché.");
                return;
            }
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                 VISUALISATION DU CHEMIN                   ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            Console.WriteLine($"\nChemin de {chemin[0].Id} ({chemin[0]}) à {chemin[chemin.Count - 1].Id} ({chemin[chemin.Count - 1]}) :");
            Console.WriteLine();
            
            double distanceTotale = 0;
            
            for (int i = 0; i < chemin.Count - 1; i++)
            {
                Lien? lienEntreSommets = TrouverLienEntreSommets(chemin[i], chemin[i + 1]);
                double distance = lienEntreSommets?.Valeur ?? 0;
                distanceTotale += distance;
                
                Console.Write($"Noeud {chemin[i].Id} ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"──({distance:F1} km)──> ");
                Console.ResetColor();
                Console.WriteLine($"Noeud {chemin[i + 1].Id}");
            }
            
            Console.WriteLine($"\nDistance totale du chemin : {distanceTotale:F1} km");
        }
        
        private Lien? TrouverLienEntreSommets(Noeud noeud1, Noeud noeud2)
        {
            foreach (var lien in noeud1.Liens)
            {
                if ((lien.Noeud1.Id == noeud1.Id && lien.Noeud2.Id == noeud2.Id) ||
                    (lien.Noeud1.Id == noeud2.Id && lien.Noeud2.Id == noeud1.Id))
                {
                    return lien;
                }
            }
            return null;
        }
    }
}