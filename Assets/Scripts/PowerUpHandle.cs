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
                Img1.sprite = EmptyItem;
            }
            else
            {
                if (shuffleSprite1)
                {
                    Invoke("Shuffle1", TimeBtwShuffle);
                    shuffleSprite1 = false;
                }
            }
        }
        else
        {
            Img1.sprite = SnakeItems.PUUse1.Visual;
        }

        if (SnakeItems.heldPU2 == -1)
        {
            if (SnakeItems.canPickUp2)
            {
                Img2.sprite = EmptyItem;
            }
            else
            {
                if (shuffleSprite2)
                {
                    Invoke("Shuffle2", TimeBtwShuffle);
                    shuffleSprite2 = false;
                }
            }
        }
        else
        {
            Img2.sprite = SnakeItems.PUUse2.Visual;
        }

        if (SnakeItems.heldPU3 == -1)
        {
            if (SnakeItems.canPickUp3)
            {
                Img3.sprite = EmptyItem;
            }
            else
            {
                if (shuffleSprite3)
                {
                    Invoke("Shuffle3", TimeBtwShuffle);
                    shuffleSprite3 = false;
                }
            }
        }
        else
        {
            Img3.sprite = SnakeItems.PUUse3.Visual;
        }
    }

    void Shuffle1()
    {
        Img1.sprite = ItemGraphics1[Random.Range(0, ItemGraphics1.Length)];
        shuffleSprite1 = true;
    }

    void Shuffle2()
    {
        Img2.sprite = ItemGraphics2[Random.Range(0, ItemGraphics2.Length)];
        shuffleSprite2 = true;
    }

    void Shuffle3()
    {
        Img3.sprite = ItemGraphics3[Random.Range(0, ItemGraphics3.Length)];
        shuffleSprite3 = true;
    }
}
