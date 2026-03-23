using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace Gerenciador_de_Senhas;

public class DatabaseHelper {
    private static string dbPath = "senhas.db";
    // A string de conexão é mais simples aqui
    private static string connectionString = $"Data Source={dbPath}";

    public static void CriarTabela() {
        using (var conexao = new SqliteConnection(connectionString)) {
            conexao.Open();
            string sql = @"CREATE TABLE IF NOT EXISTS senhas (
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            site TEXT NOT NULL,
                            nome TEXT,
                            usuario TEXT,
                            senha BLOB NOT NULL)";
            using (var comando = new SqliteCommand(sql, conexao)) {
                comando.ExecuteNonQuery();
            }
        }
    }

    public static void SalvarSenha(string site, string nome, string usuario, byte[] senhaCripto) {
        using (var conexao = new SqliteConnection(connectionString)) {
            conexao.Open();
            string sql = "INSERT INTO senhas (site, nome, usuario, senha) VALUES (@site, @nome, @usuario, @senha)";
            using (var cmd = new SqliteCommand(sql, conexao)) {
                cmd.Parameters.AddWithValue("@site", site);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@usuario", usuario);
                cmd.Parameters.AddWithValue("@senha", senhaCripto);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static void ExcluirSenha(int id) {
    using (var conexao = new SqliteConnection(connectionString)) {
        conexao.Open();
        string sql = "DELETE FROM senhas WHERE id = @id";
        using (var cmd = new SqliteCommand(sql, conexao)) {
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}

public static void AtualizarSenha(int id, string site, string nome, string usuario, byte[] senhaCripto) {
    using (var conexao = new SqliteConnection(connectionString)) {
        conexao.Open();
        string sql = "UPDATE senhas SET site=@site, nome=@nome, usuario=@usuario, senha=@senha WHERE id=@id";
        using (var cmd = new SqliteCommand(sql, conexao)) {
            cmd.Parameters.AddWithValue("@site", site);
            cmd.Parameters.AddWithValue("@nome", nome);
            cmd.Parameters.AddWithValue("@usuario", usuario);
            cmd.Parameters.AddWithValue("@senha", senhaCripto);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}

    public static List<SenhaItem> BuscarSenhas(string termo) {
        var lista = new List<SenhaItem>();
        using (var conexao = new SqliteConnection(connectionString)) {
            conexao.Open();
            string sql = "SELECT * FROM senhas WHERE site LIKE @termo OR nome LIKE @termo";
            using (var cmd = new SqliteCommand(sql, conexao)) {
                cmd.Parameters.AddWithValue("@termo", "%" + termo + "%");
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        lista.Add(new SenhaItem {
                            Id = Convert.ToInt32(reader["id"]),
                            Site = reader["site"]?.ToString() ?? "",
                            Nome = reader["nome"]?.ToString() ?? "",
                            Usuario = reader["usuario"]?.ToString() ?? "",
                            SenhaCriptografada = (byte[])reader["senha"]
                        });
                    }
                }
            }
        }
        return lista;
    }
}

public class SenhaItem {
    public int Id { get; set; }
    public string Site { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Usuario { get; set; } = "";
    public byte[] SenhaCriptografada { get; set; } = Array.Empty<byte>();
}