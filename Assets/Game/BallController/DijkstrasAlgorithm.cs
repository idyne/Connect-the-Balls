// C# program for Dijkstra's
// single source shortest path
// algorithm. The program is for
// adjacency matrix representation
// of the graph.
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DijkstrasAlgorithm
{
    public static readonly int NOT_REACHABLE_DISTANCE = 1000;
    private static readonly int NO_PARENT = -1;

    // Function that implements Dijkstra's
    // single source shortest path
    // algorithm for a graph represented
    // using adjacency matrix
    // representation
    public static int[] dijkstra(int[,] adjacencyMatrix,
                                        int startVertex, out int[] shortestDistances)
    {
        int nVertices = adjacencyMatrix.GetLength(0);

        // shortestDistances[i] will hold the
        // shortest distance from src to i
         shortestDistances = new int[nVertices];

        // added[i] will true if vertex i is
        // included / in shortest path tree
        // or shortest distance from src to
        // i is finalized
        bool[] added = new bool[nVertices];

        // Initialize all distances as
        // INFINITE and added[] as false
        for (int vertexIndex = 0; vertexIndex < nVertices;
                                            vertexIndex++)
        {
            shortestDistances[vertexIndex] = int.MaxValue;
            added[vertexIndex] = false;
        }

        // Distance of source vertex from
        // itself is always 0
        shortestDistances[startVertex] = 0;

        // Parent array to store shortest
        // path tree
        int[] parents = new int[nVertices];

        // The starting vertex does not
        // have a parent
        parents[startVertex] = NO_PARENT;

        // Find shortest path for all
        // vertices
        for (int i = 1; i < nVertices; i++)
        {

            // Pick the minimum distance vertex
            // from the set of vertices not yet
            // processed. nearestVertex is
            // always equal to startNode in
            // first iteration.
            int nearestVertex = -1;
            int shortestDistance = int.MaxValue;
            for (int vertexIndex = 0;
                    vertexIndex < nVertices;
                    vertexIndex++)
            {
                if (!added[vertexIndex] &&
                    shortestDistances[vertexIndex] <
                    shortestDistance)
                {
                    nearestVertex = vertexIndex;
                    shortestDistance = shortestDistances[vertexIndex];
                }
            }
            

            // Mark the picked vertex as
            // processed
            added[nearestVertex] = true;

            // Update dist value of the
            // adjacent vertices of the
            // picked vertex.
            for (int vertexIndex = 0;
                    vertexIndex < nVertices;
                    vertexIndex++)
            {
                int edgeDistance = adjacencyMatrix[nearestVertex, vertexIndex];

                if (edgeDistance > 0
                    && ((shortestDistance + edgeDistance) <
                        shortestDistances[vertexIndex]))
                {
                    parents[vertexIndex] = nearestVertex;
                    shortestDistances[vertexIndex] = shortestDistance +
                                                    edgeDistance;
                }
            }
        }

        //printSolution(startVertex, shortestDistances, parents);
        return parents;
    }

    // A utility function to print
    // the constructed distances
    // array and shortest paths
    private static void printSolution(int startVertex,
                                    int[] distances,
                                    int[] parents)
    {
        int nVertices = distances.Length;
        Debug.Log("Vertex\t Distance\tPath");

        for (int vertexIndex = 0; vertexIndex < nVertices; vertexIndex++)
        {
            if (vertexIndex != startVertex)
            {
                Debug.Log(printPath(vertexIndex, parents));
            }
        }
    }

    // Function to print shortest path
    // from source to currentVertex
    // using parents array
    public static string printPath(int currentVertex,
                                int[] parents)
    {

        // Base case : Source node has
        // been processed
        if (currentVertex == NO_PARENT)
        {
            return "";
        }
        return printPath(parents[currentVertex], parents) + (currentVertex + " ");
    }
    public static int[] path(int currentVertex,
                                int[] parents)
    {
        // Base case : Source node has
        // been processed
        int[] result;
        if (currentVertex == NO_PARENT)
        {
            result = new int[] { };
            return result;
        }
        result = new int[] { currentVertex };
        return path(parents[currentVertex], parents).Union(result).ToArray();
    }


}

// This code has been contributed by 29AjayKumar
