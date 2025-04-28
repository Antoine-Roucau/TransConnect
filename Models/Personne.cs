using System;

namespace TransConnect.Models
{
    public abstract class Personne
    {
        #region Propriétés
        protected string numeroSS;
        protected string nom;
        protected string prenom;
        protected DateTime dateNaissance;
        protected string adressePostale;
        protected string adresseMail;
        protected string telephone;
        #endregion

        #region Constructeurs
        protected Personne(string numeroSS, string nom, string prenom, DateTime dateNaissance, string adressePostale, string adresseMail, string telephone)
        {
            this.numeroSS = numeroSS;
            this.nom = nom;
            this.prenom = prenom;
            this.dateNaissance = dateNaissance;
            this.adressePostale = adressePostale;
            this.adresseMail = adresseMail;
            this.telephone = telephone;
        }
        #endregion

        #region Getters et Setters
        public string NumeroSS 
        {
            get { return numeroSS; }
            set { numeroSS = value; }
        }
        public string Nom 
        { 
            get { return nom; } 
            set { nom = value; } 
        }
        public string Prenom 
        { 
            get { return prenom; } 
            set { prenom = value; } 
        }
        public DateTime DateNaissance 
        { 
            get { return dateNaissance; } 
            set { dateNaissance = value; } 
        }
        public string AdressePostale 
        { 
            get { return adressePostale; } 
            set { adressePostale = value; } 
        }
        public string AdresseMail 
        { 
            get { return adresseMail; } 
            set { adresseMail = value; } 
        }
        public string Telephone 
        { 
            get { return telephone; } 
            set { telephone = value; } 
        }
        #endregion

        #region Méthodes
        public virtual string AfficherInfos()
        {
            return $"\n{prenom} {nom} \n- Tél: {telephone} \n- Email: {adresseMail}";
        }
        #endregion
    }
}