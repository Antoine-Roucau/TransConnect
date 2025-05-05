using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using TransConnect.Models;
using Transconnect.Services;
using TransConnect.Models.Graphe;
using TransConnect.UI;

namespace TransConnect.Data
{
    public class DataInitializer
    {
        #region  Propriétés
        public List<Salarie> salaries = new List<Salarie>();
        public List<Vehicule> vehicules = new List<Vehicule>();
        public List<Client> clients = new List<Client>();
        public List<String> villes = new List<String>();
        public List<Commande> commandes = new List<Commande>();

        public Graphe grapheSalarie = new Graphe(new Salarie("GrapheSalarie", "GrapheSalarie", "GrapheSalarie", DateTime.Now, "GrapheSalarie", "GrapheSalarie", "GrapheSalarie", DateTime.Now, "GrapheSalarie", 0));
        public Graphe grapheVille = new Graphe("Ville");
        #endregion
        #region Constructeur
        public DataInitializer()
        {
            InitialiserSalaries();
            InitialiserVehicules();
            InitialiserClients();
            InitialiserVilles();
            InitialiserCommandes();
            InitialiserGrapheSalarie();
            InitialiserGrapheVille();
        }

        private void InitialiserSalaries()
        {
            TextReader reader = new StreamReader("Data/CSV/Salarie.csv");
            string line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                string[] colonnes = line.Split(';');
                Salarie salarie = new Salarie(colonnes[0], colonnes[1], colonnes[2], Convert.ToDateTime(colonnes[3]), colonnes[4], colonnes[5], colonnes[6], Convert.ToDateTime(colonnes[7]), colonnes[8], Convert.ToDecimal(colonnes[9]));
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
                string[] colonnes = line.Split(';');
                Vehicule vehicule = new Vehicule(colonnes[0], ParseEnum<TypeVehicule>(colonnes[1]), colonnes[2], Convert.ToInt32(colonnes[3]), Convert.ToDecimal(colonnes[4]));
                vehicule.EstDisponible = Convert.ToBoolean(colonnes[5]);
                vehicule.SpecificiteVehicule = colonnes[6];
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
                string[] colonnes = line.Split(';');
                Client client = new Client(colonnes[0], colonnes[1], colonnes[2], Convert.ToDateTime(colonnes[3]), colonnes[4], colonnes[5], colonnes[6]);
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
                villes.Add(line);
            }
            reader.Close();
        }

        private void InitialiserCommandes()
        {
            TextReader reader = new StreamReader("Data/CSV/Commande.csv");
            string line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                string[] colonnes = line.Split(';');
                Commande commande = new Commande(Convert.ToInt32(colonnes[0]), colonnes[1], colonnes[2],Convert.ToDateTime(colonnes[3]),Convert.ToDecimal(colonnes[4]));
                commande.Statut = ParseEnum<StatutCommande>(colonnes[5]);
                Client client = clients.Find(c => c.NumeroSS == colonnes[6]);
                commande.Client = client;
                commande.Chauffeur = salaries.Find(s => s.NumeroSS == colonnes[7]);
                commande.Vehicule = vehicules.Find(v => v.Immatriculation == colonnes[8]);
                commandes.Add(commande);
                client.AddCommande(commande);
            }
            reader.Close();
        }
        
        private void InitialiserGrapheSalarie()
        {
            this.grapheSalarie.Racine = new Noeud(this.salaries[0]);
            
            List<String[]> liens = new List<String[]>();
            TextReader reader = new StreamReader("Data/CSV/Hierachie.csv");
            string line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                string[] colonnes = line.Split(';');
                liens.Add(colonnes);
            }
            reader.Close();
            
            Dictionary<string, Noeud> mapSalarieNoeud = new Dictionary<string, Noeud>();
            
            if (this.grapheSalarie.Racine.Entite is Salarie)
            {
                mapSalarieNoeud.Add(((Salarie)this.grapheSalarie.Racine.Entite).NumeroSS, this.grapheSalarie.Racine);
            }
            
            foreach (var salarie in this.salaries)
            {
                if (salarie != this.salaries[0])
                {
                    Noeud noeud = new Noeud(salarie);
                    this.grapheSalarie.AjouterNoeud(noeud);
                    mapSalarieNoeud.Add(salarie.NumeroSS, noeud);
                }
            }
            foreach (var relation in liens)
            {
                if (mapSalarieNoeud.TryGetValue(relation[0], out Noeud manager) && mapSalarieNoeud.TryGetValue(relation[1], out Noeud subordonne))
                {
                    Lien lien = new Lien(manager, subordonne, null, true);
                    this.grapheSalarie.AjouterLien(lien);
                    
                    if (manager.Entite is Salarie managerSalarie && subordonne.Entite is Salarie subordonneSalarie)
                    {
                        managerSalarie.AddSubordonnes(subordonneSalarie);
                    }
                }
            }
        }
        
        private void InitialiserGrapheVille()
        {

            this.grapheVille.Racine = new Noeud(this.villes[0]);
            

            for (int i = 0; i < this.villes.Count; i++)
            {
                Noeud noeud = new Noeud(this.villes[i]);
                this.grapheVille.AjouterNoeud(noeud);
            }
            
            List<String[]> liens = new List<String[]>();
            TextReader reader = new StreamReader("Data/CSV/distances_villes_france.csv");
            string line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                string[] colonnes = line.Split(';');
                liens.Add(colonnes);
            }
            reader.Close();
            
            for (int i = 0; i < liens.Count; i++)
            {
                Noeud noeud1 = this.grapheVille.TrouverNoeudVille(liens[i][0]);
                Noeud noeud2 = this.grapheVille.TrouverNoeudVille(liens[i][1]);
                
                if (noeud1 != null && noeud2 != null)
                {
                    double distance = Convert.ToDouble(liens[i][2], CultureInfo.InvariantCulture);
                    Lien lien = new Lien(noeud1, noeud2, distance, null);
                    this.grapheVille.AjouterLien(lien);
                }
            }
        }
        #endregion
        #region Méthodes
        public static T ParseEnum<T>(string value)
        {
            switch (typeof(T).Name)
            {
                case "TypeVehicule":
                    switch (value)
                    {
                        case "0":
                            return (T) Enum.Parse(typeof(TypeVehicule), "Voiture", true);
                        case "1":
                            return (T) Enum.Parse(typeof(TypeVehicule), "Camionnette", true);
                        case "2":
                            return (T) Enum.Parse(typeof(TypeVehicule), "CamionCiterne", true);
                        case "3":
                            return (T) Enum.Parse(typeof(TypeVehicule), "CamionBenne", true);
                        case "4":
                            return (T) Enum.Parse(typeof(TypeVehicule), "CamionFrigorifique", true);
                    }
                    return (T) Enum.Parse(typeof(TypeVehicule), value, true);
                case "StatutCommande":
                    switch (value)
                    {
                        case "0":
                            return (T) Enum.Parse(typeof(StatutCommande), "EnAttente", true);
                        case "1":
                            return (T) Enum.Parse(typeof(StatutCommande), "EnCours", true);
                        case "2":
                            return (T) Enum.Parse(typeof(StatutCommande), "Livree", true);
                        case "3":
                            return (T) Enum.Parse(typeof(StatutCommande), "Payee", true);
                        case "4":
                            return (T) Enum.Parse(typeof(StatutCommande), "Annulee", true);
                    }
                    return (T) Enum.Parse(typeof(StatutCommande), value, true);
                default:
                    throw new ArgumentException($"PB Enum");
            }
        }

        public void AfficherGrapheVilleGraphique()
        {
            var visualiseur = new GrapheVisualiseur(grapheVille);
            visualiseur.AfficherGraphe();
        }

        public void AfficherGrapheSalarieGraphique()
        {
            var visualiseur = new OrganigrammeVisualiseur(grapheSalarie);
            visualiseur.AfficherOrganigramme();
        }

        #endregion
    }
}