using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [SerializeField] public string id = "";
    [SerializeField] public int width = 1, length = 1;
    [SerializeField] public HeadPair[] headPairs = new HeadPair[0];
    [SerializeField] public Obstacle[] obstacles = new Obstacle[0];
    [SerializeField] public Solution solution;
    [System.Serializable]
    public class Head
    {
        public int row, col;

        public Head(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }
    [System.Serializable]
    public class HeadPair
    {
        public BallType type;
        public Head a, b;
    }
    [System.Serializable]
    public class Obstacle
    {
        public int row, col;
        public Direction direction;
    }
    [System.Serializable]
    public class Solution
    {
        [System.Serializable]
        public class Path
        {
            [SerializeField] public Coordinates[] path;

            public Path(Coordinates[] path)
            {
                this.path = path;
            }
        }
        [System.Serializable]
        public class Coordinates
        {
            [SerializeField] public int row, col;

            public Coordinates(int row, int col)
            {
                this.row = row;
                this.col = col;
            }
        }
        [SerializeField] public Path[] paths;

        public Solution(string solutionString)
        {
            string[] lines = solutionString.Trim().Split("\n", System.StringSplitOptions.RemoveEmptyEntries);
            paths = new Path[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                string input = lines[i];
                Regex pattern = new Regex(@"\((?<x>\d+), (?<y>\d+)\)");
                MatchCollection matches = pattern.Matches(input);
                paths[i] = new Path(new Coordinates[matches.Count]);
                for (int j = 0; j < matches.Count; j++)
                {
                    Match match = matches[j];
                    int x = int.Parse(match.Groups["x"].Value);
                    int y = int.Parse(match.Groups["y"].Value);
                    paths[i].path[j] = new(y, x);
                }
            }
        }
    }
}
