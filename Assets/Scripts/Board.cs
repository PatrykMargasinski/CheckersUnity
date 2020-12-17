using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject pawn;
    public GameObject blackPawn;
    public GameObject field;
    private RaycastHit hit;


    void Start()
    {
        for(int i=0;i<8;i++)
        {
            Instantiate(blackPawn, new Vector3(3.5f-i,0.2f,3.5f-i%2), Quaternion.identity); 
            Instantiate(pawn, new Vector3(3.5f-i,0.2f,-3.5f+(i+1)%2), Quaternion.identity); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            field.transform.position=new Vector3((float)Math.Floor(hit.point.x/0.5f*0.5f)+0.5f,0.11f,(float)Math.Floor(hit.point.z/0.5f*0.5f)+0.5f);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(hit.point.x+" "+hit.point.z);
        }
    }
}
