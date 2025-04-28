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
        List<Lien> liens = new List<Lien>();
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
        public List<Lien> Liens
        {
            get { return liens; }
            set { liens = value; }
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

        public void AjouterLien(Lien lien)
        {
            liens.Add(lien);
            if (!noeuds.Contains(lien.Noeud1))
            {
                noeuds.Add(lien.Noeud1);
            }
            if (!noeuds.Contains(lien.Noeud2))
            {
                noeuds.Add(lien.Noeud2);
            }
        }
        public void SupprimerLien(Lien lien)
        {
            liens.Remove(lien);
        }

        public bool LiensExistants(Lien lien)
        {
            foreach (var l in liens)
            {
                if (l.Noeud1 == lien.Noeud1 && l.Noeud2 == lien.Noeud2)
                {
                    return true;
                }
            }
            return false;
        }

        public void AfficherGraphe()
        {
            AffichageGrapheConsole affichage = new AffichageGrapheConsole(this);
            affichage.AfficherGraphe();
        }

        public Noeud TrouverNoeudVille(string ville)
        {
            foreach (var noeud in noeuds)
            {
                if (noeud.Entite.ToString() == ville)
                {
                    return noeud;
                }
            }
            return null;
        }
        public Noeud TrouverNoeudParSalarieNumeroSS(string numeroSS)
        {
            foreach (var noeud in noeuds)
            {
                if (noeud.Entite is Salarie && ((Salarie)noeud.Entite).NumeroSS == numeroSS)
                {
                    return noeud;
                }
            }
            return null;
        }
        #endregion

    }

    //Utilisation D'une IA pour générer la classe AffichageGrapheConsole
    public class AffichageGrapheConsole
    {
        private Graphe _graphe;
        private bool _estGrapheVilles;
        private bool _estGrapheSalaries;
        
        public AffichageGrapheConsole(Graphe graphe)
        {
            _graphe = graphe;
            
            // Détecte le type de graphe
            if (_graphe.Noeuds.Count > 0)
            {
                var entite = _graphe.Noeuds[0].Entite;
                _estGrapheVilles = entite is string;
                _estGrapheSalaries = entite is Salarie;
            }
        }
        
        public void AfficherGraphe()
        {
            if (_estGrapheVilles)
            {
                AfficherGrapheVilles();
            }
            else if (_estGrapheSalaries)
            {
                AfficherGrapheSalaries();
            }
            else
            {
                AfficherGrapheGenerique();
            }
        }
        
        private void AfficherGrapheGenerique()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔═════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                VISUALISATION DU GRAPHE                      ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            // Affichage du résumé
            Console.WriteLine($"\nLe graphe contient {_graphe.Noeuds.Count} noeuds et {_graphe.Liens.Count} liens.");
            
            // Affichage des noeuds
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n╔═════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                        NOEUDS                               ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            foreach (var noeud in _graphe.Noeuds)
            {
                Console.WriteLine($"• Noeud {noeud.Id}: {noeud}");
            }
            
            // Affichage des liens
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n╔═════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                        LIENS                                ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            foreach (var lien in _graphe.Liens)
            {
                if (lien.Valeur != null)
                {
                    Console.WriteLine($"• {lien.Noeud1} -- {lien.Noeud2} (Valeur: {lien.Valeur})");
                }
                else if (lien.Oriente != null)
                {
                    if (lien.Oriente == true)
                    {
                        Console.WriteLine($"• {lien.Noeud1} --> {lien.Noeud2}");
                    }
                    else
                    {
                        Console.WriteLine($"• {lien.Noeud2} --> {lien.Noeud1}");
                    }
                }
                else
                {
                    Console.WriteLine($"• {lien.Noeud1} -- {lien.Noeud2}");
                }
            }
        }
        
        private void AfficherGrapheVilles()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔═════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║              CARTE DES DISTANCES ENTRE VILLES               ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            // Affichage des statistiques
            Console.WriteLine($"\nLa carte contient {_graphe.Noeuds.Count} villes et {_graphe.Liens.Count} connexions.");
            
            // Affichage de la capitale (racine)
            if (_graphe.Racine != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n══════ CAPITALE: {_graphe.Racine} ══════");
                Console.ResetColor();
                
                // Trouver toutes les connexions directes depuis la capitale
                var liensCapitale = _graphe.Liens
                    .Where(l => l.Noeud1.Id == _graphe.Racine.Id || l.Noeud2.Id == _graphe.Racine.Id)
                    .OrderBy(l => l.Valeur)
                    .ToList();
                
                Console.WriteLine($"La capitale {_graphe.Racine} est connectée à {liensCapitale.Count} autres villes:\n");
                
                foreach (var lien in liensCapitale)
                {
                    string autreVille = lien.Noeud1.Id == _graphe.Racine.Id 
                        ? lien.Noeud2.ToString() 
                        : lien.Noeud1.ToString();
                    
                    double distance = lien.Valeur ?? 0;
                    
                    Console.Write($"• {_graphe.Racine.ToString().PadRight(15)} ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"──({distance,4:F0} km)──> ");
                    Console.ResetColor();
                    Console.WriteLine($"{autreVille}");
                }
            }
            
            // Catégoriser les villes par nombre de connexions
            var villesConnectees = new Dictionary<Noeud, int>();
            foreach (var lien in _graphe.Liens)
            {
                if (!villesConnectees.ContainsKey(lien.Noeud1))
                    villesConnectees[lien.Noeud1] = 0;
                if (!villesConnectees.ContainsKey(lien.Noeud2))
                    villesConnectees[lien.Noeud2] = 0;
                
                villesConnectees[lien.Noeud1]++;
                villesConnectees[lien.Noeud2]++;
            }
            
            // Affichage des villes principales (plus de 5 connexions)
            var villesPrincipales = villesConnectees.Where(v => v.Value > 5)
                                               .OrderByDescending(v => v.Value)
                                               .ToList();
            
            if (villesPrincipales.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n╔═════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                   VILLES PRINCIPALES                        ║");
                Console.WriteLine("╚═════════════════════════════════════════════════════════════╝");
                Console.ResetColor();
                
                foreach (var ville in villesPrincipales)
                {
                    Console.WriteLine($"• {ville.Key} - {ville.Value} connexions");
                }
            }
            
            // Affichage des connexions (autres que celles de la capitale déjà affichées)
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n╔═════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  AUTRES CONNEXIONS                           ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            // Trier les liens par distance
            var liensTriesParDistance = _graphe.Liens
                .OrderBy(l => l.Valeur)
                .ToList();
            
            // Filtrer pour exclure les liens avec la capitale si elle existe
            if (_graphe.Racine != null)
            {
                liensTriesParDistance = liensTriesParDistance
                    .Where(l => l.Noeud1.Id != _graphe.Racine.Id && l.Noeud2.Id != _graphe.Racine.Id)
                    .ToList();
            }
            
            foreach (var lien in liensTriesParDistance)
            {
                string ville1 = lien.Noeud1.ToString();
                string ville2 = lien.Noeud2.ToString();
                double distance = lien.Valeur ?? 0;
                
                Console.Write($"• {ville1.PadRight(15)} ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"──({distance,4:F0} km)──> ");
                Console.ResetColor();
                Console.WriteLine($"{ville2}");
            }
            
            // Affichage du réseau en étoile depuis la capitale
            if (_graphe.Racine != null)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n╔═════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                 RÉSEAU DEPUIS LA CAPITALE                   ║");
                Console.WriteLine("╚═════════════════════════════════════════════════════════════╝");
                Console.ResetColor();
                
                AfficherReseauCapitale();
            }
            
            // Affichage de statistiques
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n╔═════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    STATISTIQUES                             ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            if (_graphe.Liens.Count > 0)
            {
                var distanceMin = _graphe.Liens.Min(l => l.Valeur);
                var distanceMax = _graphe.Liens.Max(l => l.Valeur);
                var distanceMoyenne = _graphe.Liens.Average(l => l.Valeur);
                
                // Trouver les villes les plus proches
                var lienDistanceMin = _graphe.Liens.First(l => l.Valeur == distanceMin);
                // Trouver les villes les plus éloignées
                var lienDistanceMax = _graphe.Liens.First(l => l.Valeur == distanceMax);
                
                Console.WriteLine($"Distance minimale : {distanceMin:F0} km ({lienDistanceMin.Noeud1} - {lienDistanceMin.Noeud2})");
                Console.WriteLine($"Distance maximale : {distanceMax:F0} km ({lienDistanceMax.Noeud1} - {lienDistanceMax.Noeud2})");
                Console.WriteLine($"Distance moyenne : {distanceMoyenne:F0} km");
                
                // Statistiques spécifiques à la capitale si elle existe
                if (_graphe.Racine != null)
                {
                    var liensCapitale = _graphe.Liens
                        .Where(l => l.Noeud1.Id == _graphe.Racine.Id || l.Noeud2.Id == _graphe.Racine.Id);
                    
                    if (liensCapitale.Any())
                    {
                        var distanceMinCapitale = liensCapitale.Min(l => l.Valeur);
                        var distanceMaxCapitale = liensCapitale.Max(l => l.Valeur);
                        var distanceMoyenneCapitale = liensCapitale.Average(l => l.Valeur);
                        
                        Console.WriteLine($"\nDepuis la capitale ({_graphe.Racine}):");
                        Console.WriteLine($"Distance minimale : {distanceMinCapitale:F0} km");
                        Console.WriteLine($"Distance maximale : {distanceMaxCapitale:F0} km");
                        Console.WriteLine($"Distance moyenne : {distanceMoyenneCapitale:F0} km");
                    }
                }
            }
        }
        
        private void AfficherReseauCapitale()
        {
            if (_graphe.Racine == null)
                return;
            
            // Ensemble pour suivre les villes déjà affichées
            var villesAffichees = new HashSet<int>();
            villesAffichees.Add(_graphe.Racine.Id);
            
            // Affiche l'arbre en partant de la capitale
            AfficherReseauCapitaleRecursif(_graphe.Racine, "", true, villesAffichees, 0, 2);
        }
        
        private void AfficherReseauCapitaleRecursif(Noeud noeud, string prefixe, bool estDernier, HashSet<int> villesAffichees, int niveau, int profondeurMax)
        {
            // Limiter la profondeur pour éviter un affichage trop complexe
            if (niveau > profondeurMax)
                return;
            
            // Afficher le noeud actuel (sauf la racine qui est déjà affichée)
            if (niveau > 0)
            {
                Console.WriteLine($"{prefixe}{(estDernier ? "└── " : "├── ")}{noeud}");
            }
            else
            {
                Console.WriteLine($"{noeud} (Capitale)");
            }
            
            // Trouver toutes les villes connectées directement à celle-ci et pas encore affichées
            var liensConnectes = _graphe.Liens
                .Where(l => (l.Noeud1.Id == noeud.Id || l.Noeud2.Id == noeud.Id))
                .OrderBy(l => l.Valeur)
                .ToList();
            
            // Extraire les noeuds connectés
            var noeudsConnectes = new List<Noeud>();
            foreach (var lien in liensConnectes)
            {
                Noeud noeudConnecte = lien.Noeud1.Id == noeud.Id ? lien.Noeud2 : lien.Noeud1;
                if (!villesAffichees.Contains(noeudConnecte.Id))
                {
                    noeudsConnectes.Add(noeudConnecte);
                    villesAffichees.Add(noeudConnecte.Id);
                }
            }
            
            // Préfixe pour les enfants
            string nouveauPrefixe = prefixe + (estDernier ? "    " : "│   ");
            
            // Afficher récursivement les villes connectées
            for (int i = 0; i < noeudsConnectes.Count; i++)
            {
                bool estDernierConnecte = (i == noeudsConnectes.Count - 1);
                AfficherReseauCapitaleRecursif(noeudsConnectes[i], nouveauPrefixe, estDernierConnecte, 
                                             villesAffichees, niveau + 1, profondeurMax);
            }
        }
        
        private void AfficherGrapheSalaries()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔═════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    ORGANIGRAMME                             ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            // Affichage du résumé
            Console.WriteLine($"\nL'organigramme contient {_graphe.Noeuds.Count} salariés.");
            
            // Affichage de la racine (directeur général)
            if (_graphe.Racine != null && _graphe.Racine.Entite is Salarie)
            {
                Salarie directeur = (Salarie)_graphe.Racine.Entite;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\nDIRECTION GÉNÉRALE: {directeur.Prenom} {directeur.Nom} - {directeur.Poste}");
                Console.ResetColor();
            }
            
            // Affichage hiérarchique
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n╔═════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                 STRUCTURE HIÉRARCHIQUE                      ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            // Utiliser un HashSet pour éviter les doublons dans l'affichage
            var noeudsAffiches = new HashSet<int>();
            
            // Commence à la racine
            if (_graphe.Racine != null)
            {
                AfficherOrganigrammeRecursif(_graphe.Racine, "", true, noeudsAffiches);
            }
            else if (_graphe.Noeuds.Count > 0)
            {
                AfficherOrganigrammeRecursif(_graphe.Noeuds[0], "", true, noeudsAffiches);
            }
            
            // Afficher les salariés non connectés
            bool salariesSansManagerTrouves = false;
            foreach (var noeud in _graphe.Noeuds)
            {
                if (!noeudsAffiches.Contains(noeud.Id))
                {
                    if (!salariesSansManagerTrouves)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("\n╔═════════════════════════════════════════════════════════════╗");
                        Console.WriteLine("║               SALARIÉS SANS RATTACHEMENT                    ║");
                        Console.WriteLine("╚═════════════════════════════════════════════════════════════╝");
                        Console.ResetColor();
                        salariesSansManagerTrouves = true;
                    }
                    
                    if (noeud.Entite is Salarie)
                    {
                        Salarie salarie = (Salarie)noeud.Entite;
                        Console.WriteLine($"• {salarie.Prenom} {salarie.Nom} - {salarie.Poste}");
                    }
                }
            }
        }
        
        private void AfficherOrganigrammeRecursif(Noeud noeud, string prefixe, bool estDernier, HashSet<int> noeudsAffiches)
        {
            // Marquer ce noeud comme affiché
            noeudsAffiches.Add(noeud.Id);
            
            // Afficher le salarié actuel
            if (noeud.Entite is Salarie)
            {
                Salarie salarie = (Salarie)noeud.Entite;
                Console.WriteLine($"{prefixe}{(estDernier ? "└── " : "├── ")}{salarie.Prenom} {salarie.Nom} - {salarie.Poste}");
            }
            else
            {
                Console.WriteLine($"{prefixe}{(estDernier ? "└── " : "├── ")}{noeud}");
            }
            
            // Trouver tous les subordonnés directs
            var subordonnes = new List<Noeud>();
            
            foreach (var lien in _graphe.Liens)
            {
                // Si ce salarié est le manager (lien orienté de ce salarié vers un autre)
                if (lien.Noeud1.Id == noeud.Id && lien.Oriente == true)
                {
                    subordonnes.Add(lien.Noeud2);
                }
                // Ou si ce salarié est le manager (lien orienté d'un autre vers ce salarié, avec orientation inverse)
                else if (lien.Noeud2.Id == noeud.Id && lien.Oriente == false)
                {
                    subordonnes.Add(lien.Noeud1);
                }
            }
            
            // Préfixe pour les enfants
            string nouveauPrefixe = prefixe + (estDernier ? "    " : "│   ");
            
            // Afficher récursivement tous les subordonnés
            for (int i = 0; i < subordonnes.Count; i++)
            {
                bool estDernierSubordonne = (i == subordonnes.Count - 1);
                
                // Si ce subordonné n'a pas déjà été affiché pour éviter les cycles
                if (!noeudsAffiches.Contains(subordonnes[i].Id))
                {
                    AfficherOrganigrammeRecursif(subordonnes[i], nouveauPrefixe, estDernierSubordonne, noeudsAffiches);
                }
                else
                {
                    // Affiche que le noeud a déjà été affiché pour éviter les cycles infinis
                    Console.WriteLine($"{nouveauPrefixe}{(estDernierSubordonne ? "└── " : "├── ")}[Référence circulaire]");
                }
            }
        }
        
        public void AfficherPlusCourtChemin(List<Noeud> chemin)
        {
            if (chemin == null || chemin.Count < 2)
            {
                Console.WriteLine("Aucun chemin à afficher.");
                return;
            }
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n╔═════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  PLUS COURT CHEMIN                          ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            Console.WriteLine($"\nChemin de {chemin[0]} à {chemin[chemin.Count - 1]} :");
            
            double distanceTotale = 0;
            
            for (int i = 0; i < chemin.Count - 1; i++)
            {
                Noeud noeudActuel = chemin[i];
                Noeud noeudSuivant = chemin[i + 1];
                
                // Trouver le lien entre ces deux noeuds
                var lien = _graphe.Liens.FirstOrDefault(l => 
                    (l.Noeud1.Id == noeudActuel.Id && l.Noeud2.Id == noeudSuivant.Id) ||
                    (l.Noeud1.Id == noeudSuivant.Id && l.Noeud2.Id == noeudActuel.Id));
                
                double? distance = lien?.Valeur;
                if (distance.HasValue)
                {
                    distanceTotale += distance.Value;
                }
                
                Console.Write($"• {noeudActuel} ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"──{(distance.HasValue ? $"({distance:F0} km)" : "")}──> ");
                Console.ResetColor();
                Console.WriteLine($"{noeudSuivant}");
            }
            
            if (distanceTotale > 0)
            {
                Console.WriteLine($"\nDistance totale : {distanceTotale:F0} km");
            }
        }
    }
}