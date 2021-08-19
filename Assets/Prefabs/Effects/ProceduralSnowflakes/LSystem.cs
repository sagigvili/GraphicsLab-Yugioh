using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LSystemLightning : MonoBehaviour
{
    private Snow snow;
    private GameObject system;
    private const int TEXTURE_DIMENSIONS = 128;

    private Texture2D texture;

    private struct State
    {
        public State(Vector3 point, Vector3 dir)
        {
            currentPoint = point;
            direction = dir;
        }

        public Vector3 currentPoint;
        public Vector3 direction;
    }

    private struct Line
    {
        public Line(Vector2 s, Vector2 e)
        {
            start = s;
            end = e;
        }

        public Vector2 start;
        public Vector2 end;
    }

    private string axiom = "C[--R]++C[--R]++C[--R]++C[--R]++C[--R]++C[--R]";
    private Dictionary<char, List<string>> rules = new Dictionary<char, List<string>>();
    public float angle = 30f;

    private string current;
    private float length = 1f;
    private Stack<State> stack = new Stack<State>();
    private float snowflakeExtent;
    private uint snowflakeCounter = 0;

    public void Init_system(Snow s, GameObject wm)
    {
        rules.Add('F', new List<string>());
        rules['F'].Add("C[-F][F][+F]");
        //rules['F'].Add("C[-C][F][+C]");
        rules['F'].Add("C[-C++C][+C--C+F]");
        //rules['F'].Add("CF");
        //rules['F'].Add("C");
        rules['F'].Add("C[++C--C--C][--C++C++C--F]");
        rules.Add('R', new List<string>());
        rules['R'].Add("[-F][F][+F]");
        //rules['R'].Add("[-C][F][+C]");
        rules['R'].Add("[-C++C][+C--C+F]");
        rules['R'].Add("F");
        rules['R'].Add("[++C--C--C][--C++C++C--F]");
        current = axiom;
        stack.Push(new State(Vector3.zero, new Vector3(0, length, 0)));

        texture = new Texture2D(TEXTURE_DIMENSIONS, TEXTURE_DIMENSIONS , TextureFormat.Alpha8, true);
        clearTexture();
        snow = s;
        system = wm;
        current = axiom;
        clearTexture();
        iterate();
        iterate();
        endIteration();
        draw();
        system.SetActive(true);
    }

    public void create_flake()
    {
        clearTexture();
        current = axiom;
        clearTexture();
        iterate();
        iterate();
        endIteration();
        draw();
    }

    public void abort()
    {
        system.SetActive(false);
    }

    void iterate()
    {
        foreach (KeyValuePair<char, List<string>> r in rules)
        {
            string altToUse = r.Value[Random.Range(0, r.Value.Count)];

            current = current.Replace(r.Key.ToString(), altToUse);
        }
    }

    void endIteration()
    {
        current = current.Replace("F", "C[-C++C][+C--C]");
    }

    void draw()
    {
        State initialState = stack.Pop();
        initialState.currentPoint = Vector3.zero;
        initialState.direction = new Vector3(0, length, 0);
        stack.Push(initialState);

        List<Line> lines = new List<Line>();

        for (int i = 0; i < current.Length; i++)
        {
            switch (current[i])
            {
                case 'F':
                case 'C':
                    {
                        State currentState = stack.Pop();
                        // add line to list so we can find extents of resulting texture
                        lines.Add(new Line(currentState.currentPoint, currentState.currentPoint + currentState.direction));
                        currentState.currentPoint += currentState.direction;
                        stack.Push(currentState);
                    }
                    break;
                case '-':
                    {
                        State currentState = stack.Pop();
                        currentState.direction = Quaternion.Euler(0, 0, -angle) * currentState.direction;
                        stack.Push(currentState);
                    }
                    break;
                case '+':
                    {
                        State currentState = stack.Pop();
                        currentState.direction = Quaternion.Euler(0, 0, angle) * currentState.direction;
                        stack.Push(currentState);
                    }
                    break;
                case '[':
                    {
                        State newState = new State(stack.Peek().currentPoint, stack.Peek().direction);
                        stack.Push(newState);
                    }
                    break;
                case ']':
                    stack.Pop();
                    break;
                default:
                    break;
            }
        }

        float min = 0f;
        float max = 0f;
        // find extents of texture
        foreach (Line l in lines)
        {
            if (l.end.x > max)
            {
                max = l.end.x;
            }
            if (l.end.y > max)
            {
                max = l.end.y;
            }

            if (l.end.x < min)
            {
                min = l.end.x;
            }
            if (l.end.y < min)
            {
                min = l.end.y;
            }
        }

        snowflakeExtent = (max - min) / 2f;
        // actually draw
        foreach (Line l in lines)
        {

            drawLineInTexture(worldSpaceToTextureSpace(l.start),
                worldSpaceToTextureSpace(l.end),
                Color.white);
        }
        //byte[] bytes = texture.EncodeToPNG();
        snow.ChangeAlbedo(texture);
        //string filepath = Application.dataPath + "/output/snowflake" + snowflakeCounter.ToString() + ".png";
        //File.WriteAllBytes(filepath, bytes);
        //print("output to " + filepath);
        snowflakeCounter++;
    }

    Vector2 worldSpaceToTextureSpace(Vector2 point)
    {
        point -= new Vector2(-Mathf.Cos(Mathf.Deg2Rad * 30.0f) * length, 0.5f * length);
        point = TEXTURE_DIMENSIONS * (point + new Vector2(snowflakeExtent, snowflakeExtent)) / (2 * snowflakeExtent);
        return point;
    }

    void drawLineInTexture(Vector2 start, Vector2 end, Color c)
    {
        Vector2 temp = start;
        float frac = 1 / Mathf.Sqrt(Mathf.Pow(end.x - start.x, 2) + Mathf.Pow(end.y - start.y, 2));
        float ctr = 0;

        while ((int)temp.x != (int)end.x || (int)temp.y != (int)end.y)
        {
            temp = Vector2.Lerp(start, end, ctr);
            ctr += frac;
            texture.SetPixel((int)temp.x, (int)temp.y, c);
        }

        texture.Apply();
    }

    void clearTexture()
    {
        var colourArray = texture.GetPixels();

        for (int i = 0; i < colourArray.Length; i++)
        {
            colourArray[i] = Color.clear;
        }

        texture.SetPixels(colourArray);
        texture.Apply();
    }
}
