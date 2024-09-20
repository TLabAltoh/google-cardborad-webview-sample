using UnityEngine;
using TLab.Android.WebView;

public class DisableScreenKeyborad : MonoBehaviour
{
#if UNITY_EDITOR
    private void Reset()
    {
        var webview = GetComponent<TLabWebView>();
        if (webview == null)
            return;

        // https://stackoverflow.com/a/65358992/22575350
        webview.eventCallback.onPageFinish = @"
            var elements = [];

            function searchShadowRoot(node, id) {
                if (node == null) {
                    return;
                }

                if (node.shadowRoot != undefined && node.shadowRoot != null) {
                    if (!elements.includes(node.shadowRoot)) {
                        elements.push(node.shadowRoot);
                    }
                    searchShadowRoot(node.shadowRoot, id);
                }

                for (var i = 0; i < node.childNodes.length; i++) {
                    searchShadowRoot(node.childNodes[i], id);
                }
            }

            elements = elements.concat([...document.querySelectorAll('input, textarea')]);
            searchShadowRoot(document, 'input');
            searchShadowRoot(document, 'textarea');

            for (var i = 0; i < elements.length; i++) {
                elements[i].setAttribute('inputmode', 'none');
            }
            ";

        UnityEditor.EditorUtility.SetDirty(webview);
    }
#endif
}
