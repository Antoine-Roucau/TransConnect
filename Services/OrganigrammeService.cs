using System;
using System.Collections.Generic;
using Transconnect.Models;

namespace Transconnect.Services
{
    public class OrganigrammeService
    {
        public Dictionary<Salarie, List<Salarie>> graphe;

        public OrganigrammeService(Dictionary<Salarie, List<Salarie>> grapheInitial)
        {
            graphe = grapheInitial;
        }

        public void AjouterRelation(Salarie manager, Salarie subordonne)
        {
            if (!graphe.ContainsKey(manager))
            {
                graphe[manager] = new List<Salarie>();
            }

            if (!graphe[manager].Contains(subordonne))
            {
                graphe[manager].Add(subordonne);
            }
        }

        public void SupprimerRelation(Salarie manager, Salarie subordonne)
        {
            if (graphe.ContainsKey(manager))
            {
                graphe[manager].Remove(subordonne);
            }
        }

        public List<Salarie> ObtenirSubordonnes(Salarie manager)
        {
            if (graphe.ContainsKey(manager))
            {
                return graphe[manager];
            }

            return new List<Salarie>();
        }

        // Vérifie si un salarié est subordonné à un autre
        public bool EstSubordonne(Salarie manager, Salarie subordonne)
        {
            // Vérifie d'abord si le manager est dans le graphe
            if (!graphe.ContainsKey(manager))
            {
                return false;
            }

            // Parcourt les subordonnés directs
            foreach (var s in graphe[manager])
            {
                if (s == subordonne)
                {
                    return true; 
                }
                // Appel récursif : cherche dans les subordonnés du subordonné
            if (EstSubordonne(s, subordonne))
                {
                    return true; // subordonné indirect trouvé
                }
            }
            return false;
        }

        public Salarie TrouverSuperieur(Salarie subordonne)
        {
            foreach (var entry in graphe)
            {
                if (entry.Value.Contains(subordonne))
                {
                    return entry.Key;
                }
            }
            return null;
        }
    }
}
