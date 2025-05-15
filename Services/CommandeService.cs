using System;
using System.Collections.Generic;
using Transconnect.Models;

namespace Transconnect.Services
{
    public class CommandeService
    {
        private readonly List<Commande> _commandes;

        public CommandeService()
        {
            _commandes = new List<Commande>();
        }
        public CommandeService(List<Commande> commandes)
        {
            _commandes = commandes;
        }
        public void AbonnerAuxChangementsDeStatut(Commande commande)
        {
            commande.StatusChanged += OnCommandeStatusChanged;
        }

        private void OnCommandeStatusChanged(Commande commande, StatutCommande ancienStatut, StatutCommande nouveauStatut)
        {
            Console.WriteLine($"Commande {commande.Id} : {ancienStatut} -> {nouveauStatut}");
            
            if (nouveauStatut == StatutCommande.Livree && commande.Vehicule != null)
            {
                commande.Vehicule.EstDisponible = true;
            }
        }

        public void AjouterCommande(Commande commande)
        {
            _commandes.Add(commande);
        }

        public List<Commande> GetCommandes()
        {
            return _commandes;
        }

        public void SupprimerCommande(int id)
        {
            var commande = _commandes.Find(c => c.Id == id);
            if (commande != null)
            {
                _commandes.Remove(commande);
            }
        }

        public void ModifierCommande(int id, Commande nouvelleCommande)
        {
            var commande = _commandes.Find(c => c.Id == id);
            if (commande != null)
            {
                commande.Client = nouvelleCommande.Client;
                commande.VilleDepart = nouvelleCommande.VilleDepart;
                commande.VilleArrivee = nouvelleCommande.VilleArrivee;
                commande.Date = nouvelleCommande.Date;
                commande.Prix = nouvelleCommande.Prix;
                commande.Statut = nouvelleCommande.Statut;
                commande.Chauffeur = nouvelleCommande.Chauffeur;
                commande.Vehicule = nouvelleCommande.Vehicule;
            }
        }

        public List<Commande> TrierCommandesParDate()
        {
            List<Commande> commandesTriees = new List<Commande>(_commandes);
            commandesTriees.Sort((c1, c2) => c1.Date.CompareTo(c2.Date));
            return commandesTriees;
        }

        public List<Commande> CommandesTrieesParClient(string nomClient)
        {
            return _commandes.FindAll(c => c.Client != null && c.Client.Nom.Equals(nomClient, StringComparison.OrdinalIgnoreCase));
        }
    }
}