﻿using System.Globalization;

namespace Umbraco.Web.Routing
{
    /// <summary>
    /// Represents a facade domain.
    /// </summary>
    public class Domain
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Domain"/> class.
        /// </summary>
        /// <param name="id">The unique identifier of the domain.</param>
        /// <param name="name">The name of the domain.</param>
        /// <param name="contentId">The identifier of the content which supports the domain.</param>
        /// <param name="culture">The culture of the domain.</param>
        /// <param name="isWildcard">A value indicating whether the domain is a wildcard domain.</param>
        public Domain(int id, string name, int contentId, CultureInfo culture, bool isWildcard)
        {
            Id = id;
            Name = name;
            ContentId = contentId;
            Culture = culture;
            IsWildcard = isWildcard;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Domain"/> class.
        /// </summary>
        /// <param name="domain">An origin domain.</param>
        protected Domain(Domain domain)
        {
            Id = domain.Id;
            Name = domain.Name;
            ContentId = domain.ContentId;
            Culture = domain.Culture;
            IsWildcard = domain.IsWildcard;
        }

        /// <summary>
        /// Gets the unique identifier of the domain.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the name of the domain.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the identifier of the content which supports the domain.
        /// </summary>
        public int ContentId { get; private set; }

        /// <summary>
        /// Gets the culture of the domain.
        /// </summary>
        public CultureInfo Culture { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the domain is a wildcard domain.
        /// </summary>
        public bool IsWildcard { get; private set; }
    }
}
