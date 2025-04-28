using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
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
                string[] colonnes = line.Split(';');
                decimal salaire = decimal.Parse(colonnes[9].Replace(",", "."), CultureInfo.InvariantCulture);
                Salarie salarie = new Salarie(colonnes[0], colonnes[1], colonnes[2], Convert.ToDateTime(colonnes[3]), 
                    colonnes[4], colonnes[5], colonnes[6], Convert.ToDateTime(colonnes[7]), colonnes[8], salaire);
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
                decimal tarifKm = decimal.Parse(colonnes[4].Replace(",", "."), CultureInfo.InvariantCulture);
                Vehicule vehicule = new Vehicule(colonnes[0], ParseEnum<TypeVehicule>(colonnes[1]), colonnes[2], 
                    Convert.ToInt32(colonnes[3]), tarifKm);
                vehicule.EstDisponible = Convert.ToBoolean(colonnes[5]);
                
                // Vérifier si la spécificité est null ou vide
                string specificite = colonnes[6];
                if (!string.IsNullOrEmpty(specificite) && specificite.ToLower() != "null")
                {
                    // Enlever les guillemets s'ils existent
                    if (specificite.StartsWith("\"") && specificite.EndsWith("\""))
                    {
                        specificite = specificite.Substring(1, specificite.Length - 2);
                    }
                    vehicule.SpecificiteVehicule = specificite;
                }
                
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
                string adressePostale = colonnes[4];
                
                // Traiter l'adresse qui peut contenir des points-virgules entre guillemets
                if (adressePostale.StartsWith("\"") && !adressePostale.EndsWith("\""))
                {
                    // L'adresse contient des points-virgules, reconstituer l'adresse complète
                    for (int i = 5; i < colonnes.Length - 2; i++)
                    {
                        adressePostale += ";" + colonnes[i];
                        if (adressePostale.EndsWith("\""))
                            break;
                    }
                }
                
                // Enlever les guillemets
                if (adressePostale.StartsWith("\"") && adressePostale.EndsWith("\""))
                {
                    adressePostale = adressePostale.Substring(1, adressePostale.Length - 2);
                }
                
                // Les deux dernières colonnes sont toujours email et téléphone
                string email = colonnes[colonnes.Length - 2];
                string telephone = colonnes[colonnes.Length - 1];
                
                Client client = new Client(colonnes[0], colonnes[1], colonnes[2], Convert.ToDateTime(colonnes[3]), 
                    adressePostale, email, telephone);
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
        
        private void InitialiserGrapheSalarie()
        {
            // Trouver le directeur général (par convention devrait être le premier salarié)
            this.grapheSalarie.Racine = new Noeud(this.salaries[0]);
            
            // Ajouter les autres salariés
            for (int i = 1; i < this.salaries.Count; i++)
            {
                Noeud noeud = new Noeud(this.salaries[i]);
                this.grapheSalarie.AjouterNoeud(noeud);
            }
            
            // Charger les liens hiérarchiques
            List<String[]> liens = new List<String[]>();
            TextReader reader = new StreamReader("Data/CSV/Hierachie.csv");
            string line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                string[] colonnes = line.Split(';');
                liens.Add(colonnes);
            }
            reader.Close();
            
            // Créer les liens
            for (int i = 0; i < liens.Count; i++)
            {
                // Correction de l'indice: Utiliser l'indice 2 (3ème colonne) pour le subordonné
                // Format: NumeroSS_employe1;poste1;NumeroSS_employe2;poste2
                Noeud noeud1 = this.grapheSalarie.TrouverNoeudParSalarieNumeroSS(liens[i][0]);
                Noeud noeud2 = this.grapheSalarie.TrouverNoeudParSalarieNumeroSS(liens[i][2]); // Corrigé: indice 2 au lieu de 1
                
                if (noeud1 != null && noeud2 != null)
                {
                    Lien lien = new Lien(noeud1, noeud2, null, true);
                    this.grapheSalarie.AjouterLien(lien);
                }
            }
        }
        
        private void InitialiserGrapheVille()
        {
            // Rechercher Paris pour en faire la capitale
            int indexParis = -1;
            for (int i = 0; i < villes.Count; i++)
            {
                if (villes[i].Equals("Paris", StringComparison.OrdinalIgnoreCase))
                {
                    indexParis = i;
                    break;
                }
            }
            
            // Si Paris n'est pas trouvée, utiliser la première ville
            int indexCapitale = (indexParis >= 0) ? indexParis : 0;
            
            // Définir la capitale comme racine
            this.grapheVille.Racine = new Noeud(this.villes[indexCapitale]);
            
            // Ajouter les autres villes
            for (int i = 0; i < this.villes.Count; i++)
            {
                if (i != indexCapitale) // Ne pas ajouter la capitale deux fois
                {
                    Noeud noeud = new Noeud(this.villes[i]);
                    this.grapheVille.AjouterNoeud(noeud);
                }
            }
            
            // Charger les distances
            List<String[]> liens = new List<String[]>();
            TextReader reader = new StreamReader("Data/CSV/distances_villes_france.csv");
            string line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                string[] colonnes = line.Split(';');
                liens.Add(colonnes);
            }
            reader.Close();
            
            // Créer les liens
            for (int i = 0; i < liens.Count; i++)
            {
                Noeud noeud1 = this.grapheVille.TrouverNoeudVille(liens[i][0]);
                Noeud noeud2 = this.grapheVille.TrouverNoeudVille(liens[i][1]);
                
                if (noeud1 != null && noeud2 != null)
                {
                    double distance = Convert.ToDouble(liens[i][2], CultureInfo.InvariantCulture);
                    Lien lien = new Lien(noeud1, noeud2, distance, null);
                    
                    // Vérifier si le lien existe déjà pour éviter les doublons
                    if (!this.grapheVille.LiensExistants(lien))
                    {
                        this.grapheVille.AjouterLien(lien);
                    }
                }
            }
        }
        
        public static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }

        public void AfficherGrapheSalarie()
        {
            grapheSalarie.AfficherGraphe();
        }
        
        public void AfficherGrapheVille()
        {
            grapheVille.AfficherGraphe();
        }
    }
}