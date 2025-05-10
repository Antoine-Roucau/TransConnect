using System;
using System.Drawing;

namespace Transconnect.UI.Helpers
{
    /// <summary>
    /// Classe statique définissant les couleurs utilisées dans l'application
    /// </summary>
    public static class UIColors
    {
        // Couleurs principales
        public static readonly Color PrimaryColor = Color.FromArgb(44, 110, 171);     // #2C6EAB - Bleu
        public static readonly Color SecondaryColor = Color.FromArgb(61, 175, 127);   // #3DAF7F - Vert
        public static readonly Color AccentColor = Color.FromArgb(245, 166, 35);      // #F5A623 - Orange

        // Couleurs neutres
        public static readonly Color BackgroundColor = Color.FromArgb(248, 249, 250); // #F8F9FA - Fond principal
        public static readonly Color CardColor = Color.White;                         // #FFFFFF - Cartes, panneaux
        public static readonly Color LightGrayColor = Color.FromArgb(233, 236, 239);  // #E9ECEF - Séparateurs
        public static readonly Color MediumGrayColor = Color.FromArgb(108, 117, 125); // #6C757D - Texte secondaire
        public static readonly Color DarkGrayColor = Color.FromArgb(52, 58, 64);      // #343A40 - Texte principal
        
        // Couleurs sémantiques
        public static readonly Color SuccessColor = Color.FromArgb(40, 167, 69);      // #28A745 - Succès
        public static readonly Color WarningColor = Color.FromArgb(255, 193, 7);      // #FFC107 - Avertissement
        public static readonly Color DangerColor = Color.FromArgb(220, 53, 69);       // #DC3545 - Danger
        public static readonly Color InfoColor = Color.FromArgb(23, 162, 184);        // #17A2B8 - Information
        
        // Couleurs par module
        public static readonly Color ClientModuleColor = Color.FromArgb(91, 106, 191);   // #5B6ABF - Violet-bleu
        public static readonly Color CommandeModuleColor = Color.FromArgb(230, 126, 34); // #E67E22 - Orange foncé
        public static readonly Color SalarieModuleColor = Color.FromArgb(41, 128, 185);  // #2980B9 - Bleu ciel
        public static readonly Color StatistiqueModuleColor = Color.FromArgb(142, 68, 173); // #8E44AD - Violet
        public static readonly Color VisualisationModuleColor = Color.FromArgb(79, 70, 229); // #4F46E5 - Indigo
        
        // Méthodes utilitaires pour les couleurs
        
        /// <summary>
        /// Crée une version plus foncée d'une couleur
        /// </summary>
        public static Color Darken(Color color, double factor = 0.2)
        {
            int r = Math.Max(0, (int)(color.R * (1 - factor)));
            int g = Math.Max(0, (int)(color.G * (1 - factor)));
            int b = Math.Max(0, (int)(color.B * (1 - factor)));
            return Color.FromArgb(color.A, r, g, b);
        }
        
        /// <summary>
        /// Crée une version plus claire d'une couleur
        /// </summary>
        public static Color Lighten(Color color, double factor = 0.2)
        {
            int r = Math.Min(255, (int)(color.R + (255 - color.R) * factor));
            int g = Math.Min(255, (int)(color.G + (255 - color.G) * factor));
            int b = Math.Min(255, (int)(color.B + (255 - color.B) * factor));
            return Color.FromArgb(color.A, r, g, b);
        }
        
        /// <summary>
        /// Crée une couleur semi-transparente
        /// </summary>
        public static Color WithAlpha(Color color, int alpha)
        {
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }
    }
}