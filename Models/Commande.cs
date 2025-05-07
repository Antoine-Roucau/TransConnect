using System;
using System.Collections.Generic;

namespace Transconnect.Models
{

    public enum StatutCommande
    {
        EnAttente,
        EnCours,
        Livree,
        Payee,
        Annulee
    }

    public class Commande
    {
        #region Propriétés
        int id;
        static int nextId;
        string villeDepart;
        string villeArrivee;
        DateTime date;
        decimal prix;
        StatutCommande statut = StatutCommande.EnAttente;

        
        Client? client;
        Salarie? chauffeur;
        Vehicule? vehicule;
        #endregion
        
        #region Constructeurs
        public Commande(int id, string villeDepart, string villeArrivee, DateTime date, decimal prix)
        {
            this.id = id;
            this.villeDepart = villeDepart;
            this.villeArrivee = villeArrivee;
            this.date = date;
            this.prix = prix;
        }

        public Commande(string villeDepart, string villeArrivee, DateTime date,decimal prix)
        {
            this.id = nextId;
            nextId++;
            this.villeDepart = villeDepart;
            this.villeArrivee = villeArrivee;
            this.date = date;
            this.prix = prix;
        }

        #endregion

        #region Getters et Setters
        public int Id 
        {
            get { return id; }
        }
        public static int NextId 
        {
            get { return nextId; }
            set { nextId = value; }
        }
        public string VilleDepart 
        { 
            get { return villeDepart; } 
            set { villeDepart = value; } 
        }
        public string VilleArrivee 
        { 
            get { return villeArrivee; } 
            set { villeArrivee = value; } 
        }
        public DateTime Date 
        { 
            get { return date; } 
            set { date = value; } 
        }
        public decimal Prix 
        { 
            get { return prix; } 
            set { prix = value; } 
        }
        public StatutCommande Statut 
        { 
            get { return statut; } 
            set { statut = value; } 
        }
        public Client? Client 
        { 
            get { return client; } 
            set { client = value; } 
        }
        public Salarie? Chauffeur 
        { 
            get { return chauffeur; } 
            set { chauffeur = value; } 
        }
        public Vehicule? Vehicule 
        { 
            get { return vehicule; } 
            set { vehicule = value; } 
        }
        #endregion

        #region Méthodes
        public bool EstValide()
        {
            return client != null && chauffeur != null && vehicule != null;
        }
        public string AfficherInfos()
        {
            return $"Commande {id} : {villeDepart} -> {villeArrivee} le {date.ToShortDateString()} \nPrix: {prix:C} \nStatut: {statut}";
        }
        public void ChangerStatut(StatutCommande nouveauStatut)
        {
            if (statut == StatutCommande.Annulee)
                throw new InvalidOperationException("Impossible de modifier une commande annulée");
            
            if (statut == StatutCommande.Payee && nouveauStatut != StatutCommande.Payee)
                throw new InvalidOperationException("Impossible de modifier une commande déjà payée");
            
            statut = nouveauStatut;
        }
        #endregion
    }
}