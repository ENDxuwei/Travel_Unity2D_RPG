using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏主菜单
/// </summary>
public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;
    [SerializeField] UI_FadeScreen fadeScreen;

    /// <summary>
    /// 启动时检查是否存在存档数据,如果没有存档，隐藏 "继续游戏" 按钮，避免玩家点击无效
    /// </summary>
    private void Start()
    {
        if (SaveManager.instance.HasSavedData() == false)
            continueButton.SetActive(false);
    }

    /// <summary>
    /// 继续游戏
    /// </summary>
    public void ContinueGame()
    {
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
    }

    /// <summary>
    /// 开始新游戏
    /// </summary>
    public void NewGame()
    {
        SaveManager.instance.DeleteSavedData();
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("Exit game");
        Application.Quit();
    }

    /// <summary>
    /// 协程，淡入淡出效果加载游戏场景
    /// </summary>
    /// <param name="_delay"></param>
    /// <returns></returns>
    IEnumerator LoadSceneWithFadeEffect(float _delay)
    {
        fadeScreen.FadeOut();

        yield return new WaitForSeconds(_delay);

        SceneManager.LoadScene(sceneName);
    }
}
