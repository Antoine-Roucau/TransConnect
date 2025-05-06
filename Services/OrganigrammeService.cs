using System;
using System.Collections.Generic;
using TransConnect.Models;

namespace TransConnect.Services
{
    public class OrganigrammeService
    {
        private Dictionary<Salarie, List<Salarie>> graphe;

        public OrganigrammeService(Dictionary<Salarie, List<Salarie>> grapheInitial)
        {
            graphe = grapheInitial;
        }

        // Ajouter une relation hiérarchique
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

        // Supprimer une relation hiérarchique
        public void SupprimerRelation(Salarie manager, Salarie subordonne)
        {
            if (graphe.ContainsKey(manager))
            {
                graphe[manager].Remove(subordonne);
            }
        }

        // Obtenir les subordonnés directs d’un salarié
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
                    return true; // subordonné direct trouvé
                }
                // Appel récursif : cherche dans les subordonnés du subordonné
            if (EstSubordonne(s, subordonne))
                {
                    return true; // subordonné indirect trouvé
                }
            }
            // Aucun lien trouvé
            return false;
        }
    }
}
