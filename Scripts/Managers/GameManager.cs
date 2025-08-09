using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏管理类,负责游戏的保存加载、检查点系统、丢失货币管理和场景重启等核心功能
/// </summary>
public class GameManager : MonoBehaviour, ISaveManager
{

    public static GameManager instance;

    private Transform player;

    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private string closestCheckpointId;

    [Header("魂的丢失")]
    [SerializeField] private GameObject lostCurrencyPrefab;
    public int lostCurrencyAmount;
    [SerializeField] private float lostCurrencyX;
    [SerializeField] private float lostCurrencyY;

    /// <summary>
    /// 单例
    /// </summary>
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Start()
    {
        checkpoints = FindObjectsOfType<Checkpoint>();

        player = PlayerManager.instance.player.transform;
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            RestartScene();
    }

    /// <summary>
    /// 重启当前场景
    /// </summary>
    public void RestartScene()
    {
        SaveManager.instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    /// <summary>
    /// 加载数据
    /// </summary>
    /// <param name="_data"></param>
    public void LoadData(GameData _data) => StartCoroutine(LoadWithDelay(_data));

    /// <summary>
    /// 加载检查点，确保玩家重新加载存档时，之前激活过的检查点保持激活状态，不丢失进度
    /// </summary>
    /// <param name="_data"></param>
    private void LoadCheckpoints(GameData _data)
    {
        // 遍历存档中所有检查点的ID和激活状态
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
        {
            // 遍历场景中所有检查点，匹配ID并激活
            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (checkpoint.id == pair.Key && pair.Value == true)
                    checkpoint.ActivateCheckpoint();
            }
        }
    }

    /// <summary>
    /// 加载掉落货币，通常用于玩家死亡后重新加载时，让掉落的货币在原地可重新拾取
    /// </summary>
    /// <param name="_data"></param>
    private void LoadLostCurrency(GameData _data)
    {
        // 从存档读取掉落货币的数量和位置
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;

        // 若有掉落的货币，在对应位置实例化货币物体
        if (lostCurrencyAmount > 0)
        {
            GameObject newLostCurrency = Instantiate(lostCurrencyPrefab, new Vector3(lostCurrencyX, lostCurrencyY), Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0;
    }

    /// <summary>
    /// 协程，延迟加载
    /// </summary>
    /// <param name="_data"></param>
    /// <returns></returns>
    private IEnumerator LoadWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(.1f);

        LoadCheckpoints(_data);
        LoadClosestCheckpoint(_data);
        LoadLostCurrency(_data);
    }

    /// <summary>
    /// 数据保存逻辑
    /// </summary>
    /// <param name="_data"></param>
    public void SaveData(ref GameData _data)
    {
        // 记录当前掉落的货币数量和玩家位置（货币掉落的位置）
        _data.lostCurrencyAmount = lostCurrencyAmount;
        _data.lostCurrencyX = player.position.x;
        _data.lostCurrencyY = player.position.y;

        //保存最近检查点ID
        if (FindClosestCheckpoint() != null)
            _data.closestCheckpointId = FindClosestCheckpoint().id;

        _data.checkpoints.Clear();

        // 遍历所有检查点，保存ID和激活状态
        foreach (Checkpoint checkpoint in checkpoints)
        {
            _data.checkpoints.Add(checkpoint.id, checkpoint.activationStatus);
        }
    }

    /// <summary>
    /// 加载最近检查点位置
    /// </summary>
    /// <param name="_data"></param>
    private void LoadClosestCheckpoint(GameData _data)
    {
        if (_data.closestCheckpointId == null)
            return;

        closestCheckpointId = _data.closestCheckpointId;

        // 遍历检查点，找到最近的检查点并将玩家位置设为该检查点位置
        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (closestCheckpointId == checkpoint.id)
                player.position = checkpoint.transform.position;
        }
    }

    /// <summary>
    /// 查找最近检查点
    /// </summary>
    /// <returns></returns>
    private Checkpoint FindClosestCheckpoint()
    {
        float closestDistance = Mathf.Infinity;
        Checkpoint closestCheckpoint = null;

        foreach (var checkpoint in checkpoints)
        {
            // 计算玩家与检查点的距离
            float distanceToCheckpoint = Vector2.Distance(player.position, checkpoint.transform.position);

            // 只考虑已激活的检查点，更新最近距离和对象
            if (distanceToCheckpoint < closestDistance && checkpoint.activationStatus == true)
            {
                closestDistance = distanceToCheckpoint;
                closestCheckpoint = checkpoint;
            }
        }

        return closestCheckpoint;
    }

    /// <summary>
    /// 游戏暂停
    /// </summary>
    /// <param name="_pause"></param>
    public void PauseGame(bool _pause)
    {
        if (_pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}
