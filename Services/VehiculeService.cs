using System;
using System.Data;
using Transconnect.Models;

namespace Transconnect.Services
{
    public class VehiculeService
    {
        public DataTable GetVehiculesDF(List<Vehicule> vehiculeList)
        {
            DataTable dfVehicule=new DataTable();
            dfVehicule.Columns.Add("Immatriculation", typeof(string));
            dfVehicule.Columns.Add("Type", typeof(string));
            dfVehicule.Columns.Add("Modèle", typeof(string));
            dfVehicule.Columns.Add("Capacité", typeof(int));
            dfVehicule.Columns.Add("Est Disponible", typeof(bool));
            dfVehicule.Columns.Add("Spécificité", typeof(string));
            dfVehicule.Columns.Add("Tarif Kilométrique", typeof(decimal));

            foreach (Vehicule v in vehiculeList)
            {
                dfVehicule.Rows.Add(v.Immatriculation, v.Type.ToString(), v.Modele, v.Capacite, v.EstDisponible, v.SpecificiteVehicule, v.TarifKilometrique);
            }
            return dfVehicule;
        }
        
        public void AjouterVehicule(Vehicule vehicule, List<Vehicule> vehiculeList)
        {
            foreach (Vehicule v in vehiculeList)
            {
                if (v.Immatriculation == vehicule.Immatriculation)
                {
                    throw new Exception("Le véhicule existe déjà.");
                }
            }
            vehiculeList.Add(vehicule);
        }

        public void SupprimerVehicule(Vehicule vehicule, List<Vehicule> vehiculeList)
        {
            if (!vehiculeList.Contains(vehicule))
            {
                throw new Exception("Le véhicule n'existe pas.");
            }
            vehiculeList.Remove(vehicule);
        }
        
        public void ModifierVehicule(Vehicule vehicule, List<Vehicule> vehiculeList)
        {
            for (int i = 0; i < vehiculeList.Count; i++)
            {
                if (vehiculeList[i].Immatriculation == vehicule.Immatriculation)
                {
                    vehiculeList[i] = vehicule;
                    return;
                }
            }
            throw new Exception("Le véhicule n'existe pas.");
        }

        public Vehicule TrouverVehicule(string immatriculation, List<Vehicule> vehiculeList)
        {
            foreach (Vehicule v in vehiculeList)
            {
                if (v.Immatriculation == immatriculation)
                {
                    return v;
                }
            }
            throw new Exception("Le véhicule n'existe pas.");
        }
    }
}