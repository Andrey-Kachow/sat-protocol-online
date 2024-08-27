using UnityEngine;

public class BaseSceneManager : MonoBehaviour
{
    public virtual void SetQualitySettings(string qualityLevel)
    {
        switch (qualityLevel)
        {
            case "Low":
                QualitySettings.SetQualityLevel(0);
                break;
            case "High":
                QualitySettings.SetQualityLevel(4);
                break;
            case "Medium":
            default:
                QualitySettings.SetQualityLevel(2);
                break;
        };
    }
}