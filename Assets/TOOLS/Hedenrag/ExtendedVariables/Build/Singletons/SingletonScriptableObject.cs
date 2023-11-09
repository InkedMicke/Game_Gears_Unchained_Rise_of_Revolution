using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Hedenrag
{
    namespace ExVar
    {
        public class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>, ICallOnAll
        {
            private static T instance;
            public static T Instance
            {
                get
                {
                    if (instance == null)
                    {
                        T[] assets = Resources.LoadAll<T>("Singletons");
                        if (assets == null || assets.Length < 1)
                        {
#if UNITY_EDITOR
                            GenerateSingleton();
                            assets = Resources.LoadAll<T>("Singletons");
                            if (assets == null || assets.Length < 1)
                            {
                                throw new System.Exception("could not generate automaticaly singleton");
                            }
                            Debug.Log("Generated singleton automatically");
#else
                    throw new System.Exception("Could not find any singleton scriptable object instances in the resources!");
#endif
                        }
                        else if (assets.Length > 1)
                        {
                            Debug.LogWarning(assets.Length + " instances of the singleton scriptable object found in cesources, using this", assets[0]);
                            instance = assets[0];
                        }
                        else
                        {
                            instance = assets[0];
                        }
                    }
                    return instance;
                }
            }

#if UNITY_EDITOR
            public static void GenerateSingleton()
            {
                //check folders
                if(AssetDatabase.IsValidFolder("Assets/Resources"))
                {
                    if(!AssetDatabase.IsValidFolder("Assets/Resources/Singletons"))
                    {
                        AssetDatabase.CreateFolder("Assets/Resources", "Singletons");
                    }
                } else 
                { 
                    AssetDatabase.CreateFolder("Assets", "Resources");
                    AssetDatabase.CreateFolder("Assets/Resources", "Singletons");
                }
                string path = "Assets/Resources/Singletons/";

                //create singleton
                Debug.Log(typeof(T));
                T[] assets = Resources.LoadAll<T>("");
                if (!(assets == null || assets.Length < 1)) { Debug.Log("found Instance"); return; }

                if (typeof(T).IsSubclassOf(typeof(SingletonScriptableObject<T>)))
                {
                    var a = ScriptableObject.CreateInstance<T>();

                    AssetDatabase.CreateAsset(a, path + typeof(T).Name + ".asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = a;
                }
            }
            

            protected static void GenerateAllSingletons()
            {
                ICallOnAll.CallOnAll("GenerateSingleton");
            }
#endif
        }

        public interface ICallOnAll
        {
            public static void CallOnAll(string methodName)
            {
                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type type in GetAllImplemented(asm))
                    {
                        Debug.Log(type);
                        MethodInfo generateMethod = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                        if (generateMethod != null)
                            generateMethod.Invoke(null, null);
                        else
                            Debug.Log("Method is null");
                    }
                }
            }

            static IEnumerable<Type> GetAllImplemented(Assembly asm)
            {
                return  from type in asm.GetTypes()
                        where typeof(ICallOnAll).IsAssignableFrom(type)
                        select type;
            }
        }
        
    }
}
