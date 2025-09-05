using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace SemanticKernelPlugins
{
    /// <summary>
    /// Plugin for text processing operations
    /// </summary>
    public class TextPlugin
    {
        [KernelFunction("analyze_text")]
        [Description("Analyzes text and provides statistics like word count, character count, and basic sentiment")]
        public async Task<string> AnalyzeTextAsync(
            [Description("Text to analyze")] string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "No text provided to analyze.";
            }

            // Clean text for analysis
            var cleanText = text.Trim();
            var words = cleanText.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var sentences = Regex.Split(cleanText, @"[.!?]+").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            var paragraphs = cleanText.Split(new string[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            // Basic sentiment analysis
            var positiveWords = new[] { "good", "great", "excellent", "amazing", "wonderful", "love", "like", "happy", "fantastic", "awesome", "brilliant", "perfect" };
            var negativeWords = new[] { "bad", "terrible", "awful", "hate", "horrible", "sad", "angry", "disappointed", "worst", "ugly", "stupid" };

            var positiveCount = positiveWords.Count(word => cleanText.ToLower().Contains(word.ToLower()));
            var negativeCount = negativeWords.Count(word => cleanText.ToLower().Contains(word.ToLower()));

            var sentiment = positiveCount > negativeCount ? "Positive" : 
                           negativeCount > positiveCount ? "Negative" : "Neutral";

            var analysis = $"📊 Text Analysis Results:\n" +
                          $"   • Words: {words.Length}\n" +
                          $"   • Characters: {cleanText.Length}\n" +
                          $"   • Characters (no spaces): {cleanText.Replace(" ", "").Length}\n" +
                          $"   • Sentences: {sentences.Length}\n" +
                          $"   • Paragraphs: {paragraphs.Length}\n" +
                          $"   • Basic Sentiment: {sentiment}\n" +
                          $"   • Average words per sentence: {(sentences.Length > 0 ? Math.Round((double)words.Length / sentences.Length, 1) : 0)}";

            Console.WriteLine($"📝 Text Plugin: Analyzed {words.Length} words");
            return await Task.FromResult(analysis);
        }

        [KernelFunction("extract_keywords")]
        [Description("Extracts key words from text by removing common stop words and showing frequency")]
        public async Task<string> ExtractKeywordsAsync(
            [Description("Text to extract keywords from")] string text,
            [Description("Maximum number of keywords to return")] int maxKeywords = 10)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "No text provided for keyword extraction.";
            }

            var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "the", "a", "an", "and", "or", "but", "in", "on", "at", "to", "for", "of", "with", "by",
                "is", "are", "was", "were", "be", "been", "being", "have", "has", "had", "do", "does", "did",
                "will", "would", "could", "should", "may", "might", "must", "can", "this", "that", "these",
                "those", "i", "you", "he", "she", "it", "we", "they", "me", "him", "her", "us", "them",
                "my", "your", "his", "her", "its", "our", "their", "myself", "yourself", "himself", "herself",
                "itself", "ourselves", "yourselves", "themselves", "what", "which", "who", "when", "where",
                "why", "how", "all", "any", "both", "each", "few", "more", "most", "other", "some", "such",
                "no", "nor", "not", "only", "own", "same", "so", "than", "too", "very", "just", "now"
            };

            // Extract words and filter
            var words = Regex.Matches(text.ToLower(), @"\b[a-zA-Z]{3,}\b")
                             .Cast<Match>()
                             .Select(m => m.Value)
                             .Where(word => !stopWords.Contains(word))
                             .ToList();

            var keywordFrequency = words
                .GroupBy(word => word)
                .OrderByDescending(group => group.Count())
                .Take(maxKeywords)
                .Select(group => $"{group.Key} ({group.Count()})")
                .ToList();

            var result = keywordFrequency.Any()
                ? $"🔍 Top {Math.Min(maxKeywords, keywordFrequency.Count)} Keywords:\n   • {string.Join("\n   • ", keywordFrequency)}"
                : "No significant keywords found.";

            Console.WriteLine($"📝 Text Plugin: Extracted {keywordFrequency.Count} keywords");
            return await Task.FromResult(result);
        }

        [KernelFunction("transform_case")]
        [Description("Transforms text to different cases: upper, lower, title, sentence, or alternating")]
        public async Task<string> TransformCaseAsync(
            [Description("Text to transform")] string text,
            [Description("Transformation type: upper, lower, title, sentence, alternating")] string caseType = "upper")
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "No text provided for transformation.";
            }

            string result = caseType.ToLower() switch
            {
                "upper" => text.ToUpper(),
                "lower" => text.ToLower(),
                "title" => System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower()),
                "sentence" => char.ToUpper(text[0]) + text.Substring(1).ToLower(),
                "alternating" => new string(text.Select((c, i) => i % 2 == 0 ? char.ToUpper(c) : char.ToLower(c)).ToArray()),
                _ => text
            };

            Console.WriteLine($"📝 Text Plugin: Transformed text to {caseType} case");
            return await Task.FromResult($"Transformed to {caseType} case: {result}");
        }

        [KernelFunction("count_occurrences")]
        [Description("Counts how many times a word or phrase appears in text")]
        public async Task<string> CountOccurrencesAsync(
            [Description("Text to search in")] string text,
            [Description("Word or phrase to count")] string searchTerm,
            [Description("Whether the search should be case sensitive")] bool caseSensitive = false)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(searchTerm))
            {
                return "Both text and search term must be provided.";
            }

            var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            var count = 0;
            var index = 0;

            while ((index = text.IndexOf(searchTerm, index, comparison)) != -1)
            {
                count++;
                index += searchTerm.Length;
            }

            var result = $"The term '{searchTerm}' appears {count} time(s) in the text.";
            Console.WriteLine($"📝 Text Plugin: {result}");
            return await Task.FromResult(result);
        }

        [KernelFunction("extract_emails")]
        [Description("Extracts email addresses from text")]
        public async Task<string> ExtractEmailsAsync(
            [Description("Text to extract emails from")] string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "No text provided for email extraction.";
            }

            var emailPattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b";
            var matches = Regex.Matches(text, emailPattern);
            
            var emails = matches.Cast<Match>().Select(m => m.Value).Distinct().ToList();

            var result = emails.Any()
                ? $"📧 Found {emails.Count} email address(es):\n   • {string.Join("\n   • ", emails)}"
                : "No email addresses found in the text.";

            Console.WriteLine($"📝 Text Plugin: Extracted {emails.Count} email addresses");
            return await Task.FromResult(result);
        }

        [KernelFunction("reverse_text")]
        [Description("Reverses the order of characters in text")]
        public async Task<string> ReverseTextAsync(
            [Description("Text to reverse")] string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "No text provided to reverse.";
            }

            var reversed = new string(text.Reverse().ToArray());
            Console.WriteLine($"📝 Text Plugin: Reversed text");
            return await Task.FromResult($"Reversed text: {reversed}");
        }
    }
}