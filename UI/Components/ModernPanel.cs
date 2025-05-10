using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using Transconnect.UI.Helpers;

namespace Transconnect.UI.Components
{
    /// <summary>
    /// Panneau moderne avec coins arrondis, ombre, et en-tête optionnel
    /// </summary>
    public class ModernPanel : Panel
    {
        #region Propriétés privées
        private int _cornerRadius = 8;
        private bool _showShadow = true;
        private int _shadowDepth = 3;
        private Color _shadowColor = Color.FromArgb(60, 0, 0, 0);
        private bool _showHeader = false;
        private int _headerHeight = 40;
        private Color _headerColor = UIColors.PrimaryColor;
        private string _headerText = "";
        private Font _headerFont;
        private Color _headerTextColor = Color.White;
        private Color _borderColor = Color.FromArgb(230, 230, 230);
        private bool _drawBorder = true;
        private int _borderWidth = 1;
        private bool _collapsible = false;
        private bool _collapsed = false;
        private int _expandedHeight;
        private System.Windows.Forms.Timer _animationTimer;
        private int _targetHeight;
        private bool _animating = false;
        private int _animationStep = 30;
        private Button _collapseButton;
        #endregion

        #region Propriétés publiques
        /// <summary>
        /// Rayon des coins arrondis
        /// </summary>
        [Category("Appearance"), Description("Rayon des coins arrondis")]
        public int CornerRadius
        {
            get => _cornerRadius;
            set
            {
                _cornerRadius = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Indique si l'ombre doit être affichée
        /// </summary>
        [Category("Appearance"), Description("Indique si l'ombre doit être affichée")]
        public bool ShowShadow
        {
            get => _showShadow;
            set
            {
                _showShadow = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Profondeur de l'ombre
        /// </summary>
        [Category("Appearance"), Description("Profondeur de l'ombre")]
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
        [Category("Appearance"), Description("Couleur de l'ombre")]
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
        /// Indique si l'en-tête doit être affiché
        /// </summary>
        [Category("Appearance"), Description("Indique si l'en-tête doit être affiché")]
        public bool ShowHeader
        {
            get => _showHeader;
            set
            {
                _showHeader = value;
                UpdateHeaderLayout();
                Invalidate();
            }
        }

        /// <summary>
        /// Hauteur de l'en-tête
        /// </summary>
        [Category("Appearance"), Description("Hauteur de l'en-tête")]
        public int HeaderHeight
        {
            get => _headerHeight;
            set
            {
                _headerHeight = value;
                UpdateHeaderLayout();
                Invalidate();
            }
        }

        /// <summary>
        /// Couleur de l'en-tête
        /// </summary>
        [Category("Appearance"), Description("Couleur de l'en-tête")]
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
        /// Texte de l'en-tête
        /// </summary>
        [Category("Appearance"), Description("Texte de l'en-tête")]
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
        /// Police du texte de l'en-tête
        /// </summary>
        [Category("Appearance"), Description("Police du texte de l'en-tête")]
        public Font HeaderFont
        {
            get => _headerFont;
            set
            {
                _headerFont = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Couleur du texte de l'en-tête
        /// </summary>
        [Category("Appearance"), Description("Couleur du texte de l'en-tête")]
        public Color HeaderTextColor
        {
            get => _headerTextColor;
            set
            {
                _headerTextColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Couleur de la bordure
        /// </summary>
        [Category("Appearance"), Description("Couleur de la bordure")]
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
        /// Indique si la bordure doit être dessinée
        /// </summary>
        [Category("Appearance"), Description("Indique si la bordure doit être dessinée")]
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
        /// Épaisseur de la bordure
        /// </summary>
        [Category("Appearance"), Description("Épaisseur de la bordure")]
        public int BorderWidth
        {
            get => _borderWidth;
            set
            {
                _borderWidth = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Indique si le panneau peut être replié
        /// </summary>
        [Category("Behavior"), Description("Indique si le panneau peut être replié")]
        public bool Collapsible
        {
            get => _collapsible;
            set
            {
                _collapsible = value;
                UpdateCollapseButton();
                Invalidate();
            }
        }

        /// <summary>
        /// Indique si le panneau est replié
        /// </summary>
        [Category("Behavior"), Description("Indique si le panneau est replié")]
        public bool Collapsed
        {
            get => _collapsed;
            set
            {
                if (_collapsed != value)
                {
                    _collapsed = value;
                    if (_collapsible)
                    {
                        if (_collapsed)
                            CollapsePanel();
                        else
                            ExpandPanel();
                    }
                }
            }
        }
        #endregion

        #region Événements
        /// <summary>
        /// Se produit lorsque le panneau est replié ou déplié
        /// </summary>
        public event EventHandler<CollapsedChangedEventArgs> CollapsedChanged;
        #endregion

        #region Constructeur
        public ModernPanel()
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
            Padding = new Padding(8);
            _headerFont = UIFonts.CreateFont(UIFonts.SubtitleSize, FontStyle.Bold);

            // Configurer le timer d'animation
            _animationTimer = new System.Windows.Forms.Timer
            {
                Interval = 10,
                Enabled = false
            };
            _animationTimer.Tick += AnimationTimer_Tick;

            // Initialiser la hauteur développée
            _expandedHeight = Height;
        }
        #endregion

        #region Méthodes Override
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Déterminer les dimensions et la position du contenu
            Rectangle contentRect = new Rectangle(
                _showShadow ? _shadowDepth : 0,
                _showShadow ? _shadowDepth : 0,
                Width - (_showShadow ? _shadowDepth * 2 : 0),
                Height - (_showShadow ? _shadowDepth * 2 : 0)
            );

            // Créer le chemin pour les coins arrondis
            using (GraphicsPath path = CreateRoundedRectangle(contentRect, _cornerRadius))
            {
                // Dessiner l'ombre si nécessaire
                if (_showShadow && !_animating)
                {
                    using (PathGradientBrush shadowBrush = new PathGradientBrush(path))
                    {
                        shadowBrush.CenterColor = _shadowColor;
                        shadowBrush.SurroundColors = new Color[] { Color.Transparent };
                        shadowBrush.FocusScales = new PointF(0.95f, 0.95f);
                        
                        using (Matrix matrix = new Matrix())
                        {
                            matrix.Translate(_shadowDepth / 2, _shadowDepth / 2);
                            shadowBrush.Transform = matrix;
                            g.FillPath(shadowBrush, path);
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
                    using (Pen pen = new Pen(_borderColor, _borderWidth))
                    {
                        g.DrawPath(pen, path);
                    }
                }

                // Dessiner l'en-tête si nécessaire
                if (_showHeader)
                {
                    Rectangle headerRect = new Rectangle(
                        contentRect.X,
                        contentRect.Y,
                        contentRect.Width,
                        _headerHeight
                    );

                    using (GraphicsPath headerPath = CreateRoundedRectangle(headerRect, _cornerRadius, true))
                    {
                        using (SolidBrush brush = new SolidBrush(_headerColor))
                        {
                            g.FillPath(brush, headerPath);
                        }

                        // Dessiner le texte de l'en-tête
                        if (!string.IsNullOrEmpty(_headerText))
                        {
                            using (SolidBrush brush = new SolidBrush(_headerTextColor))
                            using (StringFormat sf = new StringFormat
                            {
                                Alignment = StringAlignment.Near,
                                LineAlignment = StringAlignment.Center
                            })
                            {
                                Rectangle textRect = new Rectangle(
                                    headerRect.X + 10,
                                    headerRect.Y,
                                    headerRect.Width - 20,
                                    headerRect.Height
                                );
                                g.DrawString(_headerText, _headerFont ?? Font, brush, textRect, sf);
                            }
                        }
                    }

                    // Ligne de séparation
                    g.DrawLine(new Pen(_borderColor, _borderWidth),
                        contentRect.X,
                        contentRect.Y + _headerHeight,
                        contentRect.X + contentRect.Width,
                        contentRect.Y + _headerHeight);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!_animating && !_collapsed)
            {
                _expandedHeight = Height;
            }
            UpdateHeaderLayout();
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            UpdateHeaderLayout();
            UpdateCollapseButton();
            if (!DesignMode)
            {
                _expandedHeight = Height;
            }
        }
        
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (_showHeader)
            {
                if (e.Control != _collapseButton)
                {
                    e.Control.Location = new Point(
                        e.Control.Location.X,
                        e.Control.Location.Y + _headerHeight
                    );
                }
            }
        }
        #endregion

        #region Méthodes privées
        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius, bool topOnly = false)
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

            if (topOnly)
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

        private void UpdateHeaderLayout()
        {
            if (_showHeader)
            {
                // Ajuster le Padding pour tenir compte de l'en-tête
                Padding = new Padding(
                    Padding.Left,
                    Padding.Top + _headerHeight,
                    Padding.Right,
                    Padding.Bottom
                );
            }
            else
            {
                // Restaurer le Padding par défaut
                Padding = new Padding(8);
            }

            UpdateCollapseButton();
        }

        private void UpdateCollapseButton()
        {
            if (_collapsible && _showHeader)
            {
                if (_collapseButton == null)
                {
                    _collapseButton = new Button
                    {
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(24, 24),
                        Text = _collapsed ? "+" : "-",
                        ForeColor = _headerTextColor,
                        BackColor = Color.Transparent,
                        Cursor = Cursors.Hand,
                        UseVisualStyleBackColor = true
                    };
                    _collapseButton.FlatAppearance.BorderSize = 0;
                    _collapseButton.Click += CollapseButton_Click;
                    Controls.Add(_collapseButton);
                }

                _collapseButton.Location = new Point(
                    Width - _collapseButton.Width - 10,
                    (_headerHeight - _collapseButton.Height) / 2
                );
                _collapseButton.BringToFront();
                _collapseButton.Visible = true;
            }
            else if (_collapseButton != null)
            {
                _collapseButton.Visible = false;
            }
        }

        private void CollapseButton_Click(object sender, EventArgs e)
        {
            Collapsed = !Collapsed;
        }

        private void CollapsePanel()
        {
            if (!_animating)
            {
                _animating = true;
                _targetHeight = _headerHeight + 1;
                _animationTimer.Start();
            }
        }

        private void ExpandPanel()
        {
            if (!_animating)
            {
                _animating = true;
                _targetHeight = _expandedHeight;
                _animationTimer.Start();
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (_collapsed)
            {
                // Animation de repliement
                if (Height > _targetHeight)
                {
                    Height -= _animationStep;
                    if (Height <= _targetHeight)
                    {
                        Height = _targetHeight;
                        _animating = false;
                        _animationTimer.Stop();
                        OnCollapsedChanged(EventArgs.Empty);
                    }
                }
                else
                {
                    _animating = false;
                    _animationTimer.Stop();
                }
            }
            else
            {
                // Animation de dépliement
                if (Height < _targetHeight)
                {
                    Height += _animationStep;
                    if (Height >= _targetHeight)
                    {
                        Height = _targetHeight;
                        _animating = false;
                        _animationTimer.Stop();
                        OnCollapsedChanged(EventArgs.Empty);
                    }
                }
                else
                {
                    _animating = false;
                    _animationTimer.Stop();
                }
            }

            if (_collapseButton != null)
            {
                _collapseButton.Text = _collapsed ? "+" : "-";
            }
        }
        
        protected virtual void OnCollapsedChanged(EventArgs e)
        {
            CollapsedChanged?.Invoke(this, new CollapsedChangedEventArgs(_collapsed));
        }
        #endregion

        #region Méthodes publiques
        /// <summary>
        /// Crée un ModernPanel avec en-tête
        /// </summary>
        public static ModernPanel CreateWithHeader(string headerText, Color headerColor)
        {
            return new ModernPanel
            {
                ShowHeader = true,
                HeaderText = headerText,
                HeaderColor = headerColor
            };
        }

        /// <summary>
        /// Crée un ModernPanel collapsible avec en-tête
        /// </summary>
        public static ModernPanel CreateCollapsible(string headerText, Color headerColor)
        {
            return new ModernPanel
            {
                ShowHeader = true,
                HeaderText = headerText,
                HeaderColor = headerColor,
                Collapsible = true
            };
        }
        #endregion
    }

    /// <summary>
    /// Arguments pour l'événement CollapsedChanged
    /// </summary>
    public class CollapsedChangedEventArgs : EventArgs
    {
        public bool Collapsed { get; }

        public CollapsedChangedEventArgs(bool collapsed)
        {
            Collapsed = collapsed;
        }
    }
}