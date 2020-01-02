using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Boundary  // used to turn a size and offset into a range of x and y values  
{   //  *** It's easier to visualize objects by changing it's side lengths and position, so the boundary converts it into straight x and y values for ease of use ***
    public float xMin;
    public float xMax;  // variables for the x's and y's min and max values
    public float yMin;
    public float yMax;

    public Boundary(Vector2 size, Vector2 offset)   // takes in the box lengths and the box pos
    {
        xMin = offset.x - (size.x / 2); // finds the x value of the bottom left
        yMin = offset.y - (size.y / 2);
        xMax = offset.x + (size.x / 2);
        yMax = offset.y + (size.y / 2);
    }

    /*
        little Diagram of how it works
    
        
        
        |   --------------- BEFORE CONVERTING ---------------
        |
        |                       
        |            _  @ ---------------- @
        |           |    
        |           |   |                  |   
        |  SIZE.Y - |            @
        |           |   |      OFFSET      |
        |           |
        |            -  @ ---------------- @
        |                |________________|          
        |                      SIZE.X
        |
        |
        |   --------------- AFTER CONVERTING ---------------
        |
        |
        |               @ ---------------- @       <- MAX Y VALUE
        |               
        |               |                  |   
        |                
        |               |                  |
        |            
        |               @ ---------------- @      <- MIN Y VALUE
        |               ^                  ^          
        |           MIN X VALUE        MAX X VALUE
        |
         --------------------------------------------
    */
}