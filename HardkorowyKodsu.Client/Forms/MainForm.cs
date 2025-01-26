using System.Configuration;
using System.Text.Json;
using HardkorowyKodsu.Core.DTO;

namespace HardkorowyKodsu.Client
{
    public partial class MainForm : Form
    {
        private readonly HttpClient _httpClient;

        public MainForm()
        {
            InitializeComponent();

            var baseAddress = ConfigurationManager.AppSettings["ApiBaseAddress"];
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                MessageBox.Show("Brak wartości 'ApiBaseAddress' w pliku App.config. " +
                                "Aplikacja nie może się uruchomić.",
                                "Błąd konfiguracji",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                Environment.Exit(1);
            }

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseAddress) 
            };
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await LoadObjectsAsync();

            comboBoxObjects.Enabled = true;

            if (comboBoxObjects.Items.Count > 0)
                comboBoxObjects.SelectedIndex = 0;
        }

        private async Task LoadObjectsAsync()
        {
            var objects = await PerformHttpRequestAsync<List<string>>(
                () => _httpClient.GetAsync("api/databaseschema/objects"),
                maxRetries: 3, // Ile razy ma próbować
                delayMilliseconds: 2000 // Przerwa między próbami
            );

            comboBoxObjects.Items.Clear();
            if (objects != null)
            {
                comboBoxObjects.Items.AddRange(objects.ToArray());
            }
        }

        private async Task<T?> PerformHttpRequestAsync<T>(Func<Task<HttpResponseMessage>> httpAction, int maxRetries = 3, int delayMilliseconds = 2000)
        {
            int attempt = 0;

            while (true)
            {
                try
                {
                    attempt++;
                    var response = await httpAction.Invoke();

                    response.EnsureSuccessStatusCode(); // Rzuca wyjątek, jeśli kod nie jest 2xx

                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<T>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return result;
                }
                catch (HttpRequestException ex)
                {
                    if (attempt >= maxRetries)
                    {
                        MessageBox.Show(
                            $"Nie można połączyć się z serwerem po {attempt} próbach.\n" +
                            $"Szczegóły: {ex.Message}",
                            "Błąd połączenia",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    else
                    {
                        await Task.Delay(delayMilliseconds);
                    }
                }
            }
        }

        private async void comboBoxObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxObjects.SelectedItem is string objName)
            {
                await LoadColumnsAsync(objName);
            }
        }

        private void InitializeComponent()
        {
            comboBoxObjects = new ComboBox();
            dataGridViewColumns = new DataGridView();
            tableLayoutPanel1 = new TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)dataGridViewColumns).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // comboBoxObjects
            // 
            comboBoxObjects.Dock = DockStyle.Fill;
            comboBoxObjects.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxObjects.Enabled = false;
            comboBoxObjects.FormattingEnabled = true;
            comboBoxObjects.Location = new Point(3, 3);
            comboBoxObjects.Name = "comboBoxObjects";
            comboBoxObjects.Size = new Size(922, 28);
            comboBoxObjects.TabIndex = 1;
            comboBoxObjects.SelectedIndexChanged += comboBoxObjects_SelectedIndexChanged;
            // 
            // dataGridViewColumns
            // 
            dataGridViewColumns.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewColumns.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewColumns.Dock = DockStyle.Fill;
            dataGridViewColumns.Location = new Point(3, 37);
            dataGridViewColumns.Name = "dataGridViewColumns";
            dataGridViewColumns.Size = new Size(922, 422);
            dataGridViewColumns.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(comboBoxObjects, 0, 0);
            tableLayoutPanel1.Controls.Add(dataGridViewColumns, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(928, 462);
            tableLayoutPanel1.TabIndex = 3;
            // 
            // MainForm
            // 
            ClientSize = new Size(928, 462);
            Controls.Add(tableLayoutPanel1);
            Name = "MainForm";
            Text = "Hardkorowy Kodsu";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewColumns).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        private async Task LoadColumnsAsync(string objectName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/databaseschema/columns/{objectName}");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Błąd: {response.StatusCode}\n{error}");
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var columns = JsonSerializer.Deserialize<List<Column>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                dataGridViewColumns.DataSource = columns;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas wczytywania kolumn:\n{ex.Message}");
            }
        }
        private ComboBox comboBoxObjects;
        private TableLayoutPanel tableLayoutPanel1;
        private DataGridView dataGridViewColumns;
    }
}
