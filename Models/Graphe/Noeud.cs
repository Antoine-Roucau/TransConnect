using System;
using System.Collections.Generic;


namespace Transconnect.Models.Graphe
{

    public class Noeud 
    {
        #region Propriétés
        int id;
        static int nextId = 1;
        Object entite;
        #endregion

        #region Constructeurs
        public Noeud(Object entite)
        {
            this.id = nextId++;
            this.entite = entite;
        }
        #endregion

        #region Getters et Setters
        public int Id
        {
            get { return id; }
        }
        public Object Entite
        {
            get { return entite; }
            set { entite = value; }
        }
        #endregion
    
        #region Méthodes
        public override string ToString()
        {
            if(entite.ToString() != null)
            {
                return entite.ToString();
            }
            else 
            {
                return "Noeud sans entité";
            }

        }
        #endregion
    }
}
