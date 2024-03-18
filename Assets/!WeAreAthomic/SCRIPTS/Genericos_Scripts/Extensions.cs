using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions
{
    public static T GetRandom<T>(List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }    
    
    public static T GetObject<T>(List<T> list, T obj)
    {
        return list.FirstOrDefault(x => x.Equals(obj));
    }  
   
}
