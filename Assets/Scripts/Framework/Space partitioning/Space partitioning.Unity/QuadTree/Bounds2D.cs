using System;

using UnityEngine;

namespace HereticalSolutions.SpacePartitioning
{
    public class Bounds2D
    {
        public float Left;

        public float Top;

        public float Width;

        public float Height;

        public float Right;

        public float Bottom;

        public Vector2 Min;

        public Vector2 Center;

        public Vector2 Max;

        public Vector2 Size;

        public Bounds2D(float left, float top, float width, float height)
        {
            Left = left;

            Top = top;

            Width = width;

            Height = height;

            Right = Left + Width;
            
            Bottom = Top + Height;
            
            Min = new Vector2(Left, Top);
            
            Center = new Vector2(Left + Width / 2f, Top + Height / 2f);
            
            Max = new Vector2(Right, Bottom);
            
            Size = new Vector2(Width, Height);
        }

        public Bounds2D(float x, float y, float radius)
        {
            Left = x - radius;

            Top = y - radius;

            Width = radius * 2f;

            Height = radius * 2f;
            
            Right = Left + Width;
            
            Bottom = Top + Height;
            
            Min = new Vector2(Left, Top);
            
            Center = new Vector2(Left + Width / 2f, Top + Height / 2f);
            
            Max = new Vector2(Right, Bottom);
            
            Size = new Vector2(Width, Height);
        }

        public Bounds2D(Vector2 position, float radius)
        {
            Left = position.x - radius;

            Top = position.y - radius;

            Width = radius * 2f;

            Height = radius * 2f;
            
            Right = Left + Width;
            
            Bottom = Top + Height;
            
            Min = new Vector2(Left, Top);
            
            Center = new Vector2(Left + Width / 2f, Top + Height / 2f);
            
            Max = new Vector2(Right, Bottom);
            
            Size = new Vector2(Width, Height);
        }

        public Bounds2D(Vector2 position, Vector2 size)
        {
            Left = position.x - size.x / 2f;

            Top = position.y - size.y / 2f;

            Width = size.x;

            Height = size.y;
            
            Right = Left + Width;
            
            Bottom = Top + Height;
            
            Min = new Vector2(Left, Top);
            
            Center = new Vector2(Left + Width / 2f, Top + Height / 2f);
            
            Max = new Vector2(Right, Bottom);
            
            Size = new Vector2(Width, Height);
        }
        
        public void Update(float left, float top, float width, float height)
        {
            Left = left;

            Top = top;

            Width = width;

            Height = height;

            Right = Left + Width;
            
            Bottom = Top + Height;
            
            Min = new Vector2(Left, Top);
            
            Center = new Vector2(Left + Width / 2f, Top + Height / 2f);
            
            Max = new Vector2(Right, Bottom);
            
            Size = new Vector2(Width, Height);
        }

        public void Update(float x, float y, float radius)
        {
            Left = x - radius;

            Top = y - radius;

            Width = radius * 2f;

            Height = radius * 2f;
            
            Right = Left + Width;
            
            Bottom = Top + Height;
            
            Min = new Vector2(Left, Top);
            
            Center = new Vector2(Left + Width / 2f, Top + Height / 2f);
            
            Max = new Vector2(Right, Bottom);
            
            Size = new Vector2(Width, Height);
        }

        public void Update(Vector2 position, float radius)
        {
            Left = position.x - radius;

            Top = position.y - radius;

            Width = radius * 2f;

            Height = radius * 2f;
            
            Right = Left + Width;
            
            Bottom = Top + Height;
            
            Min = new Vector2(Left, Top);
            
            Center = new Vector2(Left + Width / 2f, Top + Height / 2f);
            
            Max = new Vector2(Right, Bottom);
            
            Size = new Vector2(Width, Height);
        }

        public void Update(Vector2 position, Vector2 size)
        {
            Left = position.x - size.x / 2f;

            Top = position.y - size.y / 2f;

            Width = size.x;

            Height = size.y;
            
            Right = Left + Width;
            
            Bottom = Top + Height;
            
            Min = new Vector2(Left, Top);
            
            Center = new Vector2(Left + Width / 2f, Top + Height / 2f);
            
            Max = new Vector2(Right, Bottom);
            
            Size = new Vector2(Width, Height);
        }

        public void UpdateCenter(float x, float y)
        {
            Left = x - Width / 2f;

            Top = y - Height / 2f;
            
            Right = Left + Width;
            
            Bottom = Top + Height;
            
            Min = new Vector2(Left, Top);
            
            Center = new Vector2(Left + Width / 2f, Top + Height / 2f);
            
            Max = new Vector2(Right, Bottom);
            
            Size = new Vector2(Width, Height);
        }

        public bool Contains(Bounds2D other)
        {
            return Left <= other.Left
                && other.Right <= Right
                && Top <= other.Top
                && other.Bottom <= Bottom;
        }

        public bool Intersects(Bounds2D box)
        {
            return !(
                Left >= box.Right
                || Right <= box.Left
                || Top >= box.Bottom
                || Bottom <= box.Top);
        }

        public Bounds2D Divide(Quadrants quadrant)
        {
            float halfWidth = Width / 2f;

            float halfHeight = Height / 2f;

            switch (quadrant)
            {
                case Quadrants.NORTHWEST:
                    return new Bounds2D(Left, Top, halfWidth, halfHeight);

                case Quadrants.NORTHEAST:
                    return new Bounds2D(Left + halfWidth, Top, halfWidth, halfHeight);

                case Quadrants.SOUTHWEST:
                    return new Bounds2D(Left, Top + halfHeight, halfWidth, halfHeight);

                case Quadrants.SOUTHEAST:
                    return new Bounds2D(Left + halfWidth, Top + halfHeight, halfWidth, halfHeight);
            }

            throw new Exception("[Bounds2D] INVALID QUADRANT");
        }

        public Quadrants GetQuadrant(Bounds2D valueBox)
        {
            var center = Center;

            // West
            if (valueBox.Right < center.x)
            {
                // North West
                if (valueBox.Bottom < center.y)
                    return Quadrants.NORTHWEST;

                // South West
                else if (valueBox.Top >= center.y)
                    return Quadrants.SOUTHWEST;

                // Not contained in any quadrant
                else
                    return Quadrants.UNIDENTIFIED;
            }
            // East
            else if (valueBox.Left >= center.x)
            {
                // North East
                if (valueBox.Bottom < center.y)
                    return Quadrants.NORTHEAST;

                // South East
                else if (valueBox.Top >= center.y)
                    return Quadrants.SOUTHEAST;

                // Not contained in any quadrant
                else
                    return Quadrants.UNIDENTIFIED;
            }
            // Not contained in any quadrant
            else
                return Quadrants.UNIDENTIFIED;
        }

        public override string ToString()
        {
            return string.Format("[ [{0}:{1}] -> [{2}:{3}] ]", Left, Top, Right, Bottom);
        }  
    }
}