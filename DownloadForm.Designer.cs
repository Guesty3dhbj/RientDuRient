namespace RientDuRient
{
    partial class DownloadForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            txtUrl = new TextBox();
            txtDestination = new TextBox();
            btnBrowse = new Button();
            btnStartDownload = new Button();
            btnCancelSelected = new Button();
            btnCancelAll = new Button();
            btnRemoveCompleted = new Button();
            labelProgress = new Label();
            progressBar1 = new ProgressBar();
            lblActiveDownloads = new Label();
            listBoxDownloads = new ListBox();
            panelSimultaneousDownloads = new Panel();
            lblSimultaneous = new Label();
            numSimultaneousDownloads = new NumericUpDown();
            lblDownloadStatus = new Label();
            panelSimultaneousDownloads.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numSimultaneousDownloads).BeginInit();
            SuspendLayout();
            // 
            // txtUrl
            // 
            txtUrl.Location = new Point(4, 4);
            txtUrl.Margin = new Padding(5, 6, 5, 6);
            txtUrl.Name = "txtUrl";
            txtUrl.Size = new Size(596, 27);
            txtUrl.TabIndex = 0;
            txtUrl.Text = "https://softlibre.unizar.es/ubuntu/releases/24.04.3/ubuntu-24.04.3-desktop-amd64.iso";
            // 
            // txtDestination
            // 
            txtDestination.Location = new Point(3, 40);
            txtDestination.Margin = new Padding(5, 6, 5, 6);
            txtDestination.Name = "txtDestination";
            txtDestination.Size = new Size(486, 27);
            txtDestination.TabIndex = 1;
            txtDestination.Text = "C:\\tmp\\";
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(499, 36);
            btnBrowse.Margin = new Padding(5, 6, 5, 6);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(101, 34);
            btnBrowse.TabIndex = 2;
            btnBrowse.Text = "Examinar";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnStartDownload
            // 
            btnStartDownload.Location = new Point(3, 72);
            btnStartDownload.Margin = new Padding(5, 6, 5, 6);
            btnStartDownload.Name = "btnStartDownload";
            btnStartDownload.Size = new Size(597, 46);
            btnStartDownload.TabIndex = 3;
            btnStartDownload.Text = "Descargar";
            btnStartDownload.UseVisualStyleBackColor = true;
            btnStartDownload.Click += btnStartDownload_Click;
            // 
            // btnCancelSelected
            // 
            btnCancelSelected.Location = new Point(3, 120);
            btnCancelSelected.Margin = new Padding(5, 6, 5, 6);
            btnCancelSelected.Name = "btnCancelSelected";
            btnCancelSelected.Size = new Size(219, 38);
            btnCancelSelected.TabIndex = 4;
            btnCancelSelected.Text = "Cancelar Selección";
            btnCancelSelected.UseVisualStyleBackColor = true;
            btnCancelSelected.Click += btnCancelSelected_Click;
            // 
            // btnCancelAll
            // 
            btnCancelAll.Location = new Point(232, 120);
            btnCancelAll.Margin = new Padding(5, 6, 5, 6);
            btnCancelAll.Name = "btnCancelAll";
            btnCancelAll.Size = new Size(166, 38);
            btnCancelAll.TabIndex = 5;
            btnCancelAll.Text = "Cancelar Todas";
            btnCancelAll.UseVisualStyleBackColor = true;
            btnCancelAll.Click += btnCancelAll_Click;
            // 
            // btnRemoveCompleted
            // 
            btnRemoveCompleted.Location = new Point(408, 120);
            btnRemoveCompleted.Margin = new Padding(5, 6, 5, 6);
            btnRemoveCompleted.Name = "btnRemoveCompleted";
            btnRemoveCompleted.Size = new Size(192, 38);
            btnRemoveCompleted.TabIndex = 6;
            btnRemoveCompleted.Text = "Limpiar Completadas";
            btnRemoveCompleted.UseVisualStyleBackColor = true;
            btnRemoveCompleted.Click += btnRemoveCompleted_Click;
            // 
            // labelProgress
            // 
            labelProgress.AutoSize = true;
            labelProgress.Location = new Point(3, 166);
            labelProgress.Margin = new Padding(5, 0, 5, 0);
            labelProgress.Name = "labelProgress";
            labelProgress.Size = new Size(152, 20);
            labelProgress.TabIndex = 7;
            labelProgress.Text = "Listo para descargar...";
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(332, 4);
            progressBar1.Margin = new Padding(5, 6, 5, 6);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(268, 27);
            progressBar1.TabIndex = 8;
            // 
            // lblActiveDownloads
            // 
            lblActiveDownloads.AutoSize = true;
            lblActiveDownloads.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblActiveDownloads.Location = new Point(3, 260);
            lblActiveDownloads.Margin = new Padding(5, 0, 5, 0);
            lblActiveDownloads.Name = "lblActiveDownloads";
            lblActiveDownloads.Size = new Size(116, 13);
            lblActiveDownloads.TabIndex = 9;
            lblActiveDownloads.Text = "Descargas activas:";
            // 
            // listBoxDownloads
            // 
            listBoxDownloads.FormattingEnabled = true;
            listBoxDownloads.HorizontalScrollbar = true;
            listBoxDownloads.Location = new Point(3, 192);
            listBoxDownloads.Margin = new Padding(5, 6, 5, 6);
            listBoxDownloads.Name = "listBoxDownloads";
            listBoxDownloads.Size = new Size(597, 284);
            listBoxDownloads.TabIndex = 10;
            listBoxDownloads.SelectedIndexChanged += listBoxDownloads_SelectedIndexChanged;
            // 
            // panelSimultaneousDownloads
            // 
            panelSimultaneousDownloads.Controls.Add(lblSimultaneous);
            panelSimultaneousDownloads.Controls.Add(numSimultaneousDownloads);
            panelSimultaneousDownloads.Controls.Add(lblDownloadStatus);
            panelSimultaneousDownloads.Controls.Add(progressBar1);
            panelSimultaneousDownloads.Dock = DockStyle.Bottom;
            panelSimultaneousDownloads.Location = new Point(0, 478);
            panelSimultaneousDownloads.Name = "panelSimultaneousDownloads";
            panelSimultaneousDownloads.Size = new Size(604, 35);
            panelSimultaneousDownloads.TabIndex = 11;
            // 
            // lblSimultaneous
            // 
            lblSimultaneous.AutoSize = true;
            lblSimultaneous.Location = new Point(8, 9);
            lblSimultaneous.Name = "lblSimultaneous";
            lblSimultaneous.Size = new Size(162, 20);
            lblSimultaneous.TabIndex = 0;
            lblSimultaneous.Text = "Descargas simultáneas:";
            // 
            // numSimultaneousDownloads
            // 
            numSimultaneousDownloads.Location = new Point(176, 5);
            numSimultaneousDownloads.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numSimultaneousDownloads.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numSimultaneousDownloads.Name = "numSimultaneousDownloads";
            numSimultaneousDownloads.Size = new Size(50, 27);
            numSimultaneousDownloads.TabIndex = 1;
            numSimultaneousDownloads.Value = new decimal(new int[] { 4, 0, 0, 0 });
            // 
            // lblDownloadStatus
            // 
            lblDownloadStatus.AutoSize = true;
            lblDownloadStatus.Location = new Point(229, 8);
            lblDownloadStatus.Name = "lblDownloadStatus";
            lblDownloadStatus.Size = new Size(103, 20);
            lblDownloadStatus.TabIndex = 2;
            lblDownloadStatus.Text = "Act: 0 | Cola: 0";
            // 
            // DownloadForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(604, 513);
            Controls.Add(panelSimultaneousDownloads);
            Controls.Add(listBoxDownloads);
            Controls.Add(lblActiveDownloads);
            Controls.Add(labelProgress);
            Controls.Add(btnRemoveCompleted);
            Controls.Add(btnCancelAll);
            Controls.Add(btnCancelSelected);
            Controls.Add(btnStartDownload);
            Controls.Add(btnBrowse);
            Controls.Add(txtDestination);
            Controls.Add(txtUrl);
            Margin = new Padding(5, 6, 5, 6);
            Name = "DownloadForm";
            Text = "Download Manager";
            Load += DownloadForm_Load;
            panelSimultaneousDownloads.ResumeLayout(false);
            panelSimultaneousDownloads.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numSimultaneousDownloads).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.TextBox txtDestination;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnStartDownload;
        private System.Windows.Forms.Button btnCancelSelected;
        private System.Windows.Forms.Button btnCancelAll;
        private System.Windows.Forms.Button btnRemoveCompleted;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblActiveDownloads;
        private System.Windows.Forms.ListBox listBoxDownloads;
        private Panel panelSimultaneousDownloads;
        private Label lblSimultaneous;
        private NumericUpDown numSimultaneousDownloads;
        private Label lblDownloadStatus;
    }
}