using HandyControl.Controls;
using HandyControl.Data;

namespace HandyDoc
{
    public class AppConfig : GlobalDataHelper<AppConfig>
    {
        public string Lang { get; set; } = "English";
        public SkinType Skin { get; set; } = SkinType.Default;
    }
}
