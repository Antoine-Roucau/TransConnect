using System;
using System.Collections.Generic;
using System.Linq;
using Gra = Transconnect.Models.Graphe;
using System.Diagnostics;

namespace Transconnect.Algorithms.PlusCourtChemin
{
    public class BellmanFord
    {
        private Gra.Graphe _graphe;
        private Dictionary<Gra.Noeud, double> _distances;
        private Dictionary<Gra.Noeud, Gra.Noeud> _predecesseurs;
        public static TimeSpan TempsExecution { get; private set; }
        public static long UtilisationMemoire { get; private set; }

        public BellmanFord(Gra.Graphe graphe)
        {
            _graphe = graphe;
            _distances = new Dictionary<Gra.Noeud, double>();
            _predecesseurs = new Dictionary<Gra.Noeud, Gra.Noeud>();
        }

        public bool CalculerPlusCourtsChemins(Gra.Noeud source)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long memoireAvant = GC.GetTotalMemory(true);

            Stopwatch chrono = Stopwatch.StartNew();

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
            chrono.Stop();
            TempsExecution = chrono.Elapsed;
            long memoireApres = GC.GetTotalMemory(false);
            UtilisationMemoire = memoireApres - memoireAvant;
            return true;
        }

        private bool Relaxer(Gra.Lien lien, bool verifierSeulement = false)
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