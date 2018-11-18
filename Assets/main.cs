using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class main : MonoBehaviour {
    public Button bed1;
    public Button bed2;
    public Button bed3;
    public Button bed4;
    public Button detailButton;
    public GameObject patientDetail;
    public Text detailText;
    public Image pressurePoints;
    public Image splashScreen;
    public TextAsset risksInput;

    private float risksFPS = 0.5f;
    private Vector2 bed1PositionStatic;
    private Vector2 bed2PositionStatic;
    private Vector2 bed3PositionStatic;
    private Vector2 bed4PositionStatic;
    private float[,] risksData;
    private int risksDataPtr;

    // Use this for initialization
    void Start () {
        bed1PositionStatic = bed1.transform.position;
        bed2PositionStatic = bed2.transform.position;
        bed3PositionStatic = bed3.transform.position;
        bed4PositionStatic = bed4.transform.position;
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                if ((i == 2 && j == 2) || (i==1 && j==2) || (i==1 && j==0) || (i==2 && j==0) || (i == 1 && j == 3) || (i == 2 && j == 3))
                {
                    continue;
                }
                Image img = Instantiate(pressurePoints);
                img.transform.parent = patientDetail.transform;
                img.transform.localPosition = new Vector2(i * 90 - 135, j * 90 - 210);
                img.color = new Color(0, 1, 0);
                if ((i == 1 && j == 3) || (i == 2 && j == 3))
                    img.color = new Color(1, 1, 0);
                else if ((i == 1 && j == 2) || (i == 1 && j == 0) || (i == 2 && j == 0))
                    img.color = new Color(1, 0, 0);
            }
        }        
        risksData = new float[10, 4];
        risksDataPtr = 0;
        string[] risksStr = risksInput.text.Split(new char[]{' ', '\n'});
        for(int i = 0; i < 10; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                risksData[i, j] = float.Parse(risksStr[i * 40 + j]);
            }
        }
        patientDetail.SetActive(false);
        detailText.gameObject.SetActive(false);
        startSplashTime = Time.time;
    }

    private float startSceneTime;
    private float startSplashTime;
	// Update is called once per frame
	void Update () {
        if (Time.time > 3)
        {
            splashScreen.gameObject.SetActive(false);
            startSceneTime = Time.time;
        }
        if ((Time.time-3)>risksFPS*(risksDataPtr+1))
        {
            risksDataPtr = Mathf.Min(risksDataPtr + 1, 9);
        }
        AnimateButtonBasedOnProbability(bed1, bed1PositionStatic, risksData[risksDataPtr, 0]);
        AnimateButtonBasedOnProbability(bed2, bed2PositionStatic, risksData[risksDataPtr, 1]);
        AnimateButtonBasedOnProbability(bed3, bed3PositionStatic, risksData[risksDataPtr, 2]);
        AnimateButtonBasedOnProbability(bed4, bed4PositionStatic, risksData[risksDataPtr, 3]);
    }

    public void BedButtonPressed()
    {
        patientDetail.SetActive(true);
    }

    public void DetailTextButtonPressed()
    {
        detailText.gameObject.SetActive(true);
    }

    private void AnimateButtonBasedOnProbability(Button button, Vector2 staticPosition, float p)
    {
        button.GetComponent<Image>().color = Prob2Color(p);
        if (risksDataPtr == 9 && p>0.9f)
        {
            float[] shakeParams = Prob2ShakeSpeedAndAmplitude(p);
            button.transform.position = staticPosition + new Vector2(Mathf.Sin(Time.time * shakeParams[0]) * shakeParams[1], 0);
        }
    }

    private Color Prob2Color(float p)
    {
        float R = Mathf.Min(p * 2, 1);
        float G = Mathf.Max((1 - p) * 2, 0); 
        return new Color(R, G, 0);
    }

    private float[] Prob2ShakeSpeedAndAmplitude(float p)
    {
        float cutoff = 0f;
        float[] ret = new float[2];
        if (p < cutoff)
        {
            ret[0] = 0;
            ret[1] = 0;
        }
        else
        {
            ret[0] = 30f;
            ret[1] = 2.5f;
        }
        return ret;
    }
}