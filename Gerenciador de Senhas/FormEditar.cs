using System;
using System.Drawing;
using System.Windows.Forms;

namespace Gerenciador_de_Senhas;

public class FormEditar : Form {
    private byte[] chaveMestra;
    private string siteOriginal;
    private string usuarioOriginal;
    private TextBox txtSite, txtNome, txtUsuario, txtSenha;
    public bool DadosSalvos { get; private set; } = false;

    public FormEditar(string site, string nome, string usuario, string senha, byte[] chave) {
        this.siteOriginal = site;
        this.usuarioOriginal = usuario;
        this.chaveMestra = chave;
        
        this.Text = "Editar Registro (NoSQL)";
        this.Size = new Size(400, 450);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        ConfigurarInterface(site, nome, usuario, senha);
    }

    private void ConfigurarInterface(string site, string nome, string usuario, string senha) {
        int y = 20;
        
        txtSite = CriarCampo("Site:", site, ref y);
        txtNome = CriarCampo("Nome da Conta:", nome, ref y);
        txtUsuario = CriarCampo("Usuário:", usuario, ref y);
        txtSenha = CriarCampo("Senha:", senha, ref y);

        Button btnSalvar = new Button {
            Text = "ATUALIZAR DADOS",
            Location = new Point(20, y + 20),
            Size = new Size(345, 45),
            BackColor = Color.DodgerBlue,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnSalvar.Click += Salvar_Click;

        this.Controls.Add(btnSalvar);
    }

    private TextBox CriarCampo(string label, string valor, ref int y) {
        Label lbl = new Label { Text = label, Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        TextBox txt = new TextBox { Text = valor, Location = new Point(20, y + 22), Width = 345, Font = new Font("Segoe UI", 10) };
        this.Controls.Add(lbl);
        this.Controls.Add(txt);
        y += 65;
        return txt;
    }

    private void Salvar_Click(object sender, EventArgs e) {
        try {
            if (string.IsNullOrWhiteSpace(txtSite.Text) || string.IsNullOrWhiteSpace(txtSenha.Text)) {
                MessageBox.Show("Site e Senha são obrigatórios.");
                return;
            }

            byte[] senhaCripto = CryptoHelper.Criptografar(txtSenha.Text, chaveMestra);
            
            // Passa as referências originais e os novos dados para o Helper JSON
            DatabaseHelper.AtualizarSenha(
                siteOriginal, 
                usuarioOriginal, 
                txtSite.Text, 
                txtNome.Text, 
                txtUsuario.Text, 
                senhaCripto
            );

            this.DadosSalvos = true;
            MessageBox.Show("Dados atualizados com sucesso no banco JSON!", "Sucesso");
            this.Close();
        } catch (Exception ex) {
            MessageBox.Show("Erro ao atualizar: " + ex.Message);
        }
    }
}
