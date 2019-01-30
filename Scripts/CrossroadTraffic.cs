using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossroadTraffic : MonoBehaviour {
    //Liste de liste de Point (route)

    int indexListToUse;

    //Speed in intersection
    float Speed = 0.0001f;

    //Sphere de check 
    public float sphereRadius;

    BoxCollider m_collider;

	// Use this for initialization
	void Start () {
        m_collider = GetComponent<BoxCollider>();
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        //Active le collider en présence d'objet dans une zone créé par une sphere.
        GameObject[] walkers = GameObject.FindGameObjectsWithTag("Walker");
        for (int i = 0; i < walkers.Length; i++)
        {
            if (Vector3.Distance(gameObject.transform.position, walkers[i].transform.position) < sphereRadius)
                m_collider.enabled = true;
            else
                m_collider.enabled = false;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        while(other.transform.position != transform.position)
            other.transform.position = Vector3.Lerp(other.transform.position, transform.position, Time.deltaTime * Speed);


        List<Extension> exts = GetComponent<Road>().getExtensionsPlaces();
        indexListToUse = Random.Range(0, exts.Count);
        other.GetComponent<TraficRoadFollower>().newListToFollow(exts[indexListToUse].ExitRoadRef.getPath());
    }


}
