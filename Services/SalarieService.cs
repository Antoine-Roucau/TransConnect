using System;
using System.Data;
using TransConnect.Models;
namespace Transconnect.Services
{
    public class SalarieService
    {
        public DataTable GetSalariesTries(List<Salarie> salarieList)
        {
            DataTable dfSalarie = new DataTable();
            dfSalarie.Columns.Add("Numéro de Sécurité Sociale", typeof(string));
            dfSalarie.Columns.Add("Nom", typeof(string));
            dfSalarie.Columns.Add("Prénom", typeof(string));
            dfSalarie.Columns.Add("Date de Naissance", typeof(DateTime));
            dfSalarie.Columns.Add("Adresse Postale", typeof(string));
            dfSalarie.Columns.Add("Adresse Email", typeof(string));
            dfSalarie.Columns.Add("Téléphone", typeof(string));
            dfSalarie.Columns.Add("Date d'Entrée", typeof(DateTime));
            dfSalarie.Columns.Add("Poste", typeof(string));
            dfSalarie.Columns.Add("Salaire", typeof(decimal));

            foreach (Salarie s in salarieList)
            {
                dfSalarie.Rows.Add(s.NumeroSS, s.Nom, s.Prenom, s.DateNaissance, s.AdressePostale, s.AdresseMail, s.Telephone, s.DateEntree, s.Poste, s.Salaire);
            }
            return dfSalarie;
        }
        public Salarie TrouverSalarieParNumeroSS(string numeroSS, List<Salarie> salarieList)
        {
            foreach (Salarie s in salarieList)
            {
                if (s.NumeroSS == numeroSS)
                {
                    return s;
                }
            }
            throw new Exception("Le salarié n'existe pas.");
        }

        public void AjouterSalarie(Salarie salarie,List<Salarie> salarieList)
        {
            foreach (Salarie s in salarieList)
            {
                if (s.NumeroSS == salarie.NumeroSS)
                {
                    throw new Exception("Le salarié existe déjà.");
                }
            }
            salarieList.Add(salarie);
        }

        public void SupprimerSalarie(Salarie salarie, List<Salarie> salarieList)
        {
            if (!salarieList.Contains(salarie))
            {
                throw new Exception("Le salarié n'existe pas.");
            }
            salarieList.Remove(salarie);
        }

        public void ModifierSalarie(Salarie salarie, string numeroSS, string nom, string prenom, DateTime dateNaissance, string adressePostale, string adresseMail, string telephone, DateTime dateEntree, string poste, decimal salaire, List<Salarie> salarieList)
        {
            if (!salarieList.Contains(salarie))
            {
                throw new Exception("Le salarié n'existe pas.");
            }

            salarie.NumeroSS = numeroSS;
            salarie.Nom = nom;
            salarie.Prenom = prenom;
            salarie.DateNaissance = dateNaissance;
            salarie.AdressePostale = adressePostale;
            salarie.AdresseMail = adresseMail;
            salarie.Telephone = telephone;
            salarie.DateEntree = dateEntree;
            salarie.Poste = poste;
            salarie.Salaire = salaire;
        }
        public void Licenciement(Personne personne)
        {
            ///a faire une fois que l'on aura la liste des personnes
        }
    }
}