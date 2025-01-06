using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    [Header("Component")]
    public CanvasGroup fadeCanvasGroup;
    [SerializeField] private Dictionary<string, AssetReference> sceneDict = new();
    
    [Header("Settings")]
    [SceneName]
    public string firstLoadScene;
    
    [Header("Debug")]
    private AssetReference currentScene;


    public override void Awake()
    {
        base.Awake();
        FirstLoadScene();
    }

    private void FirstLoadScene()
    {
        currentScene = sceneDict[firstLoadScene];
        fadeCanvasGroup.alpha = 1;
        currentScene.LoadSceneAsync(LoadSceneMode.Additive, true).WaitForCompletion();
        fadeCanvasGroup.DOFade(0, 0.5f).WaitForCompletion();
        // Debug.Log("FirstLoad");
    }

    public void CallLoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }
    
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        yield return fadeCanvasGroup.DOFade(1, 0.5f).WaitForCompletion();
        currentScene.UnLoadScene();
        currentScene = sceneDict[sceneName];
        Debug.Log("Loading Scene " + sceneName);
        
        if(!SceneManager.GetSceneByName(sceneName).isLoaded)
            yield return currentScene.LoadSceneAsync(LoadSceneMode.Additive, true);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        yield return new WaitForSeconds(1);
        yield return fadeCanvasGroup.DOFade(0, 0.5f).WaitForCompletion();
    }
}
