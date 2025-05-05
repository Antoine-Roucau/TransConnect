using System;
using System.Collections.Generic;
using Gra = TransConnect.Models.Graphe;

namespace TransConnect.Algorithms.Parcours
{
    public class ParcoursProfondeur
    {
        private Gra.Graphe _graphe;

        public ParcoursProfondeur(Gra.Graphe graphe)
        {
            _graphe = graphe;
        }

        public List<Gra.Noeud> Parcourir(Gra.Noeud depart)
        {
            var visites = new HashSet<Gra.Noeud>();
            var resultat = new List<Gra.Noeud>();
            Explorer(depart, visites, resultat);
            return resultat;
        }

        private void Explorer(Gra.Noeud courant, HashSet<Gra.Noeud> visites, List<Gra.Noeud> resultat)
        {
            visites.Add(courant);
            resultat.Add(courant);

            foreach (var voisin in ObtenirVoisins(courant))
            {
                if (!visites.Contains(voisin))
                {
                    Explorer(voisin, visites, resultat);
                }
            }
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
