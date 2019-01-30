using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadEditorController : MonoBehaviour {

    static RoadEditorController instance = null;
    public static RoadEditorController Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("RoadEditor").GetComponent<RoadEditorController>();
            return instance;
        }
    }

    public GameObject gizmoDirection;

    public GameObject D_N;
    public GameObject D_NE;
    public GameObject D_E;
    public GameObject D_ES;
    public GameObject D_S;
    public GameObject D_SW;
    public GameObject D_W;
    public GameObject D_WN;

    public GameObject extensionObj;
    public GameObject roadEnd;
    public GameObject roadBase;

    public LayerMask extensionMask;

    List<GameObject> roads = new List<GameObject>();
    List<GameObject> extensions = new List<GameObject>();

    GameObject currentlySelcted = null;
    GameObject toUseRoad = null;
    Extension extensionTemp = null;

    public int offset = 5;
    public int size = 100;
    public int toolUsed = 1;
    int currentheight = 0;
    int lastHeight = 0;

    Phase curretnPhase = Phase.StartPoint;

    Vector3[,] matrix;
    public Color baseColor = Color.green;
    Vector3[] visualGrid;
    public bool showGrid = false;

    /// <summary>
    /// ADD TAG ::: Terrain / GizmoDirection / Extension / RoadController / Walker
    /// ADD Layer ::: Walker / Extensions / Road / Crossroad
    /// </summary>

    //VIUSAL AIDS

    public float getLastHeight()
    {
        return lastHeight;
    }

    public float getHeight()
    {
        return currentheight;
    }

    public void addHeight()
    {
        if(currentheight != lastHeight + offset)
        {
            currentheight += offset;
        }
    }

    public void subHeight()
    {
        if (currentheight != lastHeight - offset)
        {
            currentheight -= offset;
        }
    }

    public void setHeigth(int height)
    {
        currentheight = height;
        lastHeight = currentheight;
    }

    public void lastHeightConfirm()
    {
        lastHeight = currentheight;
    }

    void matrixGeneration()
    {
        Vector3[,] matrix = new Vector3[size, size];
        Vector3[,] matrixGrid = new Vector3[4, size];
        for (int y = 0; y < size * offset; y += offset)
        {
            for (int x = 0; x < size * offset; x += offset)
            {
                matrix[x / offset, y / offset] = new Vector3(x, 0, y);
            }
        }

        for (int x = 0, i = 0; x < size * offset; i++, x += offset)
        {

            matrixGrid[0, i] = new Vector3(x, 0, 0);
            matrixGrid[1, i] = new Vector3(x, 0, size * offset);
        }
        for (int x = 0, i = 0; i < size; i++, x += offset)
        {
            matrixGrid[2, i] = new Vector3(0, 0, x);
            matrixGrid[3, i] = new Vector3(size * offset, 0, x);
        }


        drawGrid(matrixGrid);
    }

    void drawGrid(Vector3[,] matrixGrid)
    {
        visualGrid = new Vector3[size * 4];

        for (int k = 0, j = 0, i = 0; k < visualGrid.Length - 2; k++, i++)
        {
            visualGrid[k] = matrixGrid[j, i];
            k++;
            visualGrid[k] = matrixGrid[j + 1, i];
            if (199 == k % 200)
            {
                i = 0;
                j += 2;
            }
        }
    }

    void OnPostRender()
    {
        if (showGrid)
            RenderLines(visualGrid);
    }

    void OnDrawGizmos()
    {
        if (showGrid)
            RenderLines(visualGrid);
    }

    void RenderLines(Vector3[] points) //Visual aid grid
    {
        if (!ValidateInput(points))
        {
            return;
        }

        GL.Begin(GL.LINES);
        GetComponent<MeshRenderer>().material.SetPass(0);
        for (int i = 0; i < points.Length; i += 2)
        {
            GL.Color(baseColor);
            GL.Vertex(points[i]);
            GL.Vertex(points[i + 1]);
        }
        GL.End();
    }

    private bool ValidateInput(Vector3[] points)
    {
        return points != null;
    }
    //VIUSAL AIDS

    public List<GameObject> getExtensions()
    {
        return extensions;
    }

    public Vector3 directionToVector(Direction direction)
    {
        Vector3 offsetO = Vector3.zero;

        if (Direction.N == direction)
        {
            offsetO += new Vector3(0, 0, offset);
        }
        if (Direction.S == direction)
        {
            offsetO += new Vector3(0, 0, -offset);
        }
        if (Direction.W == direction)
        {
            offsetO += new Vector3(-offset, 0, 0);
        }
        if (Direction.E == direction)
        {
            offsetO += new Vector3(offset, 0, 0);
        }
        if (Direction.NE == direction)
        {
            offsetO += new Vector3(offset, 0, offset);
        }
        if (Direction.ES == direction)
        {
            offsetO += new Vector3(offset, 0, -offset);
        }
        if (Direction.WN == direction)
        {
            offsetO += new Vector3(-offset, 0, offset);
        }
        if (Direction.SW == direction)
        {
            offsetO += new Vector3(-offset, 0, -offset);
        }

        return offsetO;
    }

    public Direction vectorToDirection(Vector3 direction)
    {
        Vector3 offsetO = Vector3.zero;

        direction = new Vector3(direction.x, 0, direction.z);

        if (Vector3.Cross(direction.normalized, new Vector3(0, 0, offset).normalized) == Vector3.zero && Vector3.Dot(direction, new Vector3(0, 0, offset)) > 0)
        {
            return Direction.N;
        }
        if (Vector3.Cross(direction.normalized, new Vector3(0, 0, -offset).normalized) == Vector3.zero && Vector3.Dot(direction, new Vector3(0, 0, -offset)) > 0)
        {
            return Direction.S;
        }
        if (Vector3.Cross(direction.normalized, new Vector3(-offset, 0, 0).normalized) == Vector3.zero && Vector3.Dot(direction, new Vector3(-offset, 0, 0)) > 0)
        {
            return Direction.W;
        }
        if (Vector3.Cross(direction.normalized, new Vector3(offset, 0, 0)) == Vector3.zero && Vector3.Dot(direction, new Vector3(offset, 0, 0)) > 0)
        {
            return Direction.E;
        }
        if (Vector3.Cross(direction.normalized, new Vector3(offset, 0, offset).normalized) == Vector3.zero && Vector3.Dot(direction, new Vector3(offset, 0, offset)) > 0)
        {
            return Direction.NE;
        }
        if (Vector3.Cross(direction.normalized, new Vector3(offset, 0, -offset).normalized) == Vector3.zero && Vector3.Dot(direction, new Vector3(offset, 0, -offset)) > 0)
        {
            return Direction.ES;
        }
        if (Vector3.Cross(direction.normalized, new Vector3(-offset, 0, offset).normalized) == Vector3.zero && Vector3.Dot(direction, new Vector3(-offset, 0, offset)) > 0)
        {
            return Direction.WN;
        }
        if (Vector3.Cross(direction.normalized, new Vector3(-offset, 0, -offset).normalized) == Vector3.zero && Vector3.Dot(direction, new Vector3(-offset, 0, -offset)) > 0)
        {
            return Direction.SW;
        }
        return Direction.NULL;

    }

    public Direction getOppositDirection(Direction od)
    {
        if (Direction.N == od)
        {
            return Direction.S;
        }
        if (Direction.S == od)
        {
            return Direction.N;
        }
        if (Direction.W == od)
        {
            return Direction.E;
        }
        if (Direction.E == od)
        {
            return Direction.W;
        }
        if (Direction.NE == od)
        {
            return Direction.SW;
        }
        if (Direction.ES == od)
        {
            return Direction.WN;
        }
        if (Direction.WN == od)
        {
            return Direction.ES;
        }
        if (Direction.SW == od)
        {
            return Direction.NE;
        }

        return Direction.NULL;
    }

    public bool noExt()
    {
        if (extensions.Count == 0)
            return true;
        return false;
    }

    public Direction pickDirection(RaycastHit hit)
    {

        if (hit.collider.tag == "GizmoDirection")
        {
            if (Direction.N.ToString() == hit.transform.name)
            {
                return Direction.N;
            }
            if (Direction.S.ToString() == hit.transform.name)
            {
                return Direction.S;
            }
            if (Direction.W.ToString() == hit.transform.name)
            {
                return Direction.W;
            }
            if (Direction.E.ToString() == hit.transform.name)
            {
                return Direction.E;
            }
            if (Direction.NE.ToString() == hit.transform.name)
            {
                return Direction.NE;
            }
            if (Direction.ES.ToString() == hit.transform.name)
            {
                return Direction.ES;
            }
            if (Direction.WN.ToString() == hit.transform.name)
            {
                return Direction.WN;
            }
            if (Direction.SW.ToString() == hit.transform.name)
            {
                return  Direction.SW;
            }
        }//STATEMENTS
        return Direction.NULL;
    }

    public void updateGizmo(Direction direction)
    {
        if (Direction.N == direction)
        {
            D_N.SetActive(true);
            D_NE.SetActive(true);
            D_E.SetActive(true);
            D_ES.SetActive(false);
            D_S.SetActive(false);
            D_SW.SetActive(false);
            D_W.SetActive(true);
            D_WN.SetActive(true);

        }
        if (Direction.S == direction)
        {
            D_N.SetActive(false);
            D_NE.SetActive(false);
            D_E.SetActive(true);
            D_ES.SetActive(true);
            D_S.SetActive(true);
            D_SW.SetActive(true);
            D_W.SetActive(true);
            D_WN.SetActive(false);
        }
        if (Direction.W == direction)
        {
            D_N.SetActive(true);
            D_NE.SetActive(false);
            D_E.SetActive(false);
            D_ES.SetActive(false);
            D_S.SetActive(true);
            D_SW.SetActive(true);
            D_W.SetActive(true);
            D_WN.SetActive(true);
        }
        if (Direction.E == direction)
        {
            D_N.SetActive(true);
            D_NE.SetActive(true);
            D_E.SetActive(true);
            D_ES.SetActive(true);
            D_S.SetActive(true);
            D_SW.SetActive(false);
            D_W.SetActive(false);
            D_WN.SetActive(false);
        }
        if (Direction.NE == direction)
        {
            D_N.SetActive(true);
            D_NE.SetActive(true);
            D_E.SetActive(true);
            D_ES.SetActive(true);
            D_S.SetActive(false);
            D_SW.SetActive(false);
            D_W.SetActive(false);
            D_WN.SetActive(true);
        }
        if (Direction.ES == direction)
        {
            D_N.SetActive(false);
            D_NE.SetActive(true);
            D_E.SetActive(true);
            D_ES.SetActive(true);
            D_S.SetActive(true);
            D_SW.SetActive(true);
            D_W.SetActive(false);
            D_WN.SetActive(false);
        }
        if (Direction.WN == direction)
        {
            D_N.SetActive(true);
            D_NE.SetActive(true);
            D_E.SetActive(false);
            D_ES.SetActive(false);
            D_S.SetActive(false);
            D_SW.SetActive(true);
            D_W.SetActive(true);
            D_WN.SetActive(true);
        }
        if (Direction.SW == direction)
        {
            D_N.SetActive(false);
            D_NE.SetActive(false);
            D_E.SetActive(false);
            D_ES.SetActive(true);
            D_S.SetActive(true);
            D_SW.SetActive(true);
            D_W.SetActive(true);
            D_WN.SetActive(true);
        }
    }

    public Vector3 SnapPosition(Vector3 input, float factor = 1f)
    {
        if (factor <= 0f)
            throw new UnityException("factor argument must be above 0");

        float x = Mathf.Round(input.x / factor) * factor;
        float y = Mathf.Round(input.y / factor) * factor;
        float z = Mathf.Round(input.z / factor) * factor;

        return new Vector3(x, y, z);
    }

    public void showHideGizmo()
    {
        gizmoDirection.SetActive(!gizmoDirection.activeSelf);
        D_N.SetActive(true);
        D_NE.SetActive(true);
        D_E.SetActive(true);
        D_ES.SetActive(true);
        D_S.SetActive(true);
        D_SW.SetActive(true);
        D_W.SetActive(true);
        D_WN.SetActive(true);
    }

    bool toolSlected(int index)
    {
        switch (index)
        {
            case 1:
                //UseTool1
                return true;
            case 2:
                //UseTool2
                return true;
            case 3:
                //UseTool3
                return true;
            case 4:
                //UseTool4
                return true;
            case 5:
                //UseTool5
                return true;
            case 6:
                //UseTool6
                return true;
            default:
                break;
        }
        return false;
    }

    void showHideExtensions()
    {
        foreach (var ext in extensions)
        {
            if(ext != null)
                ext.SetActive(!ext.activeSelf);
        }
    }

    public void addExtension(GameObject ext)
    {
        extensions.Add(ext);
    }

    public void addExtensions(List<Extension> exts)
    {
        foreach (Extension e in exts)
        {
            extensions.Add(e.gameObject);
        }
    }

    void bindExtensions()
    {
        for (int i = 0; i < extensions.Count; i++)
        {
            if (extensions[i] != null)
            {
                extensionTemp = extensions[i].GetComponent<Extension>();
                for (int j = 0; j < extensions.Count; j++)
                {
                    if (extensions[j] != null && !extensions[j].GetComponent<Extension>().Equals(extensionTemp) )
                    {
                        if (extensionTemp.ExitPos == extensions[j].GetComponent<Extension>().ExitPos)
                        {
                            extensionTemp.ExitRoadRef.setRef(extensionTemp.ExitRefIndex, extensions[j].GetComponent<Extension>().ExitRoadRef);
                            extensions[j].GetComponent<Extension>().ExitRoadRef.setRef(extensions[j].GetComponent<Extension>().ExitRefIndex, extensionTemp.ExitRoadRef);
                            Destroy(extensions[j]);
                            Destroy(extensionTemp);
                        }
                    }
                }
            }
        }
    }

    public void addRoad(GameObject road)
    {
        roads.Add(road);
    }

    void Start () {
        matrixGeneration();
    }
	
	void Update ()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        if(curretnPhase == Phase.StartPoint)
        {
            
            showGrid = true;
            if (hit.collider.tag == "Terrain")
            {
                if (Input.GetMouseButtonDown(1))//INPUT
                {
                    currentlySelcted = Instantiate(extensionObj);
                    gizmoDirection.transform.position = SnapPosition(new Vector3(hit.point.x, 0.1f, hit.point.z), offset);
                    currentlySelcted.transform.position = SnapPosition(new Vector3(hit.point.x, 0.1f, hit.point.z), offset);
                    currentlySelcted.transform.parent = transform;
                    showHideGizmo();
                    curretnPhase = Phase.StartDirection;
                }
            }
        }
        else if (curretnPhase == Phase.StartDirection)
        {
            showGrid = false;
            if (Input.GetMouseButtonDown(1))//INPUT
            {
                Direction d = pickDirection(hit);
                if (d != Direction.NULL)
                {
                    currentlySelcted.GetComponent<Extension>().setExtension(null,currentlySelcted.transform.position, 0,d);
                    showHideGizmo();
                    curretnPhase = Phase.ExtensionsRendering;
                }
            }
        }
        else if (curretnPhase == Phase.ExtensionsRendering)
        {
            showHideExtensions();
            curretnPhase = Phase.ExtensionSelecting;
        }
        else if (curretnPhase == Phase.ExtensionSelecting)
        {

            showGrid = false;
            if (Input.GetMouseButtonDown(1))//INPUT
            {
                if (hit.collider.tag == "Extension")
                {
                    currentlySelcted = hit.transform.gameObject;
                    showHideExtensions();
                    curretnPhase = Phase.ToolSelecting;
                }
            }
        }
        else if (curretnPhase == Phase.ToolSelecting)
        {
            if (toolSlected(toolUsed))//INPUT
            {
               curretnPhase = Phase.ToolUsing;
            }
        }
        else if (curretnPhase == Phase.ToolUsing)
        {
            if (toolUsed == 1)//direction
            {
                showGrid = true;
                if (currentlySelcted != null)
                {
                    if (currentlySelcted.GetComponent<Extension>().isEmpty())//If no reference create new road
                    {
                        toUseRoad = Instantiate(roadBase);
                        toUseRoad.GetComponent<Road>().setRoad(Nature.Normal);
                        roads.Add(toUseRoad);
                    }
                    else
                    {
                        if (currentlySelcted.GetComponent<Extension>().ExitRoadRef.Nature == Nature.Normal)
                        {
                            toUseRoad = currentlySelcted.GetComponent<Extension>().ExitRoadRef.gameObject;
                        }
                        else
                        {
                            toUseRoad = Instantiate(roadBase);
                            toUseRoad.GetComponent<Road>().setRoad(Nature.Normal);
                            toUseRoad.GetComponent<Road>().setRef(0, currentlySelcted.GetComponent<Extension>().ExitRoadRef);
                            currentlySelcted.GetComponent<Extension>().ExitRoadRef.setRef(currentlySelcted.GetComponent<Extension>().ExitRefIndex, toUseRoad.GetComponent<Road>());
                            roads.Add(toUseRoad);
                        }
                    }

                    currentlySelcted.GetComponent<Extension>().delete(out extensionTemp);
                }
                if (GetComponent<DrawByDirection>().usage(hit, extensionTemp, toUseRoad))
                {
                    toUseRoad = null;
                    currentlySelcted = null;
                    curretnPhase = Phase.ExtensionsRendering;
                    bindExtensions();
                }
            }
            else if (toolUsed == 2)//anchor
            {
                if (currentlySelcted != null)
                {
                    if (currentlySelcted.GetComponent<Extension>().isEmpty())//If no reference create new road
                    {
                        toUseRoad = Instantiate(roadBase);
                        toUseRoad.GetComponent<Road>().setRoad(Nature.Anchor);
                        roads.Add(toUseRoad);
                    }
                    else
                    {
                        if (currentlySelcted.GetComponent<Extension>().ExitRoadRef.Nature == Nature.Anchor)
                        {
                            toUseRoad = currentlySelcted.GetComponent<Extension>().ExitRoadRef.gameObject;
                        }
                        else
                        {
                            toUseRoad = Instantiate(roadBase);
                            toUseRoad.GetComponent<Road>().setRoad(Nature.Anchor);
                            toUseRoad.GetComponent<Road>().setRef(0, currentlySelcted.GetComponent<Extension>().ExitRoadRef);
                            currentlySelcted.GetComponent<Extension>().ExitRoadRef.setRef(currentlySelcted.GetComponent<Extension>().ExitRefIndex, toUseRoad.GetComponent<Road>());
                            roads.Add(toUseRoad);
                        }
                    }

                    currentlySelcted.GetComponent<Extension>().delete(out extensionTemp);
                }
                if (GetComponent<DrawByAnchorPoint>().usage(hit, extensionTemp, toUseRoad))
                {
                    toUseRoad = null;
                    currentlySelcted = null;
                    curretnPhase = Phase.ExtensionsRendering;
                    bindExtensions();
                }
            }
            else if (toolUsed == 3)//end
            {
                if (currentlySelcted != null)
                {
                    currentlySelcted.GetComponent<Extension>().delete(out extensionTemp);
                    GameObject end = Instantiate(roadEnd);
                    end.transform.position = extensionTemp.ExitPos;
                    end.GetComponent<Road>().setRoad(Nature.End);
                    end.GetComponent<Road>().exitEndElement(extensionTemp.ExitRoadRef);
                    end.GetComponent<Road>().setOrientation(directionToVector(extensionTemp.ExitOrientation));
                    extensionTemp.ExitRoadRef.setRef(extensionTemp.ExitRefIndex, end.GetComponent<Road>());
                    roads.Add(end);
                    curretnPhase = Phase.ExtensionsRendering;
                }
            }
            else if (toolUsed == 4)//crossroad
            {
                if (toUseRoad == null)
                {
                    toUseRoad = new GameObject();
                    toUseRoad.transform.position = currentlySelcted.GetComponent<Extension>().ExitPos + directionToVector(currentlySelcted.GetComponent<Extension>().ExitOrientation);
                }
                if (GetComponent<DrawCrossroad>().usage(hit, currentlySelcted.GetComponent<Extension>(), toUseRoad))
                {
                    toUseRoad = null;
                    currentlySelcted = null;
                    curretnPhase = Phase.ExtensionsRendering;
                    bindExtensions();
                }
            }
            else if (toolUsed == 5)//delete
            {
                if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Road"))
                {
                    hit.transform.gameObject.GetComponent<Road>().makeRoadTintRed();
                    if(Input.GetMouseButtonDown(0))
                    {
                        Destroy(hit.transform.gameObject);
                    }
                }
                else
                {
                    foreach (var r in roads)
                    {
                        r.GetComponent<Road>().makeRoadTintWhite();
                    }
                }
            }
            else if (toolUsed == 6)//cut
            {
                if (toUseRoad == null)
                {
                    toUseRoad = new GameObject();
                    toUseRoad.transform.position = extensionTemp.ExitPos + directionToVector(currentlySelcted.GetComponent<Extension>().ExitOrientation);
                }
                if (GetComponent<DrawCrossroad>().usage(hit, currentlySelcted.GetComponent<Extension>(), toUseRoad))
                {
                    toUseRoad = null;
                    currentlySelcted = null;
                    curretnPhase = Phase.ExtensionsRendering;
                    bindExtensions();
                }
            }

        }
    }

    enum Phase
    {
        StartPoint, StartDirection, ExtensionsRendering, ExtensionSelecting, ToolSelecting, ToolUsing
    }
}

public enum Direction
{
    N, NE, E, ES, S, SW, W, WN, NULL
}



public enum Tools
{
    N , A , EP , CRP
}