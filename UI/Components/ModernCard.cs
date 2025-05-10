using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Transconnect.UI.Helpers;

namespace Transconnect.UI.Components
{
    /// <summary>
    /// Conteneur de type carte avec ombre et coins arrondis
    /// </summary>
    public class ModernCard : Panel
    {
        private int _borderRadius = 8;
        private int _shadowDepth = 3;
        private Color _shadowColor = Color.FromArgb(50, 0, 0, 0);
        private Color _borderColor = Color.FromArgb(230, 230, 230);
        private bool _drawBorder = true;
        private Color _headerColor = Color.Transparent;
        private string _headerText = string.Empty;
        private bool _hasHeader = false;
        private int _headerHeight = 40;
        
        /// <summary>
        /// Rayon des coins arrondis
        /// </summary>
        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Profondeur de l'ombre (0 pour aucune ombre)
        /// </summary>
        public int ShadowDepth
        {
            get => _shadowDepth;
            set
            {
                _shadowDepth = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Couleur de l'ombre
        /// </summary>
        public Color ShadowColor
        {
            get => _shadowColor;
            set
            {
                _shadowColor = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Couleur de la bordure
        /// </summary>
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Détermine si une bordure doit être dessinée
        /// </summary>
        public bool DrawBorder
        {
            get => _drawBorder;
            set
            {
                _drawBorder = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Couleur d'arrière-plan de l'en-tête (si activé)
        /// </summary>
        public Color HeaderColor
        {
            get => _headerColor;
            set
            {
                _headerColor = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Texte de l'en-tête (si activé)
        /// </summary>
        public string HeaderText
        {
            get => _headerText;
            set
            {
                _headerText = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Détermine si la carte a un en-tête
        /// </summary>
        public bool HasHeader
        {
            get => _hasHeader;
            set
            {
                _hasHeader = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Hauteur de l'en-tête (si activé)
        /// </summary>
        public int HeaderHeight
        {
            get => _headerHeight;
            set
            {
                _headerHeight = value;
                Invalidate();
            }
        }
        
        public ModernCard()
        {
            // Configuration initiale
            SetStyle(
                ControlStyles.UserPaint | 
                ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.OptimizedDoubleBuffer | 
                ControlStyles.ResizeRedraw, 
                true);
            
            // Propriétés par défaut
            BackColor = UIColors.CardColor;
            Padding = new Padding(15);
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Création du rectangle pour le contenu
            Rectangle rect = new Rectangle(
                _shadowDepth, 
                _shadowDepth, 
                Width - (_shadowDepth * 2) - 1, 
                Height - (_shadowDepth * 2) - 1);
            
            // Créer le chemin pour les coins arrondis
            using (GraphicsPath path = CreateRoundedRectangle(rect, _borderRadius))
            {
                // Dessiner l'ombre si nécessaire
                if (_shadowDepth > 0)
                {
                    for (int i = 1; i <= _shadowDepth; i++)
                    {
                        using (GraphicsPath shadowPath = CreateRoundedRectangle(
                            new Rectangle(i, i, rect.Width, rect.Height), _borderRadius))
                        {
                            using (Pen shadowPen = new Pen(Color.FromArgb(
                                (10 * (_shadowDepth - i + 1)) / _shadowDepth, _shadowColor), 1))
                            {
                                g.DrawPath(shadowPen, shadowPath);
                            }
                        }
                    }
                }
                
                // Remplir le fond
                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    g.FillPath(brush, path);
                }
                
                // Dessiner la bordure si nécessaire
                if (_drawBorder)
                {
                    using (Pen pen = new Pen(_borderColor, 1))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                
                // Dessiner l'en-tête si activé
                if (_hasHeader)
                {
                    Rectangle headerRect = new Rectangle(
                        rect.X, 
                        rect.Y, 
                        rect.Width, 
                        _headerHeight);
                    
                    using (GraphicsPath headerPath = CreateRoundedRectangle(headerRect, _borderRadius, true))
                    {
                        using (SolidBrush brush = new SolidBrush(_headerColor != Color.Transparent ? 
                            _headerColor : UIColors.PrimaryColor))
                        {
                            g.FillPath(brush, headerPath);
                        }
                        
                        // Dessiner le texte de l'en-tête
                        if (!string.IsNullOrEmpty(_headerText))
                        {
                            using (StringFormat sf = new StringFormat {
                                Alignment = StringAlignment.Near,
                                LineAlignment = StringAlignment.Center
                            })
                            {
                                headerRect.X += 15; // Padding pour le texte
                                headerRect.Width -= 30;
                                g.DrawString(_headerText, UIFonts.SubtitleSize > 0 ? 
                                    UIFonts.Subtitle : new Font("Segoe UI", 16, FontStyle.Bold), 
                                    Brushes.White, headerRect, sf);
                            }
                        }
                    }
                    
                    // Ligne de séparation
                    g.DrawLine(new Pen(_borderColor, 1), 
                        rect.X, rect.Y + _headerHeight, 
                        rect.X + rect.Width, rect.Y + _headerHeight);
                }
            }
        }
        
        /// <summary>
        /// Crée un GraphicsPath avec des coins arrondis
        /// </summary>
        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius, bool onlyTopRounded = false)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            
            // Limiter le rayon à la moitié de la plus petite dimension
            diameter = Math.Min(diameter, Math.Min(rect.Width, rect.Height));
            radius = diameter / 2;
            
            Rectangle arc = new Rectangle(rect.X, rect.Y, diameter, diameter);
            
            // Coin supérieur gauche
            path.AddArc(arc, 180, 90);
            
            // Coin supérieur droit
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            
            if (onlyTopRounded)
            {
                // Pour l'en-tête, on ne veut que les coins supérieurs arrondis
                path.AddLine(rect.Right, rect.Bottom, rect.Left, rect.Bottom);
            }
            else
            {
                // Coin inférieur droit
                arc.Y = rect.Bottom - diameter;
                path.AddArc(arc, 0, 90);
                
                // Coin inférieur gauche
                arc.X = rect.Left;
                path.AddArc(arc, 90, 90);
            }
            
            path.CloseFigure();
            return path;
        }
        
        /// <summary>
        /// Crée une carte avec un en-tête
        /// </summary>
        public static ModernCard CreateWithHeader(string headerText, Color headerColor)
        {
            ModernCard card = new ModernCard
            {
                HasHeader = true,
                HeaderText = headerText,
                HeaderColor = headerColor,
                Padding = new Padding(15, 15 + 40, 15, 15) // Ajuster le padding pour l'en-tête
            };
            
            return card;
        }
    }
}