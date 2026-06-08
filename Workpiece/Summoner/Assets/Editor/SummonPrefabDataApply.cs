using System;
using UnityEditor;
using UnityEngine;

public static class SummonPrefabDataApply
{
    [MenuItem("Tools/Summoner/Apply Spirit SummonData")]
    public static void SpiritSummonDataApply()
    {
        PrefabSummonDataApply(
            "Assets/Prefabs/SummonPrefab/Water Spirit.prefab",
            "WaterSpirit",
            "Assets/Script/Summons/Data/WaterSpiritSummonData.asset");

        PrefabSummonDataApply(
            "Assets/Prefabs/SummonPrefab/Grass Spirit.prefab",
            "GrassSpirit",
            "Assets/Script/Summons/Data/GrassSpiritSummonData.asset");

        AssetDatabase.SaveAssets();
        Debug.Log("Spirit SummonData prefab references applied.");
    }

    private static void PrefabSummonDataApply(string prefabPath, string componentName, string summonDataPath)
    {
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);

        try
        {
            MonoBehaviour targetComponent = ComponentFindByName(prefabRoot, componentName);
            UnityEngine.Object summonData = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(summonDataPath);

            if (targetComponent == null)
            {
                throw new MissingComponentException($"{componentName} component was not found in {prefabPath}.");
            }

            if (summonData == null)
            {
                throw new MissingReferenceException($"{summonDataPath} was not found.");
            }

            SerializedObject serializedComponent = new SerializedObject(targetComponent);
            SerializedProperty summonDataProperty = serializedComponent.FindProperty("summonData");

            if (summonDataProperty == null)
            {
                throw new MissingFieldException($"{componentName}.summonData was not found.");
            }

            summonDataProperty.objectReferenceValue = summonData;
            serializedComponent.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }
    }

    private static MonoBehaviour ComponentFindByName(GameObject root, string componentName)
    {
        MonoBehaviour[] components = root.GetComponentsInChildren<MonoBehaviour>(true);

        foreach (MonoBehaviour component in components)
        {
            if (component != null && component.GetType().Name == componentName)
            {
                return component;
            }
        }

        return null;
    }
}
