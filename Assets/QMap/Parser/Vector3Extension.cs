using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QMap
{
    public static class Vector3Extension
    {
        public static Vector3 FromToken(Tokenizer tokenizer)
        {
            var token = tokenizer.GetNextToken();
            if (token.Type != Tokenizer.TokenType.StartParen)
            {
                throw new FormatException("Expected an open paren, received a " + token);
            }

            var x = Convert.ToSingle(tokenizer.GetNextValue());
            var y = Convert.ToSingle(tokenizer.GetNextValue());
            var z = Convert.ToSingle(tokenizer.GetNextValue());
            token = tokenizer.GetNextToken();
            if (token.Type != Tokenizer.TokenType.EndParen)
            {
                throw new FormatException("Expected an close paren, received a " + token);
            }

            return new Vector3(x, y, z);
        }
    }
}
