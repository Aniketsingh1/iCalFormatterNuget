// <copyright file="IVisitorRequestGenerator.cs" company="Kastle Systems">
// Copyright (c) Kastle Systems. All rights reserved.
// </copyright>

namespace KastleICalInputFormatter.Common.VisitorRequestGenerator
{
    using KastleICalInputFormatter.Common.ApplicationModel.VisitorEntityModel;

    /// <summary>
    /// Represents Interface to generate Visitor request model.
    /// </summary>
    public interface IVisitorRequestGenerator
    {
        /// <summary>
        /// Method to Generate Visitor request model from Ical input string.
        /// </summary>
        /// <param name="iCalRequestString">Ical string.</param>
        /// <returns>Visitor request Model.</returns>
        public VisitorRequestModel GenerateVisitorRequestFromICalString(string iCalRequestString);

        /// <summary>
        /// Method to return UID from ical string.
        /// </summary>
        /// <param name="icalString">Ical string.</param>
        /// <returns>UID.</returns>
        public string GetUIDFromICalString(string icalString);
    }
}
