using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Collections;

namespace Sinema_Rezervasyon
{
    public partial class Form1 : Form
    {
        string connectionString= @"Data Source=DESKTOP-C8L726O\SQLEXPRESS;Initial Catalog=Sinema;Integrated Security=True";//db string
        bool[] secili = new bool[24];  //true false dan oluşur , 24 koltuğu ifade eder ve koltuğa tıklanmışsa o koltuğun indexi true yapılır
        List<int> satilanKoltuklar = new List<int>(); //satilan koltukların listesi integer Liste
        ArrayList list = new ArrayList(); //

        public Form1()
        {
            InitializeComponent();
        }
        public void FilminResminiGetir() //anasayfada seçilen filmin resmi gelmesi için
        {
            string query = "SELECT FilmIsmi , FilmResim FROM Film where FilmID=@isim ";// filmin adı ve resmini getiren sorgu
                                        //@isim kısmını biz sorguya ekliyoruz paramater.addwithvalue methoduyla aşşağıda
            using (var con = new SqlConnection(connectionString))//databaseye bağlan
            {
                con.Open();//connection aç
                SqlCommand cmd = new SqlCommand(query, con);//sorguyu ve connectionu SqlCommand a yolladık bu databaseye sorguyu aktaracak
                cmd.Parameters.AddWithValue("@isim", comboBoxFilm.SelectedValue); //@isim kısmını comboboxdan aldık -filmin ismi
                SqlDataReader dr = cmd.ExecuteReader(); //sqldatareader sorguyu okur sorgudan dönen değerlere göre resmi göstericez
                dr.Read();
                if (dr.HasRows)//eğer dataread boş değilse 
                {
                    byte[] images = ((byte[])dr[1]); //dr[1] FilmResim kolonuna karşılık geliyor veritabanı kısmı için
                    //eğer image yani resim varsa byte dizisi türünden images e koyuyoruz . resimler byte cinsinden saklanıyor
                    if (images == null)// eğer resim yoksa resim bölümünü null yap yani dabaseden resim dönmediyse
                    {
                        pictureBox1.Image = null;
                    }
                    else//eğer databaseden resim döndüyse bunu PictureBox a koy 
                    {//resmi koymak için MemoryStream methodu kullanılıyor bu method c# da gömülü 
                        MemoryStream mstreem = new MemoryStream(images);//yeni class oluştur images i parametre olarak ver ve bu classı
                       //hafızada tutulan bir stream oluşturur (MemoryStream) parametre olarak byte[] alır bu yüzden resimleri byte[] şeklinde tuttuk
                        pictureBox1.Image = Image.FromStream(mstreem);//picturebox ı az önce oluşturulan mstreem i image  oluşturduk
                    }
                }
                con.Close();//bağlantyı kapadık

            }
        }
       
        public void KoltukDuzenle()
        {
            using (SqlConnection con = new SqlConnection(connectionString))//bağlatı oluşturduk
            {
                string sql = "select Koltuk from Bilet ";//dolu koltukları db den aldık
                SqlDataAdapter da = new SqlDataAdapter(sql, con);//data adapter ve dataset oluşturduk
                DataSet ds = new DataSet();
                da.Fill(ds);//datasetimizi data adapter kullanarak doldurduk
                //datasete sql den çektiğimiz koltuk kolonunun üyeleriyle doldurduk
                foreach(DataRow item in ds.Tables[0].Rows)// dataset(ds) deki  satırları teker teker bakıyoruz ve 
                {                                       // satırlardaki değerleri listeye ekliyoruz
                    list.Add(item["Koltuk"].ToString());//koltukları listeye attık birden fazla olduğu için
                }     //ArrayList list = new ArrayList();  eklediğimiz listeyi program başında tanımladık global değişken yani her methodda kullanılabilir değişken


            }
            foreach (Control control in this.Controls)//o formdaki tüm nesneleri control eder (this olduğu için bu form u işaret ediyor)
            {//Control türünden bir control isimli nesne formumuzdaki combobox buton text gibi şeyleri dolaşır ve bunlardan
                //buton olanları alır ve işlem yapar
                foreach (string butonName in list) { // yukarıdali koltukları doldurduğumuz listeyi burada kullanıp buton isimleriyle karşılaştırdık 
                    if (control is Button && control.Text == butonName) {//eğer ekranda buton varsa ve adı listedeki butonlara eşitse
                        control.Enabled = false;                        //yani dolu (koltuk numaraları= butonda yazan sayı) ise o butonun
                        control.BackColor = Color.Green;                //rengini değiştir ve enable false yap ki tıklanamasın
                    }
                    //control burda button a denk geliyor if içinde control butonsa ve buton adı listeden çektiğimiz koltuk isimlerine eşitse dediğimiz için
                }        
            }
        }
        private void btnBiletAl_Click(object sender, EventArgs e)
        {   //sql e gidecek olan sorgu, ekranda seçilenleri sql server a kaydeder
            string query = "INSERT INTO[dbo].[Bilet]([IsimSoyisim],[FilmID],[Salon],[Ucret],[Tarih],[Seans],[Koltuk]) "+
                          " VALUES (@isimSoyisim,@film,@salon,@ucret,@tarih,@seans,@koltuk)";
                            //sql de tabloya veri eklemek için gereken sorgu @ yazılanlar bizim ekleyeceğimiz parametreler .
            for (int i = 0; i < secili.Length; i++)//koltuk sayısı kadar for döngüsü
            {
                if (secili[i] == true) //seçili olan koltuk varsa if içine girer
                    satilanKoltuklar.Add(i + 1); //index 0 dan başladığından koltuk sayısı +1 olarak ekleniyor
            }


            using (SqlConnection con = new SqlConnection(connectionString))//bağlantı tanımlandı
            {
                using (SqlCommand cmd = new SqlCommand(query, con))//sql commanda sorgu ve bağlantı parametreleri verildi
                {  // List<int> satilanKoltuklar = new List<int>(); satılan koltukları yukarıda listeye eklemişti
                    for (int i =0;i<satilanKoltuklar.Count;i++)//liste uzunluğu kadar for döngüsü yapıyoruz
                    {//for döngüsüyle veritabanına veri ekleyeceğiz çünkü birden fazla koltuk seçilmiş olabilir.
                        //isim texti boş bırakılmış mı veya white space yani boşluk girimiş mi diye kontrol ediyoruz böyleyse hata yolluyoruz ekrana
                        if (txtAdSoyad != null && !string.IsNullOrWhiteSpace(txtAdSoyad.Text))
                        {//text adsoyad boş veya boşluktan oluşmuyorsa 

                            con.Open(); //bağlantıyı açıyoruz sorgu göndermek için açmak gerekiyor
                            //yukarıdaki sorguya dışarıdan ekleyeceğimiz parametreler
                            //parametre eklemek için Parameters.AddWithValue kullanıyoruz
                            cmd.Parameters.AddWithValue("@isimSoyisim", txtAdSoyad.Text);
                            cmd.Parameters.AddWithValue("@film", comboBoxFilm.SelectedValue);
                            cmd.Parameters.AddWithValue("@salon", comboBoxSalon.SelectedValue);//selected valuelarda ID saklı
                            cmd.Parameters.AddWithValue("@ucret", comboBoxUcret.SelectedValue);
                            cmd.Parameters.AddWithValue("@tarih", dateTimePicker.Value);
                            cmd.Parameters.AddWithValue("@seans", comboSeans.SelectedValue);
                            cmd.Parameters.AddWithValue("@koltuk", satilanKoltuklar[i]);//seçilen koltukları listeden for döngüsüne göre teker teker ekliyoruz
                            try//aşağıdakiler çalıştırıyoruz olursa catch e girmeyecek ama kod hatalıysa catch yakalayacak 
                            {
                                cmd.ExecuteNonQuery();//sorguyu çalıştırmak için
                                MessageBox.Show(
                                   "   İSİM SOYİSİM  :   " + txtAdSoyad.Text + "\n\n" +
                                   "   KOLTUK NUMARASI  :  " + satilanKoltuklar[i].ToString() + "\n\n" +
                                   "   SALON NUMARASI  :   " +comboBoxSalon.Text  + "\n\n" +
                                   "   BİLET TUTARI  :   " +comboBoxUcret.Text + "\n\n" +
                                   "   FİLM SAATİ   :   "+comboSeans.SelectedValue , "Bilet Bilgileri ");
                                seciliTemizle();//listeyi temizlemek icin .Secili dizisi koltukları işaret ediyordu 
                                //aynı koltukları birdaha satın almamasını engellemek için diziyi false ile doldurduk.
                            }
                            catch (Exception ex)
                            {// hata varsa yakala ve ekrana mesaj yazdır program kapanmasın
                                MessageBox.Show("Bilet Alınamadı");
                            }
                            finally
                            {// son olarak finally içindekiler kesin yapılır 
                                cmd.Parameters.Clear();//parametreleri temizledik aynı parametre değerleri tekrar giderse hata alıyoruz.
                                con.Close();//bağlantıyı kapadık
                            }
                        }
                        else
                            MessageBox.Show("Lütfen adınızı girin.");//yukarıda isim textini kontrol etmiştik eğer yanlışsa bu hatayı veriyor.
              
                    }
                }
            }
            satilanKoltuklar.Clear();//döngü btince satılan koltukların listesini temizliyor  birdahaki alımda karışmaması için
            KoltukDuzenle();//yukarıdaki koltuk düzenle methodunu çağırıyor ve aldığımız koltukların renkleri değişiyor ve tıklanamyacakh hale getiriliyor.
        }
        public void seciliTemizle()//bu method secili dizisini false ile dolduruyor ki koltukların seçilmiş olduğu gözükmesin
        {                           //aynı koltuklar bir daha satın alınmasın liste temizlensin diye.
            for(int i=0; i < secili.Length; i++)
            {
                secili[i] = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {//LOAD bize form yani program çalıştırılırken gelen ekrana neler yükleneceğini gösterir
            Ucret();//ücret comboboxını dolduran method
            Seans();//seans comboboxını dolduran method
            Salon();//salon comboboxıı dolduran emthod 
            comboBoxFilm.ValueMember = "FilmID";    //film combobox ını veritabanından dolduruyoruz value member ekranda gözükmez seçtiğimiz filmin ID sini gösterir biz hangi filmi seçersen onun ID si burada kayıtlıdır ve o ID ile veritabanında işlem yaparız.
            comboBoxFilm.DisplayMember = "FilmIsmi";//film combobox ında göüken film isimleri.
            comboBoxFilm.DataSource = Film(); //combobox veri kaynağı film() methoduyla dolduruluyor ki yukarıdaki comboBoxFilm.ValueMember = "FilmID"; ve comboBoxFilm.DisplayMember = "FilmIsmi" işlemlerini veritabanından çekip yapabilelim.
             KoltukDuzenle(); //koltuklar öceden satılmışsa bize satılan koltukları satın alınamayacak şekilde göstermeyi sağlayan method.
        }
        public DataTable Film()
        {// bu method veritabanından filmin ID sini ver ismini çekiyor bu methodu comboboxa koyabilmek için kullanıyoruz
            using(var con = new SqlConnection(connectionString))//bağlantı
            using(SqlCommand cmd = new SqlCommand("Select FilmID, FilmIsmi From Film ",con))
            using (var da = new SqlDataAdapter(cmd))//sql komutu
            {
                var dt = new DataTable();//datatable oluştur ve data adapter ile data table i doldur
                da.Fill(dt);
                return dt; //datatable ı geri döndür ve comboBoxFilm.DataSource = Film() da kullan.
            }
        }

        private void buttonAllClick(object sender, EventArgs e)
        {
            Button tiklananButon = (Button)sender; //sender tıklanan butonu temsil ediyor
            int koltukNo = Convert.ToInt32(tiklananButon.Text);//string i int e çevirdik
            int siraNo = koltukNo - 1; //index sayisi 0 dan başladığından indexte kullanmak için koltuk sayısı -1 kullandık
            //      bool[] secili = new bool[24];

            if (secili[siraNo])//==true ise 
            {//eğer tıklanmışsa eski resmi yükle ve o dizideki indexe karşılık gelen değeri false yap
                tiklananButon.Image= Image.FromFile(@"C:\Users\bar1s\Desktop\kltSec.png");
                secili[siraNo] = false;
            }
            else//eğer tıklanmamışsa tıklanmış halini aşağıdaki fotoğraf ile değiştir ve secili dizisinde o indexin değerini true yap
            {
                tiklananButon.Image = Image.FromFile(@"C:\Users\bar1s\Desktop\kltSenin.png");
                secili[siraNo] = true;
            }




        }
        public void Ucret()
        {//ücret combobox ını doldurmak için ..
            comboBoxUcret.DisplayMember = "Text";//comboboxda görülen kısım
            comboBoxUcret.ValueMember = "Value";//görülen satırın değeri
            var items = new[] {   //item new{Text , value}
                new { Text = "Öğrenci Bileti 10 ₺", Value = "10" },//ekranda gösterilen ve değeri
                new { Text = "Normal bilet 15 ₺", Value = "15" }};
            comboBoxUcret.DataSource = items;// bu diziyi satasource a yüklüyoruz 
        }
        public void Salon()//salon combobox ını doldurmak için ..
        {
            comboBoxSalon.DisplayMember = "Text";//yine text ve value kısımları istediğimiz değerlerde dolduruluyor ve
            comboBoxSalon.ValueMember = "Value";//data source a konuluyor ki combobox bu değerler göstersin
            var items = new[] {  
                new { Text = "Salon 1", Value = "1" },
                new { Text = "Salon 2", Value = "2" },
                new { Text = "Salon 3", Value = "3" }
            };
            comboBoxSalon.DataSource = items;

        }
        public void Seans()//seans combobox ını doldurmak için ..
        {
            comboSeans.DisplayMember = "Text";
            comboSeans.ValueMember = "Value";

            var items = new[]{
            new{Text = "Sabah Seansı",Value="12:00:00"},
            new{Text = "Gündüz Seansı",Value="18:00:00"},
            new{Text = "Gece Seansı",Value="23:00:00"}

            };
            comboSeans.DataSource = items;
        }

        private void btnFilmEkle_Click(object sender, EventArgs e)
        {//film eklenen butonu tetikler ve yeni sayfa aaçar
            this.Hide(); //bu sayfayı gizler
            Admin admin = new Admin();//film formunu açmak için filmEkle classından f değişkenini oluştur ve f.show ile aç
            admin.Show();
        }


        private void comboBoxFilm_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FilminResminiGetir(); //seçilen film hangisiyse onun resmini ekrana göseren methodu çağırıyor 
        }

        private void BtnKoltukYenile(object sender, EventArgs e) //koltukları siler
        {//bu kısım evet hayır kutusu çıkarır 
            DialogResult dialogResult = MessageBox.Show("Tüm koltukları silmek istediğinize emin misiniz ? ", 
                "Biletler Silinecek", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)//eğer eveti seçersek
            {
                string query = " delete from Bilet where BiletID>0 ";//tüm biletleri silen sorgu
                using (SqlConnection con = new SqlConnection(connectionString)) //bağlantı tanımlanıyor
                using (SqlCommand cmd = new SqlCommand(query, con))//komut(command) tanımlanıyor
                {
                    con.Open();//bağlantıyı a.tık
                    try//silmeyi deniyoruz
                    {
                        cmd.ExecuteNonQuery(); //komutu yani sorguyu çalıştırıyoruz
                        MessageBox.Show("Tüm Koltuklar Yenilendi");//silindiğine dair mesaj
                        foreach (Control control in this.Controls)//o formdaki tüm nesneleri control eder
                        {
                            if (control is Button && control.BackColor == Color.Green)//silinen koltuk butonlarını eski haline 
                            {                                                           //getirmek için renk ve resim şeklini değiştirir
                                control.Enabled = true;                                 //seçilebilirliği true yapar ve artık tıklayabiliriz.
                                control.BackColor = Color.Azure;
                            }
                        }
                    }
                    catch(Exception ex1) //silme başarısızsa burada hata yakalanır
                    {
                        MessageBox.Show("Koltuk Yenileme Başarısız");
                    }
                    finally//en sonunda silse de silemese de bağlantıyı kapatır
                    {
                        con.Close();
                    }

                }
            }
            else if (dialogResult == DialogResult.No)
            {
                //hayır ı seçersek burası çalışır şimdilik boş
            }
        }

        private void txtAdSoyad_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back);
            //bu kod sadece karakter girmeyi sağlar. İsim kısmına sayı yazmayı engeller
        }
    }
}
