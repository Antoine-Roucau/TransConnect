using System;
using System.Collections.Generic;
using TransConnect.Models;
using TransConnect.Models.Graphe;


namespace TransConnect.Data
{
    public class DataInitializer
    {
        List<Salarie> salaries = new List<Salarie>();
        List<Vehicule> vehicules = new List<Vehicule>();
        List<Client> clients = new List<Client>();
        List<String> villes = new List<String>();
        List<Commande> commandes = new List<Commande>();

        Graphe grapheSalarie = new Graphe(new Salarie("GrapheSalarie", "GrapheSalarie", "GrapheSalarie", DateTime.Now, "GrapheSalarie", "GrapheSalarie", "GrapheSalarie", DateTime.Now, "GrapheSalarie", 0));
        Graphe grapheVille = new Graphe("Ville");
        public DataInitializer()
        {

            InitialiserSalaries();
            InitialiserVehicules();
            InitialiserClients();
            InitialiserVilles();
            InitialiserGrapheSalarie();
            InitialiserGrapheVille();
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
        private void InitialiserVilles()
        {
            TextReader reader = new StreamReader("Data/CSV/Ville.csv");
            string line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                string[] collonnes = line.Split(';');
                villes.Add(collonnes[0]);
            }
            reader.Close();
        }
        private void InitialiserGrapheSalarie()
        {
            this.grapheSalarie.Racine = new Noeud(this.salaries[0]);
            for (int i = 1; i < this.salaries.Count; i++)
            {
                Noeud noeud = new Noeud(this.salaries[i]);
                this.grapheSalarie.AjouterNoeud(noeud);
            }
            List<String[]> liens = new List<String[]>();
            TextReader reader = new StreamReader("Data/CSV/Hierachie.csv");
            string line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                string[] collonnes = line.Split(';');
                liens.Add(collonnes);
            }
            reader.Close();


        }
        private void InitialiserGrapheVille()
        {
            this.grapheVille.Racine = new Noeud(this.villes[0]);
            for (int i = 1; i < this.villes.Count; i++)
            {
                Noeud noeud = new Noeud(this.villes[i]);
                this.grapheVille.AjouterNoeud(noeud);
            }
            List<String[]> liens = new List<String[]>();
            TextReader reader = new StreamReader("Data/CSV/distances_villes_france.csv");
            string line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                string[] collonnes = line.Split(';');
                liens.Add(collonnes);
            }
            reader.Close();
            for (int i = 0; i < liens.Count; i++)
            {
                Noeud noeud1 = this.grapheVille.TrouverNoeudVille(liens[i][0]);
                Noeud noeud2 = this.grapheVille.TrouverNoeudVille(liens[i][1]);
                if (noeud1 != null && noeud2 != null)
                {
                    Lien lien = new Lien(noeud1, noeud2, Convert.ToDouble(liens[i][2]), null);
                    this.grapheVille.AjouterLien(lien);
                }
            }
        }
        public static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }

        public void AfficherGrapheVille()
        {
            grapheVille.AfficherGraphe();
        }

    }
}