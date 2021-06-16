using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sinema_Rezervasyon
{
    public partial class Admin : Form
    {
        public Admin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtKullaniciAdi.Text == "admin" && txtSifre.Text == "admin")
            {
                this.Hide();
                AraMenu m = new AraMenu();
                m.Show();
            }
            else
                MessageBox.Show("kullanıcı adı veya şifre yanlış");


        }
    }
}
