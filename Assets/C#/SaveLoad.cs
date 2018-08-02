using UnityEngine;
// next line enables use of the operating system's serialization capabilities within the script
using System.Runtime.Serialization.Formatters.Binary;
// next line, IO stands for Input/Output, and is what allows us to write to and read from
// our computer or mobile device. Allowing to create unique files and then read them.
using System.IO;

public static class SaveLoad
{
    public const int SOUND_NOICE = 0;
    public const int MUSIC_NOICE = 1;
    public const int PLAYER_POS_X = 2;
    public const int PLAYER_POS_Y = 3;
    public const int PLAYER_POS_Z = 4;
    public const int PLAYER_EULER_X = 5;
    public const int PLAYER_EULER_Y = 6;
    public const int PLAYER_EULER_Z = 7;
    public const int CHECKPOINT_INITIALIZED = 8;
    public const string FILE_PATH = "/GameCHAMP.gd";

    public static float[] Floats { get; set; }

    public static bool FindSaveFile()
    {
        return File.Exists(Application.persistentDataPath + FILE_PATH);
    }

    public static void MakeSaveFile()
    {
        Floats = new float[9] { 0.6f, 0.4f, 0f, 0f, 0f, 0f, 0f, 0f, 0 };

        Save();
    }

    public static void SaveCheckpoint(Transform checkpoint)
    {
        Floats[PLAYER_POS_X] = checkpoint.position.x;
        Floats[PLAYER_POS_Y] = checkpoint.position.y;
        Floats[PLAYER_POS_Z] = checkpoint.position.z;

        Floats[PLAYER_EULER_X] = checkpoint.eulerAngles.x;
        Floats[PLAYER_EULER_Y] = checkpoint.eulerAngles.y;
        Floats[PLAYER_EULER_Z] = checkpoint.eulerAngles.z;

        Floats[CHECKPOINT_INITIALIZED] = 1;

        Save();
    }

    public static void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + FILE_PATH);
        bf.Serialize(file, Floats);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + FILE_PATH))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + FILE_PATH, FileMode.Open);
            Floats = (float[])bf.Deserialize(file);
            file.Close();
        }
    }

    public static void Delete()
    {
        if (File.Exists(Application.persistentDataPath + FILE_PATH))
            File.Delete(Application.persistentDataPath + FILE_PATH);
    }
}
