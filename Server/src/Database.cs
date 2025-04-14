using System.Text.RegularExpressions;
using BCrypt.Net;
using Microsoft.Data.Sqlite;
using Shared;

public class Database
{
    private static Database? instance;
    private SqliteConnection connection;

    private Database(string dbPath = "ProjetDev.db")
    {
        connection = new SqliteConnection($"Data Source={dbPath};");
        connection.Open();
        CreateTableAsync().Wait();  // Attendre que la table soit créée
    }

    public static Database GetInstance()
    {
        if (instance == null)
        {
            instance = new Database();
        }
        return instance;
    }

    // Créer la table si elle n'existe pas encore
    private async Task CreateTableAsync()
    {
        var createTableQuery = @"
        CREATE TABLE IF NOT EXISTS users (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            username TEXT NOT NULL UNIQUE,
            email TEXT NOT NULL UNIQUE,
            password TEXT NOT NULL,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            deleted_at DATETIME
        )";
        
        using var cmd = new SqliteCommand(createTableQuery, connection);
        await cmd.ExecuteNonQueryAsync();
    }

    // Ajouter un utilisateur
    public async Task<bool> AddUserAsync(string username, string email, string password)
    {
        if (!VerifyValidUsername(username))
        {
            Console.WriteLine("Nom d'utilisateur invalide.");
            return false;
        }

        if (!VerifyValidEmail(email))
        {
            Console.WriteLine("Email invalide.");
            return false;
        }

        if (!VerifyValidPassword(password))
        {
            Console.WriteLine("Mot de passe invalide.");
            return false;
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, 12);

        var cmd = new SqliteCommand("INSERT INTO users (username, email, password) VALUES (@username, @email, @hashedPassword)", connection);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@hashedPassword", hashedPassword);

        try
        {
            await cmd.ExecuteNonQueryAsync();
            Console.WriteLine("Utilisateur ajouté avec succès.");
            return true;
        }
        catch (SqliteException ex) when (ex.Message.Contains("UNIQUE constraint failed"))
        {
            Console.WriteLine("L'email ou le nom d'utilisateur est déjà pris.");
            return false;
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Erreur lors de l'ajout de l'utilisateur: {ex.Message}");
            return false;
        }
    }

    // Vérifier si un utilisateur existe
    public async Task<bool> UserExistsAsync(string email)
    {
        var cmd = new SqliteCommand("SELECT COUNT(*) FROM users WHERE email = @email", connection);
        cmd.Parameters.AddWithValue("@email", email);
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result) > 0;
    }

    // Récupérer un utilisateur par son email
    public async Task<User?> GetUserAsync(string email, string password)
    {
        var cmd = new SqliteCommand("SELECT id, username, email, password FROM users WHERE email = @email AND deleted_at IS NULL", connection);
        cmd.Parameters.AddWithValue("@email", email);
        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync()){
            var storedPassword = reader.GetString(3); // Récupérer le mot de passe haché depuis la DB
            // Vérifier si le mot de passe fourni correspond au mot de passe stocké
            if (BCrypt.Net.BCrypt.Verify(password, storedPassword)){
                return new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), storedPassword);
            }else{
                Console.WriteLine("Mot de passe incorrect.");
                return null; // Mot de passe incorrect
            }
        }
        return null;
    }

    // Méthode pour supprimer un utilisateur (soft delete)
    public async Task<bool> DeleteUserAsync(string email)
    {
        var cmd = new SqliteCommand("UPDATE users SET deleted_at = CURRENT_TIMESTAMP WHERE email = @email", connection);
        cmd.Parameters.AddWithValue("@email", email);

        try
        {
            await cmd.ExecuteNonQueryAsync();
            Console.WriteLine("Utilisateur supprimé avec succès.");
            return true;
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Erreur lors de la suppression de l'utilisateur: {ex.Message}");
            return false;
        }
    }

    // Vérification du mot de passe
    private bool VerifyValidPassword(string password)
    {
        string pattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$";
        return Regex.IsMatch(password, pattern);
    }

    // Vérification de l'email
    private bool VerifyValidEmail(string email)
    {
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }

    // Vérification du nom d'utilisateur
    private bool VerifyValidUsername(string username)
    {
        return username.Length >= 3 && username.Length <= 16;
    }
}
