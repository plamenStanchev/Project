namespace Scheduler.Web.ViewModels.Settings
{
    using Scheduler.Data.Models;

    public class SettingViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string NameAndValue { get; set; }

    }
}
