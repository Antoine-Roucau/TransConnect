using System;
using System.Collections.Generic;
using Mod = Transconnect.Models;

namespace Transconnect.Services
{
    public class CommandeService
    {
        private readonly List<Mod.Commande> _commandes;

        public CommandeService()
        {
            _commandes = new List<Mod.Commande>();
        }

        public void AjouterCommande(Mod.Commande commande)
        {
            _commandes.Add(commande);
        }

        public List<Mod.Commande> GetCommandes()
        {
            return _commandes;
        }

        public void SupprimerCommande(Guid id)
        {
            var commande = _commandes.Find(c => c.Id == id);
            if (commande != null)
            {
                _commandes.Remove(commande);
            }
        }

        public void ModifierCommande(Guid id, Mod.Commande nouvelleCommande)
        {
            var commande = _commandes.Find(c => c.Id == id);
            if (commande != null)
            {
                commande.Client = nouvelleCommande.Client;
                commande.Produits = nouvelleCommande.Produits;
                commande.DateCommande = nouvelleCommande.DateCommande;
                commande.Statut = nouvelleCommande.Statut;
            }
        }

        public List<Mod.Commande> TrierCommandesParDate()
        {
            List<Mod.Commande> commandesTriees = new List<Mod.Commande>(_commandes);
            commandesTriees.Sort((c1, c2) => c1.DateCommande.CompareTo(c2.DateCommande));
            return commandesTriees;
        }

        public List<Mod.Commande> CommandeTrierParClient(string client)
        {
            return _commandes.FindAll(c => c.Mod.Client.Equals(client, StringComparison.OrdinalIgnoreCase));
        }
    }
}