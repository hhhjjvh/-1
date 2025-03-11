#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;

public class FontAssetChanger : MonoBehaviour
{
    // 将字体拖到此处，或在 Inspector 中分配
    public TMP_FontAsset newFontAsset;

    [ContextMenu("Change All TextMesh Pro Fonts in Scene and Prefabs")]
    public void ChangeAllTextMeshProFonts()
    {
        if (newFontAsset == null)
        {
            Debug.LogError("No Font Asset assigned! Please assign a TMP_FontAsset.");
            return;
        }

        // 修改场景中的 TextMeshPro 组件
        ChangeFontInScene();

        // 修改所有预制体中的 TextMeshPro 组件
        ChangeFontInPrefabs();

        Debug.Log("Font asset update completed for both scene objects and prefabs.");
    }

    private void ChangeFontInScene()
    {
        // 修改场景中的 TextMeshProUGUI 组件
        TextMeshProUGUI[] textUIComponents = FindObjectsOfType<TextMeshProUGUI>();
        foreach (var textUI in textUIComponents)
        {
            textUI.font = newFontAsset;
        }

        // 修改场景中的 TextMeshPro 组件
        TextMeshPro[] textComponents = FindObjectsOfType<TextMeshPro>();
        foreach (var text in textComponents)
        {
            text.font = newFontAsset;
        }

        Debug.Log($"Updated Font Asset for {textUIComponents.Length + textComponents.Length} TextMesh Pro components in the scene.");
    }

    private void ChangeFontInPrefabs()
    {
        // 获取项目中所有的预制体路径
        string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab");
        int updatedCount = 0;

        foreach (string guid in prefabPaths)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            // 查找预制体中的 TextMeshProUGUI 和 TextMeshPro 组件
            TextMeshProUGUI[] textUIComponents = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);
            TextMeshPro[] textComponents = prefab.GetComponentsInChildren<TextMeshPro>(true);

            bool prefabModified = false;

            foreach (var textUI in textUIComponents)
            {
                if (textUI.font != newFontAsset)
                {
                    textUI.font = newFontAsset;
                    prefabModified = true;
                }
            }

            foreach (var text in textComponents)
            {
                if (text.font != newFontAsset)
                {
                    text.font = newFontAsset;
                    prefabModified = true;
                }
            }

            // 如果预制体有修改，则保存
            if (prefabModified)
            {
                EditorUtility.SetDirty(prefab);
                updatedCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Updated Font Asset for {updatedCount} prefabs.");
    }
}
#endif
