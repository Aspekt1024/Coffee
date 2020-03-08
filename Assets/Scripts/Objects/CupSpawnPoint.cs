using System;
using Coffee.Characters;
using UnityEngine;

namespace Coffee
{
    public class CupSpawnPoint : MonoBehaviour, IInteractible
    {
        private enum States
        {
            CupSet,
            CupRemoved,
        }

        private States state;

        private Cup cup;

        public void Set(Cup c)
        {
            cup = c;
            
            var cupTf = cup.transform;
            var spawnTf = transform;
            
            cupTf.position = spawnTf.position;
            cupTf.rotation = spawnTf.rotation;
            
            cupTf.SetParent(spawnTf);

            state = States.CupSet;
        }
        
        public InteractionTypes Use(IInteractionComponent interactor)
        {
            switch (state)
            {
                case States.CupSet:
                    var used = cup.Use(interactor);
                    if (!used && interactor.CurrentItem == null)
                    {
                        interactor.ReceiveItem(cup);
                        cup = null;
                        state = States.CupRemoved;
                        return InteractionTypes.Grab;
                    }
                    return InteractionTypes.None;
                case States.CupRemoved:
                    if (interactor.CurrentItem is Cup)
                    {
                        var c = (Cup)interactor.RemoveItem();
                        Set(c);
                        return InteractionTypes.Grab;
                    }
                    break;
                default:
                    Debug.LogError("invalid state: " + state);
                    break;
            }

            return InteractionTypes.None;
        }
    }
}