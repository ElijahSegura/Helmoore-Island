using UnityEngine;
using System.Collections;

public class PorabolaTes : MonoBehaviour {
    Perabula[] p = { new Perabula(new Vector2(-10, 8)) { iteration = 2}, new Perabula(new Vector2(-7, 5)) { iteration = 0} };

    void CalculateIntersections()
    {
        Perabula A = p[0];
        Perabula B = p[1];
        int Ab = (int)(A.Origin().z * 2);
        float Bb = B.Origin().z * 2;
        int Ac = (int)(A.Origin().z * A.Origin().z);
        int Bc = (int)((B.Origin().z * B.Origin().z) + -B.Origin().x);
        int Aa = B.getIteration();
        int Ba = A.getIteration();
        Debug.Log(Aa);
        Debug.Log(Ba);
        Bc += (int)A.Origin().x;
        Bb *= Ba;
        Bc *= Ba;
        Ba -= Aa;
        Bc -= Ac;
        Bb -= Ab;
        Bb *= -1;
        Bb /= Ba;
        Bc /= Ba;
        Bb /= 2;
        float tempBy = -(Mathf.Sqrt(Mathf.Pow(Bb, 2) - Aa) + Bb);
        float tempAy = Mathf.Sqrt(Mathf.Pow(Bb, 2) - Aa) - Bb;
        Debug.Log(-(Mathf.Pow((tempBy + A.Origin().z), 2) - A.Origin().x));
        Debug.Log(-(Mathf.Pow((tempAy - B.Origin().z), 2) - B.Origin().x));
        Debug.Log(tempBy);
        Debug.Log(tempAy);

        Vector3 start = new Vector3(-(Mathf.Pow((tempBy - B.Origin().z), 2) - B.Origin().x), 0, tempBy);
        Vector3 end = new Vector3(-(Mathf.Pow((tempAy - B.Origin().z), 2) - B.Origin().x),0 , tempAy);
        Debug.DrawLine(start, end, Color.green, 0.025F);
        //x = (((y - A.z) ^ 2) / iteration) - A.x
        //x = (((y - A.z) ^ 2) / iteration) - A.x

        //x = (y * y)(-A.z + y)(-A.z + y)(-A.z * -A.z)
        //--------------------------------------------- - A.x
        //                     3

        //x = (y * y)(-B.z + y)(-B.z + y)(-B.z * -B.z) - B.x

        //x = (y * y)(-A.z + y)(-A.z + y)(-A.z * -A.z)
        //---------------------------------------------
        //                     3

        //x = (y * y)(-B.z + y)(-B.z + y)(-B.z * -B.z) - B.x + A.x

        //x = (y * y)(-A.z + y)(-A.z + y)(-A.z * -A.z)


        //x = 3(y * y)  3(-B.z + y)  3(-B.z + y)  3(-B.z * -B.z) - 3(B.x + A.x)


    }

    void Update()
    {
        
        foreach(Perabula d in p)
        {
            d.calculate();
            d.draw();
        }
        CalculateIntersections();
    }
}
