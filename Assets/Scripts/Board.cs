using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject pawnPrefab;
    public GameObject blackPawnPrefab;
    public GameObject bluePlatform;
    private RaycastHit hit;
    private Pawn[,] fields=new Pawn[8,8];


    void Start()
    {
        GenerateBoard();
        fields[0,0].Promotion();
    }

    void Update()
    {
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnLeftClick();
            }
        }
    }
    public void OnLeftClick()
    {
        int x=(int)Math.Floor(hit.point.x+4), y=(int)Math.Floor(hit.point.z+4);
        if((x+y)%2==0)
        {
            bluePlatform.transform.position=new Vector3((float)Math.Floor(hit.point.x/0.5f*0.5f)+0.5f,0.11f,(float)Math.Floor(hit.point.z/0.5f*0.5f)+0.5f);
        }
        Debug.Log(x+" "+y + (IsPawnHere(x,y)? $"{fields[x,y].player.ToString()} pawn":" No pawn"));
    }
    public void GenerateBoard()
    {
        //white pawns
        for(int i=0;i<8;i++)
        {
            GeneratePawn(i,i%2,Player.White);
        }

        //black pawns
        for(int i=0;i<8;i++)
        {
            GeneratePawn(i,i%2+6,Player.Black);
        }
    }

    public void GeneratePawn(int x, int y, Player player)
    {
        GameObject temp;
        if(player==Player.White)temp=pawnPrefab;
        else temp=blackPawnPrefab;
        temp.transform.GetChild(0).gameObject.SetActive(false);
        Pawn pawnInstance=Instantiate(temp, new Vector3(x-3.5f,0.2f,y-3.5f), Quaternion.identity).GetComponent<Pawn>();
        pawnInstance.player=player;
        fields[x,y]=pawnInstance;
    }

    public bool IsPawnHere(int x, int y)
    {
        return fields[x,y]!=null;
    }


    public void MovePawn(int xFrom,int yFrom,int xTo, int yTo)
    {
        int xMove=xTo-xFrom;
        int yMove=yTo-yFrom;
        Pawn temp=fields[xFrom,yFrom];
        fields[xFrom,yFrom]=null;
        fields[xTo,yTo]=temp;
        Vector3 oldPosition=temp.gameObject.transform.position;
        Vector3 newPosition=new Vector3(oldPosition.x+=xMove,0.2f,oldPosition.z+=yMove);
        temp.transform.position=newPosition;
    }

    public void DestroyPawn(int x, int y)
    {
        Pawn destroyingPawn=fields[x,y];
        fields[x,y]=null;
        Destroy(destroyingPawn.gameObject);
    }
}
