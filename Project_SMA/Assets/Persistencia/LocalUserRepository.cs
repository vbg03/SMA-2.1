using System;
using System.IO;
using System.Text;
using UnityEngine;

public class LocalUserRepository
{
    public const string DefaultFileName = "users.json";

    private readonly object _lock = new object();
    private readonly string _filePath;
    private readonly string _backupPath;
    private readonly string _tempPath;

    /// Base de datos en memoria después de cargar/crear.
    public UsersDatabase Database { get; private set; }

    /// Ruta absoluta al archivo JSON.
    public string FilePath => _filePath;

    public LocalUserRepository(string fileName = DefaultFileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            fileName = DefaultFileName;

        var dir = Application.persistentDataPath;
        EnsureDirectory(dir);

        _filePath = Path.Combine(dir, fileName);
        _backupPath = _filePath + ".bak";
        _tempPath = _filePath + ".tmp";
    }

    /// Carga desde disco si existe; si no, crea una DB vacía y persiste.
    /// Si el archivo está corrupto, renombra a .corrupt y crea una DB nueva.
    public void LoadOrCreate(int initialVersion = 1)
    {
        lock (_lock)
        {
            if (!File.Exists(_filePath))
            {
                Database = UsersDatabase.CreateEmpty(initialVersion);
                SaveAll(); // crea el archivo base
                return;
            }

            try
            {
                var json = File.ReadAllText(_filePath, Encoding.UTF8);
                Database = JsonUtility.FromJson<UsersDatabase>(json);

                // Si por alguna razón vino null (JSON mal formado)
                if (Database == null)
                    throw new Exception("JSON inválido o incompatible con UsersDatabase.");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[LocalUserRepository] Error al cargar JSON: {ex.Message}");
                // Rescata el archivo corrupto
                try
                {
                    var corruptPath = _filePath + ".corrupt_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                    File.Move(_filePath, corruptPath);
                    Debug.LogWarning($"[LocalUserRepository] Archivo corrupto movido a: {corruptPath}");
                }
                catch (Exception moveEx)
                {
                    Debug.LogWarning($"[LocalUserRepository] No se pudo renombrar archivo corrupto: {moveEx.Message}");
                }

                // Crea base vacía
                Database = UsersDatabase.CreateEmpty(initialVersion);
                SaveAll();
            }
        }
    }

    /// Relee desde disco descartando cambios en memoria (útil para depuración).
    public void ReloadFromDisk()
    {
        LoadOrCreate(Database != null ? Database.version : 1);
    }

    /// Guarda el estado actual de Database al archivo principal con escritura atómica
    /// (escribe a .tmp, respalda .bak y luego reemplaza).
    public void SaveAll()
    {
        lock (_lock)
        {
            if (Database == null)
                Database = UsersDatabase.CreateEmpty(1);

            string json = JsonUtility.ToJson(Database, prettyPrint: false);

            try
            {
                // 1) Escribir al temporal
                File.WriteAllText(_tempPath, json, Encoding.UTF8);

                // 2) Hacer backup del actual si existe
                if (File.Exists(_filePath))
                {
                    // Reemplaza/crea el .bak
                    File.Copy(_filePath, _backupPath, overwrite: true);
                }

                // 3) Reemplazar el archivo final
                // Intento con Replace (cuando esté disponible)
                try
                {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
                    // File.Replace no está disponible en todas las plataformas,
                    // por seguridad hacemos Move (borrar y mover).
                    if (File.Exists(_filePath))
                        File.Delete(_filePath);
                    File.Move(_tempPath, _filePath);
#else
                    // En plataformas con soporte:
                    File.Replace(_tempPath, _filePath, _backupPath, ignoreMetadataErrors: true);
#endif
                }
                catch
                {
                    // Fallback robusto
                    if (File.Exists(_filePath))
                        File.Delete(_filePath);
                    File.Move(_tempPath, _filePath);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LocalUserRepository] Error guardando DB: {ex.Message}");
                // Limpieza del temporal si quedó colgado
                SafeDelete(_tempPath);
                throw;
            }
        }
    }

    /// Busca un usuario por username (case-insensitive por defecto).
    public UserProfile FindByUsername(string username, bool ignoreCase = true)
    {
        lock (_lock)
        {
            EnsureDb();
            return Database.FindByUsername(username, ignoreCase);
        }
    }

    /// Agrega un usuario si no existe el username; devuelve true si lo agregó.
    public bool AddUser(UserProfile profile, bool ignoreCase = true, bool autoSave = true)
    {
        if (profile == null || string.IsNullOrEmpty(profile.username))
            return false;

        lock (_lock)
        {
            EnsureDb();

            if (Database.ContainsUsername(profile.username, ignoreCase))
                return false;

            Database.Add(profile);
            if (autoSave) SaveAll();
            return true;
        }
    }

    /// Reemplaza un usuario existente por ID; devuelve true si lo reemplazó (y guarda si autoSave).
    public bool UpdateUser(UserProfile profile, bool autoSave = true)
    {
        if (profile == null || string.IsNullOrEmpty(profile.id))
            return false;

        lock (_lock)
        {
            EnsureDb();

            var replaced = Database.Replace(profile);
            if (replaced && autoSave) SaveAll();
            return replaced;
        }
    }

    /// Elimina un usuario por ID; devuelve true si lo eliminó (y guarda si autoSave).
    public bool RemoveUserById(string id, bool autoSave = true)
    {
        if (string.IsNullOrEmpty(id)) return false;

        lock (_lock)
        {
            EnsureDb();

            var removed = Database.RemoveById(id);
            if (removed && autoSave) SaveAll();
            return removed;
        }
    }

    // Utilidades privadas

    private static void EnsureDirectory(string dir)
    {
        if (string.IsNullOrEmpty(dir)) return;
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    private void EnsureDb()
    {
        if (Database == null)
            Database = UsersDatabase.CreateEmpty(1);
    }

    private static void SafeDelete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch { /* swallow */ }
    }
}