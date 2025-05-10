using System;
using System.Drawing;

namespace Transconnect.UI.Helpers
{
    /// <summary>
    /// Classe statique définissant les polices utilisées dans l'application
    /// </summary>
    public static class UIFonts
    {
        // Nom de la police principale
        private const string DefaultFontFamily = "Segoe UI"; // Police standard sur Windows
        private const string FallbackFontFamily = "Arial";   // Police de secours
        
        // Tailles de police
        public const float TitleSize = 20f;         // Titres principaux
        public const float SubtitleSize = 16f;      // Titres secondaires
        public const float HeadingSize = 14f;       // Sous-titres
        public const float BodySize = 13f;          // Corps de texte
        public const float SecondaryTextSize = 12f; // Texte secondaire
        public const float SmallTextSize = 11f;     // Petit texte
        public const float TinyTextSize = 9f;       // Très petit texte
        
        // Méthodes pour obtenir des polices avec des styles différents
        
        /// <summary>
        /// Obtient la police pour les titres principaux
        /// </summary>
        public static Font Title => new Font(DefaultFontFamily, TitleSize, FontStyle.Bold);
        
        /// <summary>
        /// Obtient la police pour les titres secondaires
        /// </summary>
        public static Font Subtitle => new Font(DefaultFontFamily, SubtitleSize, FontStyle.Bold);
        
        /// <summary>
        /// Obtient la police pour les sous-titres
        /// </summary>
        public static Font Heading => new Font(DefaultFontFamily, HeadingSize, FontStyle.Bold);
        
        /// <summary>
        /// Obtient la police pour le corps de texte
        /// </summary>
        public static Font Body => new Font(DefaultFontFamily, BodySize, FontStyle.Regular);
        
        /// <summary>
        /// Obtient la police pour le texte secondaire
        /// </summary>
        public static Font SecondaryText => new Font(DefaultFontFamily, SecondaryTextSize, FontStyle.Regular);
        
        /// <summary>
        /// Obtient la police pour le petit texte
        /// </summary>
        public static Font SmallText => new Font(DefaultFontFamily, SmallTextSize, FontStyle.Regular);
        
        /// <summary>
        /// Obtient la police pour le très petit texte
        /// </summary>
        public static Font TinyText => new Font(DefaultFontFamily, TinyTextSize, FontStyle.Regular);
        
        /// <summary>
        /// Crée une police personnalisée avec la taille et le style spécifiés
        /// </summary>
        public static Font CreateFont(float size, FontStyle style = FontStyle.Regular)
        {
            return new Font(DefaultFontFamily, size, style);
        }
        
        /// <summary>
        /// Vérifie si la police est disponible, sinon utilise la police de secours
        /// </summary>
        public static Font GetSafeFont(string fontName, float size, FontStyle style = FontStyle.Regular)
        {
            try
            {
                // Essayer de créer la police demandée
                using (Font testFont = new Font(fontName, size, style))
                {
                    // Si la police est créée avec succès, retourner une nouvelle instance
                    return new Font(fontName, size, style);
                }
            }
            catch
            {
                // Si la police n'est pas disponible, utiliser la police de secours
                return new Font(FallbackFontFamily, size, style);
            }
        }
    }
}