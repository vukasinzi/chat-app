using Klijent.Domen;
using Klijent.Kontroleri_GUI_;
using System;
using System.Collections.Generic;
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
using System.Windows.Threading;
using Zajednicki;
using Zajednicki.Domen;

namespace Klijent
{
    
    public partial class MainWindow : Window
    {
        private Korisnik k;
        Korisnik primalac;
        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(Korisnik k)
        {
            InitializeComponent();
            this.k = k;
            DataContext = MainGuiKontroler.Instance;
            Komunikacija.Instance.PorukaPrimljena += OnPorukaPrimljena;
            this.Closed += (_, __) => Komunikacija.Instance.PorukaPrimljena -= OnPorukaPrimljena;

            Komunikacija.Instance.PrijateljaDodaj += OnDodajPrijatelja;
            Komunikacija.Instance.PrijateljaPrihvati += OnPrijateljaPrihvati;
            Komunikacija.Instance.PrijateljaObrisi += OnPrijateljaObrisi;
            Komunikacija.Instance.PrijateljaOdbij += OnPrijateljaOdbij;
            korisnikText.Text += "korisnik: "+k.Korisnicko_ime;

        }

        //??PORUKE??/??SEND??//
        private void OnPorukaPrimljena(Poruka p)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                int drugi;
                if (p.posiljalac_id == k.Id)
                    drugi = p.primalac_id;
                else
                    drugi = p.posiljalac_id;

                if (Kontakti.SelectedItem is Korisnik izabrani && izabrani.Id == drugi)
                    DodajPoruka(p);
            }));
        }
        private void OnDodajPrijatelja(PrijateljstvoView p)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                MainGuiKontroler.Instance.Prijateljstva.Add(p);
            }));
        }
        private void OnPrijateljaPrihvati(Korisnik k)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                MainGuiKontroler.Instance.Kontakti.Add(k);
            }));

        }
        private void OnPrijateljaObrisi(Korisnik k)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                MainGuiKontroler.Instance.Kontakti.Remove(k);//jako bitna stvar, uradicu override equalsa zato sto ovo nisu isti objekti.
                primalac = null;
                user.Text = "";
                PorukePanel.Children.Clear();
            }));

        }
        private void OnPrijateljaOdbij(PrijateljstvoView p)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                MainGuiKontroler.Instance.Prijateljstva.Remove(p);

            }));

        }
        ///////////////////////////////////////////////////////////////////
        private void DodajPoruka(Poruka p)
        {

            bool incoming = p.posiljalac_id != k.Id;

            Border bubble = new Border
            {
                Background = Brushes.Black,
  
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(10, 6, 10, 6),
                Margin = new Thickness(6),
                HorizontalAlignment = incoming ? HorizontalAlignment.Left : HorizontalAlignment.Right,
                MaxWidth = 420
            };

            TextBlock tb = new TextBlock
            {
                Text = p.poruka_text,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap
            };

            bubble.Child = tb;

            PorukePanel.Children.Add(bubble);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                ChatScroll.ScrollToEnd();
            }), DispatcherPriority.Background);
        }
        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            string poruka_text = messageText.Text;
            if (primalac == null || string.IsNullOrWhiteSpace(poruka_text))
                return;
            await MainGuiKontroler.Instance.Posalji(poruka_text, k.Id, primalac.Id);
            Poruka p = new Poruka(primalac.Id, k.Id, poruka_text);
            DodajPoruka(p);
            messageText.Clear();
        }
        //krajkrajkraj//
   
       
      

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == k.Korisnicko_ime)
                return;
            Korisnik coveculjak = new Korisnik();
            coveculjak = await MainGuiKontroler.Instance.Pretrazi(SearchTextBox.Text);
            if (coveculjak == null || coveculjak.Korisnicko_ime == "greska")
                return;
           bool uspeh =  await MainGuiKontroler.Instance.DodajPrijatelja(k.Id,coveculjak.Id);
            if (!uspeh)
                return;
            
            MessageBox.Show("Uspešno poslat zahtev za prijateljstvo korisniku " + SearchTextBox.Text);
            SearchTextBox.Clear();

        }
        private async Task UcitajPrijatelje()
        {
            try
            {
                await MainGuiKontroler.Instance.vratiSvePrijatelje(k);
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }
        private async Task UcitajZahtevePrijateljstva()
        {
            try
            {
                await MainGuiKontroler.Instance.VratiZahtevePrijatelja(k.Id);
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }


        private async void Kontakti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Kontakti.SelectedItem is not Korisnik l)
                return;

            user.Text = l.Korisnicko_ime;
            PorukePanel.Children.Clear();
            primalac = l;

            var poruke = await MainGuiKontroler.Instance.ucitajSvePoruke(primalac, k);
            if (poruke == null)
                return;

            foreach (var x in poruke)
                OnPorukaPrimljena(x);
        }


        //dinamicki buttoni
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b || b.Tag is not Korisnik u)
                return;

            bool ok = await MainGuiKontroler.Instance.ObrisiPrijateljstvo(k.Id, u.Id);
            if (!ok)
                return;

            MainGuiKontroler.Instance.Kontakti.Remove(u);

            if (primalac != null && primalac.Id == u.Id)
            {
                primalac = null;
                user.Text = "";
                PorukePanel.Children.Clear();
            }
        }
  
        

        //BUTTONI
        private async void Razgovori_Click(object sender, RoutedEventArgs e)
        {
            Kontakti.Visibility = Visibility.Visible;
            Prijatelji.Visibility = Visibility.Collapsed;
           
        }
        private void MessageText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Send_Click(Send, null);
            }
        }


        private async void Decline_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b || b.Tag is not PrijateljstvoView p)
                return;

            bool Uspesno = await MainGuiKontroler.Instance.OdbijPrijatelja(p.Link);
            if (Uspesno)
                MainGuiKontroler.Instance.Prijateljstva.Remove(p);


        }

        private async void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b || b.Tag is not PrijateljstvoView p)
                return;

            bool Uspesno = await MainGuiKontroler.Instance.PrihvatiPrijatelja(p.Link);
            if (Uspesno)
            {
                
                MainGuiKontroler.Instance.Prijateljstva.Remove(p);
            }
        }

    
        private async void PrijateljiBtn_Click(object sender, RoutedEventArgs e)
        {
            Kontakti.Visibility = Visibility.Collapsed;
            Prijatelji.Visibility = Visibility.Visible;
           

        }
        //WINDOW FORM MENADZMENT
        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Minimized;

        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
           await UcitajPrijatelje();
            await UcitajZahtevePrijateljstva();
        }
    }
}
