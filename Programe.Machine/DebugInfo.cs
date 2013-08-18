using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Programe.Machine
{
    public class DebugInfo
    {
        public class LineAddress
        {
            public readonly int Line;
            public readonly int Address;

            public LineAddress(int line, int address)
            {
                Line = line;
                Address = address;
            }
        }

        public class Symbol
        {
            public readonly string Name;
            public readonly int Address;

            public Symbol(string name, int address)
            {
                Name = name;
                Address = address;
            }
        }

        private const int Header = 147483647; // ty krix

        private List<LineAddress> lineAddresses;
        private List<Symbol> symbols;

        /// <summary>
        /// Create a new DebugInfo file.
        /// </summary>
        public DebugInfo()
        {
            lineAddresses = new List<LineAddress>();
            symbols = new List<Symbol>();
        }

        /// <summary>
        /// Search for the line containing an address.
        /// </summary>
        public bool FindLine(int address, out LineAddress result)
        {
            foreach (var lineAddress in lineAddresses)
            {
                if (address >= lineAddress.Address)
                {
                    result = lineAddress;
                    return true;
                }
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Search for the symbol at an address.
        /// </summary>
        public bool FindSymbol(int address, out Symbol result)
        {
            foreach (var symbol in symbols)
            {
                if (address >= symbol.Address)
                {
                    result = symbol;
                    return true;
                }
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Save DebugInfo to a file.
        /// </summary>
        public void Save(string filename)
        {
            lineAddresses = lineAddresses.OrderBy(i => i.Address).ToList();

            using (var file = File.Open(filename, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(file))
            {
                writer.Write(Header);

                writer.Write(lineAddresses.Count);
                foreach (var lineAddress in lineAddresses)
                {
                    writer.Write(lineAddress.Line);
                    writer.Write(lineAddress.Address);
                }

                writer.Write(symbols.Count);
                foreach (var symbol in symbols)
                {
                    writer.Write(symbol.Name);
                    writer.Write(symbol.Address);
                }
            }
        }

        /// <summary>
        /// Load DebugInfo from a file.
        /// </summary>
        public static DebugInfo Load(string filename)
        {
            var db = new DebugInfo();

            using (var file = File.Open(filename, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(file))
            {
                if (reader.ReadInt32() != Header)
                    throw new Exception("Bad header");

                var lineAddressCount = reader.ReadInt32();
                for (var i = 0; i < lineAddressCount; i++)
                {
                    var line = reader.ReadInt32();
                    var address = reader.ReadInt32();
                    db.lineAddresses.Add(new LineAddress(line, address));
                }

                var symbolCount = reader.ReadInt32();
                for (var i = 0; i < symbolCount; i++)
                {
                    var name = reader.ReadString();
                    var address = reader.ReadInt32();
                    db.symbols.Add(new Symbol(name, address));
                }
            }

            return db;
        }

        public void AddLineAddress(int line, int address)
        {
            lineAddresses.Add(new LineAddress(line, address));
        }

        public void AddSymbol(string name, int address)
        {
            symbols.Add(new Symbol(name, address));
        }
    }
}
