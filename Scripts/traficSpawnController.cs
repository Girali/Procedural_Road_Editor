using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class traficSpawnController : MonoBehaviour {

    public Road road;
    public GameObject f;

    IEnumerator spawnCycle  () {
        while (true)
        {
            Vector3Int[]vcts = road.getPath();
            GameObject follower = Instantiate(f, vcts[0] , Quaternion.identity);
            follower.GetComponent<TraficRoadFollower>().setPoints(vcts);
            yield return new WaitForSeconds(5f);
        }
	}
}
