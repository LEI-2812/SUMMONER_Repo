using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StageEnemyPlacement))]
public class StageEnemyPlacementPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty stage = property.FindPropertyRelative("stage");
        SerializedProperty slots = property.FindPropertyRelative("enemyPlacementSlots");

        label.text = "Stage " + stage.intValue + " enemy placements (" + slots.arraySize + ")";
        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}

[CustomPropertyDrawer(typeof(EnemyPlacementSlot))]
public class EnemyPlacementSlotPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty plateIndex = property.FindPropertyRelative("plateIndex");
        SerializedProperty enemySummonPrefab = property.FindPropertyRelative("enemySummonPrefab");

        string enemyName = enemySummonPrefab.objectReferenceValue == null
            ? "None"
            : enemySummonPrefab.objectReferenceValue.name;

        label.text = "Plate " + plateIndex.intValue + " - " + enemyName;
        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
