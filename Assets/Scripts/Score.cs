using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {

    public GameObject textObj;
    public int score = 0;
	
    public void AdjustScore(int deltaScore) {
        score += deltaScore;
        TextMesh textMesh = textObj.GetComponent<TextMesh>();
        textMesh.text = score.ToString();
    }
}
