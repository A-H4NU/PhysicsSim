namespace Hanu.ElectroLib.ComponentModel
{
    /// <summary>
    /// Notifies listeners of dynamic changes, such as when an item is added and removed, and even changes in property
    //  or the whole list is cleared.
    /// </summary>
    internal interface INotifyDeepCollectionChanged
    {
        /// <summary>
        /// Occurs when the collection changed
        /// </summary>
        event NotifyDeepCollectionChangedEventHandler DeepCollectionChanged;
    }

    public delegate void NotifyDeepCollectionChangedEventHandler(object sender, NotifyDeepCollectionChangedEventArgs e);
}