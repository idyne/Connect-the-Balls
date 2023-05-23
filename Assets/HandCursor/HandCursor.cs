using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandCursor : MonoBehaviour
{
    private static HandCursor instance = null;
    public static HandCursor Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<HandCursor>();
            return instance;
        }
    }
    [SerializeField] private RectTransform cursor;
    [SerializeField] private Image image;
    [SerializeField] private Hand[] hands;
    int currentIndex = 0;
    int currentSizeIndex = 0;
    float height = 200;


    private void Update()
    {
        cursor.position = Input.mousePosition;
    }

    public void NextImage()
    {
        int index = ++currentIndex % (hands.Length + 1);
        if (index == 0)
        {
            image.enabled = false;
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
            image.enabled = true;
            Hand hand = hands[index - 1];
            image.sprite = hand.sprite;
            image.rectTransform.pivot = hand.pivot;
            float aspectRatio = hand.sprite.rect.size.y / height;
            image.rectTransform.sizeDelta = hand.sprite.rect.size / aspectRatio * (1 + currentSizeIndex * 0.2f);
        }
    }

    public void NextSize()
    {
        currentSizeIndex = (currentSizeIndex + 1) % 9;
        float aspectRatio = image.sprite.rect.size.y / height;
        image.rectTransform.sizeDelta = image.sprite.rect.size / aspectRatio * (1 + currentSizeIndex * 0.2f);
    }


    [System.Serializable]
    public class Hand
    {
        [SerializeField] public Sprite sprite;
        [SerializeField] public Vector2 pivot;
    }
}
