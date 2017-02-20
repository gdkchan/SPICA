using SPICA.WinForms.Properties;

using System;
using System.Windows.Forms;

namespace SPICA.WinForms
{
    static class SettingsManager
    {
        public static void BindBool(
            string PropertyName,
            Action OnSettingChange,
            ToolStripMenuItem Item, 
            ToolStripButton TBtn = null)
        {
            bool CurrentValue = (bool)Settings.Default[PropertyName];
            bool DefaultValue = Item.Checked;

            EventHandler Handler = (sender, e) =>
            {
                bool Value = !(bool)Settings.Default[PropertyName];

                Settings.Default[PropertyName] = Item.Checked = Value;

                if (TBtn != null) TBtn.Checked = Value;

                OnSettingChange();
            };

            Item.Checked = CurrentValue;
            Item.Click += Handler;

            if (TBtn != null)
            {
                TBtn.Checked = CurrentValue;
                TBtn.Click += Handler;
            }

            if (CurrentValue != DefaultValue) OnSettingChange();
        }
    }
}
