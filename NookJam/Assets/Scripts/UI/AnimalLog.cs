using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalLog : MonoBehaviour
{
    [SerializeField] private int current_screen = 0;
    [SerializeField] private TMP_Text animal;
    [SerializeField] private TMP_Text missing;
    [SerializeField] public Slider menu_slider;
    [SerializeField] RawImage AnimalImage;
    private GameObject current_animal;
    private bool update_photo = false;

    void FixedUpdate()
    {
        switch (current_screen)
        {
            case 0:
                animal.text = "Pig";
                current_animal = GameObject.FindGameObjectWithTag("Pig");
                if (current_animal != null && current_animal.GetComponent<Animal>().marked)
                {
                    missing.text = "A Pig has been spotted";
                    AnimalImage.enabled = true;
                    if(update_photo)
                    {
                        UpdateImage("Pig");
                        update_photo = false;
                    }
                    
                }
                else
                {
                    missing.text = "?";
                    AnimalImage.enabled = false;
                }
                break;
            case 1:
                animal.text = "Chicken";
                current_animal = GameObject.FindGameObjectWithTag("chicken");
                if (current_animal != null && current_animal.GetComponent<Animal>().marked)
                {
                    missing.text = "A Chicken has been spotted";
                    AnimalImage.enabled = true;
                    if (update_photo)
                    {
                        UpdateImage("Chicken");
                        update_photo = false;
                    }
                }
                else
                {
                    missing.text = "?";
                    AnimalImage.enabled = false;
                }
                break;
            case 2:
                animal.text = "Dog";
                current_animal = GameObject.FindGameObjectWithTag("Dog");
                if (current_animal != null && current_animal.GetComponent<Animal>().marked)
                {
                    missing.text = "A Dog has been spotted";
                    AnimalImage.enabled = true;
                    if (update_photo)
                    {
                        UpdateImage("Dog");
                        update_photo = false;
                    }
                }
                else
                {
                    missing.text = "?";
                    AnimalImage.enabled = false;
                }
                break;
            case 3:
                animal.text = "Rooster";
                current_animal = GameObject.FindGameObjectWithTag("Rooster");
                if (current_animal != null && current_animal.GetComponent<Animal>().marked)
                {
                    missing.text = "A Rooster has been spotted";
                    AnimalImage.enabled = true;
                    if (update_photo)
                    {
                        UpdateImage("Rooster");
                        update_photo = false;
                    }
                }
                else
                {
                    missing.text = "?";
                    AnimalImage.enabled = false;
                }
                break;
            case 4:
                animal.text = "Cow";
                current_animal = GameObject.FindGameObjectWithTag("Cow");
                if (current_animal != null && current_animal.GetComponent<Animal>().marked)
                {
                    missing.text = "A Cow has been spotted";
                    AnimalImage.enabled = true;
                    if (update_photo)
                    {
                        UpdateImage("Cow");
                        update_photo = false;
                    }
                }
                else
                {
                    missing.text = "?";
                    AnimalImage.enabled = false;
                }
                break;
            case 5:
                animal.text = "Sheep";
                current_animal = GameObject.FindGameObjectWithTag("Sheep");
                if (current_animal != null && current_animal.GetComponent<Animal>().marked)
                {
                    missing.text = "A Sheep has been spotted";
                    AnimalImage.enabled = true;
                    if (update_photo)
                    {
                        UpdateImage("Sheep");
                        update_photo = false;
                    }
                }
                else
                {
                    missing.text = "?";
                    AnimalImage.enabled = false;
                }
                break;
            case 6:
                animal.text = "Turkey";
                current_animal = GameObject.FindGameObjectWithTag("Turkey");
                if (current_animal != null && current_animal.GetComponent<Animal>().marked)
                {
                    missing.text = "A Turkey has been spotted";
                    AnimalImage.enabled = true;
                    if (update_photo)
                    {
                        UpdateImage("Turkey");
                        update_photo = false;
                    }
                }
                else
                {
                    missing.text = "?";
                    AnimalImage.enabled = false;
                }
                break;
        }
    }

    public void Slider()
    {
        current_screen = (int)menu_slider.value;
    }

    public void UpdateImage(string AnimalImageToUpdate)
    {
        byte[] image_as_bytes = null;
        image_as_bytes = System.IO.File.ReadAllBytes(Application.dataPath + "/Screenshots/" + AnimalImageToUpdate + ".png");

        Texture2D new_texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        if (image_as_bytes != null)
        {
            new_texture.LoadImage(image_as_bytes);

            Color[] pix = new_texture.GetPixels();
            for (int i = 0; i < pix.Length; i++)
                pix[i].a = pix[i].grayscale;
            new_texture.SetPixels(pix);
            new_texture.Apply();
            AnimalImage.texture = new_texture;
        }
    }

    public void TakenPhoto()
    {
        update_photo = true;
    }
}
