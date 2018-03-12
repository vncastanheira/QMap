using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MapControls : EditorWindow
{
    [MenuItem("QMap/Reimport Maps")]
    public static void ReimportMap()
    {
        AssetImporter.GetAtPath("Assets/Resources/bogus.map").SaveAndReimport();
    }

    [MenuItem("QMap/Print Local Folder")]
    public static void PrintLocalFolder()
    {
    }
}
