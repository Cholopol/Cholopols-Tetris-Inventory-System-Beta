using UnityEngine;
using UnityEngine.UI;

namespace ChosTIS
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class InformationlayoutComponent : MonoBehaviour, IItemComponent, IInformation
    {
        private Image bgImage;
        private RectTransform rectTransform;
        private Text itemName;
        private Text StackNum;



        private TetrisItem _owner;
        public void OnAttach(TetrisItem item) => _owner = item;
        public void OnDetach() => _owner = null;

        public void Initialize()
        {

        }




    }
}