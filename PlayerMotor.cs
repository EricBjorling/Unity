using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase;
using Firebase.Database;
using Firebase.Analytics;
using System.Linq;

public class PlayerMotor : MonoBehaviour
{

    [SerializeField] Rigidbody2D rb;
    private RaycastHit hit;
    GameObject targetObject;
    Tilemap groundTileMap;
    Tilemap caveTileMap;
    Tilemap oreTileMap;
    public float speed;
    public float jumpForce;
    private ProceduralGeneration proc;
    public StatsScreen statsScreen;
    public int blocksDestroyed = 0;
    public int blocksPlaced = 0;
    public Text destroyedText;
    public Text placedText;
    public Text leaderboardText;
    public DatabaseReference databaseReference;

    // Start is called before the first frame update
    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        targetObject = GameObject.Find("Procedural Generation");
        groundTileMap = targetObject.GetComponent<ProceduralGeneration>().groundTileMap;
        caveTileMap = targetObject.GetComponent<ProceduralGeneration>().caveTileMap;
        oreTileMap = targetObject.GetComponent<ProceduralGeneration>().oreTileMap;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            Vector3Int selectedTile = groundTileMap.WorldToCell(point);
            groundTileMap.SetTile(selectedTile, null);
            caveTileMap.SetTile(selectedTile, null);
            oreTileMap.SetTile(selectedTile, null);
            blocksDestroyed++;
            destroyedText.text = "Blocks Destroyed: " + blocksDestroyed;
        }
        if (Input.GetMouseButtonDown(1)) // Right click
        {
            Vector3Int selectedTile = groundTileMap.WorldToCell(point);
            if (caveTileMap.GetTile(selectedTile) != null)
            {
                caveTileMap.SetTile(selectedTile, null);
            }
            groundTileMap.SetTile(selectedTile, targetObject.GetComponent<ProceduralGeneration>().groundTile);
            blocksPlaced++;
            placedText.text = "Blocks Placed: " + blocksPlaced;

        }
        if (Input.GetMouseButtonDown(2)) // Middle click
        {
            Vector3Int selectedTile = groundTileMap.WorldToCell(point);
            caveTileMap.SetTile(selectedTile, targetObject.GetComponent<ProceduralGeneration>().caveTile);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            //rb.AddForce(new Vector2(-2,0));
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            //rb.AddForce(new Vector2(2, 0));
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            rb.AddForce(new Vector2(0, jumpForce*10));
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            StartCoroutine(LoadScoreboardData());
        }
    }

    public IEnumerator LoadScoreboardData()
    {
        //Get all the users data ordered by kills amount
        var DBTask = databaseReference.Child("users").OrderByChild("kills").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string userID = childSnapshot.Child("userID").Value.ToString();
                int destroyedBlocks = int.Parse(childSnapshot.Child("destroyedBlocks").Value.ToString());
                int placedBlocks = int.Parse(childSnapshot.Child("placedBlocks").Value.ToString());

                //Instantiate new scoreboard elements
                Debug.Log("Username: " + userID + " kills: " + destroyedBlocks);
                
                leaderboardText.text = "UserID: " + userID + " Destroyed blocks: " + destroyedBlocks + " Placed blocks: " + placedBlocks;
            }

            
            //Go to scoareboard screen
            //UIManager.instance.ScoreboardScreen();
        }
    }
}
