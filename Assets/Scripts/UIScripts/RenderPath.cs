using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RenderPath : MonoBehaviour
{
    private LineRenderer currentPlotRenderer;
    private LineRenderer plannedPlotRenderer;

    public Color c1 = Color.red;
    public Color c2 = new Color(1, 1, 1, 0);

    public Color c3 = Color.blue;
    public Color c4 = new Color(1, 1, 1, 0);

    public static RenderPath instance { get;  set; }


    void Start() {

        instance = this;

        print("starting RenderPath");

        //lineRenderer = gameObject.AddComponent<LineRenderer>();
        currentPlotRenderer = gameObject.GetComponent<LineRenderer>();
        currentPlotRenderer.material = new Material(Shader.Find("Particles/Additive"));
        currentPlotRenderer.SetColors(c1, c2);

        plannedPlotRenderer = gameObject.GetComponent<LineRenderer>();
        plannedPlotRenderer.material = new Material(Shader.Find("Particles/Additive"));
        plannedPlotRenderer.SetColors(c3, c4);
    }

    public RenderPath ()
    {

    }

    public void renderPlotPath(Army army, List<Vector3> plotList, bool isCurrent) {

        // get the starting position - army or castle
        Vector3 startingPosition = army.getPosition();

        if (isCurrent) {
            drawLines(startingPosition, plotList, currentPlotRenderer);
        }
        else {
            drawLines(startingPosition, plotList, plannedPlotRenderer);
        }

    }

    private void drawLines(Vector3 startingPoint, List<Vector3> linePoints, LineRenderer lineRenderer) {

        if (linePoints.Count > 0) {
            lineRenderer.SetVertexCount(linePoints.Count + 1);

            lineRenderer.SetPosition(0, startingPoint);

            int position = 1; 
            foreach (Vector3 plot in linePoints) {
                lineRenderer.SetPosition(position, plot);
                position++;
            }
        }
        else {
            lineRenderer.SetVertexCount(0); // turn it off if nothing to display
        }
    }

    public void Clear() {

        currentPlotRenderer.SetVertexCount(0);
    }
}

