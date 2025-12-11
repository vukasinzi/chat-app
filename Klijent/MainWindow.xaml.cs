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

namespace Klijent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Korisnik k;
        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(Korisnik k)
        {
            InitializeComponent();
            this.k = k;
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
