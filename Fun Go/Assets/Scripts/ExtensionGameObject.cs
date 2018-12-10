using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Extend Game Object for finding Inactive GameObject
// Ex : parentObject.FindObject("anyInactiveGameObjectName");
// Source : https://forum.unity.com/threads/c-extension-methods-on-gameobject.183491/
namespace ExtensionMethods
{
    internal static class ExtendGameObject
    {
        public static GameObject FindObject(this GameObject parent, string name)
        {
            Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in trs)
            {
                if (t.name == name)
                {
                    return t.gameObject;
                }
            }
            return null;
        }
    }
}
