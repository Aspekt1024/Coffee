using System;
using UnityEngine;

namespace Coffee
{
    public class Controller : MonoBehaviour
    {
        private Actor player;
        
        private void Start()
        {
            player = GameManager.Character.Player;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleClick(Input.mousePosition);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                HandleInteract();
            }
        }

        private void HandleClick(Vector2 mousePos)
        {
            var cam = Camera.main;
            var ray = cam.ScreenPointToRay(mousePos);

            var layers = 1 << LayerMask.NameToLayer("Interactible") | LayerMask.NameToLayer("Floor");

            bool hit = Physics.Raycast(ray, out var hitInfo, 100f, layers);
            if (hit)
            {
                Debug.Log("hit " + hitInfo.collider.name);
            }
            
        }

        private void HandleInteract()
        {
            var origin = player.transform.position;
            origin.y = 1f;

            var layer = 1 << LayerMask.NameToLayer("Interactible");

            bool hit = Physics.Raycast(origin, player.transform.forward, out var hitInfo, 5f, layer);

            if (!hit) return;
            
            var interactible = hitInfo.collider.GetComponent<IInteractible>();
            if (interactible == null)
            {
                interactible = hitInfo.collider.GetComponentInParent<IInteractible>();
                if (interactible == null) return;
            }
            interactible.Use(player);

        }
    }
}