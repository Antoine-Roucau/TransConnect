using System;
using System.Data;
using Mod = TransConnect.Models;

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
        public void LicencierSalarie(Mod.Salarie salarieALicencier, List<Mod.Salarie> salarie, OrganigrammeService graphe)
        {
            if (salarieALicencier == null)
            {
                Console.WriteLine("Le salarié à licencier est nul.");
                return;
            }

            if (!salarie.Contains(salarieALicencier))
            {
                Console.WriteLine("Le salarié à licencier n'existe pas dans la liste.");
                return;
            }

            // Étape 1 : Trouver le supérieur hiérarchique
            Mod.Salarie superieur = null;
            foreach (var s in salarie)
            {
                if (s.Subordonnes.Contains(salarieALicencier))
                {
                    superieur = s;
                    break;
                }
            }

            // Étape 2 : Promouvoir les subordonnés
            foreach (var subordonne in salarieALicencier.Subordonnes)
            {
                if (subordonne == null) continue;

                // Changement de rattachement
                if (superieur != null)
                {
                    superieur.AddSubordonnes(subordonne);
                }

                // Mise à jour du poste (promotion simplifiée)
                subordonne.Poste = $"Ancien poste de {salarieALicencier.Nom}";
            }

            // Étape 3 : Nettoyer la liste des subordonnés du supérieur
            if (superieur != null)
            {
                superieur.SupSubordonnes(salarieALicencier);
            }

            // Étape 4 : Supprimer le salarié de la liste principale
            salarie.Remove(salarieALicencier);

            // Étape 5 : Supprimer le nœud dans le graphe
            var noeudASupprimer = graphe.TrouverNoeudParSalarieNumeroSS(salarieALicencier.NumeroSS);
            if (noeudASupprimer != null)
            {
                graphe.SupprimerNoeud(noeudASupprimer);
            }
            else
            {
                Console.WriteLine("Noeud du salarié non trouvé dans le graphe.");
            }
        }
    }
}