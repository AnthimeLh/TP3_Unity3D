using UnityEngine;

// Utilitaire : génère un Sprite en forme de "pilule" (rectangle aux extrémités
// parfaitement arrondies, comme une gélule) directement en mémoire, sans PNG externe.
public static class PillSpriteFactory
{
    public static Sprite CreatePill(int width = 256, int height = 64)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        float radius = height / 2f;
        Color32[] pixels = new Color32[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float alpha = 1f;

                if (x < radius)
                {
                    // Demi-cercle gauche
                    float dx = radius - x - 0.5f;
                    float dy = (height / 2f) - y - 0.5f;
                    alpha = (dx * dx + dy * dy) <= radius * radius ? 1f : 0f;
                }
                else if (x > width - radius)
                {
                    // Demi-cercle droit
                    float dx = x - (width - radius) - 0.5f;
                    float dy = (height / 2f) - y - 0.5f;
                    alpha = (dx * dx + dy * dy) <= radius * radius ? 1f : 0f;
                }
                // sinon : zone centrale rectangulaire, alpha = 1

                pixels[y * width + x] = new Color32(255, 255, 255, alpha >= 1f ? (byte)255 : (byte)0);
            }
        }

        tex.SetPixels32(pixels);
        tex.Apply();

        // Border (gauche, bas, droite, haut) utilisé en mode "Sliced" pour que
        // les extrémités restent rondes quelle que soit la largeur de la barre.
        Vector4 border = new Vector4(radius, 0, radius, 0);
        return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
    }
}
