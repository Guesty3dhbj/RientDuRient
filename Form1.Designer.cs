namespace RientDuRient
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    if (disposing)
        //    {
        //        browser?.Dispose();
        //        _downloadCts?.Dispose();
        //        _downloadForm?.Close();
        //    }
        //    base.Dispose(disposing);
        //}

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador.
        /// No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            btnBack = new Button();
            lblPath = new Label();
            listView = new ListView();
            colNombre = new ColumnHeader();
            colTipo = new ColumnHeader();
            colTam = new ColumnHeader();
            colFech = new ColumnHeader();
            panel1 = new Panel();
            panelTop = new Panel();
            btnConfigurarDescargas = new Button();
            btnDescargarTodo = new Button();
            btnDescargarSeleccionados = new Button();
            btnAbrirDescargas = new Button();
            panel1.SuspendLayout();
            panelTop.SuspendLayout();
            SuspendLayout();
            // 
            // btnBack
            // 
            btnBack.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnBack.Location = new Point(3, 3);
            btnBack.Margin = new Padding(3, 4, 3, 4);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(139, 39);
            btnBack.TabIndex = 0;
            btnBack.Text = "⬆ Subir un nivel";
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += btnBack_Click;
            // 
            // lblPath
            // 
            lblPath.Dock = DockStyle.Bottom;
            lblPath.Font = new Font("Consolas", 9F);
            lblPath.Location = new Point(0, 653);
            lblPath.Name = "lblPath";
            lblPath.Padding = new Padding(9, 0, 0, 0);
            lblPath.Size = new Size(854, 32);
            lblPath.TabIndex = 1;
            lblPath.Text = "Ruta actual:";
            lblPath.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // listView
            // 
            listView.Columns.AddRange(new ColumnHeader[] { colNombre, colTipo, colTam, colFech });
            listView.Dock = DockStyle.Fill;
            listView.FullRowSelect = true;
            listView.GridLines = true;
            listView.Location = new Point(0, 0);
            listView.Margin = new Padding(3, 4, 3, 4);
            listView.Name = "listView";
            listView.Size = new Size(854, 608);
            listView.TabIndex = 2;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = View.Details;
            listView.DrawColumnHeader += listView_DrawColumnHeader;
            listView.DrawItem += listView_DrawItem;
            listView.DrawSubItem += listView_DrawSubItem;
            listView.DoubleClick += listView_DoubleClick;
            // 
            // colNombre
            // 
            colNombre.Text = "Nombre";
            colNombre.Width = 400;
            // 
            // colTipo
            // 
            colTipo.Text = "Tipo";
            colTipo.Width = 150;
            // 
            // colTam
            // 
            colTam.Text = "Tamaño";
            colTam.Width = 150;
            // 
            // colFech
            // 
            colFech.Text = "Fecha";
            colFech.Width = 150;
            // 
            // panel1
            // 
            panel1.Controls.Add(listView);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 45);
            panel1.Name = "panel1";
            panel1.Size = new Size(854, 608);
            panel1.TabIndex = 4;
            // 
            // panelTop
            // 
            panelTop.Controls.Add(btnConfigurarDescargas);
            panelTop.Controls.Add(btnDescargarTodo);
            panelTop.Controls.Add(btnDescargarSeleccionados);
            panelTop.Controls.Add(btnAbrirDescargas);
            panelTop.Controls.Add(btnBack);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(0, 0);
            panelTop.Name = "panelTop";
            panelTop.Size = new Size(854, 45);
            panelTop.TabIndex = 5;
            // 
            // btnConfigurarDescargas
            // 
            btnConfigurarDescargas.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnConfigurarDescargas.Location = new Point(573, 3);
            btnConfigurarDescargas.Name = "btnConfigurarDescargas";
            btnConfigurarDescargas.Size = new Size(139, 39);
            btnConfigurarDescargas.TabIndex = 4;
            btnConfigurarDescargas.Text = "Configurar Descargas";
            btnConfigurarDescargas.UseVisualStyleBackColor = true;
            btnConfigurarDescargas.Visible = false;
            btnConfigurarDescargas.Click += btnConfigurarDescargas_Click;
            // 
            // btnDescargarTodo
            // 
            btnDescargarTodo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnDescargarTodo.Location = new Point(428, 3);
            btnDescargarTodo.Name = "btnDescargarTodo";
            btnDescargarTodo.Size = new Size(139, 39);
            btnDescargarTodo.TabIndex = 3;
            btnDescargarTodo.Text = "Descargar Todo";
            btnDescargarTodo.UseVisualStyleBackColor = true;
            btnDescargarTodo.Click += btnDescargarTodo_Click;
            // 
            // btnDescargarSeleccionados
            // 
            btnDescargarSeleccionados.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnDescargarSeleccionados.Location = new Point(283, 3);
            btnDescargarSeleccionados.Name = "btnDescargarSeleccionados";
            btnDescargarSeleccionados.Size = new Size(139, 39);
            btnDescargarSeleccionados.TabIndex = 2;
            btnDescargarSeleccionados.Text = "Descargar Seleccionados";
            btnDescargarSeleccionados.UseVisualStyleBackColor = true;
            btnDescargarSeleccionados.Click += btnDescargarSeleccionados_Click;
            // 
            // btnAbrirDescargas
            // 
            btnAbrirDescargas.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAbrirDescargas.Location = new Point(148, 3);
            btnAbrirDescargas.Name = "btnAbrirDescargas";
            btnAbrirDescargas.Size = new Size(129, 39);
            btnAbrirDescargas.TabIndex = 1;
            btnAbrirDescargas.Text = "Abrir Descargas";
            btnAbrirDescargas.UseVisualStyleBackColor = true;
            btnAbrirDescargas.Click += btnAbrirDescargas_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(854, 685);
            Controls.Add(panel1);
            Controls.Add(panelTop);
            Controls.Add(lblPath);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Explorador Web de Archivos";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panelTop.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader colNombre;
        private System.Windows.Forms.ColumnHeader colTipo;
        private System.Windows.Forms.ColumnHeader colTam;
        private System.Windows.Forms.ColumnHeader colFech;
        private Panel panel1;
        private Panel panelTop;
        private Button btnAbrirDescargas;
        private Button btnDescargarSeleccionados;
        private Button btnDescargarTodo;
        private Button btnConfigurarDescargas;
    }
}