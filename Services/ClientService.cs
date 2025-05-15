using System;
using System.Data;
using System.Collections.Generic; // pour List<Client>
using Transconnect.Models;
using System.Runtime.CompilerServices;

namespace Transconnect.Services
{
    public class ClientService
    {
        public DataTable GetClientsDF(List<Client> clientList)
        {
            DataTable dfClient = new DataTable();
            dfClient.Columns.Add("Numéro de Sécurité Sociale", typeof(string));
            dfClient.Columns.Add("Nom", typeof(string));
            dfClient.Columns.Add("Prénom", typeof(string));
            dfClient.Columns.Add("Date de Naissance", typeof(DateTime));
            dfClient.Columns.Add("Adresse Postale", typeof(string));
            dfClient.Columns.Add("Adresse Email", typeof(string));
            dfClient.Columns.Add("Téléphone", typeof(string));
            dfClient.Columns.Add("Montant Achats Cumulés", typeof(decimal));

            foreach (Client c in clientList)
            {
                dfClient.Rows.Add(c.NumeroSS, c.Nom, c.Prenom, c.DateNaissance, c.AdressePostale, c.AdresseMail, c.Telephone, c.MontantTotalAchats);
            }
            return dfClient;
        }
        public void AjouterClient(Client client, List<Client> clientList)
        {
            foreach (Client c in clientList)
            {
                if (c.NumeroSS == client.NumeroSS)
                {
                    throw new Exception("Le client existe déjà.");
                }
            }

            clientList.Add(client);
        }
         public void SupprimerClient(Client client, List<Client> clientList)
        {
            if (!clientList.Contains(client))
            {
                throw new Exception("Le client n'existe pas.");
            }

            clientList.Remove(client);
        }

        public DataTable TrierClientsParNom(DataTable dfClient)
        {
            DataView vueParNom = dfClient.DefaultView;
            vueParNom.Sort = "Nom ASC";
            return vueParNom.ToTable();
        }

        public DataTable TrierClientsParMontant(DataTable dfClient)
        {
            DataView vueParMontant = dfClient.DefaultView;
            vueParMontant.Sort = "Montant_Total_Achats DESC";
            return vueParMontant.ToTable();
        }

        public DataTable TrierClientsParVille(DataTable dfClient)
        {
            DataView vueParVille = dfClient.DefaultView;
            vueParVille.Sort = "Adresse_Postale ASC";
            return vueParVille.ToTable();
        }

        public DataTable TrierClientsParDateNaissance(DataTable dfClient)
        {
            DataView vueParDateNaissance = dfClient.DefaultView;
            vueParDateNaissance.Sort = "Date_de_Naissance ASC";
            return vueParDateNaissance.ToTable();
        }

        public void ModifierClient(Client client, Client clientModif, List<Client> clientList)
        {
            if (!clientList.Contains(client))
            {
                throw new Exception("Le client n'existe pas.");
            }

            client.NumeroSS = clientModif.NumeroSS;
            client.Nom = clientModif.Nom;
            client.Prenom = clientModif.Prenom;
            client.DateNaissance = clientModif.DateNaissance;
            client.AdressePostale = clientModif.AdressePostale;
            client.AdresseMail = clientModif.AdresseMail;
            client.Telephone = clientModif.Telephone;
        }
    }
}

