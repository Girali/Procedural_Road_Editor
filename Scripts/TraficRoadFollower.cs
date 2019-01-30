using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraficRoadFollower : MonoBehaviour {
    //vitesse de déplacement
    public float Speed = 0.001f;

    //Est ce une route courbe ? 
    public bool onRoad = false;

    //Array de points Pour la route Courbe à suivre
    public Vector3Int[] RoadListPoint;
   
    //Distance entre PeoplePos et NextPointPos
    int DistanceNextPoint;

    //Vecteur direction + Position Int 
    Vector3 Direction;
    Vector3Int PeoplePosition = new Vector3Int();

    Vector3 V1, V2, VN, N1, N2, Normal;

    public float offsetRoad = 1f;

    //Init i 
    int i = 0;


    // Use this for initialization
    public void setPoints (Vector3Int[] vcts) {
        //Initialisation de points pour la démonstration
        RoadListPoint = vcts;
        //Initialisation Distance et Position en Int
        PeoplePosition = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        DistanceNextPoint = (int)Vector3Int.Distance(PeoplePosition, RoadListPoint[i]);
    }
	
	// Update
	void FixedUpdate () {

        //Si i est inférieur a la taille du array de points on fait déplacer
        if (onRoad)
        {
            //Definition des normal 
            if (i < RoadListPoint.Length - 1)
            {
                //Init Normals
                //Vecteur directeur et 1 
                V1 = RoadListPoint[i] - PeoplePosition;
                V2 = RoadListPoint[i + 1] - RoadListPoint[i];

                N1 = new Vector3(-V1.z, V1.y, V1.x);
                N2 = new Vector3(-V2.z, V2.y, V2.x);

                Normal = N1 + N2;
                Normal = Normal.normalized;
                Normal = Normal * offsetRoad;
            }

            //Déplacement
            if (i < RoadListPoint.Length)
            {
                //on déplace seulement si la distance est différente de 0
                if (DistanceNextPoint != 0)
                {
                    //on créé le vecteur directeur
                    //Direction = (RoadListPoint[i] + Normal) - PeoplePosition;
                    //On créé le déplacement dont la direction est le vecteur directeur
                    //transform.Translate(Direction * Speed * Time.deltaTime);

                    transform.position = Vector3.Lerp(transform.position, RoadListPoint[i] + Normal, Time.deltaTime * Speed);

                    //On recalcul la distance
                    DistanceNextPoint = (int)Vector3Int.Distance(PeoplePosition, RoadListPoint[i]);


                    if (DistanceNextPoint == 1)
                    {
                        //Si la distance est = 1 on incrémente i pour passer au point suivant
                        ++i;
                        //Si i est toujours dans le Array on recalcul la postion de People pour la prochaine boucle et la distance
                        if (i < RoadListPoint.Length)
                        {
                            PeoplePosition = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
                            DistanceNextPoint = (int)Vector3Int.Distance(PeoplePosition, RoadListPoint[i]);
                        }
                    }
                    else
                    {
                        //si la distance est différente de 1  on recalcul la postion en int de People et la distance.
                        PeoplePosition = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
                        DistanceNextPoint = (int)Vector3Int.Distance(PeoplePosition, RoadListPoint[i]);
                    }
                }
            }
            else
            {
                i = 0;
                onRoad = false;
            }
        }
	}


    public void newListToFollow(Vector3Int[] newList)
    {
        int DistancePointPremier = (int)Vector3Int.Distance(PeoplePosition, newList[0]);
        int DistancePointDernier = (int)Vector3Int.Distance(PeoplePosition, newList[newList.Length]);

        if(DistancePointDernier < DistancePointPremier){
            Vector3Int[] ListReverse = new Vector3Int[newList.Length];
            for (int p = 0; p < newList.Length; ++p)
            {
                ListReverse[p] = newList[newList.Length - p];
            }
            RoadListPoint = ListReverse;
            return;
        }else{
            RoadListPoint = newList;
            return;
        }
    }

}
