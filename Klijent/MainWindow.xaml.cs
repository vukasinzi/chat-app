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
using Zajednicki.Domen;

namespace Klijent
{
    
    public partial class MainWindow : Window
    {
        private Korisnik k;
        private readonly Dictionary<int, List<Poruka>> chats = new();
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

        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == k.Korisnicko_ime)
                return;
            Korisnik coveculjak = new Korisnik();
            coveculjak = await MainGuiKontroler.Instance.Pretrazi(SearchTextBox.Text);
            if (coveculjak.Korisnicko_ime == "greska")
                return;
            foreach(ListBoxItem item in Kontakti.Items)
            {
                if (item.Content == coveculjak.Korisnicko_ime)
                    return;
            }
            ListBoxItem i = new ListBoxItem
            {
                Content = coveculjak.Korisnicko_ime,
                Tag = coveculjak
            };
            Kontakti.Items.Add(i);
            
            
        }

        private void Kontakti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem i = (ListBoxItem)Kontakti.SelectedItem;

            user.Text = i.Content.ToString();
            PorukePanel.Children.Clear();
        }
        

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await MainGuiKontroler.Instance.vratiSvePrijatelje(k);
            foreach(Korisnik k in MainGuiKontroler.Instance.prijatelji)
            {
                ListBoxItem i = new ListBoxItem
                {
                    Content = k.Korisnicko_ime,
                    Tag = k
                };
                Kontakti.Items.Add(i);
            }
         

        }
    }
}
