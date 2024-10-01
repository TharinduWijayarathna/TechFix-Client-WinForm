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
    public partial class QuotationForm : Form
    {
        public QuotationForm()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            string url = "https://localhost:7135/api/Quotation";
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
                                        Deserialize<List<Quotation>>(items);

                AddActionColumn();
            }
        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int r = e.RowIndex;
            int c = e.ColumnIndex;
            if (c == dgvItems.Columns.Count -1)
            {
                int id = Convert.ToInt32(dgvItems.Rows[r].Cells["Id"].Value);
                this.Hide();
                QuotationItemForm form = new QuotationItemForm(id);             
                form.ShowDialog();
                this.Close();
            }
        }

        private void QuotationForm_Load(object sender, EventArgs e)
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
            if (dgvItems.Columns["ViewItemsColumn"] != null)
                dgvItems.Columns.Remove("ViewItemsColumn");

            // Add Edit button column
            DataGridViewButtonColumn viewItemsButton = new DataGridViewButtonColumn();
            viewItemsButton.Name = "ViewItemsColumn";
            viewItemsButton.HeaderText = "Action";
            viewItemsButton.Text = "View Items";
            viewItemsButton.UseColumnTextForButtonValue = true;
            dgvItems.Columns.Add(viewItemsButton);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            HomeForm form = new HomeForm();
            form.ShowDialog();
            this.Close();
          
        }
    }
}
