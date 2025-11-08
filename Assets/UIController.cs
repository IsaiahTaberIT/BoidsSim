using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static PersonalHelpers.Processing;








public class UIController : MonoBehaviour
{
    public Dictionary<UnityEngine.Object, int> ObjectIndicies = new Dictionary<UnityEngine.Object, int>();

    public List<UISaveLoadWrapper> SaveLoadWrappers;

    [System.Serializable]
    public class UISaveLoadWrapper
    {
        //  public int Index = -1;

        public string m_TargetValue;

        public string TargetValue {
            get
            {
                string potential = TryGetTargetValue();

                if (potential != null)
                {
                    m_TargetValue = potential;

                }
                else
                {
                    Debug.LogWarning("Aborting Get. Target Value came back null. have you used the right field name?");
                }

                return m_TargetValue;
            }

            set
            {
                m_TargetValue = value;
            }
        }


        public MonoBehaviour TargetClass;
        public string TargetVariableName;
        public UnityEngine.Object UIComponent;
        //  public ComponentType UIComponentType;
        public string SaveLoadKey;
        public enum ComponentType
        {
            Input_Field_TMP_Pro = 0,
        }
        string TryGetTargetValue()
        {
            System.Type type = TargetClass.GetType();
            FieldInfo info = type.GetField(TargetVariableName);
            return info.GetValue(TargetClass).ToString();
        }

        public string GetUIValue()
        {
            if (UIComponent == null)
            {
                Debug.LogWarning("UI Compnent is null and cannot return a value");
                return null;

            }
            else if (UIComponent is GameObject g)
            {
                if (g.TryGetComponent<TMP_InputField>(out TMP_InputField t))
                {
                    return t.text;
                }
            }

            return null;
        }


        public string GetSavedValue()
        {
            return PlayerPrefs.GetString(SaveLoadKey, TargetValue);
        }
    }


    bool TryFindIndex(UnityEngine.Object obj, ref int index)
    {
        for (int i = 0; i < SaveLoadWrappers.Count; i++)
        {
            if (SaveLoadWrappers[i].UIComponent == obj)
            {
                ObjectIndicies.Add(obj, i);
                index = i;
                return true;
            }
            else if (i == SaveLoadWrappers.Count - 1)
            {
                Debug.LogWarning("Unable to find object reference");
                break;
            }
        }

        return false;

    }


    void ParseStringToValueOfType(string str, int index)
    {
        System.Type type = SaveLoadWrappers[index].TargetClass.GetType();
        FieldInfo infoF = type.GetField(SaveLoadWrappers[index].TargetVariableName);
        PropertyInfo infoP = type.GetProperty(SaveLoadWrappers[index].TargetVariableName);

        string name = "";
        MemberInfo info = null;



        if (infoF != null)
        {
            name = infoF.FieldType.Name;
            info = infoF;
        }
        else if (infoP != null)
        {
            name = infoP.PropertyType.Name;
            ;
            info = infoP;

        }
        else
        {
            Debug.LogWarning("Member not a property or a field");

            return;
        }

        Debug.Log(name);


            switch (name)
            {
                case "String":
                    info.SetValueWrapper(SaveLoadWrappers[index].TargetClass, str);
                    break;
                case "Single":
                    if (float.TryParse(str, out float sValue))
                    {
                        info.SetValueWrapper(SaveLoadWrappers[index].TargetClass, sValue);
                    }
                    else
                    {
                        Debug.Log("string is in incorrect format");
                    }

                    break;

                case "Int32":
                    if (int.TryParse(str, out int iValue))
                    {
                        info.SetValueWrapper(SaveLoadWrappers[index].TargetClass, iValue);
                    }
                    else
                    {
                        Debug.Log("string is in incorrect format");
                    }

                    break;

                case "Double":

                    if (double.TryParse(str, out double dValue))
                    {
                        info.SetValueWrapper(SaveLoadWrappers[index].TargetClass, dValue);
                    }
                    else
                    {
                        Debug.Log("string is in incorrect format");
                    }
                    break;
                default:
                    break;
            }
    }





    




    public void SetValue(UnityEngine.Object obj)
    {
        int index = -1;

        if (ObjectIndicies.TryGetValue(obj,out index))
        {
            if (index < SaveLoadWrappers.Count)
            {
                if (SaveLoadWrappers[index].UIComponent != obj)
                {
                    ObjectIndicies.Remove(obj);

                    if (!TryFindIndex(obj,ref index))
                    {
                        return;
                    }
                }

            }
            else
            {
                if (!TryFindIndex(obj, ref index))
                {
                    Debug.LogWarning("index out of range of collection");
                    return;
                }
            }

          

           // SaveLoadWrappers[index]
        }
        else
        {
            if (!TryFindIndex(obj, ref index))
            {
                return;
            }
        }

        ParseStringToValueOfType(SaveLoadWrappers[index].GetUIValue(),index);

    }







    [ContextMenu("UpdateAll")]

    public void UpdateAll()
    {
        for (int i = 0; i < SaveLoadWrappers.Count; i++)
        {
            System.Type type = SaveLoadWrappers[i].TargetClass.GetType();
            FieldInfo info = type.GetField(SaveLoadWrappers[i].TargetVariableName);
            Debug.Log(info.GetValue(SaveLoadWrappers[i].TargetClass));
            Debug.Log(SaveLoadWrappers[i].GetUIValue());
            Debug.Log(SaveLoadWrappers[i].GetSavedValue());

        }
    }


}
