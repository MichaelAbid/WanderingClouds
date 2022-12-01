#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;


public static class Extension_Handles
{
    public static void DrawWireSquare(Vector3 center, Vector2 size)
    {
        Vector3 halfSize = ((Vector3)size)/2;

        Handles.DrawLines(new Vector3[]{
            center + Vector3.Scale(halfSize, new Vector2(-1,1)),
            center + halfSize,
            center + halfSize,
            center + Vector3.Scale(halfSize, new Vector2(1,-1)),
            center + Vector3.Scale(halfSize, new Vector2(1,-1)),
            center - halfSize,
            center - halfSize,
            center + Vector3.Scale(halfSize, new Vector2(-1,1)),
        });
    }

    public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color _color = default(Color))
    {
        if (_color != default(Color))
            Handles.color = _color;
        Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);
        using (new Handles.DrawingScope(angleMatrix))
        {
            var pointOffset = (_height - (_radius * 2)) / 2;

            //draw sideways
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
            Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
            Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
            //draw frontways
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
            Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
            Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
            //draw center
            Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
            Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);

        }
    }
    public static void DrawWireCapsule(Vector3 _pos, Vector3 _pos2, float _radius, Color _color = default)
    {
        if (_color != default) Handles.color = _color;

        var forward = _pos2 - _pos;
        var _rot = Quaternion.LookRotation(forward);
        var pointOffset = _radius / 2f;
        var length = forward.magnitude;
        var center2 = new Vector3(0f, 0, length);

        Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);

        using (new Handles.DrawingScope(angleMatrix))
        {
            Handles.DrawWireDisc(Vector3.zero, Vector3.forward, _radius);
            Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.left * pointOffset, -180f, _radius);
            Handles.DrawWireArc(Vector3.zero, Vector3.left, Vector3.down * pointOffset, -180f, _radius);
            Handles.DrawWireDisc(center2, Vector3.forward, _radius);
            Handles.DrawWireArc(center2, Vector3.up, Vector3.right * pointOffset, -180f, _radius);
            Handles.DrawWireArc(center2, Vector3.left, Vector3.up * pointOffset, -180f, _radius);

            DrawLine(_radius, 0f, length);
            DrawLine(-_radius, 0f, length);
            DrawLine(0f, _radius, length);
            DrawLine(0f, -_radius, length);
        }

        void DrawLine(float arg1, float arg2, float forward)
        {
            Handles.DrawLine(new Vector3(arg1, arg2, 0f), new Vector3(arg1, arg2, forward));
        }
    }
}
#endif
