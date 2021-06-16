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
    public partial class AraMenu : Form
    {
        public AraMenu()
        {
            InitializeComponent();
        }

        private void btnFilmEkle_Click(object sender, EventArgs e)
        {
            this.Hide();
            FilmEkle film = new FilmEkle();
            film.Show();
        }

        private void btnBilet_Click(object sender, EventArgs e)
        {
            this.Hide();
            Bilet bilet = new Bilet();
            bilet.Show();
        }

        private void btnGeri_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 f = new Form1();
            f.Show();
        }
    }
}
