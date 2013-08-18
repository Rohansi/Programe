using System;

namespace Programe.Machine
{
    public class VirtualMachine
    {
        [Flags]
        private enum Flags : short
        {
            Zero = 1 << 0,
            Equal = 1 << 1,
            Above = 1 << 2,
            Below = 1 << 3,

            None = 0
        }

        private static readonly Random Random = new Random();

        public readonly short[] Registers;
        public short IP;
        public short SP;
        public readonly short[] Memory;

        private Flags flags;
        private Instruction instruction;
        private bool interrupted;
        private short ivt;
        private Device[] devices;
        private short errorIP;

        public VirtualMachine()
        {
            Registers = new short[16];
            Memory = new short[short.MaxValue];

            IP = 0;
            SP = (short)(Memory.Length - 1);

            flags = Flags.None;
            instruction = new Instruction(this);
            interrupted = false;
            ivt = 0;
            devices = new Device[32];
        }

        public void Attach(Device device)
        {
            if (devices[device.Id] != null)
                throw new Exception("Duplicate Device ID");

            devices[device.Id] = device;
        }

        public void Step()
        {
            errorIP = IP;

            try
            {
                if (ivt != 0 && !interrupted)
                {
                    for (var i = 0; i < devices.Length; i++)
                    {
                        var device = devices[i];
                        if (device == null || !device.InterruptRequest)
                            continue;

                        Interrupt(device.Id);
                        device.HandleInterruptRequest(this);
                        break;
                    }
                }

                instruction.Decode();

                short result;
                switch (instruction.Opcode)
                {
                    case Opcode.Mov:
                        instruction.Left.Set(instruction.Right.Get());
                        break;
                    case Opcode.Add:
                        result = (short)(instruction.Left.Get() + instruction.Right.Get());
                        instruction.Left.Set(result);
                        SetZero(result);
                        break;
                    case Opcode.Sub:
                        result = (short)(instruction.Left.Get() - instruction.Right.Get());
                        instruction.Left.Set(result);
                        SetZero(result);
                        break;
                    case Opcode.Mul:
                        result = (short)(instruction.Left.Get() * instruction.Right.Get());
                        instruction.Left.Set(result);
                        SetZero(result);
                        break;
                    case Opcode.Div:
                        result = (short)(instruction.Left.Get() / instruction.Right.Get());
                        instruction.Left.Set(result);
                        SetZero(result);
                        break;
                    case Opcode.Rem:
                        result = (short)(instruction.Left.Get() % instruction.Right.Get());
                        instruction.Left.Set(result);
                        SetZero(result);
                        break;
                    case Opcode.Inc:
                        result = (short)(instruction.Left.Get() + 1);
                        instruction.Left.Set(result);
                        SetZero(result);
                        break;
                    case Opcode.Dec:
                        result = (short)(instruction.Left.Get() - 1);
                        instruction.Left.Set(result);
                        SetZero(result);
                        break;
                    case Opcode.Not:
                        result = (short)(~instruction.Left.Get());
                        instruction.Left.Set(result);
                        SetZero(result);
                        break;
                    case Opcode.And:
                        result = (short)(instruction.Left.Get() & instruction.Right.Get());
                        instruction.Left.Set(result);
                        SetZero(result);
                        break;
                    case Opcode.Or:
                        result = (short)(instruction.Left.Get() | instruction.Right.Get());
                        instruction.Left.Set(result);
                        SetZero(result);
                        break;
                    case Opcode.Xor:
                        result = (short)(instruction.Left.Get() ^ instruction.Right.Get());
                        instruction.Left.Set(result);
                        SetZero(result);
                        break;
                    case Opcode.Shl:
                        result = (short)(instruction.Left.Get() << instruction.Right.Get());
                        instruction.Left.Set(result);
                        SetZero(result);
                        break;
                    case Opcode.Shr:
                        result = (short)(instruction.Left.Get() >> instruction.Right.Get());
                        instruction.Left.Set(result);
                        SetZero(result);
                        break;
                    case Opcode.Push:
                        Push(instruction.Left.Get());
                        break;
                    case Opcode.Pop:
                        instruction.Left.Set(Pop());
                        break;
                    case Opcode.Jmp:
                        IP = instruction.Left.Get();
                        break;
                    case Opcode.Call:
                        Push(IP);
                        IP = instruction.Left.Get();
                        break;
                    case Opcode.Ret:
                        IP = Pop();
                        break;
                    case Opcode.Cmp:
                        var cmpValL = instruction.Left.Get();
                        var cmpValR = instruction.Right.Get();

                        flags = Flags.None;
                        if (cmpValL == 0)
                            flags |= Flags.Zero;
                        if (cmpValL == cmpValR)
                            flags |= Flags.Equal;
                        if (cmpValL > cmpValR)
                            flags |= Flags.Above;
                        if (cmpValL < cmpValR)
                            flags |= Flags.Below;
                        break;
                    case Opcode.Jz:
                        if ((flags & Flags.Zero) != 0)
                            IP = instruction.Left.Get();
                        break;
                    case Opcode.Jnz:
                        if ((flags & Flags.Zero) == 0)
                            IP = instruction.Left.Get();
                        break;
                    case Opcode.Je:
                        if ((flags & Flags.Equal) != 0)
                            IP = instruction.Left.Get();
                        break;
                    case Opcode.Jne:
                        if ((flags & Flags.Equal) == 0)
                            IP = instruction.Left.Get();
                        break;
                    case Opcode.Ja:
                        if ((flags & Flags.Above) != 0)
                            IP = instruction.Left.Get();
                        break;
                    case Opcode.Jae:
                        if ((flags & Flags.Above) != 0 || (flags & Flags.Equal) != 0)
                            IP = instruction.Left.Get();
                        break;
                    case Opcode.Jb:
                        if ((flags & Flags.Below) != 0)
                            IP = instruction.Left.Get();
                        break;
                    case Opcode.Jbe:
                        if ((flags & Flags.Below) != 0 || (flags & Flags.Equal) != 0)
                            IP = instruction.Left.Get();
                        break;
                    case Opcode.Rand:
                        instruction.Left.Set((short)Random.Next(short.MinValue, short.MaxValue));
                        break;
                    case Opcode.Int:
                        var intVal = instruction.Left.Get();
                        if (intVal < 0 || intVal >= devices.Length || devices[intVal] == null)
                            break;
                        devices[intVal].HandleInterrupt(this);
                        break;
                    case Opcode.Iret:
                        InterruptReturn();
                        break;
                    case Opcode.Ivt:
                        ivt = instruction.Left.Get();
                        break;
                    case Opcode.Abs:
                        result = Math.Abs(instruction.Left.Get());
                        instruction.Left.Set(result);
                        SetZero(result);
                        break;
                    default:
                        throw new VirtualMachineException(errorIP, "Bad opcode");
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new VirtualMachineException(errorIP, "Out of memory bounds", e);
            }
            catch (DivideByZeroException e)
            {
                throw new VirtualMachineException(errorIP, "Divide by zero", e);
            }
            catch (VirtualMachineException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new VirtualMachineException(errorIP, "Error: " + e, e);
            }
        }

        public void Interrupt(byte index)
        {
            Push(IP);
            Push((short)flags);

            foreach (var r in Registers)
            {
                Push(r);
            }

            IP = Memory[ivt + index];

            interrupted = true;
        }

        private void InterruptReturn()
        {
            for (var i = Registers.Length - 1; i >= 0; i--)
            {
                Registers[i] = Pop();
            }

            flags = (Flags)Pop();
            IP = Pop();

            interrupted = false;
        }

        private void Push(short value)
        {
            Memory[SP--] = value;
        }

        private short Pop()
        {
            return Memory[++SP];
        }

        private void SetZero(short value)
        {
            flags &= ~Flags.Zero;
            if (value == 0)
                flags |= Flags.Zero;
        }
    }
}
