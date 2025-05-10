using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Transconnect.UI.Helpers;

namespace Transconnect.UI.Components
{
    /// <summary>
    /// Barre d'outils moderne avec boutons et contrôles intégrés
    /// </summary>
    public class ModernToolbar : Panel
    {
        private int _buttonSpacing = 5;
        private int _buttonHeight = 32;
        private Color _backgroundColor = UIColors.CardColor;
        private Color _borderColor = UIColors.LightGrayColor;
        private bool _drawBorder = true;
        private bool _drawShadow = true;
        private ToolbarAlignment _alignment = ToolbarAlignment.Left;
        private List<Control> _toolbarItems = new List<Control>();
        
        /// <summary>
        /// Espacement entre les boutons
        /// </summary>
        public int ButtonSpacing
        {
            get => _buttonSpacing;
            set
            {
                _buttonSpacing = value;
                ArrangeItems();
                Invalidate();
            }
        }
        
        /// <summary>
        /// Hauteur des boutons
        /// </summary>
        public int ButtonHeight
        {
            get => _buttonHeight;
            set
            {
                _buttonHeight = value;
                ArrangeItems();
                Invalidate();
            }
        }
        
        /// <summary>
        /// Couleur d'arrière-plan de la barre d'outils
        /// </summary>
        public Color ToolbarBackColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                BackColor = value;
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
        /// Détermine si une bordure est dessinée
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
        /// Détermine si une ombre est dessinée
        /// </summary>
        public bool DrawShadow
        {
            get => _drawShadow;
            set
            {
                _drawShadow = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Alignment des éléments dans la barre d'outils
        /// </summary>
        public ToolbarAlignment Alignment
        {
            get => _alignment;
            set
            {
                _alignment = value;
                ArrangeItems();
                Invalidate();
            }
        }
        
        public ModernToolbar()
        {
            // Configuration initiale
            SetStyle(
                ControlStyles.UserPaint | 
                ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.OptimizedDoubleBuffer | 
                ControlStyles.ResizeRedraw, 
                true);
            
            // Propriétés par défaut
            BackColor = _backgroundColor;
            BorderStyle = BorderStyle.None;
            Height = _buttonHeight + 10; // Hauteur par défaut
            Padding = new Padding(5);
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Dessiner le fond
            g.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            
            // Dessiner la bordure si nécessaire
            if (_drawBorder)
            {
                using (Pen pen = new Pen(_borderColor))
                {
                    g.DrawLine(pen, 0, Height - 1, Width, Height - 1);
                }
            }
            
            // Dessiner l'ombre si nécessaire
            if (_drawShadow)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    new Point(0, Height - 3),
                    new Point(0, Height),
                    Color.FromArgb(20, 0, 0, 0),
                    Color.Transparent))
                {
                    g.FillRectangle(brush, new Rectangle(0, Height - 3, Width, 3));
                }
            }
        }
        
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ArrangeItems();
        }
        
        /// <summary>
        /// Ajoute un bouton à la barre d'outils
        /// </summary>
        public Button AddButton(string text, Image image = null, EventHandler clickHandler = null)
        {
            Button btn = new Button
            {
                Text = text,
                Image = image,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = UIColors.MediumGrayColor,
                Height = _buttonHeight,
                AutoSize = false,
                Padding = new Padding(10, 0, 10, 0),
                UseVisualStyleBackColor = true
            };
            
            // Calculer la largeur en fonction du texte
            using (Graphics g = CreateGraphics())
            {
                SizeF textSize = g.MeasureString(text, btn.Font);
                int imageWidth = image != null ? image.Width + 5 : 0;
                btn.Width = (int)textSize.Width + imageWidth + 20; // 20 = padding
            }
            
            btn.FlatAppearance.BorderSize = 0;
            
            if (clickHandler != null)
                btn.Click += clickHandler;
            
            _toolbarItems.Add(btn);
            Controls.Add(btn);
            
            ArrangeItems();
            
            return btn;
        }
        
        /// <summary>
        /// Ajoute un séparateur à la barre d'outils
        /// </summary>
        public Panel AddSeparator()
        {
            Panel separator = new Panel
            {
                BackColor = _borderColor,
                Width = 1,
                Height = _buttonHeight - 10,
                Margin = new Padding(5)
            };
            
            _toolbarItems.Add(separator);
            Controls.Add(separator);
            
            ArrangeItems();
            
            return separator;
        }
        
        /// <summary>
        /// Ajoute un contrôle personnalisé à la barre d'outils
        /// </summary>
        public T AddControl<T>(T control) where T : Control
        {
            _toolbarItems.Add(control);
            Controls.Add(control);
            
            ArrangeItems();
            
            return control;
        }
        
        /// <summary>
        /// Arrange les éléments dans la barre d'outils
        /// </summary>
        private void ArrangeItems()
        {
            if (_toolbarItems.Count == 0) return;
            
            int x = Padding.Left;
            int rightAlignedStartX = Width - Padding.Right;
            
            // Centrer verticalement les éléments
            int y = (Height - _buttonHeight) / 2;
            
            // Calculer la largeur totale pour le centrage
            int totalWidth = 0;
            foreach (Control item in _toolbarItems)
            {
                totalWidth += item.Width + _buttonSpacing;
            }
            totalWidth -= _buttonSpacing; // Retirer le dernier espacement
            
            // Point de départ pour l'alignement centré
            int centerStartX = (Width - totalWidth) / 2;
            
            if (_alignment == ToolbarAlignment.Center && centerStartX > 0)
                x = centerStartX;
            
            // Ranger les éléments alignés à gauche
            List<Control> rightAlignedItems = new List<Control>();
            
            foreach (Control item in _toolbarItems)
            {
                if (item.Tag != null && item.Tag.ToString() == "right")
                {
                    rightAlignedItems.Add(item);
                    continue;
                }
                
                item.Location = new Point(x, y);
                x += item.Width + _buttonSpacing;
            }
            
            // Ranger les éléments alignés à droite (de droite à gauche)
            foreach (Control item in rightAlignedItems)
            {
                rightAlignedStartX -= item.Width;
                item.Location = new Point(rightAlignedStartX, y);
                rightAlignedStartX -= _buttonSpacing;
            }
        }
        
        /// <summary>
        /// Marque un contrôle comme aligné à droite
        /// </summary>
        public void SetRightAligned(Control control)
        {
            if (Controls.Contains(control))
            {
                control.Tag = "right";
                ArrangeItems();
            }
        }
    }
    
    /// <summary>
    /// Types d'alignement pour la barre d'outils
    /// </summary>
    public enum ToolbarAlignment
    {
        Left,
        Center,
        Right
    }
}