using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpHandle : MonoBehaviour
{

    private bool shuffleSprite1;
    private bool shuffleSprite2;
    private bool shuffleSprite3;

    public float TimeBtwShuffle;

    public Sprite[] ItemGraphics1;
    public Sprite[] ItemGraphics2;
    public Sprite[] ItemGraphics3;

    public Sprite EmptyItem;

    public Image Img1;
    public Image Img2;
    public Image Img3;

    public PlayerItems SnakeItems;
    // Start is called before the first frame update
    private void Start()
    {
        shuffleSprite1 = true;
        shuffleSprite2 = true;
        shuffleSprite3 = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(SnakeItems.heldPU1 == -1)
        {
            if (SnakeItems.canPickUp1)
            {
                // As long as the player has no power-up in PU1, have the icon display in the first power-up indicator square be the EmptyItem image
                Img1.sprite = EmptyItem;
            }
            else
            {
                // If the player is starting to pick up a power-up, pick a random power-up to display
                if (shuffleSprite1)
                {
                    Invoke("Shuffle1", TimeBtwShuffle); // Run the Shuffle1 function after TimeBtwShuffle seconds (set to 0.05)
                    shuffleSprite1 = false; // Set shuffleSprite1 to false (means that the power-up icon won't be shuffled again until Shuffle1 sets this boolean to true again
                }
                // Repeat this until heldPU1 is no longer -1 (a power-up has been given)
            }
        }
        else
        {
            // When the player gets a power-up, set the icon as the visual sprite in the power-up object's Visual variable
            Img1.sprite = SnakeItems.PUUse1.Visual;
        }

        if (SnakeItems.heldPU2 == -1)
        {
            if (SnakeItems.canPickUp2)
            {
                // As long as the player has no power-up in PU2, have the icon display in the first power-up indicator square be the EmptyItem image
                Img2.sprite = EmptyItem;
            }
            else
            {
                // If the player is starting to pick up a power-up, pick a random power-up to display
                if (shuffleSprite2)
                {
                    Invoke("Shuffle2", TimeBtwShuffle); // Run the Shuffle2 function after TimeBtwShuffle seconds (set to 0.05)
                    shuffleSprite2 = false; // Set shuffleSprite2 to false (means that the power-up icon won't be shuffled again until Shuffle2 sets this boolean to true again
                }
                // Repeat this until heldPU2 is no longer -1 (a power-up has been given)
            }
        }
        else
        {
            // When the player gets a power-up, set the icon as the visual sprite in the power-up object's Visual variable
            Img2.sprite = SnakeItems.PUUse2.Visual;
        }

        if (SnakeItems.heldPU3 == -1)
        {
            if (SnakeItems.canPickUp3)
            {
                // As long as the player has no power-up in PU3, have the icon display in the first power-up indicator square be the EmptyItem image
                Img3.sprite = EmptyItem;
            }
            else
            {
                // If the player is starting to pick up a power-up, pick a random power-up to display
                if (shuffleSprite3)
                {
                    Invoke("Shuffle3", TimeBtwShuffle); // Run the Shuffle3 function after TimeBtwShuffle seconds (set to 0.05)
                    shuffleSprite3 = false; // Set shuffleSprite3 to false (means that the power-up icon won't be shuffled again until Shuffle3 sets this boolean to true again
                }
                // Repeat this until heldPU3 is no longer -1 (a power-up has been given)
            }
        }
        else
        {
            // When the player gets a power-up, set the icon as the visual sprite in the power-up object's Visual variable
            Img3.sprite = SnakeItems.PUUse3.Visual;
        }
    }

    void Shuffle1()
    {
        // Randomise the icon in the first power-up indicator square
        Img1.sprite = ItemGraphics1[Random.Range(0, ItemGraphics1.Length)];
        shuffleSprite1 = true;
    }

    void Shuffle2()
    {
        // Randomise the icon in the second power-up indicator square
        Img2.sprite = ItemGraphics2[Random.Range(0, ItemGraphics2.Length)];
        shuffleSprite2 = true;
    }

    void Shuffle3()
    {
        // Randomise the icon in the third power-up indicator square
        Img3.sprite = ItemGraphics3[Random.Range(0, ItemGraphics3.Length)];
        shuffleSprite3 = true;
    }
}
