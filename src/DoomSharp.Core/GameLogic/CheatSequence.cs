namespace DoomSharp.Core.GameLogic;

/// <summary>
/// Cheat sequence checking - port of m_cheat.c from the original DOOM source.
/// Uses the SCRAMBLE bit-shuffling algorithm to encode cheat sequences.
/// </summary>
public class CheatSequence
{
    private static bool _firstTime = true;
    private static readonly byte[] CheatXlateTable = new byte[256];

    private readonly byte[] _sequence;
    private int _position;

    /// <summary>
    /// Create a cheat sequence from a plaintext string.
    /// The string is encoded using the SCRAMBLE algorithm.
    /// Use '\x01' as a placeholder for parameter characters (digits typed by user).
    /// Use no special suffix for sequences without parameters.
    /// </summary>
    public CheatSequence(string cheatString)
    {
        // Encode the cheat string using SCRAMBLE, then add 0xff terminator
        // Parameter placeholder bytes (0x01) are kept as-is
        _sequence = new byte[cheatString.Length + 1];
        for (var i = 0; i < cheatString.Length; i++)
        {
            var c = (byte)cheatString[i];
            _sequence[i] = c == 0x01 ? c : Scramble(c);
        }
        _sequence[cheatString.Length] = 0xff; // end marker

        _position = 0;
    }

    /// <summary>
    /// The SCRAMBLE macro from the original source - shuffles bits of a byte.
    /// </summary>
    private static byte Scramble(int a)
    {
        return (byte)(
            ((a & 1) << 7) +
            ((a & 2) << 5) +
            (a & 4) +
            ((a & 8) << 1) +
            ((a & 16) >> 1) +
            (a & 32) +
            ((a & 64) >> 5) +
            ((a & 128) >> 7)
        );
    }

    /// <summary>
    /// Check if the given key advances or completes the cheat sequence.
    /// Returns true if the full cheat has been entered.
    /// Port of cht_CheckCheat from m_cheat.c.
    /// </summary>
    public bool CheckCheat(char key)
    {
        if (_firstTime)
        {
            _firstTime = false;
            for (var i = 0; i < 256; i++)
            {
                CheatXlateTable[i] = Scramble(i);
            }
        }

        if (_sequence[_position] == 0)
        {
            // Parameter position - store the raw key
            _sequence[_position] = (byte)key;
            _position++;
        }
        else if (CheatXlateTable[(byte)key] == _sequence[_position])
        {
            _position++;
        }
        else
        {
            _position = 0;
        }

        if (_sequence[_position] == 1)
        {
            // Hit a parameter marker - advance past it
            _position++;
        }
        else if (_sequence[_position] == 0xff)
        {
            // Completed the sequence
            _position = 0;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Extract parameter characters that were captured during sequence entry.
    /// Port of cht_GetParam from m_cheat.c.
    /// </summary>
    public void GetParam(char[] buffer)
    {
        var p = 0;

        // Skip past the encoded prefix to find the parameter bytes
        // In the sequence, parameters were originally 0x01 markers that got
        // replaced with 0x00 (zeroed) then filled with user keystrokes
        // Find the first 0x01 in the original sequence (which is now after the encoded chars)
        while (_sequence[p] != 1)
        {
            p++;
        }

        // Skip the 0x01 marker
        p++;

        // Copy parameter bytes into the buffer and zero them out
        var bufIdx = 0;
        do
        {
            var c = _sequence[p];
            buffer[bufIdx] = (char)c;
            _sequence[p] = 0;
            bufIdx++;
            p++;
        } while (buffer[bufIdx - 1] != 0 && _sequence[p] != 0xff);

        if (_sequence[p] == 0xff)
        {
            buffer[bufIdx] = '\0';
        }
    }
}
