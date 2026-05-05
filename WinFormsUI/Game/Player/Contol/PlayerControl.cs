using XEngine.Core.Input;

namespace WinFormsUI.Game.Player.Contol
{
    public partial class PlayerControl(IInputService input)
    {
        public string Name = "";

        public float HorizotnalInput()
        {
            return input.GetAxis($"Horizontal{Name}");
        }

        public float VerticalInput()
        {
            return input.GetAxis($"Vertical{Name}");
        }

        public bool Fetch(string action, ActionType type)
        {
            return type switch
            {
                ActionType.ActionStart => input.IsActionJustPressed(action + Name),
                ActionType.ActionActive => input.IsActionActive(action + Name),
                ActionType.ActionEnd => input.IsActionJustReleased(action + Name),
                ActionType.ActionInactive => input.IsActionInactive(action + Name),
                _ => false
            };
        }
    }
}
