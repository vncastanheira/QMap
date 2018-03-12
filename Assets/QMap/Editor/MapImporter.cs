using QMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QMap.Editor
{
    public class MapImporter : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            // process only MAP files
            IEnumerable<string> mapFiles = importedAssets.Where(asset => asset.EndsWith(".map", System.StringComparison.CurrentCultureIgnoreCase));
            foreach (string mapPath in mapFiles)
            {
                string mapName = Path.GetFileName(mapPath);
                using (StreamReader reader = new StreamReader(mapPath))
                {
                    // read asset as text file
                    var mapText = reader.ReadToEnd();
                    if (mapText == null)
                    {
                        Debug.Log("Failed to import map " + mapName);
                        break;
                    }

                    Tokenizer tokenizer = new Tokenizer(mapText);

                    // create a new game object 
                    // TODO: what if map already exists? how to update?
                    GameObject mapGO = new GameObject(mapName.Replace(".map", string.Empty));
                    Map map = mapGO.AddComponent<Map>();

                    var snapTime = DateTime.Now;

                    try
                    {
                        map.ReadToken(tokenizer);   // process the map
//                        map.CreateMap();            // create the visual brushes and entities
                    }
                    catch (System.FormatException ex)
                    {
                        Debug.LogError(ex.Message);
                    }

                    Debug.Log("Reimported Map: " + mapName + "\nTook " + (DateTime.Now - snapTime).Seconds + " seconds");
                }
            }

            //foreach (string str in deletedAssets)
            //{
            //    Debug.Log("Deleted Asset: " + str);
            //}

            //for (int i = 0; i < movedAssets.Length; i++)
            //{
            //    Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
            //}
        }
    }
}
