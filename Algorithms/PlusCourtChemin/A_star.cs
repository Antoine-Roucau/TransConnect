using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Gra = Transconnect.Models.Graphe;

namespace Transconnect.Algorithms.PlusCourtChemin
{
    public class AStar
    {
        private Gra.Graphe _graphe;
        private Dictionary<Gra.Noeud, double> _distances;
        private Dictionary<Gra.Noeud, Gra.Noeud> _predecesseurs;
        public static TimeSpan TempsExecution { get; private set; }
        public static long UtilisationMemoire { get; private set; }

        public AStar(Gra.Graphe graphe)
        {
            _graphe = graphe;
            _distances = new Dictionary<Gra.Noeud, double>();
            _predecesseurs = new Dictionary<Gra.Noeud, Gra.Noeud>();
        }

        public bool CalculerPlusCourtChemin(Gra.Noeud source, Gra.Noeud destination, Func<Gra.Noeud, Gra.Noeud, double> heuristique)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            long memoireAvant = GC.GetTotalMemory(true);

            Stopwatch chrono = Stopwatch.StartNew();

            var ouvert = new PriorityQueue<Gra.Noeud, double>();
            var coutEstimeTotal = new Dictionary<Gra.Noeud, double>();

            foreach (var noeud in _graphe.Noeuds)
            {
                _distances[noeud] = double.PositiveInfinity;
                coutEstimeTotal[noeud] = double.PositiveInfinity;
                _predecesseurs[noeud] = null;
            }

            _distances[source] = 0;
            coutEstimeTotal[source] = heuristique(source, destination);
            ouvert.Enqueue(source, coutEstimeTotal[source]);

            while (ouvert.Count > 0)
            {
                var courant = ouvert.Dequeue();

                if (courant.Equals(destination))
                {
                    chrono.Stop();
                    TempsExecution = chrono.Elapsed;
                    UtilisationMemoire = GC.GetTotalMemory(false) - memoireAvant;
                    return true;
                }

                foreach (var lien in courant.Liens)
                {
                    var voisin = (lien.Noeud1.Equals(courant)) ? lien.Noeud2 : lien.Noeud1;
                    double poids = lien.Valeur ?? 1;
                    double tentative = _distances[courant] + poids;

                    if (tentative < _distances[voisin])
                    {
                        _distances[voisin] = tentative;
                        _predecesseurs[voisin] = courant;
                        coutEstimeTotal[voisin] = tentative + heuristique(voisin, destination);
                        ouvert.Enqueue(voisin, coutEstimeTotal[voisin]);
                    }
                }
            }

            chrono.Stop();
            TempsExecution = chrono.Elapsed;
            UtilisationMemoire = GC.GetTotalMemory(false) - memoireAvant;
            return false;
        }

        public List<Gra.Noeud> RecupererChemin(Gra.Noeud destination)
        {
            var chemin = new List<Gra.Noeud>();
            var courant = destination;

            while (courant != null)
            {
                chemin.Insert(0, courant);
                courant = _predecesseurs[courant];
            }

            return chemin;
        }

        public double GetDistance(Gra.Noeud noeud)
        {
            return _distances.ContainsKey(noeud) ? _distances[noeud] : double.PositiveInfinity;
        }
    }
}