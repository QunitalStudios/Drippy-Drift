using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public CarControllerTEST playerCont;
    public TMP_Text currentScore;
    public TMP_Text currentMultipier;
    public TMP_Text currentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        currentScore.text = "Score: 0";
        currentMultipier.text = "";
        currentSpeed.text = "Speed: 0 KPH";
    }

    // Update is called once per frame
    void Update()
    {
        currentSpeed.text = "Speed: " + Mathf.Round(playerCont.currentSpeed) + " KPH";

        currentMultipier.GetComponent<Animator>().SetBool("x5", false);
        currentMultipier.GetComponent<Animator>().SetBool("x4", false);

        currentScore.text = "Score: " + Mathf.Round(playerCont.playerScore);
        currentMultipier.text = playerCont.currentMultiplier;
        currentMultipier.color = playerCont.currentMPColor;

        if (playerCont.currentMultiplier == "x4")
        {
            currentMultipier.GetComponent<Animator>().SetBool("x5", false);
            currentMultipier.GetComponent<Animator>().SetBool("x4", true);
        }
        if (playerCont.currentMultiplier == "x5")
        {
            currentMultipier.GetComponent<Animator>().SetBool("x4", false);
            currentMultipier.GetComponent<Animator>().SetBool("x5", true);
        }
    }
}