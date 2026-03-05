
namespace PortMoniter.PartialViews
{
    public interface IView
    {
        /// <summary>
        /// Close the view with the given result
        /// </summary>
        void CloseDialog(bool result);
    }
}