using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawByDirection : MonoBehaviour {

    RoadEditorController roadEditorController;
    Phase currentPhase = Phase.Init;
    Direction selectedDirection;
    Direction firstDirection;
    Vector3 lastCenter;
    List<Vector3> roadTemp;

    void updatePreview(GameObject road)
    {
        road.GetComponent<RoadCreator>().RenderRoad(road.GetComponent<Road>().processPoints());
    }

    void changeSegment(Road road,int distance)
    {
        road.setPoints(roadTemp);
        road.addPoint(road[road.Length - 1] + roadEditorController.directionToVector(selectedDirection).normalized * roadEditorController.offset);
        if (selectedDirection == Direction.N || selectedDirection == Direction.E || selectedDirection == Direction.W || selectedDirection == Direction.S) {
            for (int i = 0; i < distance / roadEditorController.offset; i++)
            {
                float f = Mathf.Lerp(roadEditorController.getLastHeight(), roadEditorController.getHeight(), (float)i / (distance / roadEditorController.offset));
                road.addPoint(road[road.Length - 1] + roadEditorController.directionToVector(selectedDirection).normalized * roadEditorController.offset);
                road[road.Length - 1] = new Vector3(road[road.Length - 1].x, f, road[road.Length - 1].z);
            }
        }
        else
        {
            for (int i = 0; i < distance / (roadEditorController.offset*Mathf.Sqrt(2f)); i++)
            {
                float f = Mathf.Lerp(roadEditorController.getLastHeight(), roadEditorController.getHeight(), (float)i / (distance / (roadEditorController.offset * Mathf.Sqrt(2f))));
                road.addPoint(road[road.Length - 1] + roadEditorController.directionToVector(selectedDirection));
                road[road.Length - 1] = new Vector3(road[road.Length - 1].x, f, road[road.Length - 1].z);
            }
        }
    }

    public bool usage(RaycastHit hit,Extension extension,GameObject road)
    {
        if(currentPhase == Phase.Init)
        {

            roadEditorController = RoadEditorController.Instance;
            selectedDirection = extension.ExitOrientation;
            firstDirection = roadEditorController.getOppositDirection(extension.ExitOrientation);
            lastCenter = extension.ExitPos;
            roadEditorController.setHeigth((int)lastCenter.y);
            if (!extension.isEmpty() && extension.ExitRoadRef.Nature == Nature.Normal)
            {
                if (road.GetComponent<Road>().Length != 0)
                    if (Vector3.Distance(lastCenter, road.GetComponent<Road>()[0]) < Vector3.Distance(lastCenter, road.GetComponent<Road>()[road.GetComponent<Road>().Length - 1]))
                        road.GetComponent<Road>().invertRoadOrder();
            }
            road.GetComponent<Road>().addPoint(lastCenter);
            roadTemp = new List<Vector3>(road.GetComponent<Road>().getPoints());

            currentPhase = Phase.Streaching;
        }
        else if(currentPhase == Phase.Streaching)
        {
            float distance = Vector3.Distance(roadEditorController.SnapPosition(new Vector3(hit.point.x, roadEditorController.getHeight(), hit.point.z), roadEditorController.offset), lastCenter);
            if (distance < roadEditorController.offset)
                distance = roadEditorController.offset;

            changeSegment(road.GetComponent<Road>(), (int)distance);

            updatePreview(road);
            bool roadGood = road.GetComponent<Road>().chekRoad();

            if (Input.GetMouseButtonDown(0) && roadGood)         //Right Pad Click Confirme Length
            {
                currentPhase = Phase.AxesRender;
                roadEditorController.lastHeightConfirm();
                roadTemp = new List<Vector3>(road.GetComponent<Road>().getPoints());
            }
            else if (Input.GetKeyDown(KeyCode.Return) && roadGood)        //Left Click Pad Confirme Path
            {
                currentPhase = Phase.ConfrimeAll;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
                roadEditorController.addHeight();
            if (Input.GetKeyDown(KeyCode.DownArrow))
                roadEditorController.subHeight();
        }
        else if(currentPhase == Phase.AxesRender)
        {
            roadEditorController.showHideGizmo();
            roadEditorController.updateGizmo(selectedDirection);
            roadEditorController.gizmoDirection.transform.position = road.GetComponent<Road>()[road.GetComponent<Road>().Length - 1];
            currentPhase = Phase.AxesSelection;
        }
        else if(currentPhase == Phase.AxesSelection)
        {
            if (Input.GetMouseButtonDown(0))//Left Pad Selection
            {
                Direction d = roadEditorController.pickDirection(hit);
                if (d != Direction.NULL)
                {
                    roadEditorController.showHideGizmo();
                    lastCenter = road.GetComponent<Road>()[road.GetComponent<Road>().Length - 1];
                    currentPhase = Phase.Streaching;
                    selectedDirection = d;
                }
            }

            if (Input.GetKeyDown(KeyCode.Return) && road.GetComponent<Road>().chekRoad())        //Left Click Pad Confirme Path
            {
                currentPhase = Phase.ConfrimeAll;
            }
        }
        else if(currentPhase == Phase.ConfrimeAll)
        {
            road.GetComponent<Road>().rendererExtensions();
            roadEditorController.gizmoDirection.SetActive(false);
            currentPhase = Phase.Init;
            road.GetComponent<Road>().makeRoadTintWhite();
            return true;
        }
        return false;
    }

    enum Phase
    {
        Init,Streaching,AxesRender, AxesSelection, ConfrimeAll
    }
}
