using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase;
using Firebase.Database;
using Firebase.Analytics;
using UnityEngine.UI;
using System.Linq;

public class FireBaseManager : MonoBehaviour
{
    public DatabaseReference databaseReference;
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser player;
    public PlayerMotor playerMotor;
    public string userIDtwo;
    public Text leaderboardText;
    public string userIDtest;
    public int destroyedBlocksTest;
    DataSnapshot childSnapShot;
   

    // Start is called before the first frame update

    void Awake()
    {
        try
        {
            StartCoroutine(Login());
        }
        catch (System.Exception ex)
        {
            // Tip: when logging errors, use LogException and pass the whole exception,
            // that way you will get pretty links to the error line in the whole stack trace.
            Debug.LogException(ex);
        }
    }

    public void InitFirebase()
    {
        Debug.Log("Setting up firebase auth");
        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }


    public IEnumerator Login()
    {
        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        //Call the Firebase auth signin function passing the email and password
        var loginTask = auth.SignInAnonymouslyAsync();

        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);


        player = loginTask.Result;
        Debug.LogFormat("User signed in successfully: {0} ({1})",
        player.DisplayName, player.UserId);

        StartCoroutine(LoadUserData());


        //yield return new WaitForSeconds(2);

    }

    public IEnumerator LoadUserData()
    {
        Debug.Log("Loading User Data");
        //Get the currently logged in user data
        var DBTask = databaseReference.Child("users").Child(player.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        DatabaseReference usersRef = databaseReference.Child("users");
        Debug.Log(player.UserId);

        User user = new User(player.UserId, 0, 0);
        usersRef.Child(player.UserId).SetRawJsonValueAsync(JsonUtility.ToJson(user));
    }

    public IEnumerator LoadScoreboardData()
    {
        //Get all the users data ordered by kills amount
        var DBTask = databaseReference.Child("users").OrderByChild("destroyedBlocks").GetValueAsync();

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
                userIDtest = userID;
                destroyedBlocksTest = destroyedBlocks;
                Debug.Log("User ID2: " + userID);
                Debug.Log("Destroyed bl2ocks: " + destroyedBlocks);
                leaderboardText.text = "User ID: " + userID + " Destroyed Blocks: " + destroyedBlocks;
                //Instantiate new scoreboard elements
            }
            
        }
    
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            StartCoroutine(Login());
        }

        if (playerMotor.blocksDestroyed != null && playerMotor.blocksDestroyed != 0 && playerMotor.blocksPlaced != 0)
        {
            DatabaseReference usersRef = databaseReference.Child("users");
            User user = new User(player.UserId, playerMotor.blocksDestroyed, playerMotor.blocksPlaced);
            usersRef.Child(player.UserId).SetRawJsonValueAsync(JsonUtility.ToJson(user));
            Debug.Log("hello");
        }

        try
        {
            //StartCoroutine(LoadScoreboardData());
        }
        catch (System.Exception ex)
        {
            // Tip: when logging errors, use LogException and pass the whole exception,
            // that way you will get pretty links to the error line in the whole stack trace.
            Debug.LogException(ex);
        }
    }
}
