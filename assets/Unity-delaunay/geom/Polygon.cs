using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Delaunay
{
	namespace Geo
	{
		public sealed class Polygon
		{
			private List<Vector2> _vertices;
            private Vector3 center;
            private bool isConnected = false;
            private int edgedConnections = 0;
            private List<Polygon> connections = new List<Polygon>();
            private float leastX, leastY, greatX, greatY;
            //private List<Vector2> Connected = new List<Vector2>();

			public Polygon (List<Vector2> vertices)
			{


				_vertices = vertices;
                center = Center();
                leastX = vertices.ToArray()[0].x;
                leastY = vertices.ToArray()[0].y;
                greatX = vertices.ToArray()[0].x;
                greatY = vertices.ToArray()[0].y;
                foreach (Vector2 v in _vertices)
                {
                    if(v.x > greatX)
                    {
                        greatX = v.x;
                    }
                    if(v.y > greatY)
                    {
                        greatY = v.y;
                    }
                    if(v.x < leastX)
                    {
                        leastX = v.x;
                    }
                    if(v.y < leastY)
                    {
                        leastY = v.y;
                    }
                }
			}

			public float Area ()
			{
				return Mathf.Abs (SignedDoubleArea () * 0.5f); // XXX: I'm a bit nervous about this; not sure what the * 0.5 is for, bithacking?
			}

			public Winding Winding ()
			{
				float signedDoubleArea = SignedDoubleArea ();
				if (signedDoubleArea < 0) {
					return Geo.Winding.CLOCKWISE;
				}
				if (signedDoubleArea > 0) {
					return Geo.Winding.COUNTERCLOCKWISE;
				}
				return Geo.Winding.NONE;
			}

			private float SignedDoubleArea () // XXX: I'm a bit nervous about this because Actionscript represents everything as doubles, not floats
			{
				int index, nextIndex;
				int n = _vertices.Count;
				Vector2 point, next;
				float signedDoubleArea = 0; // Losing lots of precision?
				for (index = 0; index < n; ++index) {
					nextIndex = (index + 1) % n;
					point = _vertices [index];
					next = _vertices [nextIndex];
					signedDoubleArea += point.x * next.y - next.x * point.y;
				}
				return signedDoubleArea;
			}

            public List<Vector2> points()
            {
                return _vertices;
            }

            private Vector3 Center()
            {
                float x = 0;
                float y = 0;
                foreach(Vector2 v in _vertices)
                {
                    x += v.x;
                    y += v.y;
                }
                y /= _vertices.Count;
                x /= _vertices.Count;
                return new Vector3(x, 0, y);
            }

            public Vector3 getCenter()
            {
                return center;
            }

            public void testConnected(Polygon A)
            {
                List<Vector2> tempConnections = new List<Vector2>();
                foreach(Vector2 v in _vertices)
                {
                    if (A.points().Contains(v))
                    {
                        tempConnections.Add(v);
                    }
                }
                if(tempConnections.Count > 1)
                {
                    edgedConnections++;
                    connections.Add(A);
                }
            }

            public int getConnections()
            {
                return edgedConnections;
            }

            public List<Polygon> getIsland()
            {
                return connections;
            }

            public bool isAroundPolygon(Vector3 A)
            {
                if(A.x <= greatX && A.x >= leastX && A.z <= greatY && A.z >= leastY)
                {
                    return true;
                }
                return false;
            }

            public bool isInPolygon(Vector3 A)
            {
                Dictionary<LineSegment, Vector2> intersections = new Dictionary<LineSegment, Vector2>();
                
                List<Vector2> tempIntersect = new List<Vector2>();
                //want to get both lines that hav an X at A.y
                //have to find function of all lines
                //have to plug in A.y into all functions
                //if the line has an x i use it.
                //if the lines intersect creating 4 intersections in a + shape. returns true if 4
                Vector2[] temparray = _vertices.ToArray();
                if (isAroundPolygon(A))
                {
                    for (int i = 0; i < temparray.Length; i++)
                    {
                        if (i + 1 < temparray.Length)
                        {
                            Vector2 ps1 = temparray[i];
                            Vector2 pe1 = temparray[i + 1];

                            Vector2 ps2 = new Vector2(-200000, A.z);
                            Vector2 pe2 = new Vector2(200000, A.z);

                            float A1 = pe1.y - ps1.y;
                            float B1 = ps1.x - pe1.x;
                            float C1 = A1 * ps1.x + B1 * ps1.y;

                            float A2 = pe2.y - ps2.y;
                            float B2 = ps2.x - pe2.x;
                            float C2 = A2 * ps2.x + B2 * ps2.y;

                            float delta = A1 * B2 - A2 * B1;
                            if (delta != 0)
                            {
                                Vector2 tempvec = new Vector2((B2 * C1 - B1 * C2) / delta, (A1 * C2 - A2 * C1) / delta);
                                if(Mathf.Abs(ps1.x) > Mathf.Abs(pe1.x))
                                {
                                    if(Mathf.Abs(tempvec.x) < Mathf.Abs(ps1.x) && Mathf.Abs(tempvec.x) > Mathf.Abs(pe1.x))
                                    {
                                        tempIntersect.Add(tempvec);
                                        intersections.Add(new LineSegment(ps1, pe1), tempvec);
                                    }
                                }
                                else
                                {
                                    if (Mathf.Abs(tempvec.x) > Mathf.Abs(ps1.x) && Mathf.Abs(tempvec.x) < Mathf.Abs(pe1.x))
                                    {
                                        tempIntersect.Add(tempvec);
                                        intersections.Add(new LineSegment(ps1, pe1), tempvec);
                                    }
                                }
                                /*
                                tempIntersect.Add(tempvec);
                                intersections.Add(new LineSegment(ps1, pe1), tempvec);
                                */
                            }
                        }
                        else if (i + 1 >= temparray.Length)
                        {
                            Vector2 ps1 = temparray[0];
                            Vector2 pe1 = temparray[i];

                            Vector2 ps2 = new Vector2(-200000, A.z);
                            Vector2 pe2 = new Vector2(200000, A.z);

                            float A1 = pe1.y - ps1.y;
                            float B1 = ps1.x - pe1.x;
                            float C1 = A1 * ps1.x + B1 * ps1.y;

                            float A2 = pe2.y - ps2.y;
                            float B2 = ps2.x - pe2.x;
                            float C2 = A2 * ps2.x + B2 * ps2.y;

                            float delta = A1 * B2 - A2 * B1;





                            if (delta != 0)
                            {
                                Vector2 tempvec = new Vector2((B2 * C1 - B1 * C2) / delta, (A1 * C2 - A2 * C1) / delta);
                                if (Mathf.Abs(ps1.x) > Mathf.Abs(pe1.x))
                                {
                                    if (Mathf.Abs(tempvec.x) < Mathf.Abs(ps1.x) && Mathf.Abs(tempvec.x) > Mathf.Abs(pe1.x))
                                    {
                                        tempIntersect.Add(tempvec);
                                        intersections.Add(new LineSegment(ps1, pe1), tempvec);
                                    }
                                }
                                else
                                {
                                    if (Mathf.Abs(tempvec.x) > Mathf.Abs(ps1.x) && Mathf.Abs(tempvec.x) < Mathf.Abs(pe1.x))
                                    {
                                        tempIntersect.Add(tempvec);
                                        intersections.Add(new LineSegment(ps1, pe1), tempvec);
                                    }
                                }
                            }
                        }
                    }
                }
                
                //NOW, I HAVE TO MAKE IT SEE IF IT'S COLLIDING ON THE GIVIN LINE

                Vector2? l = null;
                Vector2? r = null;
                foreach(KeyValuePair<LineSegment, Vector2> item in intersections)
                {
                    bool inx = false;
                    Vector2 left = (Vector2)item.Key.p0;
                    Vector2 right = (Vector2)item.Key.p1;
                    if(left.x > right.x)
                    {
                        if(item.Value.x <= left.x && item.Value.x >= right.x)
                        {
                            inx = true;
                        }
                    }
                    else
                    {
                        if (item.Value.x >= left.x && item.Value.x <= right.x)
                        {
                            inx = true;
                        }
                    }
                    
                    if (inx)
                    {
                        if(l == null)
                        {
                            l = item.Value;
                        }
                        else
                        {
                            if(r == null)
                            {
                                r = item.Value;
                            }
                            
                        }
                    }
                }
                if(intersections.Count >= 2 && (l != null && r != null))
                {
                    if (l.Value.x < r.Value.x)
                    {
                        if ((A.x <= r.Value.x) && (A.x >= l.Value.x))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if ((A.x >= r.Value.x) && (A.x <= l.Value.x))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

        }
	}
}
/*
foreach (Vector2 v in tempIntersect)
                {
                    if(isAroundPolygon(new Vector3(v.x, 0, v.y)))
                    {
                        connections++;
                    }
                }
    */
