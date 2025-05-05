using System;
using System.Collections.Generic;
using Gra = TransConnect.Models.Graphe;

namespace TransConnect.Algorithms.PlusCourtChemin
{
    public class FloydWarshall
    {
        private Gra.Graphe _graphe;
        private Dictionary<Gra.Noeud, Dictionary<Gra.Noeud, double>> _distances;
        private Dictionary<Gra.Noeud, Dictionary<Gra.Noeud, Gra.Noeud>> _predecesseurs;

        public FloydWarshall(Gra.Graphe graphe)
        {
            _graphe = graphe;
            _distances = new Dictionary<Gra.Noeud, Dictionary<Gra.Noeud, double>>();
            _predecesseurs = new Dictionary<Gra.Noeud, Dictionary<Gra.Noeud, Gra.Noeud>>();
            Initialiser();
        }

        private void Initialiser()
        {
            var noeuds = _graphe.Noeuds;

            foreach (var u in noeuds)
            {
                _distances[u] = new Dictionary<Gra.Noeud, double>();
                _predecesseurs[u] = new Dictionary<Gra.Noeud, Gra.Noeud>();
                foreach (var v in noeuds)
                {
                    _distances[u][v] = u == v ? 0 : double.PositiveInfinity;
                    _predecesseurs[u][v] = null;
                }
            }

            foreach (var lien in _graphe.Liens)
            {
                var u = lien.Noeud1;
                var v = lien.Noeud2;
                var poids = lien.Valeur ?? 1;

                _distances[u][v] = poids;
                _predecesseurs[u][v] = u;

                if (lien.Oriente == null || lien.Oriente == false)
                {
                    _distances[v][u] = poids;
                    _predecesseurs[v][u] = v;
                }
            }
        }

        public void CalculerPlusCourtsChemins()
        {
            var noeuds = _graphe.Noeuds;

            foreach (var k in noeuds)
            {
                foreach (var i in noeuds)
                {
                    foreach (var j in noeuds)
                    {
                        double dIk = _distances[i][k];
                        double dKj = _distances[k][j];
                        double dIj = _distances[i][j];

                        if (dIk + dKj < dIj)
                        {
                            _distances[i][j] = dIk + dKj;
                            _predecesseurs[i][j] = _predecesseurs[k][j];
                        }
                    }
                }
            }
        }

        public List<Gra.Noeud> RecupererChemin(Gra.Noeud source, Gra.Noeud destination)
        {
            if (_predecesseurs[source][destination] == null)
                return null;

            var chemin = new List<Gra.Noeud> { destination };
            var courant = destination;

            while (courant != source)
            {
                courant = _predecesseurs[source][courant];
                chemin.Insert(0, courant);
            }

            return chemin;
        }

        public double GetDistance(Gra.Noeud source, Gra.Noeud destination)
        {
            return _distances[source][destination];
        }
    }
}
