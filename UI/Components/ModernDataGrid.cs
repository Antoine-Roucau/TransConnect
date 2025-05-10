using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Transconnect.UI.Helpers;

namespace Transconnect.UI.Components
{
    /// <summary>
    /// DataGridView personnalisé avec un style moderne
    /// </summary>
    public class ModernDataGrid : DataGridView
    {
        // Propriétés pour la personnalisation
        private Color _headerBackColor = UIColors.LightGrayColor;
        private Color _headerForeColor = UIColors.MediumGrayColor;
        private Color _alternateRowColor = Color.FromArgb(249, 250, 251);
        private Color _selectionBackColor;
        private Color _selectionForeColor = UIColors.DarkGrayColor;
        private Color _gridColor = Color.FromArgb(230, 230, 230);
        private Color _borderColor = Color.FromArgb(230, 230, 230);
        private int _rowHeight = 40;
        private int _headerHeight = 40;
        private Color _moduleColor = UIColors.PrimaryColor;
        
        /// <summary>
        /// Couleur principale du module (affecte la sélection)
        /// </summary>
        public Color ModuleColor
        {
            get => _moduleColor;
            set
            {
                _moduleColor = value;
                _selectionBackColor = UIColors.WithAlpha(value, 40);
                UpdateStyles();
            }
        }
        
        /// <summary>
        /// Couleur d'arrière-plan des en-têtes de colonnes
        /// </summary>
        public Color HeaderBackColor
        {
            get => _headerBackColor;
            set
            {
                _headerBackColor = value;
                UpdateStyles();
            }
        }
        
        /// <summary>
        /// Couleur du texte des en-têtes de colonnes
        /// </summary>
        public Color HeaderForeColor
        {
            get => _headerForeColor;
            set
            {
                _headerForeColor = value;
                UpdateStyles();
            }
        }
        
        /// <summary>
        /// Couleur des lignes alternées
        /// </summary>
        public Color AlternateRowColor
        {
            get => _alternateRowColor;
            set
            {
                _alternateRowColor = value;
                UpdateStyles();
            }
        }
        
        /// <summary>
        /// Hauteur des lignes
        /// </summary>
        public int RowHeight
        {
            get => _rowHeight;
            set
            {
                _rowHeight = value;
                this.RowTemplate.Height = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Hauteur des en-têtes de colonnes
        /// </summary>
        public int HeaderHeight
        {
            get => _headerHeight;
            set
            {
                _headerHeight = value;
                this.ColumnHeadersHeight = value;
                Invalidate();
            }
        }
        
        public ModernDataGrid()
        {
            // Configuration initiale
            SetStyle(
                ControlStyles.UserPaint | 
                ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.OptimizedDoubleBuffer,
                true);
            
            // Initialiser la couleur de sélection
            _selectionBackColor = UIColors.WithAlpha(_moduleColor, 40);
            
            // Configuration de base
            BackgroundColor = UIColors.CardColor;
            BorderStyle = BorderStyle.None;
            RowHeadersVisible = false;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            ReadOnly = true;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            MultiSelect = false;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            RowTemplate.Height = _rowHeight;
            
            // Configuration des en-têtes
            EnableHeadersVisualStyles = false;
            ColumnHeadersHeight = _headerHeight;
            
            // Appliquer les styles
            UpdateStyles();
            
            // Gérer l'événement CellPainting pour les bordures personnalisées
            CellPainting += ModernDataGrid_CellPainting;
        }
        
        private void UpdateStyles()
        {
            // Styles d'en-tête
            ColumnHeadersDefaultCellStyle.BackColor = _headerBackColor;
            ColumnHeadersDefaultCellStyle.ForeColor = _headerForeColor;
            ColumnHeadersDefaultCellStyle.Font = UIFonts.CreateFont(UIFonts.SecondaryTextSize, FontStyle.Bold);
            ColumnHeadersDefaultCellStyle.Padding = new Padding(10);
            
            // Styles des cellules
            DefaultCellStyle.Font = UIFonts.Body;
            DefaultCellStyle.BackColor = UIColors.CardColor;
            DefaultCellStyle.SelectionBackColor = _selectionBackColor;
            DefaultCellStyle.SelectionForeColor = _selectionForeColor;
            DefaultCellStyle.Padding = new Padding(5);
            
            // Styles des lignes alternées
            AlternatingRowsDefaultCellStyle.BackColor = _alternateRowColor;
            
            // Couleur de la grille
            GridColor = _gridColor;
            
            // Appliquer les changements
            Invalidate();
        }
        
        private void ModernDataGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // Personnalisation des bordures et de l'apparence des cellules
            if (e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                // Personnalisation des en-têtes de colonnes
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Dessiner l'arrière-plan
                using (SolidBrush brush = new SolidBrush(_headerBackColor))
                {
                    e.Graphics.FillRectangle(brush, e.CellBounds);
                }
                
                // Dessiner le texte de l'en-tête
                Rectangle textRect = new Rectangle(
                    e.CellBounds.X + 10,
                    e.CellBounds.Y,
                    e.CellBounds.Width - 20,
                    e.CellBounds.Height);
                
                TextRenderer.DrawText(
                    e.Graphics,
                    e.Value?.ToString() ?? string.Empty,
                    UIFonts.CreateFont(UIFonts.SecondaryTextSize, FontStyle.Bold),
                    textRect,
                    _headerForeColor,
                    TextFormatFlags.VerticalCenter);
                
                // Dessiner la bordure inférieure
                using (Pen pen = new Pen(_borderColor))
                {
                    e.Graphics.DrawLine(
                        pen,
                        e.CellBounds.Left,
                        e.CellBounds.Bottom - 1,
                        e.CellBounds.Right,
                        e.CellBounds.Bottom - 1);
                }
                
                e.Handled = true;
            }
            else if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Personnalisation des cellules normales
                // Laisser le comportement par défaut pour le moment
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            // Dessiner une bordure autour du contrôle
            using (Pen pen = new Pen(_borderColor))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            }
        }
    }
}