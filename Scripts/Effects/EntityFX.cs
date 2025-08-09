using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 实体特效类
/// </summary>
public class EntityFX : MonoBehaviour
{
    protected Player player;
    protected SpriteRenderer sr;

    [Header("Pop Up Text")]
    [SerializeField] private GameObject popUpTextPrefab;

    [Header("闪光特效")]
    [SerializeField] private float flashDuration;
    [SerializeField] private Material hitMat;
    private Material originalMat;


    [Header("异常颜色")]
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] chillColor;
    [SerializeField] private Color[] shockColor;

    [Header("异常粒子效果")]
    [SerializeField] private ParticleSystem igniteFx;
    [SerializeField] private ParticleSystem chillFx;
    [SerializeField] private ParticleSystem shockFx;

    [Header("受击特效")]
    [SerializeField] private GameObject hitFx;
    [SerializeField] private GameObject criticalHitFx;

    /// <summary>
    /// 初始化
    /// </summary>
    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        player = PlayerManager.instance.player;

        originalMat = sr.material;
    }

    /// <summary>
    /// 创建文字弹出效果
    /// </summary>
    /// <param name="_text"></param>
    public void CreatePopUpText(string _text,Color _color)
    {
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(1.5f, 3);

        Vector3 positionOffset = new Vector3(randomX, randomY, 0);

        GameObject newText = Instantiate(popUpTextPrefab, transform.position + positionOffset, Quaternion.identity);

        newText.GetComponent<TextMeshPro>().text = _text;
        newText.GetComponent<TextMeshPro>().color = _color;
    }

    /// <summary>
    /// 透明化
    /// </summary>
    /// <param name="_transprent"></param>
    public void MakeTransprent(bool _transprent)
    {
        if (_transprent)
            sr.color = Color.clear;
        else
            sr.color = Color.white;
    }

    /// <summary>
    /// 协程,实体被攻击时播放闪光特效并停留一段时间，然后返回
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlashFX()
    {
        sr.material = hitMat;
        Color currentColor = sr.color;
        sr.color = Color.white;

        yield return new WaitForSeconds(flashDuration);

        sr.color = currentColor;
        sr.material = originalMat;
    }

    /// <summary>
    /// 红色闪烁特效，用于部分敌人被眩晕时显示
    /// </summary>
    private void RedColorBlink()
    {
        if (sr.color != Color.white)
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }

    /// <summary>
    /// 取消颜色改变，恢复原状
    /// </summary>
    private void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;

        igniteFx.Stop();
        chillFx.Stop();
        shockFx.Stop();
    }

    /// <summary>
    /// 点燃效果特效
    /// </summary>
    /// <param name="_seconds"></param>
    public void IgniteFxFor(float _seconds)
    {
        igniteFx.Play();

        InvokeRepeating("IgniteColorFx", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }

    /// <summary>
    /// 冰冷状态特效
    /// </summary>
    /// <param name="_seconds"></param>
    public void ChillFxFor(float _seconds)
    {
        chillFx.Play();
        InvokeRepeating("ChillColorFx", 0, 1f);
        Invoke("CancelColorChange", _seconds);
    }

    /// <summary>
    /// 震惊状态特效
    /// </summary>
    /// <param name="_seconds"></param>
    public void ShockFxFor(float _seconds)
    {
        shockFx.Play();
        InvokeRepeating("ShockColorFx", 0, .5f);
        Invoke("CancelColorChange", _seconds);
    }

    /// <summary>
    /// 点燃状态颜色
    /// </summary>
    private void IgniteColorFx()
    {
        if (sr.color != igniteColor[0])
            sr.color = igniteColor[0];
        else
            sr.color = igniteColor[1];
    }

    /// <summary>
    /// 冰冷状态颜色
    /// </summary>
    private void ChillColorFx()
    {
        if (sr.color != chillColor[0])
            sr.color = chillColor[0];
        else
            sr.color = chillColor[1];
    }

    /// <summary>
    /// 震惊状态颜色
    /// </summary>
    private void ShockColorFx()
    {
        if (sr.color != shockColor[0])
            sr.color = shockColor[0];
        else
            sr.color = shockColor[1];
    }


    public void CreateHitFx(Transform _target, bool _critical)
    {


        float zRotation = Random.Range(-90, 90);
        float xPosition = Random.Range(-.5f, .5f);
        float yPosition = Random.Range(-.5f, .5f);

        Vector3 hitFxRotaion = new Vector3(0, 0, zRotation);

        GameObject hitPrefab = hitFx;

        if (_critical)
        {
            hitPrefab = criticalHitFx;

            float yRotation = 0;
            zRotation = Random.Range(-45, 45);

            if (GetComponent<Entity>().facingDir == -1)
                yRotation = 180;

            hitFxRotaion = new Vector3(0, yRotation, zRotation);

        }

        GameObject newHitFx = Instantiate(hitPrefab, _target.position + new Vector3(xPosition, yPosition), Quaternion.identity); // uncomment this if you want particle to follow target ,_target);
        newHitFx.transform.Rotate(hitFxRotaion);
        Destroy(newHitFx, .5f);
    }


}
