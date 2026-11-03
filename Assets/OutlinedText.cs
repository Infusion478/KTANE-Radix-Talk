using UnityEngine;

public class OutlinedText : MonoBehaviour {

	public TextMesh[] mainTexts;
	public TextMesh[] outlineTexts;

	public void SetText(int index, string text) {
        mainTexts[index].text = text;
        outlineTexts[index].text = text;
    }

	public void SetColors(int index, Color textColor, Color outlineColor) {
        mainTexts[index].color = textColor;
        outlineTexts[index].color = outlineColor;
    }
}
