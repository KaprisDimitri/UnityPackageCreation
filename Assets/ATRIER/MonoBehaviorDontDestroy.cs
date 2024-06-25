using UnityEngine;
using UnityEngine.SceneManagement;


//QUESTIONNEMENT SUR L UTILITER DE CETTE CLASS APRES COUP
//Pourrais recuperer des donnee
public class MonoBehaviorDontDestroy : MonoBehaviour
{
    protected virtual void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Logique � ex�cuter lorsque la sc�ne est charg�e
    }

    protected virtual void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
