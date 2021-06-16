using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sinema_Rezervasyon
{
    public partial class Bilet : Form
    {
        string connectionString = @"Data Source=DESKTOP-C8L726O\SQLEXPRESS;Initial Catalog=Sinema;Integrated Security=True";

        public Bilet()
        {
            InitializeComponent();
        }

        private void Bilet_Load(object sender, EventArgs e)
        {
            DataGridViewDoldur();
        }
        public void  DataGridViewDoldur()
        {
            string query = "select IsimSoyisim, Salon,Tarih,Seans,Koltuk,FilmIsmi,Ucret from Bilet "+
                            "inner join Film on Film.FilmID = Bilet.FilmID ";
            using (SqlConnection con = new SqlConnection(connectionString))//bağlantı oluşturuluyor
            using(SqlCommand cmd = new SqlCommand(query, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                con.Open();
                cmd.ExecuteNonQuery();
                var dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                con.Close();
            }

        }

        private void btnGeri_Click(object sender, EventArgs e)
        {
            this.Hide();
            AraMenu a = new AraMenu();
            a.Show();
        }
    }
}
