// Creates or rewrites a .txt file for each .resx file in the same folder
// whenever the .resx changes

using UnityEditor;
using UnityEngine;
using System.IO;

public class TmxImporter : AssetPostprocessor
{
    static bool first = true;
    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        
        foreach (string asset in importedAssets)
        {
            if (asset.EndsWith(".tmx"))
            {
                if (first)
                {
                    Debug.Log("Map changes detected, moving .tmx to Resources and changing extension to xml...");
                    first = false;
                }
                //string filePath = asset.Substring(0, asset.Length - Path.GetFileName(asset).Length);
                string filePath = "Assets/Resources/Maps/";

                string newFileName = filePath + Path.GetFileNameWithoutExtension(asset) + ".xml";

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                StreamReader reader = new StreamReader(asset);
                string fileData = reader.ReadToEnd();
                reader.Close();

                FileStream resourceFile = new FileStream(newFileName, FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(resourceFile);
                writer.Write(fileData);
                writer.Close();
                resourceFile.Close();

                AssetDatabase.Refresh(ImportAssetOptions.Default);
                Debug.Log("Success! File saved at " + newFileName);
            }
        }
        first = true;
    }

}