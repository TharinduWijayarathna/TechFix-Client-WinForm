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
using Newtonsoft.Json;

namespace StoreClient
{
    public partial class QuoteRequestForm : Form
    {
        public QuoteRequestForm()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            string url = "https://localhost:7135/api/QuoteRequest";
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
                                        Deserialize<List<QuoteRequest>>(items);
            }
        }

        private void QuoteRequestForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            string url = "https://localhost:7135/api/QuoteRequest";

            using (HttpClient client = new HttpClient())
            {
                QuoteRequest item = new QuoteRequest
                {
                    Name = txtName.Text,
                    Date = dateTimePicker1.Value,
                    SupplierId = comboBox1.SelectedIndex + 1
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

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int r = e.RowIndex;
            int c = e.ColumnIndex;
            if (c == 0)
            {
                txtID.Text = dgvItems.Rows[r].Cells[2].Value.ToString();
                txtName.Text = dgvItems.Rows[r].Cells[3].Value.ToString();
                dateTimePicker1.Value = Convert.ToDateTime(dgvItems.Rows[r].Cells[4].Value);
                comboBox1.SelectedIndex = Convert.ToInt32(dgvItems.Rows[r].Cells[5].Value) - 1;
            }

            //if c == 1 pass the id to the next form
            if (c == 1)
            {
                int id = Convert.ToInt32(dgvItems.Rows[r].Cells[2].Value);
                QuoteRequestItemForm form = new QuoteRequestItemForm(id);
                form.Show();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string url = "https://localhost:7135/api/QuoteRequest/" + txtID.Text;

            using (HttpClient client = new HttpClient())
            {
                QuoteRequest item = new QuoteRequest
                {
                    Name = txtName.Text,
                    Date = dateTimePicker1.Value
                };

                string info = JsonConvert.SerializeObject(item);
                var content = new StringContent(info, Encoding.UTF8, "application/json");

                try
                {
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
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string url = "https://localhost:7135/api/QuoteRequest/" + txtID.Text;
            using (HttpClient client = new HttpClient())
            {
                try
                {
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
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            HomeForm homeForm = new HomeForm();
            homeForm.ShowDialog();
            this.Close();
        }
    }
}
