﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour {

	public IntVector2 coordinates;
	private int initializedEdgeCount;

	public MazeRoom room;

	public void Initialize (MazeRoom room) {
		room.Add(this);
		transform.GetChild(0).GetComponent<Renderer>().material = room.settings.floorMaterial;
	}

	private MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.Count];

	public MazeCellEdge GetEdge (MazeDirection direction) {
		return edges[(int)direction];
	}

	public MazeCellEdge GetEdge (int direction) {
		return edges[direction];
	}
		
	public bool IsFullyInitialized {
		get {
			return initializedEdgeCount == MazeDirections.Count;
		}
	}

	public void SetEdge (MazeDirection direction, MazeCellEdge edge) {
		edges[(int)direction] = edge;
		initializedEdgeCount += 1;
	}

	public MazeDirection RandomUninitializedDirection {
		get {
			int skips = Random.Range(0, MazeDirections.Count - initializedEdgeCount);
			for (int i = 0; i < MazeDirections.Count; i++) {
				if (edges[i] == null) {
					if (skips == 0) {
						return (MazeDirection)i;
					}
					skips -= 1;
				}
			}
			throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
		}

	}

	// Try very basic rules for now and see how waypoints are added
	public bool isTurn () {
		int numPassages = 0;
		ArrayList passageDirections = new ArrayList();
		for (int i = 0; i < edges.Length; i ++) {
			MazeCellEdge currentEdge = edges [i];
			if (currentEdge is MazePassage) {
				numPassages += 1;
				passageDirections.Add((int)currentEdge.direction);
			}
		}
		if (numPassages > 2) {
			return true;
		}

		if (numPassages == 2) {
			int firstDirection = (int)passageDirections [0];
			int secondDirection = (int)passageDirections [1];
			if ((firstDirection + 1) % MazeDirections.Count == secondDirection) {
				return true;
			} else if ((secondDirection + 1) % MazeDirections.Count == firstDirection) {
				return true;
			}
		} 
		return false;
	}

	public MazeCell[] getAccessibleNeighbors(int maxHops) {
		MazeCell[] accessibleNeighbors = new MazeCell[MazeDirections.Count];

		for (int i = 0; i < edges.Length; i++) {
			if (edges[i] is MazePassage)
				accessibleNeighbors [i] = getLongestWaypointPath(edges[i].otherCell, edges[i].direction, maxHops);
		}
		return accessibleNeighbors;
	}


	private MazeCell getLongestWaypointPath(MazeCell cell, MazeDirection dir, int max) {
		if (cell == null)
			return null;

		if (max == 0)
			return null;

		MazeCellEdge cellEdge = cell.GetEdge (dir);
		MazeCell nextCell = cellEdge.otherCell, recursiveCell;


		if (!cell.isTurn () && cellEdge is MazePassage) {
			recursiveCell = getLongestWaypointPath (nextCell, dir, max - 1);
			//Debug.Log ("Maze Cell: " + cell + " RecursiveCell: " + recursiveCell);
			if (recursiveCell != null)
				return recursiveCell;
			else
				return cell;
		} else {
			return cell;
		}

	}

}
