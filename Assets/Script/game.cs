using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class game : MonoBehaviour
{
    public static game Instance { get; private set; }

    [Header("Core References")]
    [SerializeField] private Puzzle1Manager puzzle1Manager;
    [SerializeField] private RunRandomizer runRandomizer;
    [SerializeField] private FloorPressureManager floorPressureManager;

    [Header("Restart")]
    [SerializeField] private bool allowRestartWithR = true;
    [SerializeField] private bool restartOnlyAfterGameEnds = true;

    public enum GameState
    {
        Playing,
        Win,
        Lose
    }

    public GameState CurrentState { get; private set; } = GameState.Playing;
    public Puzzle1Manager Puzzle1 => puzzle1Manager;
    public RunRandomizer Randomizer => runRandomizer;
    public FloorPressureManager FloorPressure => floorPressureManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Lebih dari satu game manager ditemukan di scene.");
        }

        Instance = this;

        if (puzzle1Manager == null)
        {
            puzzle1Manager = GetComponent<Puzzle1Manager>();
        }

        if (runRandomizer == null)
        {
            runRandomizer = GetComponent<RunRandomizer>();
        }

        if (floorPressureManager == null)
        {
            floorPressureManager = GetComponent<FloorPressureManager>();
        }
    }

    private void Update()
    {
        if (!allowRestartWithR || Keyboard.current == null)
        {
            return;
        }

        if (!Keyboard.current.rKey.wasPressedThisFrame)
        {
            return;
        }

        if (restartOnlyAfterGameEnds && CurrentState == GameState.Playing)
        {
            Debug.Log("Restart dengan R aktif saat state Win/Lose.");
            return;
        }

        RestartRun();
    }

    public void WinRun()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        CurrentState = GameState.Win;
        Debug.Log("WIN: objective selesai dan berhasil keluar.");
    }

    public void LoseRun()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        CurrentState = GameState.Lose;
        Debug.Log("LOSE: player gagal bertahan.");
    }

    public void RestartRun()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
