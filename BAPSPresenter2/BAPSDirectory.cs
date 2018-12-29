using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace BAPSPresenter2
{
    public partial class BAPSDirectory : UserControl
    {
        /// <summary>
        /// The ID of this directory.
        /// <para>
        /// The first time this property is accessed, we retrieve the
        /// directory ID from this directory's Tag.  The tag must be
        /// a string.
        /// </para>
        /// <para>
        /// If the ID is negative, there was an error retrieving the ID.
        /// </para>
        /// </summary>
        public int DirectoryID
        {
            get => _directoryID;
            set
            {
                if (0 <= _directoryID) throw new InvalidOperationException("Can't set a directory ID multiple times");
                _directoryID = value;
            }
        }

        private int _directoryID = -1;

        #region Events

        public event EventHandler<ushort> RefreshRequest;

        #endregion Events

        public BAPSDirectory()
        {
            InitializeComponent();
        }

        #region Directory listing

        /// <summary>
        /// Gets the track at the given index of this directory as a string.
        /// </summary>
        /// <param name="index">The index to request.</param>
        /// <returns>The string form of the track at the given index.</returns>
        public string TrackAt(int index) => Listing.Items[index].ToString();

        /// <summary>
        /// Adds an entry into the directory.
        /// </summary>
        /// <param name="entry">The new entry to add.</param>
        public void Add(string entry) => Listing.Items.Add(entry);

        /// <summary>
        /// Clears the directory listing and updates its name.
        /// </summary>
        /// <param name="directoryName">The new name to display on the directory.</param>
        public void Clear(string directoryName)
        {
            Listing.Items.Clear();
            RefreshButton.Text = directoryName;
        }

        #endregion Directory listing

        #region Event handlers

        private void Listing_MouseDown(object sender, MouseEventArgs e)
        {
            Debug.Assert(sender == Listing, "Mouse down event somehow fired from somewhere else");

            var index = Listing.SelectedIndex;
            if (index < 0) return;
            var id = DirectoryID;
            if (id < 0) return;

            // Starts a drag-and-drop operation.
            var fts = new BAPSPresenter.FolderTempStruct(index, id);
            _ = Listing.DoDragDrop(fts, DragDropEffects.Copy);
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            var id = DirectoryID;
            if (id < 0) return;
            RefreshRequest?.Invoke(this, (ushort)id);
        }

        #endregion Event handlers
    }
}
