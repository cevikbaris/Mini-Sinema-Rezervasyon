using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sinema_Rezervasyon
{
    public partial class FilmEkle : Form
    {   //bizi veritabanına bağlayacak string
        string connectionString = @"Data Source=DESKTOP-C8L726O\SQLEXPRESS;Initial Catalog=Sinema;Integrated Security=True";

        public FilmEkle()
        {
            InitializeComponent();
        }

        private void btnResim_Click(object sender, EventArgs e) //resim seçme butonu
        {                                  //bu türdeki resim türlerini kabul eder. Bize bilgisayardan dosya seçebileceğimiz bir ekran açar
            using(OpenFileDialog ofd = new OpenFileDialog() {Filter = "Image Files(*.jpeg;*.bmp;*.png;*.jpg)|*.jpeg;*.bmp;*.png;*.jpg", Multiselect=false })
            {
                if (ofd.ShowDialog() == DialogResult.OK)// ekrandan seçtiğimiz bir resim varsa
                {
                    pictureBox1.Image = Image.FromFile(ofd.FileName); //seçtiğimiz resmi picture box kısmına yükler resim görünür hale gelir
                    txtFileName.Text = ofd.FileName; //resimin yolunu bize text de gösterir
                }
            }
        }
        public void Insert(string filename, byte[] image)//veritabanına resim i filmi kaydetmek için
        {
            string query = "INSERT INTO Film  (FilmIsmi ,FilmResim) VALUES(@filmAdı,@resim) ";//veritabanına yollanan kayıt sorgusu
            using (SqlConnection con = new SqlConnection(connectionString))//bağlantı oluşturuluyor
            {
                using(SqlCommand cmd =new SqlCommand(query,con))//sorgu ve bağlantı komuta aktarılıyor
                {
                    con.Open();//bağlantı açılıyor
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@filmAdı", txtFilmAdı.Text);//sorguya parametreleri eke
                    cmd.Parameters.AddWithValue("@resim", image);//
                    cmd.ExecuteNonQuery();//sorguyu çalıştır
                    con.Close();//bağlantıyı kapat
                }
            }
        }


        byte[] ConvertImageToBytes(Image img)
        {
            using(MemoryStream ms= new MemoryStream())
            {
                if (img != null)
                {
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray(); // byte[] cinsinden method
                }
                else
                {
                    MessageBox.Show("Lütfen Resim Ekleyin.");
                    return null;
                }
            }
        }

        private void btnFilmEkle_Click(object sender, EventArgs e)
        {
            if (txtFilmAdı.Text == "")
                MessageBox.Show("Lütfen Filmin İsmini Girin.");
            else
            {
                try
                {
                    Insert(txtFileName.Text, ConvertImageToBytes(pictureBox1.Image));
                    MessageBox.Show("Film Başarıyla Eklendi");
                }
                catch
                {
                    MessageBox.Show("Film Ekleme Başarısız");
                }
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
