using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;

//---------------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------------
//--------------------------------TESTES DE INTERGRAÇÃO E IMPLEMENTAÇÃO DA ARQUITETURA---------------------------
//---------------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------------


public class GameManager : MonoBehaviour
{
    public GameObject menuPrincipal;
    public GameObject TelaPause;
    public Text EmailUser;
    public Text PassUser;

    public Text NomeCadastro;
    public Text EmailCadastro;
    public Text PassCadastro;
    public Text ConfirmPassCadastro;
    public Text ConfirmEmailCadastro;

    // Start is called before the first frame update
    void Start()
    {
    }

    //---------------------------------------------------------------------------------------------------------------
    //Teste de Login
    //---------------------------------------------------------------------------------------------------------------

    public void Login()
    {
        StartCoroutine(StartLogin(EmailUser.text.ToString(), PassUser.text.ToString()));
    }

    private IEnumerator StartLogin(string email, string password)
    {
        var LoginTask = Autenticator.instance.auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            HandleLoginErrors(LoginTask.Exception);
        }
        else
        {
            LoginUser(LoginTask);
        }
    }

    void HandleLoginErrors(System.AggregateException loginEx)
    {
        Debug.LogWarning(message: $"Falhou com{loginEx}");
        FirebaseException ex = loginEx.GetBaseException() as FirebaseException;
        AuthError erroCode = (AuthError)ex.ErrorCode;
    }

    void LoginUser(System.Threading.Tasks.Task<Firebase.Auth.FirebaseUser> loginTask)
    {
        Autenticator.instance.user = loginTask.Result;
        Debug.LogFormat("Sucesso: {0} ({1})", Autenticator.instance.user.DisplayName, Autenticator.instance.user.Email);
        SceneManager.LoadScene(2);

    }

    //---------------------------------------------------------------------------------------------------------------
    //Teste de Cadastro
    //---------------------------------------------------------------------------------------------------------------

    public void Register()
    {
        StartCoroutine(StartRegister(EmailCadastro.text.ToString(), NomeCadastro.text.ToString(), PassCadastro.text.ToString()));
    }

    private IEnumerator StartRegister(string email, string nome, string pass)
    {
        if (!CheckRegistrationErrors())
        {
            var RegisterTask = Autenticator.instance.auth.CreateUserWithEmailAndPasswordAsync(email, pass);
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                HandleRegisterErros(RegisterTask.Exception);
            }
            else
            {
                StartCoroutine(RegisterUser(RegisterTask, nome, email, pass));
            }
        }
    }

    bool CheckRegistrationErrors()
    {
        if (NomeCadastro.text == "")
        {
            Debug.LogError("Sem Nome de Usuario");
            return true;
        }
        else if (ConfirmPassCadastro.text != PassCadastro.text)
        {
            Debug.LogError("Senhas não batem");
            return true;
        }
        else if (ConfirmEmailCadastro.text != EmailCadastro.text)
        {
            Debug.LogError("Emails não batem");
            return true;
        }
        else
        {
            return false;
        }
    }

    void HandleRegisterErros(System.AggregateException rex)
    {
        Debug.LogWarning(message: $"Falhou: {rex}");
        FirebaseException ex = rex.GetBaseException() as FirebaseException;
        AuthError errorCode = (AuthError)ex.ErrorCode;
    }

    private IEnumerator RegisterUser(System.Threading.Tasks.Task<Firebase.Auth.FirebaseUser> registerTask, string nome, string email, string pass)
    {
        Autenticator.instance.user = registerTask.Result;
        if(Autenticator.instance.user != null)
        {
            UserProfile profile = new UserProfile { DisplayName = nome};
            var ProfileTask = Autenticator.instance.user.UpdateUserProfileAsync(profile);
            yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);
            if(ProfileTask.Exception != null)
            {
                HandleProfileError(ProfileTask.Exception);
            }
            else
            {
                StartCoroutine(StartLogin(email, pass));
            }
        }
    }

    void HandleProfileError(System.AggregateException pex)
    {
        Debug.LogWarning(message: $"Falhou: {pex}");
        FirebaseException ex = pex.GetBaseException() as FirebaseException;
        AuthError errorCode = (AuthError)ex.ErrorCode;
    }

    //---------------------------------------------------------------------------------------------------------------
    //Testes de passagens entre as telas pela interface
    //---------------------------------------------------------------------------------------------------------------

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && SceneManager.GetActiveScene().buildIndex == 5)
        {
            SceneManager.LoadScene(7);
        }

        if (Input.GetKeyDown(KeyCode.Space) && SceneManager.GetActiveScene().buildIndex == 6)
        {
            SceneManager.LoadScene(8);
        }
    }

    public void abrirTelaCadastro()
    {
        SceneManager.LoadScene(1);
    }

    

    public void abrirLogin()
    {
        SceneManager.LoadScene(0);
    }

    public void abrirSubMenus(GameObject obj)
    {
        if (menuPrincipal != null)
        {
            menuPrincipal.SetActive(false);
        }
        obj.SetActive(true);
    }

    public void fecharSubMenus(GameObject obj)
    {
        if (menuPrincipal != null)
        {
            menuPrincipal.SetActive(true);
        }
        obj.SetActive(false);
    }

    public void sairJogo()
    {
        SceneManager.LoadScene(0);
    }

    public void abrirJogo()
    {
        SceneManager.LoadScene(3);
    }

    public void abrirSelecaoFases()
    {
        SceneManager.LoadScene(4);
    }

    public void abrirFaseBatalha()
    {
        SceneManager.LoadScene(5);
    }

    public void abrirFasePuzzle()
    {
        SceneManager.LoadScene(6);
    }

    public void Pausar()
    {
        TelaPause.SetActive(true);
    }

    public void voltarJogo()
    {
        TelaPause.SetActive(false);
    }

}
