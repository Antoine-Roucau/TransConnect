using System;
using System.Collections.Generic;
using System.Linq;
using TransConnect.Models.Graphe;

namespace TransConnect.Models.Graphe
{
    public class Dijkstra
    {
        public static List<Noeud> TrouverCheminLePlusCourt(Graphe graphe, Noeud source, Noeud destination)
        {
            // Dictionnaires pour stocker les distances et les prédecesseurs
            Dictionary<Noeud, double> distances = new Dictionary<Noeud, double>();
            Dictionary<Noeud, Noeud> predecesseurs = new Dictionary<Noeud, Noeud>();
            
            // Initialiser toutes les distances à l'infini et les prédecesseurs à null
            foreach (var noeud in graphe.Noeuds)
            {
                distances[noeud] = double.MaxValue;
                predecesseurs[noeud] = null;
            }
            
            // La distance de la source est 0
            distances[source] = 0;

            // Liste des noeuds non visités
            var nonVisites = new List<Noeud>(graphe.Noeuds);

            while (nonVisites.Count > 0)
            {
                // Trier la liste pour trouver le noeud avec la distance la plus faible
                var noeudActuel = nonVisites.OrderBy(n => distances[n]).First();

                // Si on a atteint la destination, on peut revenir en arrière pour reconstruire le chemin
                if (noeudActuel == destination)
                {
                    var chemin = new List<Noeud>();
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
            }

            // Si la destination n'a pas été atteinte
            return null;
        }
    }
}
