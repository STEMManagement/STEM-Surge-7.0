using System;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Linq;
using System.Windows.Forms;
using STEM.Sys.Serialization;

namespace STEM.Surge.ControlPanel
{
    public partial class DictionaryEditorForm : Form
    {
        IDictionaryPropertyGridAdapter _DictionaryPropertyGridAdapter;

        System.ComponentModel.PropertyDescriptor _Descriptor;
        object _BoundObject;

        public bool PropertyValueChanged { get; private set; }

        public DictionaryEditorForm(System.ComponentModel.PropertyDescriptor descriptor, object boundObject)
        {
            InitializeComponent();

            PropertyValueChanged = false;

            _Descriptor = descriptor;
            _BoundObject = boundObject;

            this.Text = _Descriptor.DisplayName;

            Type[] argTypes = _Descriptor.PropertyType.GetGenericArguments();

            _DictionaryPropertyGridAdapter = Activator.CreateInstance(typeof(DictionaryPropertyGridAdapter<,>).MakeGenericType(argTypes[0], argTypes[1]), _Descriptor.GetValue(_BoundObject)) as IDictionaryPropertyGridAdapter;

            propertyGrid1.SelectedObject = _DictionaryPropertyGridAdapter;
            propertyGrid1.SelectedGridItemChanged += new SelectedGridItemChangedEventHandler(propertyGrid1_SelectedGridItemChanged);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (Updated())
            {
                if (MessageBox.Show(this, "Cancel Changes?", "Unsaved", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnFormClosing(e);
        }

        void propertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            if (e.NewSelection.PropertyDescriptor == null)
                return;

            if (_DictionaryPropertyGridAdapter.GetValues().Contains(e.NewSelection.Value))
                textBox1.Text = propertyGrid1.SelectedGridItem.Label;

            if (e.NewSelection.Parent != null && e.NewSelection.Parent.Value != null && _DictionaryPropertyGridAdapter.GetValues().Contains(e.NewSelection.Parent.Value))
            {
                textBox1.Text = e.NewSelection.Parent.Label;

                if (e.NewSelection.PropertyDescriptor.PropertyType == typeof(System.Collections.Generic.List<string>))
                {
                    StringCollectionEditor strings = new StringCollectionEditor(e.NewSelection.PropertyDescriptor, e.NewSelection.Parent.Value);
                    strings.ShowDialog(this);

                    if (strings.PropertyValueChanged)
                        PropertyValueChanged = true;

                    return;
                }

                if (e.NewSelection.PropertyDescriptor.PropertyType.ToString().Contains("STEM.Sys.Serialization.Dictionary"))
                {
                    DictionaryEditorForm dict = new DictionaryEditorForm(e.NewSelection.PropertyDescriptor, e.NewSelection.Parent.Value);
                    dict.ShowDialog(this);

                    if (dict.PropertyValueChanged)
                        PropertyValueChanged = true;

                    return;
                }
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            try
            {
                _DictionaryPropertyGridAdapter.Add(textBox1.Text.Trim());
                propertyGrid1.SelectedObject = _DictionaryPropertyGridAdapter;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            try
            {
                _DictionaryPropertyGridAdapter.Remove(textBox1.Text.Trim());
                propertyGrid1.SelectedObject = _DictionaryPropertyGridAdapter;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ReKey_Click(object sender, EventArgs e)
        {
            try
            {
                _DictionaryPropertyGridAdapter.ReKey(propertyGrid1.SelectedGridItem.Label, textBox1.Text.Trim());
                propertyGrid1.SelectedObject = _DictionaryPropertyGridAdapter;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void apply_Click(object sender, EventArgs e)
        {
            if (Updated())
            {
                PropertyValueChanged = true;
                _Descriptor.SetValue(_BoundObject, _DictionaryPropertyGridAdapter.GetDictionary());
            }

            Close();
        }

        bool Updated()
        {
            return _DictionaryPropertyGridAdapter.HasChanges(_Descriptor.GetValue(_BoundObject));
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            Type[] argTypes = _Descriptor.PropertyType.GetGenericArguments();
            _DictionaryPropertyGridAdapter = Activator.CreateInstance(typeof(DictionaryPropertyGridAdapter<,>).MakeGenericType(argTypes[0], argTypes[1]), _Descriptor.GetValue(_BoundObject)) as IDictionaryPropertyGridAdapter;
            propertyGrid1.SelectedObject = _DictionaryPropertyGridAdapter;

            Close();
        }
    }

    public interface IDictionaryPropertyGridAdapter
    {
        object GetDictionary();
        System.Collections.Generic.List<object> Getkeys();
        System.Collections.Generic.List<object> GetValues();
        void Add(string key);
        void Remove(string key);
        void ReKey(string oldkey, string newkey);
        bool HasChanges(object origDict);
    }

    public class DictionaryPropertyGridAdapter<TKey, TValue> : ICustomTypeDescriptor, IDictionaryPropertyGridAdapter
    {
        Dictionary<TKey, TValue> _Dictionary;

        public DictionaryPropertyGridAdapter(Dictionary<TKey, TValue> d)
        {
            _Dictionary = new Dictionary<TKey, TValue>();

            foreach (TKey k in d.Keys)
                _Dictionary[k] = d[k];
        }

        public object GetDictionary()
        {
            return _Dictionary;
        }

        public System.Collections.Generic.List<object> Getkeys()
        {
            return _Dictionary.Keys.Select(i => i as object).ToList();
        }

        public System.Collections.Generic.List<object> GetValues()
        {
            return _Dictionary.Values.Select(i => i as object).ToList();
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return _Dictionary;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(Array.Empty<Attribute>());
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            System.Collections.Generic.List<DictionaryPropertyDescriptor<TKey, TValue>> properties = new System.Collections.Generic.List<DictionaryPropertyDescriptor<TKey, TValue>>();
            foreach (TKey key in _Dictionary.Keys)
            {
                properties.Add(new DictionaryPropertyDescriptor<TKey, TValue>(_Dictionary, key));
            }

            return new PropertyDescriptorCollection(properties.ToArray());
        }

        public void Add(string key)
        {
            object value = null;
            try
            {
                value = Activator.CreateInstance(typeof(TValue));
            }
            catch { }

            if (value == null && typeof(TValue) == typeof(string))
            {
                value = "value";
            }

            if (value == null)
            {
                throw new Exception("The type of the 'value' has no parameterless constructor.");
            }

            _Dictionary[(TKey)Convert.ChangeType(key, typeof(TKey), System.Globalization.CultureInfo.CurrentCulture)] = (TValue)value;
        }

        public void Remove(string key)
        {
            _Dictionary.Remove((TKey)Convert.ChangeType(key, typeof(TKey), System.Globalization.CultureInfo.CurrentCulture));
        }

        public void ReKey(string oldkey, string newkey)
        {
            object value = _Dictionary[(TKey)Convert.ChangeType(oldkey, typeof(TKey), System.Globalization.CultureInfo.CurrentCulture)];
            _Dictionary[(TKey)Convert.ChangeType(newkey, typeof(TKey), System.Globalization.CultureInfo.CurrentCulture)] = (TValue)value;
            _Dictionary.Remove((TKey)Convert.ChangeType(oldkey, typeof(TKey), System.Globalization.CultureInfo.CurrentCulture));
        }

        public bool HasChanges(object origDict)
        {
            Dictionary<TKey, TValue> orig = (Dictionary<TKey, TValue>)origDict;

            if (orig.Keys.Count != _Dictionary.Keys.Count)
                return true;

            if (orig.Keys.Except(_Dictionary.Keys).Count() > 0)
                return true;

            // Same key count and key values, check values

            foreach (TKey k in orig.Keys)
                if (!orig[k].Equals(_Dictionary[k]))
                    return true;

            return false;
        }
    }

    public class DictionaryPropertyDescriptor<TKey, TValue> : PropertyDescriptor
    {
        Dictionary<TKey, TValue> _dictionary;
        TKey _key;

        internal DictionaryPropertyDescriptor(Dictionary<TKey, TValue> d, TKey key)
            : base(key.ToString(), null)
        {
            _dictionary = d;
            _key = key;
        }

        public override Type PropertyType
        {
            get { return _dictionary[_key].GetType(); }
        }

        public override void SetValue(object component, object value)
        {
            _dictionary[_key] = (TValue)value;
        }

        public override object GetValue(object component)
        {
            return _dictionary[_key];
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type ComponentType
        {
            get { return null; }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
