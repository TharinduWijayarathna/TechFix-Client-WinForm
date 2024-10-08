﻿using Newtonsoft.Json;
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
    public partial class OrderItemForm : Form
    {
        int orderId;
        public OrderItemForm()
        {
            InitializeComponent();
        }

        public OrderItemForm(int orderId)
        {
            InitializeComponent();
            LoadData();
            this.orderId = orderId;
        }

        private void LoadData()
        {
            string url = "https://localhost:7135/api/OrderItem/order/" + orderId;
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
                                        Deserialize<List<OrderItem>>(items);

                AddActionColumn();
            }
        }

        private void OrderItemForm_Load(object sender, EventArgs e)
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

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int r = e.RowIndex;
            int c = e.ColumnIndex;
            if (c == dgvItems.Columns.Count - 1) // Check if it's the last column (Edit button)
            {
                txtID.Text = dgvItems.Rows[r].Cells["Id"].Value.ToString();
                txtName.Text = dgvItems.Rows[r].Cells["Name"].Value.ToString();
                txtQuantity.Text = dgvItems.Rows[r].Cells["Quantity"].Value.ToString();
                txtDes.Text = dgvItems.Rows[r].Cells["Description"].Value.ToString();
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            string url = "https://localhost:7135/api/OrderItem";

            using (HttpClient client = new HttpClient())
            {
                OrderItem item = new OrderItem
                {
                    Name = txtName.Text,
                    OrderId = orderId,
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
                        MessageBox.Show("Order Item added successfully");
                        LoadData();
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Failed to add Order Item: {response.StatusCode} - {errorContent}");
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
            string url = "https://localhost:7135/api/OrderItem/" + txtID.Text;
            HttpClient client = new HttpClient();

            OrderItem item = new OrderItem
            {
                Id = Convert.ToInt32(txtID.Text),
                Name = txtName.Text,
                OrderId = orderId,
                Quantity = Convert.ToInt32(txtQuantity.Text),
                Description = txtDes.Text
            };

            string info = JsonConvert.SerializeObject(item);
            var content = new StringContent(info, Encoding.UTF8, "application/json");

            var response = client.PutAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Order Item updated successfully");
                LoadData();
            }
            else
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                MessageBox.Show($"Failed to update Order Item: {response.StatusCode} - {errorContent}");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string url = "https://localhost:7135/api/OrderItem/" + txtID.Text;
            HttpClient client = new HttpClient();

            var response = client.DeleteAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Order Item deleted successfully");
                LoadData();
            }
            else
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                MessageBox.Show($"Failed to delete Order Item: {response.StatusCode} - {errorContent}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            OrderForm orderForm = new OrderForm();
            orderForm.ShowDialog();
            this.Close();
        }
    }
}
