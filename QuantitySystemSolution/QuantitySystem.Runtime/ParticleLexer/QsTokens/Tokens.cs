﻿using ParticleLexer;
using ParticleLexer.StandardTokens;
using System;
using System.Linq;

namespace ParticleConsole.QsTokens
{
    /// <summary>
    /// || Groups
    /// </summary>
    public class MatrixGroupToken : TokenClass
    {
    }

    [TokenPattern(RegexPattern = "|")]
    public class MatrixBracketToken : TokenClass
    {
    }


    /// <summary>
    /// Unitized number is
    /// 90, 90.9
    /// 90.9e2, 90.9e+2, 90.9e-2
    /// 90.09&lt;m&gt;, 90.2e+2&lt;m&gt;, etc.
    /// </summary>
    [TokenPattern(
        RegexPattern = @"\d+(\.|\.\d+)?([eE][-+]?\d+)?"                       //floating number
        + "\\s*<(°?\\w+!?(\\^\\d+)?\\.?)+(/(°?\\w+!?(\\^\\d+)?\\.?)+)?>"          // unit itself.
        )
    ]
    public class UnitizedNumberToken : TokenClass
    {

    }



    #region Sequence tokens
    [TokenPattern(RegexPattern = "..>")]
    public class PositiveSequenceToken : TokenClass
    {
    }

    [TokenPattern(RegexPattern = "<..")]
    public class NegativeSequenceToken : TokenClass
    {
    }

    /// <summary>
    /// a=S[u SequenceRangeToken ]   like  a=S[2++5]
    /// The operator range that appears in the sequence calling square brackets n..m
    /// where .. is
    ///         ++ Series:                  Sum elements                    returns Scalar
    ///         ** Product:                 Multiply elements               returns Scalar
    ///         !! Average:                 Get the Mean of elements.       returns Scalar.
    ///         ## Standard Deviation       
    ///         .. Range                                                    returns Vector if components are scalars, Matrix if components are vectors
    /// </summary>
    [TokenPattern(RegexPattern = "((\\+\\+)?|(!!)?|(\\*\\*)?|(\\#\\#)?|(\\.\\.)?)")]
    public class SequenceRangeToken : TokenClass
    {
    }

    /// <summary>
    /// fifo[i1, i2, ..., in](p1, p2, ..., p3)    whole token
    /// </summary>
    public class SequenceCallToken : TokenClass
    {
    }

    /// <summary>
    /// [i1, i2, sequenceIndexToken, ..., in]  i(s) inside parenthesis
    /// </summary>
    public class SequenceIndexToken : TokenClass
    {
    }

    /// <summary>
    /// S[] ..> elementToken; elementToken; elementToken.
    /// </summary>
    public class SequenceElementToken : TokenClass
    {
    }

    #endregion


    /// <summary>
    /// Reference token on the form WordToken, ColonToken  x:  oh:
    /// </summary>
    [TokenPattern(RegexPattern = @"\w+:")]
    public class NameSpaceToken : TokenClass
    {
    }

    /// <summary>
    /// Reference Namespace with its value  x:r   x:u  xd:Abs  etc...
    /// </summary>
    [TokenPattern(RegexPattern = @"\w+:\w+")]
    public class NameSpaceAndValueToken : TokenClass
    {
    }


    /// <summary>
    /// | x |
    /// </summary>
    [TokenPattern(RegexPattern = @"\|[^\|]+?\|")]
    public class AbsoluteToken : TokenClass
    {
    }

    /// <summary>
    /// || x ||
    /// </summary>
    [TokenPattern(RegexPattern = @"\|\|[^\|]+\|\|")]
    public class MagnitudeToken : TokenClass
    {
    }

    [TokenPattern(RegexPattern = "when")]
    public class WhenStatementToken : TokenClass
    {
    }

    [TokenPattern(RegexPattern = "otherwise")]
    public class OtherwiseStatementToken : TokenClass
    {
    }

    [TokenPattern(RegexPattern = @"\.")]
    public class DotMultiplyToken : TokenClass
    {
    }

    [TokenPattern(RegexPattern = @"\x")]
    public class CrossMultiplyToken : TokenClass
    {
    }



    [TokenPattern(RegexPattern = @"\^\.")]
    public class PowerDotToken : TokenClass
    {
    }

    [TokenPattern(RegexPattern = @"\^x")]
    public class PowerCrossToken : TokenClass
    {
    }



    [TokenPattern(RegexPattern = "!=")]
    public class NotEqualToken : TokenClass
    {
    }

    [TokenPattern(RegexPattern = "==")]
    public class EqualityToken : TokenClass
    {
    }

    [TokenPattern(RegexPattern = "and")]
    public class AndStatementToken : TokenClass
    {
    }

    [TokenPattern(RegexPattern = "or")]
    public class OrStatementToken : TokenClass
    {
    }

    public static class TokenExtensions
    {
        //public static Token GroupBrackets(this Token token)
        //{
        //    var t = token.MergeTokensInGroups(new ParenthesisGroupToken(), new SquareBracketsGroupToken());

        //    return t;
        //}



        /// <summary>
        /// Should be used after merging by word
        /// and grouping brackets
        /// The function tries to find Word followed by complete parenthesis group or square brackets group.
        /// And can parse this example:
        ///            fn(434,fn((434+434)+8/4,50,fifo(5)))
        ///            
        ///            S[5,F[4,2,1],G](4, 3, 2,R[2], p(30*o(9)))
        ///            -              - Parameters              -
        ///            -    Indexes   -
        ///            -              Sequence Call             -
        /// </summary>
        /// <param name="ignoreWords">list of words that should be ignored when discovering calls </param>
        /// <returns></returns>
        public static Token DiscoverQsCalls(this Token token, StringComparer stringComparer, params string[] ignoreWords)
        {
            Token first = new Token();
            Token current = first;

            int ci = 0;

            while (ci < token.Count)
            {
                var c = token[ci];

                if (c.Contains(typeof(ParenthesisGroupToken)) | c.Contains(typeof(SquareBracketsGroupToken)))
                {
                    //recursive call if the token have inside groups
                    current.AppendSubToken(c.DiscoverQsCalls(stringComparer, ignoreWords));
                }
                else
                {
                    //sub groups then test this token 
                    if (
                        (
                              c.TokenClassType == typeof(WordToken)               // word token
                           || c.TokenClassType == typeof(NameSpaceAndValueToken)  // or namespace:value token
                        )
                        && ignoreWords.Contains(c.TokenValue, stringComparer) == false         // and the whole value is not in  the ignore words
                        )
                    {
                        //check if the next token is group
                        if (ci < token.Count - 1)
                        {
                            Token cnext = token[ci + 1];

                            #region Parenthesis group discovery
                            if (cnext.TokenClassType == typeof(ParenthesisGroupToken))
                            {
                                // so this is a function
                                //take the current token with the next token and make it as functionToken

                                Token functionCallToken = new Token();
                                functionCallToken.TokenClassType = typeof(ParenthesisCallToken);
                                functionCallToken.AppendSubToken(c);



                                if (cnext.Contains((typeof(ParenthesisGroupToken))) | cnext.Contains(typeof(SquareBracketsGroupToken)))
                                {
                                    cnext = cnext.DiscoverQsCalls(stringComparer, ignoreWords);
                                }

                                
                                cnext = token.SplitParamerers(cnext, new CommaToken());


                                functionCallToken.AppendSubToken(cnext);

                                current.AppendSubToken(functionCallToken);

                                ci += 2;
                                continue;
                            }
                            #endregion

                            #region Square Brackets discovery
                            if (cnext.TokenClassType == typeof(SquareBracketsGroupToken))
                            {
                                // so this is a sequence
                                //take the current token with the next token and make it as sequenceToken

                                Token sequenceCallToken = new Token();
                                sequenceCallToken.TokenClassType = typeof(SequenceCallToken);
                                sequenceCallToken.AppendSubToken(c);

                                if (cnext.Contains((typeof(SquareBracketsGroupToken))) | cnext.Contains((typeof(ParenthesisGroupToken))))
                                {
                                    cnext = cnext.DiscoverQsCalls(stringComparer, ignoreWords);
                                }

                                cnext = token.SplitParamerers(cnext, new CommaToken());

                                sequenceCallToken.AppendSubToken(cnext);

                                if (token.Count > ci + 2)
                                {
                                    //check if we have a Parenthesis parameters after Square Brackets
                                    Token cnextnext = token[ci + 2];
                                    if (cnextnext.TokenClassType == typeof(ParenthesisGroupToken))
                                    {
                                        //then this is a sequence call with parameters.
                                        if ((cnextnext.Contains((typeof(SquareBracketsGroupToken))) | cnextnext.Contains((typeof(ParenthesisGroupToken)))))
                                        {
                                            cnextnext = cnextnext.DiscoverQsCalls(stringComparer, ignoreWords);
                                        }

                                        cnextnext = token.SplitParamerers(cnextnext, new CommaToken());

                                        sequenceCallToken.AppendSubToken(cnextnext);

                                        ci += 3;
                                    }
                                    else
                                    {
                                        ci += 2;
                                    }
                                }
                                else
                                {
                                    ci += 2;
                                }
                                current.AppendSubToken(sequenceCallToken);
                                continue;
                            }

                            #endregion

                        }
                    }

                    // if all conditions failed we put the token and resume to another one
                    current.AppendSubToken(token[ci]);
                }

                ci++;
            }
            first.TokenClassType = token.TokenClassType;

            return Token.Zabbat(first);
        }




        /// <summary>
        /// returns the value of tokens starting from specific token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static string SubTokensValue(this Token token, int startIndex)
        {
            int idx = startIndex;
            string total = string.Empty;
            while (idx < token.Count)
            {
                total += token[idx].TokenValue;
                idx++;
            }
            return total;

        }
    
    }

}