using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour, ISaveManager
{
    [Header("结束界面")]
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;
    [Space]

    [SerializeField] private GameObject charcaterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject inGameUI;



    public UI_SkillToolTip skillToolTip;
    public UI_ItemTooltip itemToolTip;
    public UI_StatToolTip statToolTip;
    public UI_CraftWindow craftWindow;

    [SerializeField] private UI_VolumeSlider[] volumeSettings;

    /// <summary>
    /// 初始化
    /// </summary>
    private void Awake()
    {
        // 在对技能脚本进行关联之前，需要分配技能树槽上的事件
        SwitchToAwake(skillTreeUI);
        fadeScreen.gameObject.SetActive(true);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    void Start()
    {
        SwitchTo(inGameUI);

        itemToolTip.gameObject.SetActive(false);
        statToolTip.gameObject.SetActive(false);
    }

    /// <summary>
    /// 控制按键切换界面
    /// </summary>
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.C))
            SwitchWithKeyTo(charcaterUI);

        if (Input.GetKeyDown(KeyCode.B))
            SwitchWithKeyTo(craftUI);


        if (Input.GetKeyDown(KeyCode.K))
            SwitchWithKeyTo(skillTreeUI);

        if (Input.GetKeyDown(KeyCode.O))
            SwitchWithKeyTo(optionsUI);
    }

    /// <summary>
    /// 初始执行切换
    /// </summary>
    /// <param name="_menu"></param>
    public void SwitchToAwake(GameObject _menu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            // 保持淡入淡出屏幕游戏对象处于活动状态
            bool fadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null;


            if (fadeScreen == false)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        if (_menu != null)
        {
            _menu.SetActive(true);
        }
    }

    /// <summary>
    /// 切换面板
    /// </summary>
    /// <param name="_menu"></param>
    public void SwitchTo(GameObject _menu)
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            // 保持淡入淡出屏幕游戏对象处于活动状态
            bool fadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null; 


            if (fadeScreen == false)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        if (_menu != null)
        {
            AudioManager.instance.PlaySFX(7, null);
            _menu.SetActive(true);
        }


        if (GameManager.instance != null)
        {
            if (_menu == inGameUI)
                GameManager.instance.PauseGame(false);
            else
                GameManager.instance.PauseGame(true);
        }
    }

    /// <summary>
    /// 如果目标菜单存在且处于激活状态，则隐藏它并检查是否处于游戏界面；否则调用SwitchTo显示菜单
    /// </summary>
    /// <param name="_menu"></param>
    public void SwitchWithKeyTo(GameObject _menu)
    {
        if (_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            CheckForInGameUI();
            return;
        }

        SwitchTo(_menu);
    }

    /// <summary>
    /// 检查是否处于游戏界面
    /// </summary>
    private void CheckForInGameUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UI_FadeScreen>() == null)
                return;
        }

        SwitchTo(inGameUI);
    }

    /// <summary>
    /// 切换到死亡页面，黑屏淡出
    /// </summary>
    public void SwitchOnEndScreen()
    {
        fadeScreen.FadeOut();
        StartCoroutine(EndScreenCorutione());
    }

    /// <summary>
    /// 协程，加载死亡页面
    /// </summary>
    /// <returns></returns>
    IEnumerator EndScreenCorutione()
    {
        yield return new WaitForSeconds(1);
        endText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        restartButton.SetActive(true);

    }

    /// <summary>
    /// 重启游戏场景
    /// </summary>
    public void RestartGameButton() => GameManager.instance.RestartScene();

    /// <summary>
    /// 加载音量设置
    /// </summary>
    /// <param name="_data"></param>
    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string, float> pair in _data.volumeSettings)
        {
            foreach (UI_VolumeSlider item in volumeSettings)
            {
                if (item.parametr == pair.Key)
                    item.LoadSlider(pair.Value);
            }
        }
    }

    /// <summary>
    /// 保存音量设置
    /// </summary>
    /// <param name="_data"></param>
    public void SaveData(ref GameData _data)
    {
        _data.volumeSettings.Clear();

        foreach (UI_VolumeSlider item in volumeSettings)
        {
            _data.volumeSettings.Add(item.parametr, item.slider.value);
        }
    }
}
