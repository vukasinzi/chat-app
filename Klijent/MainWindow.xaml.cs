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
        private readonly Dictionary<int, List<Poruka>> chats = new();
        private readonly DispatcherTimer tajmer;
        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(Korisnik k)
        {
            InitializeComponent();
            this.k = k;
       
            Komunikacija.Instance.PorukaPrimljena += OnPorukaPrimljena;
            this.Closed += (_, __) => Komunikacija.Instance.PorukaPrimljena -= OnPorukaPrimljena;

            tajmer = new DispatcherTimer(DispatcherPriority.Background);
            tajmer.Interval = TimeSpan.FromSeconds(1);
           tajmer.Tick += Tajmer_Tick;
            korisnikText.Text += "korisnik: "+k.Korisnicko_ime;
        }

        private void Tajmer_Tick(object? sender, EventArgs e)
        {
            ProveriNovePrijatelje();
            UcitajPrijatelje();
        }

        private void OnPorukaPrimljena(Poruka p)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                int otherId;
                if (p.posiljalac_id == k.Id)
                    otherId = p.primalac_id;
                else
                    otherId = p.posiljalac_id;

                if (!chats.ContainsKey(otherId))
                    chats[otherId] = new List<Poruka>();

                chats[otherId].Add(p);

                if (Kontakti.SelectedItem is ListBoxItem item &&
                    item.Tag is Korisnik other &&
                    other.Id == otherId)
                {
                    DodajPoruka(p);
                }
            }));
        }

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

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            string poruka_text = messageText.Text;
            ListBoxItem item = (ListBoxItem)Kontakti.SelectedItem;
            Korisnik primalac = (Korisnik)item.Tag;
            await MainGuiKontroler.Instance.Posalji(poruka_text, k.Id,primalac.Id);
            Poruka p = new Poruka(primalac.Id, k.Id, poruka_text);
            DodajPoruka(p);
            messageText.Clear();
        }

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
            UcitajPrijatelje();
            MessageBox.Show("Uspešno poslat zahtev za prijateljstvo korisniku " + SearchTextBox.Text);
            SearchTextBox.Clear();

        }
        private async void UcitajPrijatelje()
        {
            try
            {
                await MainGuiKontroler.Instance.vratiSvePrijatelje(k);
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }

            Kontakti.Items.Clear();
            foreach (Korisnik k in MainGuiKontroler.Instance.prijatelji)
            {
                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var txt = new TextBlock
                {
                    Text =k.Korisnicko_ime,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 13,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    Margin = new Thickness(8, 0, 8, 0)
                };
                Grid.SetColumn(txt, 0);
                ListBoxItem i = new ListBoxItem
                {
                    Content = grid,
                    Tag = k,
                    Padding = new Thickness(6),
                    Margin = new Thickness(2),
                    MinHeight = 40
                };
                grid.Children.Add(txt);
                Kontakti.Items.Add(i);
            }
        }
        private async void Kontakti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem i = (ListBoxItem)Kontakti.SelectedItem;
            Korisnik primalac = new Korisnik();
            if (Kontakti.SelectedItem is ListBoxItem item && item.Tag is Korisnik l)
            {
                user.Text = l.Korisnicko_ime;   
                PorukePanel.Children.Clear();
                primalac = l;
            }
            List<Poruka> poruke = new List<Poruka>();
            if(primalac != null)
                 poruke = await MainGuiKontroler.Instance.ucitajSvePoruke(primalac,k);
            foreach(Poruka x in poruke)
            {
                OnPorukaPrimljena(x);
            }    

        }
        

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UcitajPrijatelje();
        }

        private async void Razgovori_Click(object sender, RoutedEventArgs e)
        {
            Kontakti.Visibility = Visibility.Visible;
            Prijatelji.Visibility = Visibility.Collapsed;
            UcitajPrijatelje();
        }
        private ListBoxItem DinamickiItem(string v, Prijateljstvo p)
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var txt = new TextBlock
            {
                Text = v,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 13,
                TextTrimming = TextTrimming.CharacterEllipsis,
                Margin = new Thickness(8, 0, 8, 0)
            };
            Grid.SetColumn(txt, 0);

            var btnAccept = new Button
            {
                Content = "✓",
                ToolTip = "Prihvati",
                Tag = p,
                Style = (Style)FindResource("CircleActionButton"),
                Margin = new Thickness(3, 0, 0, 0),
                Foreground = Brushes.Green
            };
            btnAccept.Click += Accept_Click;
            Grid.SetColumn(btnAccept, 1);

            var btnDecline = new Button
            {
                Content = "×",
                ToolTip = "Odbij",
                Tag = p,
                Style = (Style)FindResource("CircleActionButton"),
                Margin = new Thickness(3, 0, 0, 0),
                Foreground = Brushes.Red


            };
           btnDecline.Click += Decline_Click;
            Grid.SetColumn(btnDecline, 2);

            grid.Children.Add(txt);
            grid.Children.Add(btnAccept);
            grid.Children.Add(btnDecline);

            return new ListBoxItem
            {
                Content = grid,
                Tag = p,
                Padding = new Thickness(6),
                Margin = new Thickness(2),
                MinHeight = 40
            };
        }

        private async void Decline_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var prijatelj = (Prijateljstvo)btn.Tag;
            bool uspesno = await MainGuiKontroler.Instance.OdbijPrijatelja(prijatelj);
            if (uspesno)
                ProveriNovePrijatelje();
        }

        private async void Accept_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var prijatelj = (Prijateljstvo)btn.Tag;
            bool uspesno = await MainGuiKontroler.Instance.PrihvatiPrijatelja(prijatelj);
            if (uspesno)
                ProveriNovePrijatelje();

        }

        async void ProveriNovePrijatelje()
        {
            Odgovor o = await MainGuiKontroler.Instance.ProveriNovePrijatelje(k.Id);
            List<Prijateljstvo> naCekanju = (List<Prijateljstvo>)o.Rezultat;
            Prijatelji.Items.Clear();
            if (naCekanju.Count == 0)
                return;
            foreach(Prijateljstvo p in naCekanju)
            {
                string v = await MainGuiKontroler.Instance.Pretrazi(p.korisnik1_id);
                Prijatelji.Items.Add(DinamickiItem(v, p));
            }
        }
        private async void PrijateljiBtn_Click(object sender, RoutedEventArgs e)
        {
            Kontakti.Visibility = Visibility.Collapsed;
            Prijatelji.Visibility = Visibility.Visible;
            ProveriNovePrijatelje();

        }
        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }


    }
}
