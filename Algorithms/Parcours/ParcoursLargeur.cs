using System;
using System.Collections.Generic;
using Gra = TransConnect.Models.Graphe;

namespace TransConnect.Algorythme.Parcours
{
    public class ParcoursLargeur
    {
        private Gra.Graphe _graphe;

        public ParcoursLargeur(Gra.Graphe graphe)
        {
            _graphe = graphe;
        }

        public List<Gra.Noeud> Parcourir(Gra.Noeud depart)
        {
            var visites = new HashSet<Gra.Noeud>();
            var file = new Queue<Gra.Noeud>();
            var resultat = new List<Gra.Noeud>();

            file.Enqueue(depart);
            visites.Add(depart);

            while (file.Count > 0)
            {
                var courant = file.Dequeue();
                resultat.Add(courant);

                foreach (var voisin in ObtenirVoisins(courant))
                {
                    if (!visites.Contains(voisin))
                    {
                        visites.Add(voisin);
                        file.Enqueue(voisin);
                    }
                }
            }

            return resultat;
        }

        private List<Gra.Noeud> ObtenirVoisins(Gra.Noeud noeud)
        {
            var voisins = new List<Gra.Noeud>();

            foreach (var lien in _graphe.Liens)
            {
                if (lien.Noeud1 == noeud)
                    voisins.Add(lien.Noeud2);
                else if ((lien.Oriente == null || lien.Oriente == false) && lien.Noeud2 == noeud)
                    voisins.Add(lien.Noeud1);
            }

            return voisins;
        }
    }
}
