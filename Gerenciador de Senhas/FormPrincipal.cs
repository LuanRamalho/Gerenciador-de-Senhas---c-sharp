using System;
using System.Drawing;
using System.Windows.Forms;

namespace Gerenciador_de_Senhas;

public partial class FormPrincipal : Form {
    private byte[] chaveMestra;
    private TextBox txtSite, txtNome, txtUsuario, txtSenha;

    public FormPrincipal() {
        chaveMestra = CryptoHelper.ObterOuCriarChave();
        DatabaseHelper.CriarTabela();
        ConfigurarInterface();
    }

    private void ConfigurarInterface() {
        this.Text = "Gerenciador de Senhas Pro";
        this.Size = new Size(450, 550);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(224, 242, 247);
        this.Font = new Font("Segoe UI", 10);

        int campoWidth = 380;
        int margemEsq = 25;

        // --- Estilização dos Campos ---
        CriarLabel("Site:", margemEsq, 20);
        txtSite = CriarTextBox(margemEsq, 45, campoWidth);

        CriarLabel("Nome da conta:", margemEsq, 85);
        txtNome = CriarTextBox(margemEsq, 110, campoWidth);

        CriarLabel("Usuário:", margemEsq, 150);
        txtUsuario = CriarTextBox(margemEsq, 175, campoWidth);

        CriarLabel("Senha:", margemEsq, 215);
        txtSenha = CriarTextBox(margemEsq, 240, campoWidth);
        txtSenha.PasswordChar = '*';

        // --- Botão Salvar ---
        Button btnSalvar = new Button {
            Text = "💾 SALVAR SENHA",
            Location = new Point(margemEsq, 320),
            Width = campoWidth,
            Height = 50,
            BackColor = Color.FromArgb(0, 105, 86),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnSalvar.Click += SalvarSenha_Click;

        // --- Botão Visualizar ---
        Button btnVer = new Button {
            Text = "🔍 VISUALIZAR SENHAS",
            Location = new Point(margemEsq, 385),
            Width = campoWidth,
            Height = 50,
            BackColor = Color.FromArgb(58, 124, 0),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        btnVer.Click += (s, e) => {
            FormVisualizar telaDados = new FormVisualizar(chaveMestra);
            telaDados.ShowDialog(); // Abre como uma janela sobreposta
        };

        this.Controls.Add(btnSalvar);
        this.Controls.Add(btnVer);
    }

    private void SalvarSenha_Click(object sender, EventArgs e) {
        if (string.IsNullOrWhiteSpace(txtSite.Text) || string.IsNullOrWhiteSpace(txtSenha.Text)) {
            MessageBox.Show("Por favor, preencha pelo menos o Site e a Senha.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try {
            byte[] senhaCripto = CryptoHelper.Criptografar(txtSenha.Text, chaveMestra);
            DatabaseHelper.SalvarSenha(txtSite.Text, txtNome.Text, txtUsuario.Text, senhaCripto);
            
            MessageBox.Show("Senha armazenada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            // Limpar campos
            txtSite.Clear(); txtNome.Clear(); txtUsuario.Clear(); txtSenha.Clear();
        } catch (Exception ex) {
            MessageBox.Show("Erro ao salvar: " + ex.Message);
        }
    }

    // Funções auxiliares para evitar repetição de código de layout
    private void CriarLabel(string texto, int x, int y) {
        Label lbl = new Label { Text = texto, Location = new Point(x, y), ForeColor = Color.FromArgb(26, 35, 126), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
        this.Controls.Add(lbl);
    }

    private TextBox CriarTextBox(int x, int y, int w) {
        TextBox txt = new TextBox { Location = new Point(x, y), Width = w, BorderStyle = BorderStyle.FixedSingle };
        this.Controls.Add(txt);
        return txt;
    }
}