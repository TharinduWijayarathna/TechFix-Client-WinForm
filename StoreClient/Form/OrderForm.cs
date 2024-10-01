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
    public partial class OrderForm : Form
    {
        public OrderForm()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            string url = "https://localhost:7135/api/Order";
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
                                        Deserialize<List<Order>>(items);

                AddActionColumn();
            }
        }

        private void OrderForm_Load(object sender, EventArgs e)
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

            if (dgvItems.Columns["AddItemsColumn"] != null)
                dgvItems.Columns.Remove("AddItemsColumn");

            // Add Edit button column
            DataGridViewButtonColumn editButton = new DataGridViewButtonColumn();
            editButton.Name = "EditColumn";
            editButton.HeaderText = "Action";
            editButton.Text = "Edit";
            editButton.UseColumnTextForButtonValue = true;
            dgvItems.Columns.Add(editButton);

            // Add Order Items button column
            DataGridViewButtonColumn orderItemsButton = new DataGridViewButtonColumn();
            orderItemsButton.Name = "AddItemsColumn";
            orderItemsButton.HeaderText = "Action";
            orderItemsButton.Text = "Add Items";
            orderItemsButton.UseColumnTextForButtonValue = true;
            dgvItems.Columns.Add(orderItemsButton);
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            string url = "https://localhost:7135/api/Order";

            using (HttpClient client = new HttpClient())
            {
                Order item = new Order
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string url = "https://localhost:7135/api/Order/" + txtID.Text;

            using (HttpClient client = new HttpClient())
            {
                Order item = new Order
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
            string url = "https://localhost:7135/api/Order/" + txtID.Text;
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

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int r = e.RowIndex;
            int c = e.ColumnIndex;
            if (c == dgvItems.Columns.Count - 2)
            {
                txtID.Text = dgvItems.Rows[r].Cells["Id"].Value.ToString();
                txtName.Text = dgvItems.Rows[r].Cells["Name"].Value.ToString();
                dateTimePicker1.Value = Convert.ToDateTime(dgvItems.Rows[r].Cells["Date"].Value);
            }
            else if (c == dgvItems.Columns.Count - 1)
            {
                int id = Convert.ToInt32(dgvItems.Rows[r].Cells["Id"].Value);
                OrderItemForm form = new OrderItemForm(id);
                form.Show();
            }
        }
    }
}
