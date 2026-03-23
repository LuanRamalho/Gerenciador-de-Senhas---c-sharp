using System;
using System.Windows.Forms;

namespace Gerenciador_de_Senhas;

static class Program {
    [STAThread]
    static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormPrincipal());

    }
}