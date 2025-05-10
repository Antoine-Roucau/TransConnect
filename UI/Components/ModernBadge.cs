using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Transconnect.UI.Helpers;

namespace Transconnect.UI.Components
{
    /// <summary>
    /// Badge visuel pour afficher des statuts ou notifications
    /// </summary>
    public class ModernBadge : Control
    {
        private string _text = "";
        private Color _badgeColor = UIColors.PrimaryColor;
        private Color _textColor = Color.White;
        private int _borderRadius = 10;
        private BadgeStyle _style = BadgeStyle.Standard;
        private int _padding = 5;
        private bool _autoSize = true;
        
        /// <summary>
        /// Texte affiché dans le badge
        /// </summary>
        public override string Text
        {
            get => _text;
            set
            {
                _text = value;
                if (_autoSize)
                    RecalculateSize();
                Invalidate();
            }
        }
        
        /// <summary>
        /// Couleur d'arrière-plan du badge
        /// </summary>
        public Color BadgeColor
        {
            get => _badgeColor;
            set
            {
                _badgeColor = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Couleur du texte
        /// </summary>
        public Color TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                Invalidate();
            }
        }
        
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
        /// Style du badge
        /// </summary>
        public BadgeStyle Style
        {
            get => _style;
            set
            {
                _style = value;
                
                // Mise à jour des couleurs selon le style
                switch (_style)
                {
                    case BadgeStyle.Success:
                        _badgeColor = UIColors.SuccessColor;
                        _textColor = Color.White;
                        break;
                    case BadgeStyle.Warning:
                        _badgeColor = UIColors.WarningColor;
                        _textColor = Color.White;
                        break;
                    case BadgeStyle.Danger:
                        _badgeColor = UIColors.DangerColor;
                        _textColor = Color.White;
                        break;
                    case BadgeStyle.Info:
                        _badgeColor = UIColors.InfoColor;
                        _textColor = Color.White;
                        break;
                    case BadgeStyle.Primary:
                        _badgeColor = UIColors.PrimaryColor;
                        _textColor = Color.White;
                        break;
                    case BadgeStyle.Secondary:
                        _badgeColor = UIColors.SecondaryColor;
                        _textColor = Color.White;
                        break;
                    case BadgeStyle.Outlined:
                        _badgeColor = Color.Transparent;
                        _textColor = UIColors.PrimaryColor;
                        break;
                }
                
                Invalidate();
            }
        }
        
        /// <summary>
        /// Padding interne du badge
        /// </summary>
        public int BadgePadding
        {
            get => _padding;
            set
            {
                _padding = value;
                if (_autoSize)
                    RecalculateSize();
                Invalidate();
            }
        }
        
        /// <summary>
        /// Détermine si le badge s'adapte automatiquement à son contenu
        /// </summary>
        public bool AutoSize
        {
            get => _autoSize;
            set
            {
                _autoSize = value;
                if (_autoSize)
                    RecalculateSize();
            }
        }
        
        public ModernBadge()
        {
            // Configuration initiale
            SetStyle(
                ControlStyles.UserPaint | 
                ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.OptimizedDoubleBuffer | 
                ControlStyles.ResizeRedraw, 
                true);
            
            // Propriétés par défaut
            Font = UIFonts.SmallText;
            Size = new Size(80, 24);
        }
        
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            if (_autoSize)
                RecalculateSize();
        }
        
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (_autoSize)
                RecalculateSize();
        }
        
        private void RecalculateSize()
        {
            if (string.IsNullOrEmpty(_text)) return;
            
            using (Graphics g = CreateGraphics())
            {
                SizeF textSize = g.MeasureString(_text, Font);
                Size = new Size(
                    (int)Math.Ceiling(textSize.Width) + (_padding * 2) + 4,
                    (int)Math.Ceiling(textSize.Height) + (_padding * 2));
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Adapter le radius à la hauteur du badge
            int radius = Math.Min(_borderRadius, Height / 2);
            
            // Créer le rectangle pour le badge
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            
            // Créer le chemin pour les coins arrondis
            using (GraphicsPath path = CreateRoundedRectangle(rect, radius))
            {
                // Remplir le fond
                using (SolidBrush brush = new SolidBrush(_badgeColor))
                {
                    g.FillPath(brush, path);
                }
                
                // Dessiner la bordure si nécessaire
                if (_style == BadgeStyle.Outlined)
                {
                    using (Pen pen = new Pen(_textColor))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                
                // Dessiner le texte
                if (!string.IsNullOrEmpty(_text))
                {
                    using (StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    })
                    {
                        using (SolidBrush brush = new SolidBrush(_textColor))
                        {
                            g.DrawString(_text, Font, brush, rect, sf);
                        }
                    }
                }
            }
        }
        
        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
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
            
            // Coin inférieur droit
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            
            // Coin inférieur gauche
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            
            path.CloseFigure();
            return path;
        }
        
        /// <summary>
        /// Crée un badge de statut
        /// </summary>
        public static ModernBadge CreateStatusBadge(string text, BadgeStyle style)
        {
            ModernBadge badge = new ModernBadge
            {
                Text = text,
                Style = style,
                AutoSize = true
            };
            
            return badge;
        }
    }
    
    /// <summary>
    /// Types de styles pour le badge
    /// </summary>
    public enum BadgeStyle
    {
        Standard,
        Primary,
        Secondary,
        Success,
        Warning,
        Danger,
        Info,
        Outlined
    }
}