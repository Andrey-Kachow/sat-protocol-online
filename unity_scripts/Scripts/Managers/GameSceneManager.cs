using AvatarSDK.MetaPerson.Loader;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : BaseSceneManager
{
    // Some bench marks
    private const float GAME_BENCH_SEAT_HEIGHT = 0.60f;
    private const float AVG_IRL_BENCH_HEIGHT = 0.505f;
    public const float GAME_OVERSCALE = GAME_BENCH_SEAT_HEIGHT / AVG_IRL_BENCH_HEIGHT;

    public ChildAvatar ChildAvatar;
    public MetaPersonLoader MetaPersonLoader;

    public void Start()
    {
        //LoadChildAvatarMetaPersonModel();
        StartCoroutine(LoadModelAsync());
    }

    /// <summary>
    /// The <see cref="IEnumerator"/> version of MetaPersonLoader is a coroutine,
    /// that shown to be superior to async/await by letting the game catch the breath
    /// while played in the browser. Otherwise the frame requests to much time and
    /// the game fails, because WebGL is single-threaded and concurrency is simulated
    /// by the Unity Scheduler.
    /// The downside is that the avatar is not loaded ASAP, but checked every second,
    /// so a potential delay of up to one extra second is introduced.
    /// Since the child should be downloaded once, the latter was identified as a flawless success.
    /// If the wi-fi gets slow, the coroutine approach will eventually download,
    /// while await is risky.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadModelAsync()
    {
        MetaPersonLoader.animatorController = ChildAvatar.AnimationSwitcher.OverrideController;

        Debug.Log($"The url is {UserSessionData.Instance.AvatarDownloadUrl}");
        Task<bool> download = MetaPersonLoader.LoadModelAsync(UserSessionData.Instance.AvatarDownloadUrl);
        while (!download.IsCompleted)
        {
            yield return new WaitForSeconds(1);
        }
        bool successfulDownload = download.IsCompletedSuccessfully;
        if (!successfulDownload)
        {
            StartCoroutine(ExitGame());
            yield break;
        }

        GameObject avatarRoot = MetaPersonLoader.transform.GetChild(0).gameObject;
        ChildAvatar.SetModel(avatarRoot);
    }

    //private async void LoadChildAvatarMetaPersonModel()
    //{
    //    MetaPersonLoader.animatorController = ChildAvatar.AnimationSwitcher.OverrideController;
    //    bool successfulDownload = await MetaPersonLoader.LoadModelAsync(UserSessionData.Instance.AvatarDownloadUrl);
    //    if (!successfulDownload)
    //    {
    //        StartCoroutine(ExitGame());
    //        return;
    //    }
    //    GameObject avatarRoot = MetaPersonLoader.transform.GetChild(0).gameObject;
    //    ChildAvatar.SetModel(avatarRoot);
    //}

    private IEnumerator ExitGame()
    {
        yield return new WaitForSeconds(6);
        SceneManager.LoadScene("MainMenuScene");
    }

#if UNITY_EDITOR
    /*
     *   Debugging Area. This only works in Unity Editor runs and can be used
     *   for additional debug logs called in a coroutine.
     */
    public GameObject ObjectToMessWith;
    public void Awake()
    {
        StartCoroutine(DoSomeDebug());
    }
    IEnumerator DoSomeDebug()
    {
        yield return new WaitForSeconds(4);
        
        if (ObjectToMessWith != null)
        {
            Debug.Log($"The height of the bench is {ObjectToMessWith.transform.position.y}");
        }
        yield return new WaitForSeconds(1);
    }
#endif
}
