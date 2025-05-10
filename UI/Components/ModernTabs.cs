using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Transconnect.UI.Helpers;


namespace Transconnect.UI.Components
{
    /// <summary>
    /// Contrôle de navigation par onglets avec design moderne
    /// </summary>
    public class ModernTabs : Control
    {
        // Propriétés et événements
        private List<ModernTab> _tabs = new List<ModernTab>();
        private ModernTab _selectedTab;
        private int _tabHeight = 40;
        private int _tabPadding = 20;
        private Color _selectedTabColor = UIColors.PrimaryColor;
        private Color _tabTextColor = UIColors.MediumGrayColor;
        private Color _selectedTabTextColor = UIColors.PrimaryColor;
        private Color _tabBackColor = UIColors.CardColor;
        private int _indicatorHeight = 3;
        private int _animationSpeed = 8;
        private Rectangle _indicatorRect;
        private int _indicatorDestination = 0;
        private System.Windows.Forms.Timer _animationTimer;
        
        /// <summary>
        /// Événement déclenché lorsqu'un onglet est sélectionné
        /// </summary>
        public event EventHandler<ModernTabSelectedEventArgs> SelectedTabChanged;

        /// <summary>
        /// Liste des onglets
        /// </summary>
        public List<ModernTab> Tabs => _tabs;
        
        /// <summary>
        /// Onglet actuellement sélectionné
        /// </summary>
        public ModernTab SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (_selectedTab != value && _tabs.Contains(value))
                {
                    ModernTab oldTab = _selectedTab;
                    _selectedTab = value;
                    
                    // Calculer la nouvelle position de l'indicateur
                    if (_selectedTab != null)
                    {
                        _indicatorDestination = _selectedTab.TabRect.X;
                        StartAnimation();
                    }
                    
                    SelectedTabChanged?.Invoke(this, new ModernTabSelectedEventArgs(oldTab, _selectedTab));
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Hauteur des onglets
        /// </summary>
        public int TabHeight
        {
            get => _tabHeight;
            set
            {
                _tabHeight = value;
                UpdateTabRects();
                Invalidate();
            }
        }
        
        /// <summary>
        /// Padding horizontal des onglets
        /// </summary>
        public int TabPadding
        {
            get => _tabPadding;
            set
            {
                _tabPadding = value;
                UpdateTabRects();
                Invalidate();
            }
        }
        
        /// <summary>
        /// Couleur de l'indicateur d'onglet sélectionné
        /// </summary>
        public Color SelectedTabColor
        {
            get => _selectedTabColor;
            set
            {
                _selectedTabColor = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Couleur du texte des onglets
        /// </summary>
        public Color TabTextColor
        {
            get => _tabTextColor;
            set
            {
                _tabTextColor = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Couleur du texte de l'onglet sélectionné
        /// </summary>
        public Color SelectedTabTextColor
        {
            get => _selectedTabTextColor;
            set
            {
                _selectedTabTextColor = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Hauteur de l'indicateur de sélection
        /// </summary>
        public int IndicatorHeight
        {
            get => _indicatorHeight;
            set
            {
                _indicatorHeight = value;
                Invalidate();
            }
        }
        
        public ModernTabs()
        {
            // Configuration initiale
            SetStyle(
                ControlStyles.UserPaint | 
                ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.OptimizedDoubleBuffer | 
                ControlStyles.ResizeRedraw, 
                true);
            
            // Propriétés par défaut
            BackColor = _tabBackColor;
            Size = new Size(400, _tabHeight);
            Font = UIFonts.Body;
            
            // Configurer le timer d'animation
            _animationTimer = new System.Windows.Forms.Timer { Interval = 10 };
            _animationTimer.Tick += AnimationTimer_Tick;
            
            // Initialiser le rectangle de l'indicateur
            _indicatorRect = new Rectangle(0, _tabHeight - _indicatorHeight, 0, _indicatorHeight);
        }
        
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            bool needsRedraw = false;
            
            // Animer la position horizontale de l'indicateur
            if (_indicatorRect.X != _indicatorDestination)
            {
                int diff = _indicatorDestination - _indicatorRect.X;
                int step = Math.Max(1, Math.Abs(diff) / _animationSpeed);
                
                if (Math.Abs(diff) <= step)
                    _indicatorRect.X = _indicatorDestination;
                else if (diff > 0)
                    _indicatorRect.X += step;
                else
                    _indicatorRect.X -= step;
                
                needsRedraw = true;
            }
            
            // Animer la largeur de l'indicateur
            if (_selectedTab != null)
            {
                int targetWidth = _selectedTab.TabRect.Width;
                if (_indicatorRect.Width != targetWidth)
                {
                    int diff = targetWidth - _indicatorRect.Width;
                    int step = Math.Max(1, Math.Abs(diff) / _animationSpeed);
                    
                    if (Math.Abs(diff) <= step)
                        _indicatorRect.Width = targetWidth;
                    else if (diff > 0)
                        _indicatorRect.Width += step;
                    else
                        _indicatorRect.Width -= step;
                    
                    needsRedraw = true;
                }
            }
            
            if (needsRedraw)
                Invalidate();
            else
                _animationTimer.Stop();
        }
        
        private void StartAnimation()
        {
            if (!_animationTimer.Enabled)
                _animationTimer.Start();
        }
        
        /// <summary>
        /// Ajoute un nouvel onglet
        /// </summary>
        public ModernTab AddTab(string text, object tag = null)
        {
            ModernTab tab = new ModernTab
            {
                Text = text,
                Tag = tag
            };
            
            _tabs.Add(tab);
            
            // Si c'est le premier onglet, le sélectionner
            if (_tabs.Count == 1)
                _selectedTab = tab;
            
            UpdateTabRects();
            Invalidate();
            
            return tab;
        }
        
        /// <summary>
        /// Met à jour les dimensions des onglets
        /// </summary>
        private void UpdateTabRects()
        {
            if (_tabs.Count == 0) return;
            
            int x = 0;
            int width;
            
            // Mode fixe (tous les onglets ont la même largeur)
            width = Width / _tabs.Count;
            
            for (int i = 0; i < _tabs.Count; i++)
            {
                _tabs[i].TabRect = new Rectangle(x, 0, width, _tabHeight);
                x += width;
            }
            
            // Mettre à jour le rectangle de l'indicateur
            if (_selectedTab != null)
            {
                _indicatorRect.X = _selectedTab.TabRect.X;
                _indicatorRect.Width = _selectedTab.TabRect.Width;
                _indicatorRect.Y = _tabHeight - _indicatorHeight;
            }
        }
        
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateTabRects();
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Dessiner le fond
            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                g.FillRectangle(brush, ClientRectangle);
            }
            
            // Dessiner la ligne du bas (séparateur)
            using (Pen pen = new Pen(Color.FromArgb(220, 220, 220)))
            {
                g.DrawLine(pen, 0, _tabHeight - 1, Width, _tabHeight - 1);
            }
            
            // Dessiner l'indicateur de l'onglet sélectionné
            if (_selectedTab != null)
            {
                using (SolidBrush brush = new SolidBrush(_selectedTabColor))
                {
                    g.FillRectangle(brush, _indicatorRect);
                }
            }
            
            // Dessiner chaque onglet
            foreach (ModernTab tab in _tabs)
            {
                bool isSelected = tab == _selectedTab;
                
                using (StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                {
                    Rectangle textRect = tab.TabRect;
                    
                    using (SolidBrush brush = new SolidBrush(isSelected ? _selectedTabTextColor : _tabTextColor))
                    {
                        Font font = isSelected ? 
                            new Font(Font.FontFamily, Font.Size, FontStyle.Bold) : Font;
                        
                        g.DrawString(tab.Text, font, brush, textRect, sf);
                    }
                }
            }
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            // Vérifier si un onglet a été cliqué
            for (int i = 0; i < _tabs.Count; i++)
            {
                if (_tabs[i].TabRect.Contains(e.Location))
                {
                    SelectedTab = _tabs[i];
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// Représente un onglet dans le contrôle ModernTabs
    /// </summary>
    public class ModernTab
    {
        public string Text { get; set; }
        public object Tag { get; set; }
        public Rectangle TabRect { get; set; }
        
        public ModernTab()
        {
            Text = "Onglet";
            TabRect = Rectangle.Empty;
        }
    }
    
    /// <summary>
    /// Arguments pour l'événement de changement d'onglet
    /// </summary>
    public class ModernTabSelectedEventArgs : EventArgs
    {
        public ModernTab OldTab { get; }
        public ModernTab NewTab { get; }
        
        public ModernTabSelectedEventArgs(ModernTab oldTab, ModernTab newTab)
        {
            OldTab = oldTab;
            NewTab = newTab;
        }
    }
}