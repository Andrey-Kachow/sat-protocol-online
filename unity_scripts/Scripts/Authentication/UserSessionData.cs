using System;
using System.Net;

/// <summary>
/// A convenient singleton that stores the key user details to load avatar.
/// The default values store the url and code for the backup child avatar.
/// The backup child avatar is used as Unity Editor sample child avatar.
/// </summary>
public sealed class UserSessionData
{
    private static UserSessionData instance = null;

    private const int GUEST_ID_SUBSTITUTE = -1;
    private const string DEFAULT_AVATAR_CODE = "09c77056-9715-48ce-98fc-c7f499aa349e";
    private const string DEFAULT_DOWNLOAD_URL = "https://metaperson.avatarsdk.com/avatars/c458d7cf-0a98-4dbb-9519-74c367ff2fcd/model.glb";

    private const string CSV_DELIMITER = "\n\n";

    public int UserID { get; private set; } = GUEST_ID_SUBSTITUTE;
    public string AvatarCode { get; private set; } = DEFAULT_AVATAR_CODE;


    private string _avatarDownloadUrl = DEFAULT_DOWNLOAD_URL;
    public string AvatarDownloadUrl 
    {
        get
        {
            //return WebUtility.UrlEncode(_avatarDownloadUrl);
            return  _avatarDownloadUrl;
        }
    }

    private UserSessionData()
    {
    }

    public static UserSessionData Instance
    {
        get
        {
            instance ??= new UserSessionData();
            return instance;
        }
    }

    internal void DeserializeFromCSV(string serializedSessionData)
    {
        if (serializedSessionData != "-1\n\nnull\n\nnull" && serializedSessionData.Contains("://"))
        {
            string[] sessionDetails = serializedSessionData.Split(CSV_DELIMITER);
            UserID = int.Parse(sessionDetails[0]);
            AvatarCode = sessionDetails[1];
            _avatarDownloadUrl = sessionDetails[2];
        }
    }
}