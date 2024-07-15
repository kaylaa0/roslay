using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinding
{
    public static List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        Debug.Log(start + "    " + end);

        List<Vector3> path = new List<Vector3>();

        // Nodes to be evaluated
        var openSet = new HashSet<Vector3>();

        // Nodes already evaluated
        var closedSet = new HashSet<Vector3>();

        // Set of nodes to track the path
        var cameFrom = new Dictionary<Vector3, Vector3>();

        // Cost from start to current point
        var gScore = new Dictionary<Vector3, float>();

        // Total cost of getting from start to goal
        var fScore = new Dictionary<Vector3, float>();

        openSet.Add(start);
        gScore[start] = 0;
        fScore[start] = Vector3.Distance(start, end);

        while (openSet.Count > 0)
        {
            Vector3 current = Vector3.zero;
            float lowestFScore = Mathf.Infinity;

            foreach (var node in openSet)
            {
                if (fScore.ContainsKey(node) && fScore[node] < lowestFScore)
                {
                    current = node;
                    lowestFScore = fScore[node];
                }
            }

            if (current == end)
            {
                // Reconstruct path
                while (cameFrom.ContainsKey(current))
                {
                    path.Insert(0, current);
                    current = cameFrom[current];
                }
                path.Insert(0, start);
                return path;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (var neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                float tentativeGScore = gScore[current] + Vector3.Distance(current, neighbor);

                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
                else if (tentativeGScore >= gScore[neighbor])
                    continue; // This is not a better path

                // This path is the best until now
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Vector3.Distance(neighbor, end);
            }
        }

        return null; // No path found
    }

    // Replace this method with your own implementation to get valid neighbors of a node
    private static List<Vector3> GetNeighbors(Vector3 node)
    {
        List<Vector3> neighbors = new List<Vector3>();

        // Define the directions to check for neighbors (you may adjust this based on your grid or graph structure)
        Vector3[] directions = new Vector3[]
        {
        Vector3.forward,
        Vector3.back,
        Vector3.left,
        Vector3.right
            // Add more directions as needed
        };

        foreach (Vector3 direction in directions)
        {
            RaycastHit hit;
            if (!Physics.Raycast(node, direction, out hit, 4.0f)) // Adjust the distance as needed
            {
                neighbors.Add(node + direction);
            }
        }

        return neighbors;
    }
}