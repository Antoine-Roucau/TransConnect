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
                dfClient.Rows.Add();
            }

            // Tri par Nom
            DataView vueParNom = dfClient.DefaultView;
            vueParNom.Sort = "Nom ASC";
            DataTable dfClientTrierParNom = vueParNom.ToTable();

            // Tri par Montant
            DataView vueParMontant = dfClient.DefaultView;
            vueParMontant.Sort = "Montant Achats Cumulés DESC";
            DataTable dfClientTrieParMontant = vueParMontant.ToTable();

            return dfClientTrierParNom;
        }
    }
}

