using System;
using UnityEngine.InputSystem;
using UsefulTools.AutoGenerate;

public interface IInputDispatcher
{
    /// <summary>
    /// Actionの登録状態を変更する
    /// </summary>
    /// <param name="actionName"></param>
    /// <param name="action">登録／解除するAction</param>
    /// <param name="registration">Registerで登録、UnRegisterで解除</param>
    /// <param name="actionMap"></param>
    public void ChangeRegistrationStarted<TAction>(ActionMaps actionMap, TAction actionName,
        Action<InputAction.CallbackContext> action, Registration registration) where TAction : Enum;

    /// <summary>
    /// Actionの登録状態を変更する
    /// </summary>
    /// <param name="actionMap">ActionMapsをstringにパースする</param>
    /// <param name="actionName">ActionMap + Actions のEnumをstringにパースして使う</param>
    /// <param name="action">登録／解除するAction</param>
    /// <param name="registration">Registerで登録、UnRegisterで解除</param>
    public void ChangeRegistrationPerformed<TAction>(ActionMaps actionMap, TAction actionName,
        Action<InputAction.CallbackContext> action, Registration registration) where TAction : Enum;

    /// <summary>
    /// Actionの登録状態を変更する
    /// </summary>
    /// <param name="actionMap">ActionMapsをstringにパースする</param>
    /// <param name="actionName">ActionMap + Actions のEnumをstringにパースして使う</param>
    /// <param name="action">登録／解除するAction</param>
    /// <param name="registration">Registerで登録、UnRegisterで解除</param>
    public void ChangeRegistrationCancelled<TAction>(ActionMaps actionMap, TAction actionName,
        Action<InputAction.CallbackContext> action, Registration registration) where TAction : Enum;

    /// <summary>
    /// Actionの登録状態を変更する
    /// </summary>
    /// <param name="actionMap">ActionMapsをstringにパースする</param>
    /// <param name="actionName">ActionMap + Actions のEnumをstringにパースして使う</param>
    /// <param name="action">登録／解除するAction</param>
    /// <param name="registration">Registerで登録、UnRegisterで解除</param>
    public void ChangeRegistrationAll<TAction>(ActionMaps actionMap, TAction actionName,
        Action<InputAction.CallbackContext> action, Registration registration) where TAction : Enum;

    /// <summary>
    /// Actionの登録状態を変更する
    /// </summary>
    /// <param name="actionMap">ActionMapsをstringにパースする</param>
    /// <param name="actionName">ActionMap + Actions のEnumをstringにパースして使う</param>
    /// <param name="action">登録／解除するAction</param>
    /// <param name="registration">Registerで登録、UnRegisterで解除</param>
    public void ChangeRegistrationStartCancelled<TAction>(ActionMaps actionMap, TAction actionName,
        Action<InputAction.CallbackContext> action, Registration registration) where TAction : Enum;


    /// <summary>
    /// 他のActionMapをすべて無効化し、一つのActionMapのみ有効化する
    /// </summary>
    /// <param name="actionMap">ActionMapsをstringにパースする</param>
    public void SwitchActionMap(ActionMaps actionMap);

    /// <summary>
    /// ActionMapを追加で有効化する
    /// </summary>
    /// <param name="actionMap"></param>
    public void EnableActionMap(ActionMaps actionMap);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionMap"></param>
    public void DisableActionMap(ActionMaps actionMap);

    /// <summary>
    /// 現在有効なActionMapを取得する
    /// </summary>
    /// <returns></returns>
    public ActionMaps[] GetActiveActionMap();

    /// <summary>
    /// 入力を有効化する
    /// </summary>
    public void EnableInput();

    /// <summary>
    /// 入力を無効化する
    /// </summary>
    public void DisableInput();
}

public enum Registration
{
    Register,
    UnRegister
}