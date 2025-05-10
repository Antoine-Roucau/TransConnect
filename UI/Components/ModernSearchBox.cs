using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Transconnect.UI.Helpers;

namespace Transconnect.UI.Components
{
    /// <summary>
    /// Champ de recherche moderne avec icône intégrée
    /// </summary>
    public class ModernSearchBox : TextBox
    {
        private Color _borderColor = UIColors.LightGrayColor;
        private Color _focusBorderColor = UIColors.PrimaryColor;
        private int _borderRadius = 4;
        private Color _iconColor = UIColors.MediumGrayColor;
        private bool _hasClearButton = true;
        private bool _isHoveringClear = false;
        private Rectangle _clearButtonRect;
        private bool _isFocused = false;
        
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
        /// Couleur de la bordure lorsque le contrôle a le focus
        /// </summary>
        public Color FocusBorderColor
        {
            get => _focusBorderColor;
            set
            {
                _focusBorderColor = value;
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
        /// Couleur de l'icône de recherche
        /// </summary>
        public Color IconColor
        {
            get => _iconColor;
            set
            {
                _iconColor = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Détermine si un bouton d'effacement est affiché
        /// </summary>
        public bool HasClearButton
        {
            get => _hasClearButton;
            set
            {
                _hasClearButton = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Texte affiché comme placeholder quand le champ est vide
        /// </summary>
        public string PlaceholderText { get; set; } = "Rechercher...";
        
        public ModernSearchBox()
        {
            // Configuration initiale
            SetStyle(
                ControlStyles.UserPaint | 
                ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.OptimizedDoubleBuffer | 
                ControlStyles.ResizeRedraw, 
                true);
            
            // Propriétés par défaut
            BorderStyle = BorderStyle.None;
            BackColor = UIColors.CardColor;
            Font = UIFonts.Body;
            Height = 30;
            Padding = new Padding(30, 5, _hasClearButton ? 30 : 5, 5);
            
            // Initialiser le rectangle du bouton d'effacement
            UpdateClearButtonRect();
        }
        
        private void UpdateClearButtonRect()
        {
            _clearButtonRect = new Rectangle(Width - 25, (Height - 16) / 2, 16, 16);
        }
        
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Height = Math.Max(30, Height); // Garantir une hauteur minimale
            UpdateClearButtonRect();
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Créer le rectangle pour le fond
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            
            // Sélectionner la couleur de bordure
            Color currentBorderColor = _isFocused ? _focusBorderColor : _borderColor;
            
            // Dessiner le fond avec coins arrondis
            using (GraphicsPath path = CreateRoundedRectangle(rect, _borderRadius))
            {
                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    g.FillPath(brush, path);
                }
                
                using (Pen pen = new Pen(currentBorderColor))
                {
                    g.DrawPath(pen, path);
                }
            }
            
            // Dessiner l'icône de recherche
            DrawSearchIcon(g, new Rectangle(8, (Height - 16) / 2, 16, 16), _iconColor);
            
            // Dessiner le bouton d'effacement si nécessaire
            if (_hasClearButton && !string.IsNullOrEmpty(Text))
            {
                DrawClearButton(g, _clearButtonRect, _isHoveringClear ? UIColors.DarkGrayColor : _iconColor);
            }
            
            // Dessiner le placeholder si le texte est vide
            if (string.IsNullOrEmpty(Text) && !Focused && !string.IsNullOrEmpty(PlaceholderText))
            {
                using (SolidBrush brush = new SolidBrush(UIColors.MediumGrayColor))
                using (StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center })
                {
                    Rectangle placeholderRect = new Rectangle(30, 0, Width - 35, Height);
                    g.DrawString(PlaceholderText, Font, brush, placeholderRect, sf);
                }
            }
            
            base.OnPaint(e);
        }
        
        protected override void OnGotFocus(EventArgs e)
        {
            _isFocused = true;
            base.OnGotFocus(e);
            Invalidate();
        }
        
        protected override void OnLostFocus(EventArgs e)
        {
            _isFocused = false;
            base.OnLostFocus(e);
            Invalidate();
        }
        
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }
        
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            if (_hasClearButton && !string.IsNullOrEmpty(Text))
            {
                bool hovering = _clearButtonRect.Contains(e.Location);
                if (hovering != _isHoveringClear)
                {
                    _isHoveringClear = hovering;
                    Invalidate();
                }
            }
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            if (_hasClearButton && !string.IsNullOrEmpty(Text) && _clearButtonRect.Contains(e.Location))
            {
                Text = string.Empty;
                Focus();
            }
        }
        
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            
            if (_isHoveringClear)
            {
                _isHoveringClear = false;
                Invalidate();
            }
        }
        
        private void DrawSearchIcon(Graphics g, Rectangle rect, Color color)
        {
            using (Pen pen = new Pen(color, 2))
            {
                // Dessiner le cercle
                g.DrawEllipse(pen, rect.X, rect.Y, 12, 12);
                
                // Dessiner la ligne
                g.DrawLine(pen, rect.X + 10, rect.Y + 10, rect.X + 16, rect.Y + 16);
            }
        }
        
        private void DrawClearButton(Graphics g, Rectangle rect, Color color)
        {
            using (Pen pen = new Pen(color, 2))
            {
                // Dessiner un X
                g.DrawLine(pen, rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
                g.DrawLine(pen, rect.X + rect.Width, rect.Y, rect.X, rect.Y + rect.Height);
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
    }
}