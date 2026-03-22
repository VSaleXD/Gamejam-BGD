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
    [SerializeField] private MonoBehaviour popupUIBehaviour;

    private GameObject spawnedPaper;

    private void Awake()
    {
        if (puzzle1Manager == null)
        {
            puzzle1Manager = FindFirstObjectByType<Puzzle1Manager>();
        }

        TryFindPopupUIBehaviour();
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

        SetupInteractionOnSpawnedPaper();
        RefreshObjectiveText();
    }

    [ContextMenu("Refresh Objective Paper Text")]
    public void RefreshObjectiveText()
    {
        if (spawnedPaper == null)
        {
            return;
        }

        string objectiveText = puzzle1Manager != null
            ? puzzle1Manager.GetObjectiveTextForPaper()
            : "OBJECTIVE\n- Puzzle 1";

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

    private Transform PickSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return null;
        }

        int index = Random.Range(0, spawnPoints.Length);
        return spawnPoints[index];
    }

    private void SetupInteractionOnSpawnedPaper()
    {
        if (spawnedPaper == null)
        {
            return;
        }

        Component interact = spawnedPaper.GetComponent("Puzzle1ObjectivePaperInteract");
        if (interact == null)
        {
            Debug.LogWarning("Puzzle1ObjectivePaperSpawner: Puzzle1ObjectivePaperInteract tidak ditemukan di prefab kertas.");
        }
    }

    private void TryFindPopupUIBehaviour()
    {
        if (popupUIBehaviour != null)
        {
            return;
        }

        MonoBehaviour[] behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] != null && behaviours[i].GetType().Name == "Puzzle1ObjectivePopupUI")
            {
                popupUIBehaviour = behaviours[i];
                return;
            }
        }
    }
}
