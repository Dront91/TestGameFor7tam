using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    private string _nextScene;
    private float _target;
    public float Target => _target;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void StartScene(string sceneName)
    {
        _nextScene = sceneName;
        LoadLevel("Loading");
    }
    private async void LoadLevel(string sceneName)
    {
        _target = 0;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; 
        do
        {
            if (sceneName != "Loading")
            {
                await Task.Delay(100);
            }
            _target = asyncLoad.progress;
        }
        while (asyncLoad.progress < 0.9f);
        if (sceneName != "Loading")
        {
            await Task.Delay(1000);
        }
        asyncLoad.allowSceneActivation = true;
        if (_nextScene != null)
        {
           LoadLevel(_nextScene);
        }
        _nextScene = null;
    }
    
}
