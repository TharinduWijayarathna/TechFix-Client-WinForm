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
    public partial class QuotationItemForm : Form
    {
        int quotationId;
        public QuotationItemForm()
        {
            InitializeComponent();
        }

        public QuotationItemForm(int quotationId)
        {
            InitializeComponent();
            LoadData(quotationId);
            this.quotationId = quotationId;
        }

        private void LoadData(int quotationId)
        {
            string url = "https://localhost:7135/api/QuotationItem/quotation/"+ quotationId;
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
                                        Deserialize<List<QuotationItem>>(items);
            }
        }

        private void QuotationItemForm_Load(object sender, EventArgs e)
        {
            LoadData(quotationId);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            QuotationForm Form = new QuotationForm();
            Form.ShowDialog();
            this.Close();

        }
    }
}
