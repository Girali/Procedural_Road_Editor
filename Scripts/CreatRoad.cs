using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatRoad : MonoBehaviour {

    public Path path;
    public float spacing = 1;
    public float roadWidth = 1;
    public float roadHeight = 1;
    public bool autoUpdate;
    public float tiling = 1;
    public Mesh roadMesh;
    // Use this for initialization


    public Color anchorCol = Color.red;
    public Color controlCol = Color.white;
    public Color segmentCol = Color.green;
    public Color selectedSegmentCol = Color.yellow;
    public float anchorDiameter = .1f;
    public float controlDiameter = .075f;
    public bool displayControlPoints = true;

    public void CreatePath()
    {
        path = new Path(transform.position);
    }

    void Reset()
    {
        CreatePath();
    }
    void Start () {
        





        path = new Path(new Vector3(0, 0, 0));
        

	}
	
	// Update is called once per frame
	void Update () {
		//Road
	}

}
