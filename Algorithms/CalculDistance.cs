using System;
using System.Collections.Generic;
using System.Linq;
using Gra = Transconnect.Models.Graphe;

namespace Transconnect.Algorithms.CalculDistance
{
    public class CalculDistance
    {
        public static double CalculerDistanceTotale(Gra.Graphe graphe, List<Gra.Noeud> chemin)
        {
            if (chemin == null || chemin.Count < 2)
                return 0;

            double distanceTotale = 0;

            for (int i = 0; i < chemin.Count - 1; i++)
            {
                var noeudA = chemin[i];
                var noeudB = chemin[i + 1];

                // Cherche le lien entre les deux nœuds
                var lien = graphe.Liens.FirstOrDefault(l =>
                    (l.Noeud1 == noeudA && l.Noeud2 == noeudB) ||
                    (l.Noeud1 == noeudB && l.Noeud2 == noeudA && (l.Oriente == null || l.Oriente == false)));


                distanceTotale += lien.Valeur ?? 1;
            }

            return distanceTotale;
        }
    }
}
