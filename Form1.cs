using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RientDuRient
{
    public partial class Form1 : Form
    {
        private WebDirectoryBrowser browser;
        private CancellationTokenSource _downloadCts;
        private static DownloadForm _downloadForm; // Instancia estática para reutilizar

        public Form1()
        {
            InitializeComponent();
        }

        private void ActualizarLista()
        {
            listView.Items.Clear();

            foreach (var entry in browser.Entries)
            {
                // CORREGIDO: Solo 4 subitems para coincidir con las 4 columnas
                var item = new ListViewItem(entry.Name);  // Columna 0: Nombre
                item.SubItems.Add(entry.IsDirectory ? "Carpeta" : "Archivo");  // Columna 1: Tipo
                item.SubItems.Add(entry.Size ?? "-");  // Columna 2: Tamaño
                item.SubItems.Add(entry.Date?.ToString("yyyy-MM-dd HH:mm") ?? "-");  // Columna 3: Fecha

                item.Tag = entry;

                listView.Items.Add(item);
            }

            lblPath.Text = $"Ruta actual: {browser.CurrentUri}";
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // Mostrar mensaje de carga
            lblPath.Text = "Cargando...";

            string baseUrl = "https://myrient.erista.me/files/";
            browser = new WebDirectoryBrowser(baseUrl);

            try
            {
                await browser.LoadAsync();
                ActualizarLista();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblPath.Text = "Error al cargar";
            }
        }

        private async void listView_DoubleClick(object sender, EventArgs e)
        {
            if (browser == null || listView.SelectedItems.Count == 0)
                return;

            var item = listView.SelectedItems[0];
            var entry = item.Tag as WebEntry;
            if (entry == null) return;

            if (entry.IsDirectory)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    bool ok = await browser.EnterAsync(entry.Name);
                    if (ok)
                    {
                        ActualizarLista();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo entrar en la carpeta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al entrar en la carpeta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
            else
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    dialog.Description = "Selecciona la carpeta donde guardar el archivo";
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        GetOrCreateDownloadForm().AddDownload(entry.Url, dialog.SelectedPath);
                    }
                }
            }
        }

        // Método mejorado para obtener o crear el DownloadForm
        // Método mejorado para obtener o crear el DownloadForm
        private DownloadForm GetOrCreateDownloadForm()
        {
            // CORRECCIÓN: Verificar explícitamente si está disposed
            if (_downloadForm == null || _downloadForm.IsDisposed)
            {
                _downloadForm = new DownloadForm();
                _downloadForm.MaxSimultaneousDownloads = 4; // Valor por defecto
                _downloadForm.Show();

                // Configurar el formulario para que se cierre correctamente
                _downloadForm.FormClosing += (s, e) =>
                {
                    if (e.CloseReason == CloseReason.UserClosing)
                    {
                        // Opción 1: Cerrar completamente
                        // No hacer nada - permitir que se cierre

                        // Opción 2: Ocultar en lugar de cerrar (comenta la opción 1 y usa esta)
                        // e.Cancel = true;
                        // _downloadForm.Hide();
                    }
                };

                // Opcional: Limpiar la referencia cuando se cierre completamente
                _downloadForm.FormClosed += (s, e) =>
                {
                    // No poner _downloadForm = null aquí porque podría causar problemas
                    // La verificación con IsDisposed es suficiente
                };
            }
            else
            {
                // Asegurarse de que el formulario sea visible
                _downloadForm.BringToFront();
                _downloadForm.WindowState = FormWindowState.Normal;
                _downloadForm.Show();

                // Si está minimizado, restaurarlo
                if (_downloadForm.WindowState == FormWindowState.Minimized)
                {
                    _downloadForm.WindowState = FormWindowState.Normal;
                }
            }

            return _downloadForm;
        }

        // Método para cambiar el número de descargas simultáneas
        private void CambiarDescargasSimultaneas(int numero)
        {
            var downloadForm = GetOrCreateDownloadForm();
            if (downloadForm != null && !downloadForm.IsDisposed)
            {
                downloadForm.MaxSimultaneousDownloads = numero;
            }
        }


        // Método para descargar múltiples archivos seleccionados
        private void DescargarSeleccionados()
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selecciona al menos un archivo para descargar",
                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Selecciona la carpeta donde guardar los archivos";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var downloadForm = GetOrCreateDownloadForm();

                    int count = 0;
                    foreach (ListViewItem item in listView.SelectedItems)
                    {
                        var entry = item.Tag as WebEntry;
                        if (entry != null && !entry.IsDirectory)
                        {
                            downloadForm.AddDownload(entry.Url, dialog.SelectedPath);
                            count++;
                        }
                    }

                    if (count > 0)
                    {
                        MessageBox.Show($"Se agregaron {count} archivos a la cola de descargas",
                            "Descargas en cola", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }


        // Botón para abrir el gestor de descargas
        private void btnAbrirDescargas_Click(object sender, EventArgs e)
        {
            var downloadForm = GetOrCreateDownloadForm();
            downloadForm?.BringToFront();
        }

        // Botón para descargar los elementos seleccionados
        private void btnDescargarSeleccionados_Click(object sender, EventArgs e)
        {
            DescargarSeleccionados();
        }

        // Método para descargar todo en la carpeta actual (excepto subcarpetas)
        private void btnDescargarTodo_Click(object sender, EventArgs e)
        {
            var archivos = new List<WebEntry>();
            foreach (var entry in browser.Entries)
            {
                if (!entry.IsDirectory)
                {
                    archivos.Add(entry);
                }
            }

            if (archivos.Count == 0)
            {
                MessageBox.Show("No hay archivos para descargar en esta carpeta",
                    "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Selecciona la carpeta donde guardar los archivos";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var downloadForm = GetOrCreateDownloadForm();

                    foreach (var archivo in archivos)
                    {
                        downloadForm.AddDownload(archivo.Url, dialog.SelectedPath);
                    }

                    MessageBox.Show($"Se agregaron {archivos.Count} archivos a la cola de descargas",
                        "Descargas en cola", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // Configuración de descargas simultáneas
        private void btnConfigurarDescargas_Click(object sender, EventArgs e)
        {
            using (var formConfig = new Form())
            {
                formConfig.Text = "Configurar Descargas";
                formConfig.Size = new Size(300, 150);
                formConfig.StartPosition = FormStartPosition.CenterParent;

                var lbl = new Label() { Text = "Número máximo de descargas simultáneas:", Location = new Point(10, 20), AutoSize = true };
                var num = new NumericUpDown() { Value = _downloadForm?.MaxSimultaneousDownloads ?? 4, Minimum = 1, Maximum = 10, Location = new Point(10, 50), Width = 60 };
                var btnAceptar = new Button() { Text = "Aceptar", Location = new Point(100, 80), DialogResult = DialogResult.OK };

                formConfig.Controls.AddRange(new Control[] { lbl, num, btnAceptar });
                formConfig.AcceptButton = btnAceptar;

                if (formConfig.ShowDialog() == DialogResult.OK)
                {
                    CambiarDescargasSimultaneas((int)num.Value);
                    MessageBox.Show($"Número máximo de descargas simultáneas cambiado a: {num.Value}",
                        "Configuración Actualizada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private async void btnBack_Click(object sender, EventArgs e)
        {
            if (browser == null) return;

            try
            {
                Cursor = Cursors.WaitCursor;
                bool ok = await browser.UpAsync();
                if (ok)
                {
                    ActualizarLista();
                }
                else
                {
                    MessageBox.Show("Ya estás en la raíz.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al subir: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void listView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                browser?.Dispose();
                _downloadCts?.Dispose();
                _downloadForm?.Close();
            }
            base.Dispose(disposing);
        }
    }
}