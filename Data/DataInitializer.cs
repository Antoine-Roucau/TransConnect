using System;
using System.Collections.Generic;
using TransConnect.Models;


namespace TransConnect.Data
{
    public class DataInitializer
    {
        List<Salarie> salaries = new List<Salarie>();
        List<Vehicule> vehicules = new List<Vehicule>();
        List<Commande> commandes = new List<Commande>();
        List<Client> clients = new List<Client>();


        public DataInitializer()
        {

            InitialiserSalaries();
            InitialiserVehicules();
            InitialiserClients();
        }

        private void InitialiserSalaries()
        {
            TextReader reader = new StreamReader("Data/CSV/Salarie.csv");
            string line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                string[] collonnes = line.Split(';');
                Salarie salarie = new Salarie(collonnes[0], collonnes[1], collonnes[2], Convert.ToDateTime(collonnes[3]),collonnes[4], collonnes[5], collonnes[6], Convert.ToDateTime(collonnes[7]), collonnes[8], Convert.ToDecimal(collonnes[9]));
                salaries.Add(salarie);
                
            }
            reader.Close();
        }

        private void InitialiserVehicules()
        {
            TextReader reader = new StreamReader("Data/CSV/Vehicule.csv");
            string line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                string[] collonnes = line.Split(';');
                Vehicule vehicule = new Vehicule(collonnes[0], ParseEnum<TypeVehicule>(collonnes[1]), collonnes[2],Convert.ToInt32(collonnes[3]), Convert.ToDecimal(collonnes[4]));
                vehicule.EstDisponible = Convert.ToBoolean(collonnes[5]);
                vehicule.SpecificiteVehicule = collonnes[6];
                vehicules.Add(vehicule);
            }
            reader.Close();
        }

        private void InitialiserClients()
        {
            TextReader reader = new StreamReader("Data/CSV/Client.csv");
            string line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                string[] collonnes = line.Split(';');
                Client client = new Client(collonnes[0], collonnes[1], collonnes[2], Convert.ToDateTime(collonnes[3]), collonnes[4], collonnes[5],collonnes[6]);
                clients.Add(client);
            }
            reader.Close();
        }


        public static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }

        public void AfficherSalaries()
        {
            foreach (var salarie in salaries)
            {
                Console.WriteLine(salarie.AfficherInfos());
            }
        }

        public void AfficherVehicules()
        {
            foreach (var vehicule in vehicules)
            {
                Console.WriteLine(vehicule.AfficherInfos());
            }
        }

        public void AfficherClients()
        {
            foreach (var client in clients)
            {
                Console.WriteLine(client.AfficherInfos());
            }
        }


    }
}