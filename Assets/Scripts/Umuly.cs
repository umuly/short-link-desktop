using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseBase<T>
{
    public int status;

    public string statusText;

    public List<string> statusTexts;

    public T item;

    public int count;

    public static ResponseBase<T> CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<ResponseBase<T>>(jsonString);
    }
}

public class Register
{
    public string name;
    public string email;
    public string password;

    public static string CreateJSON(Register user)
    {
        return JsonUtility.ToJson(user);
    }
}

[Serializable]
public class Umuly
{
    public string Name;
    public string Email;
    public string Password;
}