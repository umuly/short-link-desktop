using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseBase<T>
{
    public int status;

    public string statusText;

    public T item;

    public int count;


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