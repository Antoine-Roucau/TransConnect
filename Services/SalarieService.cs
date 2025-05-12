using System;
using System.Data;
using Mod = Transconnect.Models;

namespace Transconnect.Services
{
    public class SalarieService
    {
        public DataTable GetSalariesDF(List<Mod.Salarie> salarieList)
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

            foreach (Mod.Salarie s in salarieList)
            {
                dfSalarie.Rows.Add(s.NumeroSS, s.Nom, s.Prenom, s.DateNaissance, s.AdressePostale, s.AdresseMail, s.Telephone, s.DateEntree, s.Poste, s.Salaire);
            }
            return dfSalarie;
        }
        public Mod.Salarie TrouverSalarieParNumeroSS(string numeroSS, List<Mod.Salarie> salarieList)
        {
            foreach (Mod.Salarie s in salarieList)
            {
                if (s.NumeroSS == numeroSS)
                {
                    return s;
                }
            }
            throw new Exception("Le salarié n'existe pas.");
        }

        public void AjouterSalarie(Mod.Salarie salarie,List<Mod.Salarie> salarieList)
        {
            foreach (Mod.Salarie s in salarieList)
            {
                if (s.NumeroSS == salarie.NumeroSS)
                {
                    throw new Exception("Le salarié existe déjà.");
                }
            }
            salarieList.Add(salarie);
        }

        public void SupprimerSalarie(Mod.Salarie salarie, List<Mod.Salarie> salarieList)
        {
            if (!salarieList.Contains(salarie))
            {
                throw new Exception("Le salarié n'existe pas.");
            }
            salarieList.Remove(salarie);
        }

        public void ModifierSalarie(Mod.Salarie salarie, string numeroSS, string nom, string prenom, DateTime dateNaissance, string adressePostale, string adresseMail, string telephone, DateTime dateEntree, string poste, decimal salaire, List<Mod.Salarie> salarieList)
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
        public void LicencierSalarie(Mod.Salarie salarieALicencier, List<Mod.Salarie> salarieList, OrganigrammeService graphe)
        {
            if (salarieALicencier == null)
            {
                Console.WriteLine("Le salarié à licencier est nul.");
                return;
            }

            if (!salarieList.Contains(salarieALicencier))
            {
                Console.WriteLine("Le salarié à licencier n'existe pas dans la liste.");
                return;
            }

            // Étape 1 : Trouver le supérieur hiérarchique via le graphe
            var superieur = graphe.TrouverSuperieur(salarieALicencier);

            // Étape 2 : Gérer les subordonnés
            var subordonnes = new List<Mod.Salarie>(salarieALicencier.Subordonnes);

            foreach (var sub in subordonnes)
            {
                if (sub == null) continue;

                // Promouvoir le subordonné (changer de poste si nécessaire)
                if (!string.IsNullOrEmpty(salarieALicencier.Poste))
                {
                    // On peut ajuster cette logique selon les besoins métier
                    sub.Poste = sub.Poste; // Garder son poste actuel ou le promouvoir
                }

                // Retirer de la liste des subordonnés du licencié
                salarieALicencier.SupSubordonnes(sub);

                // Ajouter au supérieur si il y en a un
                if (superieur != null)
                {
                    graphe.AjouterRelation(superieur, sub);
                }
            }

            // Étape 3 : Supprimer les relations hiérarchiques du salarié licencié
            if (superieur != null)
            {
                graphe.SupprimerRelation(superieur, salarieALicencier);
            }

            // Étape 4 : Supprimer de la structure de données du graphe
            if (graphe.graphe.ContainsKey(salarieALicencier))
            {
                graphe.graphe.Remove(salarieALicencier);
            }

            // Étape 5 : Retirer le salarié de la liste principale
            salarieList.Remove(salarieALicencier);
        }

    }
}