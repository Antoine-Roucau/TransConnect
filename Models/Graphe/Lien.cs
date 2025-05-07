using System;
using System.Collections.Generic;

namespace Transconnect.Models.Graphe
{
    public class Lien
    {
        #region Propriétés
        Noeud noeud1;
        Noeud noeud2;
        //Pour arbre distance
        double? valeur;
        //Null pas d'orientation, true 1-->2, false 2-->1
        bool? oriente;
        #endregion

        #region Constructeurs
        public Lien(Noeud noeud1, Noeud noeud2)
        {
            this.noeud1 = noeud1;
            this.noeud2 = noeud2;
        }
        public Lien(Noeud noeud1, Noeud noeud2, double? valeur = null, bool? oriente = null)
        {
            this.noeud1 = noeud1;
            this.noeud2 = noeud2;
            this.Valeur = valeur;
            this.Oriente = oriente;
        }
        #endregion

        #region Getters et Setters
        public Noeud Noeud1
        {
            get { return noeud1; }
            set { noeud1 = value; }
        }
        public Noeud Noeud2
        {
            get { return noeud2; }
            set { noeud2 = value; }
        }
        public double? Valeur
        {
            get { return valeur; }
            set { valeur = value; }
        }
        public bool? Oriente
        {
            get { return oriente; }
            set { oriente = value; }
        }
        #endregion

        #region Méthodes
        public override string ToString()
        {
            if (valeur != null)
            {
                return $"Lien entre {noeud1.Id} et {noeud2.Id} avec une valeur de {valeur}.";
            }
            else if (oriente != null)
            {
                return $"Lien entre {noeud1.Id} et {noeud2.Id} / orienté : {(oriente != true ? $"De {noeud1.Id} à {noeud2.Id}" : $"De {noeud2.Id} à {noeud1.Id}")}.";
            }
            else
            {
                return $"Lien entre {noeud1.Id} et {noeud2.Id}.";
            }
        }
        #endregion
    }
}

