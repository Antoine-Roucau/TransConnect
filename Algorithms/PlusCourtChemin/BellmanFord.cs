using System;
using System.Collections.Generic;
using System.Linq;
using TransConnect.Models.Graphe;

namespace TransConnect.Models.Graphe
{
    public class BellmanFord
    {
        private Graphe _graphe;
        private Dictionary<Noeud, double> _distances;
        private Dictionary<Noeud, Noeud> _predecesseurs;

        public BellmanFord(Graphe graphe)
        {
            _graphe = graphe;
            _distances = new Dictionary<Noeud, double>();
            _predecesseurs = new Dictionary<Noeud, Noeud>();
        }

        public bool CalculerPlusCourtsChemins(Noeud source)
        {
            // Initialisation
            foreach (var noeud in _graphe.Noeuds)
            {
                _distances[noeud] = double.PositiveInfinity;
                _predecesseurs[noeud] = null;
            }
            _distances[source] = 0;

            // Étape principale : relaxation des arêtes
            for (int i = 0; i < _graphe.Noeuds.Count - 1; i++)
            {
                foreach (var lien in _graphe.Liens)
                {
                    Relaxer(lien);
                }
            }

            // Vérification des cycles de poids négatif
            foreach (var lien in _graphe.Liens)
            {
                if (Relaxer(lien, verifierSeulement: true))
                {
                    Console.WriteLine("Cycle de poids négatif détecté !");
                    return false;
                }
            }

            return true;
        }

        private bool Relaxer(Lien lien, bool verifierSeulement = false)
        {
            var u = lien.Noeud1;
            var v = lien.Noeud2;
            double poids = lien.Valeur ?? 1;

            if (_distances[u] + poids < _distances[v])
            {
                if (verifierSeulement) return true;
                _distances[v] = _distances[u] + poids;
                _predecesseurs[v] = u;
            }

            // Si non orienté, on vérifie aussi dans l'autre sens
            if (lien.Oriente == null || lien.Oriente == false)
            {
                if (_distances[v] + poids < _distances[u])
                {
                    if (verifierSeulement) return true;
                    _distances[u] = _distances[v] + poids;
                    _predecesseurs[u] = v;
                }
            }

            return false;
        }

        public List<Noeud> RecupererChemin(Noeud destination)
        {
            var chemin = new List<Noeud>();
            var courant = destination;

            while (courant != null)
            {
                chemin.Insert(0, courant);
                courant = _predecesseurs[courant];
            }

            return chemin;
        }

        public double GetDistance(Noeud noeud)
        {
            return _distances.ContainsKey(noeud) ? _distances[noeud] : double.PositiveInfinity;
        }
    }
}      