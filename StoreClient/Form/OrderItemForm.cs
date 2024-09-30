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
            }
        }

        private void OrderItemForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
