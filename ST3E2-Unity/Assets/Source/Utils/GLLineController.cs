using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

/// <summary>
/// Renders a series of GL Line shapes passed by user code.
/// </summary>
public class GLLineController : MonoBehaviour 
{
    private static Material lineMaterial;
    private List<LineShape> shapes;

    public GLLineController()
    {
        shapes = new List<LineShape>();
    }
    
    public void Start()
    {
        CreateLineMaterial();
    }

    public void AddShape(LineShape shape)
    {
        shapes.Add(shape);
    }

    public void RemoveShape(LineShape shape)
    {
        if (shapes.Contains(shape))
        {
            shapes.Remove(shape);
        }
    }

    public void ClearAllShapes()
    {
        shapes = new List<LineShape>();
    }

    private static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            var shader = Shader.Find ("Hidden/Internal-Colored");
			lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt ("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt ("_ZWrite", 0);
        }
    }

    // Will be called after all regular rendering is done
    public void OnRenderObject ()
    {
        // Apply the line material
        lineMaterial.SetPass (0);

        GL.PushMatrix ();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix (transform.localToWorldMatrix);

        for (int i = 0, count = shapes.Count; i < count; i++)
        {
            LineShape shape = shapes[i];
            int numVerts = shape.Verts.Count;
            if (numVerts > 1)
            {
                GL.Begin (GL.LINES);
                GL.Color (shape.Color);
                for (int j = 1, lineCt = shape.Verts.Count; j < lineCt; j++)
                {
                    Vector3 startPt = shape.Verts[j - 1];
                    Vector3 endPt = shape.Verts[j];
                    GL.Vertex3(startPt.x, startPt.y, startPt.z);
                    GL.Vertex3(endPt.x, endPt.y, endPt.z);
                }
                if (shape.IsClosed)
                {
                    Vector3 startPt = shape.Verts[0];
                    Vector3 endPt = shape.Verts[shape.Verts.Count - 1];
                    GL.Vertex3(startPt.x, startPt.y, startPt.z);
                    GL.Vertex3(endPt.x, endPt.y, endPt.z);
                }
                GL.End ();
            }
        }
        GL.PopMatrix ();
    }
}
