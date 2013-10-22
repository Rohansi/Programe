using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using SFML.Graphics;
using SFML.Window;
using FarseerCircleShape = FarseerPhysics.Collision.Shapes.CircleShape;
using Transform = FarseerPhysics.Common.Transform;

namespace PhysicsTest
{
    class SFMLDebugView : DebugViewBase
    {
        public Color DefaultShapeColor = new Color(229, 178, 178);
        public Color InactiveShapeColor = new Color(128, 128, 76);
        public Color KinematicShapeColor = new Color(128, 128, 229);
        public Color SleepingShapeColor = new Color(153, 153, 153);
        public Color StaticShapeColor = new Color(128, 229, 128);

        private RenderTarget rt;

        public SFMLDebugView(World world) : base(world)
        {
            
        }

        public void Draw(RenderTarget target)
        {
            rt = target;

            if ((Flags & DebugViewFlags.Shape) == DebugViewFlags.Shape)
            {
                foreach (Body b in World.BodyList)
                {
                    Transform xf;
                    b.GetTransform(out xf);
                    foreach (Fixture f in b.FixtureList)
                    {
                        if (b.Enabled == false)
                        {
                            DrawShape(f, xf, InactiveShapeColor);
                        }
                        else if (b.BodyType == BodyType.Static)
                        {
                            DrawShape(f, xf, StaticShapeColor);
                        }
                        else if (b.BodyType == BodyType.Kinematic)
                        {
                            DrawShape(f, xf, KinematicShapeColor);
                        }
                        else if (b.Awake == false)
                        {
                            DrawShape(f, xf, SleepingShapeColor);
                        }
                        else
                        {
                            DrawShape(f, xf, DefaultShapeColor);
                        }
                    }
                }
            }
        }

        private void DrawShape(Fixture fixture, Transform xf, Color color)
        {
            switch (fixture.Shape.ShapeType)
            {
                case ShapeType.Circle:
                    {
                        var circle = (FarseerCircleShape)fixture.Shape;

                        Vector2 center = MathUtils.Mul(ref xf, circle.Position);
                        float radius = circle.Radius;
                        Vector2 axis = xf.q.GetXAxis();

                        DrawSolidCircle(center, radius, axis, color.R, color.G, color.B);
                    }
                    break;

                case ShapeType.Polygon:
                    {
                        var poly = (PolygonShape)fixture.Shape;
                        int vertexCount = poly.Vertices.Count;
                        var vertices = new Vector2[vertexCount];

                        for (int i = 0; i < vertexCount; ++i)
                        {
                            vertices[i] = MathUtils.Mul(ref xf, poly.Vertices[i]);
                        }

                        DrawSolidPolygon(vertices, vertexCount, color.R, color.G, color.B);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override void DrawPolygon(Vector2[] vertices, int count, float red, float blue, float green, bool closed = true)
        {
            throw new NotImplementedException();
        }

        public override void DrawSolidPolygon(Vector2[] vertices, int count, float red, float blue, float green)
        {
            var col = new Color((byte)red, (byte)green, (byte)blue);
            var va = new VertexArray(PrimitiveType.Quads, (uint)count);
            for (uint i = 0; i < count; i++)
            {
                va[i] = new Vertex(new Vector2f(vertices[i].X * 64, vertices[i].Y * 64), col);
            }
            rt.Draw(va);
        }

        public override void DrawCircle(Vector2 center, float radius, float red, float blue, float green)
        {
            throw new NotImplementedException();
        }

        public override void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, float red, float blue, float green)
        {
            var col = new Color((byte)red, (byte)green, (byte)blue);
            var shape = new SFML.Graphics.CircleShape(radius * 64, 8);
            shape.Position = new Vector2f(center.X * 64, center.Y * 64);
            shape.Origin = new Vector2f(radius * 64, radius * 64);
            shape.FillColor = col;
            rt.Draw(shape);
        }

        public override void DrawSegment(Vector2 start, Vector2 end, float red, float blue, float green)
        {
            throw new NotImplementedException();
        }

        public override void DrawTransform(ref Transform transform)
        {
            throw new NotImplementedException();
        }
    }
}
