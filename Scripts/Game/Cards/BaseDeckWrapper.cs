using Godot;
using System;
using System.Collections.Generic;

public abstract class BaseDeckWrapper
{
    public BaseDeckModel Model { get; private set; }

    public BaseDeckWrapper(BaseDeckModel deckInfo) 
    {
        Model = deckInfo;
    }

    public abstract BaseCardWrapper[] GetBaseDeck();
}
