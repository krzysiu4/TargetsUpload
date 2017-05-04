using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TargetsUpload
{
    public partial class TargetsUI : Form
    {
        public TargetsUI()
        {
            InitializeComponent();
        }

        public class Target
        {
           public string type { get; set; }
           public double latitude { get; set; }
           public double longitude { get; set; }
           public string orientantion { get; set; }
           public string shape { get; set; }
           public string backgroud_color { get; set; }
           public string alphanumeric { get; set; }
           public string alphanumeric_color { get; set; }
        }

        private CookieContainer cookies = new CookieContainer();                 // Kontener na ciasteczka
        private string interopURL;                                               // Adres serwera sędziów    
        bool zalogowano = false;                                                 // Przechowuje informacje czy zalogowano

        // Logowanie do serwera sędziów
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string url = textBoxURL.Text;
            interopURL = url;                                                                    // Tylko ponowne zalogowanie zmienia url serwera
            string uri = "/api/login";
            string username = textBoxUsername.Text;
            string password = textBoxPassword.Text;
            string myParameters = "username=" + username + "&" + "password=" + password;
            using (CookieWebClient wc = new CookieWebClient(cookies))
            {
                try
                {


                    wc.BaseAddress = url;
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded"; // Potrzebne
                    string httpResult = wc.UploadString(uri, myParameters);
                    cookies = wc.CookieContainer;                                                   // Zapisuje otrzymane cookies
                    textBoxHTTPResult.Text = wc.StatusCode + httpResult;
                    zalogowano = true;
                    // textBoxHTTPResult.Text += "\nCookies: " + cookies.GetCookieHeader(new Uri(url + uri)); // Wyświetla cookie
                }
                catch (WebException ex)
                {
                    textBoxHTTPResult.Text = "Error (400) Bad Request";
                }
            }

        }

        private void buttonUploadTarget_Click(object sender, EventArgs e)
        {
            string url = interopURL;
            string uri = "/api/targets";
            string httpResult = "Error: Bad URL or not logged in";

            Target target1 = new Target();
            target1.type = "standard";
            target1.latitude = 12.133;
            target1.longitude = 12.444;
            target1.orientantion = "n";
            target1.shape = "star";
            target1.backgroud_color = "orange";
            target1.alphanumeric = "C";
            target1.alphanumeric_color = "black";

            string targetToUpload = JsonConvert.SerializeObject(target1);

            using (CookieWebClient wc = new CookieWebClient(cookies))
            {
                try
                {
                    wc.BaseAddress = url;
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json"; //  Za każdym razem trzeba dodać
                    httpResult = wc.UploadString(uri, targetToUpload);

                    textBoxHTTPResult.Text = wc.StatusCode + httpResult;
                }
                catch (WebException)
                {

                }
                catch (Exception ex)
                {

                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JPG|*jpg;*jpeg;|All files (*.*)|*.*";
            dialog.InitialDirectory = @"C:\";
            dialog.Title = "Wybierz zdjęcie do przesłania";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
               textBoxHTTPResult.Text = dialog.FileName;
               string path = dialog.FileName;
               // byte[] data = File.ReadAllBytes(path);
                pictureBox1.ImageLocation = path;
            }
        }
    }
}
