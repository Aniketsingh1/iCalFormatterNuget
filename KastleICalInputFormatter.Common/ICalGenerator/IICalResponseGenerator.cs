// <copyright file="IICalResponseGenerator.cs" company="Kastle Systems">
// Copyright (c) Kastle Systems. All rights reserved.
// </copyright>

namespace KastleICalInputFormatter.Common.ICalGenerator
{
    using KastleICalInputFormatter.Common.ApplicationModel.ICalEntityModel;
    using KastleICalInputFormatter.Common.ApplicationModel.VisitorEntityModel;

    /// <summary>
    /// Represents Interface for Ical Generator.
    /// </summary>
    public interface IICalResponseGenerator
    {
        /// <summary>
        /// Method to Generate Ical model.
        /// </summary>
        /// <param name="visitorRequestModel">VisitorRsequestModel.</param>
        /// <returns>ICalModel.</returns>
        public ICalModel GenerateICalResponseFromVisitorModel(VisitorRequestModel visitorRequestModel);

        /// <summary>
        /// Method to generate Ical string.
        /// </summary>
        /// <param name="calModel">Ical model.</param>
        /// <returns>Ical string.</returns>
        public string ConvertToIcalString(ICalModel calModel);
    }
}
