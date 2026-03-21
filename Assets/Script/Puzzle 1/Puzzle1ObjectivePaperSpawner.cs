using UnityEngine;
using UnityEngine.UI;

public class Puzzle1ObjectivePaperSpawner : MonoBehaviour
{
    [Header("Objective Paper")]
    [SerializeField] private GameObject objectivePaperPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private bool spawnOnStart = true;

    [Header("Optional")]
    [SerializeField] private Transform spawnedPaperRoot;
    [SerializeField] private Puzzle1Manager puzzle1Manager;

    private GameObject spawnedPaper;

    private void Awake()
    {
        if (puzzle1Manager == null)
        {
            puzzle1Manager = FindFirstObjectByType<Puzzle1Manager>();
        }
    }

    private void Start()
    {
        if (spawnOnStart)
        {
            PrepareForRound(1);
        }
    }

    public void PrepareForRound(int roundNumber)
    {
        if (objectivePaperPrefab == null)
        {
            Debug.LogWarning("Puzzle1ObjectivePaperSpawner: objectivePaperPrefab belum di-assign.");
            return;
        }

        Transform selectedSpawn = PickSpawnPoint();
        if (selectedSpawn == null)
        {
            Debug.LogWarning("Puzzle1ObjectivePaperSpawner: spawnPoints masih kosong.");
            return;
        }

        if (spawnedPaper != null)
        {
            if (Application.isPlaying)
            {
                Destroy(spawnedPaper);
            }
            else
            {
                DestroyImmediate(spawnedPaper);
            }
        }

        Transform parent = spawnedPaperRoot != null ? spawnedPaperRoot : transform;
        spawnedPaper = Instantiate(objectivePaperPrefab, selectedSpawn.position, selectedSpawn.rotation, parent);

        RefreshObjectiveText();
    }

    [ContextMenu("Refresh Objective Paper Text")]
    public void RefreshObjectiveText()
    {
        if (spawnedPaper == null)
        {
            return;
        }

        string objectiveText = BuildObjectiveText();

        Text uiText = spawnedPaper.GetComponentInChildren<Text>(true);
        if (uiText != null)
        {
            uiText.text = objectiveText;
            return;
        }

        TextMesh worldText = spawnedPaper.GetComponentInChildren<TextMesh>(true);
        if (worldText != null)
        {
            worldText.text = objectiveText;
            return;
        }

        Debug.LogWarning("Puzzle1ObjectivePaperSpawner: Tidak ada komponen Text atau TextMesh pada prefab kertas.");
    }

    private string BuildObjectiveText()
    {
        if (puzzle1Manager == null)
        {
            return "OBJECTIVE\n- Puzzle 1";
        }

        string[] requiredItems = puzzle1Manager.GetRequiredItemsSnapshot();
        if (requiredItems == null || requiredItems.Length == 0)
        {
            return "OBJECTIVE\n- Tidak ada item";
        }

        string text = "OBJECTIVE SUBMIT:\n";

        for (int i = 0; i < requiredItems.Length; i++)
        {
            text += "- " + requiredItems[i] + "\n";
        }

        return text.TrimEnd();
    }

    private Transform PickSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return null;
        }

        int index = Random.Range(0, spawnPoints.Length);
        return spawnPoints[index];
    }
}
