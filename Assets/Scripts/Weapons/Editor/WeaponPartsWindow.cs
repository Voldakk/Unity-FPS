using UnityEngine;
using UnityEditor;
using System.IO;

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

            WeaponPart part;

            switch (newPartType)
            {
                case WeaponPartType.Barrel:
                    part = CreateInstance<Barrel>();
                    break;
                case WeaponPartType.Body:
                    part = CreateInstance<Body>();
                    break;
                case WeaponPartType.Grip:
                    part = CreateInstance<Grip>();
                    break;
                case WeaponPartType.Mag:
                    part = CreateInstance<Mag>();
                    break;
                case WeaponPartType.Sight:
                    part = CreateInstance<Sight>();
                    break;
                case WeaponPartType.Stock:
                    part = CreateInstance<Stock>();
                    break;
                case WeaponPartType.Muzzle:
                    part = CreateInstance<Muzzle>();
                    break;
                default:
                    part = CreateInstance<WeaponPart>();
                    break;
            }

            part.stortCode = newPartShortCode;
            part.partName = newPartName;
            part.prefab = newPartPrefab;
            part.partType = newPartType;

            part.icon = GetPrefabIcon(newPartPrefab);

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
        EditorGUILayout.ObjectField("Sprite", selected.icon, typeof(Sprite), allowSceneObjects: true);

        EditorGUILayout.BeginVertical();

        GUILayout.Label("Edit part");
        GUILayout.Label("Short code: " + selected.stortCode);
        selected.partName = EditorGUILayout.TextField("Part name", selected.partName);
        GUILayout.Label("Part type: " + selected.partType.ToString());
        //selected.partType = (WeaponPartType)EditorGUILayout.EnumPopup("Type", selected.partType);

        GameObject newPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", selected.prefab, typeof(GameObject), false);
        if(selected.prefab != newPrefab)
        {
            //AssetDatabase.StartAssetEditing();
            selected.prefab = newPrefab;
            selected.icon = GetPrefabIcon(newPrefab);
            //AssetDatabase.StopAssetEditing();
            EditorUtility.SetDirty(selected);
        }

        if (GUILayout.Button("Update icon"))
        {
            //AssetDatabase.StartAssetEditing();
            selected.icon = GetPrefabIcon(selected.prefab, true);
            //AssetDatabase.StopAssetEditing();
            EditorUtility.SetDirty(selected);
        }

        if (GUILayout.Button("Delete part"))
        {
            AssetDatabase.DeleteAsset("Assets/Resources/WeaponParts/" + selected.stortCode + ".asset");
            partList.Repaint();
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    Sprite GetPrefabIcon(GameObject prefab, bool reset = false)
    {
        string prefabPath = AssetDatabase.GetAssetPath(prefab);

        string path = Path.GetDirectoryName(prefabPath) + "/" + Path.GetFileNameWithoutExtension(prefabPath);

        Sprite icon = AssetDatabase.LoadAssetAtPath<Sprite>(path + "_icon.asset");
        if (icon != null && !reset)
            return icon;

        Texture2D texture = AssetPreview.GetAssetPreview(prefab);
        File.WriteAllBytes(path + "_icon_texture.png", texture.EncodeToPNG());
        AssetDatabase.Refresh();
        texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path + "_icon_texture.png");

        icon = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.zero);
        AssetDatabase.CreateAsset(icon, path + "_icon.asset");

        return icon;
    }
}