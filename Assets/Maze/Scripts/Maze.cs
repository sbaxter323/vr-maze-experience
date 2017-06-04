using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Maze : MonoBehaviour {

	public IntVector2 size;

	public MazeCell cellPrefab;
	public MazePassage passagePrefab;
	public MazeWall[] wallPrefabs;
	public MazeDoor doorPrefab;
	public GameObject solutionRoomPrefab;
	public Waypoint waypointPrefab;
	public GameObject keyPrefab;

	//public MazeWaypoint waypointPrefab;
	[Range(0f, 1f)]
	//public float waypointProbability;
	public float doorProbability;
	public MazeRoomSettings[] roomSettings;

	public float generationStepDelay;
	private MazeCell[,] cells;
	private Waypoint[,] waypoints;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//public IEnumerator Generate (Player playerInstance) {
	public void Generate(Player playerInstance) {
		WaitForSeconds delay = new WaitForSeconds (generationStepDelay);
		cells = new MazeCell[size.x, size.z];
		waypoints = new Waypoint[size.x, size.z];

		// Generate maze cells by using Growing tree algorithm
		List<MazeCell> activeCells = new List<MazeCell>();
		DoFirstGenerationStep(activeCells);
		MazeCell firstCell = activeCells [0];
		playerInstance.SetLocation (firstCell);
		while (activeCells.Count > 0) {
			//yield return delay;
			DoNextGenerationStep(activeCells);
		}

		// Generate Waypoints, Coins, and Exit Room
		DoFinalGenerationStep();
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
			addWaypoint (currentCell);
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

	private void DoFinalGenerationStep () {
		MazePassage passage = Instantiate(passagePrefab) as MazePassage;
		MazeCell solutionCell = cells[size.x - 1, size.z - 1];
		// Remove southern wall
		Destroy(solutionCell.GetEdge(MazeDirection.East).gameObject);
		solutionCell.SetEdge (MazeDirection.East, passage);

		GameObject solutionRoom = Instantiate (solutionRoomPrefab) as GameObject;
		solutionRoom.transform.parent = this.transform;
		solutionRoom.transform.localPosition = new Vector3 (solutionCell.coordinates.x - size.x * 0.5f + 0.5f + 3.2f, .5f, solutionCell.coordinates.z - size.z * 0.5f + 0.5f + 1.59f);	
	}

	private void addWaypoint(MazeCell currentCell) {
		if (currentCell.isTurn () || ! isWaypointAccessible(currentCell.coordinates)) {
			Waypoint waypoint = Instantiate (waypointPrefab);
			waypoint.transform.parent = this.transform;
			waypoint.transform.localPosition = new Vector3 (currentCell.coordinates.x - size.x * 0.5f + 0.5f, .5f, currentCell.coordinates.z - size.z * 0.5f + 0.5f);
			waypoints[currentCell.coordinates.x, currentCell.coordinates.z] = waypoint;
		}
	}

	private bool isWaypointAccessible(IntVector2 coordinates) {
		return true;
	}
		

	private void CreatePassage (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		MazePassage prefab = Random.value < doorProbability ? doorPrefab : passagePrefab;
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
}