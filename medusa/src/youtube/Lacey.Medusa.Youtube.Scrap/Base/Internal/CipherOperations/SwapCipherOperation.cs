﻿using System.Text;

namespace Lacey.Medusa.Youtube.Scrap.Base.Internal.CipherOperations
{
    public class SwapCipherOperation : ICipherOperation
    {
        private readonly int _index;

        public SwapCipherOperation(int index)
        {
            _index = index;
        }

        public string Decipher(string input)
        {
            var sb = new StringBuilder(input)
            {
                [0] = input[_index],
                [_index] = input[0]
            };
            return sb.ToString();
        }
    }
}