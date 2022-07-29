namespace pdq
{
    public interface ISelectColumnBuilder
    {
        /// <summary>
        /// Allows the column to be specified when constructing a dynamic response object.<br/>
        /// Note: This method is only used for expression parsing and will never be executed.
        /// </summary>
        /// <param name="name">The name of the column to be selected.</param>
        /// <returns>Returns null</returns>
        object Is(string name);

        /// <summary>
        /// Allows the column to be specified when constructing a dynamic response object.<br/>
        /// Note: This method is only used for expression parsing and will never be executed.
        /// </summary>
        /// <param name="name">The name of the column to be selected.</param>
        /// <param name="alias">The table alias that this column is selected from.</param>
        /// <returns>Returns null</returns>
        object Is(string name, string alias);

        /// <summary>
        /// Allows the column to be specified when constructing a concrete response object.<br/>
        /// Note: This method is only used for expression parsing and will never be executed.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the column, this will need to match the type
        /// of the property in the return type otherwise you will get build errors.
        /// </typeparam>
        /// <param name="name">The name of the column to be selected.</param>
        /// <returns>Returns the default value for <see cref="{T}"/>.</returns>
        T Is<T>(string name);

        /// <summary>
        /// Allows the column to be specified when constructing a concrete response object.<br/>
        /// Note: This method is only used for expression parsing and will never be executed.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the column, this will need to match the type
        /// of the property in the return type otherwise you will get build errors.
        /// </typeparam>
        /// <param name="name">The name of the column to be selected.</param>
        /// <param name="alias">The table alias that this column is selected from.</param>
        /// <returns>Returns the default value for <see cref="{T}"/>.</returns>
        T Is<T>(string name, string alias);
    }
}

