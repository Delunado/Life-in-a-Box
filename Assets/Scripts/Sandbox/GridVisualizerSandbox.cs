using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sand.Sandbox
{
    public class GridVisualizerSandbox : MonoBehaviour
    {
        [SerializeField] private Renderer renderer;

        private Texture2D texture;

        private Color[] colors;

        public void Configure(int width, int height)
        {
            texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
            };

            renderer.material.mainTexture = texture;

            colors = new Color[width * height];
        }

        public void Visualize(ParticleGrid particles, int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    colors[x + y * width] = particles.GetColorAt(x, y);
                }
            }

            texture.SetPixels(colors);

            texture.Apply();
        }
    }
}