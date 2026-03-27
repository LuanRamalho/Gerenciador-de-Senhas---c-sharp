using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Gerenciador_de_Senhas;

public class FormVisualizar : Form {
    private byte[] chaveMestra;
    private DataGridView grid;
    private TextBox txtBusca;

    public FormVisualizar(byte[] chave) {
        this.chaveMestra = chave;
        ConfigurarInterface();
        CarregarDados("");
    }

    private void ConfigurarInterface() {
        this.Text = "Gerenciador de Dados";
        this.Size = new Size(850, 550);
        this.StartPosition = FormStartPosition.CenterParent;

        Panel painelBusca = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(240, 240, 240) };
        Label lblBusca = new Label { Text = "Buscar (Site ou Conta):", Location = new Point(15, 20), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        txtBusca = new TextBox { Location = new Point(160, 18), Width = 300 };
        txtBusca.TextChanged += (s, e) => CarregarDados(txtBusca.Text);

        painelBusca.Controls.Add(lblBusca);
        painelBusca.Controls.Add(txtBusca);

        grid = new DataGridView {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false,
            BackgroundColor = Color.White
        };

        grid.Columns.Add("Site", "Site / URL");
        grid.Columns.Add("Nome", "Nome da Conta");
        grid.Columns.Add("Usuario", "Usuário");
        grid.Columns.Add("Senha", "Senha (Criptografada)");

        Panel painelAcoes = new Panel { Dock = DockStyle.Right, Width = 150, BackColor = Color.FromArgb(230, 230, 230) };
        
        // Botão Ver Senha
        Button btnVer = CriarBotaoAcao("Ver Senha", Color.DarkCyan, 20);
        btnVer.Click += VerSenha_Click;

        // --- O BOTÃO QUE ESTAVA FALTANDO ---
        Button btnGerar = CriarBotaoAcao("Gerar Senha", Color.MediumPurple, 80);
        btnGerar.Click += (s, e) => {
            using (var frmGerador = new FormGerador()) {
                frmGerador.ShowDialog();
            }
        };

        // Botão Editar (Ajustado o Y para 140)
        Button btnEditar = CriarBotaoAcao("Editar", Color.DodgerBlue, 140);
        btnEditar.Click += Editar_Click;

        // Botão Excluir (Ajustado o Y para 200)
        Button btnExcluir = CriarBotaoAcao("Excluir", Color.Crimson, 200);
        btnExcluir.Click += Excluir_Click;

        painelAcoes.Controls.AddRange(new Control[] { btnVer, btnGerar, btnEditar, btnExcluir });

        this.Controls.Add(grid);
        this.Controls.Add(painelAcoes);
        this.Controls.Add(painelBusca);
    }

    private Button CriarBotaoAcao(string texto, Color cor, int y) {
        return new Button {
            Text = texto,
            Location = new Point(15, y),
            Size = new Size(120, 45),
            BackColor = cor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };
    }

    private void CarregarDados(string termo) {
        grid.Rows.Clear();
        var lista = DatabaseHelper.BuscarSenhas(termo);
        foreach (var item in lista) {
            int n = grid.Rows.Add(item.Site, item.Nome, item.Usuario, "********");
            grid.Rows[n].Tag = Convert.ToBase64String(item.SenhaCriptografada);
        }
    }

    private void VerSenha_Click(object sender, EventArgs e) {
        if (grid.SelectedRows.Count > 0) {
            var linha = grid.SelectedRows[0];
            byte[] dadosCripto = Convert.FromBase64String(linha.Tag.ToString());
            string senhaReal = CryptoHelper.Descriptografar(dadosCripto, chaveMestra);

            var celulaSenha = linha.Cells[3];
            if (celulaSenha.Value.ToString() == "********") {
                celulaSenha.Value = senhaReal;
            } else {
                celulaSenha.Value = "********";
            }
        }
    }

    private void Excluir_Click(object sender, EventArgs e) {
        if (grid.SelectedRows.Count > 0) {
            var linha = grid.SelectedRows[0];
            string site = linha.Cells[0].Value.ToString();
            string usuario = linha.Cells[2].Value.ToString();

            var resp = MessageBox.Show($"Deseja excluir a conta '{usuario}' de '{site}'?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (resp == DialogResult.Yes) {
                DatabaseHelper.ExcluirSenha(site, usuario);
                CarregarDados(txtBusca.Text);
            }
        }
    }

    private void Editar_Click(object sender, EventArgs e) {
        if (grid.SelectedRows.Count > 0) {
            var linha = grid.SelectedRows[0];
            string site = linha.Cells[0].Value.ToString();
            string nome = linha.Cells[1].Value.ToString();
            string usuario = linha.Cells[2].Value.ToString();
            
            byte[] dadosCripto = Convert.FromBase64String(linha.Tag.ToString());
            string senha = CryptoHelper.Descriptografar(dadosCripto, chaveMestra);

            using (var frmEditar = new FormEditar(site, nome, usuario, senha, chaveMestra)) {
                frmEditar.ShowDialog();
                if (frmEditar.DadosSalvos) CarregarDados(txtBusca.Text);
            }
        }
    }
}
