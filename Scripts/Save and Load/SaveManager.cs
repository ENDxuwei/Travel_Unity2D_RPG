using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

/// <summary>
/// 存档管理器，负责处理游戏数据的保存、加载和管理功能
/// </summary>
public class SaveManager : MonoBehaviour 
{
    public static SaveManager instance;

    [SerializeField] private string fileName;
    [SerializeField] private string filePath = "idbfs/data";
    [SerializeField] private bool encryptData;
    private GameData gameData;
    [SerializeField] private List<ISaveManager> saveManagers;
    private FileDataHandler dataHandler;

    /// <summary>
    /// 在Unity编辑器中调用删除存档文件，测试用。
    /// </summary>
    [ContextMenu("删除存档")]
    public void DeleteSavedData()
    {
        dataHandler = new FileDataHandler(filePath, fileName,encryptData);
        dataHandler.Delete();

    }
    
    /// <summary>
    /// 单例模式
    /// </summary>
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }


    /// <summary>
    /// 初始化文件操作工具，自动扫描场景中所有实现ISaveManager的组件，尝试加载存档。
    /// </summary>
    private void Start()
    {
        dataHandler = new FileDataHandler(filePath, fileName,encryptData);
        saveManagers = FindAllSaveManagers();

        //确保其他系统先初始化
        Invoke("LoadGame", .05f);
           
    }

    /// <summary>
    /// 创建新游戏
    /// </summary>
    public void NewGame()
    {
        gameData = new GameData();
    }

    /// <summary>
    /// 加载流程通过文件管理工具从文件加载数据。
    /// 若加载失败，则调用NewGame()初始化默认数据。
    /// 反之遍历所有ISaveManager，通知各系统加载数据。
    /// </summary>
    public void LoadGame()
    {
        gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("未找到存档！");
            NewGame();
        }

        // 通知所有系统加载数据
        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
    }

    /// <summary>
    /// 保存流程，遍历所有ISaveManager，让各系统将自身当前状态写入gameData，然后序列化存储。
    /// </summary>
    public void SaveGame()
    {

        foreach(ISaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    /// <summary>
    /// 应用退出时自动调用SaveGame()，避免玩家忘记手动存档导致的数据丢失
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    /// <summary>
    /// 遍历所有调用存储接口的文件
    /// </summary>
    /// <returns></returns>
    private List<ISaveManager> FindAllSaveManagers()
    {
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>();

        return new List<ISaveManager>(saveManagers);
    }

    /// <summary>
    /// 判断有无存档
    /// </summary>
    /// <returns></returns>
    public bool HasSavedData()
    {
        if (dataHandler.Load() != null)
        {
            return true;
        }

        return false;
    }

    public void ExitGame()
    {
        Debug.Log("Exit game");
        Application.Quit();
    }
}
