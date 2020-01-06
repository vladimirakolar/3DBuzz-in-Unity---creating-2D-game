using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFloatingTextPositioner
{
    bool GetPosition(ref Vector2 position, GUIContent content, Vector2 size);
}
