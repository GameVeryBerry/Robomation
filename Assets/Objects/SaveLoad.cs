using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

// Super easy saving/loading of binary data for Unity games
// by @kkukshtel
// License: Do whatever!
// https://gist.github.com/kkukshtel/ebaff3953d6db3567b81652bc55101b2

public enum SaveType
{
    //Add more types here
    Floor,
    Instruction,
}
public static class SaveLoad
{
    public delegate T DataReader<T>(FileStream fs)
        where T : class;
    public delegate void DataWriter<T>(FileStream fs, T data)
        where T : class;

    // Make more properites like this to easily access file names
    public static List<string> Maps
    {
        get { return GetFilesOfType(SaveType.Floor); }
    }

    //Points to User/AppData/LocalLow/DefaultCompany/YourGame/...
    public static string dataPath = Application.dataPath;
    public static string userDataPath = Application.persistentDataPath;

    // Add other directories here to indicate where you want to save
    public static Dictionary<SaveType, string> gameDirectories = new Dictionary<SaveType, string>
    {
        {SaveType.Floor, dataPath + "/floors/"},
        {SaveType.Instruction, userDataPath + "/instructions/"},
    };
    
     // Add your file extensions here - they can be anything!
    public static Dictionary<SaveType, string> gameFileExtensions = new Dictionary<SaveType, string>
    {
        {SaveType.Floor, ".map"},
        {SaveType.Instruction, ".map"},
    };

    // Call this at the start of your game if you want unity to make your directories for you to ensure they are there
    public static void CreateGameDirectories()
    {
        foreach (KeyValuePair<SaveType, string> directory in gameDirectories)
        {
            Directory.CreateDirectory(directory.Value);
        }
    }

    //Call this to save the data of your game. highly recommend using JsonUtility.ToJson to populate the data variable
    public static void SaveData(string data, string nameOfFile, SaveType typeOfFile, bool overwrite = false)
    {
        string file = FileName(nameOfFile, typeOfFile);

        try
        {
            //overwrite protection
            if (!overwrite && File.Exists(file))
            {
                Debug.Log(file + " already exists.");
                return;
            }

            Debug.Log("writing " + file);

            // create the file
            using (FileStream fs = File.Create(file))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(data);
                fs.Write(info, 0, info.Length);
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public static void SaveBinary<T>(T data, string nameOfFile, SaveType typeOfFile, DataWriter<T> dataWriter, bool overwrite = false)
        where T : class
    {
        if (data == null)
            return;

        string file = FileName(nameOfFile, typeOfFile);

        try
        {
            //overwrite protection
            if (!overwrite && File.Exists(file))
            {
                Debug.Log(file + " already exists.");
                return;
            }

            Debug.Log("writing " + file);

            // create the file
            using (FileStream fs = File.Create(file))
            {
                dataWriter(fs, data);
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    // Call this to load the data of your game. highly recommend wrapping this call with JSON utility like so:
    // YourSerializeableClass loadedObject = JsonUtility.FromJson<YourSerializeableClass>(SaveLoad.LoadData(nameOfThingToLoad, SaveType.Floor));
    public static string LoadData(string nameOfFile, SaveType typeOfFile)
    {
        string file = FileName(nameOfFile, typeOfFile);
        string data = "";
        try
        {
            //make sure file exists
            if (File.Exists(file))
            {
                Debug.Log("loading : " + file);
                using (StreamReader sr = File.OpenText(file))
                {
                    data = sr.ReadToEnd();
                    return data;
                }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return null;
    }

    public static T LoadBinary<T>(string nameOfFile, SaveType typeOfFile, DataReader<T> dataReader)
        where T: class
    {
        string file = FileName(nameOfFile, typeOfFile);
        try
        {
            //make sure file exists
            if (File.Exists(file))
            {
                Debug.Log("loading : " + file);
                using (FileStream sr = File.OpenRead(file))
                {
                    return dataReader(sr);
                }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return null;
    }

    //Gets the full directory filename for a given filename
    static string FileName(string fileName, SaveType fileType)
    {
        return gameDirectories[fileType] + fileName + gameFileExtensions[fileType];
    }
    
    //Get all files in your save directory of file type
    //great for filling out menus
    public static List<string> GetFilesOfType(SaveType fileType)
    {
        Debug.Log($"gettting files at {gameDirectories[fileType]} of extension type {gameFileExtensions[fileType]}");
        DirectoryInfo dir = new DirectoryInfo(gameDirectories[fileType]);
        FileInfo[] info = dir.GetFiles("*" + gameFileExtensions[fileType]);
        List<string> fileNames = new List<string>();
        foreach (FileInfo f in info) 
        { 
            fileNames.Add(Path.GetFileNameWithoutExtension(f.FullName)); 
        }
        return fileNames;
    }
}

