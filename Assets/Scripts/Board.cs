using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public GameObject pawnPrefab;
    public GameObject blackPawnPrefab;
    public GameObject bluePlatform;
    public Text gameText;
    private RaycastHit hit;
    private Pawn[,] fields=new Pawn[8,8];
    private bool isPawnClicked = false;
    private int previousX, previousY;
    private Player gracz;
    private bool canMoveUpwards = false, canMoveDownwards = false;
    private bool blockCancellingPawn = false;
    private int whiteCounter = 8, blackCounter = 8;
    private bool gameFinished = false;

    void Start()
    {
        GenerateBoard();
        SetText("Ruch białego");
    }

    void Update()
    {
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            if (Input.GetMouseButtonDown(0)&&!gameFinished)
            {
                if (isPawnClicked)
                {
                    MoveToClickedDestination();
                }
                else
                {
                    OnLeftClick();
                }
                if (CheckVictoryCondition())
                {
                    gameFinished = true;
                }
            }
            
        }

    }
    public bool CheckVictoryCondition()
    {
        bool canWhiteMove=false, canBlackMove=false;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (IsPawnHere(i, j))
                {
                    if (CheckIfYouCanMakeAnyStrike(i, j)||CheckIfYouCanMoveThere(i,j,i+1,j+1)|| CheckIfYouCanMoveThere(i, j, i - 1, j + 1) || CheckIfYouCanMoveThere(i, j, i + 1, j - 1) || CheckIfYouCanMoveThere(i, j, i - 1, j - 1))
                    {
                        if (fields[i, j].player.ToString() == "White")
                        {
                            canWhiteMove = true;
                        }
                        else
                        {
                            canBlackMove = true;
                        }
                    }
                }
            }
        }
        
        if (whiteCounter == 0||!canWhiteMove)
        {
            SetText("Czarne wygrały!");
            return true;
        }
        if (blackCounter == 0||!canBlackMove)
        {
            SetText("Białe wygrały!");
            return true;
        }
        return false;
    }
    public void OnLeftClick()
    {
        int x=(int)Math.Floor(hit.point.x+4), y=(int)Math.Floor(hit.point.z+4);
        if((x+y)%2==0)
        {
            
            bluePlatform.transform.position=new Vector3((float)Math.Floor(hit.point.x/0.5f*0.5f)+0.5f,0.11f,(float)Math.Floor(hit.point.z/0.5f*0.5f)+0.5f);
            bluePlatform.SetActive(true);
        }

        if (IsPawnHere(x, y) && IsPawnYours(x,y))
        {
            isPawnClicked = true;
            previousX = x;
            previousY = y;
        }

    }

    public void MoveToClickedDestination()
    {
        int x = (int)Math.Floor(hit.point.x + 4), y = (int)Math.Floor(hit.point.z + 4);

        if (previousX == x && previousY == y&&!blockCancellingPawn)
        {
            isPawnClicked = false;
            bluePlatform.SetActive(false);
            return;
        }

        if (!IsPawnHere(x, y))
        {
            if (CheckIfYouCanMoveThere(previousX, previousY, x, y)&&!CheckIfYouCanMakeAnyStrikeWithAnyPawn())
            {
                MovePawn(previousX, previousY, x, y);
                isPawnClicked = false;
                ChangePlayer();
                return;
            }

            if (CheckIfStrikeLocationCorrect(previousX, previousY, x, y) && CheckIfYouCanStrikeThere(previousX, previousY, x, y))
            {
                MovePawn(previousX, previousY, x, y);
                DestroyPawn((previousX + x) / 2, (previousY + y) / 2);
                ChangeBluePlatformLocation(x, y);
                previousX = x;
                previousY = y;
                if (!CheckIfYouCanMakeAnyStrike(previousX, previousY))
                {
                    blockCancellingPawn = false;
                    isPawnClicked = false;
                    ChangePlayer();
                }
                else
                {
                    blockCancellingPawn = true;
                }
            }
        }
    }

    public bool CheckIfYouCanMakeAnyStrikeWithAnyPawn()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (fields[i, j] != null)
                {
                    if ((fields[i, j].player.ToString() == "White" && gracz == Player.White) || (fields[i, j].player.ToString() == "Black" && gracz == Player.Black))
                    {
                        if (CheckIfYouCanMakeAnyStrike(i, j))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    public bool CheckIfYouCanMakeAnyStrike(int previousX, int previousY)
    {

        if( CheckIfYouCanStrikeThere(previousX,previousY,previousX+2,previousY+2) && CheckIfStrikeLocationCorrect(previousX, previousY, previousX + 2, previousY + 2))
        {
            return true;
        }
        if (CheckIfYouCanStrikeThere(previousX, previousY, previousX - 2, previousY + 2) && CheckIfStrikeLocationCorrect(previousX, previousY, previousX - 2, previousY + 2))
        {
            return true;
        }
        if (CheckIfYouCanStrikeThere(previousX, previousY, previousX + 2, previousY - 2) && CheckIfStrikeLocationCorrect(previousX, previousY, previousX + 2, previousY - 2))
        {
            return true;
        }
        if (CheckIfYouCanStrikeThere(previousX, previousY, previousX - 2, previousY - 2) && CheckIfStrikeLocationCorrect(previousX, previousY, previousX - 2, previousY - 2))
        {
            return true;
        }
        return false;
    }
    public bool CheckIfStrikeLocationCorrect(int previousX, int previousY, int x, int y)
    {
        if ((x + y) % 2 == 0 && x == previousX + 2 || x == previousX - 2)
        {
            if (y == previousY + 2)
            {
                return true;
            }
            if (y == previousY - 2)
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckIfYouCanStrikeThere(int previousX, int previousY, int x, int y)
    {
        if (IsPawnHere((previousX + x) / 2, (previousY + y) / 2) && !IsPawnYours((previousX + x) / 2, (previousY + y) / 2) && !IsPawnHere(x, y))
        {
            if (x >= 0 && x < 8 && y >= 0 && y < 8)
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckIfYouCanMoveThere(int previousX, int previousY, int x, int y)
    {
        CheckMovementPossibilities(previousX,previousY);
        if ((x + y) % 2 == 0 && (x == previousX+1 || x == previousX-1) && x >= 0 && x < 8 && y >= 0 && y < 8&&!IsPawnHere(x,y))
        {
            if (y == previousY+1 && canMoveUpwards)
            {
                return true;
            }
            if (y == previousY-1 && canMoveDownwards)
            {
                return true;
            }
        }
        return false;
    }
    public void CheckMovementPossibilities(int x, int y)
    {
        if(fields[x,y]==null)
        {
            canMoveDownwards = false;
            canMoveUpwards = false;
            return;
        }
        if (fields[x, y].IsPromoted())
        {
            canMoveDownwards = true;
            canMoveUpwards = true;
        }
        else
        {
            if (fields[x,y].player.ToString()=="White")
            {
                canMoveUpwards = true;
                canMoveDownwards = false;
            }
            else
            {
                canMoveDownwards = true;
                canMoveUpwards = false;
            }
        }
    }
    public void ChangePlayer()
    {
        if (gracz == Player.White)
        {
            SetText("Ruch czarnego");
            gracz = Player.Black;
        }
        else
        {
            SetText("Ruch białego");
            gracz = Player.White;
        }
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
        if (x < 0 || x > 7 || y < 0 || y > 7)
        {
            return false;
        }
        return fields[x, y] != null;
    }
    public bool IsPawnYours(int x, int y)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7)
        {
            return false;
        }
        if (gracz == Player.White && fields[x, y].player.ToString() == "White")
        {
            return true;
        }
        else
        {
            if (gracz == Player.Black && fields[x, y].player.ToString() == "Black")
            {
                return true;
            }
            return false;
        }
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
        PromotePawnIfPossible(xTo, yTo);
    }
    public void PromotePawnIfPossible(int x, int y)
    {
        if (gracz == Player.White)
        {
            if (y == 7)
            {
                fields[x, y].Promotion();
            }
        }
        else
        {
            if (y == 0)
            {
                fields[x, y].Promotion();
            }
        }
    }
    public void DestroyPawn(int x, int y)
    {
        Pawn destroyingPawn=fields[x,y];
        fields[x,y]=null;
        Destroy(destroyingPawn.gameObject);
        DecrementCounter();
    }
    public void DecrementCounter()
    {
        if (gracz == Player.White)
        {
            blackCounter--;
        }
        else
        {
            whiteCounter--;
        }
    }

    public void SetText(string text)
    {
        gameText.text=text;
    }
    public string GetText()
    {
        return gameText.text;
    }
    public void ChangeBluePlatformLocation(int x, int y)
    {
        bluePlatform.transform.position = new Vector3(x - 3.5f, 0.11f, y - 3.5f);
    }
}
