using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class FileUtil
{
    // save image in device for later use
    public static void SaveImage(Texture2D image, string filename)
    {
        string savePath = Application.persistentDataPath;
        try
        {
            // check if directory exists, if not create it
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            string path = Path.Combine(savePath, filename);
            File.WriteAllBytes(path, image.EncodeToPNG());
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.Message);
        }
    }

    // get texture stored in device if exists, if doesn't exists, return null
    public static Texture2D GetImage(string fileName,int width = 1, int height = 1)
    {
        string savePath = Application.persistentDataPath;
        
        string path = Path.Combine(savePath, fileName);
        try
        {
            //first check if texture exists , if exists, start fetching
            if (File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(savePath + fileName);
                Texture2D texture = new Texture2D(2, 2);
                if(texture.LoadImage(bytes))
                return texture;
            }

            return null; // texture not found so return null
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.Message);
            return null;
        }
    }
}
