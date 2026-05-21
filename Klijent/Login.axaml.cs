using Klijent.Kontroleri_GUI_;
using Avalonia.Controls;
using Avalonia.Interactivity;

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
            bool successful = await LoginGuiKontroler.Instance.LogIn(txt_korisnicko_ime.Text ?? string.Empty, txt_lozinka.Text ?? string.Empty);
            if (successful)
                Close();
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            bool successful = await LoginGuiKontroler.Instance.RegistrujSe(txt_korisnicko_ime.Text ?? string.Empty, txt_lozinka.Text ?? string.Empty);
            if (successful)
                Close();
        }
    }
}
