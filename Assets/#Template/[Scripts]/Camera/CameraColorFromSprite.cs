using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraColorFromSprite : MonoBehaviour
{
    public Image image;
    public int sampleCount = 100;

    void Start()
    {
        GetColor();
    }

    void GetColor()
    {
        Texture2D texture = image.sprite.texture;
        Color mainColor = GetMainColor(texture);
        Camera.main.backgroundColor = mainColor;

        Invoke("GetColor", 1);
    }

    Color GetMainColor(Texture2D texture)
    {
        Dictionary<Color, int> colorCount = new Dictionary<Color, int>();
        for (int i = 0; i < sampleCount; i++)
        {
            int x = Random.Range(0, texture.width);
            int y = Random.Range(0, texture.height);
            Color color = texture.GetPixel(x, y);
            if (colorCount.ContainsKey(color))
            {
                colorCount[color]++;
            }
            else
            {
                colorCount[color] = 1;
            }
        }
        Color mainColor = default(Color);
        int maxCount = 0;
        foreach (KeyValuePair<Color, int> pair in colorCount)
        {
            if (pair.Value > maxCount)
            {
                maxCount = pair.Value;
                mainColor = pair.Key;
            }
        }
        return mainColor;
    }
}
