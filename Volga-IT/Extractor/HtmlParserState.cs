// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

namespace Volga_IT.Extractor
{
    public enum HtmlParserState
    {
        Character,
        Entity,
        Tag,
        Comment
    }
}