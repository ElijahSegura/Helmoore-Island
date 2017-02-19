using UnityEngine;
internal class Triangle
{
    public Vector2 solveCenter(Vector3 A, Vector3 B, Vector3 C)
    {
        Vector3 center;
        int Cx = (int)(A.x + B.x + C.x) / 3;
        int Cy = (int)(A.y + B.y + C.y) / 3;
        center = new Vector3(Cx, Cy, 0);
        return center;
    }
}