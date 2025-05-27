using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneImages : MonoBehaviour
{
    public List<Texture> CutScenesImages = new List<Texture>();
    int index = 0;
    bool flag = true;
    [Header("----------------------------------")]
    public RawImage image;
    public float transitionTime = 2f;
    float alpha =0f;
    public void InitiateFade()
    {

        if (flag)
            StartCoroutine(Fade());
        else
            return;
    }
    private IEnumerator Fade()
    {
        flag = false;
        image.CrossFadeAlpha(0, transitionTime, true);
        yield return new WaitForSeconds(transitionTime);
        if (index < CutScenesImages.Count)
        {
            image.texture = CutScenesImages[index];
            image.CrossFadeAlpha(1, transitionTime, true);
            index++;
            flag = true;
        }
        else
            image.CrossFadeAlpha(0f, transitionTime, true);
        
    }
}
