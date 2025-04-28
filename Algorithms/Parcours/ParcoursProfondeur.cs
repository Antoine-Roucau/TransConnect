using System;
using System.Collections.Generic;
using TransConnect.Models.Graphe;

namespace TransConnect.Algorithms.Parcours
{
    public class ParcoursProfondeur
    {
        public static List<Noeud> Parcourir(Noeud noeudInitial)
        {
            List<Noeud> noeudsVisites = new List<Noeud>();
            Stack<Noeud> pile = new Stack<Noeud>();
            pile.Push(noeudInitial);

            while (pile.Count > 0)
            {
                Noeud noeudActuel = pile.Pop();
                if (!noeudsVisites.Contains(noeudActuel))
                {
                    noeudsVisites.Add(noeudActuel);
                    foreach (var lien in noeudActuel.Liens)
                    {
                        Noeud voisin = lien.Noeud2;
                        if (!noeudsVisites.Contains(voisin))
                        {
                            pile.Push(voisin);
                        }
                    }
                }
            }

            return noeudsVisites;
        }
    }
}