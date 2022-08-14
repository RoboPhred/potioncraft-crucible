// <copyright file="LocalizedString.cs" company="RoboPhredDev">
// This file is part of the Crucible Modding Framework.
//
// Crucible is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// Crucible is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License
// along with Crucible; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
// </copyright>

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System.Collections.Generic;
    using System.Linq;
    using global::PotionCraft.LocalizationSystem;

    /// <summary>
    /// Represents a string that can be localized to different languages.
    /// </summary>
    public class LocalizedString
    {
        private readonly Dictionary<string, string> localizedStrings = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedString"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value to use if no localized string is found for the current language.</param>
        public LocalizedString(string defaultValue)
        {
            this.DefaultValue = defaultValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedString"/> class.
        /// </summary>
        public LocalizedString()
        {
        }

        /// <summary>
        /// Gets or sets the default value to use if no localized string is found for the current language.
        /// </summary>
        public string DefaultValue { get; set; }

        public static implicit operator LocalizedString(string s)
        {
            return new LocalizedString(s);
        }

        public static implicit operator string(LocalizedString s)
        {
            return s.GetText();
        }

        /// <summary>
        /// Adds a localized value for this string.
        /// </summary>
        /// <param name="locale">The name of the locale to set.</param>
        /// <param name="value">The value to set for this locale.</param>
        public void SetLocalization(string locale, string value)
        {
            this.localizedStrings[locale.ToLowerInvariant()] = value;
        }

        /// <summary>
        /// Gets the localized text.
        /// </summary>
        /// <returns>The localized text.</returns>
        public string GetText()
        {
            var locale = LocalizationManager.CurrentLocale.ToString().ToLowerInvariant();
            if (this.localizedStrings.TryGetValue(locale, out var value))
            {
                return value;
            }

            return this.DefaultValue ?? this.localizedStrings.Values.FirstOrDefault() ?? string.Empty;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.GetText();
        }
    }
}
