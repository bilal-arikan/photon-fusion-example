using Fusion.Sample.DedicatedServer;
using UnityEngine;
using UnityEngine.UI;

public class ColorList : MonoBehaviour
{
    [SerializeField] private Button[] colorButtons;

    private void Start()
    {
        for (int i = 0; i < colorButtons.Length; i++)
        {
            var btn = colorButtons[i];
            btn.onClick.AddListener(() =>
            {
                SpaceshipBehaviour.Local.RPC_SetColor(btn.targetGraphic.color);
            });
        }
    }
}