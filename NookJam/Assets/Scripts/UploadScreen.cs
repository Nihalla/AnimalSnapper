using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UploadScreen : MonoBehaviour
{
    private List<GameObject> planes = new();

    void Start()
    {
        foreach (Transform child in gameObject.transform)
        {
            planes.Add(child.gameObject);
        }
    }

    public void SetTexture(string animal)
    {
        GameObject plane_to_update = GiveChild(animal);
        if (plane_to_update != null)
        {
            byte[] image_as_bytes = null;
            image_as_bytes = System.IO.File.ReadAllBytes(Application.dataPath + "/Screenshots/" + animal + ".png");

            Texture2D new_texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            if (image_as_bytes != null)
            {
                new_texture.LoadImage(image_as_bytes);

                Color[] pix = new_texture.GetPixels();
                for (int i = 0; i < pix.Length; i++)
                    pix[i].a = pix[i].grayscale;
                new_texture.SetPixels(pix);
                new_texture.Apply();
                Material m = plane_to_update.GetComponent<MeshRenderer>().material;
                m.mainTexture = new_texture;
            }
        }
    }

    private GameObject GiveChild(string animal)
    {
        foreach (GameObject child in planes)
        {
            if (child.transform.tag == animal)
            {
                return child;
            }
        }

        return null;
    }
}