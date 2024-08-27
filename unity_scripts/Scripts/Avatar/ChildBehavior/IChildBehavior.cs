using System.Collections;

/// <summary>
/// The unified interface for scheduling or providing coroutine actions that
/// a child avatar can perform during the game.
/// </summary>
public interface IChildBehavior
{
    /// <summary>
    /// Perform some jobs that describe Child behavior relevant to context and state.
    /// Can involve some <see cref="AnimationSwitcher "/> manipulations or decision making
    /// process of the AI (basic if-else perhaps) of the child behavior.
    /// </summary>
    IEnumerator PerformAction();

    /// <summary>
    /// If Child behavior is joined by some concurrent coroutine, then
    /// this method allows to unschedule all related actions that are performed
    /// together. For example, then the current IChildBehavior is paused by the avatar manager.
    /// </summary>
    void StopCoroutinesRelated();
}