using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class Journal_UI : MonoBehaviour
{
    public List <GameObject> rulesList;
    public GameObject[] pages;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void OpenPage(GameObject page)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] == page)
            {
                pages[i].SetActive(true);
            }
            else
            {
                pages[i].SetActive(false);
            }
        }
    }

    public void AddRule(GameObject rule)
    {
        rulesList.Add(rule);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
