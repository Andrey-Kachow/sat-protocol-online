using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_WEBGL && !UNITY_EDITOR
// Do not delete line
using System.Runtime.InteropServices; 
#endif

public class MainMenuManager : BaseSceneManager
{
    public Button QuickStartButton;
    public Button GoToEditorButton;
    public Button ExitButton;
    public GameObject MainMenuElementsUI;
    public SceneManagerRotation Rotation;


#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void RedirectToChildAvatarEditor();
    [DllImport("__Internal")]
    private static extern void RedirectToHomePage();
#else
    private static void RedirectToChildAvatarEditor() { }
    private static void RedirectToHomePage() { }
#endif


    void Awake()
    {
        MainMenuElementsUI.transform.localScale = Vector3.zero;
    }

    /// <summary>
    /// Was useful for runtime text setting. Not used at the moment.
    /// </summary>
    /// <param name="button"></param>
    /// <param name="label"></param>
    private void SetUnityUIButtonText(Button button, string label)
    {
        Text buttonText = button.GetComponentInChildren<Text>();
        buttonText.text = label;
        RectTransform buttonRectTransform = button.GetComponent<RectTransform>();
        float paddingV = 10;
        buttonRectTransform.sizeDelta = new Vector2(buttonRectTransform.sizeDelta.x, buttonText.preferredHeight + 2 * paddingV);
    }

    private void Start()
    {
        StartCoroutine(Rotation.SpinFastAndShrink(acelerationDuration: 2f, backwards: true));
        StartCoroutine(MainMenuAppearAnimation());
    }
    public void OnQuickStartButtonClick()
    {
        SceneManager.LoadScene("QuickStartScene");
    }

    public void OnAvatarEditorButtonClick()
    {
        RedirectToChildAvatarEditor();
    }

    public void OnExitButtonClick()
    {
        RedirectToHomePage();
    }

    IEnumerator MainMenuAppearAnimation()
    {
        float scalingUpDuration = 1.5f;
        float fallBackDuration = 0.2f;
        Vector3 oversizeScale = Vector3.one * 1.1f;
        Vector3 oversize = oversizeScale - Vector3.one;

        float elapsedTime = 0f;
        while (elapsedTime < scalingUpDuration)
        {
            float linearInterpolationRatio = elapsedTime / scalingUpDuration;
            MainMenuElementsUI.transform.localScale = oversizeScale * linearInterpolationRatio;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0f;
        while (elapsedTime < fallBackDuration)
        {
            float linearInterpolationRatio = elapsedTime / fallBackDuration;
            MainMenuElementsUI.transform.localScale = oversizeScale - oversize * linearInterpolationRatio;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        MainMenuElementsUI.transform.localScale = Vector3.one;
    }
}
