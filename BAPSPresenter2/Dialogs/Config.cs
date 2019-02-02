using BAPSClientCommon;
using BAPSClientCommon.BapsNet;
using System;
using System.Linq; // for collection queries
using System.Windows.Forms;
using BAPSClientCommon.ServerConfig;
using Message = BAPSClientCommon.BapsNet.Message;

namespace BAPSPresenter2.Dialogs
{
    public partial class Config : Form
    {
        /// <summary>
        /// Forwards key-down events that aren't handled by this dialog.
        /// </summary>
        public event KeyEventHandler KeyDownForward;

        /// <summary>
        /// optionid to optioninfo lookup.
        /// </summary>
        private System.Collections.Generic.SortedDictionary<uint, ConfigOptionInfo> options =
            new System.Collections.Generic.SortedDictionary<uint, ConfigOptionInfo>();

        private bool optionCountSet = false;

        private int numberOfOptions = 0;

        /// <summary>
        /// Used to store datagrid controls (for indexed options) options
        /// are columns in the datagrid that share the same index control.
        /// </summary>
        private System.Collections.Generic.Dictionary<int, IndexControlsLookup> indexControls =
            new System.Collections.Generic.Dictionary<int, IndexControlsLookup>();

        /// <summary>
        /// A mutex to stop the form closing while it is being updated.
        /// </summary>
        public System.Threading.Mutex closeMutex = new System.Threading.Mutex();

        private System.Collections.Concurrent.BlockingCollection<Message> msgQueue = null;

        public Config(System.Collections.Concurrent.BlockingCollection<Message> msgQueue)
        {
            this.msgQueue = msgQueue;

            InitializeComponent();

            saveButton.Enabled = false;
        }

        /** When the form has received all the options it was told it will receive,
			it can start to generate the UI, this is used by the main client so that
			it can 'Invoke' the UI generation as it is supposed to
		**/

        public bool isReadyToShow() => optionCountSet && (numberOfOptions == 0);

        /** Config System control flow...
            construct form -> SEND: get all options
            receive option -> if choice type, SEND: get all choices for the option
            continue receiving options until... last option received
            create UI
            IF all the options are not yet complete (IE there are choice types...)
                receive choice
                continue receiving choices until the form data is complete
            SEND: request all settings
            receive setting -. enable control as setting is received
                WORK NEEDED: disable datagrid cells until data is set in them correctly
            continue receiving settings until all received
            enable save button
        **/
        /** work on security for settings, ie saving to one without write perm is not severe **/
        /** test logic sequence slowly! + different option sets! **/
        /** Only save settings that have had a value received **/
        /** WORK NEEDED: crash on text in an integer field! **/

        private struct IndexControlsLookup
        {
            public readonly DataGrid dg;
            public readonly System.Data.DataTable dt;

            public IndexControlsLookup(DataGrid _dg, System.Data.DataTable _dt)
            {
                dg = _dg;
                dt = _dt;
            }
        }

        public void setNumberOfOptions(int _numberOfOptions)
        {
            numberOfOptions = _numberOfOptions;
            optionCountSet = true;
            if (numberOfOptions == 0)
            {
                MessageBox.Show("There are no options which you are able to configure.", "Error:", MessageBoxButtons.OK);
                Close();
            }
        }

        public void addOption(ConfigOptionInfo option)
        {
            /** Decrement the number of options remaining so that we know when we can draw the form **/
            numberOfOptions--;
            /** Add the option to the hashtable **/
            options.Add((uint)option.getOptionid(), option);
            /** If it is a choice type we must get the choices for it before requesting the settings **/
            if ((ConfigType)option.getType() == ConfigType.Choice)
            {
                var cmd = Command.Config | Command.GetOptionChoices;
                msgQueue.Add(new Message(cmd).Add((uint)option.getOptionid()));
            }
        }

        public void updateUI()
        {
            /** Handles for holding controls while filling them in **/
            TextBox tb;
            Label lbl;
            ComboBox cb;
            /** We don't care about the association of optionid to optioninfo just the info **/
            var ops = options.Values;

            /** The vertical multiplier (essentially how many control have been generated into the
                current column.
            **/
            int yMultiplier = 0;
            /** Which column are we generating **/
            int columnNumber = 1;
            /** The total number of columns **/
            const int totalColumns = 3;

            const int columnTopBottomPadding = 16;
            const int columnWidth = 350;
            const int columnLRPadding = 10;

            const int datagridHeight = 120;

            const int labelWidth = 160;
            const int labelHeight = 20;
            const int controlHeight = 20;
            const int controlWidth = columnWidth - labelWidth;
            const int rowBottomPadding = 10;
            const int rowHeight = controlHeight + rowBottomPadding;

            /** Vertical offset from the fixed start coordinarte **/
            int yOffset = columnTopBottomPadding;
            /** Horizontal offset from the fixed start coordinarte **/
            int xOffset = columnLRPadding;

            /** Decide if now is the time to switch to a new column. **/
            bool splitColumn;

            /** Lets save some time and wait until the end to draw all this! **/
            SuspendLayout();
            /** Increment the index and the multiplier each iteration (so that the multiplier
                can reset at start of column 2
            **/

            /** i . The current index into the option array **/
            int i = 0;
            foreach (var option in options.Values)
            {
                splitColumn = columnNumber < totalColumns && i - 1 >= columnNumber * (ops.Count / totalColumns);
                /**
                 * If splitting to new column or if we've reached the end of the last column
                 * check if we need to make the window height larger based on the last row's contents
                **/
                if (splitColumn || (i == ops.Count - 1))
                {
                    /** Set the form height to the height of the first column **/
                    int newHeight = (rowHeight * yMultiplier) + status.Height + (2 * columnTopBottomPadding);
                    if (newHeight > Height) Height = newHeight;
                }
                /** When half the controls have been generated move onto the second column
                    WORK NEEDED: a better method of splitting controls into columns, eg. half
                    the options != half space on screen due to indexed options.
                **/
                if (splitColumn)
                {
                    /** Place the statusBar correctly after resize **/
                    status.Top = yOffset + (rowHeight * yMultiplier) + columnTopBottomPadding;

                    /** Move the buttons to a sensible place **/
                    int buttonTopOffset = yOffset + (rowHeight * yMultiplier) + (2 * columnTopBottomPadding);
                    saveButton.Location = new System.Drawing.Point(columnLRPadding, buttonTopOffset);
                    cancelButton.Location = new System.Drawing.Point(saveButton.Right + 10, buttonTopOffset);
                    restartButton.Location = new System.Drawing.Point(cancelButton.Right + 10, buttonTopOffset);

                    /** New horizontal offset to represent next column **/
                    xOffset = (columnNumber * (columnWidth + columnLRPadding)) + columnLRPadding;
                    /** Back to the top row **/
                    yMultiplier = 0;
                    /** We are now on the next **/
                    columnNumber++;
                    /** Set the offset to the same value as originally **/
                    yOffset = columnTopBottomPadding;
                }
                /** Indexed options must be treated differently, they are placed into datagrids **/
                if (option.isIndexed())
                {
                    /** This needs to be done using a datagrid so that we can have indexable
                        settings for it.
                    **/
                    /** If this is the first option for this set of indexed options (options based on
                        the same maximum index value then there will be no datagrid already so we
                        make one.
                    **/
                    /** Create a handle for the datagrid and datatable **/
                    DataGrid dg;
                    System.Data.DataTable dt;
                    if (!indexControls.ContainsKey(option.getGroupid()))
                    {
                        dg = new DataGrid();
                        /** Create a table style for the datagrid **/
                        var dgts = new DataGridTableStyle();
                        /** Taken from the forms designer... assumed to be needed **/
                        ((System.ComponentModel.ISupportInitialize)dg).BeginInit();
                        dg.DataMember = "";
                        dg.HeaderForeColor = System.Drawing.SystemColors.ControlText;
                        /** Use the horizontal and vertical attributes to place the control correctly **/
                        dg.Location = new System.Drawing.Point(xOffset, yOffset + (rowHeight * yMultiplier));
                        /** The control refers to the group of options and is so named **/
                        dg.Name = string.Concat("groupOption", option.getGroupid().ToString());
                        /** Set the size of the entire grid. **/
                        dg.Size = new System.Drawing.Size(columnWidth, datagridHeight);
                        /** Account for the size of the datagrid, so that the next control boxes don't draw ontop of it **/
                        yOffset += datagridHeight + rowBottomPadding - rowHeight;
                        /** WORK NEEDED: tabindex counter **/
                        dg.TabIndex = 2;
                        dg.TableStyles.Add(dgts);
                        /** Hide the description bit at the top, it serves no purpose **/
                        dg.CaptionVisible = false;
                        /** link the grid and the gridstyle **/
                        dgts.DataGrid = dg;
                        dgts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
                        dgts.MappingName = "";
                        /** Add it to the form controls **/
                        Controls.Add(dg);
                        ((System.ComponentModel.ISupportInitialize)dg).EndInit();
                        /** Finished basic datagrid initialization **/

                        /** Create a datatable to hold all the info for this set of options **/
                        dt = new System.Data.DataTable();

                        /** Add an index column so that visually we can see which index we are editing **/
                        dt.Columns.Add("index", Type.GetType("System.Int32"));
                        dt.Columns["index"].ReadOnly = true;
                        /** Add a column style so that the column created above can be seen **/
                        var tbc = new DataGridTextBoxColumn
                        {
                            HeaderText = "",
                            /** map it onto the index field **/
                            MappingName = "index",
                            /** It is only a 1 or 2 digit number **/
                            Width = 20
                        };
                        /** Add the style onto the grid **/
                        dg.TableStyles[0].GridColumnStyles.Add(tbc);
                        /** Make a dataview for the datatable so that we cant delete or create new rows
                            rows are deleted or created by altering the value of the option controlling
                            the number of indices
                        **/

                        var dv = new System.Data.DataView(dt)
                        {
                            AllowNew = false,
                            AllowDelete = false
                        };
                        dg.SetDataBinding(dv, "");
                        /** Add the datagrid and table to the hashtable so that future options in
                            the same group can reuse them
                        **/
                        indexControls.Add(option.getGroupid(), new IndexControlsLookup(dg, dt));
                    }
                    else
                    {
                        /** Get the datatable and the grid out of the cache and effectively remove an item
                            from the column by backing up the vertical offset by the row height (the same value as
                            the multiplier's multiplicand)
                        **/
                        var icl = indexControls[option.getGroupid()];
                        dt = icl.dt;
                        dg = icl.dg;
                        yOffset -= rowHeight;
                    }
                    /** Each option type has to be treated differently as it is stored differently in the datatable **/
                    switch ((ConfigType)option.getType())
                    {
                        case ConfigType.Str:
                            {
                                /** Strings are stored as... strings **/
                                dt.Columns.Add(option.getDescription(), typeof(string));
                                /** In... textboxes **/
                                var tbc = new DataGridTextBoxColumn
                                {
                                    /** The description is used as the field name. WORK NEEDED: this could cause
                                        bugs, consider using the optionid
                                    **/
                                    HeaderText = option.getDescription(),
                                    MappingName = option.getDescription()
                                };
                                dg.TableStyles[0].GridColumnStyles.Add(tbc);
                            }
                            break;

                        case ConfigType.Int:
                            {
                                /** Integers are stored as... integers **/
                                dt.Columns.Add(option.getDescription(), typeof(int));
                                /** In text boxes... WORK NEEDED: perhaps add a textchanged event to validate it being a number **/
                                var tbc = new DataGridTextBoxColumn
                                {
                                    /** Same rules as above **/
                                    HeaderText = option.getDescription(),
                                    MappingName = option.getDescription()
                                };
                                dg.TableStyles[0].GridColumnStyles.Add(tbc);
                            }
                            break;

                        case ConfigType.Choice:
                            {
                                /** Choice types are stored as integers as well because we refer to the choice index not the value **/
                                dt.Columns.Add(option.getDescription(), typeof(int));
                                var cbc = new BAPSPresenter.DataGridComboBoxColumn
                                {
                                    HeaderText = option.getDescription(),
                                    MappingName = option.getDescription(),
                                    Width = 200
                                };
                                dg.TableStyles[0].GridColumnStyles.Add(cbc);
                                /** In order for the combo box to receive its values it has to be exposed
                                    DO NOT MESS WITH IT.. AT ALL
                                **/
                                option.setComboBoxControl(cbc.getComboBox());
                            }
                            break;
                    }
                    /** The option needs to know where its data is stored, it assumes it will have a valid
                        handle to a datatable if it is an indexed option
                    **/
                    option.setDataTable(dt);
                }
                else
                {
                    /** NON INDEXED options **/
                    /** The non indexed options have labels to describe them **/
                    lbl = new Label
                    {
                        /** Place the label according to the horizontal and vertical rules **/
                        Location = new System.Drawing.Point(xOffset, yOffset + ((controlHeight + rowBottomPadding) * yMultiplier)),
                        Name = string.Concat("optionLabel", option.getOptionid().ToString()),
                        Size = new System.Drawing.Size(labelWidth, labelHeight),
                        TabStop = false,
                        /** The label contains the option description **/
                        Text = option.getDescription()
                    };
                    /** For completeness the option is told what its label handle is **/
                    option.setLabelControl(lbl);
                    /** Add the label to the form **/
                    Controls.Add(lbl);

                    switch ((ConfigType)option.getType())
                    {
                        case ConfigType.Str:
                        case ConfigType.Int:
                            {
                                /** String and integer options can be treated the same (For now)
                                    WORK NEEDED: validate entries to integer text boxes to ensure they are integers
                                **/
                                tb = new TextBox
                                {
                                    /** Place the textbox according to the horizontal and vertical rules **/
                                    Location = new System.Drawing.Point(labelWidth + xOffset, yOffset + ((controlHeight + rowBottomPadding) * yMultiplier)),
                                    Name = string.Concat("optionBox", option.getOptionid().ToString()),
                                    Size = new System.Drawing.Size(controlWidth, controlHeight),
                                    /** WORK NEEDED: tab indices **/
                                    TabIndex = option.getOptionid(),
                                    /** Start with the control disabled and with set-me as its text to show it
                                        has not received a value from the server (useful ofr debugging only)
                                    **/
                                    Text = "[set-me]",
                                    /** The control knows what option it refers to... unused for now maybe **/
                                    Tag = option
                                };
                                /** Inform the option what control it is hosted in **/
                                option.setTextBoxControl(tb);
                                tb.Enabled = true;
                                /** Show the control on the form **/
                                Controls.Add(tb);
                            }
                            break;

                        case ConfigType.Choice:
                            {
                                cb = new ComboBox
                                {
                                    /** Place the textbox according to the horizontal and vertical rules **/
                                    Location = new System.Drawing.Point(labelWidth + xOffset, yOffset + ((controlHeight + rowBottomPadding) * yMultiplier)),
                                    Name = string.Concat("optionBox", option.getOptionid().ToString()),
                                    Size = new System.Drawing.Size(controlWidth, controlHeight),
                                    /** WORK NEEDED: tab indices **/
                                    TabIndex = option.getOptionid(),
                                    /** The control knows what option it refers to... unused for now maybe **/
                                    Tag = option,
                                    /** Make it uneditable **/
                                    DropDownStyle = ComboBoxStyle.DropDownList
                                };
                                /** Inform the option what control it is hosted in **/
                                option.setComboBoxControl(cb);
                                /** Disable the control to start with (so it cannot be set until the server
                                    has sent its current value
                                **/
                                cb.Enabled = true;
                                /** Show the control on the form **/
                                Controls.Add(cb);
                            }
                            break;
                    }
                }

                i++;
                yMultiplier++;
            }
            /** Get the required height for column 2 (same equation as used after end of column 1) **/
            int minHeight = 28 + yOffset + (24 * (yMultiplier + 1)) + status.Height + 32;
            /** If the height of the form is less that what is needed resize as above **/
            if (Height < minHeight)
            {
                Height = minHeight;
                status.Top = yOffset + 24 * (yMultiplier + 1) + 32;
                saveButton.Top = yOffset + (24 * yMultiplier) + 4;
                restartButton.Top = yOffset + (24 * yMultiplier) + 4;
                cancelButton.Top = yOffset + (24 * yMultiplier) + 4;
            }
            Width = totalColumns * (columnWidth + 2 * columnLRPadding);
            /** Show all the controls that have just been made **/
            ResumeLayout(false);
            PerformLayout();
            /** Attempt to receive all the settings (will not happen if there are outstanding choices
                for some of the CHOICE type options still to receive)
            **/
            receiveSettingsIfReady();
        }

        public void setResult(uint optionid, ConfigResult res)
        {
            // WORK NEEDED: fix this
            options[optionid].setResult(res == ConfigResult.Success);
            if (res != ConfigResult.Success)
            {
                statusLabel.Text = string.Concat("Failed to set: ", options[optionid].getDescription(), ". Error: ", ConfigResultText.Text[(int)res]);
            }
            var allReceived = options.Values.All(op => op.hasReceivedResult());
            if (!allReceived) return;

            var allSucceeded = options.Values.All(op => op.getResult());

            if (allSucceeded)
            {
                closeMutex.WaitOne();
                Close();
                closeMutex.ReleaseMutex();
            }
            else
            {
                saveButton.Enabled = true;
            }
        }

        public void setValue(uint id, string str)
        {
            options[id].setValue(str);
            enableSaveButtonIfReady();
        }

        public void setValue(uint id, int value)
        {
            options[id].setValue(value);
            enableSaveButtonIfReady();
        }

        public void setValue(uint id, int index, string str)
        {
            options[id].setValue(index, str);
            enableSaveButtonIfReady();
        }

        public void setValue(uint id, int index, int value)
        {
            options[id].setValue(index, value);
            enableSaveButtonIfReady();
        }

        public void addChoice(uint optionid, int choiceid, string description)
        {
            options[optionid].addChoice(choiceid, description);
            receiveSettingsIfReady();
        }

        public void setChoiceCount(uint optionid, int count)
        {
            options[optionid].setChoiceCount(count);
        }

        private void receiveSettingsIfReady()
        {
            if (!(optionCountSet && (numberOfOptions == 0))) return;
            if (options.Values.Any(op => !op.isValid())) return;
            Command cmd = Command.Config | Command.GetConfigSettings;
            msgQueue.Add(new Message(cmd));
            optionCountSet = false;
        }

        private void enableSaveButtonIfReady()
        {
            System.Collections.ArrayList ops = new System.Collections.ArrayList(options.Values);
            for (int i = 0; i < options.Count; i++)
            {
                if (!((ConfigOptionInfo)ops[i]).isComplete()) return;
            }
            saveButton.Enabled = true;
        }

        #region Event handlers

        private void saveButton_Click(object sender, EventArgs e)
        {
            saveButton.Enabled = false;
            var ops = new System.Collections.ArrayList(options.Values);
            Command cmd;
            ConfigOptionInfo coi;
            for (int i = 0; i < options.Count; i++)
            {
                coi = (ConfigOptionInfo)ops[i];
                if (coi.isIndexed())
                {
                    for (int j = 0; j < coi.getIndexCount(); j++)
                    {
                        cmd = Command.Config | Command.SetConfigValue | Command.ConfigUseValueMask | (Command)j;
                        switch ((ConfigType)coi.getType())
                        {
                            case ConfigType.Str:
                                {
                                    msgQueue.Add(new Message(cmd).Add((uint)coi.getOptionid()).Add((uint)coi.getType()).Add(coi.getValueStr(j)));
                                }
                                break;

                            case ConfigType.Int:
                            case ConfigType.Choice:
                                {
                                    msgQueue.Add(new Message(cmd).Add((uint)coi.getOptionid()).Add((uint)coi.getType()).Add((uint)coi.getValueInt(j)));
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }
                else
                {
                    cmd = Command.Config | Command.SetConfigValue;
                    switch ((ConfigType)coi.getType())
                    {
                        case ConfigType.Str:
                            {
                                msgQueue.Add(new Message(cmd).Add((uint)coi.getOptionid()).Add((uint)coi.getType()).Add(coi.getValueStr()));
                            }
                            break;

                        case ConfigType.Int:
                        case ConfigType.Choice:
                            {
                                msgQueue.Add(new Message(cmd).Add((uint)coi.getOptionid()).Add((uint)coi.getType()).Add(coi.getValueInt()));
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private void restartButton_Click(object sender, EventArgs e)
        {
            restartButton.Enabled = false;
            Command cmd = Command.System | Command.Quit;
            msgQueue.Add(new Message(cmd));
        }

        private void ConfigDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == KeyShortcuts.Config) return;
            KeyDownForward?.Invoke(sender, e);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            closeMutex.WaitOne();
            Close();
            closeMutex.ReleaseMutex();
        }

        private void ConfigDialog_Load(object sender, EventArgs e)
        {
            Command cmd = Command.Config | Command.GetOptions;
            msgQueue.Add(new Message(cmd));
        }

        #endregion Event handlers
    }
}
