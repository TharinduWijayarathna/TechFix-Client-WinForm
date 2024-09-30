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
                        MessageBox.Show("Quote Request added successfully");
                        LoadData();
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Failed to add Quote Request: {response.StatusCode} - {errorContent}");
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
            }
        }

        private void QuoteRequestItemForm_Load(object sender, EventArgs e)
        {
            LoadData();
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
                MessageBox.Show("Quote Request updated successfully");
                LoadData();
            }
            else
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                MessageBox.Show($"Failed to update Quote Request: {response.StatusCode} - {errorContent}");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string url = "https://localhost:7135/api/QuoteRequestItem/" + txtID.Text;
            HttpClient client = new HttpClient();

            var response = client.DeleteAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Quote Request deleted successfully");
                LoadData();
            }
            else
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                MessageBox.Show($"Failed to delete Quote Request: {response.StatusCode} - {errorContent}");
            }
        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int r = e.RowIndex;
            int c = e.ColumnIndex;
            if (c == 0)
            {
                txtID.Text = dgvItems.Rows[r].Cells[1].Value.ToString();
                txtName.Text = dgvItems.Rows[r].Cells[2].Value.ToString();
                txtQuantity.Text = dgvItems.Rows[r].Cells[4].Value.ToString();
                txtDes.Text = dgvItems.Rows[r].Cells[5].Value.ToString();
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
