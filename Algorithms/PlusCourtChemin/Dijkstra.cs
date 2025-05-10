using System;
using System.Collections.Generic;
using System.Linq;
using Gra = Transconnect.Models.Graphe;
using System.Diagnostics;

namespace Transconnect.Algorithms.PlusCourtChemin
{
    public class Dijkstra
    {
        public static TimeSpan TempsExecution { get; private set; }
        public static long UtilisationMemoire { get; private set; }
        public static List<Gra.Noeud> TrouverCheminLePlusCourt(Gra.Graphe graphe, Gra.Noeud source, Gra.Noeud destination)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long memoireAvant = GC.GetTotalMemory(true);


            Stopwatch chrono = Stopwatch.StartNew();

            // Dictionnaires pour stocker les distances et les prédecesseurs
            Dictionary<Gra.Noeud, double> distances = new Dictionary<Gra.Noeud, double>();
            Dictionary<Gra.Noeud, Gra.Noeud> predecesseurs = new Dictionary<Gra.Noeud, Gra.Noeud>();
            
            
            // Initialiser toutes les distances à l'infini et les prédecesseurs à null
            foreach (var noeud in graphe.Noeuds)
            {
                distances[noeud] = double.MaxValue;
                predecesseurs[noeud] = null;
            }
            
            // La distance de la source est 0
            distances[source] = 0;

            // Liste des noeuds non visités
            var nonVisites = new List<Gra.Noeud>(graphe.Noeuds);

            while (nonVisites.Count > 0)
            {
                // Trier la liste pour trouver le noeud avec la distance la plus faible
                var noeudActuel = nonVisites.OrderBy(n => distances[n]).First();

                // Si on a atteint la destination, on peut revenir en arrière pour reconstruire le chemin
                if (noeudActuel == destination)
                {
                    var chemin = new List<Gra.Noeud>();
                    while (predecesseurs[noeudActuel] != null)
                    {
                        chemin.Insert(0, noeudActuel);
                        noeudActuel = predecesseurs[noeudActuel];
                    }
                    chemin.Insert(0, source); // Ajouter la source au début du chemin
                    return chemin;
                }

                // Enlever le noeud actuel de la liste des non visités
                nonVisites.Remove(noeudActuel);

                // Mettre à jour les distances des voisins
                foreach (var lien in graphe.Liens.Where(l => l.Noeud1 == noeudActuel || l.Noeud2 == noeudActuel))
                {
                    var voisin = lien.Noeud1 == noeudActuel ? lien.Noeud2 : lien.Noeud1;

                    if (nonVisites.Contains(voisin))
                    {
                        double nouvelleDistance = distances[noeudActuel] + (lien.Valeur ?? double.MaxValue);
                        
                        // Si une distance plus courte est trouvée
                        if (nouvelleDistance < distances[voisin])
                        {
                            distances[voisin] = nouvelleDistance;
                            predecesseurs[voisin] = noeudActuel;
                        }
                    }
                }
                chrono.Stop();
                TempsExecution = chrono.Elapsed;
                long memoireApres = GC.GetTotalMemory(false);
                UtilisationMemoire = memoireApres - memoireAvant;
            }

            // Si la destination n'a pas été atteinte
            return null;
        }
    }
}
