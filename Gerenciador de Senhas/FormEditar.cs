using System;
using System.Drawing;
using System.Windows.Forms;

namespace Gerenciador_de_Senhas;

public class FormEditar : Form {
    private byte[] chaveMestra;
    private int idRegistro;
    private TextBox txtSite, txtNome, txtUsuario, txtSenha;
    public bool DadosSalvos { get; private set; } = false;

    public FormEditar(int id, string site, string nome, string usuario, string senha, byte[] chave) {
        this.idRegistro = id;
        this.chaveMestra = chave;
        
        this.Text = "Editar Registro";
        this.Size = new Size(400, 450);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;

        ConfigurarInterface(site, nome, usuario, senha);
    }

    private void ConfigurarInterface(string site, string nome, string usuario, string senha) {
        int y = 20;
        
        txtSite = CriarCampo("Site:", site, ref y);
        txtNome = CriarCampo("Nome da Conta:", nome, ref y);
        txtUsuario = CriarCampo("Usuário:", usuario, ref y);
        txtSenha = CriarCampo("Senha:", senha, ref y);

        Button btnSalvar = new Button {
            Text = "ATUALIZAR",
            Location = new Point(20, y + 20),
            Size = new Size(345, 40),
            BackColor = Color.DodgerBlue,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnSalvar.Click += Salvar_Click;

        this.Controls.Add(btnSalvar);
    }

    private TextBox CriarCampo(string label, string valor, ref int y) {
        Label lbl = new Label { Text = label, Location = new Point(20, y), AutoSize = true };
        TextBox txt = new TextBox { Text = valor, Location = new Point(20, y + 20), Width = 345 };
        this.Controls.Add(lbl);
        this.Controls.Add(txt);
        y += 60;
        return txt;
    }

    private void Salvar_Click(object sender, EventArgs e) {
        try {
            byte[] senhaCripto = CryptoHelper.Criptografar(txtSenha.Text, chaveMestra);
            DatabaseHelper.AtualizarSenha(idRegistro, txtSite.Text, txtNome.Text, txtUsuario.Text, senhaCripto);
            this.DadosSalvos = true;
            this.Close();
        } catch (Exception ex) {
            MessageBox.Show("Erro ao atualizar: " + ex.Message);
        }
    }
}