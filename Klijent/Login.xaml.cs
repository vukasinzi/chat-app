using Klijent.Kontroleri_GUI_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Klijent
{
    
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            await LoginGuiKontroler.Instance.LogIn(txt_korisnicko_ime.Text,txt_lozinka.Password);
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
