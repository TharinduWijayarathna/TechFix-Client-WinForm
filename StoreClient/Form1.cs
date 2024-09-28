using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Web.Script.Serialization;

namespace StoreClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string url = "https://localhost:7135/api/Stock";
            HttpClient client = new HttpClient();
            Item item = new Item();
            item.Name = txtName.Text;
            item.Description = txtDes.Text;
            item.Price = Convert.ToDecimal(txtPrice.Text);
            item.Quantity = Convert.ToInt32(txtQuantity.Text);
            string info=(new JavaScriptSerializer()).Serialize(item);
            var content=new StringContent(info,
                Encoding.UTF8,"application/json");
            var response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Stock added");
                LoadData();
            }
            else
                MessageBox.Show("Fail to add Stock");
        }

        private void LoadData()
        {
            string url = "https://localhost:7135/api/Stock";
            HttpClient client = new HttpClient();
            var resTask = client.GetAsync(url);
            resTask.Wait();
            var result = resTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsStringAsync();
                readTask.Wait();
                var items= readTask.Result;
                dgvItems.DataSource = null;
                dgvItems.DataSource = (new JavaScriptSerializer()).
                                        Deserialize<List<Item>>(items);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string url = "https://localhost:7135/api/Stock/"+txtID.Text;
            HttpClient client = new HttpClient();
            Item item = new Item();
            item.Name = txtName.Text;
            item.Description = txtDes.Text;
            item.Price = Convert.ToDecimal(txtPrice.Text);
            item.Quantity = Convert.ToInt32(txtQuantity.Text);
            string info = (new JavaScriptSerializer()).Serialize(item);
            var content = new StringContent(info,
                Encoding.UTF8, "application/json");
            var response = client.PutAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Stock Updated");
                LoadData();
            }
            else
                MessageBox.Show("Fail to update Stock");
        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int r = e.RowIndex;
            int c = e.ColumnIndex;
            if (c == 0)
            {
                txtID.Text = dgvItems.Rows[r].Cells[1].Value.ToString();
                txtName.Text = dgvItems.Rows[r].Cells[2].Value.ToString();
                txtPrice.Text = dgvItems.Rows[r].Cells[3].Value.ToString();
                txtQuantity.Text = dgvItems.Rows[r].Cells[4].Value.ToString();
                txtDes.Text = dgvItems.Rows[r].Cells[5].Value.ToString();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string url = "https://localhost:7135/api/Stock/" + txtID.Text;
            HttpClient client = new HttpClient();
            var res=client.DeleteAsync(url).Result;
            if (res.IsSuccessStatusCode)
                LoadData();
            else
                MessageBox.Show("Fail to Delete");
        }
    }
}
