using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawByAnchorPoint : MonoBehaviour {
    RoadEditorController roadEditorController;
    Phase currentPhase = Phase.Init;

    float maxPivot = 25f;

    float magnitudeA = 10f;
    float magnitudeB = 10f;
    Vector3 anchorA = Vector3.zero;
    Vector3 anchorB = Vector3.zero;
    Vector3 pivotA = Vector3.zero;
    Vector3 pivotB = Vector3.zero;
    Direction dirA;
    Direction dirB;

    void updatePreview(GameObject road)
    {
        road.GetComponent<RoadCreator>().RenderRoad(road.GetComponent<Road>().processPoints());
    }

    public bool usage(RaycastHit hit, Extension extension, GameObject road)
    {
        if (currentPhase == Phase.Init)
        {
            roadEditorController = RoadEditorController.Instance;
            roadEditorController.showGrid = true;


            if (road.GetComponent<Road>().isEmpty())
            {
                dirA = extension.ExitOrientation;
                dirB = roadEditorController.getOppositDirection(extension.ExitOrientation);
                anchorA = extension.ExitPos;
                road.GetComponent<Road>().addPoint(anchorA);
                road.GetComponent<Road>().addPoint(roadEditorController.directionToVector(dirA) * magnitudeA + anchorA);
                road.GetComponent<Road>().addPoint(roadEditorController.directionToVector(dirB) * magnitudeB + new Vector3(hit.point.x, roadEditorController.getHeight(), hit.point.z));
                road.GetComponent<Road>().addPoint(new Vector3(hit.point.x, roadEditorController.getHeight(), hit.point.z));
            }
            else
            {
                anchorA = road.GetComponent<Road>()[road.GetComponent<Road>().Length - 4];
            }


            currentPhase = Phase.Streaching;
        }
        else if(currentPhase == Phase.InitStreaching)
        {
            dirA = roadEditorController.getOppositDirection(dirB);
            dirB = roadEditorController.getOppositDirection(dirA);
            anchorA = road.GetComponent<Road>()[road.GetComponent<Road>().Length-1];
            pivotA = roadEditorController.directionToVector(dirA) * magnitudeA + anchorA;
            pivotB = roadEditorController.directionToVector(dirB) * magnitudeB + anchorB;
            road.GetComponent<Road>().addPoint(pivotA);
            road.GetComponent<Road>().addPoint(pivotB);
            road.GetComponent<Road>().addPoint(anchorA);
            currentPhase = Phase.Streaching;
        }
        else if (currentPhase == Phase.Streaching)
        {
            anchorB = roadEditorController.SnapPosition(new Vector3(hit.point.x, roadEditorController.getHeight(), hit.point.z),roadEditorController.offset);

            pivotA = roadEditorController.directionToVector(dirA) * magnitudeA + anchorA;
            pivotB = roadEditorController.directionToVector(dirB) * magnitudeB + anchorB;

            road.GetComponent<Road>()[road.GetComponent<Road>().Length - 3] = pivotA;
            road.GetComponent<Road>()[road.GetComponent<Road>().Length - 2] = pivotB;
            road.GetComponent<Road>()[road.GetComponent<Road>().Length - 1] = anchorB;

            if (Input.GetKeyDown(KeyCode.G))
                roadEditorController.showGrid = !roadEditorController.showGrid;

            if (Input.GetKey(KeyCode.A))            //INPUT Slider Left
                magnitudeA = Mathf.Clamp(magnitudeA + .5f, 1f, maxPivot);
            if (Input.GetKey(KeyCode.S))
                magnitudeA = Mathf.Clamp(magnitudeA - .5f, 1f, maxPivot);

            if (Input.GetKey(KeyCode.Z))            //INPUT Slider Right
                magnitudeB = Mathf.Clamp(magnitudeB + .5f, 1f, maxPivot);
            if (Input.GetKey(KeyCode.X))
                magnitudeB = Mathf.Clamp(magnitudeB - .5f, 1f, maxPivot);

            if (Input.GetKey(KeyCode.Keypad8)) //INPUT Pad
                dirB = Direction.N;
            else if (Input.GetKey(KeyCode.Keypad4))
                dirB = Direction.W;
            else if (Input.GetKey(KeyCode.Keypad6))
                dirB = Direction.E;
            else if (Input.GetKey(KeyCode.Keypad2))
                dirB = Direction.S;
            else if (Input.GetKey(KeyCode.Keypad7))
                dirB = Direction.WN;
            else if (Input.GetKey(KeyCode.Keypad9))
                dirB = Direction.NE;
            else if (Input.GetKey(KeyCode.Keypad3))
                dirB = Direction.ES;
            else if (Input.GetKey(KeyCode.Keypad1))
                dirB = Direction.SW;

            if (Input.GetKeyDown(KeyCode.UpArrow))
                roadEditorController.addHeight();
            if (Input.GetKeyDown(KeyCode.DownArrow))
                roadEditorController.subHeight();

            updatePreview(road);

            if (Input.GetMouseButtonDown(0))         //Right Pad Click Confirme Length
            {
                roadEditorController.lastHeightConfirm();
                currentPhase = Phase.InitStreaching;
            }
            else if (Input.GetKeyDown(KeyCode.Return))        //Left Click Pad Confirme Path
            {
                currentPhase = Phase.ConfrimeAll;
            }
        }
        else if (currentPhase == Phase.ConfrimeAll)
        {
            road.GetComponent<Road>().rendererExtensions();
            magnitudeA = 10f;
            magnitudeB = 10f;
            anchorA = Vector3.zero;
            anchorB = Vector3.zero;
            pivotA = Vector3.zero;
            pivotB = Vector3.zero;
            currentPhase = Phase.Init;
            roadEditorController.showGrid = false;
            return true;

        }
        return false;
    }

    enum Phase
    {
        Init,InitStreaching, Streaching, ConfrimeAll
    }

}

