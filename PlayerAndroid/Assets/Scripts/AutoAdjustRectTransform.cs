using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoAdjustRectTransform : MonoBehaviour
{
    // Referencia al RectTransform que se va a ajustar automáticamente
    private RectTransform rectTransform;

    // Referencia al VerticalLayoutGroup
    private VerticalLayoutGroup verticalLayoutGroup;

    // Tamaño mínimo para el RectTransform
    public float minHeight = 100f;

    void Awake()
    {
        // Obtener las referencias a los componentes necesarios
        rectTransform = GetComponent<RectTransform>();
        verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();

        // Llamar a la función para ajustar el tamaño inicialmente
        AdjustRectTransform();
    }

    void Update()
    {
        // Llamar a la función para ajustar el tamaño si hay cambios en los botones
        if (CheckForButtonChanges())
        {
            AdjustRectTransform();
        }
    }

    void AdjustRectTransform()
    {
        // Calcular el tamaño del contenido (sumando alturas de todos los botones)
        float contentHeight = 0f;
        int buttonCount = verticalLayoutGroup.transform.childCount;
        for (int i = 0; i < buttonCount; i++)
        {
            RectTransform buttonRect = (RectTransform)verticalLayoutGroup.transform.GetChild(i);
            ContentSizeFitter contentSizeFitter = buttonRect.GetComponent<ContentSizeFitter>();

            // Ajustar temporalmente el ContentSizeFitter para obtener la altura preferida
            ContentSizeFitter.FitMode originalVerticalFit = contentSizeFitter.verticalFit;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Obtener la altura preferida del botón
            float preferredHeight = LayoutUtility.GetPreferredHeight(buttonRect);

            // Restaurar el ajuste original del ContentSizeFitter
            contentSizeFitter.verticalFit = originalVerticalFit;

            contentHeight += preferredHeight;
        }

        // Añadir el espacio entre los elementos (espaciado vertical del VerticalLayoutGroup)
        contentHeight += (buttonCount - 1) * verticalLayoutGroup.spacing;

        // Añadir el padding superior e inferior del VerticalLayoutGroup
        contentHeight += verticalLayoutGroup.padding.top + verticalLayoutGroup.padding.bottom;

        // Limitar el tamaño mínimo para el RectTransform
        contentHeight = Mathf.Max(contentHeight, minHeight);

        // Asignar el nuevo tamaño al RectTransform
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, contentHeight);
    }

    bool CheckForButtonChanges()
    {
        // Verificar si ha habido cambios en la cantidad de botones
        return verticalLayoutGroup.transform.childCount != rectTransform.childCount;
    }
}