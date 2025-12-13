// PlayerInput.cs
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] BoardController board;

    Gem firstSelection;

    private void OnEnable()
    {
        Gem.Clicked += OnGemClicked;
    }

    private void OnDisable()
    {
        Gem.Clicked -= OnGemClicked;
    }

    void OnGemClicked(Gem gem)
    {
        if (board == null || board.IsBusy)
            return;

        if (firstSelection == null)
        {
            firstSelection = gem;
        }
        else if (firstSelection == gem)
        {
            // другий кл≥к по тому ж Ц скасовуЇмо виб≥р
            firstSelection = null;
        }
        else
        {
            // перев≥р€Їмо, чи сус≥ди
            int dist = Mathf.Abs(firstSelection.X - gem.X) + Mathf.Abs(firstSelection.Y - gem.Y);
            if (dist == 1)
            {
                StartCoroutine(board.ResolveSwap(firstSelection, gem));
            }
            firstSelection = null;
        }
    }
}
