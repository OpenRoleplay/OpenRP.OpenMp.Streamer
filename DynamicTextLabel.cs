using System.Numerics;
using SampSharp.Entities.SAMP;
using SampSharp.OpenMp.Core.Std;
using OpenRP.OpenMp.Streamer.Interop;

namespace OpenRP.OpenMp.Streamer;

/// <summary>Streamer-managed 3D text label (PAWN-native: CreateDynamic3DTextLabel).</summary>
public sealed class DynamicTextLabel : DynamicEntity
{
    private string _text;
    private Color _color;

    internal DynamicTextLabel(int id, string text, Color color, Vector3 position, float drawDistance)
    {
        Id = id;
        _text = text;
        _color = color;
        Position = position;
        DrawDistance = drawDistance;
    }

    public Vector3 Position { get; }
    public float DrawDistance { get; }

    public string Text
    {
        get => _text;
        set
        {
            _text = value ?? string.Empty;
            UpdateNative();
        }
    }

    public Color Color
    {
        get => _color;
        set
        {
            _color = value;
            UpdateNative();
        }
    }

    /// <summary>Updates the colour and the text in one call to the streamer.</summary>
    public bool Update(Color color, string text)
    {
        _color = color;
        _text = text ?? string.Empty;
        return UpdateNative();
    }

    public override bool IsAlive => StreamerInterop.Streamer_TextLabel_IsValid(Id);
    protected override void DestroyNative() => StreamerInterop.Streamer_TextLabel_Destroy(Id);

    private unsafe bool UpdateNative()
    {
        // Encode text in the client encoding
        var enc = StringViewMarshaller.Encoding;
        var byteCount = enc.GetByteCount(_text);
        var buffer = new byte[byteCount + 1];
        enc.GetBytes(_text, buffer);
        buffer[byteCount] = 0;
        fixed (byte* ptr = buffer)
        {
            return StreamerInterop.Streamer_TextLabel_Update(Id, ToRgba(_color), ptr);
        }
    }

    private static uint ToRgba(Color c) => ((uint)c.R << 24) | ((uint)c.G << 16) | ((uint)c.B << 8) | c.A;
}
