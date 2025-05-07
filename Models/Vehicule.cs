using System;

namespace Transconnect.Models
{
    public enum TypeVehicule
    {
        Voiture,
        Camionnette,
        CamionCiterne,
        CamionBenne,
        CamionFrigorifique
    }

    public class Vehicule
    {
        #region Propriétés
        string immatriculation;
        TypeVehicule type;
        string modele;
        int capacite;
        bool estDisponible = true;
        string? specificiteVehicule;
        decimal tarifKilometrique;
        #endregion
        
        #region Constructeurs
        public Vehicule(string immatriculation, TypeVehicule type, string modele, int capacite, decimal tarifKilometrique)
        {
            this.immatriculation = immatriculation;
            this.type = type;
            this.modele = modele;
            this.capacite = capacite;
            this.tarifKilometrique = tarifKilometrique;
        }
        #endregion

        #region Getters et Setters
        public string Immatriculation 
        {
            get { return immatriculation; }
            set { immatriculation = value; }
        }
        public TypeVehicule Type 
        { 
            get { return type; } 
            set { type = value; } 
        }
        public string Modele 
        { 
            get { return modele; } 
            set { modele = value; } 
        }
        public int Capacite 
        { 
            get { return capacite; } 
            set { capacite = value; } 
        }
        public bool EstDisponible 
        { 
            get { return estDisponible; } 
            set { estDisponible = value; } 
        }
        public decimal TarifKilometrique 
        { 
            get { return tarifKilometrique; } 
            set { tarifKilometrique = value; } 
        }
        public string? SpecificiteVehicule 
        { 
            get { return specificiteVehicule; } 
            set { specificiteVehicule = value; } 
        }
        #endregion

        #region Méthodes
        public string AfficherInfos()
        {
            string description = $"{type} {modele} - {immatriculation}";
            switch (type)
            {
                case TypeVehicule.Voiture:
                    description += $" - {capacite} passagers";
                    break;
                case TypeVehicule.Camionnette:
                    description += $" - Usage: {specificiteVehicule}";
                    break;
                case TypeVehicule.CamionCiterne:
                    description += $" - Matière transportée: {specificiteVehicule}";
                    break;
                case TypeVehicule.CamionBenne:
                    description += $" - Type: {specificiteVehicule}";
                    break;
                case TypeVehicule.CamionFrigorifique:
                    description += $" - Température: {specificiteVehicule}";
                    break;
            }
            return description;
        }
        #endregion
    }
}