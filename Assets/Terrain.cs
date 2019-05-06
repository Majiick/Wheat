using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Terrain : MonoBehaviour {
    #region SINGLETON PATTERN
    public static Terrain _instance;
    public static Terrain Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<Terrain>();
            }

            return _instance;
        }
    }
    #endregion

    public enum TerrainType { Land, Water, Mountain };
    private float[,] _terrainHeights;
    private TerrainType[,] _terrainTypes;

    int width = 256;
    int length = 256;
    private int _height = 5;
    private float _scale = 5f;
    private float _offsetx = 0f;
    private float _offsety = 0f;




    void Start() {
        Regenerate();
    }

    public void Regenerate() {
        _offsetx = Random.Range(0, 1000000f);
        _offsety = Random.Range(0, 1000000f);

        UnityEngine.Terrain t = GetComponent<UnityEngine.Terrain>();
        t.terrainData.size = new Vector3(width, _height, length);
        t.terrainData.heightmapResolution = width;

        const int octaveAmount = 6;
        const float octaveScaleMultiplier = 4f;
        const float amplitudeMultiplier = 0.3f;


        List<float[,]> octaves = new List<float[,]>();
        float amplitude = 1f;
        float scale = 1f;
        for (int i = 0; i < octaveAmount; i++) {
            octaves.Add(new float[width,length]);

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < length; y++) {
                    octaves[i][x, y] = Mathf.PerlinNoise(((float)x + _offsetx) / width * _scale * scale, ((float)y + _offsety) / length * _scale * scale) * amplitude;
                }
            }

            amplitude = amplitude * amplitudeMultiplier;
            scale = scale * octaveScaleMultiplier;
        }

        // Generate the heights
        _terrainHeights =  new float[width,length];
        foreach (var octave in octaves) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < length; y++) {
                    _terrainHeights[x, y] += octave[x, y];
                }
            }
        }

        _terrainTypes = new TerrainType[width,length];
        // Clamp the heights
        Texture2D texture = new Texture2D(width, length);
        bool[,] land = new bool[width, length];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < length; y++) {
                if (_terrainHeights[x, y] < 0.35f) {  // Water
                    texture.SetPixel(y, x, Color.blue);
                    texture.SetPixel(y, x, new Color(0.627f, 0.321f, 0.17f));
                    //                    _terrainHeights[x, y] = 0.3f;
                    _terrainTypes[x, y] = TerrainType.Water;
                }
                else if (_terrainHeights[x, y] >= 0.8) {  // Mountain
                    texture.SetPixel(y, x, Color.gray);
                    texture.SetPixel(y, x, new Color(0.627f, 0.321f, 0.17f));
                    _terrainTypes[x, y] = TerrainType.Mountain;
                }
                else {  // Land
                    if (x % 2 == 0) {
                        texture.SetPixel(y, x, Color.green);
                        texture.SetPixel(y, x, new Color(0.627f, 0.321f, 0.17f));
                    }
                    else {
                        texture.SetPixel(y, x, Color.green + new Color(0, -0.05f, 0));
                        texture.SetPixel(y, x, new Color(0.627f, 0.321f, 0.17f));
                    }

                    land[x, y] = true;
                    _terrainTypes[x, y] = TerrainType.Land;
                }
            }
        }

        // Smooth out the land a bit.
        for (int i = 0; i < 1; i++) {
            _terrainHeights = SmoothOutTransitions(_terrainHeights, land);
        }

        //t.terrainData.terrainLayers = { }
        TerrainLayer tl = new TerrainLayer();
        tl.diffuseTexture = texture;
        tl.tileSize = new Vector2(width, length);
        tl.tileOffset = new Vector2(0, 0);
        tl.diffuseTexture.Apply(true);
        t.terrainData.terrainLayers = new TerrainLayer[] { tl };

        t.terrainData.SetHeights(0, 0, _terrainHeights);
    }
    

    float[,] SmoothOutTransitions(float[,] heights, bool[,] mask=null) {
        float[,] newHeights = (float[,])heights.Clone();
        int kernelSize = 3;
        Debug.Assert(kernelSize % 2 == 1);

        for (int x = 0; x < heights.GetLength(0); x++) {
            for (int y = 0; y < heights.GetLength(1); y++) {
                if (mask != null && !mask[x, y]) continue;

                float average = 0;
                int values_added = 0;
                for (int dx = -(kernelSize / 2); dx < kernelSize / 2 + 1; dx++) {
                    for (int dy = -(kernelSize / 2); dy < kernelSize / 2 + 1; dy++) {
                        if (x + dx > 0 && y + dy > 0 && x + dx < heights.GetLength(0) && y + dy < heights.GetLength(1)) {
                            values_added++;
                            average += heights[x + dx, y + dy];
                        }
                    }
                }

                average = average / values_added;
                newHeights[x, y] = average;
            }
        }

        return newHeights;
    }

    public TerrainType GetTerrainType(Vector2Int pos) {
        return _terrainTypes[pos.x, pos.y];
    }

    public float GetWorldHeight(Vector3 pos) {
        UnityEngine.Terrain t = GetComponent<UnityEngine.Terrain>();
        return t.SampleHeight(pos);
    }

    public float GetHeight(Vector2Int pos) {
        return _terrainHeights[pos.x, pos.y];
    }

    void Update() {
        Texture2D texture = new Texture2D(width, length);
        List<Ripple> toRemove = new List<Ripple>();

        foreach (Ripple ripple in ripples) {
            ripple.size += Time.deltaTime*40;
            var circle = GenerateCircleCoords((int)ripple.location.x, (int)ripple.location.z, (int)ripple.size);
            foreach (var vector2Int in circle) {
                texture.SetPixel(vector2Int.x, vector2Int.y, Color.black);
            }

            if (ripple.size > ripple.maxSize) {
                toRemove.Add(ripple);
            }
        }

        foreach (var ripple in toRemove) {
            ripples.Remove(ripple);
        }

        TerrainLayer tl = new TerrainLayer();
        tl.diffuseTexture = texture;
        tl.tileSize = new Vector2(width, length);
        tl.tileOffset = new Vector2(0, 0);
        tl.diffuseTexture.Apply(true);
        UnityEngine.Terrain t = GetComponent<UnityEngine.Terrain>();
        t.terrainData.terrainLayers = new TerrainLayer[] { tl };
    }

    List<Ripple> ripples = new List<Ripple>();
    private float rippleSpeed = 5; // Ripple radius second
    class Ripple {
        public Vector3 location;
        public int maxSize;
        public float size;

        public Ripple(Vector3 location, int maxSize) {
            this.location = location;
            this.maxSize = maxSize;
            this.size = 1;
        }
    }


    public void MakeRipple(Vector3 location, int size) {
        ripples.Add(new Ripple(location, size));
    }


    List<Vector2Int> GenerateCircleCoords(int x0, int y0, int radius) {
        List<Vector2Int> ret = new List<Vector2Int>();

        int x = radius - 1;
        int y = 0;
        int dx = 1;
        int dy = 1;
        int err = dx - (radius << 1);

        while (x >= y) {
            ret.Add(new Vector2Int(x0 + x, y0 + y));
            ret.Add(new Vector2Int(x0 + y, y0 + x));
            ret.Add(new Vector2Int(x0 - y, y0 + x));
            ret.Add(new Vector2Int(x0 - x, y0 + y));
            ret.Add(new Vector2Int(x0 - x, y0 - y));
            ret.Add(new Vector2Int(x0 - y, y0 - x));
            ret.Add(new Vector2Int(x0 + y, y0 - x));
            ret.Add(new Vector2Int(x0 + x, y0 - y));

            if (err <= 0) {
                y++;
                err += dy;
                dy += 2;
            }

            if (err > 0) {
                x--;
                dx += 2;
                err += dx - (radius << 1);
            }
        }

        return ret;
    }
}
