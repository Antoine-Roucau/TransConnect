using System;
using System.Collections.Generic;
using Transconnect.Models;
using Transconnect.Services;

namespace Transconnect.Services
{
    public class StatistiqueService
    {
        public List<Commande> CommandesParChauffeur(List<Commande> commandes, Salarie chauffeur)
        {
            List<Commande> commandesChauffeur = new List<Commande>();
            foreach (var commande in commandes)
            {
                if (commande.Chauffeur == chauffeur)
                {
                    commandesChauffeur.Add(commande);
                }
            }
            return commandesChauffeur;
        }

        public int MoyennePrixCommandes(List<Commande> commandes)
        {
            decimal total = 0;
            if (commandes.Count == 0) 
            {
                return 0;
            }
            foreach (var commande in commandes)
            {
                total += commande.Prix;
            }
            return(int)(total / commandes.Count);
        }

        public int PrixParClient(Client client, List<Commande> commandes)
        {
            decimal total = 0;
            foreach (var commande in commandes)
            {
                if (commande.Client == client)
                {
                    total += commande.Prix;
                }
            }
            return (int)total;
        }
        
        public List<Commande> CommandesParClient(List<Commande> commandes, Client client)
        {
            List<Commande> commandesClient = new List<Commande>();
            foreach (var commande in commandes)
            {
                if (commande.Client == client)
                {
                    commandesClient.Add(commande);
                }
            }
            return commandesClient;
        }

        public int NombreCommandesLivrees(List<Commande> commandes)
        {
            int count = 0;
            foreach (var commande in commandes)
            {
                if (commande.Statut == StatutCommande.Livree)
                {
                    count++;
                }
            }
            return count;
        }

        public decimal ChiffreAffaire(List<Commande> commandes)
        {
            decimal total = 0;
            foreach (var commande in commandes)
            {
                total += commande.Prix;
            }
            return total;
        }
    }
}        