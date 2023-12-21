using Godot;
using System;
using System.Collections.Generic;

public class CardCleaner : Position2D
{
    public float SecondsBetweenClean = 0.05f;
    private readonly List<CleaningInfo> cardsToClean = new List<CleaningInfo>();
    private readonly List<CleaningInfo> cardsBeingCleaned = new List<CleaningInfo>();
    private float timer = 0f;

    public override void _Process(float delta)
    {
        timer += delta;
        if (timer > SecondsBetweenClean)
        {
            timer -= SecondsBetweenClean;
            CleanACard();
            UpdateCleaning();
        }
    }

    public void AddCardToClean(Card card, bool preserveCard = false)
    {
        AddChild(card);
        cardsToClean.Add(new CleaningInfo()
        {
            card = card,
            freeAfterCleaning = !preserveCard,
        });
    }

    private void CleanACard()
    {
        if (cardsToClean.Count > 0)
        {
            CleaningInfo card = cardsToClean[0];
            cardsToClean.RemoveAt(0);
            cardsBeingCleaned.Add(card);
            card.card.FlipToPosition(Position);
        }
    }

    public bool RemoveCard(Card card)
    {
        card.ClearAnimations();
        int index = cardsToClean.FindIndex(c => c.card.GetInstanceId() == card.GetInstanceId());
        if (index != -1)
        {
            RemoveChild(cardsToClean[index].card);
            cardsToClean.RemoveAt(index);
            return true;
        }

        index = cardsBeingCleaned.FindIndex(c => c.card == card);
        if (index != -1)
        {
            RemoveChild(cardsBeingCleaned[index].card);
            cardsBeingCleaned.RemoveAt(index);
            return true;
        }
        return false;
    }

    private void UpdateCleaning()
    {
        for (int i = cardsBeingCleaned.Count - 1; i >= 0; i--)
        {
            CleaningInfo cleaningInfo = cardsBeingCleaned[i];
            if (!cleaningInfo.card.IsMoving)
            {
                RemoveChild(cleaningInfo.card);
                if (cleaningInfo.freeAfterCleaning)
                    cleaningInfo.card.QueueFree();
                cardsBeingCleaned.RemoveAt(i);
            }
        }
    }


    // === Private Struct ===


    private struct CleaningInfo
    {
        public Card card;
        public bool freeAfterCleaning;
    }
}
