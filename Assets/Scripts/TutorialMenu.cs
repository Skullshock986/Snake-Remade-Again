using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMenu : MonoBehaviour
{

    public List<GameObject> tutorialImgs;
    public int pointer;

    // Start is called before the first frame update
    void Start()
    {
        pointer = 0;
    }

    public void ForwardPage()
    {
        foreach (GameObject img in tutorialImgs)
        {
            img.SetActive(false);
        }
        pointer = (pointer + 1) % 3;
        tutorialImgs[pointer].SetActive(true);
    }

    public void BackPage()
    {
        foreach (GameObject img in tutorialImgs)
        {
            img.SetActive(false);
        }
        pointer = (pointer - 1);
        if(pointer < 0)
        {
            pointer = 2;
        }
        tutorialImgs[pointer].SetActive(true);
    }
}
