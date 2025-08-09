using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 黑洞技能热键控制器，玩家可以通过按下对应按键将敌人添加到黑洞技能的目标列表中
/// </summary>
public class Blackhole_HotKey_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotKey;
    private TextMeshProUGUI myText;

    private Transform myEnemy;
    private Blackhole_Skill_Controller blackHole;

    /// <summary>
    /// 热键构造函数
    /// </summary>
    /// <param name="_myNewHotKey"></param>
    /// <param name="_myEnemy"></param>
    /// <param name="_myBlackHole"></param>
    public void SetupHotKey(KeyCode _myNewHotKey, Transform _myEnemy, Blackhole_Skill_Controller _myBlackHole)
    {
        sr = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();

        myEnemy = _myEnemy;
        blackHole = _myBlackHole;

        myHotKey = _myNewHotKey;
        myText.text = _myNewHotKey.ToString();
    }

    /// <summary>
    /// 热键输入检测，当按下绑定的热键时，将敌人添加到黑洞技能的目标列表中
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(myHotKey))
        {
            blackHole.AddEnemyToList(myEnemy);

            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
