using Newtonsoft.Json;
using StoreClient.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace StoreClient
{
    public partial class QuoteRequestItemForm : Form
    {
        public int QuoteRequestId { get; set; }
        public QuoteRequestItemForm()
        {
            InitializeComponent();
        }

        public QuoteRequestItemForm(int quoteRequestId)
        {
            InitializeComponent();
            QuoteRequestId = quoteRequestId;
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            string url = "https://localhost:7135/api/QuoteRequestItem";

            using (HttpClient client = new HttpClient())
            {
                QuoteRequestItem item = new QuoteRequestItem
                {
                    Name = txtName.Text,
                    QuoteRequestId = QuoteRequestId,
                    Quantity = Convert.ToInt32(txtQuantity.Text),
                    Description = txtDes.Text
                };

                string info = JsonConvert.SerializeObject(item);
                var content = new StringContent(info, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Quote Request Item added successfully");
                        LoadData();
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Failed to add Quote Request Item: {response.StatusCode} - {errorContent}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void LoadData()
        {
            string url = "https://localhost:7135/api/QuoteRequestItem/quote/" + QuoteRequestId;
            HttpClient client = new HttpClient();
            var resTask = client.GetAsync(url);
            resTask.Wait();
            var result = resTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsStringAsync();
                readTask.Wait();
                var items = readTask.Result;
                dgvItems.DataSource = null;
                dgvItems.DataSource = (new JavaScriptSerializer()).
                                        Deserialize<List<QuoteRequestItem>>(items);

                AddActionColumn();
            }
        }

        private void QuoteRequestItemForm_Load(object sender, EventArgs e)
        {
            LoadData();
            CustomizeDataGridView();
        }

        private void CustomizeDataGridView()
        {
            dgvItems.EnableHeadersVisualStyles = false;
            dgvItems.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dgvItems.ColumnHeadersDefaultCellStyle.Font = new Font("Gabriola", 13, FontStyle.Bold);
            dgvItems.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvItems.DefaultCellStyle.Font = new Font("Gabriola", 12, FontStyle.Regular);
            dgvItems.DefaultCellStyle.ForeColor = Color.Black;
            dgvItems.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            dgvItems.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dgvItems.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvItems.RowTemplate.Height = 30;
            dgvItems.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;

            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            AddActionColumn();
        }

        private void AddActionColumn()
        {
            // Remove existing action column if it exists
            if (dgvItems.Columns["EditColumn"] != null)
                dgvItems.Columns.Remove("EditColumn");

            // Add Edit button column
            DataGridViewButtonColumn editButton = new DataGridViewButtonColumn();
            editButton.Name = "EditColumn";
            editButton.HeaderText = "Action";
            editButton.Text = "Edit";
            editButton.UseColumnTextForButtonValue = true;
            dgvItems.Columns.Add(editButton);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string url = "https://localhost:7135/api/QuoteRequestItem/" + txtID.Text;
            HttpClient client = new HttpClient();

            QuoteRequestItem item = new QuoteRequestItem
            {
                Id = Convert.ToInt32(txtID.Text),
                Name = txtName.Text,
                QuoteRequestId = QuoteRequestId,
                Quantity = Convert.ToInt32(txtQuantity.Text),
                Description = txtDes.Text
            };

            string info = JsonConvert.SerializeObject(item);
            var content = new StringContent(info, Encoding.UTF8, "application/json");

            var response = client.PutAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Quote Request Item updated successfully");
                LoadData();
            }
            else
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                MessageBox.Show($"Failed to update Quote Request Item: {response.StatusCode} - {errorContent}");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string url = "https://localhost:7135/api/QuoteRequestItem/" + txtID.Text;
            HttpClient client = new HttpClient();

            var response = client.DeleteAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Quote Request Item deleted successfully");
                LoadData();
            }
            else
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                MessageBox.Show($"Failed to delete Quote Request Item: {response.StatusCode} - {errorContent}");
            }
        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int r = e.RowIndex;
            int c = e.ColumnIndex;
            if (c == dgvItems.Columns.Count - 1)
            {
                txtID.Text = dgvItems.Rows[r].Cells["Id"].Value.ToString();
                txtName.Text = dgvItems.Rows[r].Cells["Name"].Value.ToString();
                txtQuantity.Text = dgvItems.Rows[r].Cells["Quantity"].Value.ToString();
                txtDes.Text = dgvItems.Rows[r].Cells["Description"].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            QuoteRequestForm quoteRequestForm = new QuoteRequestForm();
            quoteRequestForm.ShowDialog();
            this.Close();
        }
    }
}
