using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

/// <summary>
/// 文件数据处理器，负责游戏数据的实际读写、加密解密操作
/// </summary>
public class FileDataHandler 
{
    private string dataDirPath = "";
    private string dataFileName = "";

    private bool encryptData = false;
    private string codeWord = "alexdev";

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="_dataDirPath"></param>
    /// <param name="_dataFileName"></param>
    /// <param name="_encryptData"></param>
    public FileDataHandler(string _dataDirPath, string _dataFileName,bool _encryptData)
    {
        dataDirPath = _dataDirPath;
        dataFileName = _dataFileName;
        encryptData = _encryptData;
    }

    /// <summary>
    /// 数据保存
    /// </summary>
    /// <param name="_data"></param>
    public void Save(GameData _data)
    {
        //组合完整文件路径
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            //确保存储目录存在
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //将GameData对象序列化为JSON字符串
            string dataToStore = JsonUtility.ToJson(_data, true);

            //若需要加密，对JSON字符串进行加密处理
            if (encryptData)
                dataToStore = EncryptDecrypt(dataToStore);

            //写入文件
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }

        //捕获异常并输出错误日志
        catch (Exception e)
        {
            Debug.LogError("Error on trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <returns></returns>
    public GameData Load()
    {
        //组合完整文件路径
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadData = null;

        //检查文件是否存在
        if (File.Exists(fullPath))
        {
            try
            {
                //读取文件内容到字符串
                string dataToLoad = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //若数据加密，则先解密
                if (encryptData)
                    dataToLoad = EncryptDecrypt(dataToLoad);

                //将JSON字符串反序列化为GameData对象
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }

            ////捕获异常并输出错误日志
            catch (Exception e)
            {
                Debug.LogError("Error on trying to load data from file:" + fullPath + "\n" + e);
            }
        }
        
        return loadData;

    }

    /// <summary>
    /// 删除指定路径的存档文件，用于 “删除存档” 功能
    /// </summary>
    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        //确认文件存在
        if(File.Exists(fullPath))
            File.Delete(fullPath);
    }

    /// <summary>
    /// 异或加密
    /// </summary>
    /// <param name="_data"></param>
    /// <returns></returns>
    private string EncryptDecrypt(string _data)
    {
        string modifiedData = "";

        for (int i = 0; i < _data.Length; i++)
        {
            modifiedData += (char)(_data[i] ^ codeWord[i % codeWord.Length]);
        }

        return modifiedData;
    }
}
