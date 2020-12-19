using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    public Player player;
    private bool promoted=false;
    public void Promotion()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        promoted=true;
    }

    public bool IsPromoted()
    {
        return promoted;
    }

}
