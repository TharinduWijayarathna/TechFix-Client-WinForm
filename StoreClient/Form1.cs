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

            try
            {
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    List<Item> items = (new JavaScriptSerializer()).Deserialize<List<Item>>(jsonData);

                    dgvItems.DataSource = null;
                    dgvItems.Rows.Clear();
                    dgvItems.Columns.Clear();

                    dgvItems.AutoGenerateColumns = false;

                    // Add your existing columns
                    dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "Id", Name = "Id" });
                    dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "Name", Name = "Name" });
                    dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Price", DataPropertyName = "Price", Name = "Price" });
                    dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Quantity", DataPropertyName = "Quantity", Name = "Quantity" });
                    dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Description", DataPropertyName = "Description", Name = "Description" });

                    //set the column width
                    dgvItems.Columns["Id"].Width = 50;
                    dgvItems.Columns["Name"].Width = 185;
                    dgvItems.Columns["Price"].Width = 100;
                    dgvItems.Columns["Quantity"].Width = 70;
                    dgvItems.Columns["Description"].Width = 190;

                    //hide id column
                    dgvItems.Columns["Id"].Visible = false;



                    // Create the Edit Button Column
                    DataGridViewButtonColumn editButtonColumn = new DataGridViewButtonColumn();
                    editButtonColumn.Name = "Edit"; // Ensure the name is "Edit"
                    editButtonColumn.HeaderText = "";
                    editButtonColumn.Text = "Edit";
                    editButtonColumn.UseColumnTextForButtonValue = true; // Show text on buttons
                    dgvItems.Columns.Add(editButtonColumn);

                    dgvItems.DataSource = items;

                    // Styling (you can style as before)
                    dgvItems.EnableHeadersVisualStyles = false;
                    dgvItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(72, 168, 255);
                    dgvItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    dgvItems.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
                    dgvItems.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvItems.ColumnHeadersHeight = 40;

                    dgvItems.DefaultCellStyle.BackColor = Color.White;
                    dgvItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                    dgvItems.DefaultCellStyle.ForeColor = Color.Black;
                    dgvItems.DefaultCellStyle.Font = new Font("Arial", 9);
                    dgvItems.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
                    dgvItems.DefaultCellStyle.SelectionForeColor = Color.White;

                    dgvItems.GridColor = Color.FromArgb(200, 200, 200);
                    dgvItems.CellBorderStyle = DataGridViewCellBorderStyle.Single;

                    dgvItems.RowTemplate.Height = 30;

                }
                else
                {
                    MessageBox.Show("Error: Unable to load data from the server.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();

            txtID.Visible = false;
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
            if (e.RowIndex >= 0 && dgvItems.Columns[e.ColumnIndex].Name == "Edit")
            {
                // Get the selected row's data
                txtID.Text = dgvItems.Rows[e.RowIndex].Cells["Id"].Value.ToString();
                txtName.Text = dgvItems.Rows[e.RowIndex].Cells["Name"].Value.ToString();
                txtPrice.Text = dgvItems.Rows[e.RowIndex].Cells["Price"].Value.ToString();
                txtQuantity.Text = dgvItems.Rows[e.RowIndex].Cells["Quantity"].Value.ToString();
                txtDes.Text = dgvItems.Rows[e.RowIndex].Cells["Description"].Value.ToString();
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
