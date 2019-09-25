namespace emu8080.Core
{
    public class Bus{
        public void Interrupt(bool value)
        {
            this.InterruptEnabled = value;
            InterruptChanged?.Invoke(value);
        }

        public delegate void InterruptChangedHandler(bool value);
        public event InterruptChangedHandler InterruptChanged;

        public bool InterruptEnabled { get; private set; }
    }
}