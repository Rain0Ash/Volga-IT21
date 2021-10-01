// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Volga_IT.Extractor.Interfaces;
using Volga_IT.Helpers;

namespace Volga_IT.Extractor
{
    // ATTENTION!!!
    // Тут будут использоваться хаки и просто некрасивый код для посимвольной обработки html. Производительность была пожертвована в пользу эффективности работы с памятью.
    // Лучше всего было бы отменить ограничение по памяти и использовать отлаженную и оптимизированную сторонюю библиотеку, к примеру HtmlAgilityPack.
    // При помощи этой библиотеки можно сделать парсинг гораздо легче, отлаженнее и красивее.
    // А на любом современном компьютере есть достаточно ресурсов для обработки любого адекватного по размеру HTML. Если их нет - то почему мы пишем на C#, а не на C/ASM ? :)
    // P.S. тестировал на ваших примерах и некоторых рандомных простых HTML страницах (работает нормально, если результат не тот что ожидалось - сперва следует проверить HTML).
    // В очень сложных случаях не парсит (прим. новостная лента vk). Опять же - стоит использовать сторонюю библиотеку.
    // Могут быть неправильные определения с какими-нибудь комментариями посреди тегов (можно пофиксить усложнением логики состояний) или на других интересных случаях. Тогда см. выше.
    public class HtmlTextExtractor : IHtmlTextExtractor
    {
        protected Stream Stream { get; }
        protected IReadOnlySet<Char> Separators { get; }

        protected HtmlParserState State { get; set; }
        
        public Encoding? Encoding { get; init; }

        public HtmlTextExtractor(Stream stream)
            : this(stream, null)
        {
        }

        public HtmlTextExtractor(Stream stream, IReadOnlySet<Char>? separators)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream must be readable", nameof(stream));
            }

            Stream = stream;
            Separators = separators ?? HtmlHelper.DefaultSeparators;
        }

        // Обрабатываем комментарии ожидая выхода
        protected virtual Boolean CommentHandler(Char character, StringBuilder storage)
        {
            switch (character)
            {
                case '-':
                    if (storage.Length >= 1 && storage[^1] != '-') // проверяем последовательность
                    {
                        storage.Clear();
                    }
                        
                    storage.Append(character);
                    return true;
                case '>':
                    if (storage.Length >= 2 && storage[^2] == '-' && storage[^1] == '-') // также проверяем последовательность, если последовательность соответствует выходу из комментария - выходим.
                    {
                        State = HtmlParserState.Character;
                    }

                    storage.Clear();
                    return true;
                default:
                    return true;
            }
        }

        // Обрабатываем область внутри тегов <>. Если это комментарий - включаем режим комментариев.
        // Я пытался также обрабатывать <style> и <script> отдельно, но код при таком написании получается СЛИШКОМ запутанным и большим.
        protected virtual Boolean TagHandler(Char character, StringBuilder storage)
        {
            switch (character)
            {
                case '<':
                    State = HtmlParserState.Tag;
                    return true;
                case '>':
                    State = HtmlParserState.Character;
                    return true;
                case '!' when State == HtmlParserState.Tag:
                case '-' when State == HtmlParserState.Tag && storage.Length == 1 && storage[^1] == '!':
                    storage.Append(character);
                    return true;
                // Проверяем последовательность и переключаемся на режим комментариев
                case '-' when State == HtmlParserState.Tag && storage.Length == 2 && storage[^2] == '!' && storage[^1] == '-':
                    State = HtmlParserState.Comment;
                    storage.Clear();
                    return true;
                default:
                    return true;
            }
        }
        
        // Обрабатываем HTML сущности. Сущность начинается с символа & и заканчивается символом ;
        // ReSharper disable once CognitiveComplexity
        protected virtual String? EntityHandler(Char character, StringBuilder storage)
        {
            switch (character)
            {
                case '&': // Начало сущности
                    State = HtmlParserState.Entity;
                    storage.Append(character);
                    return null;
                case ';' when State == HtmlParserState.Entity:
                    Int32 index = storage.LastIndexOf('&'); // Находим индекс последнего встреченного &

                    // Если символ не найден или длина последовательности больше максимального размера HTML сущности - выходим, тем самым внося последовательность в слово.
                    if (index < 0 || storage.Length - index > HtmlHelper.LongestHtmlEntityValueLength)
                    {
                        State = HtmlParserState.Character;
                        return null;
                    }
                    
                    StringBuilder entity = new StringBuilder(HtmlHelper.LongestHtmlEntityValueLength, HtmlHelper.LongestHtmlEntityValueLength);

                    for (Int32 i = index + 1; i < storage.Length; i++) //заполняем stringbuilder символами entity
                    {
                        entity.Append(storage[i]);
                    }

                    // Получаем символ из словаря и возвращаем его для дальнейших проверок.
                    if (HtmlHelper.HtmlEntityValues.TryGetValue(entity.ToString(), out UInt16 result))
                    {
                        State = HtmlParserState.Character;
                        storage.Remove(index, storage.Length - index); // Очищаем кусок с сущностью
                        return ((Char) result).ToString();
                    }
                    
                    State = HtmlParserState.Character;
                    return character.ToString();
                default:
                    //Сущности состоят из [a-zA-Z]+. При желании можно чуть изменить проверку.
                    if (State == HtmlParserState.Entity && Char.IsLetter(character))
                    {
                        storage.Append(character);
                        return null;
                    }

                    State = HtmlParserState.Character;
                    return character.ToString();
            }
        }
        
        // Обрабатываем каждый символ
        // ReSharper disable once CognitiveComplexity
        protected virtual String? CharacterHandler(Char character, StringBuilder storage)
        {
            if (character == '<')
            {
                if (TagHandler(character, storage))
                {
                    return null;
                }
            }

            // Если режим сущности или символ начала сущности - обрабатываем случаи через специализированный обработчик
            if (State == HtmlParserState.Entity || character == '&')
            {
                String? entity = EntityHandler(character, storage);

                if (String.IsNullOrEmpty(entity))
                {
                    return null;
                }
                
                if (entity.Length > 1)
                {
                    storage.Append(character);
                    return null;
                }
                
                character = entity[0];
            }
            
            // Проверяем, является ли символ разделителем. Если да - возвращаем слово и начинаем сначала. Иначе - продолжаем.
            if (Separators.Contains(character))
            {
                String? result = null;
                
                if (storage.Length > 0)
                {
                    result = storage.ToString();
                }
                    
                storage.Clear();
                return result;
            }

            storage.Append(character);
            return null;
        }

        protected virtual String? ExtractInternal(Char character, StringBuilder storage)
        {
            if (State == HtmlParserState.Comment && CommentHandler(character, storage))
            {
                return null;
            }

            if ((State == HtmlParserState.Tag || character == '<') && TagHandler(character, storage))
            {
                return null;
            }

            return CharacterHandler(character, storage);
        }
        
        protected virtual IEnumerable<String> Extract()
        {
            StringBuilder storage = new StringBuilder();
            
            // Читаем и обрабатываем поток посимвольно (с проверкой на кодировку). Размер занимаемый хранилищем не будет занимать сильно больше чем размер максимального слова.
            // (Нужно учитывать аллокацию внутренних буфферов и прочее).
            foreach (Char character in Stream.ReadCharSequence(Encoding))
            {
                String? result = ExtractInternal(character, storage);
                if (String.IsNullOrEmpty(result))
                {
                    continue;
                }

                yield return result;
            }
            
            if (storage.Length > 0)
            {
                yield return storage.ToString();
            }
        }

        public IEnumerator<String> GetEnumerator()
        {
            return Extract().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}