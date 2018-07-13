using UnityEngine;
using UnityEditor;

public class WeaponPartsWindow : EditorWindow
{
    WeaponPartsList partList;

    [MenuItem("Data/Weapon part editor")]
    public static void ShowWindow()
    {
        GetWindow<WeaponPartsWindow>("Weapon part editor", true, typeof(WeaponPartsList));
    }

    bool showCreate;
    void OnGUI()
    {
        if (partList == null)
            partList = GetWindow<WeaponPartsList>();

        showCreate = EditorGUILayout.Foldout(showCreate, "New");
        if (showCreate)
        {
            CreatePart();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
        }

        EditPart();
    }


    string newPartShortCode, newPartName;
    GameObject newPartPrefab;
    WeaponPartType newPartType;
    void CreatePart()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(AssetPreview.GetAssetPreview(newPartPrefab));

        EditorGUILayout.BeginVertical();

        GUILayout.Label("Create part");

        newPartShortCode = EditorGUILayout.TextField("Short code", newPartShortCode);
        newPartName = EditorGUILayout.TextField("Part name", newPartName);
        newPartType = (WeaponPartType)EditorGUILayout.EnumPopup("Type", newPartType);
        newPartPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", newPartPrefab, typeof(GameObject), false);

        if (GUILayout.Button("Create part"))
        {
            WeaponPart part = CreateInstance<WeaponPart>();
            part.stortCode = newPartShortCode;
            part.partName = newPartName;
            part.prefab = newPartPrefab;
            part.partType = newPartType;

            AssetDatabase.CreateAsset(part, "Assets/Resources/WeaponParts/" + newPartShortCode + ".asset");
            partList.Repaint();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
    void EditPart()
    {
        WeaponPart selected = partList.selected;

        if (selected == null)
            return;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(AssetPreview.GetAssetPreview(selected.prefab));

        EditorGUILayout.BeginVertical();

        GUILayout.Label("Edit part");
        GUILayout.Label("Short code: " + selected.stortCode);
        selected.partName = EditorGUILayout.TextField("Part name", selected.partName);
        selected.partType = (WeaponPartType)EditorGUILayout.EnumPopup("Type", selected.partType);
        selected.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", selected.prefab, typeof(GameObject), false);

        if (GUILayout.Button("Delete part"))
        {
            AssetDatabase.DeleteAsset("Assets/Resources/WeaponParts/" + selected.stortCode + ".asset");
            partList.Repaint();
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
}