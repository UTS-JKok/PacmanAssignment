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

        RenderMap();
    }

    void Update()
    {
        
    }

    /**
     * Get and render map.
     *
     * NB: Rotation uses hardcoded values for map piece type. Also some
     *     room to improve. Consider rotation map.
     */
    void RenderMap()
    {
        int rowIndex = 0;
        int columnIndex = 0;
        Vector3 renderAnchor = new Vector3(-14, 14, 0);
        Vector3 renderPos = new Vector3(-14, 14, 0);

        List<List<int>> map = GetMap("Recreation");
        foreach (List<int> row in map)
        {
            foreach (int value in row)
            {
                if (value != 0)
                {
                    GameObject mapPiece = Instantiate(mapPieceMapping[value], renderPos, Quaternion.identity);
                    
                    // Check rotation
                    bool doFlipX = false;
                    bool doFlipY = false;
                    switch (value)
                    {
                        case 1:
                            if (columnIndex != 0 && map[rowIndex][columnIndex - 1] == 2)
                            {
                                doFlipX = true;
                            }
                            if (rowIndex != 0 && map[rowIndex - 1][columnIndex] == 2)
                            {
                                doFlipY = true;
                            }
                            break;

                        case 2:
                            if (rowIndex != 0 && (map[rowIndex - 1][columnIndex] == 1 || map[rowIndex - 1][columnIndex] == 2))
                            {
                                Rotate(mapPiece, 90);
                            }
                            break;

                        case 3:
                            if (columnIndex != 0 && (map[rowIndex][columnIndex - 1] == 3 || map[rowIndex][columnIndex - 1] == 4))
                            {
                                try
                                {
                                    if (map[rowIndex][columnIndex + 1] != 4)
                                    {
                                        doFlipX = true;
                                    }
                                }
                                catch (System.Exception e)
                                {
                                    Debug.Log("Made a quick catch.");
                                }
                            }
                            if (rowIndex != 0 && (map[rowIndex - 1][columnIndex] == 3 || map[rowIndex - 1][columnIndex] == 4))
                            {
                                try
                                {
                                    if (map[rowIndex + 1][columnIndex] != 4)
                                    {
                                        doFlipY = true;
                                    }
                                }
                                catch (System.Exception e)
                                {
                                    Debug.Log("Made a quick catch.");
                                }
                            }
                            break;

                        case 4:
                            if (rowIndex != 0 && (map[rowIndex - 1][columnIndex] == 3 || map[rowIndex - 1][columnIndex] == 4
                                                                                      || map[rowIndex - 1][columnIndex] == 7))
                            {
                                try
                                {
                                    if (map[rowIndex + 1][columnIndex] != 0 && map[rowIndex + 1][columnIndex] != 5
                                                                            && map[rowIndex + 1][columnIndex] != 6)
                                    {
                                        Rotate(mapPiece, 90);
                                    }
                                }
                                catch (System.Exception e)
                                {
                                    Debug.Log("Made a quick catch.");
                                }
                            }
                            break;

                        case 7:
                            if (rowIndex != 0 && map[rowIndex - 1][columnIndex] == 4)
                            {
                                FlipY(mapPiece);
                            }
                            break;
                    }

                    // Do rotation
                    if (doFlipX && doFlipY) {
                        FlipBoth(mapPiece);
                    }
                    else if (doFlipX) { FlipX(mapPiece); }
                    else if (doFlipY) { FlipY(mapPiece); }
                }
                renderPos.x++;
                columnIndex++;
            }
            renderPos.x = renderAnchor.x;
            renderPos.y -= 1;

            columnIndex = 0;
            rowIndex++;
        }
    }

    /**
     * Flip around the X axis.
     */
    void FlipX(GameObject gameObject, SpriteRenderer renderer = null)
    {
        renderer = (renderer is null) ? gameObject.GetComponent<SpriteRenderer>()
                                      : renderer;
        renderer.flipX = true;
    }

    /**
     * Flip around the Y axis.
     */
    void FlipY(GameObject gameObject, SpriteRenderer renderer = null)
    {
        renderer = (renderer is null) ? gameObject.GetComponent<SpriteRenderer>()
                                      : renderer;
        renderer.flipY = true;
    }

    /**
     * Flip around the X and Y axis.
     */
    void FlipBoth(GameObject gameObject)
    {
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        FlipX(gameObject, renderer);
        FlipY(gameObject, renderer);
    }

    /**
     * Rotate around the apparent X axis.
     *
     * For convenience.
     */
    void Rotate(GameObject gameObject, float angle)
    {
        gameObject.transform.Rotate(0, 0, angle);
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
