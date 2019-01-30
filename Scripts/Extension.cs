using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extension : MonoBehaviour {

    Road exitRoadRef;
    Vector3 exitPos;
    int exitRefIndex;
    Direction exitOrientation;

    public void setExtension(Road exitRoadRef, Vector3 exitPos, int exitRefIndex, Direction exitOrientation)
    {
        this.exitRoadRef = exitRoadRef;
        this.exitPos = exitPos;
        this.exitRefIndex = exitRefIndex;
        this.exitOrientation = exitOrientation;
    }

    public void updateExt()
    {
        exitPos = transform.position;
        exitOrientation = RoadEditorController.Instance.vectorToDirection(exitPos - transform.parent.position);
    }

    public bool Equals(Extension r)
    {
        if (exitPos == r.ExitPos && exitOrientation == r.ExitOrientation && exitRefIndex == r.ExitRefIndex)
        {
            return true;
        }
        return false;
    }

    public bool isEmpty()
    {
        if (exitRoadRef == null)
            return true;
        return false;
    }

    public Road ExitRoadRef
    {
        get
        {
            return exitRoadRef;
        }

        set
        {
            exitRoadRef = value;
        }
    }

    public Vector3 ExitPos
    {
        get
        {
            return exitPos;
        }

        set
        {
            exitPos = value;
        }
    }

    public int ExitRefIndex
    {
        get
        {
            return exitRefIndex;
        }

        set
        {
            exitRefIndex = value;
        }
    }

    public Direction ExitOrientation
    {
        get
        {
            return exitOrientation;
        }

        set
        {
            exitOrientation = value;
        }
    }

    public void delete(out Extension extension)
    {
        extension = this;
        Destroy(gameObject);
    }

    //BUGS AND TO DO  :
    //  Cut
    //  Delete
}
