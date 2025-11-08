using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UsersDatabase
{
    public int version = 1;
    public List<UserProfile> users = new List<UserProfile>();

    //Fábricas
    public static UsersDatabase CreateEmpty(int version = 1)
    {
        return new UsersDatabase { version = version, users = new List<UserProfile>() };
    }

    //Utilidades de consulta
    public UserProfile FindById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        for (int i = 0; i < users.Count; i++)
        {
            if (string.Equals(users[i].id, id, StringComparison.Ordinal))
                return users[i];
        }
        return null;
    }

    public UserProfile FindByUsername(string username, bool ignoreCase = true)
    {
        if (string.IsNullOrEmpty(username)) return null;
        var cmp = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        for (int i = 0; i < users.Count; i++)
        {
            if (string.Equals(users[i].username, username, cmp))
                return users[i];
        }
        return null;
    }

    public bool ContainsUsername(string username, bool ignoreCase = true)
    {
        return FindByUsername(username, ignoreCase) != null;
    }

    /// Agrega un usuario; no valida duplicados (eso normalmente lo hace el repositorio/servicio).
    public void Add(UserProfile profile)
    {
        if (profile != null)
            users.Add(profile);
    }

    /// Reemplaza un usuario existente por ID; devuelve true si lo encontró y reemplazó.
    public bool Replace(UserProfile profile)
    {
        if (profile == null || string.IsNullOrEmpty(profile.id)) return false;

        for (int i = 0; i < users.Count; i++)
        {
            if (string.Equals(users[i].id, profile.id, StringComparison.Ordinal))
            {
                users[i] = profile;
                return true;
            }
        }
        return false;
    }

    /// Elimina un usuario por ID; devuelve true si lo eliminó.
    public bool RemoveById(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;

        for (int i = 0; i < users.Count; i++)
        {
            if (string.Equals(users[i].id, id, StringComparison.Ordinal))
            {
                users.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
}
