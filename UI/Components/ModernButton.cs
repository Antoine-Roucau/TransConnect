using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Transconnect.UI.Helpers;

namespace Transconnect.UI.Components
{
    /// <summary>
    /// Bouton personnalisé avec un style moderne et des animations
    /// </summary>
    public class ModernButton : Button
    {
        // Propriétés pour la personnalisation
        private Color _baseColor = UIColors.PrimaryColor;
        private Color _hoverColor;
        private Color _pressedColor;
        private int _borderRadius = 5;
        private bool _isHovering = false;
        private bool _isPressed = false;
        private Image _buttonImage = null;
        private ContentAlignment _imageAlign = ContentAlignment.MiddleLeft;
        private int _imagePadding = 8;
        
        // Couleurs calculées
        private Color _textColor = Color.White;
        private Color _currentColor;
        
        /// <summary>
        /// Couleur principale du bouton
        /// </summary>
        public Color BaseColor
        {
            get => _baseColor;
            set
            {
                _baseColor = value;
                _hoverColor = UIColors.Darken(value, 0.1);
                _pressedColor = UIColors.Darken(value, 0.2);
                _currentColor = value;
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
        /// Image à afficher sur le bouton
        /// </summary>
        public new Image Image
        {
            get => _buttonImage;
            set
            {
                _buttonImage = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Alignement de l'image
        /// </summary>
        public new ContentAlignment ImageAlign
        {
            get => _imageAlign;
            set
            {
                _imageAlign = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Espacement entre l'image et le texte
        /// </summary>
        public int ImagePadding
        {
            get => _imagePadding;
            set
            {
                _imagePadding = value;
                Invalidate();
            }
        }
        
        public ModernButton()
        {
            // Configuration initiale du bouton
            SetStyle(
                ControlStyles.UserPaint | 
                ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.OptimizedDoubleBuffer | 
                ControlStyles.ResizeRedraw, 
                true);
            
            _hoverColor = UIColors.Darken(_baseColor, 0.1);
            _pressedColor = UIColors.Darken(_baseColor, 0.2);
            _currentColor = _baseColor;
            
            // Configurer l'apparence du bouton
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Font = UIFonts.Body;
            Cursor = Cursors.Hand;
            Size = new Size(120, 35);
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Déterminer la couleur actuelle en fonction de l'état
            Color buttonColor = Enabled 
                ? (_isPressed ? _pressedColor : (_isHovering ? _hoverColor : _baseColor)) 
                : Color.FromArgb(180, _baseColor);
            
            // Créer le fond du bouton avec coins arrondis
            using (GraphicsPath path = CreateRoundedRectangle(ClientRectangle, _borderRadius))
            {
                using (SolidBrush brush = new SolidBrush(buttonColor))
                {
                    g.FillPath(brush, path);
                }
            }
            
            // Calculer les rectangles pour le texte et l'image
            Rectangle textRect = new Rectangle(0, 0, Width, Height);
            Rectangle imageRect = Rectangle.Empty;
            
            // Dessiner l'image si présente
            if (_buttonImage != null)
            {
                imageRect = CalculateImageRectangle(textRect, _imageAlign, _buttonImage.Size);
                g.DrawImage(_buttonImage, imageRect);
                
                // Ajuster le rectangle du texte en fonction de la position de l'image
                switch (_imageAlign)
                {
                    case ContentAlignment.MiddleLeft:
                        textRect.X += imageRect.Width + _imagePadding;
                        textRect.Width -= imageRect.Width + _imagePadding;
                        break;
                    case ContentAlignment.MiddleRight:
                        textRect.Width -= imageRect.Width + _imagePadding;
                        break;
                }
            }
            
            // Dessiner le texte
            TextRenderer.DrawText(g, Text, Font, textRect, _textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
        
        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovering = true;
            Invalidate();
            base.OnMouseEnter(e);
        }
        
        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovering = false;
            Invalidate();
            base.OnMouseLeave(e);
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            _isPressed = true;
            Invalidate();
            base.OnMouseDown(e);
        }
        
        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }
        
        protected override void OnEnabledChanged(EventArgs e)
        {
            Invalidate();
            base.OnEnabledChanged(e);
        }
        
        private Rectangle CalculateImageRectangle(Rectangle clientRect, ContentAlignment alignment, Size imageSize)
        {
            int x = 0, y = 0;
            
            // Calculer la position horizontale
            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    x = _imagePadding;
                    break;
                    
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    x = (clientRect.Width - imageSize.Width) / 2;
                    break;
                    
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    x = clientRect.Width - imageSize.Width - _imagePadding;
                    break;
            }
            
            // Calculer la position verticale
            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopRight:
                    y = _imagePadding;
                    break;
                    
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleRight:
                    y = (clientRect.Height - imageSize.Height) / 2;
                    break;
                    
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomRight:
                    y = clientRect.Height - imageSize.Height - _imagePadding;
                    break;
            }
            
            return new Rectangle(x, y, imageSize.Width, imageSize.Height);
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