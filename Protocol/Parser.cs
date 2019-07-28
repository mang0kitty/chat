
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol
{
    public class Parser
    {
        public string Generate(string type, IEnumerable<string> parts) {
            var serializedParts = string.Join(" ", parts.Select(s => {
                return s.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\"", "\\\"");
            }).Select(s => {
                if (s.Contains(' ')) return $"\"{s}\"";
                return s;
            }));

            return $"{type} {serializedParts}".Trim();
        }

        public ParseResult Parse(string payload) {
            var type = ParseType(ref payload);
            Munch(ref payload);
            var parts = ParseParts(ref payload);

            return new ParseResult(type, parts);
        }

        private string ParseType(ref string payload) {
            return ReadUntil(ref payload);
        }

        private string[] ParseParts(ref string payload) {
            var parts = new List<string>();
            while (payload.Any()) {
                var part = ParsePart(ref payload);
                Munch(ref payload);
                parts.Add(part);
            }

            return parts.ToArray();
        }

        private string ParsePart(ref string payload) {
            if(payload.First() == '"') {
                payload = payload.Substring(1);

                // You can also use a try {} finally {} block to do this "post-return" cleanup
                var part = ReadUntil(ref payload,'"');
                payload = payload.Substring(1);
                return part;
            }
            return ReadUntil(ref payload);
        }

        // ReadUntil(" abc", ' ')

        private string ReadUntil(ref string payload, char endChar = ' ') {
            var output = new StringBuilder();

            while(payload.Any() && payload.First() != endChar) {
                if (payload.First() == '\\') {
                    // Decide what to do with this:
                    switch (payload.Skip(1).FirstOrDefault()) {
                        case 'n':
                            output.Append("\n");
                            break;
                        case '\\':
                            output.Append("\\");
                            break;
                        case '"':
                            output.Append('"');
                            break;
                        default:
                            throw new InvalidOperationException($"Unrecognized escape sequence '{payload.Substring(0, 2)}'");
                    }

                    payload = payload.Substring(2);
                    continue;
                }

                output.Append(payload.First());
                payload = payload.Substring(1);
            }

            return output.ToString();
        }

        private void Munch(ref string payload, char c = ' ') {
            while (payload.FirstOrDefault() == c)
                payload = payload.Substring(1);
        }
    }

    public class ParseResult {
        public ParseResult(string type, string[] parts) {
            this.Type = type;
            this.Parts = parts;
        }

        public string Type { get; private set; }

        public string[] Parts { get; private set; }
    }
}
