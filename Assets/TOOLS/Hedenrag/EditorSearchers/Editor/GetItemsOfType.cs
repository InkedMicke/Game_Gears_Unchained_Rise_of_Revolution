using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hedenrag
{
    namespace GetItemsOfType
    {
        namespace Editor
        {
            public class GetItemsOfType
            {
                public static T[] GetAllInstances<T>() where T : Object
                {
                    string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
                    T[] a = new T[guids.Length];
                    for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                        a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
                    }
                    //Debug.Log($"found {a.Length} instances");
                    return a;
                }
            }
        }
    }
}
