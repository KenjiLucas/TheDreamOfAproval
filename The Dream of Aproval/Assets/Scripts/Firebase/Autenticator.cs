using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;

public class Autenticator : MonoBehaviour
{
    public static Autenticator instance;
    public DependencyStatus ds;
    public FirebaseAuth auth;
    public FirebaseUser user;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            ds = task.Result;
            if(ds == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Erro na dependencia: " + ds);
            }
        });
    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
    }
}
