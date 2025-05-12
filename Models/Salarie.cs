using System;
using System.Collections.Generic;

namespace Transconnect.Models 
{
    public interface ISerializable
    {
        string Serialize();
        void Deserialize(string data);
    }
    public class Salarie : Personne , ISerializable
    {
        #region Propriétés
        DateTime dateEntree;
        string poste;
        decimal salaire;
        List<Salarie> subordonnes = new List<Salarie>();
        #endregion
        
        #region Constructeurs
        public Salarie(string numeroSS, string nom, string prenom, DateTime dateNaissance, string adressePostale, string adresseMail, string telephone, DateTime dateEntree, string poste, decimal salaire) 
        : base(numeroSS, nom, prenom, dateNaissance, adressePostale, adresseMail, telephone)
        {
            this.dateEntree = dateEntree;
            this.poste = poste;
            this.salaire = salaire;
        }
        #endregion
        
        #region Getters et Setters
        public DateTime DateEntree
        {
            get { return dateEntree; }
            set { dateEntree = value; }
        }
        public string Poste 
        { 
            get { return poste; }
            set { poste = value; } 
        }
        public decimal Salaire 
        { 
            get { return salaire; }
            set { salaire = value; } 
        }
        public List<Salarie> Subordonnes 
        { 
            get { return subordonnes; }
            set { subordonnes = value; } 
        }
        #endregion

        #region Méthodes
        
        public void AddSubordonnes(Salarie subordonnee)
        {
            subordonnes.Add(subordonnee);
        }
        public void SupSubordonnes(Salarie subordonnee)
        {
            subordonnes.Remove(subordonnee);
        }
        public override string AfficherInfos()
        {
            return $"{base.AfficherInfos()} \n- Poste: {poste} \n- Salaire: {salaire:C} \n- Date d'entrée: {dateEntree.ToShortDateString()}";
        }

        public string AfficherHierarchie()
        {
            string infos = $"{base.nom} {base.prenom} : {poste}";
            if (subordonnes.Count > 0)
            {
                foreach (var sub in subordonnes)
                {
                    if (sub != null)
                    {
                        string subHierarchie = sub.AfficherHierarchie();
                        subHierarchie = subHierarchie.Replace("\n", "\n\t");
                        infos += $"\n\t{subHierarchie}";
                    }
                }
            }
            return infos;
        }
        public string Serialize()
        {
            return $"{numeroSS};{nom};{prenom};{dateNaissance.ToString("yyyy-MM-dd")};{adressePostale};{adresseMail};{telephone};{dateEntree.ToString("yyyy-MM-dd")};{poste};{salaire}";
        }
        public void Deserialize(string data)
        {
            string[] parts = data.Split(';');
            if (parts.Length == 10)
            {
                numeroSS = parts[0];
                nom = parts[1];
                prenom = parts[2];
                dateNaissance = DateTime.Parse(parts[3]);
                adressePostale = parts[4];
                adresseMail = parts[5];
                telephone = parts[6];
                dateEntree = DateTime.Parse(parts[7]);
                poste = parts[8];
                salaire = decimal.Parse(parts[9]);
            }
        }
        #endregion
    }

}