using System.Windows.Forms;

namespace MyAutoBadge.Services;

public class SessionLockService
{
    public bool IsLocked()
    {
        return Cursor.Position.X == 0 && Cursor.Position.Y == 0;
    }
}