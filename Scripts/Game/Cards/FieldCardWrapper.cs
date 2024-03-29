﻿using System;


public class FieldCardWrapper : BaseCardWrapper
{
    private FieldCardModel model;
    public FieldCardModel Model
    {
        get => model;
        private set
        {
            model = value;
            Card.SetLabel(value.Name);
        }
    }

    public FieldCardWrapper(Card card, FieldCardModel model)
    {
        Card = card;
        Model = model;
    }
}
