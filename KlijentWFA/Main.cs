using Klijent;
using Klijent.Domen;
using Klijent.Kontroleri_GUI_;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zajednicki;
using Zajednicki.Domen;

namespace KlijentWFA
{
    public partial class Main : Form
    {
        private Korisnik trenutni;

        public Main(Korisnik l)
        {
            InitializeComponent();

            lblAppTitle.Text += " | " + l.Korisnicko_ime;
            trenutni = l;

            dgv_prijatelji.AutoGenerateColumns = false;
            dgv_prijatelji.DataSource = prijateljiView;

            dgv_razgovori.AutoGenerateColumns = false;
            dgv_razgovori.DataSource = razgovoriView;

            dgv_razgovori.Visible = true;
            dgv_prijatelji.Visible = false;

            update_thr.Start();
        }

        public sealed class PrijateljstvoView
        {
            public int Drugi { get; set; }
            public string Korisnicko_ime { get; set; }
            public Prijateljstvo Link { get; set; }
        }

        private readonly BindingList<PrijateljstvoView> prijateljiView = new BindingList<PrijateljstvoView>();
        private readonly BindingList<Korisnik> razgovoriView = new BindingList<Korisnik>();
        private readonly Dictionary<int, string> usernameCache = new Dictionary<int, string>();

        private void btnRazgovori_Click(object sender, EventArgs e)
        {
            dgv_razgovori.Visible = true;
            dgv_prijatelji.Visible = false;
        }

        private void btnPrijatelji_Click(object sender, EventArgs e)
        {
            dgv_razgovori.Visible = false;
            dgv_prijatelji.Visible = true;
        }

        private async void search_btn_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == trenutni.Korisnicko_ime) return;

            Korisnik coveculjak = await MainGuiKontroler.Instance.Pretrazi(txtSearch.Text);
            if (coveculjak == null) return;
            if (coveculjak.Korisnicko_ime == "greska") return;

            bool uspeh = await MainGuiKontroler.Instance.DodajPrijatelja(trenutni.Id, coveculjak.Id);
            if (!uspeh) return;

            MessageBox.Show("Uspešno poslat zahtev za prijateljstvo korisniku " + txtSearch.Text);
            txtSearch.Clear();
        }

        private bool refreshInProgress;

        private async void update_thr_Tick(object sender, EventArgs e)
        {
            if (refreshInProgress) return;

            refreshInProgress = true;
            try
            {
                await UcitajPodatke();
            }
            finally
            {
                refreshInProgress = false;
            }
        }

        private async Task UcitajPodatke()
        {
            try
            {
                await MainGuiKontroler.Instance.vratiSvePrijatelje(trenutni);
                await MainGuiKontroler.Instance.ProveriNovePrijatelje(trenutni.Id);

                razgovoriView.Clear();
                var kontakti = MainGuiKontroler.Instance.kontakti;
                if (kontakti != null)
                {
                    foreach (Korisnik k in kontakti)
                    {
                        if (k == null) continue;
                        razgovoriView.Add(k);
                    }
                }

                prijateljiView.Clear();
                var zahtevi = MainGuiKontroler.Instance.prijatelji;
                if (zahtevi == null) return;

                foreach (Prijateljstvo p in zahtevi)
                {
                    if (p == null) continue;

                    int drugi;
                    if (p.korisnik1_id == trenutni.Id) drugi = p.korisnik2_id;
                    else drugi = p.korisnik1_id;

                    string k_ime;
                    if (usernameCache.ContainsKey(drugi))
                    {
                        k_ime = usernameCache[drugi];
                    }
                    else
                    {
                        k_ime = await MainGuiKontroler.Instance.Pretrazi(drugi);
                        if (k_ime == null) k_ime = "";
                        usernameCache[drugi] = k_ime;
                    }

                    PrijateljstvoView view = new PrijateljstvoView();
                    view.Drugi = drugi;
                    view.Korisnicko_ime = k_ime;
                    view.Link = p;

                    prijateljiView.Add(view);
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
            }
        }

        private async void dgv_prijatelji_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex < 0) return;

            DataGridViewRow row = dgv_prijatelji.Rows[e.RowIndex];
            PrijateljstvoView view = row.DataBoundItem as PrijateljstvoView;
            if (view == null) return;

            string colName = dgv_prijatelji.Columns[e.ColumnIndex].Name;
            if (colName != "colAccept" && colName != "colDecline") return;

            update_thr.Stop();
            try
            {
                if (colName == "colAccept")
                {
                    Odgovor o = await Komunikacija.Instance.PrihvatiPrijatelja(view.Link);
                    if (o != null && o.Uspesno) prijateljiView.Remove(view);
                }

                if (colName == "colDecline")
                {
                    Odgovor o = await Komunikacija.Instance.OdbijPrijatelja(view.Link);
                    if (o != null && o.Uspesno) prijateljiView.Remove(view);
                }
            }
            finally
            {
                update_thr.Start();
            }
        }

        private async void dgv_razgovori_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex < 0) return;

            string colName = dgv_razgovori.Columns[e.ColumnIndex].Name;
            if (colName != "colRazgovorOdbij") return;

            DataGridViewRow row = dgv_razgovori.Rows[e.RowIndex];
            Korisnik k = row.DataBoundItem as Korisnik;
            if (k == null) return;

            update_thr.Stop();
            try
            {
                Prijateljstvo p = new Prijateljstvo();
                p.korisnik1_id = trenutni.Id;
                p.korisnik2_id = k.Id;

                Odgovor o = await Komunikacija.Instance.ObrisiPrijatelja(p);
                if (o != null && o.Uspesno) razgovoriView.Remove(k);
            }
            finally
            {
                update_thr.Start();
            }
        }
    }
}
