using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Threading;



using System.Windows.Markup;
using System.Windows.Data;

namespace WpfAppComboBox
{
    [ValueConversion(typeof(Enum), typeof(string))]
    public class EnumConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "???";

            return GetDescription((Enum)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public static string GetDescription(Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return en.ToString();
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class ValueDescription
    {
        [Description("Value")]
        public object Value;
        [Description("Description")]
        public object Description;
    }
    public static class EnumHelper
    {

        public static string Description(this Enum value)
        {
            var attributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Any())
                return (attributes.First() as DescriptionAttribute).Description;

            // If no description is found, the least we can do is replace underscores with spaces
            // You can add your own custom default formatting logic here
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            return ti.ToTitleCase(ti.ToLower(value.ToString().Replace("_", " ")));
        }

        public static IEnumerable<ValueDescription> GetAllValuesAndDescriptions(Type t)
        {
            if (!t.IsEnum)
                throw new ArgumentException($"{nameof(t)} must be an enum type");

            return Enum.GetValues(t).Cast<Enum>().Select((e) => new ValueDescription() { Value = e, Description = e.Description() }).ToList();
        }
    }

    [ValueConversion(typeof(Enum), typeof(IEnumerable<ValueDescription>))]
    public class EnumToCollectionConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return EnumHelper.GetAllValuesAndDescriptions(value.GetType());
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    internal class ViewModel
    {
        public enum ConnectionMode
        {
            [Description("ConnectOverSerial")]
            ConnectOverSerial,
            [Description("ReversClientOverIP")]
            ReversClientOverIP,
            [Description("ClientOverIP")]
            ClientOverIP
        }

        private ConnectionMode _mode = ConnectionMode.ReversClientOverIP;

        public IEnumerable<ConnectionMode> Connections => (IEnumerable<ConnectionMode>)Enum.GetValues(typeof(ConnectionMode));

        public ConnectionMode Connection
        {
            get
            {
                return _mode;
            }
            set
            {
                if (value != _mode)
                {
                    _mode = value;
                }
            }
        }

        public bool IsDisconnected => true;
        public string Title => "WPF Combobox Test";

        private readonly IEnumerable<string> _enumString = new List<string>() { "1", "2", "3" };

        public IEnumerable<string> Range => _enumString;

        private string _text = "1";
        public string CurrentText { get => _text; set { _text = value; } }



    }
}
