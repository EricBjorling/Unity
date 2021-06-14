using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    public string userID;
    public int destroyedBlocks;
    public int placedBlocks;

    public User()
    {
    }

    public User(string userID, int destroyedBlocks, int placedBlocks)
    {
        this.userID = userID;
        this.destroyedBlocks = destroyedBlocks;
        this.placedBlocks = placedBlocks;
    }

}


