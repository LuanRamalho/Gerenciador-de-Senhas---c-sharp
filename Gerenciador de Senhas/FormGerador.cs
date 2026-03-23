using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Gerenciador_de_Senhas;

public class FormGerador : Form {
    private NumericUpDown numTamanho;
    private CheckBox chkMaiusculas, chkMinusculas, chkNumeros, chkEspeciais;
    private TextBox txtResultado;

    public FormGerador() {
        this.Text = "Gerador de Senhas Seguras";
        this.Size = new Size(350, 400);
        this.StartPosition = FormStartPosition.CenterParent;
        ConfigurarInterface();
    }

    private void ConfigurarInterface() {
        Label lbl = new Label { Text = "Tamanho:", Location = new Point(20, 20), AutoSize = true };
        numTamanho = new NumericUpDown { Location = new Point(100, 18), Width = 50, Minimum = 4, Maximum = 32, Value = 12 };

        chkMaiusculas = new CheckBox { Text = "Letras Maiúsculas", Location = new Point(20, 60), AutoSize = true, Checked = true };
        chkMinusculas = new CheckBox { Text = "Letras Minúsculas", Location = new Point(20, 90), AutoSize = true, Checked = true };
        chkNumeros = new CheckBox { Text = "Números", Location = new Point(20, 120), AutoSize = true, Checked = true };
        chkEspeciais = new CheckBox { Text = "Caracteres Especiais", Location = new Point(20, 150), AutoSize = true };

        txtResultado = new TextBox { Location = new Point(20, 200), Width = 290, Font = new Font("Consolas", 12), ReadOnly = true };

        Button btnGerar = new Button { Text = "GERAR SENHA", Location = new Point(20, 240), Size = new Size(290, 40), BackColor = Color.MediumPurple, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        btnGerar.Click += GerarSenha_Click;

        Button btnCopiar = new Button { Text = "Copiar Senha", Location = new Point(20, 290), Size = new Size(290, 30) };
        btnCopiar.Click += (s, e) => { if (!string.IsNullOrEmpty(txtResultado.Text)) Clipboard.SetText(txtResultado.Text); };

        this.Controls.AddRange(new Control[] { lbl, numTamanho, chkMaiusculas, chkMinusculas, chkNumeros, chkEspeciais, txtResultado, btnGerar, btnCopiar });
    }

    private void GerarSenha_Click(object sender, EventArgs e) {
        string chars = "";
        if (chkMaiusculas.Checked) chars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        if (chkMinusculas.Checked) chars += "abcdefghijklmnopqrstuvwxyz";
        if (chkNumeros.Checked) chars += "0123456789";
        if (chkEspeciais.Checked) chars += "!@#$%^&*()_+-=[]{}|;:,.<>?";

        if (chars == "") return;

        Random rand = new Random();
        txtResultado.Text = new string(Enumerable.Repeat(chars, (int)numTamanho.Value)
            .Select(s => s[rand.Next(s.Length)]).ToArray());
    }
}