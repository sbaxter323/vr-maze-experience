using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Maze : MonoBehaviour {

	public IntVector2 size;

	public MazeCell cellPrefab;
	public MazePassage passagePrefab;
	public MazeWall[] wallPrefabs;
	public Door door;
	public Waypoint waypointPrefab;

	//public MazeWaypoint waypointPrefab;
	[Range(0f, 1f)]
	//public float waypointProbability;
	public float doorProbability;
	public MazeRoomSettings[] roomSettings;

	public float generationStepDelay;
	private MazeCell[,] cells;
	private Waypoint[] waypoints;
	private MazeCell[] coinCells;
	private int maxWaypointHops;
	private SignPost signpostInstance;
	private int coinCount;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//public IEnumerator Generate (Player playerInstance) {
	public void Generate(Player playerInstance, Key keyInstance, MazeSolutionRoom solutionRoomInstance, Coin[] coinInstances, int numCoins, int maxHops, SignPost sp) {
		WaitForSeconds delay = new WaitForSeconds (generationStepDelay);
		cells = new MazeCell[size.x, size.z];
		//waypoints = new Waypoint[size.x, size.z];

		// Generate maze cells by using Growing tree algorithm
		List<MazeCell> activeCells = new List<MazeCell>();
		DoFirstGenerationStep(activeCells);
		MazeCell firstCell = activeCells [0];
		while (activeCells.Count > 0) {
			//yield return delay;
			DoNextGenerationStep(activeCells);
		}

		// Generate Waypoints, Coins, and Exit Room
		maxWaypointHops = maxHops;
		signpostInstance = sp;
		coinCount = numCoins;
		DoFinalGenerationStep(playerInstance, keyInstance, solutionRoomInstance, coinInstances, numCoins);
	}

	private MazeCell CreateCell (IntVector2 coordinates) {
		MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
		cells[coordinates.x, coordinates.z] = newCell;
		newCell.coordinates = coordinates;
		newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
		newCell.transform.parent = transform;
		newCell.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f + 0.5f, 0f, coordinates.z - size.z * 0.5f + 0.5f);
		return newCell;
	}

	public IntVector2 RandomCoordinates {
		get {
			return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.z));
		}
	}

	public bool ContainsCoordinates (IntVector2 coordinate) {
		return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
	}

	public MazeCell GetCell (IntVector2 coordinates) {
		return cells[coordinates.x, coordinates.z];
	}

	private void DoFirstGenerationStep (List<MazeCell> activeCells) {
		MazeCell newCell = CreateCell(RandomCoordinates);
		activeCells.Add(newCell);
		newCell.Initialize(CreateRoom(-1));
	}

	private void CreatePassageInSameRoom (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		MazePassage passage = Instantiate(passagePrefab) as MazePassage;
		passage.Initialize(cell, otherCell, direction);
		passage = Instantiate(passagePrefab) as MazePassage;
		passage.Initialize(otherCell, cell, direction.GetOpposite());
	}

	private void DoNextGenerationStep (List<MazeCell> activeCells) {
		int currentIndex = activeCells.Count - 1;
		MazeCell currentCell = activeCells [currentIndex];
		if (currentCell.IsFullyInitialized) {
			activeCells.RemoveAt (currentIndex);
			//addWaypoint (currentCell);
			return;
		}
		MazeDirection direction = currentCell.RandomUninitializedDirection;
		IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2 ();

		if (ContainsCoordinates (coordinates)) {
			MazeCell neighbor = GetCell (coordinates);
			if (neighbor == null) {
				neighbor = CreateCell (coordinates);
				CreatePassage (currentCell, neighbor, direction);
				activeCells.Add (neighbor);
			//} else if (currentCell.room == neighbor.room) {
			//	CreatePassageInSameRoom (currentCell, neighbor, direction);
			} else {
				CreateWall (currentCell, neighbor, direction);
			}
		} else {
			CreateWall (currentCell, null, direction);
		}
	}

	private void DoFinalGenerationStep (Player playerInstance, Key keyInstance, MazeSolutionRoom solutionRoomInstance, Coin[] coinInstances, int numCoins) {
		playerInstance.SetLocation (cells[0, 0]);
	
		// Remove southern wall
		MazeCell solutionCell = cells[size.x - 1, size.z - 1];
		MazePassage passage = Instantiate(passagePrefab) as MazePassage;
		Destroy(solutionCell.GetEdge(MazeDirection.East).gameObject);
		solutionCell.SetEdge (MazeDirection.East, passage);

		// Create Solution Room
		solutionRoomInstance.transform.parent = this.transform;
		solutionRoomInstance.transform.localPosition = new Vector3 (solutionCell.coordinates.x - size.x * 0.5f + 0.5f + 3.2f, .5f, solutionCell.coordinates.z - size.z * 0.5f + 0.5f + 1.59f);	

		// Create the key
		int keyXIndex = (int)Random.Range(2, size.x -1);
		int keyZIndex = (int)Random.Range(2, size.z -1);
		MazeCell keyCell = cells [keyXIndex, keyZIndex];
		keyInstance.transform.localPosition = keyCell.transform.localPosition;
		keyInstance.transform.Translate(new Vector3(0.0f, 0.0f, 0.5f));

		// Generate dynamic waypoints for starting cell
		waypoints = new Waypoint[MazeDirections.Count];
		generateWaypoints(cells[0,0]);

		// Generate Coins
		int coinsToGenerate = numCoins, coinXIndex = (int)Random.Range (2, size.x - 1), coinZIndex = (int)Random.Range (2, size.z - 1);
		MazeCell coinCell = cells[coinXIndex, coinZIndex];
		bool uniqueCell;
		coinCells = new MazeCell[coinsToGenerate];

		for (coinsToGenerate = 0; coinsToGenerate < numCoins - 1; coinsToGenerate ++) {
			uniqueCell = false;
			while (uniqueCell == false) {
				
				int match = System.Array.IndexOf(coinCells, coinCell);

				// If there is no match in existing keys and coordinates don't match the generated key, continue
				Debug.Log ("match: " + match + "coinXIndex: " + coinXIndex + " coinZIndex: " + coinZIndex);
				Debug.Log ("coinCells " + coinCells);
				if ((match > -1) || (coinXIndex == keyCell.coordinates.x && coinZIndex == keyCell.coordinates.z)) {
					coinXIndex = (int)Random.Range (2, size.x - 1);
					coinZIndex = (int)Random.Range (2, size.z - 1);
					coinCell = cells [coinXIndex, coinZIndex];
				} else {
					uniqueCell = true;
				}
			}
					 
			// Add cell position to prevent overwrites, update coin instance to cell position
			Debug.Log ("coinsToGenerate: " + coinsToGenerate + "coinXIndex: " + coinXIndex + " coinZIndex: " + coinZIndex);
			coinCells[coinsToGenerate] = coinCell;
			coinInstances [coinsToGenerate].transform.localPosition = coinCell.transform.localPosition;
			// Shift Coin To Left
			coinInstances[coinsToGenerate].transform.Translate(new Vector3(0.0f, 0.0f, 0.5f));
		}
	}
		
	private void CreatePassage (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		//MazePassage prefab = Random.value < doorProbability ? doorPrefab : passagePrefab;
		MazePassage prefab = passagePrefab;
		MazePassage passage = Instantiate(prefab) as MazePassage;
		passage.Initialize(cell, otherCell, direction);
		passage = Instantiate(prefab) as MazePassage;
		if (passage is MazeDoor) {
			otherCell.Initialize(CreateRoom(cell.room.settingsIndex));
		}
		else {
			otherCell.Initialize(cell.room);
		}
		passage.Initialize(otherCell, cell, direction.GetOpposite());
	}

	private void CreateWall (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		MazeWall wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)]) as MazeWall;
		wall.Initialize(cell, otherCell, direction);
		if (otherCell != null) {
			wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)]) as MazeWall;
			wall.Initialize(otherCell, cell, direction.GetOpposite());
		}
	}

	private List<MazeRoom> rooms = new List<MazeRoom>();

	private MazeRoom CreateRoom (int indexToExclude) {
		MazeRoom newRoom = ScriptableObject.CreateInstance<MazeRoom>();
		newRoom.settingsIndex = Random.Range(0, roomSettings.Length);
		if (newRoom.settingsIndex == indexToExclude) {
			newRoom.settingsIndex = (newRoom.settingsIndex + 1) % roomSettings.Length;
		}
		newRoom.settings = roomSettings[newRoom.settingsIndex];
		rooms.Add(newRoom);
		return newRoom;
	}

	// Destroy existing waypoints from previous cell and add waypoints for each accessible neighboring cell 
	public void generateWaypoints(MazeCell cell) {

		for (int i = 0; i < waypoints.Length; i++) {
			if (waypoints [i] != null) {
				Destroy (waypoints [i].gameObject);
				waypoints [i] = null;
			}
		}
			
		MazeCell[] accessibleNeighbors = cell.getAccessibleNeighbors(maxWaypointHops);

		for (int j = 0; j < accessibleNeighbors.Length; j++) {
			if (accessibleNeighbors [j] != null) {
				Waypoint waypoint = Instantiate (waypointPrefab);
				waypoint.maze = this;
				waypoint.cell = accessibleNeighbors [j];
				waypoint.transform.parent = this.transform;
				waypoint.transform.localPosition = new Vector3 (accessibleNeighbors[j].coordinates.x - size.x * 0.5f + 0.5f, .5f, accessibleNeighbors[j].coordinates.z - size.z * 0.5f + 0.5f);
				waypoints[j] = waypoint;
			}
		}
			
	}

	public void updateSolutionCanvas() {
		coinCount--;
		signpostInstance.GetComponent<UnityEngine.UI.Text> ().text = "You Win! Coins Remaining: " + coinCount;
	}
		
}