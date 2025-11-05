using UnityEngine;
using TMPro;

using UnityEngine.UI;
using System.Collections.Generic;
using System.Reflection;
using System;
public class UIController : MonoBehaviour
{
    public List<UISaveLoadWrapper> SaveLoadWrappers;

    [System.Serializable]
    public class UISaveLoadWrapper
    {
        public MonoBehaviour TargetClass;
        public string TargetVariableName;
        public Selectable UIComponent;
        public string SaveLoadKey;
    }

   
    [ContextMenu("UpdateAll")]

    public void UpdateAll()
    {
        for (int i = 0; i < SaveLoadWrappers.Count; i++)
        {
            System.Type type = SaveLoadWrappers[i].TargetClass.GetType();
            FieldInfo info = type.GetField(SaveLoadWrappers[i].TargetVariableName);

            Debug.Log(info.GetValue(SaveLoadWrappers[i].TargetClass));
        }
    }


}
