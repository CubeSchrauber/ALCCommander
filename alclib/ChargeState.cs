using System;

namespace alclib
{
    public class ChargeState : Initializable
    {
        public int Channel;
        public int State;

        public override void Init(Buffer data, ref int pos)
        {
            if (data[pos++] != 'a')
                throw new ArgumentException();
            Channel = data[pos++];
            State = data[pos++];
        }

        public string Name
        {
            get
            {
                string result;
                if (State < 0x0b)
                    result = $"Idle ({State:x02})";
                else if (State < 0x2e)
                    result = $"Pause/Wait ({State:x02})";
                else if (State < 0x38)
                    result = $"Discharging ({State:x02})";
                else if (State < 0x6F)
                    result = $"Charging ({State:x02})";
                else if (State < 0xA1)
                    result = $"Trickle Charging ({State:x02})";
                else if (State < 0xC9)
                    result = $"Discharge Finished ({State:x02})";
                else
                    result = $"Emergency Stop ({State:x02})";
                return result;
            }
        }

        public int Value
        {
            get
            {
                if (State < 0x0b) return 0; // Idle
                else if (State < 0x2e) return 1; // Pause
                else if (State < 0x38) return 2; // Discharging
                else if (State < 0x6F) return 3; // Charging
                else if (State < 0xA1) return 4; // Trickle Charging
                else if (State < 0xC9) return 5; // Discharge Finished
                else return 6; // Emergency Stop
            }
        }
    }

}