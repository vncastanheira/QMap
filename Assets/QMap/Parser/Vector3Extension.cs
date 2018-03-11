using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QMap
{
    public static class Vector3Extension
    {
        public static void FromToken(this Vector3 vector, Tokenizer tokenizer)
        {
            var token = tokenizer.GetNextToken();
            if (token.Type != Tokenizer.TokenType.StartParen)
            {
                throw new FormatException("Expected an open paren, received a " + token);
            }
            vector.x = Convert.ToSingle(tokenizer.GetNextValue());
            vector.y = Convert.ToSingle(tokenizer.GetNextValue());
            vector.z = Convert.ToSingle(tokenizer.GetNextValue());
            token = tokenizer.GetNextToken();
            if (token.Type != Tokenizer.TokenType.EndParen)
            {
                throw new FormatException("Expected an close paren, received a " + token);
            }
        }
    }
}
