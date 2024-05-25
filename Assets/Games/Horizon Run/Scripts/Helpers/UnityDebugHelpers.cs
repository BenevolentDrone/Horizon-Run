using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
    public static class UnityDebugHelpers
    {
        //Courtesy of https://medium.com/@davidzulic/unity-drawing-custom-debug-shapes-part-1-4941d3fda905
        public static void DrawCircleXZ(
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

        public static void DrawCircle3DXY(
            Vector3 position,
            Quaternion rotation,
            float radius,
            int segments,
            Color color)
        {
            float angle = 0f;

            Vector3 lastPoint = Vector3.zero;
            Vector3 thisPoint = Vector3.zero;

            for (int i = 0; i < segments + 1; i++)
            {
                thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

                if (i > 0)
                {
                    Debug.DrawLine(
                        rotation * lastPoint + position,
                        rotation * thisPoint + position,
                        color);
                }

                lastPoint = thisPoint;

                angle += 360f / segments;
            }
        }

        public static void DrawCircle3DXZ(
            Vector3 position,
            Quaternion rotation,
            float radius,
            int segments,
            Color color)
        {
            float angle = 0f;

            Vector3 lastPoint = Vector3.zero;
            Vector3 thisPoint = Vector3.zero;

            for (int i = 0; i < segments + 1; i++)
            {
                thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                thisPoint.z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

                if (i > 0)
                {
                    Debug.DrawLine(
                        rotation * lastPoint + position,
                        rotation * thisPoint + position,
                        color);
                }

                lastPoint = thisPoint;

                angle += 360f / segments;
            }
        }

        public static void DrawCircle3DYZ(
            Vector3 position,
            Quaternion rotation,
            float radius,
            int segments,
            Color color)
        {
            float angle = 0f;

            Vector3 lastPoint = Vector3.zero;
            Vector3 thisPoint = Vector3.zero;

            for (int i = 0; i < segments + 1; i++)
            {
                thisPoint.y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                thisPoint.z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

                if (i > 0)
                {
                    Debug.DrawLine(
                        rotation * lastPoint + position,
                        rotation * thisPoint + position,
                        color);
                }

                lastPoint = thisPoint;

                angle += 360f / segments;
            }
        }
    }
}