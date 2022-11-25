using System.Collections.Generic;
using UnityEngine;

public class LineShape
{
    // The vertices in the shape.
    public List<Vector3> Verts { get; set; }

    // The color of the lines that make up this shape.
    public Color Color { get; set; }

    // Indicates whether or not this is a closed shape to the GLLineController.
    // If true, GLLineController will render a line from the last to the first node in Verts.
    public bool IsClosed { get; set; }

    public LineShape(List<Vector3> verts, Color color, bool isClosed)
    {
        Verts = verts;
        Color = color;
        IsClosed = isClosed;
    }

    public LineShape()
    {
        Verts = new List<Vector3>();
        Color = Color.white;
    }
}

