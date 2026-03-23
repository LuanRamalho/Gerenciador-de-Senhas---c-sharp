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
        this.Text = "Gerenciador de Dados - CRUD";
        this.Size = new Size(900, 550);
        this.StartPosition = FormStartPosition.CenterParent;

        // --- Painel Superior (Busca) ---
        Panel painelBusca = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(240, 240, 240) };
        Label lblBusca = new Label { Text = "Buscar (Site ou Conta):", Location = new Point(15, 20), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        txtBusca = new TextBox { Location = new Point(160, 18), Width = 300 };
        txtBusca.TextChanged += (s, e) => CarregarDados(txtBusca.Text);

        painelBusca.Controls.Add(lblBusca);
        painelBusca.Controls.Add(txtBusca);

        // --- Grid de Dados ---
        grid = new DataGridView {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AllowUserToAddRows = false,
            ReadOnly = true,
            RowHeadersVisible = false,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None
        };
        
        grid.Columns.Add("id", "ID");
        grid.Columns.Add("site", "Site");
        grid.Columns.Add("nome", "Conta");
        grid.Columns.Add("usuario", "Usuário");
        grid.Columns.Add("senha", "Senha");
        grid.Columns[0].Visible = false; // Esconde o ID do banco de dados

        // --- Painel Inferior (Botões de Ação) ---
        Panel painelBotoes = new Panel { Dock = DockStyle.Bottom, Height = 70, BackColor = Color.WhiteSmoke };
        
        Button btnExcluir = CriarBotao("🗑️ Excluir", 15, Color.Salmon);
        Button btnEditar = CriarBotao("✏️ Editar", 125, Color.LightBlue);
        Button btnGerar = CriarBotao("🔑 Gerar Senha", 235, Color.MediumPurple);
        Button btnVerSenha = CriarBotao("👁️ Mostrar/Ocultar", 365, Color.LightGray);
        btnVerSenha.Width = 140;

        // Eventos dos Botões
        btnExcluir.Click += Excluir_Click;
        btnEditar.Click += Editar_Click;
        btnGerar.Click += (s, e) => new FormGerador().ShowDialog();
        btnVerSenha.Click += AlternarVisibilidadeSenha_Click;

        painelBotoes.Controls.AddRange(new Control[] { btnExcluir, btnEditar, btnGerar, btnVerSenha });

        this.Controls.Add(grid);
        this.Controls.Add(painelBusca);
        this.Controls.Add(painelBotoes);
    }

    private Button CriarBotao(string texto, int x, Color cor) {
        return new Button {
            Text = texto,
            Location = new Point(x, 15),
            Size = new Size(100, 40),
            BackColor = cor,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };
    }

    private void CarregarDados(string filtro) {
        grid.Rows.Clear();
        var lista = DatabaseHelper.BuscarSenhas(filtro);
        foreach (var item in lista) {
            string senhaReal = CryptoHelper.Descriptografar(item.SenhaCriptografada, chaveMestra);
            
            // Adicionamos a linha com a senha mascarada (hash)
            int index = grid.Rows.Add(item.Id, item.Site, item.Nome, item.Usuario, new string('*', 10));
            
            // Guardamos a senha real na propriedade Tag da linha para recuperar depois
            grid.Rows[index].Tag = senhaReal;
        }
    }

    private void AlternarVisibilidadeSenha_Click(object sender, EventArgs e) {
        if (grid.SelectedRows.Count > 0) {
            var linha = grid.SelectedRows[0];
            var celulaSenha = linha.Cells[4];
            string senhaReal = linha.Tag.ToString();

            if (celulaSenha.Value.ToString().Contains("*")) {
                celulaSenha.Value = senhaReal; // Mostra a senha
            } else {
                celulaSenha.Value = new string('*', 10); // Volta para o hash
            }
        } else {
            MessageBox.Show("Selecione uma linha para ver a senha.");
        }
    }

    private void Excluir_Click(object sender, EventArgs e) {
        if (grid.SelectedRows.Count > 0) {
            int id = (int)grid.SelectedRows[0].Cells[0].Value;
            var resp = MessageBox.Show("Deseja excluir este item permanentemente?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp == DialogResult.Yes) {
                DatabaseHelper.ExcluirSenha(id);
                CarregarDados(txtBusca.Text);
            }
        }
    }

    private void Editar_Click(object sender, EventArgs e) {
        if (grid.SelectedRows.Count > 0) {
            var linha = grid.SelectedRows[0];
            int id = (int)linha.Cells[0].Value;
            string site = linha.Cells[1].Value.ToString();
            string nome = linha.Cells[2].Value.ToString();
            string usuario = linha.Cells[3].Value.ToString();
            string senha = linha.Tag.ToString(); // Pegamos a senha real do Tag

            using (var frmEditar = new FormEditar(id, site, nome, usuario, senha, chaveMestra)) {
                frmEditar.ShowDialog();
                if (frmEditar.DadosSalvos) {
                    CarregarDados(txtBusca.Text);
                }
            }
        } else {
            MessageBox.Show("Selecione uma linha para editar.");
        }
    }
}