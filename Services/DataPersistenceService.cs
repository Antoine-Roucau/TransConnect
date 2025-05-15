using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using Transconnect.Models;

namespace Transconnect.Services
{
    public class DataPersistenceService
    {
        private static readonly string ClientsCsvPath = "Data/CSV/Client.csv";
        private static readonly string SalariesCsvPath = "Data/CSV/Salarie.csv";
        private static readonly string CommandesCsvPath = "Data/CSV/Commande.csv";
        private static readonly string VehiculesCsvPath = "Data/CSV/Vehicule.csv";
        private static readonly string HierarchieCsvPath = "Data/CSV/Hierachie.csv";
        
        public static void SaveClients(List<Client> clients)
        {
            try
            {
                string header = "NumeroSS;Nom;Prenom;DateNaissance;AdressePostale;AdresseMail;Telephone";
                using (StreamWriter writer = new StreamWriter(ClientsCsvPath))
                {
                    writer.WriteLine(header);
                    
                    foreach (var client in clients)
                    {
                        string line = $"{client.NumeroSS};{client.Nom};{client.Prenom};{client.DateNaissance:yyyy-MM-dd};{client.AdressePostale};{client.AdresseMail};{client.Telephone}";
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving clients to CSV: {ex.Message}", ex);
            }
        }
        
        public static void SaveSalaries(List<Salarie> salaries)
        {
            try
            {
                string header = "NumeroSS;Nom;Prenom;DateNaissance;AdressePostale;AdresseMail;Telephone;DateEntree;Poste;Salaire";
                using (StreamWriter writer = new StreamWriter(SalariesCsvPath))
                {
                    writer.WriteLine(header);
                    
                    foreach (var salarie in salaries)
                    {
                        writer.WriteLine(salarie.Serialize());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving salaries to CSV: {ex.Message}", ex);
            }
        }
        
        public static void SaveHierarchie(List<Salarie> salaries)
        {
            try
            {
                string header = "NumeroSS_employe1;NumeroSS_employe2";
                using (StreamWriter writer = new StreamWriter(HierarchieCsvPath))
                {
                    writer.WriteLine(header);
                    
                    foreach (var manager in salaries)
                    {
                        foreach (var subordinate in manager.Subordonnes)
                        {
                            string line = $"{manager.NumeroSS};{subordinate.NumeroSS}";
                            writer.WriteLine(line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving hierarchy to CSV: {ex.Message}", ex);
            }
        }
        
        public static void SaveCommandes(List<Commande> commandes)
        {
            try
            {
                string header = "Id;VilleDepart;VilleArrivee;Date;Prix;Statut;NumeroSSClient;NumeroSSChauffeur;VehiculeImmatriculation";
                using (StreamWriter writer = new StreamWriter(CommandesCsvPath))
                {
                    writer.WriteLine(header);
                    
                    foreach (var commande in commandes)
                    {
                        int statutValue = (int)commande.Statut;
                        
                        string prixFormatted = commande.Prix.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
                        
                        string line = $"{commande.Id};{commande.VilleDepart};{commande.VilleArrivee};{commande.Date:yyyy-MM-dd};{prixFormatted};{statutValue};{commande.Client?.NumeroSS ?? ""};{commande.Chauffeur?.NumeroSS ?? ""};{commande.Vehicule?.Immatriculation ?? ""}";
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving commandes to CSV: {ex.Message}", ex);
            }
        }
        
        public static void SaveVehicules(List<Vehicule> vehicules)
        {
            try
            {
                string header = "Immatriculation;Type;Modele;Capacite;TarifKilometrique;EstDisponible;SpecificiteVehicule";
                using (StreamWriter writer = new StreamWriter(VehiculesCsvPath))
                {
                    writer.WriteLine(header);
                    
                    foreach (var vehicule in vehicules)
                    {
                        int typeValue = (int)vehicule.Type;
                        
                        string tarifFormatted = vehicule.TarifKilometrique.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
                        
                        string line = $"{vehicule.Immatriculation};{typeValue};{vehicule.Modele};{vehicule.Capacite};{tarifFormatted};{vehicule.EstDisponible.ToString().ToLower()};{vehicule.SpecificiteVehicule ?? "-"}";
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving vehicules to CSV: {ex.Message}", ex);
            }
        }
    }
}