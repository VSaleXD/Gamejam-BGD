using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class game : MonoBehaviour
{
    public static game Instance { get; private set; }

    private static bool persistentRunInitialized;
    private static int persistentFloorNumber = 1;
    private static int persistentSceneCycleIndex;

    [Header("Wajib - Puzzle Scene Ini")]
    [Tooltip("Isi dengan manager puzzle di scene ini (harus implement IPuzzleRound), contoh: Puzzle1Manager atau Puzzle2Manager.")]
    [SerializeField] private MonoBehaviour scenePuzzleController;

    [Header("Opsional - Sistem Tambahan")]
    [Tooltip("Kosongkan jika scene ini tidak pakai room generator runtime.")]
    [SerializeField] private roomBuilder roomGenerator;
    [Tooltip("Kosongkan jika scene ini tidak pakai pressure/retak lantai.")]
    [SerializeField] private FloorPressureManager floorPressureManager;
    [Tooltip("Kosongkan jika scene ini tidak pakai objective paper Puzzle 1.")]
    [SerializeField] private MonoBehaviour puzzle1ObjectivePaperSpawnerBehaviour;

    [Header("Endless - Pindah Scene")]
    [Tooltip("Aktifkan untuk mode endless antar scene puzzle.")]
    [SerializeField] private bool endlessMode = true;
    [Tooltip("Urutan scene endless. Nama harus sama persis dengan scene di Build Settings.")]
    [SerializeField] private string[] endlessSceneNames;

    [Header("Restart")]
    [Tooltip("Tekan R untuk restart run dari lantai 1.")]
    [SerializeField] private bool allowRestartWithR = true;

    [Header("UI")]
    [Tooltip("Panel game over yang muncul saat kalah.")]
    [SerializeField] private GameObject gameOverScreen;

    private IPuzzleRound cachedScenePuzzle;

    public int RoundNumber => persistentFloorNumber;
    public int CurrentFloorNumber => persistentFloorNumber;
    public int CurrentPuzzleIndex => persistentSceneCycleIndex;
    public string CurrentPuzzleName => GetCurrentPuzzleName();

    public enum GameState
    {
        Playing,
        Win,
        Lose
    }

    public GameState CurrentState { get; private set; } = GameState.Playing;
    public FloorPressureManager FloorPressure => floorPressureManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Lebih dari satu game manager ditemukan di scene.");
        }

        Instance = this;

        if (scenePuzzleController != null)
        {
            cachedScenePuzzle = scenePuzzleController as IPuzzleRound;
        }

        if (roomGenerator == null)
        {
            roomGenerator = GetComponent<roomBuilder>();
        }

        if (floorPressureManager == null)
        {
            floorPressureManager = GetComponent<FloorPressureManager>();
        }

        if (puzzle1ObjectivePaperSpawnerBehaviour == null)
        {
            MonoBehaviour[] behaviours = GetComponents<MonoBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] == this)
                {
                    continue;
                }

                if (behaviours[i].GetType().Name == "Puzzle1ObjectivePaperSpawner")
                {
                    puzzle1ObjectivePaperSpawnerBehaviour = behaviours[i];
                    break;
                }
            }
        }

        if (cachedScenePuzzle == null)
        {
            cachedScenePuzzle = FindScenePuzzleController();
        }

        if (cachedScenePuzzle == null)
        {
            Debug.LogWarning("Scene puzzle controller belum valid. Isi field Scene Puzzle Controller dengan script yang implement IPuzzleRound.");
        }

        ResolveGameOverScreenIfMissing();
    }

    private void OnValidate()
    {
        if (scenePuzzleController != null && !(scenePuzzleController is IPuzzleRound))
        {
            Debug.LogWarning("Scene Puzzle Controller harus implement IPuzzleRound.");
        }
    }

    private void Start()
    {
        ResolveGameOverScreenIfMissing();
        Time.timeScale = 1f;
        SetGameOverScreenVisible(false);
        InitializePersistentRunIfNeeded();
        BeginCurrentSceneRound();
    }

    private void Update()
    {
        // ⭐ LOCK INPUT SAAT POPUP / UI
        if (GameInputLock.InputLocked)
        {
            return;
        }

        if (!allowRestartWithR || Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartRun();
        }
    }

    public void WinRun()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        if (!endlessMode)
        {
            CurrentState = GameState.Win;
            Debug.Log("WIN: objective selesai dan berhasil keluar.");
            return;
        }

        persistentFloorNumber++;

        if (endlessSceneNames != null && endlessSceneNames.Length > 0)
        {
            persistentSceneCycleIndex = GetNextSceneIndex();
            string nextSceneName = endlessSceneNames[persistentSceneCycleIndex];

            if (TryLoadSceneByName(nextSceneName))
            {
                Debug.Log("Lantai selesai. Pindah ke scene berikutnya: " + nextSceneName);
                return;
            }

            Debug.LogWarning("Nama scene tidak valid / belum masuk Build Settings: " + nextSceneName + ". Fallback ke scene enabled berikutnya.");
        }

        if (TryLoadNextEnabledBuildScene())
        {
            return;
        }

        Debug.Log("Lantai selesai, tapi tidak ada scene tujuan valid. Lanjut round di scene yang sama.");
        BeginCurrentSceneRound();
    }

    public void LoseRun()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        CurrentState = GameState.Lose;

        if (floorPressureManager != null)
        {
            floorPressureManager.StopPressure();
        }

        Time.timeScale = 0f;
        SetGameOverScreenVisible(true);
        Debug.Log("LOSE: player gagal bertahan.");
    }

    public void RestartRun()
    {
        Time.timeScale = 1f;
        ResetPersistentRun();

        if (endlessSceneNames != null && endlessSceneNames.Length > 0)
        {
            if (TryLoadSceneByName(endlessSceneNames[0]))
            {
                return;
            }
        }

        if (TryLoadFirstEnabledBuildScene())
        {
            return;
        }

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public bool IsCurrentPuzzleCompleted()
    {
        return cachedScenePuzzle == null || cachedScenePuzzle.IsCompleted;
    }

    private void InitializePersistentRunIfNeeded()
    {
        if (!persistentRunInitialized)
        {
            persistentRunInitialized = true;
            persistentFloorNumber = 1;
        }

        persistentSceneCycleIndex = ResolveCurrentSceneCycleIndex();
    }

    private void BeginCurrentSceneRound()
    {
        Time.timeScale = 1f;
        CurrentState = GameState.Playing;
        SetGameOverScreenVisible(false);

        if (cachedScenePuzzle == null)
        {
            cachedScenePuzzle = FindScenePuzzleController();
        }

        if (roomGenerator != null)
        {
            roomGenerator.PrepareForRound(persistentFloorNumber);
        }

        if (cachedScenePuzzle != null)
        {
            cachedScenePuzzle.ResetPuzzleRound();
            cachedScenePuzzle.BeginPuzzleRound();
        }

        if (puzzle1ObjectivePaperSpawnerBehaviour != null)
        {
            puzzle1ObjectivePaperSpawnerBehaviour.SendMessage("PrepareForRound", persistentFloorNumber, SendMessageOptions.DontRequireReceiver);
        }

        if (floorPressureManager != null)
        {
            floorPressureManager.PrepareForRound(persistentFloorNumber);
        }

        Debug.Log("Mulai Lantai " + persistentFloorNumber + " - " + GetCurrentPuzzleName());
    }

    private int ResolveCurrentSceneCycleIndex()
    {
        if (endlessSceneNames == null || endlessSceneNames.Length == 0)
        {
            return 0;
        }

        string activeSceneName = SceneManager.GetActiveScene().name;

        for (int i = 0; i < endlessSceneNames.Length; i++)
        {
            if (endlessSceneNames[i] == activeSceneName)
            {
                return i;
            }
        }

        return 0;
    }

    private int GetNextSceneIndex()
    {
        if (endlessSceneNames == null || endlessSceneNames.Length == 0)
        {
            return 0;
        }

        return (persistentSceneCycleIndex + 1) % endlessSceneNames.Length;
    }

    private string GetCurrentPuzzleName()
    {
        if (endlessSceneNames != null && endlessSceneNames.Length > 0)
        {
            return endlessSceneNames[persistentSceneCycleIndex];
        }

        return SceneManager.GetActiveScene().name;
    }

    private IPuzzleRound FindScenePuzzleController()
    {
        MonoBehaviour[] behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for (int i = 0; i < behaviours.Length; i++)
        {
            IPuzzleRound puzzleRound = behaviours[i] as IPuzzleRound;
            if (puzzleRound != null)
            {
                return puzzleRound;
            }
        }

        return null;
    }

    private bool TryLoadSceneByName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            return false;
        }

        int total = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < total; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = Path.GetFileNameWithoutExtension(path);
            if (name == sceneName)
            {
                SceneManager.LoadScene(sceneName);
                return true;
            }
        }

        return false;
    }

    private bool TryLoadNextEnabledBuildScene()
    {
        int total = SceneManager.sceneCountInBuildSettings;
        if (total <= 0)
        {
            return false;
        }

        int current = SceneManager.GetActiveScene().buildIndex;
        if (current < 0)
        {
            return false;
        }

        int next = (current + 1) % total;
        SceneManager.LoadScene(next);
        return true;
    }

    private bool TryLoadFirstEnabledBuildScene()
    {
        int total = SceneManager.sceneCountInBuildSettings;
        if (total <= 0)
        {
            return false;
        }

        SceneManager.LoadScene(0);
        return true;
    }

    private void ResetPersistentRun()
    {
        persistentRunInitialized = true;
        persistentFloorNumber = 1;
        persistentSceneCycleIndex = 0;
    }

    private void SetGameOverScreenVisible(bool isVisible)
    {
        ResolveGameOverScreenIfMissing();

        if (gameOverScreen == null)
        {
            Debug.LogWarning("game: Game Over Screen belum di-assign.");
            return;
        }

        if (isVisible)
        {
            EnsureParentsActive(gameOverScreen.transform);
            CanvasGroup group = gameOverScreen.GetComponent<CanvasGroup>();
            if (group != null)
            {
                group.alpha = 1f;
                group.interactable = true;
                group.blocksRaycasts = true;
            }
        }

        gameOverScreen.SetActive(isVisible);
    }

    private void ResolveGameOverScreenIfMissing()
    {
        if (gameOverScreen != null)
        {
            return;
        }

        gameOverScreen = FindSceneObjectByNames(new[] { "Game Over", "gameOver", "GameOver" });
    }

    private GameObject FindSceneObjectByNames(string[] targetNames)
    {
        if (targetNames == null || targetNames.Length == 0)
        {
            return null;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        if (!activeScene.IsValid())
        {
            return null;
        }

        GameObject[] roots = activeScene.GetRootGameObjects();
        if (roots == null || roots.Length == 0)
        {
            return null;
        }

        Queue<Transform> queue = new Queue<Transform>();
        for (int i = 0; i < roots.Length; i++)
        {
            if (roots[i] != null)
            {
                queue.Enqueue(roots[i].transform);
            }
        }

        while (queue.Count > 0)
        {
            Transform current = queue.Dequeue();
            if (current == null)
            {
                continue;
            }

            for (int i = 0; i < targetNames.Length; i++)
            {
                if (current.name == targetNames[i])
                {
                    return current.gameObject;
                }
            }

            for (int i = 0; i < current.childCount; i++)
            {
                queue.Enqueue(current.GetChild(i));
            }
        }

        return null;
    }

    private void EnsureParentsActive(Transform child)
    {
        Transform current = child != null ? child.parent : null;
        while (current != null)
        {
            if (!current.gameObject.activeSelf)
            {
                current.gameObject.SetActive(true);
            }

            current = current.parent;
        }
    }
}
