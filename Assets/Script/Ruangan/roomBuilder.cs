using System.Collections.Generic;
using UnityEngine;

public class roomBuilder : MonoBehaviour
{
	[Header("Runtime Build")]
	[SerializeField] private bool buildOnStart = true;
	[SerializeField] private bool rebuildOnPrepareRound = true;

	[Header("Room Size")]
	[SerializeField] private int minColumns = 8;
	[SerializeField] private int maxColumns = 12;
	[SerializeField] private int minRows = 8;
	[SerializeField] private int maxRows = 12;
	[SerializeField] private float tileSize = 1f;

	[Header("Fullscreen Fit")]
	[SerializeField] private bool fitRoomToCamera = true;
	[SerializeField] private Camera targetCamera;
	[SerializeField] private bool centerOnCamera = true;
	[SerializeField, Min(0f)] private float viewportPaddingWorld = 0.25f;
	[SerializeField] private bool addOneTileOverscan = true;

	[Header("Edge State")]
	[SerializeField] private bool outerRingStartsBroken = false;

	[Header("Prefabs")]
	[SerializeField] private GameObject floorPrefab;
	[SerializeField] private GameObject[] obstaclePrefabs;

	[Header("Generated Roots")]
	[SerializeField] private bool useNamedRoots = true;
	[SerializeField] private string floorRootName = "tilesRoot";
	[SerializeField] private string obstacleRootName = "obstaclesRoot";
	[SerializeField] private Transform floorRoot;
	[SerializeField] private Transform obstacleRoot;
	[SerializeField] private FloorPressureManager floorPressureManager;

	[Header("Obstacle")]
	[SerializeField] private int minObstacleCount = 3;
	[SerializeField] private int maxObstacleCount = 10;
	[SerializeField] private int edgePadding = 1;

	[Header("Auto Place Target")]
	[SerializeField] private Transform playerSpawnTarget;
	[SerializeField] private Transform exitSpawnTarget;
	[SerializeField] private Transform[] dynamicTargetsToPlace;

	[Header("Random Seed")]
	[SerializeField] private bool useRoundAsSeed = true;
	[SerializeField] private int fixedSeed = 12345;

	private readonly List<Vector3> freePositions = new List<Vector3>();
	private readonly List<Vector3> generatedInteriorPositions = new List<Vector3>();
	private int currentColumns;
	private int currentRows;

	private void Start()
	{
		if (buildOnStart)
		{
			PrepareForRound(1);
		}
	}

	public void PrepareForRound(int roundNumber)
	{
		if (!rebuildOnPrepareRound && HasGeneratedContent())
		{
			return;
		}

		Random.State previousState = Random.state;
		Random.InitState(useRoundAsSeed ? roundNumber : fixedSeed);

		GenerateRoom();

		Random.state = previousState;
	}

	[ContextMenu("Generate Room Now")]
	public void GenerateRoom()
	{
		if (floorPrefab == null)
		{
			Debug.LogWarning("roomBuilder: floorPrefab belum di-assign.");
			return;
		}

		EnsureRoots();
		ClearGenerated();

		ResolveRoomSize();

		BuildFloorAndWalls();
		PlaceTargetsAndObstacles();
		NotifyFloorTileSource();

		Debug.Log("roomBuilder: room dibuat " + currentColumns + "x" + currentRows + ".");
	}

	[ContextMenu("Clear Generated Room")]
	public void ClearGenerated()
	{
		ClearChildren(floorRoot);
		ClearChildren(obstacleRoot);
		freePositions.Clear();
		generatedInteriorPositions.Clear();
	}

	private void BuildFloorAndWalls()
	{
		if (centerOnCamera)
		{
			Camera cam = ResolveCamera();
			if (cam != null)
			{
				transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, transform.position.z);
			}
		}

		float startX = -(currentColumns - 1) * tileSize * 0.5f;
		float startY = -(currentRows - 1) * tileSize * 0.5f;
		for (int y = 0; y < currentRows; y++)
		{
			for (int x = 0; x < currentColumns; x++)
			{
				Vector3 worldPos = transform.position + new Vector3(startX + x * tileSize, startY + y * tileSize, 0f);

				GameObject tile = Instantiate(floorPrefab, worldPos, Quaternion.identity, floorRoot);
				generatedInteriorPositions.Add(worldPos);

				if (outerRingStartsBroken)
				{
					bool isOuter = x == 0 || y == 0 || x == currentColumns - 1 || y == currentRows - 1;
					if (isOuter)
					{
						floorRetak retak = tile.GetComponent<floorRetak>();
						if (retak != null)
						{
							retak.ForceSetBroken();
						}
					}
				}

				if (x >= edgePadding && y >= edgePadding && x < currentColumns - edgePadding && y < currentRows - edgePadding)
				{
					freePositions.Add(worldPos);
				}
			}
		}
	}

	private void ResolveRoomSize()
	{
		if (!fitRoomToCamera)
		{
			currentColumns = Random.Range(Mathf.Max(2, minColumns), Mathf.Max(minColumns, maxColumns) + 1);
			currentRows = Random.Range(Mathf.Max(2, minRows), Mathf.Max(minRows, maxRows) + 1);
			return;
		}

		Camera cam = ResolveCamera();
		if (cam == null || !cam.orthographic)
		{
			Debug.LogWarning("roomBuilder: Fit Room To Camera aktif, tapi kamera tidak ditemukan atau bukan orthographic. Pakai random size.");
			currentColumns = Random.Range(Mathf.Max(2, minColumns), Mathf.Max(minColumns, maxColumns) + 1);
			currentRows = Random.Range(Mathf.Max(2, minRows), Mathf.Max(minRows, maxRows) + 1);
			return;
		}

		float visibleHeight = (cam.orthographicSize * 2f) - viewportPaddingWorld * 2f;
		float visibleWidth = (cam.orthographicSize * 2f * cam.aspect) - viewportPaddingWorld * 2f;

		visibleWidth = Mathf.Max(tileSize * 2f, visibleWidth);
		visibleHeight = Mathf.Max(tileSize * 2f, visibleHeight);

		currentColumns = Mathf.Max(2, Mathf.CeilToInt(visibleWidth / Mathf.Max(0.01f, tileSize)));
		currentRows = Mathf.Max(2, Mathf.CeilToInt(visibleHeight / Mathf.Max(0.01f, tileSize)));

		if (addOneTileOverscan)
		{
			currentColumns += 1;
			currentRows += 1;
		}
	}

	private Camera ResolveCamera()
	{
		if (targetCamera != null)
		{
			return targetCamera;
		}

		if (Camera.main != null)
		{
			return Camera.main;
		}

		return FindFirstObjectByType<Camera>();
	}

	private void PlaceTargetsAndObstacles()
	{
		if (playerSpawnTarget != null && TryTakeFreePosition(out Vector3 playerPos))
		{
			playerSpawnTarget.position = playerPos;
		}

		if (exitSpawnTarget != null && TryTakeFarthestPosition(playerSpawnTarget != null ? playerSpawnTarget.position : transform.position, out Vector3 exitPos))
		{
			exitSpawnTarget.position = exitPos;
		}

		if (dynamicTargetsToPlace != null)
		{
			for (int i = 0; i < dynamicTargetsToPlace.Length; i++)
			{
				Transform target = dynamicTargetsToPlace[i];
				if (target == null)
				{
					continue;
				}

				if (TryTakeFreePosition(out Vector3 pos))
				{
					target.position = pos;
				}
			}
		}

		if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
		{
			return;
		}

		int safeMin = Mathf.Max(0, minObstacleCount);
		int safeMax = Mathf.Max(safeMin, maxObstacleCount);
		int obstacleCount = Mathf.Min(Random.Range(safeMin, safeMax + 1), freePositions.Count);

		for (int i = 0; i < obstacleCount; i++)
		{
			if (!TryTakeFreePosition(out Vector3 obstaclePos))
			{
				break;
			}

			GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
			if (prefab == null)
			{
				continue;
			}

			Instantiate(prefab, obstaclePos, Quaternion.identity, obstacleRoot);
		}
	}

	private bool TryTakeFreePosition(out Vector3 position)
	{
		position = default;

		if (freePositions.Count == 0)
		{
			return false;
		}

		int index = Random.Range(0, freePositions.Count);
		position = freePositions[index];
		freePositions.RemoveAt(index);
		return true;
	}

	private bool TryTakeFarthestPosition(Vector3 fromPosition, out Vector3 result)
	{
		result = default;

		if (freePositions.Count == 0)
		{
			return false;
		}

		float bestDistance = float.MinValue;
		int bestIndex = 0;

		for (int i = 0; i < freePositions.Count; i++)
		{
			float distance = Vector3.SqrMagnitude(freePositions[i] - fromPosition);
			if (distance > bestDistance)
			{
				bestDistance = distance;
				bestIndex = i;
			}
		}

		result = freePositions[bestIndex];
		freePositions.RemoveAt(bestIndex);
		return true;
	}

	private void EnsureRoots()
	{
		if (floorRoot == null)
		{
			floorRoot = ResolveOrCreateRoot(useNamedRoots ? floorRootName : "GeneratedFloor");
		}

		if (obstacleRoot == null)
		{
			obstacleRoot = ResolveOrCreateRoot(useNamedRoots ? obstacleRootName : "GeneratedObstacles");
		}
	}

	private Transform ResolveOrCreateRoot(string rootName)
	{
		if (!string.IsNullOrWhiteSpace(rootName))
		{
			Transform existing = transform.Find(rootName);
			if (existing != null)
			{
				return existing;
			}
		}

		return CreateRoot(string.IsNullOrWhiteSpace(rootName) ? "GeneratedRoot" : rootName);
	}

	private Transform CreateRoot(string rootName)
	{
		GameObject child = new GameObject(rootName);
		child.transform.SetParent(transform);
		child.transform.localPosition = Vector3.zero;
		child.transform.localRotation = Quaternion.identity;
		child.transform.localScale = Vector3.one;
		return child.transform;
	}

	private void NotifyFloorTileSource()
	{
		if (floorRoot == null)
		{
			return;
		}

		if (floorPressureManager == null)
		{
			floorPressureManager = FindFirstObjectByType<FloorPressureManager>();
		}

		if (floorPressureManager != null)
		{
			floorPressureManager.SetTilesRoot(floorRoot, true);
		}
	}

	public Transform GetFloorRoot()
	{
		return floorRoot;
	}

	public Transform GetPlayerSpawnTarget()
	{
		return playerSpawnTarget;
	}

	public Transform GetExitSpawnTarget()
	{
		return exitSpawnTarget;
	}

	public bool TryGetGeneratedSpawnPosition(bool reservePosition, out Vector3 position)
	{
		position = default;

		if (freePositions.Count > 0)
		{
			int index = Random.Range(0, freePositions.Count);
			position = freePositions[index];

			if (reservePosition)
			{
				freePositions.RemoveAt(index);
			}

			return true;
		}

		if (!reservePosition && generatedInteriorPositions.Count > 0)
		{
			position = generatedInteriorPositions[Random.Range(0, generatedInteriorPositions.Count)];
			return true;
		}

		return false;
	}

	private bool HasGeneratedContent()
	{
		return (floorRoot != null && floorRoot.childCount > 0)
			|| (obstacleRoot != null && obstacleRoot.childCount > 0);
	}

	private void ClearChildren(Transform root)
	{
		if (root == null)
		{
			return;
		}

		for (int i = root.childCount - 1; i >= 0; i--)
		{
			Transform child = root.GetChild(i);
			child.SetParent(null);
			if (Application.isPlaying)
			{
				Destroy(child.gameObject);
			}
			else
			{
				DestroyImmediate(child.gameObject);
			}
		}
	}
}
