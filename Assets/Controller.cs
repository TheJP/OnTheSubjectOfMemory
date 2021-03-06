﻿using System;
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
    public Text countDownText;
    public float countDownStart = 60f;

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
    /// <summary>Correct button positions (starting with 1)</summary>
    private readonly int[] solution = new[] { 4, 4, 1, 4, 2 };
    private bool gameOver = false;
    private int lastBeep = 0;

    private void CreateSolution()
    {
        var random = new System.Random();
        for (int i = 0; i < setups.Length; ++i)
        {
            setups[i] = new Setup(UnityEngine.Random.Range(1, 5), new[] { 1, 2, 3, 4 }.Shuffle(random));
            solution[i] = FindSolution(i);
        }
    }

    private int FindButtonWithLabel(int[] buttons, int buttonLabel)
    {
        return Array.FindIndex(buttons, button => button == buttonLabel) + 1;
    }

    private int FindButtonWithSameLabelAsInStage(int[] buttons, int stage)
    {
        return FindButtonWithLabel(buttons, setups[stage].buttons[solution[stage] - 1]);
    }

    private int FindSolution(int stage)
    {
        var label = setups[stage].label;
        var buttons = setups[stage].buttons;
        switch (stage)
        {
            case 0:
                if (label == 1) { return 2; }
                return label;
            case 1:
                switch (label)
                {
                    case 2:
                    case 4:
                        return solution[0];
                    case 1: return FindButtonWithLabel(buttons, 4);
                    case 3: return 1;
                    default: throw new ArgumentException("Invalid label number " + label);
                }
            case 2:
                switch (label)
                {
                    case 1: return FindButtonWithSameLabelAsInStage(buttons, 1);
                    case 2: return FindButtonWithSameLabelAsInStage(buttons, 0);
                    case 3: return 3;
                    case 4: return FindButtonWithLabel(buttons, 4);
                    default: throw new ArgumentException("Invalid label number " + label);
                }
            case 3:
                switch (label)
                {
                    case 3:
                    case 4:
                        return solution[1];
                    case 1: return solution[0];
                    case 2: return 1;
                    default: throw new ArgumentException("Invalid label number " + label);
                }
            case 4:
                switch (label)
                {
                    case 1:
                    case 2:
                        return FindButtonWithSameLabelAsInStage(buttons, label - 1);
                    case 3: return FindButtonWithSameLabelAsInStage(buttons, 3);
                    case 4: return FindButtonWithSameLabelAsInStage(buttons, 2);
                    default: throw new ArgumentException("Invalid label number " + label);
                }
            default: throw new ArgumentException("Invalid stage number " + (stage + 1));
        }
    }

    private void Start()
    {
        startTime = Time.time;
        startAnimationSource = pad.rotation;
        CreateSolution();
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
        if (gameOver) { return; }
        gameOver = true;
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
        if (gameOver) { return; }
        gameOver = true;
        var indicator = FindObjectOfType<Indicator>();
        indicator.success = true;
        label.text = "";
        url.SetActive(true);
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        pad.rotation = Quaternion.Lerp(startAnimationSource, startAnimationTarget, (Time.time - startTime) / startDuration);
        pad.localScale = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(1, 1, 1), (Time.time - startTime) / startDuration);
        if (!gameOver) { UpdateCoundDown(); }
    }

    private void UpdateCoundDown()
    {
        var time = countDownStart - (Time.time - startTime);
        if ((int)time != lastBeep)
        {
            Camera.main.GetComponent<AudioSource>().Play();
            lastBeep = (int)time;
        }
        if (time < 0f) { Explosion(); }
        countDownText.text = string.Format("{0:00}:{1:00}", (int)(time / 60f), (int)(time % 60f));
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
