using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading_Screen : MonoBehaviour {

    public Text loadingText;
    public GameObject loadingBackground;

    private static Loading_Screen _instance;
    public static Loading_Screen Instance { get { return _instance; } }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    private void Start() {
        GetAllScenes();
    }

    public void LoadSceneWithLoadingScreenInt(int sceneId) {
        DoActualLoadScene(sceneId);
    }

    public void LoadSceneWithLoadingScreenString(string sceneId) {
        DoActualLoadScene(MapNameToInt(sceneId));
    }

    private void DoActualLoadScene(int scene) {
        loadingBackground.SetActive(true);
        StartCoroutine(LoadNewScene(scene));
    }

    public IEnumerator LoadNewScene(int sceneInt) {

        // yield return new WaitForSeconds(1);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneInt);

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone) {
            yield return null;
        }
    }

    private List<string> GetAllScenes() {

        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;

        List<string> allscenes = new List<string>();
        int amtScenes = 0;

        for (int i = 0; i < sceneCount; i++) {
            string SceneName = System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
            allscenes.Add(SceneName);
            amtScenes = amtScenes + 1;
        }
        return allscenes;
    }

    private int MapNameToInt(string mapName) {
        int i = 0;
        foreach (string sceneName in GetAllScenes()) {
            if (sceneName.ToUpper() == mapName.ToUpper()) {
                return i;
            }
            i++;
        }
        return -1;
    }
}
