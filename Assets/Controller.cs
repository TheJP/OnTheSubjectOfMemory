using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    public Text label;
    public Button[] buttons;
    public GameObject url;
    public GameObject explosionPrefab;
    public Transform pad;

    private Quaternion startAnimationSource;
    private readonly Quaternion startAnimationTarget = Quaternion.identity;
    private float startDuration = 1f;
    private float startTime;
    private int round = 0;
    private bool ignoreInput = true;
    private readonly Setup[] setups = new[]
    {
        new Setup(4, 3, 4, 2, 1),
        new Setup(2, 2, 4, 1, 3),
        new Setup(2, 1, 2, 3, 4),
        new Setup(1, 1, 3, 2, 4),
        new Setup(2, 4, 3, 1, 2),
    };
    /// <summary>Correct button positions</summary>
    private readonly int[] solution = new[] { 4, 4, 1, 4, 2 };

    private void Start()
    {
        startTime = Time.time;
        startAnimationSource = pad.rotation;
        for (int i = 0; i < buttons.Length; ++i)
        {
            var position = i + 1;
            buttons[i].Clicked += label => Click(position, label);
        }
        NextRound();
    }

    private void Click(int position, string label)
    {
        if (ignoreInput) { return; }
        if (position == solution[round - 1])
        {
            this.label.text = "";
            foreach (var button in buttons) { button.label.text = ""; }
            ignoreInput = true;
            if (round >= 5)
            {
                Victory();
            }
            else
            {
                Invoke("NextRound", 1.5f);
            }
        }
        else { Explosion(); }
    }

    private void Explosion()
    {
        var explosion = Instantiate(explosionPrefab, new Vector3(0, 0, -2), Quaternion.identity);
        explosion.transform.localScale = new Vector3(2, 2, 2);
        ignoreInput = true;
        Invoke("Restart", 6.5f);
        pad.gameObject.SetActive(false);
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    private void NextRound()
    {
        // Setup visuals
        var setup = setups[round];
        for (int i = 0; i < buttons.Length; ++i)
        {
            buttons[i].label.text = setup.buttons[i].ToString();
        }
        label.text = setup.label.ToString();

        ++round;
        ignoreInput = false;
    }

    private void Victory()
    {
        var indicator = FindObjectOfType<Indicator>();
        indicator.success = true;
        label.text = "";
        url.SetActive(true);
        foreach(var button in buttons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        pad.rotation = Quaternion.Lerp(startAnimationSource, startAnimationTarget, (Time.time - startTime) / startDuration);
        pad.localScale = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(1, 1, 1), (Time.time - startTime) / startDuration);
    }

    class Setup
    {
        public readonly int label;
        public readonly int[] buttons; // Warning: mutable

        public Setup(int label, params int[] buttons)
        {
            this.label = label;
            this.buttons = buttons;
        }
    }
}
