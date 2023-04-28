using ColorControl.Common;
using ColorControl.Forms;
using ColorControl.Native;
using ColorControl.Services.AMD;
using ColorControl.Services.Common;
using ColorControl.Services.LG;
using ColorControl.Services.NVIDIA;
using NLog;
using nspector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorControl.Services.GameLauncher
{
    public partial class GamePanel : UserControl, IModulePanel
    {
        public static readonly int SHORTCUTID_GAMEQA = -203;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Config _config;
        private GameService _gameService;
        private IntPtr _mainHandle;
        private NotifyIcon _trayIcon;
        private NvService _nvService;
        private AmdService _amdService;
        private LgService _lgService;

        internal GamePanel(GameService gameService, NvService nvService, AmdService amdService, LgService lgService, NotifyIcon trayIcon, IntPtr handle)
        {
            _gameService = gameService;
            _trayIcon = trayIcon;
            _mainHandle = handle;
            _nvService = nvService;
            _amdService = amdService;
            _lgService = lgService;

            _config = AppContext.CurrentContext.Config;

            InitializeComponent();

            FillGamePresets();

            FormUtils.InitSortState(lvGamePresets, _config.GamePresetsSortState);
        }

        private void FillGamePresets()
        {
            FormUtils.InitListView(lvGamePresets, GamePreset.GetColumnNames());

            foreach (var preset in _gameService.GetPresets())
            {
                AddOrUpdateItemGame(preset);
                Utils.RegisterShortcut(_mainHandle, preset.id, preset.shortcut);
            }
        }

        private void AddOrUpdateItemGame(GamePreset preset = null)
        {
            FormUtils.AddOrUpdateListItem(lvGamePresets, _gameService.GetPresets(), _config, preset);
        }

        internal void Save()
        {
            FormUtils.SaveSortState(lvGamePresets.ListViewItemSorter, _config.GamePresetsSortState);
        }

        private void edtShortcut_KeyDown(object sender, KeyEventArgs e)
        {
            ((TextBox)sender).Text = Utils.FormatKeyboardShortcut(e);
        }

        private void edtShortcut_KeyUp(object sender, KeyEventArgs e)
        {
            Utils.HandleKeyboardShortcutUp(e);
        }

        private GamePreset GetSelectedGamePreset()
        {
            if (lvGamePresets.SelectedItems.Count > 0)
            {
                var item = lvGamePresets.SelectedItems[0];
                return (GamePreset)item.Tag;
            }
            else
            {
                return null;
            }
        }

        public void UpdateInfo()
        {
            if (cbxGameStepType.Items.Count == 0)
            {
                cbxGameStepType.Items.AddRange(Utils.GetDescriptions<GameStepType>().ToArray());
                cbxGameStepType.SelectedIndex = 0;
            }
        }

        private async Task ApplySelectedGamePreset()
        {
            var preset = GetSelectedGamePreset();
            await ApplyGamePreset(preset);
        }

        internal async Task<bool> ApplyGamePreset(GamePreset preset)
        {
            if (preset == null || _gameService == null)
            {
                return false;
            }
            try
            {
                var result = await _gameService.ApplyPreset(preset, Program.AppContext);
                if (!result)
                {
                    throw new Exception("Error while applying Game-preset. At least one setting could not be applied. Check the log for details.");
                }

                return true;
            }
            catch (Exception e)
            {
                MessageForms.ErrorOk($"Error applying Game-preset ({e.TargetSite.Name}): {e.Message}");
                return false;
            }
        }

        private void lvGamePresets_SelectedIndexChanged(object sender, EventArgs _)
        {
            var preset = GetSelectedGamePreset();
            var enabled = preset != null;

            FormUtils.EnableControls(this, enabled, new List<Control> { lvGamePresets, btnGameAdd });

            if (preset != null)
            {
                edtGameName.Text = preset.name;
                edtGamePath.Text = preset.Path;
                edtGameParameters.Text = preset.Parameters;
                chkGameRunAsAdmin.Checked = preset.RunAsAdministrator;
                chkGameQuickAccess.Checked = preset.ShowInQuickAccess;
                ShowGameSteps();
            }
            else
            {
                edtGameName.Text = string.Empty;
                edtGamePath.Text = string.Empty;
                edtGameParameters.Text = string.Empty;
                chkGameRunAsAdmin.Checked = false;
                chkGameQuickAccess.Checked = false;
                edtGamePrelaunchSteps.Text = string.Empty;
            }
        }

        private async void lvGamePresets_DoubleClick(object sender, EventArgs e)
        {
            await ApplySelectedGamePreset();
        }

        private async void btnGameLaunch_Click(object sender, EventArgs e)
        {
            await ApplySelectedGamePreset();
        }

        private void btnGameClone_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedGamePreset();

            var newPreset = preset.Clone();

            AddOrUpdateItemGame(newPreset);
        }

        private void btnGameAdd_Click(object sender, EventArgs e)
        {
            var file = Utils.SelectFile();

            AddGamePreset(file);
        }

        private void AddGamePreset(FileInfo file)
        {
            var preset = _gameService.CreateNewPreset();

            if (file != null)
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(file.FullName);

                preset.Path = file.FullName;

                var fileName = Path.GetFileNameWithoutExtension(preset.Path);

                preset.name = !string.IsNullOrEmpty(versionInfo.FileDescription) ? versionInfo.FileDescription :
                    !string.IsNullOrEmpty(versionInfo.ProductName) ? versionInfo.ProductName : fileName;
            }

            AddOrUpdateItemGame(preset);
        }

        private void btnGameSave_Click(object sender, EventArgs e)
        {
            SaveGamePreset();
        }

        private void SaveGamePreset()
        {
            var shortcut = string.Empty;
            if (!Utils.ValidateShortcut(shortcut))
            {
                return;
            }

            var preset = GetSelectedGamePreset();

            var name = edtGameName.Text.Trim();

            if (name.Length == 0 || _gameService.GetPresets().Any(x => x.id != preset.id && x.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                MessageForms.WarningOk("The name can not be empty and must be unique.");
                return;
            }

            preset.name = name;
            preset.Path = edtGamePath.Text;
            preset.Parameters = edtGameParameters.Text;
            preset.RunAsAdministrator = chkGameRunAsAdmin.Checked;
            preset.ShowInQuickAccess = chkGameQuickAccess.Checked;

            var clear = !string.IsNullOrEmpty(preset.shortcut);

            var shortcutChanged = !shortcut.Equals(preset.shortcut);
            if (shortcutChanged)
            {
                preset.shortcut = shortcut;
            }

            var text = edtGamePrelaunchSteps.Text;

            var stepsList = (GameStepType)cbxGameStepType.SelectedIndex switch
            {
                GameStepType.PreLaunch => preset.PreLaunchSteps,
                GameStepType.PostLaunch => preset.PostLaunchSteps,
                GameStepType.Finalize => preset.FinalizeSteps,
                _ => throw new InvalidOperationException("Invalid game step type")
            };

            Utils.ParseWords(stepsList, text);

            AddOrUpdateItemGame();

            if (shortcutChanged)
            {
                Utils.RegisterShortcut(_mainHandle, preset.id, preset.shortcut, clear);
            }
        }

        private void btnGameBrowse_Click(object sender, EventArgs e)
        {
            var file = Utils.SelectFile();

            if (file == null)
            {
                return;
            }

            var preset = GetSelectedGamePreset();

            preset.Path = file.FullName;

            edtGamePath.Text = preset.Path;

            if (string.IsNullOrEmpty(preset.name) || preset.name.StartsWith("New preset", StringComparison.OrdinalIgnoreCase))
            {
                var fileName = Path.GetFileNameWithoutExtension(preset.Path);

                edtGameName.Text = fileName;
                preset.name = fileName;
            }

            AddOrUpdateItemGame();
        }

        private void btnGameDelete_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedGamePreset();

            if (preset == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(preset.shortcut))
            {
                WinApi.UnregisterHotKey(_mainHandle, preset.id);
            }
            _gameService.GetPresets().Remove(preset);

            var item = lvGamePresets.SelectedItems[0];
            lvGamePresets.Items.Remove(item);
        }

        private void mnuGameAddStep_Opening(object sender, CancelEventArgs e)
        {
            FormUtils.BuildServicePresetsMenu(mnuGameNvidiaPresets, _nvService, "NVIDIA", miGameAddPreset_Click);
            FormUtils.BuildServicePresetsMenu(mnuGameAmdPresets, _amdService, "AMD", miGameAddPreset_Click);
            FormUtils.BuildServicePresetsMenu(mnuGameLgPresets, _lgService, "LG", miGameAddPreset_Click);
        }

        private void miGameAddPreset_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            var preset = item.Tag as PresetBase;

            var text = $"{preset.GetType().Name}({preset.name})";

            FormUtils.AddStepToTextBox(edtGamePrelaunchSteps, text);
        }

        private void btnGameAddStep_Click(object sender, EventArgs e)
        {
            mnuGameAddStep.Show(btnGameAddStep, btnGameAddStep.PointToClient(Cursor.Position));
        }

        private void mnuGameStartProgram_Click(object sender, EventArgs e)
        {
            var file = Utils.SelectFile();

            if (file != null)
            {
                FormUtils.AddStepToTextBox(edtGamePrelaunchSteps, $"StartProgram({file.FullName})");
            }
        }

        private void lvGamePresets_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var file = new FileInfo(files.First());

                AddGamePreset(file);
            }
        }

        private void lvGamePresets_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void btnGameActions_Click(object sender, EventArgs e)
        {
            mnuGameActions.Show(btnGameSettings, btnGameSettings.PointToClient(Cursor.Position));
        }

        private void mnuGameNvInspector_Click(object sender, EventArgs e)
        {
            var form = new frmDrvSettings();
            form.Show();
        }

        private void miGameSetQuickAccessShortcut_Click(object sender, EventArgs e)
        {
            var shortcut = FormUtils.EditShortcut(_config.GameQuickAccessShortcut);

            if (shortcut == null)
            {
                return;
            }

            var clear = !string.IsNullOrEmpty(_config.GameQuickAccessShortcut);

            _config.GameQuickAccessShortcut = shortcut;

            Utils.RegisterShortcut(_mainHandle, SHORTCUTID_GAMEQA, _config.GameQuickAccessShortcut, clear);
        }

        private void mnuGameActions_Opening(object sender, CancelEventArgs e)
        {
            mnuGameNvInspector.Visible = _nvService != null;
        }

        private void lvGamePresets_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            FormUtils.ListViewItemChecked<GamePreset>(lvGamePresets, e);
        }

        private void btnGameProcessAffinity_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedGamePreset();

            if (preset == null)
            {
                return;
            }

            var numberOfProcessors = Environment.ProcessorCount;

            var options = Enumerable.Range(0, numberOfProcessors).Select(i => $"CPU #{i}");

            var values = MessageForms.ShowDialog("Set processor affinity", new[] {
                    new MessageForms.FieldDefinition
                    {
                        Label = "Set desired processors which are allowed to run program",
                        FieldType = MessageForms.FieldType.Flags,
                        Values = options,
                        Value = preset.ProcessAffinityMask
                    }
                });

            if (!values.Any())
            {
                return;
            }

            var value = values.First().Value;

            preset.ProcessAffinityMask = (uint)(int)value;
        }

        private void btnGameOptions_Click(object sender, EventArgs e)
        {
            mnuGameOptions.Show(btnGameOptions, btnGameOptions.PointToClient(Cursor.Position));
        }

        private void miGameProcessPriority_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedGamePreset();

            if (preset == null)
            {
                return;
            }

            var dropDownValues = Utils.GetDescriptions<GamePriorityClass>();

            var values = MessageForms.ShowDialog("Set process priority", new[] {
                    new MessageForms.FieldDefinition
                    {
                        Label = "Set desired process priority",
                        FieldType = MessageForms.FieldType.DropDown,
                        Values = dropDownValues,
                        Value = preset.ProcessPriorityClass > 0 ? ((GamePriorityClass)preset.ProcessPriorityClass).GetDescription() : GamePriorityClass.Normal.GetDescription()
                    }
                });

            if (!values.Any())
            {
                return;
            }

            var value = values.First().Value.ToString();
            var enumName = Utils.GetEnumNameByDescription(typeof(GamePriorityClass), value);
            if (enumName != null)
            {
                value = enumName;
            }

            var enumValue = Enum.Parse(typeof(GamePriorityClass), value);

            preset.ProcessPriorityClass = (uint)(int)enumValue;
        }

        private void cbxGameStepType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowGameSteps();
        }

        private void ShowGameSteps()
        {
            var preset = GetSelectedGamePreset();

            if (preset == null)
            {
                return;
            }

            switch ((GameStepType)cbxGameStepType.SelectedIndex)
            {
                case GameStepType.PreLaunch:
                    edtGamePrelaunchSteps.Text = string.Join(", ", preset.PreLaunchSteps);
                    break;
                case GameStepType.PostLaunch:
                    edtGamePrelaunchSteps.Text = string.Join(", ", preset.PostLaunchSteps);
                    break;
                case GameStepType.Finalize:
                    edtGamePrelaunchSteps.Text = string.Join(", ", preset.FinalizeSteps);
                    break;
            }
        }

        private void edtGamePrelaunchSteps_Leave(object sender, EventArgs e)
        {
            SaveGamePreset();
        }

        private void lvLgPresets_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            FormUtils.ListViewSort(sender, e);
        }
    }
}
