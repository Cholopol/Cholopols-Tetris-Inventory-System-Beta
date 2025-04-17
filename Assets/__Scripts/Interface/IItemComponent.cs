
namespace ChosTIS
{
    public interface IItemComponent
    {
        void OnAttach(TetrisItem item);
        void OnDetach();
    }

    public interface IInformation { }
    public interface IGridPanel { }
    public interface IStackable { }
    public interface IAttachable { }
}