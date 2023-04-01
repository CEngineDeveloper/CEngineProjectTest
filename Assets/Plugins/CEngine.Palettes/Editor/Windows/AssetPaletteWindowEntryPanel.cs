using System;
using System.Collections.Generic;
using CYM.AssetPalette.CustomEditors;
using CYM.AssetPalette.Runtime;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CYM.AssetPalette.Windows
{
    public partial class AssetPaletteWindow
    {
        private const float Padding = 2;
        private const float EntrySpacing = 4;
        private const int EntrySizeMax = 128;
        private const float EntrySizeMin = EntrySizeMax * 0.45f;
        private const float ScrollBarWidth = 13;
        
        [NonSerialized] private readonly List<PaletteEntry> entriesSelected = new List<PaletteEntry>();
        [NonSerialized] private readonly List<PaletteEntry> entriesIndividuallySelected = new List<PaletteEntry>();

        private Vector2 entriesPanelScrollPosition;
        
        private SortModes SortMode
        {
            get
            {
                if (!EditorPrefs.HasKey(EntriesSortModeEditorPref))
                    SortMode = SortModes.Alphabetical;
                return (SortModes)EditorPrefs.GetInt(EntriesSortModeEditorPref);
            }
            set => EditorPrefs.SetInt(EntriesSortModeEditorPref, (int)value);
        }

        [NonSerialized] private bool didCacheSelectedFolderEntriesSerializedProperty;
        [NonSerialized] private SerializedProperty cachedSelectedFolderEntriesSerializedProperty;
        private SerializedProperty SelectedFolderEntriesSerializedProperty
        {
            get
            {
                if (!didCacheSelectedFolderEntriesSerializedProperty)
                {
                    didCacheSelectedFolderEntriesSerializedProperty = true;
                    cachedSelectedFolderEntriesSerializedProperty =
                        SelectedFolderSerializedProperty.FindPropertyRelative("entries");
                }
                return cachedSelectedFolderEntriesSerializedProperty;
            }
        }
        
        [NonSerialized] private GUIStyle cachedMessageTextStyle;
        [NonSerialized] private bool didCacheMessageTextStyle;
        private GUIStyle MessageTextStyle
        {
            get
            {
                if (!didCacheMessageTextStyle)
                {
                    didCacheMessageTextStyle = true;
                    cachedMessageTextStyle = new GUIStyle(EditorStyles.label)
                    {
                        alignment = TextAnchor.MiddleCenter
                    };
                }
                return cachedMessageTextStyle;
            }
        }
        
        [NonSerialized] private static GUIStyle cachedGridEntryRenameTextStyle;
        [NonSerialized] private static bool didCacheGridEntryRenameTextStyle;
        public static GUIStyle GridEntryRenameTextStyle
        {
            get
            {
                if (!didCacheGridEntryRenameTextStyle)
                {
                    didCacheGridEntryRenameTextStyle = true;
                    cachedGridEntryRenameTextStyle = new GUIStyle(EditorStyles.textField)
                    {
                        wordWrap = true,
                        alignment = TextAnchor.LowerCenter,
                    };
                }
                return cachedGridEntryRenameTextStyle;
            }
        }
        
        [NonSerialized] private static GUIStyle cachedListEntryRenameTextStyle;
        [NonSerialized] private static bool didCacheListEntryRenameTextStyle;
        private GUIStyle ListEntryRenameTextStyle
        {
            get
            {
                if (!didCacheListEntryRenameTextStyle)
                {
                    didCacheListEntryRenameTextStyle = true;
                    cachedListEntryRenameTextStyle = new GUIStyle(EditorStyles.textField)
                    {
                        wordWrap = true,
                        alignment = TextAnchor.MiddleLeft
                    };
                }
                return cachedListEntryRenameTextStyle;
            }
        }

        private GUIStyle EntryRenameTextStyle =>
            ShouldDrawListView ? ListEntryRenameTextStyle : GridEntryRenameTextStyle;
        
        [NonSerialized] private PaletteEntry entryBelowCursorOnMouseDown;

        private bool ShouldDrawListView => ZoomLevel == 0;
        
        private int GetEntryCount()
        {
            return SelectedFolder.Entries.Count;
        }
        
        private List<PaletteEntry> GetEntries()
        {
            return SelectedFolder.Entries;
        }
        
        private PaletteEntry GetEntry(int index)
        {
            return SelectedFolder.Entries[index];
        }
        
        private void SortEntriesInSerializedObject()
        {
            if (SortMode == SortModes.Unsorted)
                return;
            
            // Create a list of all the Palette Entries currently in the serialized object. Doing it this way means
            // that you can sort the list while adding new entries, without the sorting operation being a separate Undo
            List<PaletteEntry> entries = new List<PaletteEntry>();
            for (int i = 0; i < SelectedFolderEntriesSerializedProperty.arraySize; i++)
            {
                SerializedProperty entryProperty = SelectedFolderEntriesSerializedProperty.GetArrayElementAtIndex(i);
                PaletteEntry entry = entryProperty.GetValue<PaletteEntry>();
                entries.Add(entry);
            }
            
            // Now sort that list of Palette Entries.
            entries.Sort();
            if (SortMode == SortModes.ReverseAlphabetical)
                entries.Reverse();
            
            // Now make sure that the actual list as it exists in the serialized object has all its values in the same
            // position as that sorted list.
            for (int i = 0; i < entries.Count; i++)
            {
                SerializedProperty entryProperty = SelectedFolderEntriesSerializedProperty.GetArrayElementAtIndex(i);
                entryProperty.managedReferenceValue = entries[i];
            }
        }
        
        private void AddEntry(PaletteEntry entry, bool apply)
        {
            if (apply)
                CurrentCollectionSerializedObject.Update();

            SerializedProperty newEntryProperty =
                SelectedFolderEntriesSerializedProperty.AddArrayElement();
            newEntryProperty.managedReferenceValue = entry;
            
            if (apply)
            {
                // Applying it before the sorting because otherwise the sorting will be unable to find the items
                CurrentCollectionSerializedObject.ApplyModifiedProperties();

                SortEntriesInSerializedObject();
                CurrentCollectionSerializedObject.ApplyModifiedProperties();
            }
            
            SelectEntry(entry, false);
        }

        private void AddEntries(List<PaletteEntry> entries, bool apply = true)
        {
            if (apply)
                CurrentCollectionSerializedObject.Update();

            for (int i = 0; i < entries.Count; i++)
            {
                AddEntry(entries[i], false);
            }
            
            if (apply)
            {
                // Applying it before the sorting because otherwise the sorting will be unable to find the items
                CurrentCollectionSerializedObject.ApplyModifiedProperties();

                SortEntriesInSerializedObject();
                CurrentCollectionSerializedObject.ApplyModifiedProperties();
            }
        }

        private int IndexOfEntry(PaletteEntry entry)
        {
            for (int i = 0; i < SelectedFolderEntriesSerializedProperty.arraySize; i++)
            {
                PaletteEntry entryAtIndex = SelectedFolderEntriesSerializedProperty
                    .GetArrayElementAtIndex(i).GetValue<PaletteEntry>();
                if (entryAtIndex == entry)
                    return i;
            }
            return -1;
        }

        private void RemoveEntry(PaletteEntry entry)
        {
            int index = IndexOfEntry(entry);
            
            if (index != -1)
                RemoveEntryAt(index);
        }

        private void RemoveEntries(List<PaletteEntry> entries)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                RemoveEntry(entries[i]);
            }
        }
        
        private void RemoveEntryAt(int index)
        {
            CurrentCollectionSerializedObject.Update();
            SelectedFolderEntriesSerializedProperty.DeleteArrayElementAtIndex(index);
            
            // NOTE: *Need* to apply this after every individual change because otherwise GetValue<> will not return
            // correct values, and we need to do it that way to have 2020 support because Unity 2020 has a setter for
            // managedReferenceValue but not a setter >_>
            CurrentCollectionSerializedObject.ApplyModifiedProperties();
        }

        private void DrawEntriesPanel()
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                entryBelowCursorOnMouseDown = null;

            entriesPanelScrollPosition = GUILayout.BeginScrollView(
                entriesPanelScrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);

            // Draw a dark background for the entries panel.
            Rect entriesPanelRect = new Rect(0, 0, position.width - FolderPanelWidth, 90000000);
            EditorGUI.DrawRect(entriesPanelRect, new Color(0, 0, 0, 0.1f));

            // If the current state is invalid, draw a message instead.
            bool hasCollection = HasCollection;
            bool hasEntries = hasCollection && GetEntryCount() > 0;
            if (!hasCollection || !hasEntries)
            {
                DrawEntryPanelMessage(hasCollection);
                GUILayout.EndScrollView();
                return;
            }
            
            float containerWidth = Mathf.Floor(EditorGUIUtility.currentViewWidth) - FolderPanelWidth - ScrollBarWidth;

            GUILayout.Space(Padding);

            bool didClickASpecificEntry;
            if (ShouldDrawListView)
                DrawListEntries(containerWidth, out didClickASpecificEntry);
            else
                DrawGridEntries(containerWidth, out didClickASpecificEntry);

            GUILayout.Space(Padding);

            // If you didn't click an entry and weren't pressing SHIFT, clear the selection.
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !didClickASpecificEntry &&
                !Event.current.shift)
            {
                StopAllRenames(false);

                ClearEntrySelection();
                Repaint();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawListEntries(float containerWidth, out bool didClickASpecificEntry)
        {
            didClickASpecificEntry = false;

            for (int i = 0; i < GetEntryCount(); i++)
            {
                PurgeInvalidEntries(i);
                if (i >= GetEntryCount())
                    break;

                DrawListEntry(i, containerWidth, ref didClickASpecificEntry);
            }
        }

        private void DrawListEntry(int index, float containerWidth, ref bool didClickASpecificEntry)
        {
            PaletteEntry entry = GetEntry(index);
            Rect rect = GUILayoutUtility.GetRect(0, 0,
                GUILayout.Width(containerWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight));
            
            // Add an indent. Looks a bit more spacious this way. Unity does it for their project view list too.
            rect = ExtensionsRect.Indent(rect, 1);

            HandleEntrySelection(index, rect, entry, ref didClickASpecificEntry);

            Rect entryContentsRect = rect;
            SerializedProperty entryProperty = SelectedFolderEntriesSerializedProperty.GetArrayElementAtIndex(index);

            PaletteDrawing.DrawListEntry(entryContentsRect, entryProperty, entry, entriesSelected.Contains(entry));

            if (entry.IsRenaming)
            {
                // This is done purely by eye
                entryContentsRect.xMin += 17;
                DrawRenameEntry(entryProperty, entryContentsRect);
            }
        }

        private void DrawGridEntries(float containerWidth, out bool didClickASpecificEntry)
        {
            int entrySize = Mathf.RoundToInt(Mathf.Lerp(EntrySizeMin, EntrySizeMax, ZoomLevel));
            int columnCount = Mathf.FloorToInt(containerWidth / (entrySize + EntrySpacing));
            int rowCount = Mathf.CeilToInt((float)GetEntryCount() / columnCount);

            didClickASpecificEntry = false;

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(Padding);
                for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
                {
                    int index = rowIndex * columnCount + columnIndex;

                    PurgeInvalidEntries(index);

                    if (index >= GetEntryCount())
                    {
                        GUILayout.FlexibleSpace();
                        break;
                    }

                    DrawGridEntry(index, entrySize, ref didClickASpecificEntry);

                    if (columnIndex < columnCount - 1)
                        EditorGUILayout.Space(EntrySpacing);
                    else
                        GUILayout.FlexibleSpace();
                }

                GUILayout.Space(Padding);
                EditorGUILayout.EndHorizontal();

                if (rowIndex < rowCount - 1)
                    EditorGUILayout.Space(EntrySpacing);
            }
        }

        private void DrawGridEntry(int index, int entrySize, ref bool didClickASpecificEntry)
        {
            PaletteEntry entry = GetEntry(index);

            Rect rect = GUILayoutUtility.GetRect(
                0, 0, GUILayout.Width(entrySize), GUILayout.Height(entrySize));

            HandleEntrySelection(index, rect, entry, ref didClickASpecificEntry);

            bool isSelected = entriesSelected.Contains(entry);

            Color borderColor = isSelected ? Color.white : new Color(0.5f, 0.5f, 0.5f);
            float borderWidth = isSelected ? 2 : 1;
            Rect borderRect = ExtensionsRect.Expand(rect, borderWidth);
            GUI.DrawTexture(
                borderRect, EditorGUIUtility.whiteTexture, ScaleMode.ScaleToFit, true, 0.0f, borderColor, borderWidth,
                borderWidth);

            // Actually draw the entry itself.
            Rect entryContentsRect = rect;
            SerializedProperty entryProperty = SelectedFolderEntriesSerializedProperty.GetArrayElementAtIndex(index);

            PaletteDrawing.DrawGridEntry(entryContentsRect, entryProperty, entry);
            
            if (entry.IsRenaming)
            {
                Rect labelRect = PaletteEntryDrawerBase.GetRenameRect(entryContentsRect, renameText);
                DrawRenameEntry(entryProperty, labelRect);
            }
        }

        private void PurgeInvalidEntries(int index)
        {
            // Purge invalid entries.
            while (index < GetEntryCount() && !GetEntry(index).IsValid)
            {
                RemoveEntryAt(index);
            }
        }

        private void DrawRenameEntry(SerializedProperty entryProperty, Rect labelRect)
        {
            string renameControlId = GetRenameControlId(entryProperty);
            GUI.SetNextControlName(renameControlId);
            renameText = EditorGUI.TextField(labelRect, renameText, EntryRenameTextStyle);
            EditorGUI.FocusTextInControl(renameControlId);
        }

        private void HandleEntrySelection(int index, Rect rect, PaletteEntry entry, ref bool didClickASpecificEntry)
        {
            if (IsRenaming)
                return;
            
            // Allow this entry to be selected by clicking it.
            bool isMouseOnEntry = rect.Contains(Event.current.mousePosition) && isMouseInEntriesPanel;
            bool wasAlreadySelected = entriesSelected.Contains(entry);
            if (isDraggingAssetIntoEntryPanel && isMouseOnEntry &&
                entry.CanAcceptDraggedAssets(DragAndDrop.objectReferences))
            {
                DragAndDrop.AcceptDrag();
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;

                if (Event.current.type == EventType.DragPerform)
                {
                    CurrentCollectionSerializedObject.Update();
                    SerializedProperty serializedProperty = GetSerializedPropertyForEntry(entry);
                    entry.AcceptDraggedAssets(DragAndDrop.objectReferences, serializedProperty);
                    CurrentCollectionSerializedObject.ApplyModifiedProperties();
                }

                // Make sure nothing else handles this, like the entry panel itself.
                isDraggingAssetIntoEntryPanel = false;
            }
            else if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && isMouseOnEntry)
            {
                entryBelowCursorOnMouseDown = entry;
                
                if (Event.current.shift)
                {
                    // Shift+click to grow selection until this point.
                    if (entriesSelected.Count == 0)
                    {
                        SelectEntry(entry, true);
                    }
                    else
                    {
                        if (wasAlreadySelected)
                        {
                            // Define a new extremity away from the first individually selected entry.
                            // Might seem convoluted and with weird edge cases, but this is how Unity does it...
                            PaletteEntry firstEntryIndividuallySelected = entriesIndividuallySelected[0];
                            int indexOfFirstIndividuallySelectedEntry =
                                GetEntries().IndexOf(firstEntryIndividuallySelected);
                            if (index > indexOfFirstIndividuallySelectedEntry)
                            {
                                PaletteEntry lowestSelectedEntry = null;
                                for (int i = 0; i < GetEntryCount(); i++)
                                {
                                    if (entriesSelected.Contains(GetEntry(i)))
                                    {
                                        lowestSelectedEntry = GetEntry(i);
                                        break;
                                    }
                                }

                                entriesIndividuallySelected.Clear();
                                entriesIndividuallySelected.Add(lowestSelectedEntry);
                                indexOfFirstIndividuallySelectedEntry = GetEntries().IndexOf(lowestSelectedEntry);
                            }
                            else if (index < indexOfFirstIndividuallySelectedEntry)
                            {
                                PaletteEntry highestSelectedEntry = null;
                                for (int i = GetEntryCount() - 1; i >= 0; i--)
                                {
                                    if (entriesSelected.Contains(GetEntry(i)))
                                    {
                                        highestSelectedEntry = GetEntry(i);
                                        break;
                                    }
                                }

                                entriesIndividuallySelected.Clear();
                                entriesIndividuallySelected.Add(highestSelectedEntry);
                                indexOfFirstIndividuallySelectedEntry = GetEntries().IndexOf(highestSelectedEntry);
                            }

                            SelectEntriesByRange(indexOfFirstIndividuallySelectedEntry, index, true);
                        }
                        else
                        {
                            // Grow the selection from the last individually selected entry.
                            PaletteEntry lastEntryIndividuallySelected =
                                entriesIndividuallySelected[entriesIndividuallySelected.Count - 1];
                            int indexOfLastIndividuallySelectedEntry =
                                GetEntries().IndexOf(lastEntryIndividuallySelected);
                            SelectEntriesByRange(indexOfLastIndividuallySelectedEntry, index, false);
                        }
                    }
                }
                else if (Event.current.control)
                {
                    // Control+click to add specific files to the selection.
                    if (!wasAlreadySelected)
                        SelectEntry(entry, false);
                    else
                        DeselectEntry(entry);
                }
                else
                {
                    // Regular click to select only this entry.
                    if (!wasAlreadySelected)
                        SelectEntry(entry, true);

                    if (Event.current.alt)
                        entry.SelectAsset();

                    // Allow assets to be opened by double clicking on them.
                    if (Event.current.clickCount == 2)
                        entry.Open();
                }

                didClickASpecificEntry = true;
                Repaint();
            }
            else if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && isMouseOnEntry
                     && !Event.current.control && !Event.current.shift)
            {
                // Regular click to select only this entry.
                SelectEntry(entry, true);
                Repaint();
            }
            else if (Event.current.type == EventType.MouseDrag && Event.current.button == 0 && isMouseOnEntry &&
                     !isResizingFolderPanel && isMouseInEntriesPanel && !IsZoomLevelFocused &&
                     entriesSelected.Contains(entryBelowCursorOnMouseDown))
            {
                StartEntriesDrag();
            }
            else if (Event.current.type == EventType.DragExited)
            {
                CancelEntriesDrag();
            }
        }

        private void StartEntriesDrag()
        {
            DragAndDrop.PrepareStartDrag();
            List<Object> selectedAssets = new List<Object>();
            foreach (PaletteEntry selectedEntry in entriesSelected)
            {
                if (selectedEntry is PaletteAsset paletteAsset)
                    selectedAssets.Add(paletteAsset.Asset);
            }

            DragAndDrop.objectReferences = selectedAssets.ToArray();
            // Mark the drag as being an asset palette entry drag, so we know not to accept it again ourselves.
            // Also pass along the name of the directory so we can handle stuff like dragging assets out into
            // another folder (but ignore the folder it was originally dragged from).
            DragAndDrop.SetGenericData(EntryDragGenericDataType, SelectedFolder.Name);
            string dragName = selectedAssets.Count == 1 ? selectedAssets[0].ToString() : "<multiple>";
            DragAndDrop.StartDrag(dragName);
        }

        private static void CancelEntriesDrag()
        {
            DragAndDrop.SetGenericData(EntryDragGenericDataType, null);
        }

        private void DrawEntryPanelMessage(bool hasCollection)
        {
            GUILayout.FlexibleSpace();

            if (!hasCollection)
            {
                EditorGUILayout.LabelField(
                    "To begin organizing assets, create a collection.", MessageTextStyle);
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    bool shouldCreate = GUILayout.Button("Create", GUILayout.Width(100));
                    GUILayout.FlexibleSpace();

                    if (shouldCreate)
                        CreateNewCollection();
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("Drag assets here!", MessageTextStyle);
            }

            GUILayout.FlexibleSpace();
        }

        private void DeselectEntry(PaletteEntry entry)
        {
            entriesSelected.Remove(entry);
            entriesIndividuallySelected.Remove(entry);
        }

        private void SelectEntry(PaletteEntry entry, bool exclusively)
        {
            if (PaletteEntry.IsEntryBeingRenamed && PaletteEntry.EntryCurrentlyRenaming != entry)
                StopEntryRename(true);

            if (exclusively)
                ClearEntrySelection();

            entriesSelected.Add(entry);
            entriesIndividuallySelected.Add(entry);
        }

        private void SelectEntriesByRange(int from, int to, bool exclusively)
        {
            if (PaletteEntry.IsEntryBeingRenamed)
                StopEntryRename(true);

            if (exclusively)
            {
                entriesSelected.Clear();
                // NOTE: Do NOT clear the individually selected entries. These are used to determine from what point we 
                // define the range. Used for SHIFT-selecting ranges of entries.
            }

            int direction = @from <= to ? 1 : -1;
            for (int i = @from; i != to; i += direction)
            {
                if (!entriesSelected.Contains(GetEntry(i)))
                    entriesSelected.Add(GetEntry(i));
            }

            if (!entriesSelected.Contains(GetEntry(to)))
                entriesSelected.Add(GetEntry(to));
        }

        private void SelectEntries(List<PaletteEntry> entries, bool exclusively)
        {
            if (PaletteEntry.IsEntryBeingRenamed)
                StopEntryRename(true);

            if (exclusively)
                ClearEntrySelection();

            foreach (PaletteEntry entry in entries)
            {
                entriesSelected.Add(entry);
            }
        }

        private void ClearEntrySelection()
        {
            entriesSelected.Clear();
            entriesIndividuallySelected.Clear();
        }

        private void AddEntryForProjectWindowSelection()
        {
            PaletteSelectionShortcut paletteSelectionShortcut = new PaletteSelectionShortcut(Selection.objects);
            AddEntry(paletteSelectionShortcut, true);
            
            if (Selection.objects.Length > PaletteSelectionShortcut.ItemNamesToDisplayMax)
                StartEntryRename(paletteSelectionShortcut);
            
            Repaint();
        }

        private SerializedProperty GetSerializedPropertyForEntry(PaletteEntry entry)
        {
            int index = IndexOfEntry(entry);

            if (index == -1)
                return null;
            
            return SelectedFolderEntriesSerializedProperty.GetArrayElementAtIndex(index);
        }
        
        public void StartEntryRename(PaletteEntry entry)
        {
            renameText = entry.Name;
            entry.StartRename();
            
            EditorGUI.FocusTextInControl(GetRenameControlId(entry));
        }

        private void StopEntryRename(bool isCancel)
        {
            if (!PaletteEntry.IsEntryBeingRenamed)
                return;

            bool isValidRename = PaletteEntry.EntryCurrentlyRenaming.Name != renameText;
            if (isValidRename && !isCancel)
            {
                CurrentCollectionSerializedObject.Update();
                int index = SelectedFolder.Entries.IndexOf(PaletteEntry.EntryCurrentlyRenaming);
                SerializedProperty entryBeingRenamedProperty =
                    SelectedFolderEntriesSerializedProperty.GetArrayElementAtIndex(index);
                SerializedProperty customNameProperty = entryBeingRenamedProperty.FindPropertyRelative("customName");
                customNameProperty.stringValue = renameText;
                CurrentCollectionSerializedObject.ApplyModifiedProperties();
                
                // Also sort the collection. Make sure to do this AFTER we apply the rename, otherwise the sort will
                // be based on the old name! Don't worry, doing this in a separate Apply doesn't cause a separate Undo
                CurrentCollectionSerializedObject.Update();
                SortEntriesInSerializedObject();
                CurrentCollectionSerializedObject.ApplyModifiedProperties();
            }

            PaletteEntry.CancelRename();
            Repaint();
        }
    }
}
