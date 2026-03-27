using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Gerenciador_de_Senhas;

public class DatabaseHelper {
    // O caminho aponta para um arquivo .json
    private static string jsonPath = "senhas.json";

    // Método para garantir que o arquivo existe (substitui CriarTabela)
    public static void CriarTabela() {
        if (!File.Exists(jsonPath)) {
            File.WriteAllText(jsonPath, "[]"); // Cria um array JSON vazio
        }
    }

    private static List<SenhaItem> LerArquivo() {
        try {
            string json = File.ReadAllText(jsonPath);
            return JsonSerializer.Deserialize<List<SenhaItem>>(json) ?? new List<SenhaItem>();
        } catch {
            return new List<SenhaItem>();
        }
    }

    private static void SalvarArquivo(List<SenhaItem> lista) {
        string json = JsonSerializer.Serialize(lista, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonPath, json);
    }

    public static void SalvarSenha(string site, string nome, string usuario, byte[] senhaCripto) {
        var lista = LerArquivo();
        
        var novoItem = new SenhaItem {
            Site = site,
            Nome = nome,
            Usuario = usuario,
            SenhaCriptografada = senhaCripto
        };

        lista.Add(novoItem);
        SalvarArquivo(lista);
    }

    public static void ExcluirSenha(string site, string usuario) {
        var lista = LerArquivo();
        // Como removemos o ID, usamos Site e Usuário como chave composta para excluir
        lista.RemoveAll(x => x.Site == site && x.Usuario == usuario);
        SalvarArquivo(lista);
    }

    public static void AtualizarSenha(string siteAntigo, string usuarioAntigo, string novoSite, string novoNome, string novoUsuario, byte[] senhaCripto) {
        var lista = LerArquivo();
        var item = lista.FirstOrDefault(x => x.Site == siteAntigo && x.Usuario == usuarioAntigo);
        
        if (item != null) {
            item.Site = novoSite;
            item.Nome = novoNome;
            item.Usuario = novoUsuario;
            item.SenhaCriptografada = senhaCripto;
            SalvarArquivo(lista);
        }
    }

    public static List<SenhaItem> BuscarSenhas(string termo) {
        var lista = LerArquivo();
        if (string.IsNullOrWhiteSpace(termo)) return lista;

        return lista.Where(x => 
            x.Site.Contains(termo, StringComparison.OrdinalIgnoreCase) || 
            (x.Nome != null && x.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase))
        ).ToList();
    }
}

// Classe adaptada: Removido o campo ID para seguir o modelo NoSQL solicitado
public class SenhaItem {
    public string Site { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Usuario { get; set; } = "";
    public byte[] SenhaCriptografada { get; set; } = Array.Empty<byte>();
}
