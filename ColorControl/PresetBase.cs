using System;
using System.Collections.Generic;

namespace ColorControl
{
    internal abstract class PresetBase
    {
        public string shortcut { get; set; }

        private int _id;

        public int id
        {
            get
            {
                return _id;
            }
            set
            {
                if (value == 0 || ids.Contains(value))
                {
                    value = GetNewId();
                }
                else
                {
                    ids.Add(value);
                }
                _id = value;
            }
        }

        public static List<int> ids = new List<int>();

        public static int GetNewId()
        {
            int id;
            do
            {
                id = new Random().Next();
            }
            while (ids.Contains(id));

            ids.Add(id);

            return id;
        }

        public abstract List<string> GetDisplayValues(Config config = null);
    }
}