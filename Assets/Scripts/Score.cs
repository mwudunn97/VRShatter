using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {

    public int score = 0;

	private void Start()
	{
        score = 0;
	}


	public void AdjustScore(int deltaScore) {
        score += deltaScore;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.text = score.ToString();
    }

    public int GetScore() {
        return score;
    }
}
