using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCrossroad : MonoBehaviour {
    RoadEditorController roadEditorController;
    GameObject toUse;
    public List<CrossRoad> crossroads;
    public int indexCrossroad;
    Vector3 positionToSpawn;
    Phase currentPhase = Phase.Init;

    public void rotate45plus()
    {
        toUse.transform.eulerAngles += new Vector3(0, 45f, 0);
    }

    public void rotate45moin()
    {
        toUse.transform.eulerAngles -= new Vector3(0, 45f, 0);
    }

    void spawnCrossroad()
    {
        if (toUse != null)
            Destroy(toUse);
        toUse = Instantiate(crossroads[indexCrossroad].prefab);
        toUse.transform.position = positionToSpawn;
        toUse.GetComponent<Road>().setRoad(Nature.CrossRoad);
        
    }

    void destroyCrossroad()
    {
        Destroy(toUse);
        toUse = null;
    }

    void addIndex()
    {
        indexCrossroad++;
        indexCrossroad = indexCrossroad % crossroads.Count;
        if(toUse != null)
        {
            destroyCrossroad();
        }
        spawnCrossroad();
    }

    void subIndex()
    {
        indexCrossroad--;
        indexCrossroad = indexCrossroad % crossroads.Count;
        if (toUse != null)
        {
            destroyCrossroad();
        }
        spawnCrossroad();
    }

    void flip()
    {
        toUse.transform.localScale = new Vector3(-toUse.transform.localScale.x, toUse.transform.localScale.y, toUse.transform.localScale.z);
    }

    List<Extension> getClosestExtensions()
    {
        List<Extension> closest = new List<Extension>();
        foreach (GameObject g in roadEditorController.getExtensions())
        {
            if (g != null)
            {
                if (g.transform.position == transform.position + new Vector3(0, 0, roadEditorController.offset))
                    closest.Add(g.GetComponent<Extension>());
                else if (g.transform.position == positionToSpawn + new Vector3(0, 0, -roadEditorController.offset))
                    closest.Add(g.GetComponent<Extension>());
                else if (g.transform.position == positionToSpawn + new Vector3(-roadEditorController.offset, 0, 0))
                    closest.Add(g.GetComponent<Extension>());
                else if (g.transform.position == positionToSpawn + new Vector3(roadEditorController.offset, 0, 0))
                    closest.Add(g.GetComponent<Extension>());
                else if (g.transform.position == positionToSpawn + new Vector3(roadEditorController.offset, 0, roadEditorController.offset))
                    closest.Add(g.GetComponent<Extension>());
                else if (g.transform.position == positionToSpawn + new Vector3(roadEditorController.offset, 0, -roadEditorController.offset))
                    closest.Add(g.GetComponent<Extension>());
                else if (g.transform.position == positionToSpawn + new Vector3(-roadEditorController.offset, 0, roadEditorController.offset))
                    closest.Add(g.GetComponent<Extension>());
                else if (g.transform.position == positionToSpawn + new Vector3(-roadEditorController.offset, 0, -roadEditorController.offset))
                    closest.Add(g.GetComponent<Extension>());
            }
        }
        foreach (Extension e in closest)
        {
            e.gameObject.SetActive(true);
        }
        return closest;
    }

    void renderClosest()
    {
        List<Extension> closest = getClosestExtensions();
        foreach (Extension e in closest)
        {
            e.ExitRoadRef.rendererExtensions();
        }
    }

    bool checkIfMatch()
    {
        toUse.GetComponent<Road>().updateExits();
        List<Extension> closest = getClosestExtensions();
        int b = 0;
        for (int j = 0; j < toUse.GetComponent<Road>().getExtensionsPlaces().Count; j++)
        {
            Debug.Log(toUse.GetComponent<Road>().getExtensionsPlaces()[j].ExitPos);
            for (int i = 0; i < closest.Count; i++)
            {
                Debug.Log(closest[i].ExitPos);
                if (toUse.GetComponent<Road>().getExtensionsPlaces()[j].ExitPos == closest[i].ExitPos)
                {
                    Debug.Log("YES");
                    b++;
                }
            }
        }
        if (b == closest.Count)
        {
            toUse.GetComponent<Road>().makeRoadTintGreen();
            return true;
        }
        else
        {
            toUse.GetComponent<Road>().makeRoadTintRed();
            return false;
        }
    }
//Next TASKS 
//Cut next week

    public bool usage(RaycastHit hit, Extension extension, GameObject road)
    {
        if (currentPhase == Phase.Init)
        {
            positionToSpawn = road.transform.position;
            roadEditorController = RoadEditorController.Instance;
            spawnCrossroad();
            checkIfMatch();
            Destroy(road);
            currentPhase = Phase.Adappting;
        }
        else if (currentPhase == Phase.Adappting)
        {
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                addIndex();
                checkIfMatch();
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                subIndex();
                checkIfMatch();
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                rotate45plus();
                checkIfMatch();
            }
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                rotate45moin();
                checkIfMatch();
            }
            if(Input.GetKeyDown(KeyCode.F))
            {
                flip();
            }

            if (Input.GetKeyDown(KeyCode.Return) && checkIfMatch())
                currentPhase = Phase.ConfrimeAll;
        }
        else if (currentPhase == Phase.ConfrimeAll)
        {
            roadEditorController.addRoad(toUse);
            roadEditorController.addExtensions(toUse.GetComponent<Road>().getExtensionsPlaces());
            toUse.GetComponent<Road>().makeRoadTintWhite();
            toUse.GetComponent<Road>().hideExits();
            currentPhase = Phase.Init;
            toUse = null;
            return true;
        }
        return false;
    }

    enum Phase
    {
        Init,Adappting,ConfrimeAll
    }

    [System.Serializable]
    public class CrossRoad
    {
        public string name;
        public GameObject prefab;
        public int entrys = 0;

    }
}
