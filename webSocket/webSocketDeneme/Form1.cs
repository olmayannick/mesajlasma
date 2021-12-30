using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace webSocketDeneme
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static int Port = 0;
        public static Mesajlar tumMesajlar = new Mesajlar();
        public static P2PClient client = new P2PClient();
        public static P2PServer server = null;
        Thread islem;
        List<string> dizi = new List<string>();

        private void button1_Click(object sender, EventArgs e)
        {
            Port = Convert.ToInt32(textBox1.Text);
            server = new P2PServer();
            server.Start();

            IPEndPoint[] liste = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            foreach (var item in liste)
            {
                for (int i = 5000; i < 5010; i++)
                {
                    if (i != Port && item.Port == i)
                    {
                        //MessageBox.Show(i.ToString());
                        client.Connect($"ws://127.0.0.1:{i}/MessengerApp");
                    }
                }
            }
        }

        public static void mesajYazdir(string mesaj)
        {
            MessageBox.Show(mesaj);
        }
        public string deComputeSha256Hash(string sifreli)
        {
            string sonuc = "";
            string pass = textBox2.Text;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] data = Convert.FromBase64String(sifreli);
                //byte[] data = UTF8Encoding.UTF8.GetBytes(sifreli);
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(pass));
                    using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                    {
                        ICryptoTransform transform = tripDes.CreateDecryptor();
                        byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                       // sonuc = Convert.ToBase64String(results, 0, results.Length);
                        sonuc = UTF32Encoding.UTF8.GetString(results);
                    }
                }
                return sonuc;

            }
        }
        public  string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256 
            /*string sonuc = "";
            string pass = "123";
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] data = UTF8Encoding.UTF8.GetBytes(rawData);
                using (SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider())
                {
                    byte[] keys = sha256.ComputeHash(UTF8Encoding.UTF8.GetBytes(pass));
                    using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode= CipherMode.ECB,Padding=PaddingMode.PKCS7})
                    {
                        ICryptoTransform transform = tripDes.CreateEncryptor();
                        byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                        sonuc= Convert.ToBase64String(results, 0, results.Length);
                    }
                }
                return sonuc;

            }*/
            string sonuc = "";
            string pass = textBox2.Text;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] data = UTF8Encoding.UTF8.GetBytes(rawData);
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(pass));
                    using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode= CipherMode.ECB,Padding=PaddingMode.PKCS7})
                    {
                        ICryptoTransform transform = tripDes.CreateEncryptor();
                        byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                        sonuc = Convert.ToBase64String(results, 0, results.Length);// +"æ"+pass;
                    }
                }
                return sonuc;

            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            textBox4.Text = ComputeSha256Hash(textBox3.Text);
            tumMesajlar.mesajListe.Add(textBox4.Text);
            //tumMesajlar.sifreliMesajList.Add(ComputeSha256Hash(textBox3.Text));
            IPEndPoint[] liste = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            foreach (var item in liste)
            {
                for (int i = 5000; i < 5010; i++)
                {
                    if (i != Port && item.Port == i)
                    {
                        //MessageBox.Show(i.ToString());
                        //client.Connect($"ws://127.0.0.1:{i}/MessengerApp");

                        client.Send($"ws://127.0.0.1:{i}/MessengerApp", JsonConvert.SerializeObject(Form1.tumMesajlar));
                    }
                }
            }
            textBox3.Text = string.Empty;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*Port = Convert.ToInt32(textBox2.Text);
            client.Connect($"ws://127.0.0.1:{Port}/MessengerApp");
            client.Send($"ws://127.0.0.1:{Port}/MessengerApp", "deneme");*/
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form1.CheckForIllegalCrossThreadCalls = false;
            islem = new Thread(new ThreadStart(veriKontrol));
            islem.Start();
        }

        void veriKontrol()
        {
            while (true)
            {
                richTextBox1.Clear();
                for (int i = 0; i < tumMesajlar.mesajListe.Count; i++)
                {
                    richTextBox1.Text += deComputeSha256Hash(tumMesajlar.mesajListe[i]) + "\n";
                }
                Thread.Sleep(500);
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            dizi.Add("asad");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            islem.Abort();
        }
    }
}
