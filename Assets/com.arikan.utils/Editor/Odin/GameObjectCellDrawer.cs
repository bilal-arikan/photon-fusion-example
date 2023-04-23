namespace Arikan
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector.Editor.Drawers;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    [DrawerPriority(0, 1, 1)]
    // [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    internal sealed class GameObjectCellDrawer<TArray> : TwoDimensionalArrayDrawer<TArray, GameObject>
        where TArray : System.Collections.IList
    {
        protected override TableMatrixAttribute GetDefaultTableMatrixAttributeSettings()
        {
            return new TableMatrixAttribute()
            {
                SquareCells = true,
                HideColumnIndices = false,
                HideRowIndices = false,
                ResizableColumns = false,
            };
        }

        protected override GameObject DrawElement(Rect rect, GameObject value)
        {
            var id = DragAndDropUtilities.GetDragAndDropId(rect);
            DragAndDropUtilities.DrawDropZone(rect, value != null ? GUIHelper.GetAssetThumbnail(value, typeof(GameObject), true) : null, null, id); // Draws the drop-zone using the items icon.
            value = DragAndDropUtilities.ObjectPickerZone(rect, value, false, id);

            // if (value != null)
            // {
            //     // Item count
            //     var countRect = rect.Padding(2).AlignBottom(16);
            //     // value.ItemCount = EditorGUI.IntField(countRect, Mathf.Max(1, value.ItemCount));
            //     GUI.Label(countRect, "/ " + value.Item.ItemStackSize, SirenixGUIStyles.RightAlignedGreyMiniLabel);
            // }

            value = DragAndDropUtilities.DropZone(rect, value, id);                                     // Drop zone for ItemSlot structs.
            value = DragAndDropUtilities.DragZone(rect, value, true, true, id);                         // Enables dragging of the ItemSlot

            return value;
        }
    }

}
