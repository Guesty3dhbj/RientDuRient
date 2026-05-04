using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace RientDuRient
{
    public partial class DownloadForm : Form
    {
        private MultiFileDownloader _downloader;
        private Dictionary<string, DownloadItem> _downloadItems;
        private DateTime _lastUpdateTime = DateTime.MinValue;
        private const int UPDATE_INTERVAL_MS = 500;
        private System.Windows.Forms.Timer _refreshTimer;

        // Variables para control de descargas simultáneas
        private Queue<DownloadRequest> _downloadQueue = new Queue<DownloadRequest>();
        private int _activeDownloads = 0;
        private int _maxSimultaneousDownloads = 4;

        // Propiedad pública para modificar el máximo de descargas simultáneas
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MaxSimultaneousDownloads
        {
            get { return _maxSimultaneousDownloads; }
            set
            {
                _maxSimultaneousDownloads = Math.Max(1, value);
                UpdateSimultaneousDownloadsLabel();
                ProcessDownloadQueue();
            }
        }

        // Clase para representar una solicitud de descarga en cola
        private class DownloadRequest
        {
            public string Url { get; set; }
            public string Destination { get; set; }
        }

        public DownloadForm()
        {
            InitializeComponent();
            _downloader = new MultiFileDownloader();
            _downloadItems = new Dictionary<string, DownloadItem>();
            SetupListBox();

            _refreshTimer = new System.Windows.Forms.Timer();
            _refreshTimer.Interval = 100;
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();

            // Configurar el evento del NumericUpDown
            numSimultaneousDownloads.ValueChanged += (s, e) =>
            {
                MaxSimultaneousDownloads = (int)numSimultaneousDownloads.Value;
            };

            // Inicializar el label de estado
            UpdateSimultaneousDownloadsLabel();
        }

        private void UpdateSimultaneousDownloadsLabel()
        {
            if (lblDownloadStatus.InvokeRequired)
            {
                lblDownloadStatus.Invoke(new Action(UpdateSimultaneousDownloadsLabel));
                return;
            }
            lblDownloadStatus.Text = $"Act: {_activeDownloads} | Sola: {_downloadQueue.Count}";
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if ((DateTime.Now - _lastUpdateTime).TotalMilliseconds >= UPDATE_INTERVAL_MS)
            {
                // Solo actualizar si hay cambios reales
                bool needsUpdate = false;
                foreach (var item in _downloadItems.Values)
                {
                    if (item.NeedsVisualRefresh)
                    {
                        needsUpdate = true;
                        item.NeedsVisualRefresh = false;
                        break; // Solo necesitamos saber si al menos uno cambió
                    }
                }

                if (needsUpdate)
                {
                    listBoxDownloads.Invalidate();
                }

                UpdateSimultaneousDownloadsLabel();
                _lastUpdateTime = DateTime.Now;
            }
        }
        // Nuevo método para invalidar solo los items que han cambiado
        private void InvalidateChangedItems()
        {
            if (listBoxDownloads.InvokeRequired)
            {
                listBoxDownloads.Invoke(new Action(InvalidateChangedItems));
                return;
            }

            // Solo invalidar si hay cambios reales
            bool needsRefresh = false;
            foreach (var item in _downloadItems)
            {
                if (item.Value.NeedsVisualRefresh)
                {
                    needsRefresh = true;
                    item.Value.NeedsVisualRefresh = false; // Reset flag
                }
            }

            if (needsRefresh)
            {
                listBoxDownloads.Invalidate();
            }
        }

        private void SetupListBox()
        {
            listBoxDownloads.DrawMode = DrawMode.OwnerDrawVariable;
            listBoxDownloads.DrawItem += ListBoxDownloads_DrawItem;
            listBoxDownloads.MeasureItem += ListBoxDownloads_MeasureItem;
            listBoxDownloads.DoubleClick += ListBoxDownloads_DoubleClick;
        }

        private void ListBoxDownloads_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxDownloads.SelectedItem != null)
            {
                var selectedUrl = listBoxDownloads.SelectedItem.ToString();
                CancelDownload(selectedUrl);
            }
        }

        private void ListBoxDownloads_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 60;
        }

        private void ListBoxDownloads_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();

            var item = listBoxDownloads.Items[e.Index].ToString();
            if (_downloadItems.ContainsKey(item))
            {
                var downloadItem = _downloadItems[item];

                var bounds = e.Bounds;
                var g = e.Graphics;

                // Color de fondo según estado
                Color bgColor;
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    g.FillRectangle(SystemBrushes.Highlight, bounds);
                }
                else
                {
                    // Usar switch expression solo con colores
                    bgColor = downloadItem.Status switch
                    {
                        "Descargando" => Color.FromArgb(220, 255, 220),
                        "En cola" => Color.FromArgb(255, 255, 200),
                        "Completado" => Color.FromArgb(220, 220, 255),
                        "Error" => Color.FromArgb(255, 220, 220),
                        "Cancelado" => Color.FromArgb(240, 240, 240),
                        _ => e.Index % 2 == 0 ? Color.White : Color.LightGray
                    };
                    g.FillRectangle(new SolidBrush(bgColor), bounds);
                }

                var urlText = downloadItem.Url.Length > 50 ? downloadItem.Url.Substring(0, 50) + "..." : downloadItem.Url;
                var urlFont = new Font("Arial", 8, FontStyle.Bold);
                g.DrawString(urlText, urlFont, Brushes.DarkBlue,
                    new Rectangle(bounds.X + 5, bounds.Y + 5, bounds.Width - 10, 20));

                var progressFont = new Font("Arial", 8);

                string progressText = downloadItem.ProgressText;
                if (downloadItem.ProgressPercentage.HasValue)
                {
                    progressText = $"[{downloadItem.ProgressPercentage:F1}%] " + progressText;
                }

                g.DrawString(progressText, progressFont, Brushes.Black,
                    new Rectangle(bounds.X + 5, bounds.Y + 25, bounds.Width - 10, 15));

                var statusColor = downloadItem.Status switch
                {
                    "Descargando" => Brushes.Green,
                    "Completado" => Brushes.Blue,
                    "Cancelado" => Brushes.Red,
                    "Error" => Brushes.DarkRed,
                    "En cola" => Brushes.Orange,
                    _ => Brushes.Gray
                };
                g.DrawString(downloadItem.Status, progressFont, statusColor,
                    new Rectangle(bounds.X + 5, bounds.Y + 40, bounds.Width - 10, 15));

                e.DrawFocusRectangle();
            }
        }

        private void RefreshListBox()
        {
            if (listBoxDownloads.InvokeRequired)
            {
                listBoxDownloads.Invoke(new Action(RefreshListBox));
                return;
            }

            // En lugar de invalidar todo, solo forzar una actualización suave
            listBoxDownloads.BeginUpdate();
            listBoxDownloads.EndUpdate();
        }

        // MÉTODO PÚBLICO para agregar descargas desde otros formularios
        // MÉTODO PÚBLICO para agregar descargas desde otros formularios
        public void AddDownload(string url, string destination)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(destination))
                return;

            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string, string>(AddDownload), url, destination);
                return;
            }

            // Si ya existe y está activa, no agregar duplicado
            if (_downloadItems.ContainsKey(url) &&
                (_downloadItems[url].IsDownloading || _downloadItems[url].Status == "En cola"))
                return;

            // Si existe pero está completada/cancelada, limpiar
            if (_downloadItems.ContainsKey(url))
            {
                _downloadItems.Remove(url);
                listBoxDownloads.Items.Remove(url);
            }

            var downloadItem = new DownloadItem
            {
                Url = url,
                Destination = destination,
                Status = "En cola",
                ProgressText = "Esperando en cola...",
                IsDownloading = false,
                ProgressPercentage = 0
            };

            _downloadItems[url] = downloadItem;
            listBoxDownloads.Items.Add(url);

            // NUEVO: Seleccionar el nuevo elemento agregado
            if (listBoxDownloads.Items.Count > 0)
            {
                listBoxDownloads.SelectedIndex = listBoxDownloads.Items.Count - 1;
                UpdateProgressBarForSelectedItem();
            }

            RefreshListBox();

            // Agregar a la cola y procesar
            _downloadQueue.Enqueue(new DownloadRequest { Url = url, Destination = destination });
            ProcessDownloadQueue();
        }

        // Método para agregar múltiples descargas
        // Método para agregar múltiples descargas
        public void AddDownloads(List<string> urls, string destination)
        {
            foreach (var url in urls)
            {
                AddDownload(url, destination);
            }

            // NUEVO: Seleccionar el primer elemento después de agregar múltiples
            if (listBoxDownloads.Items.Count > 0 && listBoxDownloads.SelectedIndex == -1)
            {
                listBoxDownloads.SelectedIndex = 0;
                UpdateProgressBarForSelectedItem();
            }
        }

        // Procesar la cola de descargas - NÚCLEO DEL SISTEMA
        private void ProcessDownloadQueue()
        {
            // Mientras haya espacio para más descargas y elementos en cola
            while (_activeDownloads < _maxSimultaneousDownloads && _downloadQueue.Count > 0)
            {
                var request = _downloadQueue.Dequeue();

                // Verificar que no se esté descargando ya
                if (_downloadItems.ContainsKey(request.Url) && !_downloadItems[request.Url].IsDownloading)
                {
                    _activeDownloads++;
                    _ = StartDownloadAsync(request.Url, request.Destination);
                }
            }

            UpdateSimultaneousDownloadsLabel();
        }

        private async void btnStartDownload_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUrl.Text))
            {
                MessageBox.Show("Por favor ingresa una URL válida", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDestination.Text))
            {
                MessageBox.Show("Por favor ingresa un destino válido", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AddDownload(txtUrl.Text.Trim(), txtDestination.Text.Trim());
            txtUrl.Clear();
        }

        private async Task StartDownloadAsync(string url, string destination)
        {
            if (!_downloadItems.ContainsKey(url))
                return;

            // CORREGIDO: Usar UpdateStatus correctamente
            _downloadItems[url].UpdateStatus("Descargando", "Iniciando...");
            _downloadItems[url].IsDownloading = true;
            _downloadItems[url].ProgressPercentage = 0;

            try
            {
                var progress = new Progress<DownloadProgress>(p =>
                {
                    if (_downloadItems.ContainsKey(url))
                    {
                        // CORREGIDO: Usar UpdateStatus para cambios
                        string progressText =
                            $" {FormatBytes(p.BytesRead)}/{FormatBytes(p.TotalBytes ?? 0)} " +
                            $"({p.ProgressPercentage:F1}%) " +
                            $"Velocidad: {p.MegabytesPerSecond:F2} MB/s";

                        _downloadItems[url].UpdateStatus("Descargando", progressText);
                        _downloadItems[url].ProgressPercentage = p.ProgressPercentage;
                        _downloadItems[url].NeedsVisualRefresh = true;

                        if (listBoxDownloads.SelectedItem != null &&
                            listBoxDownloads.SelectedItem.ToString() == url)
                        {
                            UpdateProgressBarForSelectedItem();
                        }
                    }
                });

                var result = await _downloader.DownloadFileAsync(url, destination, progress);

                if (_downloadItems.ContainsKey(url))
                {
                    _downloadItems[url].UpdateStatus("Completado", $"Completado: {Path.GetFileName(result)}");
                    _downloadItems[url].IsDownloading = false;
                    _downloadItems[url].ProgressPercentage = 100;
                }
            }
            catch (OperationCanceledException)
            {
                if (_downloadItems.ContainsKey(url))
                {
                    _downloadItems[url].UpdateStatus("Cancelado", "Descarga cancelada por el usuario");
                    _downloadItems[url].IsDownloading = false;
                }

                if (listBoxDownloads.Items.Count == 0)
                {
                    labelProgress.Text = "✗ Descarga cancelada";
                    progressBar1.Value = 0;
                }
            }
            catch (Exception ex)
            {
                if (_downloadItems.ContainsKey(url))
                {
                    _downloadItems[url].UpdateStatus("Error", $"Error: {ex.Message}");
                    _downloadItems[url].IsDownloading = false;
                }
            }
            finally
            {
                _activeDownloads--;
                ProcessDownloadQueue();

                if (listBoxDownloads.SelectedItem != null &&
                    listBoxDownloads.SelectedItem.ToString() == url)
                {
                    UpdateProgressBarForSelectedItem();
                }
            }
        }

        private void btnCancelSelected_Click(object sender, EventArgs e)
        {
            if (listBoxDownloads.SelectedItem != null)
            {
                var selectedUrl = listBoxDownloads.SelectedItem.ToString();
                CancelDownload(selectedUrl);
            }
            else
            {
                MessageBox.Show("Por favor selecciona una descarga de la lista para cancelar",
                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancelAll_Click(object sender, EventArgs e)
        {
            var activeDownloads = _downloadItems.Where(x => x.Value.IsDownloading).ToList();
            var queuedDownloads = _downloadItems.Where(x => x.Value.Status == "En cola").ToList();

            if (activeDownloads.Any() || queuedDownloads.Any())
            {
                var result = MessageBox.Show(
                    $"¿Estás seguro de que quieres cancelar {activeDownloads.Count} descarga(s) activa(s) y {queuedDownloads.Count} en cola?",
                    "Cancelar todas las descargas", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _downloader.CancelAllDownloads();

                    // Limpiar cola
                    _downloadQueue.Clear();
                    _activeDownloads = 0;

                    // **CORRECCIÓN: Limpiar solo las descargas activas y en cola**
                    var itemsToRemove = _downloadItems.Where(x => x.Value.IsDownloading || x.Value.Status == "En cola").ToList();

                    foreach (var item in itemsToRemove)
                    {
                        _downloadItems.Remove(item.Key);
                        if (listBoxDownloads.InvokeRequired)
                        {
                            listBoxDownloads.Invoke(new Action(() =>
                            {
                                listBoxDownloads.Items.Remove(item.Key);
                            }));
                        }
                        else
                        {
                            listBoxDownloads.Items.Remove(item.Key);
                        }
                    }

                    RefreshListBox();
                    UpdateSimultaneousDownloadsLabel();

                    // Actualizar barra de progreso
                    progressBar1.Value = 0;
                    labelProgress.Text = $"Canceladas {itemsToRemove.Count} descargas";

                    MessageBox.Show($"Se cancelaron {itemsToRemove.Count} descargas", "Cancelación completada",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("No hay descargas activas o en cola para cancelar", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        // Método auxiliar para remover items de forma consistente
        private void RemoveDownloadItem(string url)
        {
            // Remover del diccionario
            if (_downloadItems.ContainsKey(url))
            {
                _downloadItems.Remove(url);
            }

            // Remover de la lista de manera eficiente
            if (listBoxDownloads.InvokeRequired)
            {
                listBoxDownloads.Invoke(new Action(() =>
                {
                    RemoveSingleItemFromList(url);
                }));
            }
            else
            {
                RemoveSingleItemFromList(url);
            }
        }
        private void RemoveSingleItemFromList(string url)
        {
            int indexToRemove = -1;

            // Encontrar el índice del item a remover
            for (int i = 0; i < listBoxDownloads.Items.Count; i++)
            {
                if (listBoxDownloads.Items[i].ToString() == url)
                {
                    indexToRemove = i;
                    break;
                }
            }

            if (indexToRemove != -1)
            {
                // Guardar la selección actual para restaurarla después
                int selectedIndex = listBoxDownloads.SelectedIndex;
                bool wasSelected = (selectedIndex == indexToRemove);

                // Remover el item específico
                listBoxDownloads.Items.RemoveAt(indexToRemove);

                // Restaurar la selección si es necesario
                if (wasSelected && listBoxDownloads.Items.Count > 0)
                {
                    // Seleccionar el siguiente item, o el anterior si era el último
                    int newSelection = Math.Min(selectedIndex, listBoxDownloads.Items.Count - 1);
                    if (newSelection >= 0)
                    {
                        listBoxDownloads.SelectedIndex = newSelection;
                        UpdateProgressBarForSelectedItem();
                    }
                }
                else if (listBoxDownloads.Items.Count > 0 && listBoxDownloads.SelectedIndex == -1)
                {
                    // Si no hay selección pero hay elementos, seleccionar el primero
                    listBoxDownloads.SelectedIndex = 0;
                    UpdateProgressBarForSelectedItem();
                }
                else if (listBoxDownloads.Items.Count == 0)
                {
                    // Si no hay elementos, limpiar la barra de progreso
                    progressBar1.Value = 0;
                    labelProgress.Text = "No hay descargas activas";
                }
            }
        }

        // Método auxiliar para remover de la cola
        private void RemoveFromQueue(string url)
        {
            Console.WriteLine($"Removiendo de la cola: {url}");

            var newQueue = new Queue<DownloadRequest>();
            int removedCount = 0;

            while (_downloadQueue.Count > 0)
            {
                var item = _downloadQueue.Dequeue();
                if (item.Url != url)
                    newQueue.Enqueue(item);
                else
                    removedCount++;
            }
            _downloadQueue = newQueue;

            Console.WriteLine($"Removidos de la cola: {removedCount} items");

            RemoveDownloadItem(url);
            UpdateSimultaneousDownloadsLabel();
        }
        private void CancelDownload(string url)
        {
            Console.WriteLine($"=== INICIANDO CancelDownload para: {url} ===");
            Console.WriteLine($"¿_downloadItems contiene la URL? {_downloadItems.ContainsKey(url)}");

            if (_downloadItems.ContainsKey(url))
            {
                var downloadItem = _downloadItems[url];
                Console.WriteLine($"Estado: {downloadItem.Status}, IsDownloading: {downloadItem.IsDownloading}");

                if (downloadItem.IsDownloading || downloadItem.Status == "Descargando")
                {
                    Console.WriteLine("La descarga está activa - procediendo con cancelación...");
                    var result = MessageBox.Show($"¿Estás seguro de que quieres cancelar esta descarga?\n{url}",
                        "Cancelar descarga", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        Console.WriteLine("Usuario confirmó cancelación");
                        bool cancelled = _downloader.CancelDownloadByUrl(url);
                        Console.WriteLine($"CancelDownloadByUrl retornó: {cancelled}");

                        if (cancelled)
                        {
                            Console.WriteLine("Cancelación exitosa en el downloader");
                            _activeDownloads--;
                            downloadItem.Status = "Cancelado";
                            downloadItem.IsDownloading = false;
                            downloadItem.ProgressText = "Cancelada por el usuario";

                            // Remover inmediatamente
                            RemoveDownloadItem(url);

                            ProcessDownloadQueue();
                            UpdateSimultaneousDownloadsLabel();

                            MessageBox.Show("Descarga cancelada exitosamente", "Cancelada",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            Console.WriteLine("FALLO: CancelDownloadByUrl retornó false");
                            MessageBox.Show("No se pudo cancelar la descarga. Puede que ya haya terminado.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else if (downloadItem.Status == "En cola")
                {
                    Console.WriteLine("La descarga está en cola - removiendo...");
                    RemoveFromQueue(url);
                    MessageBox.Show("Descarga removida de la cola", "Removida",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Console.WriteLine($"La descarga tiene estado: {downloadItem.Status}");
                    var result = MessageBox.Show($"¿Quieres remover esta descarga de la lista?\nEstado: {downloadItem.Status}",
                        "Remover descarga", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        RemoveDownloadItem(url);
                    }
                }
            }
            else
            {
                Console.WriteLine("ERROR: La URL no existe en _downloadItems");
                MessageBox.Show("La descarga seleccionada no existe o ya fue removida", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            Console.WriteLine($"=== FINALIZANDO CancelDownload para: {url} ===\n");
        }

        private void btnRemoveCompleted_Click(object sender, EventArgs e)
        {
            var completedDownloads = _downloadItems.Where(x =>
                !x.Value.IsDownloading && x.Value.Status != "En cola").ToList();

            foreach (var item in completedDownloads)
            {
                listBoxDownloads.Items.Remove(item.Key);
                _downloadItems.Remove(item.Key);
            }

            RefreshListBox();
            labelProgress.Text = $"Eliminadas {completedDownloads.Count} descargas completadas";
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Seleccionar carpeta de destino";
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtDestination.Text = folderDialog.SelectedPath;
                }
            }
        }

        private string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;

            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }

            return $"{number:n1} {suffixes[counter]}";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var activeDownloads = _downloadItems.Any(x => x.Value.IsDownloading) || _downloadQueue.Count > 0;
            if (activeDownloads)
            {
                var result = MessageBox.Show($"Hay descargas activas o en cola. ¿Estás seguro de que quieres salir?",
                    "Descargas en curso", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }

                _downloader.CancelAllDownloads();
            }

            _downloader?.Dispose();
            base.OnFormClosing(e);
        }

        private void listBoxDownloads_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateProgressBarForSelectedItem();
        }

        private void UpdateProgressBarForSelectedItem()
        {
            if (listBoxDownloads.SelectedItem != null)
            {
                var selectedUrl = listBoxDownloads.SelectedItem.ToString();
                if (_downloadItems.ContainsKey(selectedUrl))
                {
                    var downloadItem = _downloadItems[selectedUrl];

                    if (downloadItem.IsDownloading && downloadItem.ProgressPercentage.HasValue)
                    {
                        progressBar1.Value = Math.Min(100, Math.Max(0, (int)downloadItem.ProgressPercentage.Value));
                        progressBar1.Style = ProgressBarStyle.Continuous;
                    }
                    else if (downloadItem.Status == "Completado")
                    {
                        progressBar1.Value = 100;
                        progressBar1.Style = ProgressBarStyle.Continuous;
                    }
                    else
                    {
                        progressBar1.Value = 0;
                        progressBar1.Style = ProgressBarStyle.Continuous;
                    }

                    labelProgress.Text = $"{downloadItem.Status}: {downloadItem.ProgressText}";
                }
            }
            else
            {
                progressBar1.Value = 0;
                progressBar1.Style = ProgressBarStyle.Continuous;
                labelProgress.Text = "Selecciona una descarga para ver detalles";
            }
        }

        private void DownloadForm_Load(object sender, EventArgs e)
        {
            this.Text = $"Gestor de Descargas (Máx: {_maxSimultaneousDownloads} simultáneas)";

            // Seleccionar automáticamente el primer elemento si hay descargas
            SelectFirstItem();
        }

        // Nuevo método para seleccionar el primer elemento
        private void SelectFirstItem()
        {
            if (listBoxDownloads.InvokeRequired)
            {
                listBoxDownloads.Invoke(new Action(SelectFirstItem));
                return;
            }

            if (listBoxDownloads.Items.Count > 0 && listBoxDownloads.SelectedIndex == -1)
            {
                listBoxDownloads.SelectedIndex = 0;
                UpdateProgressBarForSelectedItem();
            }
        }
    }
}