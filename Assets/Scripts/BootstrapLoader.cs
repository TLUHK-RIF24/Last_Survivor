using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private string firstSceneName = "MainMenu";

    private void Awake()
    {
        SceneManager.LoadScene(firstSceneName);
    }
}
