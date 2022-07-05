using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

// TODO: This implementation and interface is ENTIRELY non-final.
// TODO add functionality to disable line updating when appropriate & do data management better (an infinitely-growing List<> is not a good solution)
public class LogKeypointMovement : MonoBehaviour
{
    
    public List<Vector2> data;

    public int dataLength = 8;

    public LineRenderer lineRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        data = new List<Vector2>();
        
        lineRenderer = this.gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Unlit/Color")); // Make LineRenderer Shader Unlit
        lineRenderer.material.color = Color.cyan; // Set the material color
        lineRenderer.positionCount = 8; // The line will consist of two points
        lineRenderer.startWidth = 10f; // Set the width from the start point
        lineRenderer.endWidth = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateData();
        UpdateLine();
    }

    void UpdateData() // Add new entry to data & ensure it stays at the proper size
    {
        if (data.Count + 1 > dataLength)
        {
            data.RemoveAt(0); // Remove the 0-th entry in data
        }
        data.Add(new Vector2(this.transform.position.x, this.transform.position.y));
        
    }
    void UpdateLine() // Update the data visualization LineRenderer
    {
        lineRenderer.positionCount = data.Count; // update position count
        for (int i = 0; i < data.Count; i++)
        { 
            lineRenderer.SetPosition(i, new Vector3(data[i].x, data[i].y, 0));
        }
    }
}
