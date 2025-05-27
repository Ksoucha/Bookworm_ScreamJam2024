using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneText : MonoBehaviour
{
    public List<Texture> CutSceneText = new List<Texture>();
    int index = 0;
    bool flag = true;
    [Header("----------------------------------")]
    public TMP_Text text;
    public float transitionTime = 2f;
    float alpha = 0f;
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
        text.CrossFadeAlpha(0, transitionTime, true);
        yield return new WaitForSeconds(transitionTime);
        if (index < CutSceneText.Count)
        {
            //text.texture = CutSceneText[index];
            text.CrossFadeAlpha(1, transitionTime, true);
            index++;
            flag = true;
        }
        else
            text.CrossFadeAlpha(0f, transitionTime, true);

    }
}
