using ColorControl.Services.AMD;
using ColorControl.Services.Common;
using ColorControl.Services.LG;
using ColorControl.Services.NVIDIA;
using ColorControl.Services.Samsung;
using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Forms;
using ColorControl.Shared.Native;
using ColorControl.Shared.Services;
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

        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Config _config;
        private GameService _gameService;
        private NotifyIcon _trayIcon;
        private readonly AppContextProvider _appContextProvider;
        private NvService _nvService;
        private AmdService _amdService;
        private LgService _lgService;
        private SamsungService _samsungService;

        internal GamePanel(GameService gameService, NvService nvService, AmdService amdService, LgService lgService, SamsungService samsungService, NotifyIcon trayIcon, AppContextProvider appContextProvider)
        {
            _gameService = gameService;
            _trayIcon = trayIcon;
            _appContextProvider = appContextProvider;
            _nvService = nvService;
            _amdService = amdService;
            _lgService = lgService;
            _samsungService = samsungService;
            _config = Shared.Common.GlobalContext.CurrentContext.Config;

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
                KeyboardShortcutManager.RegisterShortcut(preset.id, preset.shortcut);
            }
        }

        private void AddOrUpdateItemGame(GamePreset preset = null)
        {
            ServiceFormUtils.AddOrUpdateListItem(lvGamePresets, _gameService.GetPresets(), _config, preset);
        }

        internal void Save()
        {
            FormUtils.SaveSortState(lvGamePresets.ListViewItemSorter, _config.GamePresetsSortState);
        }

        private void edtShortcut_KeyDown(object sender, KeyEventArgs e)
        {
            ((TextBox)sender).Text = KeyboardShortcutManager.FormatKeyboardShortcut(e);
        }

        private void edtShortcut_KeyUp(object sender, KeyEventArgs e)
        {
            KeyboardShortcutManager.HandleKeyboardShortcutUp(e);
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
            await _gameService.ApplyPresetUi(preset);
        }

        private void lvGamePresets_SelectedIndexChanged(object sender, EventArgs _)
        {
            var preset = GetSelectedGamePreset();
            var enabled = preset != null;

            FormUtils.EnableControls(this, enabled, new List<Control> { lvGamePresets, btnGameAdd, btnGameSettings });

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
            if (!KeyboardShortcutManager.ValidateShortcut(shortcut))
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

            preset.shortcut = shortcut;

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

            KeyboardShortcutManager.RegisterShortcut(preset.id, preset.shortcut);
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
                WinApi.UnregisterHotKey(_appContextProvider.GetAppContext().MainHandle, preset.id);
            }
            _gameService.GetPresets().Remove(preset);

            var item = lvGamePresets.SelectedItems[0];
            lvGamePresets.Items.Remove(item);
        }

        private void mnuGameAddStep_Opening(object sender, CancelEventArgs e)
        {
            ServiceFormUtils.BuildServicePresetsMenu(mnuGameNvidiaPresets, _nvService, "NVIDIA", miGameAddPreset_Click);
            ServiceFormUtils.BuildServicePresetsMenu(mnuGameAmdPresets, _amdService, "AMD", miGameAddPreset_Click);
            ServiceFormUtils.BuildServicePresetsMenu(mnuGameLgPresets, _lgService, "LG", miGameAddPreset_Click);
            ServiceFormUtils.BuildServicePresetsMenu(mnuGameSamsungPresets, _samsungService, "Samsung", miGameAddPreset_Click);
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
            mnuGameAddStep.ShowCustom(btnGameAddStep);
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
            mnuGameActions.ShowCustom(btnGameSettings);
        }

        private void mnuGameNvInspector_Click(object sender, EventArgs e)
        {
            var form = new frmDrvSettings();
            form.Show();
        }

        private void miGameSetQuickAccessShortcut_Click(object sender, EventArgs e)
        {
            EditGameSettings();
        }

        private void EditGameSettings()
        {
            var quickAccessField = new FieldDefinition
            {
                Label = "Quick Access shortcut",
                FieldType = FieldType.Shortcut,
                Value = _config.GameQuickAccessShortcut
            };

            var applyExternallyLaunched = new FieldDefinition
            {
                Label = "Apply presets when launching games externally",
                SubLabel = "This will monitor processes and automatically apply the preset whenever a known game is launched",
                FieldType = FieldType.CheckBox,
                Value = _gameService.Config.ApplyExternallyLaunched
            };

            var values = MessageForms.ShowDialog("Game launcher settings", new[] {
                quickAccessField,
                applyExternallyLaunched
            });
            if (!values.Any())
            {
                return;
            }

            var shortcut = (string)quickAccessField.Value;

            _config.GameQuickAccessShortcut = shortcut;

            KeyboardShortcutManager.RegisterShortcut(SHORTCUTID_GAMEQA, _config.GameQuickAccessShortcut);

            _gameService.Config.ApplyExternallyLaunched = applyExternallyLaunched.ValueAsBool;

            UpdateInfo();
        }

        private void mnuGameActions_Opening(object sender, CancelEventArgs e)
        {
            mnuGameNvInspector.Visible = _nvService != null;
        }

        private void lvGamePresets_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ServiceFormUtils.ListViewItemChecked<GamePreset>(lvGamePresets, e);
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
                    new FieldDefinition
                    {
                        Label = "Set desired processors which are allowed to run program",
                        FieldType = FieldType.Flags,
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
            mnuGameOptions.ShowCustom(btnGameOptions);
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
                    new FieldDefinition
                    {
                        Label = "Set desired process priority",
                        FieldType = FieldType.DropDown,
                        Values = dropDownValues,
                        Value = preset.ProcessPriorityClass > 0 ? ((GamePriorityClass)preset.ProcessPriorityClass).GetDescription() : GamePriorityClass.Normal.GetDescription()
                    }
                });

            if (!values.Any())
            {
                return;
            }

            var value = values.First().Value.ToString();
            var enumValue = Utils.GetEnumValueByDescription<GamePriorityClass>(value);

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

        private void miPresetNVIDIAProfileInspector_Click(object sender, EventArgs e)
        {
            var form = new frmDrvSettings();
            form.Show();

            var preset = GetSelectedGamePreset();

            var executable = Path.GetFileName(preset.Path);

            form.SetProfileByExecutable(executable);
        }

        private void miGameOptionsAutoApplySettings_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedGamePreset();

            var allowAutoApplyField = new FieldDefinition
            {
                Label = "Allow this preset to be automatically executed when the process starts",
                FieldType = FieldType.CheckBox,
                Value = preset.AutoSettings.AllowAutoApply
            };

            var processActions = new[] { ProcessAutoAction.None, ProcessAutoAction.Suspend }.Select(t => Utils.GetDescription(t));

            var processActionField = new FieldDefinition
            {
                Label = "Optional action to perform directly after the process starts",
                SubLabel = "Temporarily suspending the process may help when enabling/disabling HDR or other settings",
                FieldType = FieldType.DropDown,
                Values = processActions,
                Value = preset.AutoSettings.ProcessAutoAction
            };

            var values = MessageForms.ShowDialog("Auto apply settings", new[] {
                allowAutoApplyField,
                processActionField
            });
            if (!values.Any())
            {
                return;
            }

            preset.AutoSettings.AllowAutoApply = allowAutoApplyField.ValueAsBool;
            preset.AutoSettings.ProcessAutoAction = processActionField.ValueAsEnum<ProcessAutoAction>();
        }
    }
}
