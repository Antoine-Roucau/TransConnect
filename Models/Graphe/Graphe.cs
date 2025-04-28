using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransConnect.Models.Graphe
{
    public class Graphe 
    {
        #region Propriétés
        Noeud racine;
        List<Noeud> noeuds = new List<Noeud>();
        List<Lien> liens = new List<Lien>();
        #endregion

        #region Constructeurs
        public Graphe(Object racine)
        {
            this.racine = new Noeud(racine);
        }
        public Graphe(Noeud racine)
        {
            this.racine = racine;
            noeuds.Add(racine);
        }
        public Graphe(Noeud racine, List<Noeud> noeuds)
        {
            this.racine = racine;
            this.noeuds = noeuds;
        }
        #endregion

        #region Getters et Setters
        public Noeud Racine
        {
            get { return racine; }
            set { racine = value; }
        }
        public List<Noeud> Noeuds
        {
            get { return noeuds; }
            set { noeuds = value; }
        }
        public List<Lien> Liens
        {
            get { return liens; }
            set { liens = value; }
        }
        #endregion

        #region Méthodes
        public void AjouterNoeud(Noeud noeud)
        {
            noeuds.Add(noeud);
        }
        public void SupprimerNoeud(Noeud noeud)
        {
            noeuds.Remove(noeud);
        }

        public void AjouterLien(Lien lien)
        {
            liens.Add(lien);
            if (!noeuds.Contains(lien.Noeud1))
            {
                noeuds.Add(lien.Noeud1);
            }
            if (!noeuds.Contains(lien.Noeud2))
            {
                noeuds.Add(lien.Noeud2);
            }
        }
        public void SupprimerLien(Lien lien)
        {
            liens.Remove(lien);
        }

        public void AfficherGraphe()
        {
            AffichageGrapheConsole affichage = new AffichageGrapheConsole(this);
            affichage.AfficherGraphe();
        }

        public Noeud TrouverNoeudVille(string ville)
        {
            foreach (var noeud in noeuds)
            {
                if (noeud.Entite.ToString() == ville)
                {
                    return noeud;
                }
            }
            return null;
        }
        #endregion

    }

    //Utilisation D'une IA pour générer la classe AffichageGrapheConsole
    public class AffichageGrapheConsole
    {
        private Graphe _graphe;
        
        public AffichageGrapheConsole(Graphe graphe)
        {
            _graphe = graphe;
        }
        
        public void AfficherGraphe()
        {
            Console.WriteLine("Graphe:");
            foreach (var lien in _graphe.Liens)
            {
                if (lien.Valeur != null)
                {
                    Console.WriteLine($"{lien.Noeud1.Entite} -- {lien.Noeud2.Entite} (Valeur: {lien.Valeur})");
                }
                else if (lien.Oriente != null)
                {
                    Console.WriteLine($"{lien.Noeud1.Entite} -> {lien.Noeud2.Entite} (Orienté: {lien.Oriente})");
                }
                else
                {
                    Console.WriteLine($"{lien.Noeud1.Entite} -- {lien.Noeud2.Entite}");
                }
            }
        }
        
       
    }
}