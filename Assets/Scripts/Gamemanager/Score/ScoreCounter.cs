using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter
{
    private int score = 0;

    public ScoreCounter() {
    }

    public int CountDestructionPoints(List<Block> list) {

        int points = 0;

        foreach (Block b in list) {
            if (b && b.gameObject) {
                points += 25;
            }
        }
        return points;
    }

    public void AddScore(int s) {
        SetScore(GetScore() + s);
    }

    public int GetScore() {
        return this.score;
    }

    public void SetScore(int s) {
        this.score = s;
    }
}
