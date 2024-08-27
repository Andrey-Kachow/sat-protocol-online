using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Linq;

/// <summary>
/// The control for the emotional buttons.
/// </summary>
public class EmotionsMenuManager : MonoBehaviour
{
    private const string BEGIN_BUTTON_LABEL = "Begin SAT";
    private const string END_BUTTON_LABEL = "End SAT";

    public GameObject EmotionsMenu; // The context menu panel
    public Button EmotionButtonPrefab;
    public ChildAvatar ChildAvatar;
    public Player Player;

    public Button BeginSATButtonReference;
    private RectTransform _referenceRectTransformForSAT;

    private Button[] _satButtons;

    private bool _menuActive = false;

    void Start()
    {
        InstantiateSatButtons();
        EmotionsMenu.SetActive(_menuActive);
    }

    private void InstantiateSatButtons()
    {
        float separationCoefficient = 2f;

        ConfugureButton(BeginSATButtonReference, BEGIN_BUTTON_LABEL, () =>
            {
                string newLabel = ChildAvatar.IsPerformingExerciseSAT ? BEGIN_BUTTON_LABEL : END_BUTTON_LABEL;
                SetButtonLabel(BeginSATButtonReference, newLabel);
                ChildAvatar.ToggleSelfAttachmentExercise();
                SetSatButtonsVisible(ChildAvatar.IsPerformingExerciseSAT);
            }
        );

        RectTransform initialRelativePivot = BeginSATButtonReference.GetComponent<RectTransform>();
        RectTransform currentRelativePivot = initialRelativePivot;
        _satButtons = SelfAttachmentEmotionalState.Table.Select
        (
            stateRecord =>
            {
                Button newButton = Instantiate(EmotionButtonPrefab, Vector3.zero, Quaternion.identity);
                newButton.transform.SetParent(EmotionsMenu.transform, worldPositionStays: false);
                newButton.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                newButton.transform.localScale = Vector3.one;

                ConfugureButton(newButton, stateRecord.EmotionName, () => ChildAvatar.SetEmotion(stateRecord.StateName));
                
                RectTransform buttonRectTransform = newButton.GetComponent<RectTransform>();
                CopyRectTransform(currentRelativePivot, buttonRectTransform);

                Canvas.ForceUpdateCanvases();

                buttonRectTransform.Translate(buttonRectTransform.rect.height * separationCoefficient * Vector3.down);
                if (currentRelativePivot == initialRelativePivot)
                {
                    buttonRectTransform.Translate(buttonRectTransform.rect.height * 2 * Vector3.down); // Extra margin
                }
                currentRelativePivot = buttonRectTransform;

                return newButton;
            }
        ).ToArray();
    }

    private void SetSatButtonsVisible(bool visible)
    {
        foreach (Button button in _satButtons)
        {
            button.gameObject.SetActive(visible);
        }
    }


    private void SetButtonLabel(Button button, string label)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = label;
        RectTransform buttonRectTransform = button.GetComponent<RectTransform>();
        float paddingV = 10;
        buttonRectTransform.sizeDelta = new Vector2(buttonRectTransform.sizeDelta.x, buttonText.preferredHeight + 2 * paddingV);
    }
    private void ConfugureButton(Button button, string label, UnityAction clickListener)
    {
        SetButtonLabel(button, label);
        button.onClick.AddListener(clickListener);
    }

    /// <summary>
    /// Helper Method to copy Transform from existing source Transform
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    void CopyRectTransform(RectTransform source, RectTransform target)
    {
        target.anchorMin = source.anchorMin;
        target.anchorMax = source.anchorMax;
        target.pivot = source.pivot;
        target.anchoredPosition = source.anchoredPosition;
        target.sizeDelta = source.sizeDelta;
        target.rotation = source.rotation;
        target.localScale = source.localScale;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleContextMenu();
        }
    }

    void ToggleContextMenu()
    {
        Player.CanMove = _menuActive;
        _menuActive = !_menuActive;
        EmotionsMenu.SetActive(_menuActive);
        Cursor.visible = _menuActive;

        if (_menuActive)
        {
            Cursor.lockState = CursorLockMode.None;
            SetSatButtonsVisible(ChildAvatar.IsPerformingExerciseSAT);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}