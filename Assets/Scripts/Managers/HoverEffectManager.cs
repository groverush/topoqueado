using UnityEngine;
using UnityEngine.EventSystems;
public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject backgroundImage; // Asigna la imagen en el inspector

    private void Awake()
    {
        if (backgroundImage != null)
            backgroundImage.SetActive(false); // Asegura que inicia desactivada
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (backgroundImage != null)
            backgroundImage.SetActive(true); // Muestra la imagen al hacer hover
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (backgroundImage != null)
            backgroundImage.SetActive(false); // Oculta la imagen al salir del hover
    }
}
