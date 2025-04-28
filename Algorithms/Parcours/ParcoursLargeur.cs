using System;
using System.Collections.Generic;
using TransConnect.Models.Graphe;

namespace TransConnect.Algorithms.Parcours
{
    public class ParcoursLargeur
    {
        public static List<Noeud> Parcourir(Noeud noeudInitial)
        {
            List<Noeud> noeudsVisites = new List<Noeud>();
            Queue<Noeud> file = new Queue<Noeud>();
            file.Enqueue(noeudInitial);

            while (file.Count > 0)
            {
                Noeud noeudActuel = file.Dequeue();
                if (!noeudsVisites.Contains(noeudActuel))
                {
                    noeudsVisites.Add(noeudActuel);
                    foreach (var lien in noeudActuel.Liens)
                    {
                        Noeud voisin = lien.Noeud2;
                        if (!noeudsVisites.Contains(voisin))
                        {
                            file.Enqueue(voisin);
                        }
                    }
                }
            }

            return noeudsVisites;
        }
    }
}