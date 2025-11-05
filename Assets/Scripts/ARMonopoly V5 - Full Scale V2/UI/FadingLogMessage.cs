// In folder: ARMonopoly V5 - Full Scale V2/UI/
using UnityEngine;
using TMPro;
using System.Collections;

namespace ARMonopoly_V5___Full_Scale_V2.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadingLogMessage : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI logText;
        
        [Header("Fading")]
        [SerializeField] private float displayDuration = 2.0f; // How long to stay at full opacity
        [SerializeField] private float fadeDuration = 1.5f;    // How long to fade out
    
        private CanvasGroup canvasGroup;
    
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    
        /// <summary>
        /// Call this immediately after instantiating to set the message.
        /// </summary>
        public void Initialize(string message)
        {
            if (logText != null)
            {
                logText.text = message;
            }
            StartCoroutine(FadeOutCoroutine());
        }
    
        private IEnumerator FadeOutCoroutine()
        {
            // 1. Stay visible for the display duration
            yield return new WaitForSeconds(displayDuration);
    
            // 2. Fade out
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = 1.0f - (elapsedTime / fadeDuration);
                yield return null;
            }
    
            // 3. Destroy this object
            Destroy(gameObject);
        }
    }
}

