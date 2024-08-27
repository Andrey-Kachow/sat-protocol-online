using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_WEBGL && !UNITY_EDITOR
// Do not delete line
using System.Runtime.InteropServices; 
#endif

public class StartupScreenSceneManager : BaseSceneManager
{
    public static bool DebugMode = false;

    private bool _settingsSet;

    public SceneManagerRotation Rotation;

    /// <summary>
    /// JS Plugin functions that are not available in UnityEditor
    /// </summary>
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void CheckSessionDetails();
    [DllImport("__Internal")]
    private static extern void RedirectToAuthLogin();
#else
    private static void CheckSessionDetails()
    {
        DebugMode = true;
    }
    private static void RedirectToAuthLogin() { }
#endif

    void Start()
    {
        CheckSessionDetails();

        if (DebugMode)
        {
            Debug.Log("Authentication with simulated high latency...");
            StartCoroutine(AuthenticateUser(simulateHighLatency: true));
        }
    }

    public void SessionDetailsResponseCallback(string serializedSessionData)
    {
        UserSessionData.Instance.DeserializeFromCSV(serializedSessionData);
        StartCoroutine(AuthenticateUser(simulateHighLatency: false));
    }

    IEnumerator AuthenticateUser(bool simulateHighLatency = false)
    {
        if (simulateHighLatency)
        {
            yield return new WaitForSecondsRealtime(5);
        }
        if (!_settingsSet)
        {
            yield return new WaitForSecondsRealtime(0.69f);
        }
        yield return StartCoroutine(Rotation.SpinFastAndShrink(acelerationDuration: 1f, backwards: false));

        SceneManager.LoadScene("MainMenuScene");
    }

    public override void SetQualitySettings(string qualityLevel)
    {
        base.SetQualitySettings(qualityLevel);
        _settingsSet = true;
    }
}
