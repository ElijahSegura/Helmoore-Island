using UnityEngine;
using System.Collections.Generic;

public class Map {
    private float[,] map;
    private Square[,] Squares;
    public Map(int Size)
    {
        map = new float[Size, Size];
        Squares = new Square[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                map[i, j] = 0f;
            }
        }
    }

    public void setMap(float[,] value)
    {
    }

    public void setSquare(int x, int y, Square value)
    {

    }
}
