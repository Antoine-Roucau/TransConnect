using System;
using System.Data;
using System.Collections.Generic; // pour List<Client>
using TransConnect.Models;

namespace Transconnect.Services
{
    public class ClientService
    {
        public DataTable GetClientsTriés(List<Client> ClientList)
        {
            // Création du Dataframe
            DataTable dfClient = new DataTable();
            dfClient.Columns.Add("Numéro de Sécurité Sociale", typeof(string));
            dfClient.Columns.Add("Nom", typeof(string));
            dfClient.Columns.Add("Prénom", typeof(string));
            dfClient.Columns.Add("Date de Naissance", typeof(DateTime));
            dfClient.Columns.Add("Adresse Postale", typeof(string));
            dfClient.Columns.Add("Adresse Email", typeof(string));
            dfClient.Columns.Add("Téléphone", typeof(string));
            dfClient.Columns.Add("Montant Achats Cumulés", typeof(decimal));

            // Remplissage du Dataframe
            foreach (Client c in ClientList)
            {
                dfClient.Rows.Add(c.NumeroSS, c.Nom, c.Prenom, c.DateNaissance, c.AdressePostale, c.AdresseMail, c.Telephone, c.MontantTotalAchats);
            }
            return dfClient;
        }
        public void AjouterClient(Client client, List<Client> ClientList)
        {
            // Vérification de l'existence du client
            foreach (Client c in ClientList)
            {
                if (c.NumeroSS == client.NumeroSS)
                {
                    throw new Exception("Le client existe déjà.");
                }
            }

            ClientList.Add(client);
        }
         public void SupprimerClient(Client client, List<Client> ClientList)
        {
            // Vérification de l'existence du client
            if (!ClientList.Contains(client))
            {
                throw new Exception("Le client n'existe pas.");
            }

            ClientList.Remove(client);
        }

        public DataView trierClientsParNom(DataTable dfClient)
        {
            // Tri par Nom
            DataView vueParNom = dfClient.DefaultView;
            vueParNom.Sort = "Nom ASC";
            return vueParNom;
        }

        public DataView trierClientsParMontant(DataTable dfClient)
        {
            // Tri par Montant
            DataView vueParMontant = dfClient.DefaultView;
            vueParMontant.Sort = "Montant Achats Cumulés DESC";
            return vueParMontant;
        }

        public DataView trierClientsParVille(DataTable dfClient)
        {
            // Tri par Ville
            DataView vueParVille = dfClient.DefaultView;
            vueParVille.Sort = "Adresse Postale ASC";
            return vueParVille;
        }
    }
}

