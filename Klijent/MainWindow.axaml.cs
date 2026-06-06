using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Klijent.Domen;
using Klijent.Kontroleri_GUI_;
using Zajednicki;
using Zajednicki.Domen;

namespace Klijent;

public partial class MainWindow : Window
{
    private readonly Korisnik? _korisnik;
    private Korisnik? _primalac;

    public MainWindow()
    {
        InitializeComponent();
        Opened += Window_Opened;
    }

    public MainWindow(Korisnik korisnik) : this()
    {
        _korisnik = korisnik;
        DataContext = MainGuiKontroler.Instance;

        Komunikacija.Instance.PushPrimljen += OnPushPrimljen;

        Closed += (_, _) =>
        {
            Komunikacija.Instance.PushPrimljen -= OnPushPrimljen;
        };

        korisnikText.Text = $"korisnik: {korisnik.Korisnicko_ime}";
    }

    private void OnPushPrimljen(Zahtev zahtev)
    {
        switch (zahtev.Operacija)
        {
            case Operacija.Posalji:
                if (zahtev.Objekat is Poruka poruka)
                {
                    OnPorukaPrimljena(poruka);
                }

                break;

            case Operacija.DodajPrijatelja:
                if (zahtev.Objekat is PrijateljstvoView zahtevZaPrijateljstvo)
                {
                    OnDodajPrijatelja(zahtevZaPrijateljstvo);
                }

                break;

            case Operacija.PrihvatiPrijatelja:
                if (zahtev.Objekat is Korisnik prihvaceniKorisnik)
                {
                    OnPrijateljaPrihvati(prihvaceniKorisnik);
                }

                break;

            case Operacija.ObrisiPrijateljstvo:
                if (zahtev.Objekat is Korisnik obrisaniKorisnik)
                {
                    OnPrijateljaObrisi(obrisaniKorisnik);
                }

                break;

            case Operacija.OdbijPrijatelja:
                if (zahtev.Objekat is PrijateljstvoView odbijenoPrijateljstvo)
                {
                    OnPrijateljaOdbij(odbijenoPrijateljstvo);
                }

                break;
        }
    }

    private void OnPorukaPrimljena(Poruka poruka)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_korisnik is null)
                return;

            int drugiId;

            if (poruka.posiljalac_id == _korisnik.Id)
            {
                drugiId = poruka.primalac_id;
            }
            else
            {
                drugiId = poruka.posiljalac_id;
            }

            if (Kontakti.SelectedItem is Korisnik izabrani && izabrani.Id == drugiId)
                DodajPoruku(poruka);
        });
    }

    private void OnDodajPrijatelja(PrijateljstvoView prijateljstvo)
    {
        Dispatcher.UIThread.Post(() => MainGuiKontroler.Instance.Prijateljstva.Add(prijateljstvo));
    }

    private void OnPrijateljaPrihvati(Korisnik korisnik)
    {
        Dispatcher.UIThread.Post(() => MainGuiKontroler.Instance.Kontakti.Add(korisnik));
    }

    private void OnPrijateljaObrisi(Korisnik korisnik)
    {
        Dispatcher.UIThread.Post(() =>
        {
            RemoveKontaktById(korisnik.Id);

            if (_primalac?.Id == korisnik.Id)
            {
                _primalac = null;
                user.Text = string.Empty;
                PorukePanel.Children.Clear();
            }
        });
    }

    private void OnPrijateljaOdbij(PrijateljstvoView prijateljstvo)
    {
        Dispatcher.UIThread.Post(() => RemovePrijateljstvo(prijateljstvo));
    }

    private void DodajPoruku(Poruka poruka)
    {
        if (_korisnik is null)
            return;

        bool incoming = poruka.posiljalac_id != _korisnik.Id;
        string backgroundColor = "#2b2d2e";
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;

        if (incoming)
        {
            backgroundColor = "#202223";
            horizontalAlignment = HorizontalAlignment.Left;
        }

        var bubble = new Border
        {
            Background = new SolidColorBrush(Color.Parse(backgroundColor)),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(10, 6),
            Margin = new Thickness(4, 2),
            HorizontalAlignment = horizontalAlignment,
            MaxWidth = 420
        };

        bubble.Child = new TextBlock
        {
            Text = poruka.poruka_text,
            Foreground = Brushes.White,
            TextWrapping = TextWrapping.Wrap
        };

        PorukePanel.Children.Add(bubble);
        Dispatcher.UIThread.Post(() => ChatScroll.ScrollToEnd(), DispatcherPriority.Background);
    }

    private async void Send_Click(object? sender, RoutedEventArgs e)
    {
        if (_korisnik is null)
            return;

        string porukaText = messageText.Text ?? string.Empty;
        if (_primalac is null || string.IsNullOrWhiteSpace(porukaText))
            return;

        bool poslato = await MainGuiKontroler.Instance.Posalji(porukaText, _korisnik.Id, _primalac.Id);
        if (!poslato)
            return;

        var poruka = new Poruka(_primalac.Id, _korisnik.Id, porukaText);
        DodajPoruku(poruka);
        messageText.Text = string.Empty;
    }

    private async void SearchButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_korisnik is null)
            return;

        string searchText = SearchTextBox.Text ?? string.Empty;
        if (searchText == _korisnik.Korisnicko_ime)
            return;

        var korisnik = await MainGuiKontroler.Instance.Pretrazi(searchText);
        if (korisnik is null || korisnik.Korisnicko_ime == "greska")
            return;

        bool uspeh = await MainGuiKontroler.Instance.DodajPrijatelja(_korisnik.Id, korisnik.Id);
        if (!uspeh)
            return;

        await DialogService.ShowMessageAsync($"Uspešno poslat zahtev za prijateljstvo korisniku {searchText}.");
        SearchTextBox.Text = string.Empty;
    }

    private async Task UcitajPrijatelje()
    {
        if (_korisnik is null)
            return;

        try
        {
            await MainGuiKontroler.Instance.VratiSvePrijatelje(_korisnik);
        }
        catch (Exception ex)
        {
            await DialogService.ShowMessageAsync(ex.Message);
        }
    }

    private async Task UcitajZahtevePrijateljstva()
    {
        if (_korisnik is null)
            return;
        try
        {
            await MainGuiKontroler.Instance.VratiZahtevePrijatelja(_korisnik.Id);
        }
        catch (Exception ex)
        {
            await DialogService.ShowMessageAsync(ex.Message);
        }
    }

    private async void Kontakti_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_korisnik is null || Kontakti.SelectedItem is not Korisnik kontakt)
            return;

        user.Text = kontakt.Korisnicko_ime;
        PorukePanel.Children.Clear();
        _primalac = kontakt;

        var poruke = await MainGuiKontroler.Instance.UcitajSvePoruke(_primalac, _korisnik);
        foreach (var poruka in poruke)
            DodajPoruku(poruka);
    }

    private async void Delete_Click(object? sender, RoutedEventArgs e)
    {
        if (_korisnik is null || sender is not Button button || button.Tag is not Korisnik korisnik)
            return;

        bool ok = await MainGuiKontroler.Instance.ObrisiPrijateljstvo(_korisnik.Id, korisnik.Id);
        if (!ok)
            return;

        RemoveKontaktById(korisnik.Id);

        if (_primalac?.Id == korisnik.Id)
        {
            _primalac = null;
            user.Text = string.Empty;
            PorukePanel.Children.Clear();
        }
    }

    private void Razgovori_Click(object? sender, RoutedEventArgs e)
    {
        Kontakti.IsVisible = true;
        Prijatelji.IsVisible = false;
    }

    private void MessageText_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
            return;

        e.Handled = true;
        Send_Click(Send, new RoutedEventArgs());
    }

    private async void Decline_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not PrijateljstvoView prijateljstvo)
            return;

        bool uspesno = await MainGuiKontroler.Instance.OdbijPrijatelja(prijateljstvo.Link);
        if (uspesno)
            RemovePrijateljstvo(prijateljstvo);
    }

    private async void Accept_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not PrijateljstvoView prijateljstvo)
            return;

        bool uspesno = await MainGuiKontroler.Instance.PrihvatiPrijatelja(prijateljstvo.Link);
        if (!uspesno)
            return;

        RemovePrijateljstvo(prijateljstvo);
    }

    private void PrijateljiBtn_Click(object? sender, RoutedEventArgs e)
    {
        Kontakti.IsVisible = false;
        Prijatelji.IsVisible = true;
    }

    private async void Window_Opened(object? sender, EventArgs e)
    {
        await UcitajPrijatelje();
        await UcitajZahtevePrijateljstva();
    }

    private void RemoveKontaktById(int id)
    {
        var kontakt = MainGuiKontroler.Instance.Kontakti.FirstOrDefault(x => x.Id == id);
        if (kontakt is not null)
            MainGuiKontroler.Instance.Kontakti.Remove(kontakt);
    }

    private void RemovePrijateljstvo(PrijateljstvoView prijateljstvo)
    {
        var stavka = MainGuiKontroler.Instance.Prijateljstva.FirstOrDefault(x =>
            x.Link.korisnik1_id == prijateljstvo.Link.korisnik1_id &&
            x.Link.korisnik2_id == prijateljstvo.Link.korisnik2_id);

        if (stavka is not null)
            MainGuiKontroler.Instance.Prijateljstva.Remove(stavka);
    }
}
