using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Profiling;

public static class SAVEFileIO
{
    private enum FlagTypeID
    {
        Int = 0,
        String = 1,
        Bool = 2,
        Float = 3,
        Null = 255
    }

    public static readonly int FORMAT_VERSION = 2;

    public static readonly string MAGIC = "SAVE";

    public static readonly byte[] HASH_SALT = new byte[32]
    {
        83, 84, 79, 80, 46, 32, 80, 79, 83, 84,
        73, 78, 71, 46, 32, 65, 66, 79, 85, 84,
        46, 32, 65, 77, 79, 78, 71, 46, 32, 85,
        83, 46
    };

    public static void WriteFile(ref SAVEFile file, FileStream stream)
    {
        if (file == null || string.IsNullOrEmpty(file.name))
        {
            file = Util.GameManager().GetFile();
        }
        stream.Write(Encoding.ASCII.GetBytes(MAGIC), 0, MAGIC.Length);
        using (BinaryWriter binaryWriter = new BinaryWriter(stream))
        {
            binaryWriter.Write((short)FORMAT_VERSION);
            long position = stream.Position;
            binaryWriter.Write(HASH_SALT);
            binaryWriter.Write(file.name);
            binaryWriter.Write(file.exp);
            binaryWriter.Write((byte)file.items.Count);
            foreach (int item in file.items)
            {
                binaryWriter.Write((short)item);
            }
            binaryWriter.Write((short)file.weapon);
            binaryWriter.Write((short)file.armor);
            binaryWriter.Write(file.playTime);
            binaryWriter.Write((short)file.zone);
            binaryWriter.Write(file.gold);
            binaryWriter.Write(file.scene);
            binaryWriter.Write(file.pos.x);
            binaryWriter.Write(file.pos.y);
            binaryWriter.Write(file.deaths);
            WriteFlags(file.flags, binaryWriter);
            WriteFlags(file.persFlags, binaryWriter);
            binaryWriter.Flush();
            long position2 = stream.Position;
            byte[] array = new byte[position2];
            stream.Seek(0L, SeekOrigin.Begin);
            stream.Read(array, 0, (int)position2);
            stream.Seek(position, SeekOrigin.Begin);
            using (SHA256 sHA = SHA256.Create())
            {
                binaryWriter.Write(sHA.ComputeHash(array));
            }
            stream.SetLength(position2);
            binaryWriter.Flush();
        }
    }

    public static FileStatus ReadFile(ref SAVEFile file, FileStream fs)
    {
        long length = fs.Length;
        byte[] array = new byte[length];
        fs.Read(array, 0, (int)length);
        using (MemoryStream memoryStream = new MemoryStream(array))
        {
            byte[] array2 = new byte[MAGIC.Length];
            memoryStream.Read(array2, 0, MAGIC.Length);
            if (Encoding.ASCII.GetString(array2) == MAGIC)
            {
                if (file == null)
                {
                    file = new SAVEFile();
                }
                try
                {
                    using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                    {
                        bool flag = false;
                        byte[] array3 = null;
                        byte[] array4 = null;
                        int num = binaryReader.ReadInt16();
                        if (num > FORMAT_VERSION)
                        {
                            //Debug.Log("Not loading, save version is newer than max supported version (" + FORMAT_VERSION + ")");
                            return FileStatus.Newer;
                        }
                        if (num > 0)
                        {
                            array3 = binaryReader.ReadBytes(32);
                            memoryStream.Seek(-32L, SeekOrigin.Current);
                            memoryStream.Write(HASH_SALT, 0, HASH_SALT.Length);
                            using (SHA256 sHA = SHA256.Create())
                            {
                                array4 = sHA.ComputeHash(array);
                                if (!Enumerable.SequenceEqual(array4, array3))
                                {
                                    Debug.Log("Hash mismatch: " + ToHexString(array4) + " != " + ToHexString(array3));
                                    if (num != 1)
                                    {
                                        return FileStatus.Corrupted;
                                    }
                                    flag = true;
                                }
                            }
                        }
                        file.name = binaryReader.ReadString();
                        file.exp = binaryReader.ReadInt32();
                        file.items = new List<int>();
                        byte b = binaryReader.ReadByte();
                        for (int i = 0; i < b; i++)
                        {
                            file.items.Add(binaryReader.ReadInt16());
                        }
                        file.weapon = binaryReader.ReadInt16();
                        file.armor = binaryReader.ReadInt16();
                        file.playTime = binaryReader.ReadInt32();
                        file.zone = binaryReader.ReadInt16();
                        file.gold = binaryReader.ReadInt32();
                        file.scene = binaryReader.ReadInt32();
                        file.pos = new Vector2(binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        file.deaths = binaryReader.ReadInt32();
                        file.flags = ReadFlags(binaryReader);
                        file.persFlags = ReadFlags(binaryReader);
                        if (flag)
                        {
                            if (memoryStream.Length <= memoryStream.Position)
                            {
                                return FileStatus.Corrupted;
                            }
                            using (SHA256 sHA2 = SHA256.Create())
                            {
                                array4 = sHA2.ComputeHash(array, 0, (int)memoryStream.Position);
                                if (!Enumerable.SequenceEqual(array4, array3))
                                {
                                    Debug.Log("Hash mismatch (2): " + ToHexString(array4) + " != " + ToHexString(array3));
                                    return FileStatus.Corrupted;
                                }
                                Debug.Log("File hash discrepancy fixed (" + memoryStream.Length + " -> " + memoryStream.Position + ")");
                                fs.SetLength(memoryStream.Position);
                            }
                        }
                        if (num < FORMAT_VERSION)
                        {
                            return FileStatus.Older;
                        }
                        return FileStatus.OK;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error reading new format file\n" + ex);
                    return FileStatus.Corrupted;
                }
            }
            else
            {
                return FileStatus.Empty;
            }
        }
    }

    private static void WriteFlags(object[] flags, BinaryWriter writer)
    {
        writer.Write((short)flags.Length);
        foreach (object obj in flags)
        {
            object obj2 = obj;
            if (obj2 != null)
            {
                if (!(obj2 is int num))
                {
                    if (!(obj2 is string text))
                    {
                        if (!(obj2 is bool flag))
                        {
                            if (obj2 is float num2)
                            {
                                float value = num2;
                                writer.Write((byte)3);
                                writer.Write(value);
                            }
                            else
                            {
                                Debug.LogError("Invalid flag type " + obj.GetType());
                            }
                        }
                        else
                        {
                            bool value2 = flag;
                            writer.Write((byte)2);
                            writer.Write(value2);
                        }
                    }
                    else
                    {
                        string value3 = text;
                        writer.Write((byte)1);
                        writer.Write(value3);
                    }
                }
                else
                {
                    int value4 = num;
                    writer.Write((byte)0);
                    writer.Write(value4);
                }
            }
            else
            {
                writer.Write(byte.MaxValue);
            }
        }
    }

    private static object[] ReadFlags(BinaryReader reader)
    {
        object[] array = new object[reader.ReadInt16()];
        for (int i = 0; i < array.Length; i++)
        {
            switch (reader.ReadByte())
            {
                case byte.MaxValue:
                    array[i] = null;
                    break;
                case 0:
                    array[i] = reader.ReadInt32();
                    break;
                case 1:
                    array[i] = reader.ReadString();
                    break;
                case 2:
                    array[i] = reader.ReadBoolean();
                    break;
                case 3:
                    array[i] = reader.ReadSingle();
                    break;
            }
        }
        return array;
    }

    private static string ToHexString(byte[] bytes)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte b in bytes)
        {
            stringBuilder.Append($"{b:X2}");
        }
        return stringBuilder.ToString();
    }
}
public enum FileStatus
{
    Corrupted = -2,
    Newer = -1,
    Empty = 0,
    OK = 1,
    Older = 2
}
