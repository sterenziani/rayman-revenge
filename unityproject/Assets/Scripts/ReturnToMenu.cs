using UnityEngine;

public class ReturnToMenu : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Player>() != null)
        {
            SceneTransitions sceneTransitions = FindObjectOfType<SceneTransitions>();
            sceneTransitions.LoadNextScene();
        }
    }
}
