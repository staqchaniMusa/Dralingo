using Firebase.Extensions;
using Firebase.Storage;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FBStorageTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the storage service, using the default Firebase App
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;

        // Create a storage reference from our storage service
        StorageReference storageRef =
            storage.GetReferenceFromUrl("gs://app-and-web-firebase.appspot.com/Lessons/Lesson 0");
        // Create a reference with an initial file path and name
        StorageReference pathReference =
            storage.GetReference("images/stars.jpg");

        // Download via a Stream
        pathReference.GetStreamAsync(stream => {
            // Do something with the stream here.
            //
            // This code runs on a background thread which reduces the impact
            // to your framerate.
            //
            // If you want to do something on the main thread, you can do that in the
            // progress eventhandler (second argument) or ContinueWith to execute it
            // at task completion.
            
        }, null, CancellationToken.None);
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
