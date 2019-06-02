using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameManager : MonoBehaviour
{

    public int numCards = 20;
    public GameObject prefab;
    private List<Card> cards;
    string[] vocab = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "H", };
    // Use this for initialization
    void Start()
    {
        SetupGame();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void SetupGame()
    {
        int cols = Mathf.RoundToInt(Mathf.Sqrt(numCards));
        int rows = numCards / cols;
        cards = new List<Card>();
        for (int i = 0; i < numCards; i += 2)
        {
            string letter = vocab[Random.Range(0, vocab.Length)];
            cards.Add(new Card(letter));
        }
        Shuffle();

        int row = 0; int col = 0;
        for (int i = 0; i < numCards; i++)
        {
            Instantiate<GameObject>(prefab, new Vector3(row, 0, col), Quaternion.identity);
            col++;
            if (col > cols)
            {
                row++;
                col = 0;
            }
        }


    }
    public void Shuffle()
    {
        var count = cards.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = cards[i];
            cards[i] = cards[r];
            cards[r] = tmp;
        }
    }
}
