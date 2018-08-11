using UnityEditor;
using UnityEngine;

using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class WeaponPartsList : EditorWindow
{
    WeaponPartsWindow weaponPartsWindow;

    public WeaponPart selected;
    int selectedIndex;

    Vector2 scroll;

    [MenuItem("Data/Weapon parts list")]
    public static void ShowWindow()
    {
        GetWindow<WeaponPartsList>("Weapon part list", true, typeof(WeaponPartsWindow));
    }

    string searchText = "";

    void OnGUI()
    {
        if (weaponPartsWindow == null)
            weaponPartsWindow = GetWindow<WeaponPartsWindow>();

        searchText = EditorGUILayout.TextField(searchText);

        WeaponPart[] parts = Resources.LoadAll<WeaponPart>("WeaponParts");

        if (parts.Length == 0)
            return;

        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.ExpandWidth(false));

        string[] names = parts
            .Where(p => p.stortCode.ToLower().Contains(searchText.ToLower()) || 
                        p.name.ToLower().Contains(searchText.ToLower()))
            .Select(p => "(" + p.stortCode + ") " + p.name)
            .ToArray();

        selectedIndex = GUILayout.SelectionGrid(selectedIndex, names, 1);

        if (selectedIndex >= parts.Length)
            selectedIndex = parts.Length - 1;

        if (selected != parts[selectedIndex])
        {
            selected = parts[selectedIndex];
            weaponPartsWindow.Repaint();
        }

        EditorGUILayout.EndScrollView();
    }
}