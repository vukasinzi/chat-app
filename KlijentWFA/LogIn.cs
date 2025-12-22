using Klijent.Kontroleri_GUI_;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KlijentWFA
{
    public partial class LogIn : Form
    {
        public LogIn()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            this.Hide();
            bool successful = await LoginGuiKontroler.Instance.LogIn(txtUsername.Text, txtPassword.Text);
            if (successful)
                Close();
            else
                this.Show();
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            this.Hide();
            bool successful = await LoginGuiKontroler.Instance.RegistrujSe(txtUsername.Text, txtPassword.Text);
            if (successful)
                Close();
            else
                this.Show();
        }
    }
}
