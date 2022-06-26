using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class LogKeypointMovement : MonoBehaviour
{
    
    public List<Vector2> data;
    
    // Start is called before the first frame update
    void Start()
    {
        data = new List<Vector2>(); 
    }

    // Update is called once per frame
    void Update()
    {
        data.Add(new Vector2(this.transform.position.x, this.transform.position.y));
    }
}
