using UnityEngine;

public class FloorGridBuilder : MonoBehaviour
{
    [Header("Grid Source")]
    [SerializeField] private GameObject tilePrefab;

    [Header("Grid Size")]
    [SerializeField] private int columns = 6;
    [SerializeField] private int rows = 6;
    [SerializeField] private Vector2 spacing = new Vector2(1f, 1f);

    [Header("Placement")]
    [SerializeField] private Vector2 startOffset = Vector2.zero;
    [SerializeField] private bool centerGridOnThisObject = true;

    [Header("Build Options")]
    [SerializeField] private bool clearChildrenBeforeBuild = true;
    [SerializeField] private string generatedNamePrefix = "tile_";

    [ContextMenu("Build Grid")]
    public void BuildGrid()
    {
        if (tilePrefab == null)
        {
            Debug.LogWarning("FloorGridBuilder: tilePrefab belum di-assign.");
            return;
        }

        if (columns <= 0 || rows <= 0)
        {
            Debug.LogWarning("FloorGridBuilder: columns dan rows harus lebih dari 0.");
            return;
        }

        if (clearChildrenBeforeBuild)
        {
            ClearChildren();
        }

        Vector2 origin = startOffset;

        if (centerGridOnThisObject)
        {
            float width = (columns - 1) * spacing.x;
            float height = (rows - 1) * spacing.y;
            origin += new Vector2(-width * 0.5f, -height * 0.5f);
        }

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 localPos = new Vector3(origin.x + x * spacing.x, origin.y + y * spacing.y, 0f);
                GameObject tile = InstantiateTile(localPos);
                if (tile != null)
                {
                    tile.name = generatedNamePrefix + y + "_" + x;
                }
            }
        }

        Debug.Log("Grid berhasil dibuat: " + columns + "x" + rows);
    }

    [ContextMenu("Clear Generated Children")]
    public void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.Undo.DestroyObjectImmediate(child.gameObject);
                continue;
            }
#endif

            Destroy(child.gameObject);
        }
    }

    private GameObject InstantiateTile(Vector3 localPos)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            GameObject editorObj = UnityEditor.PrefabUtility.InstantiatePrefab(tilePrefab, transform) as GameObject;
            if (editorObj != null)
            {
                editorObj.transform.localPosition = localPos;
                editorObj.transform.localRotation = Quaternion.identity;
                editorObj.transform.localScale = Vector3.one;
                UnityEditor.Undo.RegisterCreatedObjectUndo(editorObj, "Build Floor Grid");
            }

            return editorObj;
        }
#endif

        GameObject runtimeObj = Instantiate(tilePrefab, transform);
        runtimeObj.transform.localPosition = localPos;
        runtimeObj.transform.localRotation = Quaternion.identity;
        runtimeObj.transform.localScale = Vector3.one;
        return runtimeObj;
    }
}
