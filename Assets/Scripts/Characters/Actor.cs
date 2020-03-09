using Coffee.Characters;
using Rewired;
using UnityEngine;

namespace Coffee
{
    public class Actor : MonoBehaviour, IActor
    {
        #pragma warning disable 649
        [SerializeField] private int playerId;
        [SerializeField] private InteractionComponent interaction;
        [SerializeField] private BasicMovement movement;
        #pragma warning restore 649
        
        public AnimationComponent Animator { get; private set; }
    
        private Player input;

        public Transform Transform => transform;
        
        private void Awake()
        {
            InitialiseComponents();
        }

        private void Update()
        {
            if (!IsInputAllowed)
            {
                movement.Move(0,0);
                return;
            }
            
            // Movement inputs
            var hAxis = input.GetAxis(InputLabels.MoveHorizontal);
            var vAxis = input.GetAxis(InputLabels.MoveVertical);
            movement.Move(hAxis, vAxis);
            
            // Interaction inputs
            if (input.GetButtonDown(InputLabels.Interact))
            {
                interaction.Interact();
            }
        }

        public void ResetState()
        {
            var item = interaction.RemoveItem();
            item?.Destroy();
        }
        
        private void InitialiseComponents()
        {
            input = ReInput.players.GetPlayer(playerId);
            Animator = new AnimationComponent(GetComponent<Animator>());
            
            interaction.Init(this);
            movement.Init(this);
        }

        /// <summary>
        /// Applies the interaction result. Usually called by the animator
        /// </summary>
        private void ApplyInteraction()
        {
            interaction.Apply();
        }

        private bool IsInputAllowed => !interaction.IsBusy && !GameManager.Gameplay.IsPlayerInputDisabled;
    }
}