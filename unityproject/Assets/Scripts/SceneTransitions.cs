using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitions : MonoBehaviour
{
    public Animator transitionAnim;
    public string nextSceneName;

    public virtual void ReloadScene()
    {
        StartCoroutine(ReloadSceneAfter(1.5f));
    }

    public virtual void LoadNextScene()
    {
        StartCoroutine(LoadNextSceneAfter(1.5f));
    }

    IEnumerator ReloadSceneAfter(float time)
    {
        transitionAnim.SetTrigger("end");
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator LoadNextSceneAfter(float time)
    {
        transitionAnim.SetTrigger("end");
        yield return new WaitForSeconds(time);
        //int nextSceneIndex = (1 + SceneManager.GetActiveScene().buildIndex) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneName);
    }
}
