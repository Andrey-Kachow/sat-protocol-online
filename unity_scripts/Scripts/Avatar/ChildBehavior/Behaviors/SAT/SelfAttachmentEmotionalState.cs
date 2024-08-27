public record SelfAttachmentEmotionalState
{
    public static readonly SelfAttachmentEmotionalState[] Table =
    {
        // For Russian Human Trial Participants.
        //
        //new("Злость_St", new string[]{"Angry Gesture", "Angry Point"}),
        //new("Досада_St", new string[]{"Annoyed Head Shake", "Disappointed"}),
        //new("Горе_St", new string[]{"Crying", "Rejected"}),
        //new("Покой_St", new string[]{"Yawn"}),
        //new("Счастье_St", new string[]{"Happy"}),
        //new("Радость_St", new string[]{"Cheering"}),
        //new("Потанцуем?_St", new string[]{"Silly Dancing", "Silly Twist Dancing"}),

        new("Angry_St", new string[]{"Angry Gesture", "Angry Point"}),
        new("Annoyed_St", new string[]{"Annoyed Head Shake", "Disappointed"}),
        new("Grief_St", new string[]{"Crying", "Rejected"}),
        new("Neutral_St", new string[]{"Yawn"}),
        new("Happy_St", new string[]{"Happy"}),
        new("Cheering_St", new string[]{"Cheering"}),
        new("Dancing_St", new string[]{"Silly Dancing", "Silly Twist Dancing"}),
    };
    public static readonly SelfAttachmentEmotionalState NEUTRAL_STATE = Table[3];

    public string StateName;
    public string[] AcceptableClipNames;

    /// <summary>
    /// The Name of emotional state that does not display internal characters
    /// and suitable to be diplayed as a button label.
    /// For example Angry_St -> Angry...
    /// The name is derived from <seealso cref="StateName"/>.
    /// </summary>
    public string EmotionName
    {
        get => StateName[..^3];
    }
    public SelfAttachmentEmotionalState(string stateName, params string[] acceptableClipNames)
    {
        StateName = stateName;
        AcceptableClipNames = acceptableClipNames;
    }

    public override string ToString()
    {
        return $"st( \"{StateName}\" - {Utils.ArrayToPrettyString(AcceptableClipNames)} )";
    }
}
