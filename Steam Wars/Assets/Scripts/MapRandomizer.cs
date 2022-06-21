using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapRandomizer : MonoBehaviour
{
    int[,] map;

    [Range(0, 100)]
    public int randomFillPercent;
    int width;
    int height;

    public string seed;

    public bool useRandomSeed;

    void Start()
    {
        width = (int)GridMap.Instance.gridWorldSize.x;
        height = (int)GridMap.Instance.gridWorldSize.y;

        GenerateMap();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridMap.Instance.grid[x, y].material = map[x, y];
            }
        }

        GenerateMap();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridMap.Instance.grid[x, y].fogMaterial = map[x, y];
            }
        }
    }

    string RandomString(int lenght)
    {
        string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!?./,-+=()*&^%$#@:;`~<>][{}|\"";
        string generated_string = "";

        for (int i = 0; i < lenght; i++)
            generated_string += characters[Random.Range(0, characters.Length)];

        

        return generated_string;
    }

    void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }
    }

    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = RandomString(Random.Range(1, 26));
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                {
                    map[x, y] = 1;
                }
                else if (neighbourWallTiles < 4)
                {
                    map[x, y] = 0;
                }
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;

        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }
}
