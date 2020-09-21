using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject outsideCorner;
    [SerializeField] private GameObject outsideWall;
    [SerializeField] private GameObject insideCorner;
    [SerializeField] private GameObject insideWall;
    [SerializeField] private GameObject standardPellet;
    [SerializeField] private GameObject powerPellet;
    [SerializeField] private GameObject outsideJunction;
    
    private Dictionary<int, GameObject> mapPieceMapping;

    void Start()
    {
        mapPieceMapping = new Dictionary<int, GameObject>() {
            {1, outsideCorner},
            {2, outsideWall},
            {3, insideCorner},
            {4, insideWall},
            {5, standardPellet},
            {6, powerPellet},
            {7, outsideJunction}
        };

        // Get and render map
        List<List<int>> map = GetMap("Recreation");
        Vector3 renderPos = new Vector3(-7, 7, 0);
        foreach (List<int> row in map)
        {
            foreach (int value in row)
            {
                if (value != 0)
                {
                    Instantiate(mapPieceMapping[value], renderPos, Quaternion.identity);
                }
                renderPos.x += 1;
            }
            renderPos.x = -7;
            renderPos.y -= 1;
        }
    }

    void Update()
    {
        
    }

    /**
     * Open the map with the given name and return its contents.
     */
    List<List<int>> GetMap(string name)
    {
        List<List<int>> map = new List<List<int>>();
        List<int> data;

        // Get map data
        string mapData = System.IO.File.ReadAllText($"Assets/Maps/{name}.csv").Trim();
        foreach (string row in mapData.Split('\n'))
        {
            data = new List<int>();
            foreach (string value in row.Split(','))
            {
                data.Add(int.Parse(value));
            }
            map.Add(data);
        }
        return map;
    }
}
