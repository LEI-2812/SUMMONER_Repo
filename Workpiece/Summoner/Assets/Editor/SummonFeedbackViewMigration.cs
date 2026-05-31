#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class SummonFeedbackViewMigration
{
    private const string SummonPrefabFolder = "Assets/Prefabs/SummonPrefab";

    [MenuItem("Tools/SUMMONER/Migrate Summon Feedback Views")]
    public static void Migrate()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { SummonPrefabFolder });
        int changedCount = 0;

        foreach (string guid in prefabGuids)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);

            try
            {
                bool changed = false;

                foreach (Summon summon in root.GetComponentsInChildren<Summon>(true))
                {
                    changed |= MigrateSummon(summon);
                }

                if (changed)
                {
                    PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                    changedCount++;
                }
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Summon feedback view migration finished. Changed prefabs: {changedCount}");
    }

    [MenuItem("Tools/SUMMONER/Reserialize Summon Prefabs")]
    public static void ReserializeSummonPrefabs()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { SummonPrefabFolder });
        string[] prefabPaths = new string[prefabGuids.Length];

        for (int i = 0; i < prefabGuids.Length; i++)
        {
            prefabPaths[i] = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
        }

        AssetDatabase.ForceReserializeAssets(prefabPaths);
        AssetDatabase.SaveAssets();
        Debug.Log($"Summon prefab reserialize finished. Prefabs: {prefabPaths.Length}");
    }

    private static bool MigrateSummon(Summon summon)
    {
        SerializedObject summonObject = new SerializedObject(summon);
        SerializedProperty image = summonObject.FindProperty("image");
        SerializedProperty sprites = summonObject.FindProperty("sprites");
        SerializedProperty attackSound = summonObject.FindProperty("attackSound");
        SerializedProperty downHitSound = summonObject.FindProperty("downHitSound");
        SerializedProperty upAttackSound = summonObject.FindProperty("upAttackSound");

        bool changed = false;

        SummonImageView imageView = summon.GetComponent<SummonImageView>();
        if (imageView == null)
        {
            imageView = summon.gameObject.AddComponent<SummonImageView>();
            changed = true;
        }

        changed |= CopyImageFields(imageView, image, sprites);

        SummonSoundView soundView = summon.GetComponent<SummonSoundView>();
        if (soundView == null)
        {
            soundView = summon.gameObject.AddComponent<SummonSoundView>();
            changed = true;
        }

        changed |= CopySoundFields(soundView, attackSound, downHitSound, upAttackSound);

        return changed;
    }

    private static bool CopyImageFields(SummonImageView imageView, SerializedProperty image, SerializedProperty sprites)
    {
        SerializedObject imageViewObject = new SerializedObject(imageView);
        bool changed = false;

        changed |= CopyObjectReference(image, imageViewObject.FindProperty("image"));
        changed |= CopyObjectReferenceArray(sprites, imageViewObject.FindProperty("sprites"));

        if (changed)
        {
            imageViewObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(imageView);
        }

        return changed;
    }

    private static bool CopySoundFields(
        SummonSoundView soundView,
        SerializedProperty attackSound,
        SerializedProperty downHitSound,
        SerializedProperty upAttackSound)
    {
        SerializedObject soundViewObject = new SerializedObject(soundView);
        bool changed = false;

        changed |= CopyObjectReference(attackSound, soundViewObject.FindProperty("attackSound"));
        changed |= CopyObjectReference(downHitSound, soundViewObject.FindProperty("debuffSound"));
        changed |= CopyObjectReference(upAttackSound, soundViewObject.FindProperty("buffSound"));

        if (changed)
        {
            soundViewObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(soundView);
        }

        return changed;
    }

    private static bool CopyObjectReference(SerializedProperty source, SerializedProperty target)
    {
        if (source == null || target == null)
        {
            return false;
        }

        if (target.objectReferenceValue == source.objectReferenceValue)
        {
            return false;
        }

        target.objectReferenceValue = source.objectReferenceValue;
        return true;
    }

    private static bool CopyObjectReferenceArray(SerializedProperty source, SerializedProperty target)
    {
        if (source == null || target == null || source.isArray == false || target.isArray == false)
        {
            return false;
        }

        bool changed = false;

        if (target.arraySize != source.arraySize)
        {
            target.arraySize = source.arraySize;
            changed = true;
        }

        for (int i = 0; i < source.arraySize; i++)
        {
            SerializedProperty sourceElement = source.GetArrayElementAtIndex(i);
            SerializedProperty targetElement = target.GetArrayElementAtIndex(i);

            if (targetElement.objectReferenceValue == sourceElement.objectReferenceValue)
            {
                continue;
            }

            targetElement.objectReferenceValue = sourceElement.objectReferenceValue;
            changed = true;
        }

        return changed;
    }
}
#endif
