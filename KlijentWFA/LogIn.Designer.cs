namespace KlijentWFA
{
    partial class LogIn
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlShadow;
        private System.Windows.Forms.Panel pnlCard;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnRegister;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlShadow = new Panel();
            pnlCard = new Panel();
            btnRegister = new Button();
            btnLogin = new Button();
            txtPassword = new TextBox();
            txtUsername = new TextBox();
            lblPassword = new Label();
            lblUsername = new Label();
            lblTitle = new Label();
            pnlCard.SuspendLayout();
            SuspendLayout();
            // 
            // pnlShadow
            // 
            pnlShadow.BackColor = Color.FromArgb(12, 12, 13);
            pnlShadow.Location = new Point(76, 55);
            pnlShadow.Name = "pnlShadow";
            pnlShadow.Size = new Size(456, 220);
            pnlShadow.TabIndex = 0;
            // 
            // pnlCard
            // 
            pnlCard.BackColor = Color.FromArgb(20, 20, 21);
            pnlCard.Controls.Add(btnRegister);
            pnlCard.Controls.Add(btnLogin);
            pnlCard.Controls.Add(txtPassword);
            pnlCard.Controls.Add(txtUsername);
            pnlCard.Controls.Add(lblPassword);
            pnlCard.Controls.Add(lblUsername);
            pnlCard.Controls.Add(lblTitle);
            pnlCard.Location = new Point(70, 48);
            pnlCard.Name = "pnlCard";
            pnlCard.Size = new Size(468, 232);
            pnlCard.TabIndex = 1;
            // 
            // btnRegister
            // 
            btnRegister.BackColor = Color.FromArgb(52, 52, 54);
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.FlatAppearance.MouseDownBackColor = Color.FromArgb(44, 44, 46);
            btnRegister.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 64);
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnRegister.ForeColor = Color.White;
            btnRegister.Location = new Point(268, 168);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(140, 38);
            btnRegister.TabIndex = 3;
            btnRegister.Text = "registruj se";
            btnRegister.UseVisualStyleBackColor = false;
            btnRegister.Click += btnRegister_Click;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.FromArgb(52, 52, 54);
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseDownBackColor = Color.FromArgb(44, 44, 46);
            btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 64);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(112, 168);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(140, 38);
            btnLogin.TabIndex = 2;
            btnLogin.Text = "uloguj se";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // txtPassword
            // 
            txtPassword.BackColor = Color.FromArgb(14, 14, 15);
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.Font = new Font("Segoe UI", 10F);
            txtPassword.ForeColor = Color.White;
            txtPassword.Location = new Point(200, 118);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(230, 25);
            txtPassword.TabIndex = 1;
            txtPassword.Text = "domagoj";
            txtPassword.UseSystemPasswordChar = true;
            // 
            // txtUsername
            // 
            txtUsername.BackColor = Color.FromArgb(14, 14, 15);
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtUsername.Font = new Font("Segoe UI", 10F);
            txtUsername.ForeColor = Color.White;
            txtUsername.Location = new Point(200, 78);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(230, 25);
            txtUsername.TabIndex = 0;
            txtUsername.Text = "domagoj";
            // 
            // lblPassword
            // 
            lblPassword.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPassword.ForeColor = Color.White;
            lblPassword.Location = new Point(60, 118);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(130, 24);
            lblPassword.TabIndex = 3;
            lblPassword.Text = "lozinka:";
            lblPassword.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblUsername
            // 
            lblUsername.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUsername.ForeColor = Color.White;
            lblUsername.Location = new Point(60, 78);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(130, 24);
            lblUsername.TabIndex = 1;
            lblUsername.Text = "korisničko ime:";
            lblUsername.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(0, 18);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(468, 40);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Login";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LogIn
            // 
            AcceptButton = btnLogin;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(10, 10, 11);
            ClientSize = new Size(608, 325);
            Controls.Add(pnlCard);
            Controls.Add(pnlShadow);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Name = "LogIn";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LogIn";
            pnlCard.ResumeLayout(false);
            pnlCard.PerformLayout();
            ResumeLayout(false);
        }
    }
}
