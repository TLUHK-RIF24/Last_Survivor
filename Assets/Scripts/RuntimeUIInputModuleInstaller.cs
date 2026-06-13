using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public static class RuntimeUIInputModuleInstaller
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        InstallForLoadedEventSystems();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InstallForLoadedEventSystems();
    }

    private static void InstallForLoadedEventSystems()
    {
        EventSystem[] eventSystems = UnityEngine.Object.FindObjectsByType<EventSystem>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        foreach (EventSystem eventSystem in eventSystems)
        {
            InputSystemUIInputModule module = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (module == null)
            {
                module = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }

            module.deselectOnBackgroundClick = true;
            module.moveRepeatDelay = 0.5f;
            module.moveRepeatRate = 0.1f;
            module.scrollDeltaPerTick = 6f;

            if (module.actionsAsset == null)
            {
                module.AssignDefaultActions();
            }
        }
    }
}
