using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
    public static class UnityDebugHelpers
    {
        //Courtesy of https://medium.com/@davidzulic/unity-drawing-custom-debug-shapes-part-1-4941d3fda905
        public static void DrawCircle(
            Vector3 position,
            float radius,
            int segments,
            Color color)
        {
            // If either radius or number of segments are less or equal to 0, skip drawing
            if (radius <= 0.0f || segments <= 0)
            {
                return;
            }
 
            // Single segment of the circle covers (360 / number of segments) degrees
            float angleStep = (360.0f / segments);
 
            // Result is multiplied by Mathf.Deg2Rad constant which transforms degrees to radians
            // which are required by Unity's Mathf class trigonometry methods
 
            angleStep *= Mathf.Deg2Rad;
 
            for (int i = 0; i < segments; i++)
            {
                // Points are connected using DrawLine method and using the passed color
                UnityEngine.Debug.DrawLine(
                    position + new Vector3(
                        Mathf.Cos(angleStep * i),
                        0f,
                        Mathf.Sin(angleStep * i))
                        * radius,
                    position + new Vector3(
                        Mathf.Cos(angleStep * (i + 1)),
                        0f,
                        Mathf.Sin(angleStep * (i + 1))) * radius,
                    color);
            }
        }
    }
}