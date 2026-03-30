using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Gerenciador_de_Senhas;

public class FormVisualizar : Form {
    private byte[] chaveMestra;
    private FlowLayoutPanel containerPrincipal;
    private TextBox txtBusca;

    public FormVisualizar(byte[] chave) {
        this.chaveMestra = chave;
        ConfigurarInterface();
        CarregarDados("");
    }

    private void ConfigurarInterface() {
        this.Text = "Visualizar e Editar Dados";
        this.Size = new Size(750, 750);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.White;

        // --- Painel Superior (Busca + Botão Gerar Senha) ---
        Panel painelTopo = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.FromArgb(240, 240, 240) };
        
        Label lblBusca = new Label { Text = "Buscar:", Location = new Point(20, 26), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
        txtBusca = new TextBox { Location = new Point(80, 24), Width = 300, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };
        txtBusca.TextChanged += (s, e) => CarregarDados(txtBusca.Text);

        Button btnGerarSenha = new Button {
            Text = "🔑 GERAR SENHA",
            Location = new Point(540, 18),
            Size = new Size(160, 35),
            BackColor = Color.MediumSeaGreen,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnGerarSenha.Click += (s, e) => {
            new FormGerador().ShowDialog();
        };

        painelTopo.Controls.Add(lblBusca);
        painelTopo.Controls.Add(txtBusca);
        painelTopo.Controls.Add(btnGerarSenha);

        // --- Container Principal (Scroll) ---
        containerPrincipal = new FlowLayoutPanel {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(30, 20, 30, 20),
            BackColor = Color.White
        };

        this.Controls.Add(containerPrincipal);
        this.Controls.Add(painelTopo);
    }

    private void CarregarDados(string termo) {
        containerPrincipal.Controls.Clear();
        var lista = DatabaseHelper.BuscarSenhas(termo);

        foreach (var item in lista) {
            string senhaReal = "";
            try {
                senhaReal = CryptoHelper.Descriptografar(item.SenhaCriptografada, chaveMestra);
            } catch {
                senhaReal = "ERRO_DESCRIPTOGRAFIA";
            }

            // Agrupador invisível para a conta específica
            FlowLayoutPanel painelConta = new FlowLayoutPanel {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 10)
            };

            // Adiciona os campos padrão
            painelConta.Controls.Add(CriarLinhaInput("Site:", item.Site, false, false, painelConta));
            painelConta.Controls.Add(CriarLinhaInput("Nome:", item.Nome, false, false, painelConta));
            painelConta.Controls.Add(CriarLinhaInput("Usuário:", item.Usuario, false, false, painelConta));
            painelConta.Controls.Add(CriarLinhaInput("Senha:", senhaReal, true, false, painelConta));

            // Painel para os botões de adicionar (ficam em baixo dos inputs da conta)
            FlowLayoutPanel painelBotoes = new FlowLayoutPanel {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(130, 5, 0, 10) // Alinha os botões com as caixas de texto
            };

            Button btnAddTexto = CriarBotaoAcao("+ Campo Texto", Color.DodgerBlue);
            btnAddTexto.Click += (s, e) => AdicionarCampoExtraNaConta("Nome do Campo", false, painelConta);

            Button btnAddSenha = CriarBotaoAcao("+ Senha Extra", Color.MediumPurple);
            btnAddSenha.Click += (s, e) => AdicionarCampoExtraNaConta("Nome da Senha", true, painelConta);

            painelBotoes.Controls.Add(btnAddTexto);
            painelBotoes.Controls.Add(btnAddSenha);
            
            // Adiciona os botões no final do bloco da conta
            painelConta.Controls.Add(painelBotoes);

            // Adiciona a conta inteira ao layout principal
            containerPrincipal.Controls.Add(painelConta);

            // Linha divisória entre as contas
            Panel separador = new Panel {
                Width = 650,
                Height = 2,
                BackColor = Color.FromArgb(220, 220, 220),
                Margin = new Padding(0, 15, 0, 25)
            };
            containerPrincipal.Controls.Add(separador);
        }

        if (lista.Count == 0) {
            containerPrincipal.Controls.Add(new Label { Text = "Nenhum dado encontrado.", AutoSize = true, ForeColor = Color.Gray });
        }
    }

    private void AdicionarCampoExtraNaConta(string placeholder, bool ehSenha, FlowLayoutPanel painelConta) {
        Panel novaLinha = CriarLinhaInput(placeholder, "", ehSenha, true, painelConta);
        painelConta.Controls.Add(novaLinha);
        // Move a nova linha para ficar LOGO ACIMA do painel de botões
        painelConta.Controls.SetChildIndex(novaLinha, painelConta.Controls.Count - 2);
    }

    private Panel CriarLinhaInput(string labelTexto, string valor, bool ehSenha, bool removivel, FlowLayoutPanel parentContainer) {
        Panel linha = new Panel { Width = 680, Height = 45, Margin = new Padding(0, 5, 0, 5) };
        Control elementoNomeDoCampo;

        if (removivel) {
            // Campo Extra: Caixa de texto para o usuário digitar o nome do campo (ex: PIN)
            elementoNomeDoCampo = new TextBox { 
                Text = labelTexto, Location = new Point(0, 8), Width = 120, 
                Font = new Font("Segoe UI", 10, FontStyle.Bold), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.LightYellow 
            };
        } else {
            // Campo Padrão: Texto fixo
            elementoNomeDoCampo = new Label { 
                Text = labelTexto, Location = new Point(0, 10), Width = 120, 
                Font = new Font("Segoe UI", 10, FontStyle.Bold) 
            };
        }

        TextBox txtValor = new TextBox { 
            Text = valor, Location = new Point(130, 8), Width = 350, 
            Font = new Font("Segoe UI", 10), PasswordChar = ehSenha ? '*' : '\0', BorderStyle = BorderStyle.FixedSingle
        };

        linha.Controls.Add(elementoNomeDoCampo);
        linha.Controls.Add(txtValor);

        int proximoX = 490;

        // Botão Olho (👁️)
        if (ehSenha) {
            Button btnOlho = new Button { Text = "👁️", Location = new Point(proximoX, 6), Size = new Size(40, 30), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnOlho.Click += (s, e) => txtValor.PasswordChar = (txtValor.PasswordChar == '*' ? '\0' : '*');
            linha.Controls.Add(btnOlho);
            proximoX += 45;
        }

        // Botão Excluir (X)
        if (removivel) {
            Button btnExcluir = new Button {
                Text = "X", Location = new Point(proximoX, 6), Size = new Size(40, 30), 
                BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, 
                Font = new Font("Arial", 10, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnExcluir.Click += (s, e) => parentContainer.Controls.Remove(linha);
            linha.Controls.Add(btnExcluir);
        }

        return linha;
    }

    private Button CriarBotaoAcao(string texto, Color cor) {
        return new Button {
            Text = texto,
            Size = new Size(130, 30),
            BackColor = cor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 10, 0)
        };
    }
}
