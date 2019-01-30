using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq﻿;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class RoadCreator : MonoBehaviour
{
    [Range(0.05f, 8f)]
    public float spacing = 1;
    public float roadWidth = 1;
    public float roadHeight = 1;
	public float tiling = 1;
    public Mesh roadMesh;

    public void RenderRoad(Path path)
    {
        transform.position = Vector3.zero;
        Vector3[] points = path.CalculateEvenlySpacedPoints(spacing);
        int textureRepeat = Mathf.RoundToInt(tiling * points.Length * spacing * .05f);
        GetComponent<MeshRenderer>().material.mainTextureScale = new Vector3(1, textureRepeat);
        Quaternion[] quats = getNormal(points);
        Vector3[] shapRoad = sortVects3(roadMesh.vertices);
        ExtrudeShape extrudeShape = new ExtrudeShape(shapRoad, roadHeight, roadWidth);
        GetComponent<MeshFilter>().mesh = Extrude(extrudeShape, createOrientedPoints(points, quats));
    }

    void ReverseNormals(GameObject road) {
        MeshFilter filter = road.GetComponent(typeof(MeshFilter)) as MeshFilter;
        if (filter != null)
        {
            Mesh mesh = filter.mesh;

            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
                normals[i] = -normals[i];
            mesh.normals = normals;

            for (int m = 0; m < mesh.subMeshCount; m++)
            {
                int[] triangles = mesh.GetTriangles(m);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int temp = triangles[i + 0];
                    triangles[i + 0] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                mesh.SetTriangles(triangles, m);
            }
        }
    }

    Quaternion[] getNormal(Vector3[] points)
    {

        Quaternion[] quats = new Quaternion[points.Length];

        for (int i = 0; i < points.Length-1; i++)
        {
            Vector3 forward = points[i + 1] - points[i];
            Vector3 left = new Vector3(forward.z, forward.y, -forward.x);
            quats[i] = Quaternion.LookRotation(forward,Vector3.Cross(forward, left));
        }

        quats[points.Length - 2] = quats[points.Length - 3];
        quats[points.Length - 1] = quats[points.Length - 3];
        return quats;
    }

    Vector3[] sortVects3(Vector3[]vects)
    {
        List<Vector3> sorted = new List<Vector3>();
        List<Vector3> toSorted = new List<Vector3>();
        foreach(Vector3 vect in vects)
        {
            toSorted.Add(vect);
        }

        for (int i = 0; i < vects.GetLength(0); i++) {

            float minY = float.MaxValue;
            float minX = float.MaxValue;
            Vector3 tempVect = Vector3.zero;

            foreach (Vector3 vect in toSorted)
            {

                if (vect.x < minX)
                {
                    minX = vect.x;
                    minY = vect.y;
                    tempVect = vect;
                }
                else if (vect.x == minX)
                {
                    if (vect.z < minY)
                    {
                        minY = vect.y;
                        tempVect = vect;
                    }
                }
            }
            sorted.Add(tempVect);
            toSorted.Remove(tempVect);
        }
        //Vector3 tmp = sorted[sorted.Count-2];
        //sorted[sorted.Count - 2] = sorted[sorted.Count - 1];
        //sorted[sorted.Count - 1] = tmp;
        return sorted.ToArray();
    }

    Mesh CreateRoadMesh(Vector3[] points, bool isClosed)
    {
        Vector3[] verts = new Vector3[points.Length * 2];
        Vector2[] uvs = new Vector2[verts.Length];
		int numTris = 2 * (points.Length - 1) + ((isClosed) ? 2 : 0);
		int[] tris = new int[numTris * 3];
        int vertIndex = 0;
        int triIndex = 0;
        for (int i = 0; i<points.Length; i++)
        {
            Vector3 forward = Vector3.zero;
			if (i < points.Length || isClosed)
            {
				forward += points[(i + 1) % points.Length] - points[i];
            }
            Vector3 left = new Vector3(-forward.y, forward.x);

            verts[vertIndex] = points[i] + left * roadWidth * .5f;
            verts[vertIndex + 1] = points[i] - left * roadWidth * .5f;

			float completionPercent = i / (float)(points.Length - 1);
			float v = 1 - Mathf.Abs (2 * completionPercent - 1);
			uvs [vertIndex] = new Vector2(0, v);
			uvs [vertIndex + 1] = new Vector2(1, v);

			if (i < points.Length || isClosed)
            {
                tris[triIndex] = vertIndex;
				tris[triIndex + 1] = (vertIndex + 2) % verts.Length;
                tris[triIndex + 2] = vertIndex + 1;

                tris[triIndex + 3] = vertIndex + 1;
				tris[triIndex + 4] = (vertIndex + 2) % verts.Length;
				tris[triIndex + 5] = (vertIndex + 3) % verts.Length;
            }

            vertIndex += 2;
            triIndex += 6;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
		mesh.uv = uvs;

        return mesh;
    }

    OrientedPoint[] createOrientedPoints(Vector3[] points,Quaternion[] quats)
    {
        OrientedPoint[] orientedPoint = new OrientedPoint[points.GetLength(0)];

        for (int i = 0; i < orientedPoint.GetLength(0); i++)
        {
            orientedPoint[i] = new OrientedPoint(points[i], quats[i]);
        }
        return orientedPoint;
    }

    Mesh Extrude(ExtrudeShape shape, OrientedPoint[] path)
    {
        Mesh mesh = new Mesh();
        int vertsInShape = shape.vertxs.GetLength(0);
        int segments = path.GetLength(0) - 1;
        int edgeLoops = path.GetLength(0);
        int vertCount = vertsInShape * edgeLoops;
        int triCount = shape.lines.GetLength(0) * segments;
        int triIndexCount = triCount * 3;

        int[] triangleIndices = new int[triIndexCount];
        Vector3[] vertices = new Vector3[vertCount];
        Vector3[] normals = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];

        // Generate all of the vertices and normals
        for (int i = 0; i < path.Length; i++)
        {
            int offset = i * vertsInShape;
            for (int j = 0; j < vertsInShape; j++)
            {
                int id = offset + j;
                vertices[id] = path[i].LocalToWorld(shape.vertxs[j]);
                normals[id] = path[i].LocalToWorldDirection(shape.normals[j]);
                uvs[id] = new Vector2(shape.uCoords[j], i / ((float) edgeLoops));
            }
        }

        // Generate all of the triangles
        int ti = 0;
        for (int i = 0; i < segments; i++)
        {
            int offset = i * vertsInShape;
            for (int l = 0; l < shape.lines.GetLength(0); l += 2)
            {
                int a = offset + shape.lines[l] + vertsInShape;
                int b = offset + shape.lines[l];
                int c = offset + shape.lines[l + 1];
                int d = offset + shape.lines[l + 1] + vertsInShape;
                triangleIndices[ti] = a; ti++;
                triangleIndices[ti] = b; ti++;
                triangleIndices[ti] = c; ti++;
                triangleIndices[ti] = c; ti++;
                triangleIndices[ti] = d; ti++;
                triangleIndices[ti] = a; ti++;
            }
        }
        
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangleIndices;

        return mesh;
    }


    class ExtrudeShape
    {
        public Vector2[] vertxs;
        public Vector2[] normals;
        public float[] uCoords;
        public int[] lines;
        float roadHeight;
        float roadWidth;


        Vector2[] translate2D(Vector3[] verts)
        {
            Vector2[] verts2D = new Vector2[verts.GetLength(0)];
            for (int i = 0; i < verts2D.GetLength(0); i++)
            {
                verts2D[i] = new Vector2( verts[i].x, -verts[i].z);
            }
            return verts2D;
        }



        Vector2 rotate180(Vector2 vct)
        {
            return new Vector2(vct.x * roadWidth, -vct.y * roadHeight);
        }

        Vector2 getNormals(Vector2 vct)
        {
            return new Vector2(vct.y, -vct.x);
        }

        Vector2[] lowPoly2DShape(Vector2[] vct)
        {
            Vector2[] lowPolyShape = new Vector2[vct.GetLength(0) * 2];
            lowPolyShape[0] = vct[0];
            for (int i = 1, j = 1; i < lowPolyShape.GetLength(0) - 1; i += 2, j++)
            {
                lowPolyShape[i] = vct[j];
                lowPolyShape[i + 1] = vct[j];
            }
            lowPolyShape[lowPolyShape.GetLength(0) - 1] = vct[0];
            return lowPolyShape;
        }

        public ExtrudeShape(Vector3[] vertcs,float width,float height)
        {
            roadHeight = width;
            roadWidth = height;         
            vertxs = lowPoly2DShape(translate2D(vertcs));
            for (int i = 0; i < vertxs.GetLength(0); i++)
            {
                vertxs[i] = rotate180(vertxs[i] * 100);
            }

            normals = new Vector2[vertxs.GetLength(0)];
            uCoords = new float[vertxs.GetLength(0)];
            lines = new int[vertxs.GetLength(0)];

            for (int j = 0; j < normals.GetLength(0); j += 2)
            {
                normals[j] = -getNormals(vertxs[j+1]- vertxs[j]);
                normals[j+1] = -getNormals(vertxs[j+1]- vertxs[j]);
            }

            uCoords[0] = 0;
            for (float i = 1; i < uCoords.Length-1; i+=2)
            {
                uCoords[(int)i] = (i / (uCoords.Length-1));
                uCoords[(int)i+1] = (i / (uCoords.Length - 1));

            }
            uCoords[uCoords.Length-1] = 1;

            lines[0] = lines.GetLength(0) - 1;
            for (int i = 1,j = lines.GetLength(0) - 2; j > 1;i++,j--)
            {
                lines[i] = j;
                i++;
                j--;
                lines[i] = j;
            }
            lines[lines.GetLength(0) - 1] = 0;

        }
    };

    struct OrientedPoint
    {
        public Vector3 position;
        public Quaternion rotation;

        public OrientedPoint(Vector3 position,Quaternion quat, float vCoordinate = 0)
        {
            this.position = position;
            Vector3 temp = position;
            temp.Normalize();
            this.rotation = quat;
        }

        public Vector3 LocalToWorld(Vector3 point)
        {
            return position + rotation * point;
        }

        public Vector3 WorldToLocal(Vector3 point)
        {
            return Quaternion.Inverse(rotation) * (point - position);
        }

        public Vector3 LocalToWorldDirection(Vector3 dir)
        {
            return rotation * dir;
        }
    }
}
