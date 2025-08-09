using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色的视觉特效系统
/// </summary>
public class PlayerFX : EntityFX
{
    [Header("屏幕抖动效果")]
    [SerializeField] private float shakeMultiplier;
    public Vector3 shakeSwordImpact;
    public Vector3 shakeHighDamage;
    private CinemachineImpulseSource screenShake;

    [Header("残影效果")]
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private float colorLooseRate;
    [SerializeField] private float afterImageCooldown;
    private float afterImageCooldownTimer;
    [Space]
    [SerializeField] private ParticleSystem dustFx;

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void Start()
    {
        base.Start();
        screenShake = GetComponent<CinemachineImpulseSource>();
    }

    /// <summary>
    /// 残影计时
    /// </summary>
    private void Update()
    {
        afterImageCooldownTimer -= Time.deltaTime;
    }

    /// <summary>
    /// 屏幕震动控制
    /// </summary>
    /// <param name="_shakePower"></param>
    public void ScreenShake(Vector3 _shakePower)
    {
        screenShake.m_DefaultVelocity = new Vector3(_shakePower.x * player.facingDir, _shakePower.y) * shakeMultiplier;
        screenShake.GenerateImpulse();
    }

    /// <summary>
    /// 残影生成控制
    /// </summary>
    public void CreateAfterImage()
    {
        if (afterImageCooldownTimer < 0)
        {
            afterImageCooldownTimer = afterImageCooldown;
            GameObject newAfterImage = Instantiate(afterImagePrefab, transform.position, transform.rotation);
            newAfterImage.GetComponent<AfterImageFX>().SetupAfterImage(colorLooseRate, sr.sprite);
        }
    }

    /// <summary>
    /// 灰尘粒子播放
    /// </summary>
    public void PlayDustFX()
    {
        if (dustFx != null)
            dustFx.Play();
    }
}
