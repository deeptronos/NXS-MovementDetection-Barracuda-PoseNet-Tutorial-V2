using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//TODO: Using a Hashtable<> data structure, implement tracking the position of specific keypoints. Update the start method, and the UpdateKeyPointPosition 

public class PoseSkeleton
{
    public Transform[] keypoints; // The list of key point GameObjects that make up the pose skeleton
    
    private GameObject[] lines; // the GameObjects that contain data for the lines between key points
    private static string[] partNames = new string[]
    {
        "nose", "leftEye", "rightEye", "leftEar", "rightEar", "leftShoulder",
        "rightShoulder", "leftElbow", "rightElbow", "leftWrist", "rightWrist",
        "leftHip", "rightHip", "leftKnee", "rightKnee", "leftAnkle", "rightAnkle"
    };

    public static List<string> partsToLogData = new List<string>() // The keypoint/parts we want to log the position data of, by their partNames representation.
        { "nose", "leftShoulder", "rightShoulder", "leftWrist", "rightWrist" };
    
    private static int NUM_KEYPOINTS = partNames.Length;
    private Tuple<int, int>[] jointPairs = new Tuple<int, int>[]
    {
        Tuple.Create(0, 1), // Nose to Left Eye
        Tuple.Create(0, 2), // Nose to Right Eye
        Tuple.Create(1, 3), // Left Eye to Left Ear
        Tuple.Create(2, 4), // Right Eye to Right Ear
        Tuple.Create(5, 6), // Left Shoulder to Right Shoulder
        Tuple.Create(5, 11), // Left Shoulder to Left Hip
        Tuple.Create(6, 12), // Right Shoulder to Right Hip
        Tuple.Create(5, 12), // Left Shoulder to Right Hip
        Tuple.Create(6, 11), // Right Shoulder to Left Hip
        Tuple.Create(11, 12), // Left Hip to Right Hip
        Tuple.Create(5, 7), // Left Shoulder to Left Elbow
        Tuple.Create(7, 9), // Left Elbow to Left Wrist
        Tuple.Create(6, 8), // Right Shoulder to Right Elbow
        Tuple.Create(8, 10), // Right Elbow to Right Wrist
        Tuple.Create(11, 13), // Left Hip to Left Knee
        Tuple.Create(13, 15), // Left Knee to Left Ankle
        Tuple.Create(12, 14), // Right Hip to Right Knee
        Tuple.Create(14, 16) // Right Knee to Right Ankle
    };
        // Left wrist: 9
        // Right wrist: 10
        // Nose: 0
    
    // Colors for lines:
    private Color[] colors = new Color[]
    {
        Color.magenta, Color.magenta, Color.magenta, Color.magenta, // Head
        Color.red, Color.red, Color.red, Color.red, Color.red, Color.red, // Torso
        Color.green, Color.green, Color.green, Color.green, // Arms
        Color.blue, Color.blue, Color.blue, Color.blue // Legs
    };
    private float lineWidth; // The width for the skeleton lines
    private Material keypointMat;
    
     //private List<Vector2[]> TrackedKeypointData; // A list of arrays of vector 2's.     
    //private KeyValuePair<string, Vector2[]>[] TrackedKeypointData; // A DS containing the name of the keypoint as a string associated with an array of vector2s representing the keypoint's x/y position record


    
    public PoseSkeleton(float pointScale = 10f, float lineWidth = 5f) // Constructor
    {
        
       // string[] keypointPartsToTrack = new string[]{"nose", "leftWrist", "rightWrist"}; // The names of the keypoints we want to track // TODO remove this redundancy
       // TrackedKeypointData = new KeyValuePair<string, Vector2[]>[keypointPartsToTrack.Length];
       // int dataLength = 16; // The max amount of positions logged at any given time
        
        this.keypoints = new Transform[NUM_KEYPOINTS]; // Initialize keypoints
        // Style:
        Material keypointMat = new Material(Shader.Find("Unlit/Color")); 
        keypointMat.color = Color.yellow;

        for (int i = 0; i < NUM_KEYPOINTS; i++)
        {   // Initialize & adjust a primitive sphere gameobject:
            this.keypoints[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform; 
            this.keypoints[i].position = new Vector3(0, 0, 0);
            this.keypoints[i].localScale = new Vector3(pointScale, pointScale, 0);
            this.keypoints[i].gameObject.GetComponent<MeshRenderer>().material = keypointMat;
            this.keypoints[i].gameObject.name = partNames[i];
            
            if(partsToLogData.Contains(partNames[i])) // If this keypoint is a part we want to log data for... 
            {
                this.keypoints[i].gameObject.AddComponent<LogKeypointMovement>();
            }

        }

        // for (int i = 0; i < keypointPartsToTrack.Length; i++)
        // {
        //     TrackedKeypointData[i] = new KeyValuePair<string, Vector2[]>(keypointPartsToTrack[i], new Vector2[dataLength]);
        // }
        //
        this.lineWidth = lineWidth;
        
        int numPairs = jointPairs.Length + 1; // The number of joint pairs
        lines = new GameObject[numPairs]; // Initialize the lines array
        
        InitializeSkeleton(); // Initialize the pose skeleton!
    } 
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
    /// Create a line between the key point specified by the start and end point indices
    /// </summary>
    /// <param name="pairIndex"></param>
    /// <param name="startIndex"></param>
    /// <param name="endIndex"></param>
    /// <param name="width"></param>
    /// <param name="color"></param>
    private void InitializeLine(int pairIndex, float width, Color color)
    {
        int startIndex = jointPairs[pairIndex].Item1;
        int endIndex = jointPairs[pairIndex].Item2;
        
        // Create a new line GameObject
        string name = $"{keypoints[startIndex].name}_to_{keypoints[endIndex].name}";
        lines[pairIndex] = new GameObject(name);

        LineRenderer lineRenderer = lines[pairIndex].AddComponent<LineRenderer>(); // Add LineRenderer Component
        
        lineRenderer.material = new Material(Shader.Find("Unlit/Color")); // Make LineRenderer Shader Unlit
        lineRenderer.material.color = color; // Set the material color
        lineRenderer.positionCount = 2; // The line will consist of two points
        lineRenderer.startWidth = width; // Set the width from the start point
        lineRenderer.endWidth = width;

    }

    /// <summary>
    /// Initialize the pose skeleton
    /// </summary>
    private void InitializeSkeleton()
    {
        for (int i = 0; i < jointPairs.Length; i++)
        {
            InitializeLine(i, lineWidth, colors[i]);
        }
    }
    
    /// <summary>
    /// Toggles visibility for the skeleton
    /// </summary>
    /// <param name="show"></param>
    public void ToggleSkeleton(bool show)
    {
        for (int i = 0; i < jointPairs.Length; ++i)
        {
            lines[i].SetActive(show);
            keypoints[jointPairs[i].Item1].gameObject.SetActive(show);
            keypoints[jointPairs[i].Item2].gameObject.SetActive(show);
        }
    }
    
    /// <summary>
    /// Clean up skeleton GameObjects
    /// </summary>
    public void Cleanup()
    {
        for (int i = 0; i < jointPairs.Length; ++i)
        {
            GameObject.Destroy(lines[i]);
            GameObject.Destroy(keypoints[jointPairs[i].Item1].gameObject);
            GameObject.Destroy(keypoints[jointPairs[i].Item2].gameObject);
        }
    }

    /// <summary>
    /// Update the positions for the key point GameObjects
    /// </summary>
    /// <param name="keypoints"></param>
    /// <param name="sourceScale"></param>
    /// <param name="sourceTexture"></param>
    /// <param name="mirrorImage"></param>
    /// <param name="minConfidence"></param>
    public void UpdateKeyPointPositions(Utils.Keypoint[] keypoints, float sourceScale, RenderTexture sourceTexture, bool mirrorImage, float minConfidence)
    {
        string[] keypointPartsToTrack = new string[]{"nose", "leftWrist", "rightWrist"}; // The names of the keypoints we want to track
        
        for (int k = 0; k < keypoints.Length; k++) // Iterate through the key points
        {
            if (keypoints[k].score >= minConfidence / 100f) // Check if the current confidence value meets the confidence threshold
            {
                
                this.keypoints[k].GetComponent<MeshRenderer>().enabled = true; // Activate the current key point GameObject
            }
            else
            {
                
                this.keypoints[k].GetComponent<MeshRenderer>().enabled = false; // Deactivate the current key point GameObject
            }
            
            Vector2 coords = keypoints[k].position * sourceScale; // Scale the keypoint position to the original resolution
            
            coords.y = sourceTexture.height - coords.y;// Flip the keypoint position vertically
            
            if (mirrorImage) coords.x = sourceTexture.width - coords.x;// Mirror the x position if using a webcam
            
            this.keypoints[k].position = new Vector3(coords.x, coords.y, -1f);// Update the current key point location. Set the z value to -1f to place it in front of the video screen
            
         
        }
    }
    
    /// <summary>
    /// Draw the pose skeleton based on the latest location data
    /// </summary>
    public void UpdateLines()
    {
        // Iterate through the joint pairs
        for (int i = 0; i < jointPairs.Length; i++)
        {
            // Set the GameObject for the starting key point
            Transform startingKeyPoint = keypoints[jointPairs[i].Item1];
            // Set the GameObject for the ending key point
            Transform endingKeyPoint = keypoints[jointPairs[i].Item2];

            // Check if both the starting and ending key points are active
            if (startingKeyPoint.GetComponent<MeshRenderer>().enabled &&
                endingKeyPoint.GetComponent<MeshRenderer>().enabled)
            {
                // Activate the line
                lines[i].SetActive(true);

                LineRenderer lineRenderer = lines[i].GetComponent<LineRenderer>();
                // Update the starting position
                lineRenderer.SetPosition(0, startingKeyPoint.position);
                // Update the ending position
                lineRenderer.SetPosition(1, endingKeyPoint.position);
            }
            else
            {
                // Deactivate the line
                lines[i].SetActive(false);
            }
        }
    }
    
    

}
