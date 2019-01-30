using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public bool debug;
    List<Extension> exits;
    List<Vector3> points;
    List<Road> refs;
    Path path;
    GameObject render;
    Nature nature;

    public void makeRoadTintRed()
    {
        render.GetComponent<MeshRenderer>().material.color = Color.red;
    }

    public void makeRoadTintWhite()
    {
        render.GetComponent<MeshRenderer>().material.color = Color.white;
    }
    
    public void makeRoadTintGreen()
    {
        render.GetComponent<MeshRenderer>().material.color = Color.green;
    }

    public Vector3 this[int i]
    {
        get
        {
            return points[i];
        }
        set
        {
            points[i] = value;
        }
    }

    public Vector3Int[] getPath()
    {
        return path.getPoints();
    }

    public Nature Nature
    {
        get
        {
            return nature;
        }
    }

    public void deleteAtPoisition(int i)
    {
        points.RemoveAt(i);
    }

    public int Length
    {
        get
        {
            return points.Count;
        }
    }

    public int Exits
    {
        get
        {
            return exits.Count;
        }
    }

    public void setPoints(List<Vector3> pts)
    {
        points = new List<Vector3>(pts);
    }

    public List<Vector3> getPoints()
    {
        return new List<Vector3>(points);
    }

    public List<Extension> getExtensionsPlaces()
    {
        return exits;
    }

    public void rendererExtensions()
    {
        if (nature == Nature.Anchor || nature == Nature.Normal)
        {
            if (RoadEditorController.Instance.noExt())
            {
                if (refEmptyAt(0))
                {
                    GameObject g = Instantiate(RoadEditorController.Instance.extensionObj);
                    g.GetComponent<Extension>().setExtension(this, this[0], 0, RoadEditorController.Instance.vectorToDirection(this[0] - this[1]));
                    g.transform.position = this[0];
                    setExtension(0, g.GetComponent<Extension>());
                    RoadEditorController.Instance.addExtension(g);
                    g.transform.parent = transform;
                    g.SetActive(false);
                }
                if (refEmptyAt(1))
                {
                    GameObject g = Instantiate(RoadEditorController.Instance.extensionObj);
                    g.GetComponent<Extension>().setExtension(this, this[Length - 1], 1, RoadEditorController.Instance.vectorToDirection(this[Length - 1] - this[Length - 2]));
                    g.transform.position = this[Length - 1];
                    setExtension(1, g.GetComponent<Extension>());
                    RoadEditorController.Instance.addExtension(g);
                    g.transform.parent = transform;
                    g.SetActive(false);
                }
            }
            else
            {
                if (isEmptyAtExtensions(0) && refEmptyAt(0))
                {
                    GameObject g = Instantiate(RoadEditorController.Instance.extensionObj);
                    g.GetComponent<Extension>().setExtension(this, this[0], 0, RoadEditorController.Instance.vectorToDirection(this[0] - this[1]));
                    g.transform.position = this[0];
                    setExtension(0, g.GetComponent<Extension>());
                    RoadEditorController.Instance.addExtension(g);
                    g.transform.parent = transform;
                    g.SetActive(false);
                }
                if (isEmptyAtExtensions(1) && refEmptyAt(1))
                {
                    GameObject g = Instantiate(RoadEditorController.Instance.extensionObj);
                    g.GetComponent<Extension>().setExtension(this, this[Length - 1], 1, RoadEditorController.Instance.vectorToDirection(this[Length - 1] - this[Length - 2]));
                    g.transform.position = this[Length - 1];
                    setExtension(1, g.GetComponent<Extension>());
                    RoadEditorController.Instance.addExtension(g);
                    g.transform.parent = transform;
                    g.SetActive(false);
                }
            }
        }
        else if (nature == Nature.CrossRoad)
        {
            for (int i = 0; i < Exits; i++)
            {
                if (exits[i] == null)
                {
                    if (refs[i] == null)
                    {
                        GameObject g = Instantiate(RoadEditorController.Instance.extensionObj);
                        g.GetComponent<Extension>().setExtension(this, exits[i].ExitPos, i, RoadEditorController.Instance.vectorToDirection(exits[i].ExitPos - transform.position));
                        g.transform.position = exits[i].ExitPos;
                        setExtension(i, g.GetComponent<Extension>());
                        RoadEditorController.Instance.addExtension(g);
                        g.transform.parent = transform;
                        g.SetActive(false);
                    }
                }
            }
        }
    }

    public void setRef(int i, Road r)
    {
        refs[i] = r;
    }

    public bool refEmptyAt(int i)
    {
        if (refs[i] == null)
            return true;
        return false;
    }

    public void hideExits()
    {
        foreach (Extension e in exits)
        {
            e.gameObject.SetActive(false);
        }
    }

    public void bindExtis()
    {
        int j = 0;
        for (int i = 0; i < transform.childCount; i++)
        {

            if (transform.GetChild(i).name.Contains("Extension"))
            {
                transform.GetChild(i).gameObject.GetComponent<Extension>().setExtension(this, transform.GetChild(i).position, j, RoadEditorController.Instance.vectorToDirection(transform.position - transform.GetChild(i).position));
                exits.Add(transform.GetChild(i).gameObject.GetComponent<Extension>());
                j++;
                refs.Add(null);
            }
            
        }
    }

    public void updateExits()
    {
        foreach (Extension e in exits)
        {
            e.updateExt();
        }
    }

    public void addExit(Extension r)
    {
        for (int i = 0; i < exits.Count; i++)
        {
            if(exits[i] == r)
            {
                if (!exits[i].Equals(r))
                {
                    exits[i] = r;
                    return;
                }
            }
        }
        for (int i = 0; i < exits.Count; i++)
        {
            if (exits[i] == null)
            {
                exits[i] = r;
                break;
            }
        }
    }

    public void exitEndElement(Road r)
    {
        refs[0] = r;
        refs[1] = r;
    }

    public bool isEmpty()
    {
        if (points.Count == 0)
        {
            return true;
        }
        return false;
    }

    public bool isEmptyExtensions()
    {
        for (int i = 0; i < exits.Count; i++)
        {
            if (exits[i] == null)
                return true;
        }
        return false;
    }

    public bool isEmptyAtExtensions(int i)
    {
        if (exits[i] == null)
            return true;
        return false;
    }

    public int getFirstEmptyExtensionIndex()
    {
        for (int i = 0; i < exits.Count; i++)
        {
            if (exits[i] == null)
                return i;
        }
        return 0;
    }

    public void setExtension(int i, Extension ext)
    {
        exits[i] = ext;
    }

    public void setOrientation(Vector3 v)
    {
        transform.localRotation = Quaternion.LookRotation(v);
    }

    public Road(Nature n)
    {
        points = new List<Vector3>();
        nature = n;
        gameObject.name = n.ToString();
        if (n == Nature.Anchor || n == Nature.Normal)
        {
            exits = new List<Extension>(2);
            exits.Add(null);
            exits.Add(null);
            refs = new List<Road>(2);
            refs.Add(null);
            refs.Add(null);
            render = gameObject;
        }
        else if (n == Nature.End)
        {
            exits = new List<Extension>(1);
            exits.Add(null);
            refs = new List<Road>(2);
            refs.Add(null);
            refs.Add(null);
            render = gameObject;
        }
    }

    public void setRoad(Nature n)
    {
        points = new List<Vector3>();
        nature = n;
        gameObject.name = n.ToString();
        if (n == Nature.Anchor  || n == Nature.Normal)
        {
            exits = new List<Extension>(2);
            exits.Add(null);
            exits.Add(null);
            refs = new List<Road>(2);
            refs.Add(null);
            refs.Add(null);
            render = gameObject;
        }
        else if(n == Nature.End)
        {
            exits = new List<Extension>(1);
            exits.Add(null);
            refs = new List<Road>(2);
            refs.Add(null);
            refs.Add(null);
            render = gameObject;
        }
        else if(nature == Nature.CrossRoad)
        {
            exits = new List<Extension>();
            refs = new List<Road>();
            for (int i = 0; i < transform.childCount; i++)
            {
                if(transform.GetChild(i).name == "Render")
                    render = transform.GetChild(i).gameObject;
            }
            bindExtis();
        }
    }

    

    public bool verifyIfSaved(Vector3 v)
    {
        return points.Contains(v);
    }

    public void addPoint(Vector3 v)
    {
        points.Add(v);
    }

    public void invertRoadOrder()
    {
        List<Vector3> rev = new List<Vector3>();

        for (int i = points.Count-1; i >= 0 ; i--)
        {
            rev.Add(points[i]);
        }

        points = new List<Vector3>(rev);

        if (nature == Nature.Anchor || nature == Nature.Normal)
        {
            Extension eTemp = exits[0];
            exits[0] = exits[1];
            exits[1] = eTemp;

            Road rTemp = refs[0];
            refs[0] = refs[1];
            refs[1] = rTemp;
        }

    }

    public Path processPoints()
    {
        if (nature == Nature.Normal)
        {
            path = new Path(Vector3.zero); //Create new path to exploit
            if (points.Count > 1)
            {
                Vector3 direction = points[1] - points[0]; // Get first direction from start to second point
                bool changeDirection = false;

                List<Vector3> pathPoints = new List<Vector3>();

                pathPoints.Add(points[0]);
                // Add first ANCHOR
                pathPoints.Add(new Vector3(points[0].x + (points[1].x - points[0].x) / 2f,
                                            points[0].y + (points[1].y - points[0].y) / 2f,
                                            points[0].z + (points[1].z - points[0].z) / 2f));
                // Add first PIVOT
                if (points.Count > 2)
                {
                    if (Vector3.Cross(direction.normalized, (points[2] - points[1]).normalized) != Vector3.zero) // first is short
                    {
                        direction = points[2] - points[1];//New direction
                        pathPoints.Add(points[1]);//PIVOT
                        pathPoints.Add(new Vector3(points[1].x + (points[2].x - points[1].x) / 2f,
                                                    points[1].y + (points[2].y - points[1].y) / 2f,
                                                    points[1].z + (points[2].z - points[1].z) / 2f));//ANCHOR
                        pathPoints.Add(points[2]);//PIVOT
                    }
                    for (int i = 2; i < points.Count - 2;)
                    {
                        if (Vector3.Cross(direction.normalized, (points[i + 1] - points[i]).normalized) == Vector3.zero) // if on same line so go to next point
                        {
                            changeDirection = false;
                            i++;
                        }
                        else if (Vector3.Cross(direction.normalized, (points[i + 1] - points[i]).normalized) != Vector3.zero)
                        {
                            if (changeDirection)
                            {
                                pathPoints.Add(points[i]);//PIVOT
                                pathPoints.Add(new Vector3(points[i].x + (points[i+1].x - points[i].x) / 2f,
                                                            points[i].y + (points[i+1].y - points[i].y) / 2f,
                                                            points[i].z + (points[i+1].z - points[i].z) / 2f));//ANCHOR
                                pathPoints.Add(points[i+1]);//PIVOT


                                direction = points[i + 1] - points[i];

                                i++;
                            }
                            else
                            {

                                i--;
                                pathPoints.Add(new Vector3(points[i - 1].x + (points[i].x - points[i - 1].x) / 4f,
                                                            points[i - 1].y + (points[i].y - points[i - 1].y) / 4f,
                                                            points[i - 1].z + (points[i].z - points[i - 1].z) / 4f));//PIVOT
                                pathPoints.Add(points[i]);//ANCHOR
                                pathPoints.Add(new Vector3(points[i].x + (points[i + 1].x - points[i].x) / (4f / 3f),
                                                            points[i].y + (points[i + 1].y - points[i].y) / (4f / 3f),
                                                            points[i].z + (points[i + 1].z - points[i].z) / (4f / 3f)));//PIVOT

                                i++;
                                i++;
                                pathPoints.Add(new Vector3(points[i - 1].x + (points[i].x - points[i - 1].x) / 4f,
                                                            points[i - 1].y + (points[i].y - points[i - 1].y) / 4f,
                                                            points[i - 1].z + (points[i].z - points[i - 1].z) / 4f));//PIVOT
                                pathPoints.Add(points[i]);//ANCHOR
                                pathPoints.Add(new Vector3(points[i].x + (points[i + 1].x - points[i].x) / (4f / 3f),
                                                            points[i].y + (points[i + 1].y - points[i].y) / (4f / 3f),
                                                            points[i].z + (points[i + 1].z - points[i].z) / (4f / 3f)));//PIVOT

                                direction = points[i + 1] - points[i];

                                i++;
                                changeDirection = true;
                            }
                        }
                    }

                    if (Vector3.Cross((points[points.Count - 2] - points[points.Count - 3]).normalized, (points[points.Count - 1] - points[points.Count - 2]).normalized) != Vector3.zero) // last is short
                    {
                        pathPoints.Add(points[points.Count - 3]);//PIVOT
                        pathPoints.Add(new Vector3(points[points.Count - 3].x + (points[points.Count - 2].x - points[points.Count - 3].x) / 2f,
                                                    points[points.Count - 3].y + (points[points.Count - 2].y - points[points.Count - 3].y) / 2f,
                                                    points[points.Count - 3].z + (points[points.Count - 2].z - points[points.Count - 3].z) / 2f));//ANCHOR
                        pathPoints.Add(points[points.Count - 2]);//PIVOT
                    }
                }
                pathPoints.Add(new Vector3(points[points.Count - 2].x + (points[points.Count - 1].x - points[points.Count - 2].x) / 2f,
                                            points[points.Count - 2].y + (points[points.Count - 1].y - points[points.Count - 2].y) / 2f,
                                            points[points.Count - 2].z + (points[points.Count - 1].z - points[points.Count - 2].z) / 2f));
                // Add last PIVOT
                pathPoints.Add(points[points.Count - 1]);
                //add last ANCHOR

                if (debug) ////////DEBUG///////
                {
                    for (int i = 0; i < pathPoints.Count; i++)
                    {
                        GameObject g;
                        g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        g.transform.position = pathPoints[i];
                        g.name = i.ToString();
                    }

                    for (int i = 0; i < pathPoints.Count; i++)
                    {
                        GameObject g;
                        if (i % 3 == 0)
                            g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        else
                            g = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                        g.transform.position = pathPoints[i];
                    }
                }

                if (points.Count > 2)
                {
                    for (int i = 1; i < pathPoints.Count / 3; i++)
                    {
                        path.AddSegment(Vector3.zero);
                    }
                }

                for (int i = 0; i < path.NumPoints; i++)
                {
                    path[i] = pathPoints[i];
                }
            }

        }
        if (nature == Nature.Anchor)
        {
            path = new Path(Vector3.zero);

            if (debug) ////////DEBUG///////
            {
                for (int i = 0; i < points.Count; i++)
                {
                    GameObject g;
                    if (i % 3 == 0)
                        g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    else
                        g = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                    g.transform.position = points[i];
                }
            }

            if (points.Count > 4)
            {
                for (int i = 1; i < points.Count / 3; i++)
                {
                    path.AddSegment(Vector3.zero);
                }
            }

            for (int i = 0; i < path.NumPoints; i++)
            {
                path[i] = points[i];
            }
        }
        
        return path;
        
    }

    public bool chekRoad()
    {
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = 0; j < points.Count; j++)
            {
                if (points[i] == points[j] && i != j)
                {
                    makeRoadTintRed();
                    return false;
                }
            }
        }
        makeRoadTintGreen();
        return true;
    }

    Vector3 findClosestPointInRoad(Vector3 hitPoint)
    {
        Vector3 closestPoint = Vector3.zero;
        float dist = float.MaxValue;

        foreach(Vector3 v in points)
        {
            float currentDist = Vector3.Distance(hitPoint, v);
            if (currentDist < dist)
            {
                dist = currentDist;
                closestPoint = v;
            }
        }

        return closestPoint;
    }

    public Road cutAtPoint(Vector3 cutPoint)
    {
        List<Vector3> currentRoad = new List<Vector3>();
        List<Vector3> restRoad = new List<Vector3>();
        bool first = true;

        for (int i = 0; i < points.Count; i++)
        {
            if(points[i] != cutPoint)
            {
                if (first)
                    currentRoad.Add( points[i]);
                else
                    restRoad.Add(points[i]);
            }
            else
            {
                first = false;
            }
        }

        points = currentRoad;

        Road rest = new Road(nature);
        for (int i = 0; i < restRoad.Count; i++)
        {
            rest.addPoint(restRoad[i]);
        }
        rest.setExtension(1, exits[1]);
        exits[0] = null;

        rest.setRef(0, refs[0]);
        refs[1] = null;

        rendererExtensions();
        rest.rendererExtensions();

        return rest;
    }

}

public enum Nature
{
    Anchor , Normal , End , CrossRoad
}