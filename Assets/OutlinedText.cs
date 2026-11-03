using UnityEngine;

public class OutlinedText : MonoBehaviour {

	public TextMesh[] mainTexts;
	public TextMesh[] outlineTexts;

	public void SetText(int index, string text) {
        if (mainTexts == null || outlineTexts == null ||
            index < 0 || index >= mainTexts.Length ||
            index >= outlineTexts.Length) {
            Debug.LogError("Invalid text references!");
            return;
        }

        if (mainTexts[index] == null || outlineTexts[index] == null) {
            Debug.LogError("Missing TextMesh reference at index " + index);
            return;
        }

        mainTexts[index].text = text;
        outlineTexts[index].text = text;
    }

	public void SetColors(int index, Color textColor, Color outlineColor) {
        mainTexts[index].color = textColor;
        outlineTexts[index].color = outlineColor;
    }
}
